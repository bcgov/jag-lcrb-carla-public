import type { Routes } from '@angular/router';
import { routePath } from './route-registry';
import { featureMatcher } from '@core/navigation/route-page';

// Features are added here as they are built; unbuilt routes fall through to Not Found.
export const routes: Routes = [
  { path: routePath('shell.home'), pathMatch: 'full', redirectTo: 'login' },
  { matcher: featureMatcher('account'), loadChildren: () => import('@features/account/account.routes').then((m) => m.routes) },
  { matcher: featureMatcher('dashboard'), loadChildren: () => import('@features/dashboard/dashboard.routes').then((m) => m.routes) },
  { path: '**', loadComponent: () => import('@core/shell/not-found.page').then((m) => m.NotFoundPage) },
];
