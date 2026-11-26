import type { Routes } from '@angular/router';
import { page } from '@core/navigation/route-page';
import { routePath } from '@/app/route-registry';

export const routes: Routes = [
  page(routePath('account.login'), () => import('@features/account/pages/login.page').then((m) => m.LoginPage)),
];
