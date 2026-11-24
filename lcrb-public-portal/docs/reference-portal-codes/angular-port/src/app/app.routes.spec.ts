/** Route resolution: visual matrix paths, prototype redirects, slug aliases, not-found and page titles. */
import { afterEach, vi } from 'vitest';
import { provideLocationMocks } from '@angular/common/testing';
import { TestBed } from '@angular/core/testing';
import { Title } from '@angular/platform-browser';
import { provideRouter, Router, TitleStrategy, withComponentInputBinding } from '@angular/router';
import { RouterTestingHarness } from '@angular/router/testing';
import { ROUTES as ROUTE_MATRIX } from '../../e2e/visual/routes';
import { routes } from './app.routes';
import { flattenRoutes } from '../../tests/support/flatten-routes';
import { PortalTitleStrategy, ROOT_TITLE } from '@core/navigation/title-strategy';
import { AccountPage } from '@features/account/pages/account.page';
import { ActivityPage } from '@features/dashboard/pages/activity.page';
import { ApplicationsPage } from '@features/applications/pages/applications.page';
import { ApplicationsNewPage } from '@features/applications/pages/applications-new.page';
import { ApplicationFlowPage } from '@features/applications/pages/application-flow.page';
import { ApplicationConfirmationPage } from '@features/applications/pages/application-confirmation.page';
import { ApplicationSubmittedPage } from '@features/applications/pages/application-submitted.page';
import { ApplicationDocumentsPage } from '@features/applications/pages/application-documents.page';
import { DashboardPage } from '@features/dashboard/pages/dashboard.page';
import { LoginPage } from '@features/account/pages/login.page';
import { AccessibilityPage } from '@features/public-tools/pages/accessibility.page';
import { ContactPage } from '@features/public-tools/pages/contact.page';
import { CopyrightPage } from '@features/public-tools/pages/copyright.page';
import { DisclaimerPage } from '@features/public-tools/pages/disclaimer.page';
import { PrivacyPage } from '@features/public-tools/pages/privacy.page';
import { AnnouncementsPage } from '@features/dashboard/pages/announcements.page';
import { NoticesPage } from '@features/dashboard/pages/notices.page';
import { GatePage } from '@core/shell/gate.page';
import { NotFoundPage } from '@core/shell/not-found.page';
import { ReceiptsPage } from '@features/account/pages/receipts.page';
import { UploadPage } from '@features/applications/pages/upload.page';
import { LcrbPage } from '@features/staff/pages/lcrb.page';
import { LcrbTriagePage } from '@features/staff/pages/lcrb-triage.page';
import { LcrbConversationsPage } from '@features/staff/pages/lcrb-conversations.page';
import { LcrbAnalyticsPage } from '@features/staff/pages/lcrb-analytics.page';
import { LcrbSiteAnalyticsPage } from '@features/staff/pages/lcrb-site-analytics.page';
import { LcrbMapPage } from '@features/staff/pages/lcrb-map.page';
import { LicencesPage } from '@features/licences/pages/licences.page';
import { LicenceDetailPage } from '@features/licences/pages/licence-detail.page';
import { CompliancePage } from '@features/licences/pages/compliance.page';
import { CalendarPage } from '@features/dashboard/pages/calendar.page';
import { LicenceFinderPage } from '@features/public-tools/pages/licence-finder.page';
import { EstablishmentPlannerPage } from '@features/public-tools/pages/establishment-planner.page';
import { OneDoorPage } from '@features/public-tools/pages/one-door.page';
import { MapPage } from '@features/public-tools/pages/map.page';
import { LegalEntityPage } from '@features/legal-entity/pages/legal-entity.page';
import { TiedHousePage } from '@features/legal-entity/pages/tied-house.page';
import { TiedHouseConfirmationPage } from '@features/legal-entity/pages/tied-house-confirmation.page';
import { SecurityScreeningPage } from '@features/legal-entity/pages/security-screening.page';
import { PolicePage } from '@features/partner-police/pages/police.page';
import { PoliceQueuePage } from '@features/partner-police/pages/police-queue.page';
import { PoliceEventsPage } from '@features/partner-police/pages/police-events.page';
import { PoliceReviewPage } from '@features/partner-police/pages/police-review.page';
import { LocalGovernmentPage } from '@features/partner-lg/pages/local-government.page';
import { LocalGovernmentQueuePage } from '@features/partner-lg/pages/local-government-queue.page';
import { LocalGovernmentMapPage } from '@features/partner-lg/pages/local-government-map.page';
import { LocalGovernmentReviewPage } from '@features/partner-lg/pages/local-government-review.page';
import { SpecialEventsPage } from '@features/special-events/pages/special-events.page';
import { SpecialEventsApplyPage } from '@features/special-events/pages/special-events-apply.page';
import { SpecialEventDetailPage } from '@features/special-events/pages/special-event-detail.page';
import { ServingsCalculatorPage } from '@features/special-events/pages/servings-calculator.page';

