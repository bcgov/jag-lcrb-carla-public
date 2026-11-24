import { TestBed } from '@angular/core/testing';
import { Router, provideRouter } from '@angular/router';
import { jsPDF } from 'jspdf';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { SessionProvider } from '@core/session/session-provider';
import { saveDraft } from '@data-access/repositories/applications/drafts';
import { SEP_RECORDS } from '@data-access/repositories/special-events/mock/seps';
import { SpecialEventsPage } from './special-events.page';

const settle = async (f: { detectChanges(): void; whenStable(): Promise<unknown> }) => {
  for (let i = 0; i < 3; i++) {
    f.detectChanges();
    await f.whenStable();
    await new Promise((r) => setTimeout(r, 0));
  }
  f.detectChanges();
};

// jsPDF's initialized hook swaps save() for a no-op so no PDF is written.
const noSave = [
  'initialized',
  function (this: { save: (name: string) => void }) {
    this.save = () => undefined;
  },
];
const pdfEvents = () => (jsPDF.API as unknown as { events: unknown[] }).events;

async function mount(role: 'applicant' | 'licensee' = 'applicant') {
  TestBed.configureTestingModule({ providers: [provideRouter([])] });
  TestBed.inject(SessionProvider).login({ role, firstName: 'Alex', lastName: 'Rivera', organization: 'Society' } as never);
  const fixture = TestBed.createComponent(SpecialEventsPage);
  document.body.appendChild(fixture.nativeElement);
  await settle(fixture);
  return { fixture, el: fixture.nativeElement as HTMLElement };
}
const tab = (el: HTMLElement, text: string) => [...el.querySelectorAll('button[role="tab"]')].find((b) => b.textContent!.includes(text)) as HTMLButtonElement;
const tile = (el: HTMLElement, label: string) => [...el.querySelectorAll('ui-stat-tile button, ui-stat-tile a')].find((b) => b.textContent!.includes(label)) as HTMLElement;
const rows = (el: HTMLElement) => el.querySelectorAll('[role="tabpanel"]:not([hidden]) tbody tr');
const item = (text: string) => [...document.querySelectorAll('[data-slot="dropdown-menu-item"]')].find((i) => i.textContent!.trim() === text) as HTMLElement | undefined;

