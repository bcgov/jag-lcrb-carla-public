/** The compliance and enforcement page. */
import { ChangeDetectionStrategy, Component } from '@angular/core';
import { BCGovFooter } from '@core/layout/footer';
import { BCGovHeader } from '@core/layout/header';
import { PageHero } from '@shared/ui/bc-gov/page-hero';
import { EnforcementHub } from '../components/enforcement-hub';
import { path } from '@/app/route-registry';

@Component({
  selector: 'compliance-page',
  imports: [BCGovHeader, BCGovFooter, PageHero, EnforcementHub],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="min-h-screen flex flex-col bg-background">
      <bc-gov-header currentPage="/compliance" />

      <main class="flex-1">
        <page-hero
          [breadcrumbs]="breadcrumbs"
          title="Compliance & Enforcement"
          description="Track your establishments' inspection and contravention history, and respond to any enforcement actions issued by the LCRB. Responding on time protects your licence and your right to a hearing."
        />

        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
          <enforcement-hub />
        </div>
      </main>

      <bc-gov-footer />
    </div>
  `,
})
export class CompliancePage {
  protected readonly breadcrumbs = [{ label: 'Dashboard', href: path('dashboard.home') }, { label: 'Compliance & Enforcement' }];
}
