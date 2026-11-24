/** Intake Analytics. */
import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { BCGovFooter } from '@core/layout/footer';
import { BCGovHeader } from '@core/layout/header';
import { PAGE_HERO } from '@shared/ui/bc-gov/page-hero';
import { AnalyticsDashboard } from '../components/analytics-dashboard';
import { LcrbGate } from '@core/permissions/lcrb-gate';
import { injectSession } from '@core/session/session-provider';
import { STAFF_REPOSITORY } from '@data-access/repositories/staff/staff.repository';
import { path } from '@/app/route-registry';

@Component({
  selector: 'lcrb-analytics-content',
  imports: [...PAGE_HERO, AnalyticsDashboard],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <page-hero
      [breadcrumbs]="[{ label: 'Intake Overview', href: path('staff.home') }, { label: 'Intake Analytics' }]"
      title="Intake Analytics"
      description="Operational insight into portal intake: queue distribution, agent confidence, and pre-screening check outcomes across every case synced to MS Dynamics."
    />
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <analytics-dashboard [analytics]="analytics" />
    </div>
  `,
})
export class LcrbAnalyticsContent {
  protected readonly path = path;
  private readonly staffRepository = inject(STAFF_REPOSITORY);
  protected readonly analytics = this.staffRepository.intakeAnalytics(this.staffRepository.cases());
}

@Component({
  selector: 'lcrb-analytics-page',
  imports: [BCGovHeader, BCGovFooter, LcrbGate, LcrbAnalyticsContent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="min-h-screen flex flex-col bg-background">
      <bc-gov-header currentPage="/lcrb/analytics" />
      <main class="flex-1">
        <lcrb-gate>
          @if (allowed()) {
            <lcrb-analytics-content />
          }
        </lcrb-gate>
      </main>
      <bc-gov-footer />
    </div>
  `,
})
export class LcrbAnalyticsPage {
  private readonly session = injectSession();
  /** What `LcrbGate` lets through: its content is created only then, as in React. */
  protected readonly allowed = computed(() => !this.session.loading() && this.session.session()?.role === 'lcrb');

}
