/** The map renders for real (container size stubbed like map-experience.spec.ts); PDF save() is stubbed. */
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { jsPDF } from 'jspdf';
import { afterEach, beforeEach, describe, expect, it } from 'vitest';
import { SessionProvider } from '@core/session/session-provider';
import { SEP_RECORDS } from '@data-access/repositories/special-events/mock/seps';
import { PoliceEventsPage } from './police-events.page';

const settle = async (f: { detectChanges(): void; whenStable(): Promise<unknown> }) => {
  for (let i = 0; i < 3; i++) {
    f.detectChanges();
    await f.whenStable();
    await new Promise((r) => setTimeout(r, 0));
  }
  f.detectChanges();
};

const saved: string[] = [];
const noSave = [
  'initialized',
  function (this: { save: (name: string) => void }) {
    this.save = (name: string) => void saved.push(name);
  },
];
const pdfEvents = () => (jsPDF.API as unknown as { events: unknown[] }).events;

async function mount() {
  TestBed.configureTestingModule({ providers: [provideRouter([])] });
  TestBed.inject(SessionProvider).login({
    role: 'police',
    firstName: 'Dana',
    lastName: 'Okafor',
    organization: 'Victoria PD',
    email: 'officer@police.example',
  } as never);
  const fixture = TestBed.createComponent(PoliceEventsPage);
  document.body.appendChild(fixture.nativeElement);
  await settle(fixture);
  return { fixture, el: fixture.nativeElement as HTMLElement };
}
const cards = (el: HTMLElement) => [...el.querySelectorAll('div.grid.gap-3.md\\:grid-cols-2 > div[data-slot="card"]')];
const summary = (el: HTMLElement) => el.querySelector('p.mb-4')!.textContent!.trim();
const type = (input: HTMLInputElement, value: string) => {
  input.value = value;
  input.dispatchEvent(new Event('input'));
};
const option = (text: string) => [...document.querySelectorAll('[role="option"]')].find((o) => o.textContent!.trim() === text) as HTMLElement;

describe('PoliceEventsPage', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
    saved.length = 0;
    pdfEvents().push(noSave);
    Object.defineProperty(HTMLElement.prototype, 'offsetWidth', { configurable: true, get: () => 1200 });
    Object.defineProperty(HTMLElement.prototype, 'offsetHeight', { configurable: true, get: () => 640 });
  });
  afterEach(() => {
    delete (HTMLElement.prototype as unknown as Record<string, unknown>)['offsetWidth'];
    delete (HTMLElement.prototype as unknown as Record<string, unknown>)['offsetHeight'];
    pdfEvents().splice(pdfEvents().indexOf(noSave), 1);
    document.body.innerHTML = '';
    document.querySelectorAll('.cdk-overlay-container').forEach((c) => (c.innerHTML = ''));
  });

  it('renders the hero, the embedded map, the report card, the schedule card and the upcoming list', async () => {
    const { el } = await mount();
    expect(el.querySelector('h1')!.textContent).toBe('Upcoming Events');
    expect(el.querySelector('map-experience')).not.toBeNull();
    expect(el.querySelector('map-experience > div')!.classList.contains('h-[640px]')).toBe(true);
    expect(el.textContent).toContain('Daily permit report (PDF)');
    expect(el.textContent).toContain('Pick a day to export all its Special Event Permits.');
    expect(el.querySelector('scheduled-reports-card h2')!.textContent).toBe('Automatic report delivery');
    expect(summary(el)).toMatch(/^\d+ upcoming events?$/);
    expect(cards(el).length).toBeGreaterThan(0);
    expect(el.querySelector('a[href="/police/review/SEP-2026-04812"]')!.textContent).toContain('Harbour City Music Festival');
  });

  it('filters the list by search text and shows the empty state', async () => {
    const { fixture, el } = await mount();
    const before = cards(el).length;
    type(el.querySelector('input[placeholder^="Search events"]')!, 'harbour');
    await settle(fixture);
    expect(cards(el).length).toBeGreaterThan(0);
    expect(cards(el).length).toBeLessThan(before);
    type(el.querySelector('input[placeholder^="Search events"]')!, 'zzzz');
    await settle(fixture);
    expect(el.textContent).toContain('No upcoming events match your search.');
    expect(summary(el)).toBe('0 upcoming events');
  });

  it('filters by city', async () => {
    const { fixture, el } = await mount();
    (el.querySelectorAll('button[role="combobox"]')[1] as HTMLButtonElement).click();
    await settle(fixture);
    option('Nanaimo').click();
    await settle(fixture);
    expect(cards(el)).toHaveLength(1);
    expect(cards(el)[0].textContent).toContain('Harbour City Music Festival');
  });

  it('applies a date range, clamps from to today and clears it', async () => {
    const { fixture, el } = await mount();
    const to = el.querySelector('#to-date') as HTMLInputElement;
    expect(el.querySelector('#from-date')!.getAttribute('min')).toBe(new Date().toISOString().slice(0, 10));
    const target = SEP_RECORDS.find((r) => r.id === 'SEP-2026-04788')!.eventStart.slice(0, 10);
    type(to, target);
    await settle(fixture);
    expect(summary(el)).toMatch(/in selected range$/);
    expect(cards(el).every((c) => !c.textContent!.includes('Harbour City Music Festival'))).toBe(true);
    const clear = [...el.querySelectorAll('button')].find((b) => b.textContent!.trim() === 'Clear dates')!;
    clear.click();
    await settle(fixture);
    expect(summary(el)).not.toMatch(/in selected range/);
    expect([...el.querySelectorAll('button')].some((b) => b.textContent!.trim() === 'Clear dates')).toBe(false);
  });

  it('exports the daily report: disabled until a day is chosen, then saves one PDF', async () => {
    const { fixture, el } = await mount();
    const download = () => [...el.querySelectorAll('button')].find((b) => b.textContent!.includes('Download PDF')) as HTMLButtonElement;
    expect(download().disabled).toBe(true);
    const day = SEP_RECORDS.find((r) => r.id === 'SEP-2026-04788')!.eventStart.slice(0, 10);
    type(el.querySelector('#report-date')!, day);
    await settle(fixture);
    expect(el.textContent).toMatch(/\d+ permits? on this day/);
    expect(download().disabled).toBe(false);
    download().click();
    await settle(fixture);
    await new Promise((r) => setTimeout(r, 50));
    await settle(fixture);
    expect(saved).toHaveLength(1);
    expect(download().textContent).toContain('Download PDF');
  });
});
