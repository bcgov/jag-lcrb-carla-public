/** Legal Entity Builder page; the route supplies the title. */
import { ChangeDetectionStrategy, Component } from '@angular/core';
import { BCGovFooter } from '@core/layout/footer';
import { BCGovHeader } from '@core/layout/header';
import { LegalEntityBuilder } from './legal-entity-builder';

@Component({
  selector: 'legal-entity-page',
  imports: [BCGovHeader, BCGovFooter, LegalEntityBuilder],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="flex min-h-screen flex-col bg-background">
      <bc-gov-header currentPage="/legal-entity" />
      <main class="flex-1">
        <legal-entity-builder />
      </main>
      <bc-gov-footer />
    </div>
  `,
})
export class LegalEntityPage {}
