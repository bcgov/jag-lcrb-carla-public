/** public-tools routes: loaded by app.routes.ts with loadChildren. Paths come from route-registry.ts. */
import type { Routes } from '@angular/router';
import { page } from '@core/navigation/route-page';
import { routePath } from '@/app/route-registry';

export const routes: Routes = [
  page(routePath('public.accessibility'), () => import('@features/public-tools/pages/accessibility.page').then((m) => m.AccessibilityPage), {
    title: 'Accessibility | Liquor and Cannabis Licensing',
  }),
  page(routePath('public.contact'), () => import('@features/public-tools/pages/contact.page').then((m) => m.ContactPage), {
    title: 'Contact Us | Liquor and Cannabis Licensing',
  }),
  page(routePath('public.copyright'), () => import('@features/public-tools/pages/copyright.page').then((m) => m.CopyrightPage), {
    title: 'Copyright | Liquor and Cannabis Licensing',
  }),
  page(routePath('public.disclaimer'), () => import('@features/public-tools/pages/disclaimer.page').then((m) => m.DisclaimerPage), {
    title: 'Disclaimer | Liquor and Cannabis Licensing',
  }),
  page(routePath('public.privacy'), () => import('@features/public-tools/pages/privacy.page').then((m) => m.PrivacyPage), {
    title: 'Privacy | Liquor and Cannabis Licensing',
  }),
  page(routePath('public.one-door'), () => import('@features/public-tools/pages/one-door.page').then((m) => m.OneDoorPage), {
    title: 'Government of British Columbia — How can we help?',
  }),
  page(routePath('public.licence-finder'), () => import('@features/public-tools/pages/licence-finder.page').then((m) => m.LicenceFinderPage), {
    title: 'Licence Finder | BC Liquor & Cannabis Licensing',
  }),
  page(
    routePath('public.establishment-planner'),
    () => import('@features/public-tools/pages/establishment-planner.page').then((m) => m.EstablishmentPlannerPage),
    { title: 'Establishment Planner | BC Liquor & Cannabis Licensing' },
  ),
  page(routePath('public.map'), () => import('@features/public-tools/pages/map.page').then((m) => m.MapPage)),
];
