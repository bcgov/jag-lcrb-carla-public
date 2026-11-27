import { ApplicationConfig, ErrorHandler, provideEnvironmentInitializer } from '@angular/core';
import { provideRouter, TitleStrategy, withComponentInputBinding, withInMemoryScrolling } from '@angular/router';
import { routes } from './app.routes';
import { roundOverlayPositions } from '@core/overlay/overlay-pixel-rounding';
import { RouteErrorHandler } from '@core/errors/route-error';
import { PortalTitleStrategy } from '@core/navigation/title-strategy';

export const appConfig: ApplicationConfig = {
  // No provideBrowserGlobalErrorListeners(): the route error page only shows for errors Angular catches.
  providers: [
    provideRouter(
      routes,
      withComponentInputBinding(),
      // Scroll to top on navigation; restore on back/forward.
      withInMemoryScrolling({ scrollPositionRestoration: 'enabled', anchorScrolling: 'enabled' }),
    ),
    { provide: TitleStrategy, useClass: PortalTitleStrategy },
    { provide: ErrorHandler, useClass: RouteErrorHandler },
    // Round overlay positions to whole pixels.
    provideEnvironmentInitializer(roundOverlayPositions),
  ],
};
