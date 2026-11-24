/** Route error page. */
import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { RouteErrorState } from './route-error';

@Component({
  selector: 'route-error-page',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="min-h-screen flex flex-col bg-background">
      <!-- Minimal brand bar (no context dependencies) -->
      <div class="bg-[#013366] border-b-2 border-[#FCBA19]">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4">
          <span class="text-white font-semibold">Liquor and Cannabis Licensing Portal</span>
        </div>
      </div>

      <main class="flex-1 flex items-center justify-center px-4 py-16">
        <div class="max-w-lg w-full text-center">
          <div class="mx-auto mb-6 flex h-14 w-14 items-center justify-center rounded-full bg-[#F4E1E2]">
            <svg class="h-7 w-7 text-[#CE3E39]" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true">
              <path d="M10.29 3.86 1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0Z" />
              <line x1="12" y1="9" x2="12" y2="13" />
              <line x1="12" y1="17" x2="12.01" y2="17" />
            </svg>
          </div>
          <h1 class="text-2xl font-semibold text-foreground text-balance">Something went wrong on this page</h1>
          <p class="mt-3 text-muted-foreground text-pretty leading-relaxed">
            We hit an unexpected problem while loading this part of the portal.
            Your saved information has not been lost. You can try again, or head
            back to the home page.
          </p>
          <div class="mt-8 flex flex-col sm:flex-row items-center justify-center gap-3">
            <button
              type="button"
              (click)="state.reset()"
              class="w-full sm:w-auto rounded-md bg-[#013366] px-5 py-2.5 text-sm font-medium text-white hover:bg-[#012a52] focus:outline-none focus-visible:ring-2 focus-visible:ring-[#FCBA19] focus-visible:ring-offset-2"
            >
              Try again
            </button>
            <a
              href="/"
              class="w-full sm:w-auto rounded-md border border-[#013366] px-5 py-2.5 text-sm font-medium text-[#013366] hover:bg-[#013366]/5 focus:outline-none focus-visible:ring-2 focus-visible:ring-[#FCBA19] focus-visible:ring-offset-2"
              >Go to home page</a
            >
          </div>
          @if (state.error()?.digest) {
            <p class="mt-6 text-xs text-muted-foreground">Reference code: {{ state.error()!.digest }}</p>
          }
        </div>
      </main>
    </div>
  `,
})
export class RouteErrorPage {
  protected readonly state = inject(RouteErrorState);
}
