/** Paths come from route-registry.ts. */
import { inject } from '@angular/core';
import { type CanMatchFn, type Route, Router, type Routes, type UrlSegment } from '@angular/router';
import { APPLICATIONS_REPOSITORY } from '@data-access/repositories/applications/applications.repository';
import { page } from '@core/navigation/route-page';
import { routePath } from '@/app/route-registry';

/** Unknown catalogue slugs fall through to the not-found page, like notFound(). */
const knownApplicationSlug: CanMatchFn = (_route: Route, segments: UrlSegment[]) => {
  const slug = segments[2]?.path;
  return !!slug && !!inject(APPLICATIONS_REPOSITORY).catalogueEntry(slug) ? true : inject(Router).createUrlTree(['/404-not-found']);
};

export const routes: Routes = [
  page(routePath('applications.upload'), () => import('@features/applications/pages/upload.page').then((m) => m.UploadPage)),
  page(routePath('applications.list'), () => import('@features/applications/pages/applications.page').then((m) => m.ApplicationsPage)),
  page(routePath('applications.new'), () =>
    import('@features/applications/pages/applications-new.page').then((m) => m.ApplicationsNewPage),
  ),
  page(routePath('applications.flow'), () =>
    import('@features/applications/pages/application-flow.page').then((m) => m.ApplicationFlowPage), {
    canMatch: [knownApplicationSlug],
  }),
  page(routePath('applications.confirmation'), () =>
    import('@features/applications/pages/application-confirmation.page').then((m) => m.ApplicationConfirmationPage),
  ),
  page(routePath('applications.submitted'), () =>
    import('@features/applications/pages/application-submitted.page').then((m) => m.ApplicationSubmittedPage),
  ),
  page(routePath('applications.documents'), () =>
    import('@features/applications/pages/application-documents.page').then((m) => m.ApplicationDocumentsPage),
  ),
];
