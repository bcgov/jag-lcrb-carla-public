import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { beforeEach, describe, expect, it } from 'vitest';
import { LgCommentingProvider } from '@data-access/repositories/lg-review/lg-commenting-provider';
import { SessionProvider } from '@core/session/session-provider';
import { QUEUE_META } from '@domain/staff/agent-log';
import { computeIntakeAnalytics, loadCases } from '@data-access/repositories/staff/lcrb-analytics';
import { LcrbPage } from './lcrb.page';

async function mount(role: 'lcrb' | 'licensee' | null = 'lcrb') {
  if (role) TestBed.inject(SessionProvider).login({ role, firstName: 'Sam', lastName: 'Rowe', organization: 'LCRB' } as never);
  const fixture = TestBed.createComponent(LcrbPage);
  for (let i = 0; i < 4; i++) {
    fixture.detectChanges();
    await fixture.whenStable();
    await new Promise((r) => setTimeout(r, 0));
  }
  fixture.detectChanges();
  return { fixture, el: fixture.nativeElement as HTMLElement };
}

describe('LcrbPage', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
    TestBed.configureTestingModule({ providers: [provideRouter([])] });
  });

  it('shows the Intake Overview heading, breadcrumb and stat cards for LCRB staff (flow 1)', async () => {
    const { el } = await mount('lcrb');
    expect(el.querySelector('h1')?.textContent).toContain('Intake Overview');
    expect(el.querySelector('bc-gov-header')).toBeTruthy();
    expect(el.querySelector('bc-gov-footer')).toBeTruthy();
    const a = computeIntakeAnalytics(loadCases());
    const stats = [...el.querySelectorAll('[lcrbStat]')].map((s) => s.textContent!.replace(/\s+/g, ''));
    expect(stats).toEqual([
      `${a.total}Casesinintake`,
      `${a.requiresReviewCount}Needhumanreview`,
      `${a.syncSuccessRate}%SyncedtoDynamics`,
      `${a.autoRouteRate}%Auto-routedstandard`,
    ]);
  });

  it('lists priority cases newest first with links to the triage drawer and the queue breakdown', async () => {
    const { el } = await mount('lcrb');
    const cases = loadCases()
      .filter((c) => ['beyond-licence-cap', 'section-119-review', 'enhanced-review'].includes(c.queue ?? 'standard-processing'))
      .sort((a, b) => (a.lastAttemptAt < b.lastAttemptAt ? 1 : -1));
    const links = [...el.querySelectorAll('section.lg\\:col-span-2 a[href^="/lcrb/triage?case="]')];
    expect(links.map((l) => l.getAttribute('href'))).toEqual(cases.map((c) => `/lcrb/triage?case=${encodeURIComponent(c.id)}`));
    expect(links[0].textContent).toContain(cases[0].reference);
    expect(links[0].textContent).toContain('Synced');
    expect(el.querySelector('section a[href="/lcrb/triage"]')?.textContent).toContain('Open full triage');

    const a = computeIntakeAnalytics(loadCases());
    const queueRows = [...el.querySelectorAll('.divide-y > div')];
    expect(queueRows).toHaveLength(a.byQueue.length);
    expect(queueRows[0].textContent).toContain(QUEUE_META[a.byQueue[0].queue].description);
    expect(queueRows[0].querySelector('span.text-2xl')?.textContent).toBe(String(a.byQueue[0].count));
  });

  it('has the quick links and the local-government commenting notice', async () => {
    const { el } = await mount('lcrb');
    expect([...el.querySelectorAll('a[lcrbQuickLink]')].map((a) => a.getAttribute('href'))).toEqual([
      '/lcrb/triage',
      '/lcrb/conversations',
      '/lcrb/map',
      '/lcrb/analytics',
      '/lcrb/site-analytics',
    ]);
    expect(el.textContent).toContain('Local government commenting');
    expect(el.textContent).toContain('All commenting');
  });

  it('reflects an LG opt-out in the notice', async () => {
    TestBed.inject(LgCommentingProvider).setOptedOut(true, 'City of Surrey', 'City of Surrey');
    const { el } = await mount('lcrb');
    expect(el.textContent).toContain('1 opted out');
  });

  it('is gated: other roles and signed-out visitors see the access message and no overview content', async () => {
    const { el } = await mount('licensee');
    expect(el.textContent).toContain('LCRB staff access only');
    expect(el.querySelector('h1')?.textContent).toBe('LCRB staff access only');
    expect(el.querySelector('lcrb-overview-content')).toBeNull();
    expect(el.querySelector('a[href="/login"]')).toBeTruthy();
  });
});
