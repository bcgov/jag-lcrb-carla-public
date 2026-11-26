import type { Routes } from '@angular/router';
import { page } from '@core/navigation/route-page';
import { routePath } from '@/app/route-registry';

export const routes: Routes = [
  page(routePath('dashboard.home'), () => import('@features/dashboard/pages/dashboard.page').then((m) => m.DashboardPage)),
];
