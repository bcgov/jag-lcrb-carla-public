/** Route helpers shared by app.routes.ts and the feature route files. */
import type { Route, UrlMatcher, UrlSegment } from '@angular/router';
import { ROUTES, type RouteKey } from '@/app/route-registry';

/** A ported page, lazy loaded. */
export function page(path: string, loadComponent: Route['loadComponent'], extra: Partial<Route> = {}): Route {
  return { path, loadComponent, ...extra };
}

// Matches a lazy feature's registry URLs by key prefix without consuming a segment or loading the chunk first.
export function featureMatcher(prefix: string, exclude: RouteKey[] = []): UrlMatcher {
  const patterns = (Object.keys(ROUTES) as RouteKey[])
    .filter((key) => key.startsWith(`${prefix}.`) && !exclude.includes(key))
    .map((key) => ROUTES[key].split('/'));
  return (segments: UrlSegment[]) => {
    const hit = patterns.some(
      (p) => p.length === segments.length && p.every((part, i) => part.startsWith(':') || part === segments[i].path),
    );
    return hit ? { consumed: [] } : null;
  };
}
