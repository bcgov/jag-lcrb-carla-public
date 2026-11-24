/** licences routes: loaded by app.routes.ts with loadChildren. Paths come from route-registry.ts. */
import type { Routes } from '@angular/router';
import { page } from '@core/navigation/route-page';
import { routePath } from '@/app/route-registry';

export const routes: Routes = [
  page(routePath('licences.list'), () => import('@features/licences/pages/licences.page').then((m) => m.LicencesPage)),
  page(routePath('licences.detail'), () => import('@features/licences/pages/licence-detail.page').then((m) => m.LicenceDetailPage)),
  page(routePath('licences.compliance'), () => import('@features/licences/pages/compliance.page').then((m) => m.CompliancePage), {
    title: 'Compliance & Enforcement | LCRB Portal',
  }),
];