/** The page each visual matrix entry resolves to. */
const PORTED_PAGES: Record<string, unknown> = {
  'application-unknown-slug': NotFoundPage,
  gate: GatePage,
  dashboard: DashboardPage,
  'login': LoginPage,
  'accessibility': AccessibilityPage,
  'contact': ContactPage,
  'copyright': CopyrightPage,
  'disclaimer': DisclaimerPage,
  'privacy': PrivacyPage,
  'announcements': AnnouncementsPage,
  'notices': NoticesPage,
  'home-redirect': LoginPage,
  'notifications-redirect': NoticesPage,
  'account-licensee': AccountPage,
  'account-applicant': AccountPage,
  'account-police': AccountPage,
  'account-local-government': AccountPage,
  'account-lcrb': AccountPage,
  receipts: ReceiptsPage,
  upload: UploadPage,
  'activity-police': ActivityPage,
  'activity-local-government': ActivityPage,
  applications: ApplicationsPage,
  'applications-new': ApplicationsNewPage,
  'application-submitted': ApplicationSubmittedPage,
  'application-submitted-documents': ApplicationDocumentsPage,
  'application-licence-renewal': ApplicationFlowPage,
  'application-food-primary': ApplicationFlowPage,
  'application-eligibility-blocked': ApplicationFlowPage,
  'application-food-primary-redirect': ApplicationFlowPage,
  'application-confirmation': ApplicationConfirmationPage,
  licences: LicencesPage,
  'licence-detail': LicenceDetailPage,
  'legal-entity': LegalEntityPage,
  'tied-house': TiedHousePage,
  'tied-house-confirmation': TiedHouseConfirmationPage,
  'security-screening': SecurityScreeningPage,
  compliance: CompliancePage,
  calendar: CalendarPage,
  'licence-finder': LicenceFinderPage,
  'establishment-planner': EstablishmentPlannerPage,
  'one-door': OneDoorPage,
  map: MapPage,
  'special-events': SpecialEventsPage,
  'special-events-apply': SpecialEventsApplyPage,
  'special-event-detail': SpecialEventDetailPage,
  'servings-calculator': ServingsCalculatorPage,
  'application-alias-sep': SpecialEventsApplyPage,
  police: PolicePage,
  'police-queue': PoliceQueuePage,
  'police-events': PoliceEventsPage,
  'police-review': PoliceReviewPage,
  lcrb: LcrbPage,
  'lcrb-triage': LcrbTriagePage,
  'lcrb-conversations': LcrbConversationsPage,
  'lcrb-analytics': LcrbAnalyticsPage,
  'lcrb-site-analytics': LcrbSiteAnalyticsPage,
  'lcrb-map': LcrbMapPage,
  'local-government': LocalGovernmentPage,
  'local-government-queue': LocalGovernmentQueuePage,
  'local-government-map': LocalGovernmentMapPage,
  'local-government-review': LocalGovernmentReviewPage,
};

