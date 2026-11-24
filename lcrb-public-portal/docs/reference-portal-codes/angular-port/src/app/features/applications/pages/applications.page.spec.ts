import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { SessionProvider } from '@core/session/session-provider';
import { MY_APPLICATIONS } from '@data-access/repositories/applications/mock/my-applications';
import { cancelApplication } from '@data-access/repositories/applications/cancellation-service';
import { ApplicationsPage } from './applications.page';

const settle = async (f: { detectChanges(): void; whenStable(): Promise<unknown> }) => {
  for (let i = 0; i < 3; i++) {
    f.detectChanges();
    await f.whenStable();
    await new Promise((r) => setTimeout(r, 0));
  }
  f.detectChanges();
};
async function mount() {
  TestBed.configureTestingModule({ providers: [provideRouter([])] });
  TestBed.inject(SessionProvider).login({ role: 'licensee', firstName: 'Dana', lastName: 'Lee', organization: 'Cedar' } as never);
  const fixture = TestBed.createComponent(ApplicationsPage);
  document.body.appendChild(fixture.nativeElement);
  await settle(fixture);
  return { fixture, el: fixture.nativeElement as HTMLElement };
}
const tab = (el: HTMLElement, text: string) => [...el.querySelectorAll('button[role="tab"]')].find((b) => b.textContent!.includes(text)) as HTMLButtonElement;
const tile = (el: HTMLElement, label: string) => [...el.querySelectorAll('ui-stat-tile button')].find((b) => b.textContent!.includes(label)) as HTMLButtonElement;
const menu = () => document.querySelector('[data-slot="dropdown-menu-content"]') as HTMLElement | null;
const item = (text: string) => [...document.querySelectorAll('[data-slot="dropdown-menu-item"]')].find((i) => i.textContent!.trim() === text) as HTMLElement;
const openActions = async (f: Parameters<typeof settle>[0], el: HTMLElement, row = 0) => {
  (el.querySelectorAll('[role="tabpanel"]:not([hidden]) tbody tr')[row].querySelector('button[data-slot="button"]') as HTMLButtonElement).click();
  await settle(f);
};
const cancellations = () => JSON.parse(localStorage.getItem('lcrb.v2.application-cancellations') ?? '{}') as Record<string, unknown>;

