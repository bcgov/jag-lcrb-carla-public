/** Paths come from route-registry.ts. */
import type { Routes } from '@angular/router';
import { page } from '@core/navigation/route-page';
import { routePath } from '@/app/route-registry';

export const routes: Routes = [
  page(routePath('legal-entity.builder'), () => import('@features/legal-entity/pages/legal-entity.page').then((m) => m.LegalEntityPage), {
    title: 'Legal Entity Builder | BC Liquor & Cannabis Licensing',
  }),
  page(routePath('legal-entity.tied-house'), () => import('@features/legal-entity/pages/tied-house.page').then((m) => m.TiedHousePage), {
    title: 'Tied House Declarations | BC Liquor & Cannabis Licensing',
  }),
  page(routePath('legal-entity.tied-house-confirmation'), () =>
    import('@features/legal-entity/pages/tied-house-confirmation.page').then((m) => m.TiedHouseConfirmationPage),
  ),
  page(routePath('legal-entity.security-screening'), () =>
    import('@features/legal-entity/pages/security-screening.page').then((m) => m.SecurityScreeningPage),
  ),
];
