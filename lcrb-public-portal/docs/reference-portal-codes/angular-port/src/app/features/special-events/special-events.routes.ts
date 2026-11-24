/** special-events routes: loaded by app.routes.ts with loadChildren. Paths come from route-registry.ts. */
import type { Routes } from '@angular/router';
import { page } from '@core/navigation/route-page';
import { routePath } from '@/app/route-registry';

export const routes: Routes = [
  page(routePath('special-events.list'), () => import('@features/special-events/pages/special-events.page').then((m) => m.SpecialEventsPage)),
  page(routePath('special-events.apply'), () =>
    import('@features/special-events/pages/special-events-apply.page').then((m) => m.SpecialEventsApplyPage),
  ),
  page(routePath('special-events.servings-calculator'), () =>
    import('@features/special-events/pages/servings-calculator.page').then((m) => m.ServingsCalculatorPage),
  ),
  page(routePath('special-events.detail'), () =>
    import('@features/special-events/pages/special-event-detail.page').then((m) => m.SpecialEventDetailPage),
  ),
];
