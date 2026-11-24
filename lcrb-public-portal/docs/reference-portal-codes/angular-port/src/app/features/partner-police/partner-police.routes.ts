/** partner-police routes: loaded by app.routes.ts with loadChildren. Paths come from route-registry.ts. */
import type { Routes } from '@angular/router';
import { page } from '@core/navigation/route-page';
import { routePath } from '@/app/route-registry';

export const routes: Routes = [
  page(routePath('police.home'), () => import('@features/partner-police/pages/police.page').then((m) => m.PolicePage)),
  page(routePath('police.queue'), () => import('@features/partner-police/pages/police-queue.page').then((m) => m.PoliceQueuePage)),
  page(routePath('police.events'), () => import('@features/partner-police/pages/police-events.page').then((m) => m.PoliceEventsPage)),
  page(routePath('police.review'), () => import('@features/partner-police/pages/police-review.page').then((m) => m.PoliceReviewPage)),
];
