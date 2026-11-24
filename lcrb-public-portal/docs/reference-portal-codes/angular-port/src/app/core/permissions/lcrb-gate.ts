/** Client-side gate for LCRB staff pages. TODO: protect these routes server-side against verified IDIR SSO claims; this guard reads the mock session. */
import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { injectSession } from '@core/session/session-provider';
import { Button } from '@shared/ui/primitives/button';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { path } from '@/app/route-registry';

@Component({
  selector: 'lcrb-gate',
  imports: [RouterLink, Button, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (session.loading()) {
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-16 text-center text-muted-foreground">Loading…</div>
    } @else if (!session.session() || session.session()!.role !== 'lcrb') {
      <div class="max-w-lg mx-auto px-4 py-20 text-center">
        <div class="mx-auto w-12 h-12 rounded-full bg-muted flex items-center justify-center mb-4">
          <svg lucide="Lock" class="h-6 w-6 text-muted-foreground" aria-hidden="true"></svg>
        </div>
        <h1 class="text-xl font-semibold text-foreground">LCRB staff access only</h1>
        <p class="text-muted-foreground mt-2">
          This area is restricted to LCRB licensing staff. Sign in with an LCRB Staff account to
          continue.
        </p>
        <a uiButton [routerLink]="path('account.login')" class="mt-6 bg-[#013366] hover:bg-[#012a52]">Go to sign in</a>
      </div>
    } @else {
      <ng-content />
    }
  `,
})
export class LcrbGate {
  protected readonly path = path;
  protected readonly session = injectSession();
}