describe('SpecialEventsPage', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
    pdfEvents().push(noSave);
  });
  afterEach(() => {
    pdfEvents().splice(pdfEvents().indexOf(noSave), 1);
    vi.unstubAllGlobals();
    document.body.innerHTML = '';
  });

  it('shows the SEP sign-in for a non-applicant', async () => {
    const { el } = await mount('licensee');
    expect(el.querySelector('h1')!.textContent).toContain('Sign in to apply for a Special Event Permit');
    expect(el.querySelector('special-events-page [role="tab"]')).toBeNull();
  });

  it('renders the hero, tiles, tabs, the permits table and the resources sidebar', async () => {
    const { el } = await mount();
    expect(el.querySelector('h1')!.textContent).toContain('Special Event Permits');
    expect(el.textContent).toContain('Apply for and manage permits for events where liquor will be served');
    expect(el.querySelector('main a[href="/special-events/apply"]')!.textContent).toContain('Start New Application');
    const active = SEP_RECORDS.filter((r) => r.status !== 'cancelled');
    const approved = active.filter((r) => r.status === 'approved' || r.status === 'auto-approved').length;
    const pending = active.filter((r) => ['submitted', 'under-review', 'changes-requested'].includes(r.status)).length;
    expect(tile(el, 'Approved').textContent).toContain(String(approved));
    expect(tile(el, 'Pending').textContent).toContain(String(pending));
    expect(tile(el, 'Upcoming events').getAttribute('href')).toBe('/calendar');
    expect(tab(el, 'Submitted').textContent).toContain(`Submitted (${SEP_RECORDS.length})`);
    expect(tab(el, 'Drafts').textContent).toContain('Drafts (0)');
    expect(rows(el).length).toBe(SEP_RECORDS.length);
    expect(rows(el)[0].textContent).toContain('SEP-2026-04812');
    for (const t of ['Application Checklist', 'Get Certified', 'Plan Your Drinks', 'Servings Calculator', 'Learn More', 'Submit Early']) {
      expect(el.textContent).toContain(t);
    }
    expect(el.querySelectorAll('a[href="/special-events/servings-calculator"]').length).toBe(2);
  });

  it('filters by search text and by the Pending tile, and shows the empty state', async () => {
    const { fixture, el } = await mount();
    const input = el.querySelector('input[placeholder="Search applications..."]') as HTMLInputElement;
    input.value = 'sep-2026-04812';
    input.dispatchEvent(new Event('input'));
    await settle(fixture);
    expect(rows(el).length).toBe(1);
    expect(rows(el)[0].textContent).toContain('Harbour City Music Festival');
    input.value = 'zzz';
    input.dispatchEvent(new Event('input'));
    await settle(fixture);
    expect(rows(el)[0].textContent).toContain('No applications match your search.');
    input.value = '';
    input.dispatchEvent(new Event('input'));
    tile(el, 'Pending').click();
    await settle(fixture);
    expect(tile(el, 'Pending').getAttribute('aria-pressed')).toBe('true');
    const pending = SEP_RECORDS.filter((r) => ['submitted', 'under-review', 'changes-requested'].includes(r.status));
    expect(rows(el).length).toBe(pending.length);
  });

  it('lists saved drafts, switches tabs from the Drafts tile and deletes a draft', async () => {
    saveDraft({ draftId: 'draft:sep', flowId: 'sep', title: 'SEP', resumeHref: '/special-events/apply', formData: { eventName: 'Spring Fair' }, currentStep: 1, progress: 40 } as never);
    const { fixture, el } = await mount();
    expect(tab(el, 'Drafts').textContent).toContain('Drafts (1)');
    tile(el, 'Drafts').click();
    await settle(fixture);
    expect(tab(el, 'Drafts').getAttribute('aria-selected')).toBe('true');
    expect(rows(el)[0].textContent).toContain('Spring Fair');
    expect(rows(el)[0].textContent).toContain('Location not set');
    expect(rows(el)[0].textContent).toContain('40%');
    (rows(el)[0].querySelector('button[data-slot="button"]') as HTMLButtonElement).click();
    await settle(fixture);
    expect(item('Continue Editing')!.getAttribute('href')).toBe('/special-events/apply');
    item('Delete Draft')!.click();
    await settle(fixture);
    expect(tab(el, 'Drafts').textContent).toContain('Drafts (0)');
    expect(rows(el)[0].textContent).toContain('No saved drafts.');
  });

  it('offers permit download for approved rows and navigates from the row menu', async () => {
    const { fixture, el } = await mount();
    const idx = SEP_RECORDS.findIndex((r) => r.id === 'SEP-2026-04791');
    (rows(el)[idx].querySelector('button[data-slot="button"]') as HTMLButtonElement).click();
    await settle(fixture);
    expect([...document.querySelectorAll('[data-slot="dropdown-menu-item"]')].map((i) => i.textContent!.trim())).toEqual([
      'View Details',
      'Download Permit',
      'Copy for New Event',
    ]);
    item('Download Permit')!.click();
    await settle(fixture);
    const navigate = vi.spyOn(TestBed.inject(Router), 'navigateByUrl').mockResolvedValue(true);
    (rows(el)[idx].querySelector('button[data-slot="button"]') as HTMLButtonElement).click();
    await settle(fixture);
    item('View Details')!.click();
    await settle(fixture);
    expect(navigate).toHaveBeenCalledWith('/special-events/SEP-2026-04791');
  });

  it('offers Cancel Permit only for pending rows and cancels the permit through the dialog', async () => {
    const { fixture, el } = await mount();
    const idx = SEP_RECORDS.findIndex((r) => r.id === 'SEP-2026-04812');
    (rows(el)[idx].querySelector('button[data-slot="button"]') as HTMLButtonElement).click();
    await settle(fixture);
    expect(item('Cancel Permit')!.className).toContain('text-red-600');
    item('Cancel Permit')!.click();
    await settle(fixture);
    const dialog = document.querySelector('ui-dialog-content') as HTMLElement;
    expect(dialog.querySelector('h2')!.textContent).toBe('Cancel this permit?');
    const radio = dialog.querySelector('input[type="radio"]') as HTMLInputElement;
    radio.checked = true;
    radio.dispatchEvent(new Event('change'));
    await settle(fixture);
    (document.querySelector('[role="checkbox"]') as HTMLButtonElement).click();
    await settle(fixture);
    const confirm = [...dialog.querySelectorAll('button')].find((b) => b.textContent!.trim() === 'Cancel permit') as HTMLButtonElement;
    confirm.click();
    await settle(fixture);
    expect(JSON.parse(localStorage.getItem('lcrb.sep.records') ?? '[]').find((r: { id: string }) => r.id === 'SEP-2026-04812')?.status).toBe('cancelled');
    expect(document.querySelector('ui-dialog-content')).toBeNull();
  });
});
