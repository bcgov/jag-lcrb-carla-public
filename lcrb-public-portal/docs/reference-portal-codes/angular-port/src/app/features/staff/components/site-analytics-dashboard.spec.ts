import { Component } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { NgxEchartsDirective } from 'ngx-echarts';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { computeSiteAnalytics, loadPageViews } from '@data-access/repositories/staff/site-analytics';
import { SiteAnalyticsDashboard } from './site-analytics-dashboard';

@Component({ imports: [SiteAnalyticsDashboard], template: `<site-analytics-dashboard [analytics]="analytics" />` })
class Host {
  analytics = computeSiteAnalytics(loadPageViews());
}

async function mount() {
  const fixture = TestBed.createComponent(Host);
  for (let i = 0; i < 4; i++) {
    fixture.detectChanges();
    await fixture.whenStable();
    await new Promise((r) => setTimeout(r, 0));
  }
  fixture.detectChanges();
  return { fixture, el: fixture.nativeElement as HTMLElement };
}

describe('SiteAnalyticsDashboard', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
    vi.stubGlobal('ResizeObserver', class { observe() {} unobserve() {} disconnect() {} });
    // Keep the specs about the data and options: jsdom cannot draw (no canvas text measuring).
    vi.spyOn(NgxEchartsDirective.prototype as any, 'initChart').mockResolvedValue(undefined);
    vi.spyOn(NgxEchartsDirective.prototype as any, 'onOptionsChange').mockResolvedValue(undefined);
  });
  afterEach(() => {
    vi.unstubAllGlobals();
    vi.restoreAllMocks();
  });

  it('renders the KPI tiles, the new vs returning cards and the footnote', async () => {
    const { fixture, el } = await mount();
    const a = fixture.componentInstance.analytics;
    const kpis = [...el.querySelectorAll('[siteKpi]')].map((k) => k.textContent!.replace(/\s+/g, ''));
    const up = a.visitorsTrendPct >= 0;
    expect(kpis).toEqual(([
      `Unique visitors ${a.uniqueVisitors.toLocaleString()} ${up ? '+' : ''}${a.visitorsTrendPct}% vs prior 30 days`,
      `Total page views ${a.totalViews.toLocaleString()} ${a.pagesPerSession} pages / session`,
      `Avg. visitors / day ${a.avgVisitorsPerDay.toLocaleString()} ${a.visitorsToday} today`,
      `Sessions ${a.sessions.toLocaleString()} ${a.viewsToday} views today`,
    ]).map((t) => t.replace(/\s+/g, '')));
    expect(el.querySelector('[siteKpi] svg.lucide-trending-up, [siteKpi] svg.lucide-trending-down')).toBeTruthy();
    expect(el.textContent).toContain(a.newVisitors.toLocaleString());
    expect(el.textContent).toContain('New visitors (first-time)');
    expect(el.textContent).toContain('Returning visitors');
    expect(el.textContent).toContain('Figures cover the trailing 30 days.');
  });

  it('charts daily traffic, top pages, sections, top applications and the three session donuts', async () => {
    const { fixture, el } = await mount();
    const a = fixture.componentInstance.analytics;
    expect([...el.querySelectorAll('[data-slot="card-title"]')].map((t) => t.textContent)).toEqual([
      'Daily traffic (last 30 days)',
      'Most-visited pages',
      'Traffic by section',
      'Most-accessed application types',
      'Audience mix',
      'Device split',
      'How visitors arrive',
    ]);
    const opts = fixture.debugElement.queryAll(By.directive(NgxEchartsDirective)).map((d) => d.injector.get(NgxEchartsDirective).options() as any);
    expect(opts).toHaveLength(7);
    const [daily, pages, section, apps, audience, device, referrer] = opts;

    expect(daily.xAxis.data).toEqual(a.daily.map((d) => d.label));
    expect(daily.xAxis.axisLabel.interval).toBe(3);
    expect(daily.series.map((s: any) => s.name)).toEqual(['visitors', 'pageviews']);
    expect(daily.series[0].data.map((d: any) => d.value)).toEqual(a.daily.map((d) => d.visitors));
    expect(daily.series[1].itemStyle.borderRadius).toEqual([3, 3, 0, 0]);

    expect(pages.yAxis.data).toEqual(a.topPages.map((p) => p.title));
    expect(pages.grid.left).toBe(158);
    expect(apps.yAxis.data).toEqual(a.topApplications.map((p) => p.title));
    expect(apps.grid.left).toBe(228);
    expect(pages.series[0].data.map((d: any) => d.itemStyle.color)).toHaveLength(a.topPages.length);

    expect(section.series[0].radius).toEqual([0, 110]);
    expect(section.series[0].data.map((d: any) => [d.name, d.value])).toEqual(a.bySection.map((s) => [s.label, s.views]));
    for (const [opt, data] of [[audience, a.byAudience], [device, a.byDevice], [referrer, a.byReferrer]] as const) {
      expect(opt.series[0].radius).toEqual([45, 90]);
      expect(opt.series[0].data.map((d: any) => [d.name, d.value])).toEqual(data.map((s) => [s.label, s.sessions]));
    }
  });

  it('cycles the seven-colour palette across slices and bars and wraps long legends', async () => {
    const { fixture, el } = await mount();
    const opts = fixture.debugElement.queryAll(By.directive(NgxEchartsDirective)).map((d) => d.injector.get(NgxEchartsDirective).options() as any);
    const colors = opts[1].series[0].data.map((d: any) => d.itemStyle.color);
    expect(colors[0]).toBe(colors[0]);
    expect(new Set(colors).size).toBe(Math.min(7, colors.length));
    const legends = [...el.querySelectorAll('div[chartContainer]')].map((c) => c.querySelector('.justify-center.gap-4.pt-3'));
    expect(legends[0]).toBeTruthy();
    expect(legends.slice(2, 3).concat(legends.slice(4)).every((l) => l!.classList.contains('flex-wrap'))).toBe(true);
    expect(legends[1]).toBeNull();
  });
});
