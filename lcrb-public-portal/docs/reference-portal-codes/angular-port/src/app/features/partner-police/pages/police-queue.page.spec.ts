/** Filters, search, sorting and assignment. */
import { TestBed } from '@angular/core/testing';
import { ActivatedRoute, provideRouter } from '@angular/router';
import { afterEach, beforeEach, describe, expect, it } from 'vitest';
import { SessionProvider } from '@core/session/session-provider';
import { SepStoreProvider } from '@data-access/repositories/special-events/sep-store-provider';
import { PoliceQueuePage } from './police-queue.page';

const settle = async (f: { detectChanges(): void; whenStable(): Promise<unknown> }) => {
  for (let i = 0; i < 3; i++) {
    f.detectChanges();
    await f.whenStable();
    await new Promise((r) => setTimeout(r, 0));
  }
  f.detectChanges();
};

async function mount(filter?: string) {
  TestBed.configureTestingModule({
    providers: [provideRouter([]), { provide: ActivatedRoute, useValue: { snapshot: { queryParamMap: new Map(filter ? [['filter', filter]] : []) } } }],
  });
  TestBed.inject(SessionProvider).login({ role: 'police', firstName: 'Dana', lastName: 'Okafor', organization: 'Victoria PD' } as never);
  const fixture = TestBed.createComponent(PoliceQueuePage);
  document.body.appendChild(fixture.nativeElement);
  await settle(fixture);
  return { fixture, el: fixture.nativeElement as HTMLElement };
}
const names = (el: HTMLElement) => [...el.querySelectorAll('div[policeQueueCard] a.font-semibold')].map((a) => a.textContent!.trim());
const count = (el: HTMLElement) => el.querySelector('p.mb-4')!.textContent!.trim();
const option = (text: string) => [...document.querySelectorAll('[role="option"]')].find((o) => o.textContent!.trim() === text) as HTMLElement;
const trigger = (el: HTMLElement, i: number) => el.querySelectorAll('button[role="combobox"]')[i] as HTMLButtonElement;

describe('PoliceQueuePage', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
  });
  afterEach(() => {
    document.body.innerHTML = '';
    document.querySelectorAll('.cdk-overlay-container').forEach((c) => (c.innerHTML = ''));
  });

  it('renders the hero, the AI trigger and the default needs-review view sorted by event date', async () => {
    const { el } = await mount();
    expect(el.querySelector('h1')!.textContent).toBe('Review Queue');
    expect(el.textContent).toContain('AI: Suggest priorities');
    expect(count(el)).toBe('Showing 3 applications');
    expect(names(el)).toEqual(['Westshore Charity Gala', 'Harbour City Music Festival', 'Downtown Street Party']);
    expect(el.querySelector('div[policeQueueCard] a[href="/police/review/SEP-2026-04788"]')).not.toBeNull();
    expect(el.textContent).toContain('Requires review');
  });

  it('honours ?filter= as the initial view', async () => {
    const { el } = await mount('unassigned');
    expect(names(el)).toEqual(['Harbour City Music Festival']);
    expect(count(el)).toBe('Showing 1 application');
  });

  it('searches across name, applicant, venue, address, city and id, and shows the empty state', async () => {
    const { fixture, el } = await mount('all');
    expect(count(el)).toBe('Showing 9 applications');
    const input = el.querySelector('input') as HTMLInputElement;
    input.value = 'marcus';
    input.dispatchEvent(new Event('input'));
    await settle(fixture);
    expect(names(el)).toEqual(['Westshore Charity Gala']);
    input.value = 'zzzz';
    input.dispatchEvent(new Event('input'));
    await settle(fixture);
    expect(el.textContent).toContain('No applications match your search.');
  });

  it('changes the view with the select and the sort order', async () => {
    const { fixture, el } = await mount();
    trigger(el, 0).click();
    await settle(fixture);
    option('Approved').click();
    await settle(fixture);
    expect(count(el)).toBe('Showing 3 applications');
    expect(names(el)).not.toContain('Westshore Charity Gala');

    trigger(el, 1).click();
    await settle(fixture);
    option('Event name (A–Z)').click();
    await settle(fixture);
    const sorted = names(el);
    expect(sorted).toEqual([...sorted].sort((a, b) => a.localeCompare(b)));

    trigger(el, 1).click();
    await settle(fixture);
    option('Attendance (high–low)').click();
    await settle(fixture);
    expect(names(el)).toHaveLength(3);
  });

  it('assigns a submitted file to the signed-in officer', async () => {
    const { fixture, el } = await mount();
    const assign = [...el.querySelectorAll('button')].filter((b) => b.textContent!.trim() === 'Assign to me');
    expect(assign).toHaveLength(1);
    assign[0].click();
    await settle(fixture);
    const record = TestBed.inject(SepStoreProvider).getById('SEP-2026-04812')!;
    expect(record.assignedOfficer).toBe('Cst. Dana Okafor');
    expect(record.status).toBe('under-review');
    expect([...el.querySelectorAll('button')].some((b) => b.textContent!.trim() === 'Assign to me')).toBe(false);
  });
});
