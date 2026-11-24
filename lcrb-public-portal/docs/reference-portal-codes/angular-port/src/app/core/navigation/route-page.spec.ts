import { UrlSegment } from '@angular/router';
import { describe, expect, it } from 'vitest';
import { featureMatcher, page } from './route-page';

const url = (p: string) => p.split('/').filter(Boolean).map((s) => new UrlSegment(s, {}));
// The router passes the segments, their group and the route; the matcher only reads the segments.
const run = (m: ReturnType<typeof featureMatcher>, p: string) => m(url(p), null as never, null as never);

describe('route helpers', () => {
  it('page() builds a lazy route', () => {
    const load = () => Promise.resolve(class {});
    expect(page('x', load, { title: 'X' })).toEqual({ path: 'x', loadComponent: load, title: 'X' });
  });

  it("featureMatcher matches only its feature's registry paths, consuming nothing", () => {
    const police = featureMatcher('police');
    expect(run(police, '/police')).toEqual({ consumed: [] });
    expect(run(police, '/police/review/SEP-1')).toEqual({ consumed: [] });
    expect(run(police, '/police/review')).toBeNull();
    expect(run(police, '/local-government/queue')).toBeNull();
  });

  it('featureMatcher leaves out excluded keys', () => {
    const applications = featureMatcher('applications', ['applications.food-primary']);
    expect(run(applications, '/applications/food-primary')).toBeNull();
    expect(run(applications, '/applications/new/food-primary')).toEqual({ consumed: [] });
  });
});