describe('ApplicationsPage', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
  });
  afterEach(() => {
    vi.unstubAllGlobals();
    vi.useRealTimers();
    document.body.innerHTML = '';
  });

  it('renders the shell, hero actions, stat tiles and the in-progress table', async () => {
    const { el } = await mount();
    expect(el.firstElementChild!.className).toBe('min-h-screen flex flex-col bg-background');
    expect(el.querySelector('main')!.className).toBe('flex-1');
    expect(el.querySelector('h1')!.textContent).toContain('My Applications');
    expect(el.textContent).toContain('Track and manage your licence and authorization applications');
    const actions = [...el.querySelectorAll('[heroactions] a, a[href="/receipts"], a[href="/applications/new"]')].map((a) => a.getAttribute('href'));
    expect(actions).toEqual(['/receipts', '/applications/new']);
    expect(el.querySelector('div.container.mx-auto.px-4.py-8')).toBeTruthy();
    const { inProgress, submitted, completed } = MY_APPLICATIONS;
    expect(tile(el, 'In Progress').textContent).toContain(String(inProgress.length));
    expect(tile(el, 'Under Review').textContent).toContain(String(submitted.length));
    expect(tile(el, 'Completed').textContent).toContain(String(completed.length));
    expect(tile(el, 'In Progress').getAttribute('aria-pressed')).toBe('true');
    expect(tab(el, 'In Progress').textContent).toContain(`In Progress (${inProgress.length})`);
    expect(tab(el, 'In Progress').getAttribute('aria-selected')).toBe('true');
    const rows = el.querySelectorAll('[role="tabpanel"]:not([hidden]) tbody tr');
    expect(rows.length).toBe(inProgress.length);
    expect(rows[0].textContent).toContain(`Job No: ${inProgress[0].id} | Ref: ${inProgress[0].reference}`);
    expect((rows[0].querySelector('.bg-bc-blue.rounded-full') as HTMLElement).style.width).toBe(`${inProgress[0].completionPercent}%`);
    for (const c of ['bg-gray-100', 'text-gray-800', 'border-gray-200', 'flex', 'items-center', 'gap-1']) expect(rows[0].querySelector('[data-slot="badge"]')!.classList.contains(c)).toBe(true);
    expect(el.textContent).toContain('Please note');
    expect(el.querySelector('footer')).toBeTruthy();
  });

  it('opens the row actions menu with the prototype items and links', async () => {
    const { fixture, el } = await mount();
    await openActions(fixture, el);
    expect(menu()).not.toBeNull();
    const items = [...document.querySelectorAll('[data-slot="dropdown-menu-item"]')].map((i) => i.textContent!.trim());
    expect(items).toEqual(['Complete Application', 'View Details', 'Cancel Application']);
    const first = MY_APPLICATIONS.inProgress[0];
    expect(item('Complete Application').getAttribute('href')).toBe('/applications/food-primary');
    expect(item('Cancel Application').className).toContain('text-red-600');
    expect(first.id).toBe('FP-126117');
  });

  it('keeps the query string of the resume link of a type-scoped draft', async () => {
    const { fixture, el } = await mount();
    const idx = MY_APPLICATIONS.inProgress.findIndex((a) => a.id === 'AGT-126082');
    await openActions(fixture, el, idx);
    expect(item('Complete Application').getAttribute('href')).toBe('/applications/new?type=agent');
  });

  it('View Details opens the draft summary dialog and Close dismisses it', async () => {
    const { fixture, el } = await mount();
    await openActions(fixture, el);
    item('View Details').click();
    await settle(fixture);
    expect(menu()).toBeNull();
    const dialog = document.querySelector('ui-dialog-content')!;
    expect(dialog.querySelector('h2')!.textContent).toBe(MY_APPLICATIONS.inProgress[0].type);
    expect(dialog.querySelector('a')!.getAttribute('href')).toBe('/applications/food-primary');
    ([...dialog.querySelectorAll('button')].find((b) => b.textContent!.trim() === 'Close') as HTMLButtonElement).click();
    await settle(fixture);
    expect(document.querySelector('ui-dialog-content')).toBeNull();
  });

  it('cancelling an in-progress draft removes the row and updates the counts', async () => {
    const { fixture, el } = await mount();
    const before = MY_APPLICATIONS.inProgress.length;
    await openActions(fixture, el);
    item('Cancel Application').click();
    await settle(fixture);
    expect(menu()).toBeNull();
    expect(el.querySelectorAll('[role="tabpanel"]:not([hidden]) tbody tr').length).toBe(before - 1);
    expect(tab(el, 'In Progress').textContent).toContain(`In Progress (${before - 1})`);
    expect(tile(el, 'In Progress').textContent).toContain(String(before - 1));
  });

  it('shows the submitted tab with its links; Cancel Application opens the cancel dialog', async () => {
    const { fixture, el } = await mount();
    tab(el, 'Submitted').click();
    await settle(fixture);
    expect(tab(el, 'Submitted').getAttribute('aria-selected')).toBe('true');
    expect(tile(el, 'In Progress').getAttribute('aria-pressed')).toBe('false');
    expect(tile(el, 'Under Review').getAttribute('aria-pressed')).toBe('true');
    const rows = el.querySelectorAll('[role="tabpanel"]:not([hidden]) tbody tr');
    expect(rows.length).toBe(MY_APPLICATIONS.submitted.length);
    await openActions(fixture, el);
    const id = MY_APPLICATIONS.submitted[0].id;
    expect(item('View Application').getAttribute('href')).toBe(`/applications/submitted/${id}`);
    expect(item('Upload Documents').getAttribute('href')).toBe(`/applications/submitted/${id}/documents`);
    expect(item('Message LCRB').getAttribute('href')).toBe(`/applications/submitted/${id}#conversation`);
    item('Cancel Application').click();
    await settle(fixture);
    expect(menu()).toBeNull();
    expect(document.querySelector('[role="dialog"]')?.textContent).toContain('Cancel this application?');
    expect(Object.keys(cancellations())).toHaveLength(0);
  });

  it('the Action Required tile filters the submitted tab and "Show all submitted" clears it', async () => {
    const { fixture, el } = await mount();
    tile(el, 'Action Required').click();
    await settle(fixture);
    const pending = MY_APPLICATIONS.submitted.filter((a) => a.status === 'pending_documents');
    expect(tile(el, 'Action Required').getAttribute('aria-pressed')).toBe('true');
    expect(tile(el, 'Under Review').getAttribute('aria-pressed')).toBe('false');
    const note = el.querySelector('[role="status"]')!;
    expect(note.className).toBe('mb-3 flex flex-wrap items-center justify-between gap-2 rounded-md border border-yellow-200 bg-yellow-50 px-3 py-2 text-sm text-yellow-900');
    expect(note.textContent).toContain(`Showing only applications that need something from you (${pending.length}).`);
    expect(el.querySelectorAll('[role="tabpanel"]:not([hidden]) tbody tr').length).toBe(pending.length);
    (note.querySelector('button') as HTMLButtonElement).click();
    await settle(fixture);
    expect(el.querySelector('[role="status"]')).toBeNull();
    expect(el.querySelectorAll('[role="tabpanel"]:not([hidden]) tbody tr').length).toBe(MY_APPLICATIONS.submitted.length);
  });

  it('moves a withdrawn submission to Completed as Cancelled', async () => {
    cancelApplication({ jobNumber: 'LP-125998', applicationType: 'Liquor Primary', applicant: 'a', actor: 'x', reason: 'r', priorStatus: 'under_review', referredTo: [] });
    const { fixture, el } = await mount();
    const { submitted, completed } = MY_APPLICATIONS;
    expect(tab(el, 'Submitted').textContent).toContain(`Submitted (${submitted.length - 1})`);
    expect(tab(el, 'Completed').textContent).toContain(`Completed (${completed.length + 1})`);
    tab(el, 'Completed').click();
    await settle(fixture);
    const first = el.querySelector('[role="tabpanel"]:not([hidden]) tbody tr')!;
    expect(first.textContent).toContain('Cancelled');
    for (const c of ['bg-zinc-100', 'text-zinc-600', 'border-zinc-200']) expect(first.querySelector('[data-slot="badge"]')!.classList.contains(c)).toBe(true);
  });

  it('keeps the search and type filter inert: they never filter the lists', async () => {
    const { fixture, el } = await mount();
    const rows = () => el.querySelectorAll('[role="tabpanel"]:not([hidden]) tbody tr').length;
    const before = rows();
    const input = el.querySelector('input[placeholder^="Search by application type"]') as HTMLInputElement;
    expect(input.className).toContain('pl-9');
    input.value = 'zzz-no-match';
    input.dispatchEvent(new Event('input'));
    await settle(fixture);
    expect(rows()).toBe(before);
    expect(el.querySelector('[data-slot="select-trigger"]')!.textContent).toContain('All Application Types');
  });
});
