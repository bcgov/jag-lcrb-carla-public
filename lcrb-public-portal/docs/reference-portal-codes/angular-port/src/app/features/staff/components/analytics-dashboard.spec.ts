import { Component } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { By } from '@angular/platform-browser';
import { NgxEchartsDirective } from 'ngx-echarts';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { ChartContainer } from '@shared/ui/charts/chart';
import { computeIntakeAnalytics, loadCases } from '@data-access/repositories/staff/lcrb-analytics';
import { AnalyticsDashboard } from './analytics-dashboard';

@Component({ imports: [AnalyticsDashboard], template: `<analytics-dashboard [analytics]="analytics" />` })
class Host {
  analytics = computeIntakeAnalytics(loadCases());
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

const optionsOf = (fixture: { debugElement: import('@angular/core').DebugElement }) =>
  fixture.debugElement.queryAll(By.directive(NgxEchartsDirective)).map((d) => d.injector.get(NgxEchartsDirective).options() as any);

describe('AnalyticsDashboard', () => {
  beforeEach(() => {
    localStorage.clear();
    sessionStorage.clear();
    vi.stubGlobal('ResizeObserver', class { observe() {} unobserve() {} disconnect() {} });
    // Keep the specs about the data and options: jsdom cannot draw (no canvas text measuring).
    vi.spyOn(NgxEchartsDirective.prototype as any, 'initChart').mockResolvedValue(undefined);
    vi.spyOn(NgxEchartsDirective.prototype as any, 'onOptionsChange').mockResolvedValue(undefined);
    TestBed.configureTestingModule({ providers: [provideRouter([])] });
  });
  afterEach(() => {
    vi.unstubAllGlobals();
    vi.restoreAllMocks();
  });

  it('renders the four KPI tiles from the analytics', async () => {
    const { fixture, el } = await mount();
    const a = fixture.componentInstance.analytics;
    const kpis = [...el.querySelectorAll('[analyticsKpi]')].map((k) => k.textContent!.replace(/\s+/g, ''));
    expect(kpis).toEqual(([
      `Cases synced ${a.total} ${a.syncSuccessRate}% sync success`,
      `Need human review ${a.requiresReviewCount} of ${a.total} intake cases`,
      `Auto-routed to standard ${a.autoRouteRate}% cleared with no elevated signals`,
      `Avg. agent confidence ${a.avgConfidence}% across pre-screened cases`,
    ]).map((t) => t.replace(/\s+/g, '')));
    expect(el.querySelector('a[href="/lcrb/triage"]')?.textContent).toContain('Go to case triage');
  });

  it('gives the three chart cards their titles and sizes', async () => {
    const { el } = await mount();
    expect([...el.querySelectorAll('[data-slot="card-title"]')].map((t) => t.textContent)).toEqual([
      'Cases by triage queue',
      'Agent confidence distribution',
      'Pre-screening check outcomes',
    ]);
    const containers = [...el.querySelectorAll('div[chartContainer]')];
    expect(containers.map((c) => ['h-[280px]', 'h-[300px]'].find((h) => c.classList.contains(h)))).toEqual(['h-[280px]', 'h-[280px]', 'h-[300px]']);
  });

  it('charts the queue counts, confidence bands and stacked check outcomes', async () => {
    const { fixture } = await mount();
    const a = fixture.componentInstance.analytics;
    const [queue, confidence, outcomes] = optionsOf(fixture);

    expect(queue.yAxis.data).toEqual(a.byQueue.map((q) => q.label));
    expect(queue.series[0].data.map((d: any) => d.value)).toEqual(a.byQueue.map((q) => q.count));
    expect(queue.series[0].data.map((d: any) => d.itemStyle.color)).toHaveLength(a.byQueue.length);
    expect(queue.series[0].itemStyle.borderRadius).toEqual([0, 4, 4, 0]);
    expect(queue.grid.left).toBe(138);

    expect(confidence.series[0].radius).toEqual([60, 100]);
    expect(confidence.series[0].padAngle).toBe(2);
    expect(confidence.series[0].data.map((d: any) => [d.name, d.value])).toEqual(a.byConfidence.map((c) => [c.label, c.count]));

    expect(outcomes.xAxis.data).toEqual(a.byCheckOutcome.map((c) => c.label));
    expect(outcomes.series.map((s: any) => [s.name, s.stack])).toEqual([['clear', 'a'], ['attention', 'a'], ['flag', 'a']]);
    expect(outcomes.series[1].data.map((d: any) => d.value)).toEqual(a.byCheckOutcome.map((c) => c.attention));
    expect(outcomes.series[2].itemStyle.borderRadius).toEqual([4, 4, 0, 0]);
  });

  it('shows the outcome legend (Clear, Attention, Flag) and the confidence legend swatches', async () => {
    const { fixture, el } = await mount();
    const containers = fixture.debugElement.queryAll(By.directive(ChartContainer)).map((d) => d.nativeElement as HTMLElement);
    expect(containers[2].textContent).toMatch(/Clear\s*Attention\s*Flag/);
    expect(containers[1].querySelectorAll('.rounded-\\[2px\\]').length).toBe(fixture.componentInstance.analytics.byConfidence.length);
    expect(el.querySelectorAll('div[chartContainer]')).toHaveLength(3);
  });
});
