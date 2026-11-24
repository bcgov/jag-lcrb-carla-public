/** Unit-test counterpart of `routeApi`: stubs global `fetch` with the same fixtures; unmatched calls fail with a 599 and are recorded. */
import { vi } from 'vitest';
import type { ApiFixture } from './api-fixtures';

export interface FetchCall {
  path: string;
  body: unknown;
}

export function stubFetch(fixtures: Record<string, ApiFixture>) {
  const calls: FetchCall[] = [];
  const unmatched: string[] = [];
  const counts: Record<string, number> = {};

  const fn = vi.fn(async (input: RequestInfo | URL, init?: RequestInit) => {
    const url = new URL(input instanceof Request ? input.url : String(input), 'http://localhost');
    const raw = init?.body ?? (input instanceof Request ? await input.text() : undefined);
    let body: unknown = raw;
    try {
      body = typeof raw === 'string' ? JSON.parse(raw) : raw;
    } catch {
      // Not JSON: keep the raw body.
    }
    calls.push({ path: url.pathname, body });
    const entry = fixtures[url.pathname];
    if (!entry) {
      unmatched.push(url.pathname);
      return new Response('unmatched', { status: 599 });
    }
    const n = counts[url.pathname] ?? 0;
    counts[url.pathname] = n + 1;
    const f = entry.sequence ? entry.sequence[Math.min(n, entry.sequence.length - 1)] : entry;
    const headers = { 'content-type': f.contentType ?? 'application/json', ...(f.headers ?? {}) };
    const text = f.json !== undefined ? JSON.stringify(f.json) : (f.body ?? '');
    return new Response(text, { status: f.status ?? 200, headers });
  });
  vi.stubGlobal('fetch', fn);

  return {
    calls,
    unmatched,
    /** Calls made to one path. */
    callsTo: (path: string) => calls.filter((c) => c.path === path),
  };
}
