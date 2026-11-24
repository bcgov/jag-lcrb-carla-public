/** Records a page view on every route, and on a role change at the same route. */
import { ChangeDetectionStrategy, Component, effect, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { NavigationEnd, Router } from '@angular/router';
import { filter, map } from 'rxjs';
import { injectSession } from '@core/session/session-provider';
import { STAFF_REPOSITORY } from '@data-access/repositories/staff/staff.repository';

@Component({ selector: 'page-view-tracker', template: '', changeDetection: ChangeDetectionStrategy.OnPush, host: { style: 'display: none' } })
export class PageViewTracker {
  constructor() {
    const router = inject(Router);
    const staff = inject(STAFF_REPOSITORY);
    const session = injectSession().session;
    const pathname = toSignal(
      router.events.pipe(
        filter((e) => e instanceof NavigationEnd),
        map((e) => (e as NavigationEnd).urlAfterRedirects.split(/[?#]/)[0]),
      ),
    );
    effect(() => {
      const path = pathname();
      const role = session()?.role ?? 'guest';
      if (!path) return;
      staff.recordPageView(path, role);
    });
  }
}
