import { ApplicationConfig, ErrorHandler, provideEnvironmentInitializer } from '@angular/core';
import { provideRouter, TitleStrategy, withComponentInputBinding, withInMemoryScrolling } from '@angular/router';
import { routes } from './app.routes';
import { roundOverlayPositions } from '@core/overlay/overlay-pixel-rounding';
import { RouteErrorHandler } from '@core/errors/route-error';
import { PortalTitleStrategy } from '@core/navigation/title-strategy';

export const appConfig: ApplicationConfig = {
  // No provideBrowserGlobalErrorListeners(): the route error page only shows for errors Angular catches, as with a React error boundary.
  providers: [
    provideRouter(
      routes,
      withComponentInputBinding(),
      // Next.js scrolls to the top on navigation and restores on back/forward.
      withInMemoryScrolling({ scrollPositionRestoration: 'enabled', anchorScrolling: 'enabled' }),
    ),
    { provide: TitleStrategy, useClass: PortalTitleStrategy },
    { provide: ErrorHandler, useClass: RouteErrorHandler },
    // Whole-pixel overlay positions, as Radix/floating-ui.
    provideEnvironmentInitializer(roundOverlayPositions),
  ],
};
