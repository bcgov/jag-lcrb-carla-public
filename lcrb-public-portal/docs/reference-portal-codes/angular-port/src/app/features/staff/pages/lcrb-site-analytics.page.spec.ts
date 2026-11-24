import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { NgxEchartsDirective } from 'ngx-echarts';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { SessionProvider } from '@core/session/session-provider';
import { computeSiteAnalytics, loadPageViews, recordPageView } from '@data-access/repositories/staff/site-analytics';
import { LcrbSiteAnalyticsPage } from './lcrb-site-analytics.page';

const settle = async (f: { detectChanges(): void; whenStable(): Promise<unknown> }) => {
  for (let i = 0; i < 4; i++) {
    f.detectChanges();
    await f.whenStable();
    await new Promise((r) => setTimeout(r, 0));
  }
  f.detectChanges();
};

async function mount(role: 'lcrb' | 'licensee' = 'lcrb') {
  TestBed.inject(SessionProvider).login({ role, firstName: 'Sam', lastName: 'Rowe', organization: 'LCRB' } as never);
  const fixture = TestBed.createComponent(LcrbSiteAnalyticsPage);
  await settle(fixture);
  return { fixture, el: fixture.nativeElement as HTMLElement };
}

describe('LcrbSiteAnalyticsPage', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
    vi.stubGlobal('ResizeObserver', class { observe() {} unobserve() {} disconnect() {} });
    vi.spyOn(NgxEchartsDirective.prototype as any, 'initChart').mockResolvedValue(undefined);
    vi.spyOn(NgxEchartsDirective.prototype as any, 'onOptionsChange').mockResolvedValue(undefined);
    TestBed.configureTestingModule({ providers: [provideRouter([])] });
  });
  afterEach(() => {
    vi.unstubAllGlobals();
    vi.restoreAllMocks();
  });

  it('renders the hero, KPI tiles and the seven charts for LCRB staff', async () => {
    const { el } = await mount();
    expect(el.querySelector('h1')?.textContent).toContain('Site Analytics');
    expect(el.textContent).toContain('Web traffic and product-usage insight');
    expect(el.querySelectorAll('[siteKpi]')).toHaveLength(4);
    expect(el.querySelectorAll('div[chartContainer]')).toHaveLength(7);
    const a = computeSiteAnalytics(loadPageViews());
    expect(el.textContent).toContain(a.totalViews.toLocaleString());
  });

  it('stays live: a new page view updates the dashboard', async () => {
    const { fixture, el } = await mount();
    const before = computeSiteAnalytics(loadPageViews()).totalViews;
    expect(el.textContent).toContain(before.toLocaleString());
    recordPageView('/dashboard', 'licensee');
    await settle(fixture);
    const after = computeSiteAnalytics(loadPageViews()).totalViews;
    expect(after).toBe(before + 1);
    expect(el.querySelector('[siteKpi]:nth-child(2)')?.textContent).toContain(after.toLocaleString());
  });

  it('is gated for other roles', async () => {
    const { el } = await mount('licensee');
    expect(el.textContent).toContain('LCRB staff access only');
    expect(el.querySelector('site-analytics-dashboard')).toBeNull();
  });
});
