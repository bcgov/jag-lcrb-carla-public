import { ChangeDetectionStrategy, Component } from '@angular/core';
import { BCGovFooter } from '@core/layout/footer';
import { BCGovHeader } from '@core/layout/header';
import { EstablishmentPlanner } from './planner-client';

@Component({
  selector: 'establishment-planner-page',
  imports: [BCGovHeader, BCGovFooter, EstablishmentPlanner],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="flex min-h-screen flex-col bg-background">
      <bc-gov-header currentPage="/establishment-planner" />
      <main class="flex-1 px-4 py-6 sm:px-6 lg:px-8">
        <establishment-planner />
      </main>
      <bc-gov-footer />
    </div>
  `,
})
export class EstablishmentPlannerPage {}
