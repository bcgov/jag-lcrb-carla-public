/** Replaces each `{ matcher, loadChildren }` feature entry with the routes it loads, so specs can check the full table. */
import type { Route, Routes } from '@angular/router';

export async function flattenRoutes(routes: Routes): Promise<Route[]> {
  const out: Route[] = [];
  for (const r of routes) {
    if (r.loadChildren) {
      const loaded = (await (r.loadChildren as () => Promise<Routes>)()) as Routes;
      out.push(...(await flattenRoutes(loaded)));
    } else out.push(r);
  }
  return out;
}
