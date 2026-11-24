import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { SepStoreProvider } from '@data-access/repositories/special-events/sep-store-provider';
import { LG_REVIEWER } from '@data-access/repositories/lg-review/mock/lg-applications';
import { SEP_RECORDS } from '@data-access/repositories/special-events/mock/seps';
import { sepOccursOnDate, todayIso } from '@domain/special-events/seps';
import { TodaysEventsCard } from './todays-events-card';

async function settle(fixture: { detectChanges(): void; whenStable(): Promise<unknown> }) {
  fixture.detectChanges();
  await fixture.whenStable();
  await new Promise((r) => setTimeout(r, 0));
  fixture.detectChanges();
}

describe('TodaysEventsCard', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
    TestBed.configureTestingModule({ providers: [provideRouter([])] });
  });
  afterEach(() => vi.useRealTimers());

  it('shows the empty state with the place label and the map link', async () => {
    // A date no seeded event occurs on.
    vi.useFakeTimers({ toFake: ['Date'], now: new Date('2031-03-04T12:00:00') });
    const fixture = TestBed.createComponent(TodaysEventsCard);
    await settle(fixture);
    const el = fixture.nativeElement as HTMLElement;
    const place = LG_REVIEWER.role.replace(/^City of\s+/i, '');
    expect(el.textContent).toContain('0');
    expect(el.textContent).toContain('special events today');
    expect(el.textContent).toContain(`No special events are scheduled in ${place} today.`);
    expect(el.querySelector('a')!.getAttribute('href')).toBe('/local-government/map');
    expect(el.querySelector('[data-slot="card"]')!.className).toContain('border-l-4 border-l-[#013366] overflow-hidden');
  });

  it('lists the jurisdiction events happening today, sorted by start time, hiding cancelled ones', async () => {
    const fixture = TestBed.createComponent(TodaysEventsCard);
    await settle(fixture);
    const el = fixture.nativeElement as HTMLElement;
    const day = todayIso();
    const expected = SEP_RECORDS.filter(
      (r) => r.status !== 'cancelled' && r.status !== 'denied' && r.city.toLowerCase() === 'victoria' && sepOccursOnDate(r, day),
    ).sort((a, b) => a.startTime.localeCompare(b.startTime));
    expect(expected.length).toBeGreaterThan(0);
    const rows = [...el.querySelectorAll('li')];
    expect(rows.length).toBe(expected.length);
    expected.forEach((r, i) => expect(rows[i].textContent).toContain(r.eventName));
    expect(el.textContent).toContain(`${expected.length}special event`);
    expect(el.textContent).toMatch(/Auto-approved|Approved/);
    expect(TestBed.inject(SepStoreProvider).records().length).toBeGreaterThan(0);
  });
});
