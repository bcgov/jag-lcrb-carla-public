/** Visual matrix: one entry per screenshot; every page route appears at least once. Ids, slugs and seeds come from the seed data. */
import type { Role } from '../support/roles';

export interface RouteEntry {
  /** Unique screenshot id, also the baseline file name prefix. */
  id: string;
  /** The prototype route pattern this entry covers. */
  route: string;
  /** Concrete path to open. */
  path: string;
  /** Signed-in role, or null for signed out. */
  role: Role | null;
  /** Extra localStorage entries needed for the page to render. */
  storage?: Record<string, string>;
  /** CSS selector of chart shapes to wait for (drawn and no longer animating) before the screenshot. */
  charts?: string;
}

/** Recharts bar and pie shapes in the prototype's analytics dashboards. */
const RECHARTS_SHAPES = '.recharts-rectangle, .recharts-sector';
/** The same shapes in the port: ngx-echarts with the SVG renderer. */
export const ECHARTS_SHAPES = '[_echarts_instance_] svg path';

export const CONFIRMATION_ID = 'fixture-confirmation';
const TIED_HOUSE_ID = 'th_new-application_BC0987654';

export const SUBMISSION_SEED = {
  'lcrb.app.submissions': JSON.stringify({
    [CONFIRMATION_ID]: {
      id: CONFIRMATION_ID,
      confirmationNumber: 'PAY-2026-FIX001',
      jobNumber: 'FP-126001',
      flowId: 'fp-name-change',
      title: 'Liquor Establishment Name Change Application',
      applicant: 'Restaurant ABC',
      lineItems: [],
      total: 0,
      currency: 'CAD',
      paymentMethod: 'No payment required',
      submittedAt: '2026-11-02T17:00:00.000Z',
      summary: [{ label: 'Proposed establishment name', value: 'Restaurant XYZ' }],
      syncStatus: 'synced',
      assessment: null,
    },
  }),
};

const TIED_HOUSE_SEED = {
  'lcrb.v2.tied-house-applications': JSON.stringify({
    [TIED_HOUSE_ID]: {
      id: TIED_HOUSE_ID,
      businessLegalName: 'Harding Hospitality Group Ltd.',
      businessNumber: 'BC0987654',
      source: 'new-application',
      jobNumber: 'New Application Job No. 120001',
      status: 'submitted',
      declarations: [],
      existingRelationships: [],
      submittedAt: '2026-11-02T17:00:00.000Z',
      createdAt: '2026-11-02T16:00:00.000Z',
      updatedAt: '2026-11-02T17:00:00.000Z',
    },
  }),
};

const roles: Role[] = ['licensee', 'applicant', 'police', 'local-government', 'lcrb'];

