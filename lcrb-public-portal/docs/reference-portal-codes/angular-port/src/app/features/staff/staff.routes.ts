/** Paths come from route-registry.ts. */
import type { Routes } from '@angular/router';
import { page } from '@core/navigation/route-page';
import { routePath } from '@/app/route-registry';

export const routes: Routes = [
  page(routePath('staff.home'), () => import('@features/staff/pages/lcrb.page').then((m) => m.LcrbPage)),
  page(routePath('staff.triage'), () => import('@features/staff/pages/lcrb-triage.page').then((m) => m.LcrbTriagePage)),
  page(routePath('staff.conversations'), () =>
    import('@features/staff/pages/lcrb-conversations.page').then((m) => m.LcrbConversationsPage),
  ),
  page(routePath('staff.analytics'), () => import('@features/staff/pages/lcrb-analytics.page').then((m) => m.LcrbAnalyticsPage)),
  page(routePath('staff.site-analytics'), () =>
    import('@features/staff/pages/lcrb-site-analytics.page').then((m) => m.LcrbSiteAnalyticsPage),
  ),
  page(routePath('staff.map'), () => import('@features/staff/pages/lcrb-map.page').then((m) => m.LcrbMapPage)),
];
