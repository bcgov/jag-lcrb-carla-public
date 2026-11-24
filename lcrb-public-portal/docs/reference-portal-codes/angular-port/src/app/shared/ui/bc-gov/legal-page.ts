/** Shared layout for the static BC Gov content pages (Disclaimer, Privacy, Accessibility, Copyright, Contact). */
import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { BCGovFooter } from '@core/layout/footer';
import { BCGovHeader } from '@core/layout/header';

@Component({
  selector: 'legal-page',
  imports: [BCGovHeader, BCGovFooter],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `<div class="min-h-screen flex flex-col bg-muted/30">
    <bc-gov-header />
    <main class="flex-1">
      <div class="max-w-3xl mx-auto px-4 sm:px-6 lg:px-8 py-8 md:py-12">
        <header class="mb-8 border-b border-border pb-6">
          <h1 class="text-3xl md:text-4xl font-semibold text-foreground text-balance">{{ title() }}</h1>
          @if (intro()) {
            <p class="mt-3 text-lg text-muted-foreground leading-relaxed text-pretty">{{ intro() }}</p>
          }
          @if (updated()) {
            <p class="mt-3 text-sm text-muted-foreground">Last updated: {{ updated() }}</p>
          }
        </header>
        <div class="space-y-6 leading-relaxed text-foreground [&_h2]:text-xl [&_h2]:font-semibold [&_h2]:text-foreground [&_h2]:mt-8 [&_h2]:mb-2 [&_a]:text-[#1A5A96] [&_a]:underline [&_ul]:list-disc [&_ul]:pl-6 [&_ul]:space-y-1">
          <ng-content />
        </div>
      </div>
    </main>
    <bc-gov-footer />
  </div>`,
})
export class LegalPage {
  readonly title = input.required<string>();
  readonly intro = input<string | undefined>(undefined);
  /** ISO date string for the "Last updated" line. */
  readonly updated = input<string | undefined>(undefined);
}
