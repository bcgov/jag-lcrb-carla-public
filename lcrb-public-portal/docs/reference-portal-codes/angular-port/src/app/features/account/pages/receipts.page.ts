import { ChangeDetectionStrategy, Component } from '@angular/core';
import { BCGovFooter } from '@core/layout/footer';
import { BCGovHeader } from '@core/layout/header';
import { type HeroCrumb } from '@shared/ui/bc-gov/hero-breadcrumb';
import { PageHero } from '@shared/ui/bc-gov/page-hero';
import { ReceiptsList } from '../components/receipts-list';
import { path } from '@/app/route-registry';

@Component({
  selector: 'receipts-page',
  imports: [BCGovHeader, BCGovFooter, PageHero, ReceiptsList],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="flex min-h-screen flex-col">
      <bc-gov-header currentPage="/applications" />
      <main id="main-content" class="flex-1 bg-muted/20">
        <page-hero
          containerClassName="container mx-auto px-4"
          [breadcrumbs]="breadcrumbs"
          title="Receipts"
          description="Official receipts for every fee you have paid. Each one is emailed to you when you pay and can be downloaded as a PDF here at any time."
        />
        <div class="container mx-auto max-w-4xl px-4 py-8">
          <receipts-list />
        </div>
      </main>
      <bc-gov-footer />
    </div>
  `,
})
export class ReceiptsPage {
  protected readonly breadcrumbs: HeroCrumb[] = [
    { label: 'Dashboard', href: path('dashboard.home') },
    { label: 'My Applications', href: path('applications.list') },
    { label: 'Receipts' },
  ];
}
