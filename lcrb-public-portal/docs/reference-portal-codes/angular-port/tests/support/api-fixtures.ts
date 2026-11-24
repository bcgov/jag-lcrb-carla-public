/** Answers /api/* calls from canned fixtures; any /api request without a fixture (and not passthrough) is aborted and recorded for `assertNoUnmatched()`. */
import type { Page, Route } from '@playwright/test';

export interface ApiFixture {
  status?: number;
  /** JSON body, serialised for the response. */
  json?: unknown;
  /** Raw body, e.g. a canned AI SDK UI message stream. */
  body?: string;
  contentType?: string;
  headers?: Record<string, string>;
  /** Successive responses for repeated calls; the last one repeats. Overrides the fields above. */
  sequence?: ApiFixture[];
}

export interface ApiRouter {
  unmatched: string[];
  /** Number of answered calls per path. */
  calls: Record<string, number>;
  assertNoUnmatched(): void;
}

export async function routeApi(
  page: Page,
  fixtures: Record<string, ApiFixture>,
  passthrough: string[] = [],
): Promise<ApiRouter> {
  const unmatched: string[] = [];
  const calls: Record<string, number> = {};
  await page.route(
    (url) => url.pathname.startsWith('/api/'),
    async (route: Route) => {
      const url = new URL(route.request().url());
      const entry = fixtures[url.pathname];
      if (entry) {
        const n = calls[url.pathname] ?? 0;
        calls[url.pathname] = n + 1;
        const fixture = entry.sequence ? entry.sequence[Math.min(n, entry.sequence.length - 1)] : entry;
        const isJson = fixture.json !== undefined;
        await route.fulfill({
          status: fixture.status ?? 200,
          headers: fixture.headers,
          contentType: fixture.contentType ?? (isJson ? 'application/json' : 'text/plain; charset=utf-8'),
          body: isJson ? JSON.stringify(fixture.json) : (fixture.body ?? ''),
        });
      } else if (passthrough.includes(url.pathname)) {
        await route.continue();
      } else {
        unmatched.push(`${route.request().method()} ${url.pathname}`);
        await route.abort();
      }
    },
  );
  return {
    unmatched,
    calls,
    assertNoUnmatched() {
      if (unmatched.length > 0) {
        throw new Error(`Unmatched /api requests (add fixtures): ${unmatched.join(', ')}`);
      }
    },
  };
}
