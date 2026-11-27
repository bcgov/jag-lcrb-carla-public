import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AssistantLauncher } from '@core/assistant/assistant-launcher';
import { Toaster } from '@shared/ui/primitives/sonner';
import { TopProgressBar } from '@shared/ui/primitives/top-progress-bar';
import { RouteErrorState } from '@core/errors/route-error';
import { RouteErrorPage } from '@core/errors/error';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, TopProgressBar, AssistantLauncher, Toaster, RouteErrorPage],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <a
      href="#main-content"
      class="sr-only focus:not-sr-only focus:absolute focus:left-4 focus:top-4 focus:z-[100] focus:rounded focus:bg-[#013366] focus:px-4 focus:py-2 focus:text-white focus:outline-2 focus:outline-offset-2 focus:outline-[#FCBA19]"
    >
      Skip to main content
    </a>
    <ui-top-progress-bar />
    <div class="min-h-screen">
      @if (routeError.error()) {
        <route-error-page />
      } @else {
        <router-outlet />
      }
    </div>
    <assistant-launcher />
    <ui-toaster position="top-center" richColors />
  `,
})
export class App {
  protected readonly routeError = inject(RouteErrorState);
}
