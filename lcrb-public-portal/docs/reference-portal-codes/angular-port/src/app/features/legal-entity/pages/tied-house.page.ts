/** Tied House Declarations page; the route supplies the title. */
import { ChangeDetectionStrategy, Component } from '@angular/core';
import { BCGovFooter } from '@core/layout/footer';
import { BCGovHeader } from '@core/layout/header';
import { TiedHouseTool } from './tied-house-tool';

@Component({
  selector: 'tied-house-page',
  imports: [BCGovHeader, BCGovFooter, TiedHouseTool],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="flex min-h-screen flex-col bg-background">
      <bc-gov-header currentPage="/tied-house" />
      <main id="main-content" class="flex-1">
        <tied-house-tool />
      </main>
      <bc-gov-footer />
    </div>
  `,
})
export class TiedHousePage {}