export const ROUTES: RouteEntry[] = [
  // Entry and public
  { id: 'home-redirect', route: '/', path: '/', role: null },
  { id: 'login', route: '/login', path: '/login', role: null },
  { id: 'gate', route: '/gate', path: '/gate', role: null },
  { id: 'accessibility', route: '/accessibility', path: '/accessibility', role: null },
  { id: 'contact', route: '/contact', path: '/contact', role: null },
  { id: 'copyright', route: '/copyright', path: '/copyright', role: null },
  { id: 'disclaimer', route: '/disclaimer', path: '/disclaimer', role: null },
  { id: 'privacy', route: '/privacy', path: '/privacy', role: null },
  { id: 'one-door', route: '/one-door', path: '/one-door', role: null },
  { id: 'upload', route: '/upload', path: '/upload', role: null },

  // Licensee home and shared
  { id: 'dashboard', route: '/dashboard', path: '/dashboard', role: 'licensee' },
  { id: 'announcements', route: '/announcements', path: '/announcements', role: 'licensee' },
  { id: 'notices', route: '/notices', path: '/notices', role: 'licensee' },
  { id: 'notifications-redirect', route: '/notifications', path: '/notifications', role: 'licensee' },
  { id: 'receipts', route: '/receipts', path: '/receipts', role: 'licensee' },
  ...roles.map((role): RouteEntry => ({ id: `account-${role}`, route: '/account', path: '/account', role })),
  { id: 'activity-police', route: '/activity', path: '/activity', role: 'police' },
  { id: 'activity-local-government', route: '/activity', path: '/activity', role: 'local-government' },

  // Applications
  { id: 'applications', route: '/applications', path: '/applications', role: 'licensee' },
  { id: 'applications-new', route: '/applications/new', path: '/applications/new', role: 'licensee' },
  { id: 'application-licence-renewal', route: '/applications/new/[slug]', path: '/applications/new/licence-renewal', role: 'licensee' },
  { id: 'application-food-primary', route: '/applications/new/[slug]', path: '/applications/new/food-primary', role: 'licensee' },
  { id: 'application-eligibility-blocked', route: '/applications/new/[slug]', path: '/applications/new/lrs-relocation', role: 'licensee' },
  { id: 'application-alias-sep', route: '/applications/new/[slug]', path: '/applications/new/sep', role: 'licensee' },
  { id: 'application-unknown-slug', route: '/applications/new/[slug]', path: '/applications/new/no-such-application', role: 'licensee' },
  { id: 'application-food-primary-redirect', route: '/applications/food-primary', path: '/applications/food-primary', role: 'licensee' },
  {
    id: 'application-confirmation',
    route: '/applications/confirmation/[id]',
    path: `/applications/confirmation/${CONFIRMATION_ID}`,
    role: 'licensee',
    storage: SUBMISSION_SEED,
  },
  { id: 'application-submitted', route: '/applications/submitted/[id]', path: '/applications/submitted/LP-125998', role: 'licensee' },
  {
    id: 'application-submitted-documents',
    route: '/applications/submitted/[id]/documents',
    path: '/applications/submitted/CM-125876/documents',
    role: 'licensee',
  },

  // Licences and business structure
  { id: 'licences', route: '/licences', path: '/licences', role: 'licensee' },
  { id: 'licence-detail', route: '/licences/[id]', path: '/licences/326739', role: 'licensee' },
  { id: 'legal-entity', route: '/legal-entity', path: '/legal-entity', role: 'licensee' },
  { id: 'tied-house', route: '/tied-house', path: '/tied-house', role: 'licensee' },
  {
    id: 'tied-house-confirmation',
    route: '/tied-house/confirmation/[id]',
    path: `/tied-house/confirmation/${TIED_HOUSE_ID}`,
    role: 'licensee',
    storage: TIED_HOUSE_SEED,
  },
  { id: 'security-screening', route: '/security-screening', path: '/security-screening', role: 'licensee' },

  // Compliance and planning
  { id: 'compliance', route: '/compliance', path: '/compliance', role: 'licensee' },
  { id: 'calendar', route: '/calendar', path: '/calendar', role: 'licensee' },
  { id: 'licence-finder', route: '/licence-finder', path: '/licence-finder', role: 'licensee' },
  { id: 'establishment-planner', route: '/establishment-planner', path: '/establishment-planner', role: 'licensee' },
  { id: 'map', route: '/map', path: '/map', role: 'licensee' },

  // Special events
  { id: 'special-events', route: '/special-events', path: '/special-events', role: 'applicant' },
  { id: 'special-events-apply', route: '/special-events/apply', path: '/special-events/apply', role: 'applicant' },
  { id: 'special-event-detail', route: '/special-events/[id]', path: '/special-events/SEP-2026-04812', role: 'applicant' },
  {
    id: 'servings-calculator',
    route: '/special-events/servings-calculator',
    path: '/special-events/servings-calculator',
    role: 'applicant',
  },

  // Police
  { id: 'police', route: '/police', path: '/police', role: 'police' },
  { id: 'police-queue', route: '/police/queue', path: '/police/queue', role: 'police' },
  { id: 'police-events', route: '/police/events', path: '/police/events', role: 'police' },
  { id: 'police-review', route: '/police/review/[id]', path: '/police/review/SEP-2026-04788', role: 'police' },

  // Local government
  { id: 'local-government', route: '/local-government', path: '/local-government', role: 'local-government' },
  { id: 'local-government-queue', route: '/local-government/queue', path: '/local-government/queue', role: 'local-government' },
  { id: 'local-government-map', route: '/local-government/map', path: '/local-government/map', role: 'local-government' },
  {
    id: 'local-government-review',
    route: '/local-government/review/[id]',
    path: '/local-government/review/LP-125998',
    role: 'local-government',
  },

  // LCRB staff
  { id: 'lcrb', route: '/lcrb', path: '/lcrb', role: 'lcrb' },
  { id: 'lcrb-triage', route: '/lcrb/triage', path: '/lcrb/triage', role: 'lcrb' },
  { id: 'lcrb-conversations', route: '/lcrb/conversations', path: '/lcrb/conversations', role: 'lcrb' },
  { id: 'lcrb-analytics', route: '/lcrb/analytics', path: '/lcrb/analytics', role: 'lcrb', charts: RECHARTS_SHAPES },
  { id: 'lcrb-site-analytics', route: '/lcrb/site-analytics', path: '/lcrb/site-analytics', role: 'lcrb', charts: RECHARTS_SHAPES },
  { id: 'lcrb-map', route: '/lcrb/map', path: '/lcrb/map', role: 'lcrb' },
];
