/** The renewals and compliance calendar page. */
import { ChangeDetectionStrategy, Component } from '@angular/core';
import { BCGovFooter } from '@core/layout/footer';
import { BCGovHeader } from '@core/layout/header';
import { PageHero } from '@shared/ui/bc-gov/page-hero';
import { RenewalsCalendar } from '../components/renewals-calendar';
import { path } from '@/app/route-registry';

@Component({
  selector: 'calendar-page',
  imports: [BCGovHeader, BCGovFooter, PageHero, RenewalsCalendar],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="min-h-screen flex flex-col bg-background">
      <bc-gov-header currentPage="/calendar" />

      <main class="flex-1">
        <page-hero
          [breadcrumbs]="breadcrumbs"
          title="Renewals & Compliance Calendar"
          description="Every licence renewal, annual fee, insurance expiry and reporting deadline for your establishments in one place. Subscribe or export to keep these dates in your own calendar."
        />

        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
          <renewals-calendar />
        </div>
      </main>

      <bc-gov-footer />
    </div>
  `,
})
export class CalendarPage {
  protected readonly breadcrumbs = [{ label: 'Dashboard', href: path('dashboard.home') }, { label: 'Renewals & Compliance Calendar' }];
}