/** The prototype's page routes, 3 of them redirect-only. */
const PROTOTYPE_PAGES = [
    '/',
    '/accessibility',
    '/account',
    '/activity',
    '/announcements',
    '/applications',
    '/applications/confirmation/[id]',
    '/applications/food-primary',
    '/applications/new',
    '/applications/new/[slug]',
    '/applications/submitted/[id]',
    '/applications/submitted/[id]/documents',
    '/calendar',
    '/compliance',
    '/contact',
    '/copyright',
    '/dashboard',
    '/disclaimer',
    '/establishment-planner',
    '/gate',
    '/lcrb',
    '/lcrb/analytics',
    '/lcrb/conversations',
    '/lcrb/map',
    '/lcrb/site-analytics',
    '/lcrb/triage',
    '/legal-entity',
    '/licence-finder',
    '/licences',
    '/licences/[id]',
    '/local-government',
    '/local-government/map',
    '/local-government/queue',
    '/local-government/review/[id]',
    '/login',
    '/map',
    '/notices',
    '/notifications',
    '/one-door',
    '/police',
    '/police/events',
    '/police/queue',
    '/police/review/[id]',
    '/privacy',
    '/receipts',
    '/security-screening',
    '/special-events',
    '/special-events/[id]',
    '/special-events/apply',
    '/special-events/servings-calculator',
    '/tied-house',
    '/tied-house/confirmation/[id]',
    '/upload',
];

let harness: RouterTestingHarness;
afterEach(() => vi.unstubAllGlobals());

beforeEach(async () => {
  localStorage.clear();
  // jsdom has no ResizeObserver; ngx-vflow (the /legal-entity canvas) needs one.
  vi.stubGlobal('ResizeObserver', class { observe() {} unobserve() {} disconnect() {} });
  TestBed.configureTestingModule({
    providers: [
      provideRouter(routes, withComponentInputBinding()),
      provideLocationMocks(),
      { provide: TitleStrategy, useClass: PortalTitleStrategy },
    ],
  });
  harness = await RouterTestingHarness.create();
});

/** Navigates and returns the final URL and the routed component class. */
async function resolve(url: string) {
  await harness.navigateByUrl(url);
  const component = harness.routeDebugElement?.componentInstance?.constructor;
  return { url: TestBed.inject(Router).url, component };
}

describe('routes', () => {
  it('has a route for each of the 53 prototype routes, then the wildcard', async () => {
    const all = await flattenRoutes(routes);
    const declared = all
      .filter((r) => r.path !== '**')
      .map((r) => '/' + r.path!.replace(/:(\w+)/g, '[$1]'))
      .filter((p) => PROTOTYPE_PAGES.includes(p));
    expect([...new Set(declared)].sort()).toEqual([...PROTOTYPE_PAGES].sort());
    expect(all.at(-1)?.path).toBe('**');
  });

  it.each(ROUTE_MATRIX.map((e) => [e.id, e.path] as const))('%s (%s) resolves', async (id, path) => {
    const { component } = await resolve(path);
    expect(PORTED_PAGES[id], `no page listed for ${id}`).toBeDefined();
    expect(component).toBe(PORTED_PAGES[id]);
  });

  it.each([
    ['/', '/login'],
    ['/notifications', '/notices'],
    ['/applications/food-primary', '/applications/new/food-primary'],
    ['/applications/new/sep', '/special-events/apply'],
    ['/applications/new/special-event-permit', '/special-events/apply'],
    ['/applications/new/food-primary-application', '/applications/new/food-primary'],
  ])('redirects %s to %s', async (from, to) => {
    expect((await resolve(from)).url).toBe(to);
  });

  it('renders not-found for an unknown slug and an unknown path', async () => {
    expect((await resolve('/applications/new/not-a-real-licence')).component).toBe(NotFoundPage);
    expect((await resolve('/no/such/page')).component).toBe(NotFoundPage);
  });

  it('sets page titles, falling back to the root title', async () => {
    const title = TestBed.inject(Title);
    await resolve('/receipts');
    expect(title.getTitle()).toBe('Receipts | LCRB Licensing Portal');
    await resolve('/dashboard');
    expect(title.getTitle()).toBe(ROOT_TITLE);
  });
});
