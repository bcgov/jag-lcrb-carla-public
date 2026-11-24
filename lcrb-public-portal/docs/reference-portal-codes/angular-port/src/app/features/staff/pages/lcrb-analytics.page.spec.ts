import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { NgxEchartsDirective } from 'ngx-echarts';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { SessionProvider } from '@core/session/session-provider';
import { computeIntakeAnalytics, loadCases } from '@data-access/repositories/staff/lcrb-analytics';
import { LcrbAnalyticsPage } from './lcrb-analytics.page';

async function mount(role: 'lcrb' | 'licensee' = 'lcrb') {
  TestBed.inject(SessionProvider).login({ role, firstName: 'Sam', lastName: 'Rowe', organization: 'LCRB' } as never);
  const fixture = TestBed.createComponent(LcrbAnalyticsPage);
  for (let i = 0; i < 4; i++) {
    fixture.detectChanges();
    await fixture.whenStable();
    await new Promise((r) => setTimeout(r, 0));
  }
  fixture.detectChanges();
  return { fixture, el: fixture.nativeElement as HTMLElement };
}

describe('LcrbAnalyticsPage', () => {
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

  it('renders the hero, the KPI tiles and the three charts for LCRB staff', async () => {
    const { el } = await mount();
    expect(el.querySelector('h1')?.textContent).toContain('Intake Analytics');
    expect(el.textContent).toContain('Operational insight into portal intake');
    expect(el.querySelector('nav')?.textContent).toContain('Intake Overview');
    const a = computeIntakeAnalytics(loadCases());
    expect(el.querySelector('[analyticsKpi]')?.textContent).toContain(`${a.total}`);
    expect(el.querySelectorAll('div[chartContainer]')).toHaveLength(3);
  });

  it('is gated for other roles', async () => {
    const { el } = await mount('licensee');
    expect(el.textContent).toContain('LCRB staff access only');
    expect(el.querySelector('analytics-dashboard')).toBeNull();
  });
});
