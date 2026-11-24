/** The prototype's `usePathname()`: the current path, without query or hash, as a signal. */
import { inject, type Signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { NavigationEnd, Router } from '@angular/router';
import { filter, map } from 'rxjs';

const pathOf = (url: string) => url.split(/[?#]/)[0] || '/';

export function injectPathname(): Signal<string> {
  const router = inject(Router);
  return toSignal(
    router.events.pipe(
      filter((e): e is NavigationEnd => e instanceof NavigationEnd),
      map((e) => pathOf(e.urlAfterRedirects)),
    ),
    { initialValue: pathOf(router.url) },
  );
}
