/** Tied House confirmation page; the prototype never calls notFound(), so an unknown id is handled in the confirmation. Angular already decodes route params, so no decodeURIComponent. */
import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { BCGovFooter } from '@core/layout/footer';
import { BCGovHeader } from '@core/layout/header';
import { TiedHouseConfirmation } from './tied-house-confirmation';

@Component({
  selector: 'tied-house-confirmation-page',
  imports: [BCGovHeader, BCGovFooter, TiedHouseConfirmation],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="flex min-h-screen flex-col">
      <bc-gov-header currentPage="/tied-house" />
      <main id="main-content" class="flex-1 bg-muted/20">
        <tied-house-confirmation [id]="id()" />
      </main>
      <bc-gov-footer />
    </div>
  `,
})
export class TiedHouseConfirmationPage {
  readonly id = input.required<string>();
}
