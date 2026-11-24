import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { SessionProvider } from '@core/session/session-provider';
import { getSubmittedApplication } from '@data-access/repositories/applications/mock/submitted-applications';
import { stubFetch } from '../../../../../tests/support/fetch-stub';
import { ApplicationDocumentsPage } from './application-documents.page';

const settle = async (f: { detectChanges(): void; whenStable(): Promise<unknown> }) => {
  for (let i = 0; i < 3; i++) {
    f.detectChanges();
    await f.whenStable();
    await new Promise((r) => setTimeout(r, 0));
  }
  f.detectChanges();
};
async function mount(id: string, loggedIn = true) {
  TestBed.configureTestingModule({ providers: [provideRouter([])] });
  if (loggedIn) TestBed.inject(SessionProvider).login({ role: 'licensee', firstName: 'Dana', lastName: 'Lee', organization: 'Cedar' } as never);
  const fixture = TestBed.createComponent(ApplicationDocumentsPage);
  fixture.componentRef.setInput('id', id);
  document.body.appendChild(fixture.nativeElement);
  await settle(fixture);
  return { fixture, el: fixture.nativeElement as HTMLElement };
}

describe('ApplicationDocumentsPage', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
  });
  afterEach(() => {
    vi.unstubAllGlobals();
    vi.useRealTimers();
    document.body.innerHTML = '';
  });

  it('lists each requested document with its required badge, reason and an uploader', async () => {
    const app = getSubmittedApplication('CM-125876')!;
    const { el } = await mount('CM-125876');
    expect(el.firstElementChild!.className).toBe('flex min-h-screen flex-col bg-background');
    expect(el.querySelector('h1')!.textContent).toContain('Required documents');
    expect(el.textContent).toContain(`${app.type} · Job No: ${app.id}`);
    expect(el.querySelector('div.container.mx-auto.max-w-3xl.px-4.py-8.space-y-6')).toBeTruthy();
    expect(el.textContent).toContain(app.reviewNote!);
    const n = app.requiredDocuments.length;
    expect(el.textContent).toContain(`LCRB has requested the following ${n} document${n === 1 ? '' : 's'}. Upload each item below.`);
    expect(el.querySelectorAll('document-manager').length).toBe(n);
    const first = app.requiredDocuments[0];
    expect(el.textContent).toContain(first.name);
    expect(el.textContent).toContain(first.reason);
    const badge = [...el.querySelectorAll('[data-slot="badge"]')][0];
    expect(badge.textContent!.trim()).toBe(first.required ? 'Required' : 'Optional');
    expect(el.textContent).toContain('Once all required documents are uploaded and pass the virus scan');
    expect(el.querySelectorAll('[data-slot="card"]')[0].classList.contains('border-yellow-300')).toBe(true);
  });

  it('uploads against `<job>:<document>` as the signed-in user, and falls back to "Applicant"', async () => {
    const api = stubFetch({ '/api/upload': { json: { url: 'https://blob.test/x.pdf' } } });
    const app = getSubmittedApplication('CM-125876')!;
    const doc = app.requiredDocuments[0];
    const { fixture, el } = await mount('CM-125876');
    vi.useFakeTimers({ toFake: ['setTimeout'] });
    const input = el.querySelector('document-manager input[type="file"]') as HTMLInputElement;
    Object.defineProperty(input, 'files', { value: [new File(['%PDF'], 'x.pdf', { type: 'application/pdf' })], configurable: true });
    input.dispatchEvent(new Event('change'));
    for (let i = 0; i < 3; i++) {
      await vi.advanceTimersByTimeAsync(5);
      fixture.detectChanges();
    }
    expect((api.callsTo('/api/upload')[0].body as FormData).get('subjectId')).toBe(`${app.id}:${doc.id}`);
    const stored = Object.values(JSON.parse(localStorage.getItem('lcrb.v2.documents')!)) as { uploadedBy: string; category: string }[];
    expect(stored[0]).toMatchObject({ uploadedBy: 'Dana Lee', category: doc.name });
    vi.useRealTimers();
    TestBed.resetTestingModule();
    document.body.innerHTML = '';
    localStorage.clear();
    const anon = await mount('CM-125876', false);
    expect(anon.el.querySelector('document-manager')).toBeTruthy();
  });

  it('shows the empty state when no documents are requested', async () => {
    const { el } = await mount('LP-125998');
    expect(el.textContent).toContain('No documents required');
    expect(el.textContent).toContain("There are no outstanding document requests for this application. We'll let you know if we need anything further.");
    expect(el.querySelector('document-manager')).toBeNull();
    expect(el.querySelector('[data-slot="card-content"]')!.className).toBe('flex flex-col items-center gap-3 p-12 text-center');
  });
});
