/** All 53 prototype routes, each lazy-loaded; feature pages sit in per-feature route files behind a registry matcher so only the matching chunk is fetched. */
import type { Routes } from '@angular/router';
import { routePath } from './route-registry';
import { featureMatcher, page } from '@core/navigation/route-page';

export const routes: Routes = [
  { path: routePath('shell.home'), pathMatch: 'full', redirectTo: 'login' },
  page(routePath('shell.gate'), () => import('@core/shell/gate.page').then((m) => m.GatePage), {
    title: 'Restricted access | Liquor and Cannabis Licensing',
  }),
  { matcher: featureMatcher('account'), loadChildren: () => import('@features/account/account.routes').then((m) => m.routes) },
  { matcher: featureMatcher('public'), loadChildren: () => import('@features/public-tools/public-tools.routes').then((m) => m.routes) },
  // Aliases and the redirect page stay ahead of the feature routes, so the slug guard never sees them.
  { path: 'applications/new/sep', redirectTo: '/special-events/apply' },
  { path: 'applications/new/special-event-permit', redirectTo: '/special-events/apply' },
  { path: 'applications/new/food-primary-application', redirectTo: '/applications/new/food-primary' },
  { path: routePath('applications.food-primary'), pathMatch: 'full', redirectTo: 'applications/new/food-primary' },
  { matcher: featureMatcher('applications', ['applications.food-primary']), loadChildren: () => import('@features/applications/applications.routes').then((m) => m.routes) },

  { matcher: featureMatcher('dashboard', ['dashboard.notifications']), loadChildren: () => import('@features/dashboard/dashboard.routes').then((m) => m.routes) },
  { path: routePath('dashboard.notifications'), pathMatch: 'full', redirectTo: 'notices' },

  { matcher: featureMatcher('licences'), loadChildren: () => import('@features/licences/licences.routes').then((m) => m.routes) },
  { matcher: featureMatcher('legal-entity'), loadChildren: () => import('@features/legal-entity/legal-entity.routes').then((m) => m.routes) },

  { matcher: featureMatcher('special-events'), loadChildren: () => import('@features/special-events/special-events.routes').then((m) => m.routes) },

  { matcher: featureMatcher('police'), loadChildren: () => import('@features/partner-police/partner-police.routes').then((m) => m.routes) },

  { matcher: featureMatcher('lg'), loadChildren: () => import('@features/partner-lg/partner-lg.routes').then((m) => m.routes) },

  // LCRB staff pages render LcrbGate themselves, as in the prototype.
  { matcher: featureMatcher('staff'), loadChildren: () => import('@features/staff/staff.routes').then((m) => m.routes) },

  { path: '**', loadComponent: () => import('@core/shell/not-found.page').then((m) => m.NotFoundPage) },
];
