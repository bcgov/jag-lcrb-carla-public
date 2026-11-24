import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { SessionProvider } from '@core/session/session-provider';
import { getSubmittedApplication } from '@data-access/repositories/applications/mock/submitted-applications';
import { ApplicationSubmittedPage } from './application-submitted.page';

const settle = async (f: { detectChanges(): void; whenStable(): Promise<unknown> }) => {
  for (let i = 0; i < 3; i++) {
    f.detectChanges();
    await f.whenStable();
    await new Promise((r) => setTimeout(r, 0));
  }
  f.detectChanges();
};
async function mount(id: string) {
  TestBed.configureTestingModule({ providers: [provideRouter([])] });
  TestBed.inject(SessionProvider).login({ role: 'licensee', firstName: 'Dana', lastName: 'Lee', organization: 'Cedar' } as never);
  const fixture = TestBed.createComponent(ApplicationSubmittedPage);
  fixture.componentRef.setInput('id', id);
  document.body.appendChild(fixture.nativeElement);
  await settle(fixture);
  return { fixture, el: fixture.nativeElement as HTMLElement };
}

describe('ApplicationSubmittedPage', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
  });
  afterEach(() => {
    vi.unstubAllGlobals();
    document.body.innerHTML = '';
  });

  it('renders the hero with the status badge and meta line, the file panel and the detail cards', async () => {
    const app = getSubmittedApplication('LP-125998')!;
    const { el } = await mount('LP-125998');
    expect(el.firstElementChild!.className).toBe('flex min-h-screen flex-col bg-background');
    expect(el.querySelector('main#main-content')!.className).toBe('flex-1');
    expect(el.querySelector('h1')!.textContent).toContain(app.type);
    const badge = el.querySelector('h1 + submitted-status-badge span, submitted-status-badge span')!;
    expect(badge.textContent!.trim()).toBe('Under Review');
    expect(el.textContent).toContain(`Job No: ${app.id} | Ref: ${app.reference}`);
    expect(el.querySelector('div.container.mx-auto.max-w-4xl.px-4.py-8.space-y-6')).toBeTruthy();
    expect(el.querySelector('submitted-file-panel')!.textContent).toContain('No longer proceeding?');
    expect(el.querySelector('submitted-file-panel case-thread section')).toBeTruthy();
    expect(el.textContent).toContain('Application details');
    expect(el.textContent).toContain(app.confirmationNumber);
    const dl = el.querySelector('dl')!;
    expect(dl.className).toBe('grid grid-cols-1 gap-4 sm:grid-cols-2');
    expect(dl.firstElementChild!.tagName).toBe('DIV');
    expect(dl.querySelector('dt')!.className).toBe('text-xs uppercase tracking-wide text-muted-foreground');
    expect(dl.querySelector('dd')!.className).toBe('mt-0.5 text-sm font-medium text-foreground');
    expect(el.textContent).toContain('What you submitted');
    expect(el.querySelectorAll('ol.space-y-4 > li').length).toBe(app.timeline.length);
    expect(el.textContent).not.toContain('Documents required');
  });

  it('renders the timeline states, payment summary and the receipt card', async () => {
    const app = getSubmittedApplication('LP-125998')!;
    const { el } = await mount('LP-125998');
    const first = el.querySelector('ol.space-y-4 > li > span')!;
    const state = app.timeline[0].state;
    expect(first.className).toContain(
      state === 'done' ? 'bg-green-100 text-green-700' : state === 'current' ? 'bg-blue-100 text-blue-700' : 'bg-gray-100 text-gray-400',
    );
    const total = app.fee.reduce((a, f) => a + f.amount, 0);
    const items = [...el.querySelectorAll('ul.divide-y > li')];
    expect(items.length).toBe(app.fee.length + 1);
    expect(items.at(-1)!.textContent!.replace(/\s+/g, ' ').trim()).toBe(`Total paid$${total.toLocaleString()} ${app.currency}`);
    expect(items.at(-1)!.className).toBe('flex items-center justify-between py-2 text-sm font-semibold');
    expect(el.textContent).toContain(`Paid with ${app.paymentMethod}`);
    expect(el.querySelector('application-receipt-card')).toBeTruthy();
  });

  it('shows the documents-required banner with the outstanding count for a pending file', async () => {
    const app = getSubmittedApplication('CM-125876')!;
    const outstanding = app.requiredDocuments.filter((d) => d.required).length;
    const { el } = await mount('CM-125876');
    const card = el.querySelector('[data-slot="card"]')!;
    expect(card.classList.contains('border-yellow-300')).toBe(true);
    expect(card.textContent).toContain('Documents required');
    expect(card.textContent).toContain(app.reviewNote!);
    const link = card.querySelector('a')!;
    expect(link.getAttribute('href')).toBe('/applications/submitted/CM-125876/documents');
    expect(link.textContent!.replace(/\s+/g, ' ').trim()).toBe(`Upload documents${outstanding > 0 ? ` (${outstanding})` : ''}`);
    expect(el.querySelector('submitted-status-badge span')!.textContent!.trim()).toBe('Pending Documents');
  });

  it('shows a closed conversation and no withdraw card for an approved file', async () => {
    const { el } = await mount('CAT-125654');
    expect(el.querySelector('submitted-status-badge span')!.textContent!.trim()).toBe('Approved');
    expect(el.querySelector('submitted-file-panel')!.textContent).not.toContain('No longer proceeding?');
    expect(el.querySelector('case-thread h2 + span')!.textContent!.trim()).toBe('Closed');
  });
});
