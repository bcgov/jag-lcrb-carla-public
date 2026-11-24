/** SEP pages need the applicant role; anyone else gets the inline sign-in. TODO: replace the mock `login()` with the verified BCeID / BC Services Card session. */
import { Location } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { injectSession } from '@core/session/session-provider';
import { BCGovFooter } from '@core/layout/footer';
import { BCGovHeader } from '@core/layout/header';
import { Button } from '@shared/ui/primitives/button';
import { Input } from '@shared/ui/primitives/input';
import { Label } from '@shared/ui/primitives/label';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { ROLES } from '@domain/roles';

const DEMO_APPLICANT = {
  role: 'applicant' as const,
  firstName: 'Alex',
  lastName: 'Rivera',
  organization: 'Community Events Society',
};

@Component({
  selector: 'sep-access-guard',
  imports: [BCGovHeader, BCGovFooter, Button, Input, Label, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (sessionProvider.loading()) {
      <!-- Avoid a flash of the gate before the stored session is read. -->
      <div class="min-h-screen flex items-center justify-center bg-background"><span class="sr-only">Loading</span></div>
    } @else if (sessionProvider.session()?.role === 'applicant') {
      <ng-content />
    } @else {
      <div class="min-h-screen flex flex-col bg-[#F2F2F2]">
        <bc-gov-header />
        <main id="main-content" class="flex-1">
          <div class="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-10 md:py-16">
            <button uiButton variant="ghost" (click)="location.back()" class="mb-6 text-[#255A90] hover:bg-transparent hover:underline px-0">
              <svg lucide="ArrowLeft" class="h-4 w-4" aria-hidden="true"></svg>
              Go back
            </button>

            <div class="grid gap-8 md:grid-cols-2 md:items-center">
              <div>
                <span class="inline-flex items-center gap-2 rounded-full bg-[#013366]/10 px-3 py-1 text-sm font-medium text-[#013366]">
                  <svg lucide="CalendarCheck" class="h-4 w-4" aria-hidden="true"></svg>
                  Special Event Permits
                </span>
                <h1 class="mt-4 text-3xl md:text-4xl font-bold text-[#013366] text-balance">Sign in to apply for a Special Event Permit</h1>
                <p class="mt-3 text-lg text-[#474543] leading-relaxed text-pretty">
                  Special Event Permits use their own secure sign-in, separate from the business
                  Licensee portal. Sign in with your BCeID or BC Services Card to apply for and
                  manage permits for one-time events.
                </p>
                @if (sessionProvider.session(); as session) {
                  <div class="mt-5 rounded-md bg-[#FEF6E6] border-l-4 border-[#FCBA19] p-3 text-sm text-[#474543]">
                    You're currently signed in as
                    <strong class="text-[#013366]">{{ roles[session.role].label }}</strong>. Special
                    Event Permits require a separate applicant sign-in.
                  </div>
                }
              </div>

              <div class="bg-white rounded-lg border border-[#D9D9D9] p-6 md:p-8">
                <div class="flex items-center gap-3">
                  <div class="w-11 h-11 rounded-lg bg-[#013366] text-white flex items-center justify-center">
                    <svg lucide="CalendarCheck" class="h-5 w-5" aria-hidden="true"></svg>
                  </div>
                  <div>
                    <h2 class="text-xl font-bold text-[#013366]">SEP Applicant Sign-in</h2>
                    <p class="text-sm text-[#474543]">Secured by {{ roles.applicant.idp }}</p>
                  </div>
                </div>

                <form class="mt-6 flex flex-col gap-4" (submit)="handleSubmit($event)">
                  <div class="flex flex-col gap-1.5">
                    <label uiLabel for="sep-username">BCeID or BC Services Card</label>
                    <input uiInput id="sep-username" name="username" autocomplete="username" placeholder="Enter your BCeID or BC Services Card" value="demo.applicant" />
                  </div>
                  <div class="flex flex-col gap-1.5">
                    <label uiLabel for="sep-password">Password</label>
                    <input uiInput id="sep-password" name="password" type="password" autocomplete="current-password" placeholder="Enter your password" value="demo" />
                  </div>
                  <button uiButton type="submit" class="w-full bg-[#013366] hover:bg-[#38598A] text-white mt-2">
                    <svg lucide="Lock" class="h-4 w-4" aria-hidden="true"></svg>
                    Sign in to Special Event Permits
                  </button>
                </form>

                <div class="mt-4 rounded-md bg-[#F7F9FC] border-l-4 border-[#255A90] p-3 text-sm text-[#474543]">
                  <strong class="text-[#013366]">Prototype:</strong> credentials are pre-filled. In
                  production this redirects to the {{ roles.applicant.idp }} login.
                </div>
              </div>
            </div>
          </div>
        </main>
        <bc-gov-footer />
      </div>
    }
  `,
})
export class SepAccessGuard {
  protected readonly sessionProvider = injectSession();
  protected readonly location = inject(Location);
  protected readonly roles = ROLES;

  protected handleSubmit(event: Event): void {
    event.preventDefault();
    this.sessionProvider.login(DEMO_APPLICANT);
  }
}
