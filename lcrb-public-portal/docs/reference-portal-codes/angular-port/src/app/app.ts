/** Root shell; render order matches the prototype's layout, and its context providers are root services here. */
import { ChangeDetectionStrategy, Component, inject, isDevMode } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { inject as injectAnalytics } from '@vercel/analytics';
import { AssistantLauncher } from '@core/assistant/assistant-launcher';
import { AssistantLayoutShim } from '@core/assistant/assistant-layout-shim';
import { PageViewTracker } from '@core/analytics/page-view-tracker';
import { Toaster } from '@shared/ui/primitives/sonner';
import { TopProgressBar } from '@shared/ui/primitives/top-progress-bar';
import { RouteErrorState } from '@core/errors/route-error';
import { RouteErrorPage } from '@core/errors/error';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, TopProgressBar, PageViewTracker, AssistantLayoutShim, AssistantLauncher, Toaster, RouteErrorPage],
  changeDetection: ChangeDetectionStrategy.OnPush,
  // app-root takes no box, so the page's elements sit where they do under <body> in the prototype.
  host: { style: 'display: contents' },
  template: `
    <a
      href="#main-content"
      class="sr-only focus:not-sr-only focus:absolute focus:left-4 focus:top-4 focus:z-[100] focus:rounded focus:bg-[#013366] focus:px-4 focus:py-2 focus:text-white focus:outline-2 focus:outline-offset-2 focus:outline-[#FCBA19]"
    >
      Skip to main content
    </a>
    <ui-top-progress-bar />
    <page-view-tracker />
    <assistant-layout-shim>
      @if (routeError.error()) {
        <route-error-page />
      } @else {
        <router-outlet />
      }
    </assistant-layout-shim>
    <assistant-launcher />
    <ui-toaster position="top-center" richColors />
  `,
})
export class App {
  protected readonly routeError = inject(RouteErrorState);

  constructor() {
    if (!isDevMode()) injectAnalytics();
  }
}
