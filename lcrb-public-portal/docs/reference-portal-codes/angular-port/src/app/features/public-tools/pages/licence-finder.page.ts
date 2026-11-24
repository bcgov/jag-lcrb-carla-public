import { ChangeDetectionStrategy, Component } from '@angular/core';
import { BCGovFooter } from '@core/layout/footer';
import { BCGovHeader } from '@core/layout/header';
import { LicenceFinder } from './licence-finder';

@Component({
  selector: 'licence-finder-page',
  imports: [BCGovHeader, BCGovFooter, LicenceFinder],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="flex min-h-screen flex-col bg-background">
      <bc-gov-header currentPage="/licence-finder" />
      <main class="flex-1 px-4 py-6 sm:px-6 lg:px-8">
        <licence-finder />
      </main>
      <bc-gov-footer />
    </div>
  `,
})
export class LicenceFinderPage {}
