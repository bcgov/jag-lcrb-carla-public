/** Site Analytics, live as page views accrue. */
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { injectExternalStore } from '@shared/util/external-store';
import { BCGovFooter } from '@core/layout/footer';
import { BCGovHeader } from '@core/layout/header';
import { PAGE_HERO } from '@shared/ui/bc-gov/page-hero';
import { LcrbGate } from '@core/permissions/lcrb-gate';
import { SiteAnalyticsDashboard } from '../components/site-analytics-dashboard';
import { injectSession } from '@core/session/session-provider';
import { STAFF_REPOSITORY } from '@data-access/repositories/staff/staff.repository';
import { path } from '@/app/route-registry';

@Component({
  selector: 'lcrb-site-analytics-content',
  imports: [...PAGE_HERO, SiteAnalyticsDashboard],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <page-hero
      [breadcrumbs]="[{ label: 'Intake Overview', href: path('staff.home') }, { label: 'Site Analytics' }]"
      title="Site Analytics"
      description="Web traffic and product-usage insight for the licensing portal: visitors, page views, daily trends, the most-visited pages and application types, plus audience, device, and acquisition breakdowns to guide business and product decisions."
    />
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <site-analytics-dashboard [analytics]="analytics()" />
    </div>
  `,
})
export class LcrbSiteAnalyticsContent {
  protected readonly path = path;
  private readonly staffRepository = inject(STAFF_REPOSITORY);
  // Subscribe to the page-view stream so the dashboard stays live, including the current visit.
  private readonly snapshot = injectExternalStore(this.staffRepository.siteAnalyticsCollection.subscribe, this.staffRepository.siteAnalyticsCollection.snapshot, () => '{}');
  protected readonly analytics = computed(() => (this.snapshot(), this.staffRepository.siteAnalytics(this.staffRepository.pageViews())));

  constructor() {
    // `loadPageViews()` seeds demo traffic on first use and writes the store, which a `computed` may not do, so the first read happens here before the computed runs.
    this.staffRepository.pageViews();
  }
}

@Component({
  selector: 'lcrb-site-analytics-page',
  imports: [BCGovHeader, BCGovFooter, LcrbGate, LcrbSiteAnalyticsContent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="min-h-screen flex flex-col bg-background">
      <bc-gov-header currentPage="/lcrb/site-analytics" />
      <main class="flex-1">
        <lcrb-gate>
          @if (allowed()) {
            <lcrb-site-analytics-content />
          }
        </lcrb-gate>
      </main>
      <bc-gov-footer />
    </div>
  `,
})
export class LcrbSiteAnalyticsPage {
  private readonly session = injectSession();
  /** What `LcrbGate` lets through: its content is created only then, as in React. */
  protected readonly allowed = computed(() => !this.session.loading() && this.session.session()?.role === 'lcrb');

}
