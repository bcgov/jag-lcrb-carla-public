/** One-call page setup: signed-in role, storage, frozen clock, seeded Math.random, stubbed map tiles and canned /api answers. */
import { test, type Page, type TestInfo } from '@playwright/test';
import { API_FIXTURES, API_PASSTHROUGH } from '../../tests/fixtures/api';
import { routeApi, type ApiFixture, type ApiRouter } from '../../tests/support/api-fixtures';
import { DEFAULT_SEED, freezeClock, seedRandom, seedStorage, stubMapTiles } from './determinism';

import type { Role } from './roles';
export type { Role };

export const SESSION_KEY = 'lcrb.mock.session';

/** The prototype's demo identities stored as the login page stores them. */
export const IDENTITIES: Record<Role, Record<string, string>> = {
  licensee: {
    role: 'licensee',
    firstName: 'Matt',
    lastName: 'Harding',
    organization: 'Harding Hospitality Group',
    email: 'matt.harding@hardinghospitality.ca',
  },
  applicant: {
    role: 'applicant',
    firstName: 'Alex',
    lastName: 'Rivera',
    organization: 'Community Events Society',
    email: 'alex.rivera@communityevents.org',
  },
  police: {
    role: 'police',
    firstName: 'Dana',
    lastName: 'Okafor',
    organization: 'Vancouver Police Department',
    email: 'dana.okafor@vpd.ca',
  },
  'local-government': {
    role: 'local-government',
    firstName: 'Priya',
    lastName: 'Sandhu',
    organization: 'City of Kelowna',
    email: 'priya.sandhu@kelowna.ca',
  },
  lcrb: {
    role: 'lcrb',
    firstName: 'Morgan',
    lastName: 'Lee',
    organization: 'Liquor & Cannabis Regulation Branch',
    email: 'morgan.lee@gov.bc.ca',
  },
};

export interface PrepareOptions {
  role?: Role | null;
  /** Extra localStorage entries (values are stored as given). */
  storage?: Record<string, string>;
  sessionStorage?: Record<string, string>;
  /** Per-test fixture overrides, merged over API_FIXTURES. */
  fixtures?: Record<string, ApiFixture>;
}

/** Math.random draws each framework makes at startup (React DOM 2, Angular CDK 1); the Angular target burns the difference so app code sees the same values. */
const FRAMEWORK_DRAWS: Record<Target, number> = { reference: 2, angular: 1 };

export async function preparePage(page: Page, opts: PrepareOptions = {}): Promise<ApiRouter> {
  const local: Record<string, string> = { ...(opts.storage ?? {}) };
  if (opts.role) local[SESSION_KEY] = JSON.stringify(IDENTITIES[opts.role]);
  await seedStorage(page, local, opts.sessionStorage ?? {});
  await freezeClock(page);
  await seedRandom(page, DEFAULT_SEED, FRAMEWORK_DRAWS.reference - FRAMEWORK_DRAWS[targetOf(test.info())]);
  await stubMapTiles(page);
  return routeApi(page, { ...API_FIXTURES, ...(opts.fixtures ?? {}) }, API_PASSTHROUGH);
}

export type Target = 'reference' | 'angular';

export function targetOf(testInfo: TestInfo): Target {
  return testInfo.project.metadata['target'] as Target;
}

/** URL of the same app started with ACCESS_CODE set (gate tests). */
export function gatedUrlOf(testInfo: TestInfo): string {
  return testInfo.project.metadata['gatedURL'] as string;
}

/** Skip on Angular projects until the id is listed in e2e/support/ported.ts. */
export function skipUnlessPorted(testInfo: TestInfo, ported: ReadonlySet<string>, id: string): void {
  test.skip(targetOf(testInfo) === 'angular' && !ported.has(id), `Not ported yet: ${id}`);
}
