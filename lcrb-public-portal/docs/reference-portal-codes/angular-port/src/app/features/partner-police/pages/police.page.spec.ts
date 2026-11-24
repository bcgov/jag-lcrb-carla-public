import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { afterEach, beforeEach, describe, expect, it } from 'vitest';
import { SessionProvider } from '@core/session/session-provider';
import { SepStoreProvider } from '@data-access/repositories/special-events/sep-store-provider';
import { PolicePage } from './police.page';

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
  TestBed.inject(SessionProvider).login({ role: 'police', firstName: 'Dana', lastName: 'Okafor', organization: 'Victoria PD' } as never);
  const fixture = TestBed.createComponent(PolicePage);
  document.body.appendChild(fixture.nativeElement);
  await settle(fixture);
  return { fixture, el: fixture.nativeElement as HTMLElement };
}
const tile = (el: HTMLElement, label: string) => [...el.querySelectorAll('a[policeStatCard]')].find((a) => a.textContent!.includes(label)) as HTMLAnchorElement;

describe('PolicePage', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
  });
  afterEach(() => (document.body.innerHTML = ''));

  it('renders the hero, the four stat tiles with their links, and the AI suggest trigger', async () => {
    const { el } = await mount();
    expect(el.querySelector('h1')!.textContent).toBe('Special Event Permit Review');
    expect(el.textContent).toContain('Review Special Event Permit applications in your jurisdiction');
    const tiles = [...el.querySelectorAll('a[policeStatCard]')];
    expect(tiles.map((t) => t.getAttribute('href'))).toEqual([
      '/police/queue',
      '/police/queue?filter=unassigned',
      '/police/queue?filter=mine',
      '/police/events',
    ]);
    expect(tile(el, 'Needs review').querySelector('.text-2xl')!.textContent).toBe('3');
    expect(tile(el, 'Unassigned').querySelector('.text-2xl')!.textContent).toBe('1');
    expect(tile(el, 'My jobs').querySelector('.text-2xl')!.textContent).toBe('0');
    expect(el.textContent).toContain('AI: Suggest priorities');
  });

  it('lists the files needing attention with review links and the auto-approved notice', async () => {
    const { el } = await mount();
    const rows = [...el.querySelectorAll('a[policeQueueRow]')];
    expect(rows.map((r) => r.getAttribute('href')).sort()).toEqual([
      '/police/review/SEP-2026-04760',
      '/police/review/SEP-2026-04788',
      '/police/review/SEP-2026-04812',
    ]);
    const first = el.querySelector('a[href="/police/review/SEP-2026-04812"]')!;
    expect(first.textContent).toContain('Harbour City Music Festival');
    expect(first.textContent).toContain('SEP-2026-04812');
    expect(first.textContent).toContain('Unassigned');
    expect(el.querySelector('[role="alert"]')!.textContent).toContain('2 events auto-approved by LCRB');
    expect(el.querySelector('[role="alert"] a')!.getAttribute('href')).toBe('/police/events?filter=auto-approved');
  });

  it('counts jobs assigned to the signed-in officer and shows the policy card', async () => {
    const { fixture, el } = await mount();
    TestBed.inject(SepStoreProvider).assignOfficer('SEP-2026-04812', 'Cst. Dana Okafor');
    await settle(fixture);
    expect(tile(el, 'My jobs').querySelector('.text-2xl')!.textContent).toBe('1');
    expect(tile(el, 'Unassigned').querySelector('.text-2xl')!.textContent).toBe('0');
    expect(el.textContent).toContain('SEP auto-approval');
    expect(el.textContent).toContain('Manage in Account Settings');
  });

  it('shows the empty queue message when nothing needs review', async () => {
    const { fixture, el } = await mount();
    const store = TestBed.inject(SepStoreProvider);
    for (const id of ['SEP-2026-04812', 'SEP-2026-04788', 'SEP-2026-04760']) store.decide(id, 'approved', 'Cst. Dana Okafor', '');
    await settle(fixture);
    expect(el.textContent).toContain('Nothing in your review queue right now.');
  });
});
