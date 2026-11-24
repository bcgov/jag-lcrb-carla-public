/** account routes: loaded by app.routes.ts with loadChildren. Paths come from route-registry.ts. */
import type { Routes } from '@angular/router';
import { page } from '@core/navigation/route-page';
import { routePath } from '@/app/route-registry';

export const routes: Routes = [
  page(routePath('account.login'), () => import('@features/account/pages/login.page').then((m) => m.LoginPage)),
  page(routePath('account.receipts'), () => import('@features/account/pages/receipts.page').then((m) => m.ReceiptsPage), {
    title: 'Receipts | LCRB Licensing Portal',
  }),
  page(routePath('account.settings'), () => import('@features/account/pages/account.page').then((m) => m.AccountPage)),
];
