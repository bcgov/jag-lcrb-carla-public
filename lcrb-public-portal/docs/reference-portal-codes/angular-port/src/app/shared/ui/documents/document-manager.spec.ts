import { TestBed } from '@angular/core/testing';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { completeScan, uploadDocument } from '@data-access/repositories/documents/document-service';
import { stubFetch } from '../../../../../tests/support/fetch-stub';
import { DocumentManager } from './document-manager';

async function mount(inputs: Record<string, unknown> = {}) {
  const fixture = TestBed.createComponent(DocumentManager);
  fixture.componentRef.setInput('subjectId', 'LP-1:doc-1');
  fixture.componentRef.setInput('actor', 'Dana Lee');
  for (const [k, v] of Object.entries(inputs)) fixture.componentRef.setInput(k, v);
  const el = fixture.nativeElement as HTMLElement;
  const settle = async (ms = 5) => {
    for (let i = 0; i < 3; i++) {
      await vi.advanceTimersByTimeAsync(ms);
      fixture.detectChanges();
      await fixture.whenStable();
    }
  };
  fixture.detectChanges();
  await settle();
  const pick = async (files: File[]) => {
    const input = el.querySelector('input[type="file"]') as HTMLInputElement;
    Object.defineProperty(input, 'files', { value: files, configurable: true });
    input.dispatchEvent(new Event('change'));
    await settle();
  };
  return { fixture, el, settle, pick };
}
const pdf = (name = 'plan.pdf', size?: number) => {
  const f = new File(['%PDF'], name, { type: 'application/pdf' });
  if (size) Object.defineProperty(f, 'size', { value: size });
  return f;
};
const stored = () => JSON.parse(localStorage.getItem('lcrb.v2.documents') ?? '{}') as Record<string, { name: string; scanState: string; subjectId: string; blobUrl?: string; category?: string }>;

