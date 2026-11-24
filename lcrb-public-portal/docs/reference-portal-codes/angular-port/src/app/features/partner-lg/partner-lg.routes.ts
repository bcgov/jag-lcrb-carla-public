/** partner-lg routes: loaded by app.routes.ts with loadChildren. Paths come from route-registry.ts. */
import type { Routes } from '@angular/router';
import { page } from '@core/navigation/route-page';
import { routePath } from '@/app/route-registry';

export const routes: Routes = [
  page(routePath('lg.home'), () =>
    import('@features/partner-lg/pages/local-government.page').then((m) => m.LocalGovernmentPage),
  ),
  page(routePath('lg.queue'), () =>
    import('@features/partner-lg/pages/local-government-queue.page').then((m) => m.LocalGovernmentQueuePage),
  ),
  page(routePath('lg.map'), () =>
    import('@features/partner-lg/pages/local-government-map.page').then((m) => m.LocalGovernmentMapPage),
  ),
  page(routePath('lg.review'), () =>
    import('@features/partner-lg/pages/local-government-review.page').then((m) => m.LocalGovernmentReviewPage),
  ),
];
