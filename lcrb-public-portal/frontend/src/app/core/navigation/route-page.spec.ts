import { UrlSegment } from '@angular/router';
import { describe, expect, it } from 'vitest';
import { featureMatcher, page } from './route-page';

const url = (p: string) => p.split('/').filter(Boolean).map((s) => new UrlSegment(s, {}));
// The matcher only reads the segments.
const run = (m: ReturnType<typeof featureMatcher>, p: string) => m(url(p), null as never, null as never);

describe('route helpers', () => {
  it('page() builds a lazy route', () => {
    const load = () => Promise.resolve(class {});
    expect(page('x', load, { title: 'X' })).toEqual({ path: 'x', loadComponent: load, title: 'X' });
  });

  it("featureMatcher matches only its feature's registry paths, consuming nothing", () => {
    const dashboard = featureMatcher('dashboard');
    expect(run(dashboard, '/dashboard')).toEqual({ consumed: [] });
    expect(run(dashboard, '/notices')).toEqual({ consumed: [] });
    expect(run(dashboard, '/dashboard/extra')).toBeNull();
    expect(run(dashboard, '/licences')).toBeNull();
  });

  it('featureMatcher leaves out excluded keys and matches params', () => {
    const applications = featureMatcher('applications', ['applications.food-primary']);
    expect(run(applications, '/applications/food-primary')).toBeNull();
    expect(run(applications, '/applications/new/food-primary')).toEqual({ consumed: [] });
  });
});
