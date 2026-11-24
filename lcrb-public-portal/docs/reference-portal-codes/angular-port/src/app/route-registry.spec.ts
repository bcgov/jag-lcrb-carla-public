/** Stable keys for the 53 prototype routes: the single source of paths for the routes and header navigation. */
import { provideLocationMocks } from '@angular/common/testing';
import { TestBed } from '@angular/core/testing';
import { provideRouter, Router, withComponentInputBinding } from '@angular/router';
import { RouterTestingHarness } from '@angular/router/testing';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import { routes } from './app.routes';
import { flattenRoutes } from '../../tests/support/flatten-routes';
import { NotFoundPage } from '@core/shell/not-found.page';
import { path, ROUTES, routePath, type RouteKey } from './route-registry';

/** Redirect aliases that are not prototype routes, so they have no key. */
const ALIASES = ['applications/new/sep', 'applications/new/special-event-permit', 'applications/new/food-primary-application'];
const REDIRECT_KEYS: RouteKey[] = ['shell.home', 'dashboard.notifications', 'applications.food-primary'];
/** Parameter values that render a real page (the slug must be in the catalogue). */
const SAMPLE = { slug: 'food-primary', id: 'sample-id' };

describe('route registry', () => {
  it('has one key per prototype route, each with its own path', () => {
    const paths = Object.values(ROUTES);
    expect(paths).toHaveLength(53);
    expect(new Set(paths).size).toBe(53);
  });

  it('is the source of every app.routes path, apart from the aliases and the wildcard', async () => {
    const declared = (await flattenRoutes(routes)).map((r) => r.path!).filter((p) => p !== '**' && !ALIASES.includes(p));
    expect([...declared].sort()).toEqual(Object.values(ROUTES).sort());
  });

  it('keys the three redirect pages', async () => {
    const all = await flattenRoutes(routes);
    for (const key of REDIRECT_KEYS) {
      expect(all.find((r) => r.path === routePath(key))?.redirectTo, key).toBeTruthy();
    }
  });

  it('builds URLs with their parameters filled', () => {
    expect(path('dashboard.home')).toBe('/dashboard');
    expect(path('licences.detail', { id: '326739' })).toBe('/licences/326739');
    expect(path('applications.documents', { id: 'CM-125876' })).toBe('/applications/submitted/CM-125876/documents');
    expect(path('shell.home')).toBe('/');
    expect(() => path('police.review')).toThrow(/:id/);
  });

  describe('through the router', () => {
    let harness: RouterTestingHarness;
    afterEach(() => vi.unstubAllGlobals());
    beforeEach(async () => {
      localStorage.clear();
      // jsdom has no ResizeObserver; ngx-vflow (the /legal-entity canvas) needs one.
      vi.stubGlobal('ResizeObserver', class { observe() {} unobserve() {} disconnect() {} });
      TestBed.configureTestingModule({ providers: [provideRouter(routes, withComponentInputBinding()), provideLocationMocks()] });
      harness = await RouterTestingHarness.create();
    });

    it.each((Object.keys(ROUTES) as RouteKey[]).filter((k) => !REDIRECT_KEYS.includes(k)))('%s resolves to its page', async (key) => {
      const params = Object.fromEntries((ROUTES[key].match(/:(\w+)/g) ?? []).map((p) => [p.slice(1), SAMPLE[p.slice(1) as keyof typeof SAMPLE]]));
      const url = path(key, params);
      await harness.navigateByUrl(url);
      expect(TestBed.inject(Router).url).toBe(url);
      expect(harness.routeDebugElement?.componentInstance?.constructor).not.toBe(NotFoundPage);
    });
  });
});
