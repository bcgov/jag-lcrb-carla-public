/** the Local Government home. */
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { beforeEach, describe, expect, it } from 'vitest';
import { LgCommentingProvider } from '@data-access/repositories/lg-review/lg-commenting-provider';
import { SessionProvider } from '@core/session/session-provider';
import { LG_APPLICATIONS, LG_REVIEWER } from '@data-access/repositories/lg-review/mock/lg-applications';
import { LocalGovernmentPage } from './local-government.page';

const LG = { role: 'local-government', firstName: 'Priya', lastName: 'Sandhu', organization: 'City of Victoria', email: 'p@victoria.ca' } as const;

async function settle(fixture: { detectChanges(): void; whenStable(): Promise<unknown> }) {
  fixture.detectChanges();
  await fixture.whenStable();
  await new Promise((r) => setTimeout(r, 0));
  fixture.detectChanges();
}

async function mount() {
  TestBed.inject(SessionProvider).login(LG as never);
  const fixture = TestBed.createComponent(LocalGovernmentPage);
  await settle(fixture);
  return { fixture, el: fixture.nativeElement as HTMLElement };
}

describe('LocalGovernmentPage', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
    TestBed.configureTestingModule({ providers: [provideRouter([])] });
  });

  it('renders the hero, stat cards with queue links, awaiting list, deadlines and the events card', async () => {
    const { el } = await mount();
    expect(el.querySelector('h1')!.textContent).toContain('Local Government Review');
    expect(el.textContent).toContain(`Review liquor and cannabis licence applications for ${LG_REVIEWER.role}.`);
    expect(el.querySelector('commenting-off-banner div')).toBeNull();

    const count = (s: string) => LG_APPLICATIONS.filter((a) => a.status === s).length;
    const stat = (label: string) => [...el.querySelectorAll('a')].find((a) => a.textContent!.includes(label) && a.querySelector('.text-2xl'))!;
    expect(stat('Pending review').getAttribute('href')).toBe('/local-government/queue?status=pending-review');
    expect(stat('Pending review').querySelector('.text-2xl')!.textContent).toBe(String(count('pending-review')));
    expect(stat('Documents requested').getAttribute('href')).toBe('/local-government/queue?status=documents-requested');
    expect(stat('Community review').getAttribute('href')).toBe('/local-government/queue?status=community-review');
    expect(stat('Comment submitted').getAttribute('href')).toBe('/local-government/queue?status=completed');

    expect(el.textContent).toContain('Applications awaiting review');
    expect(el.querySelector('a[href="/local-government/review/LP-125998"]')).toBeTruthy();
    expect(el.textContent).toContain('Job #LP-125998');
    expect(el.textContent).toContain('Comment deadlines');
    expect(el.textContent).toMatch(/ · due /);
    expect(el.querySelector('[role="alert"]')!.textContent).toContain('past the comment deadline');
    expect(el.querySelector('todays-events-card')!.textContent).toContain('special event');
    expect(el.querySelector('workload-suggestions')).toBeTruthy();
  });

  it('shows the commenting-off banner once the jurisdiction opts out', async () => {
    const { fixture, el } = await mount();
    TestBed.inject(LgCommentingProvider).setOptedOut(true, 'Priya Sandhu', 'City of Victoria');
    await settle(fixture);
    expect(el.querySelector('commenting-off-banner')!.textContent).toContain('Commenting is turned off for City of Victoria');
  });
});
