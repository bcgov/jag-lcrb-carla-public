import { TestBed } from '@angular/core/testing';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { stubFetch } from '../../../../../tests/support/fetch-stub';
import { UploadPage } from './upload.page';

async function mount() {
  const fixture = TestBed.createComponent(UploadPage);
  const el = fixture.nativeElement as HTMLElement;
  fixture.detectChanges();
  await fixture.whenStable();
  const settle = async () => {
    for (let i = 0; i < 5; i++) {
      await new Promise((r) => setTimeout(r, 5));
      fixture.detectChanges();
      await fixture.whenStable();
    }
  };
  const pick = async (files: File[]) => {
    const input = el.querySelector('input[type="file"]') as HTMLInputElement;
    Object.defineProperty(input, 'files', { value: files, configurable: true });
    input.dispatchEvent(new Event('change'));
    await settle();
  };
  return { fixture, el, pick, settle };
}
const pdf = (name: string) => new File(['%PDF'], name, { type: 'application/pdf' });

describe('UploadPage', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
  });
  afterEach(() => vi.unstubAllGlobals());

  it('renders the drop zone with the prototype classes', async () => {
    const { el } = await mount();
    expect(el.textContent).toContain('Upload Background Documents');
    expect(el.textContent).toContain('Drop PDF files here or click to browse');
    const zone = el.querySelector('input[type="file"]')!.parentElement!;
    expect(zone.className.split(/\s+/).join(' ').trim()).toBe(
      'relative flex min-h-48 cursor-pointer flex-col items-center justify-center rounded-lg border-2 border-dashed p-8 transition-colors border-muted-foreground/25 hover:border-primary/50',
    );
    expect(el.querySelector('input[type="file"]')!.className).toBe('absolute inset-0 cursor-pointer opacity-0');
  });

  it('rejects a non-PDF with the prototype error and does not call the API', async () => {
    const api = stubFetch({});
    const { el, pick } = await mount();
    await pick([new File(['x'], 'a.txt', { type: 'text/plain' })]);
    expect(el.textContent).toContain('Please select PDF files only');
    expect(api.calls).toHaveLength(0);
  });

  it('posts each PDF as FormData file to /api/upload and lists the results', async () => {
    const api = stubFetch({
      '/api/upload': { json: { url: 'u', pathname: 'documents/test.pdf', filename: 'test.pdf', size: 2048 } },
    });
    const { el, pick } = await mount();
    await pick([pdf('a.pdf'), pdf('b.pdf')]);
    const posts = api.callsTo('/api/upload');
    expect(posts).toHaveLength(2);
    expect(posts[0].body).toBeInstanceOf(FormData);
    expect(((posts[0].body as FormData).get('file') as File).name).toBe('a.pdf');
    expect(((posts[1].body as FormData).get('file') as File).name).toBe('b.pdf');
    expect(el.textContent).toContain('Uploaded Documents (2)');
    expect(el.textContent).toContain('2.0 KB');
  });

  it('shows the failure message when an upload fails', async () => {
    stubFetch({ '/api/upload': { status: 500, json: {} } });
    const { el, pick } = await mount();
    await pick([pdf('bad.pdf')]);
    expect(el.textContent).toContain('Failed to upload bad.pdf');
    expect(el.textContent).not.toContain('Uploaded Documents');
  });

  it('toggles the drag-over style', async () => {
    const { fixture, el } = await mount();
    const zone = el.querySelector('input[type="file"]')!.parentElement!;
    zone.dispatchEvent(new Event('dragover', { cancelable: true }));
    fixture.detectChanges();
    expect(zone.className).toContain('border-primary bg-primary/5');
    zone.dispatchEvent(new Event('dragleave', { cancelable: true }));
    fixture.detectChanges();
    expect(zone.className).not.toContain('bg-primary/5');
  });
});