describe('DocumentManager', () => {
  beforeEach(() => {
    vi.useFakeTimers();
    localStorage.clear();
    sessionStorage.clear();
  });
  afterEach(() => {
    vi.useRealTimers();
    vi.unstubAllGlobals();
  });

  it('renders the uploader, hint and the empty state with the prototype classes', async () => {
    const { el } = await mount({ className: 'mt-2' });
    expect(el.firstElementChild!.className).toBe('space-y-4 mt-2');
    const zone = el.querySelector('input[type="file"]')!.parentElement!;
    expect(zone.className).toBe(
      'flex flex-col items-center justify-center gap-2 rounded-lg border-2 border-dashed px-6 py-8 text-center transition-colors border-border bg-muted/30',
    );
    expect(zone.textContent).toContain('Drag and drop files here, or browse');
    expect(zone.textContent).toContain('PDF, image, or Word document · up to 10.0 MB');
    const input = el.querySelector('input[type="file"]') as HTMLInputElement;
    expect(input.className).toBe('sr-only');
    expect(input.getAttribute('accept')).toBe('.pdf,.png,.jpg,.jpeg,.doc,.docx');
    expect(input.multiple).toBe(true);
    expect(el.textContent).toContain('No documents uploaded yet.');
    expect(el.querySelector('ul')).toBeNull();
  });

  it('highlights the drop zone while dragging and clears on leave', async () => {
    const { fixture, el } = await mount();
    const zone = el.querySelector('input[type="file"]')!.parentElement!;
    zone.dispatchEvent(new Event('dragover', { cancelable: true }));
    fixture.detectChanges();
    expect(zone.className).toContain('border-[#013366] bg-[#013366]/5');
    zone.dispatchEvent(new Event('dragleave'));
    fixture.detectChanges();
    expect(zone.className).toContain('border-border bg-muted/30');
  });

  it('uploads through /api/upload, lists a Scanning badge, then Clean after 1.5 s', async () => {
    const api = stubFetch({ '/api/upload': { json: { url: 'https://blob.test/plan.pdf' } } });
    const { el, pick, settle } = await mount({ category: 'Site plan' });
    await pick([pdf()]);
    const posts = api.callsTo('/api/upload');
    expect(posts).toHaveLength(1);
    const form = posts[0].body as FormData;
    expect((form.get('file') as File).name).toBe('plan.pdf');
    expect(form.get('subjectId')).toBe('LP-1:doc-1');

    const docs = Object.values(stored());
    expect(docs).toHaveLength(1);
    expect(docs[0]).toMatchObject({ name: 'plan.pdf', subjectId: 'LP-1:doc-1', blobUrl: 'https://blob.test/plan.pdf', category: 'Site plan', scanState: 'pending' });
    expect(el.querySelector('ul')!.className).toBe('divide-y rounded-lg border');
    expect(el.querySelector('ul li')!.textContent).toContain('plan.pdf');
    expect(el.querySelector('ul li')!.textContent).toContain('Scanning');
    expect(el.querySelector('ul li')!.textContent).toContain('Dana Lee');
    const download = [...el.querySelectorAll('li button')].find((b) => b.textContent!.includes('Download')) as HTMLButtonElement;
    expect(download.disabled).toBe(true);

    await vi.advanceTimersByTimeAsync(1400);
    expect(Object.values(stored())[0].scanState).toBe('pending');
    await vi.advanceTimersByTimeAsync(100);
    await settle();
    expect(Object.values(stored())[0].scanState).toBe('clean');
    const badge = el.querySelector('li span.rounded-full')!;
    expect(badge.textContent!.trim()).toBe('Clean');
    expect(badge.className).toBe('flex items-center gap-1 rounded-full bg-emerald-100 px-2 py-0.5 text-xs font-medium text-emerald-800');
    expect(download.disabled).toBe(false);
  });

  it('opens the blob for a clean document and removes it on request', async () => {
    const open = vi.fn();
    vi.stubGlobal('open', open);
    const doc = uploadDocument({ subjectId: 'LP-1:doc-1', name: 'a.pdf', contentType: 'application/pdf', size: 2048, uploadedBy: 'Dana Lee', blobUrl: 'https://b.test/a.pdf' });
    completeScan(doc.id, 'clean');
    const { el, settle } = await mount();
    const download = [...el.querySelectorAll('li button')].find((b) => b.textContent!.includes('Download')) as HTMLButtonElement;
    download.click();
    expect(open).toHaveBeenCalledWith('https://b.test/a.pdf', '_blank', 'noopener,noreferrer');
    (el.querySelector('button[aria-label="Remove a.pdf"]') as HTMLButtonElement).click();
    await settle();
    expect(el.textContent).toContain('No documents uploaded yet.');
  });

  it('rejects an unsupported type or oversize file without calling the API', async () => {
    const api = stubFetch({});
    const { el, pick } = await mount();
    await pick([new File(['x'], 'a.txt', { type: 'text/plain' })]);
    expect(el.querySelector('[role="alert"]')!.textContent).toContain('Unsupported file type.');
    expect(el.querySelector('[role="alert"]')!.className).toBe('text-sm text-destructive');
    await pick([pdf('big.pdf', 11 * 1024 * 1024)]);
    expect(el.querySelector('[role="alert"]')!.textContent).toContain('File exceeds the 10 MB limit.');
    expect(api.calls).toHaveLength(0);
  });

  it('shows the failure message when the upload fails', async () => {
    stubFetch({ '/api/upload': { status: 500, json: {} } });
    const { el, pick } = await mount();
    await pick([pdf()]);
    expect(el.querySelector('[role="alert"]')!.textContent).toContain('Something went wrong uploading that file. Please try again.');
    expect(Object.keys(stored())).toHaveLength(0);
  });

  it('hides the uploader and remove buttons when read only', async () => {
    const { el } = await mount({ readOnly: true });
    expect(el.querySelector('input[type="file"]')).toBeNull();
    expect(el.textContent).toContain('No documents uploaded yet.');
  });
});
