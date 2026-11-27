// Stable route keys (50 pages and 3 redirects); routes and navigation take their paths from here.
export const ROUTES = {
  'shell.home': '',
  'shell.gate': 'gate',
  'account.login': 'login',
  'account.settings': 'account',
  'account.receipts': 'receipts',
  'dashboard.home': 'dashboard',
  'dashboard.announcements': 'announcements',
  'dashboard.notices': 'notices',
  'dashboard.notifications': 'notifications',
  'dashboard.activity': 'activity',
  'dashboard.calendar': 'calendar',
  'applications.list': 'applications',
  'applications.new': 'applications/new',
  'applications.flow': 'applications/new/:slug',
  'applications.food-primary': 'applications/food-primary',
  'applications.confirmation': 'applications/confirmation/:id',
  'applications.submitted': 'applications/submitted/:id',
  'applications.documents': 'applications/submitted/:id/documents',
  'applications.upload': 'upload',
  'licences.list': 'licences',
  'licences.detail': 'licences/:id',
  'licences.compliance': 'compliance',
  'legal-entity.builder': 'legal-entity',
  'legal-entity.tied-house': 'tied-house',
  'legal-entity.tied-house-confirmation': 'tied-house/confirmation/:id',
  'legal-entity.security-screening': 'security-screening',
  'special-events.list': 'special-events',
  'special-events.apply': 'special-events/apply',
  'special-events.servings-calculator': 'special-events/servings-calculator',
  'special-events.detail': 'special-events/:id',
  'police.home': 'police',
  'police.queue': 'police/queue',
  'police.events': 'police/events',
  'police.review': 'police/review/:id',
  'lg.home': 'local-government',
  'lg.queue': 'local-government/queue',
  'lg.map': 'local-government/map',
  'lg.review': 'local-government/review/:id',
  'staff.home': 'lcrb',
  'staff.triage': 'lcrb/triage',
  'staff.conversations': 'lcrb/conversations',
  'staff.analytics': 'lcrb/analytics',
  'staff.site-analytics': 'lcrb/site-analytics',
  'staff.map': 'lcrb/map',
  'public.accessibility': 'accessibility',
  'public.contact': 'contact',
  'public.copyright': 'copyright',
  'public.disclaimer': 'disclaimer',
  'public.privacy': 'privacy',
  'public.one-door': 'one-door',
  'public.licence-finder': 'licence-finder',
  'public.establishment-planner': 'establishment-planner',
  'public.map': 'map',
} as const;

export type RouteKey = keyof typeof ROUTES;

/** The router path of a key, without a leading slash, as `Route.path` takes it. */
export function routePath(key: RouteKey): string {
  return ROUTES[key];
}

// URL of a key with its `:params` filled, e.g. path('licences.detail', { id: '326739' }).
export function path(key: RouteKey, params: Record<string, string> = {}): string {
  const url = ROUTES[key].replace(/:([A-Za-z]+)/g, (_m, name: string) => {
    const value = params[name];
    if (value === undefined) throw new Error(`route ${key} needs :${name}`);
    return value;
  });
  return `/${url}`;
}
