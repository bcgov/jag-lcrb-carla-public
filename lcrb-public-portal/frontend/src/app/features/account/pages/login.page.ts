/** Mock login: role chooser, then a pre-filled demo sign-in form for the chosen role. */
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { AccessibilityBar } from '@core/layout/accessibility-bar';
import { BCLogo } from '@shared/ui/bc-gov/bc-logo';
import { ReferralBanner } from '@shared/ui/one-door/referral-banner';
import { injectSession, type Session } from '@core/session/session-provider';
import { Button } from '@shared/ui/primitives/button';
import { type IconName } from '@shared/ui/primitives/icons';
import { Input } from '@shared/ui/primitives/input';
import { Label } from '@shared/ui/primitives/label';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { ROLE_LIST, type RoleConfig, type UserRole } from '@domain/roles';
import { path } from '@/app/route-registry';

const ROLE_ICON: Record<UserRole, IconName> = {
  licensee: 'Building2',
  applicant: 'CalendarCheck',
  police: 'Shield',
  'local-government': 'Landmark',
  lcrb: 'Scale',
};

// Demo identities pre-filled per role so reviewers can sign in instantly.
const DEMO_IDENTITY: Record<string, Omit<Session, 'role'>> = {
  licensee: { firstName: 'Matt', lastName: 'Harding', organization: 'Harding Hospitality Group', email: 'matt.harding@hardinghospitality.ca' },
  applicant: { firstName: 'Alex', lastName: 'Rivera', organization: 'Community Events Society', email: 'alex.rivera@communityevents.org' },
  police: { firstName: 'Dana', lastName: 'Okafor', organization: 'Vancouver Police Department', email: 'dana.okafor@vpd.ca' },
  'local-government': { firstName: 'Priya', lastName: 'Sandhu', organization: 'City of Kelowna', email: 'priya.sandhu@kelowna.ca' },
  lcrb: { firstName: 'Morgan', lastName: 'Lee', organization: 'Liquor & Cannabis Regulation Branch', email: 'morgan.lee@gov.bc.ca' },
};

@Component({
  selector: 'login-page',
  imports: [AccessibilityBar, BCLogo, ReferralBanner, Button, Input, Label, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="min-h-screen flex flex-col bg-[#F2F2F2]">
      <bc-accessibility-bar />
      <!-- Branded top bar — white surface per the BC Gov Design System -->
      <div class="bg-white text-[#2d2d2d]">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div class="flex items-center gap-3 md:gap-4 h-16 md:h-20">
            <bc-logo />
            <span class="text-base md:text-lg font-semibold text-[#013366] border-l border-[#dbdcdc] pl-3 md:pl-4">
              Liquor and Cannabis Licensing
            </span>
          </div>
        </div>
      </div>
      <div class="bg-[#FCBA19] h-1"></div>

      <main id="main-content" class="flex-1">
        <div class="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-10 md:py-16">
          <referral-banner />
          @if (selectedRole(); as role) {
            <div class="max-w-md mx-auto">
              <button
                type="button"
                (click)="selectedRole.set(null)"
                class="inline-flex items-center gap-1 text-sm text-[#255A90] hover:underline mb-6"
              >
                <svg lucide="ArrowLeft" class="h-4 w-4" aria-hidden="true"></svg>
                Choose a different login
              </button>

              <div class="bg-white rounded-lg border border-[#D9D9D9] p-6 md:p-8">
                <div class="flex items-center gap-3">
                  <div class="w-11 h-11 rounded-lg bg-[#013366] text-white flex items-center justify-center">
                    <svg [lucide]="icons[role.id]" class="h-5 w-5" aria-hidden="true"></svg>
                  </div>
                  <div>
                    <h1 class="text-xl font-bold text-[#013366]">{{ role.label }}</h1>
                    <p class="text-sm text-[#474543]">Secured by {{ role.idp }}</p>
                  </div>
                </div>

                <form class="mt-6 flex flex-col gap-4" (submit)="submit($event, role)">
                  <div class="flex flex-col gap-1.5">
                    <label uiLabel for="username">{{ usernameLabel(role) }}</label>
                    <input
                      uiInput
                      id="username"
                      name="username"
                      autocomplete="username"
                      placeholder="Enter your username"
                      value="demo.user"
                    />
                  </div>
                  <div class="flex flex-col gap-1.5">
                    <label uiLabel for="password">Password</label>
                    <input
                      uiInput
                      id="password"
                      name="password"
                      type="password"
                      autocomplete="current-password"
                      placeholder="Enter your password"
                      value="demo"
                    />
                  </div>
                  <button uiButton type="submit" class="w-full bg-[#013366] hover:bg-[#38598A] text-white mt-2">
                    <svg lucide="Lock" class="h-4 w-4" aria-hidden="true"></svg>
                    Sign in
                  </button>
                </form>

                <div class="mt-4 rounded-md bg-[#F7F9FC] border-l-4 border-[#255A90] p-3 text-sm text-[#474543]">
                  <strong class="text-[#013366]">Prototype:</strong> credentials are pre-filled. In production this redirects to the {{ role.idp }} login.
                </div>
              </div>
            </div>
          } @else {
            <div>
              <div class="max-w-2xl">
                <h1 class="text-3xl md:text-4xl font-bold text-[#013366] text-balance">
                  Sign in to Liquor and Cannabis Licensing
                </h1>
                <p class="mt-3 text-lg text-[#474543] leading-relaxed text-pretty">
                  Choose how you want to sign in. Each user type has its own secure login backed by BCeID or BC Services Card credentials.
                </p>
              </div>

              <div class="mt-8 grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
                @for (role of roles; track role.id) {
                  <button
                    type="button"
                    (click)="selectedRole.set(role)"
                    class="group text-left bg-white rounded-lg border border-[#D9D9D9] p-6 transition-all hover:border-[#013366] hover:shadow-md focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-[#255A90]"
                  >
                    <div class="w-12 h-12 rounded-lg bg-[#013366] text-white flex items-center justify-center group-hover:bg-[#38598A]">
                      <svg [lucide]="icons[role.id]" class="h-6 w-6" aria-hidden="true"></svg>
                    </div>
                    <h2 class="mt-4 text-lg font-semibold text-[#2D2D2D]">{{ role.label }}</h2>
                    <p class="mt-1 text-sm text-[#474543] leading-relaxed">{{ role.description }}</p>
                    <span class="mt-4 inline-flex items-center gap-1 text-sm font-medium text-[#255A90] group-hover:underline">
                      Sign in as {{ role.shortLabel }}
                      <span aria-hidden="true">→</span>
                    </span>
                  </button>
                }
              </div>

              <p class="mt-8 text-sm text-[#474543]">
                Don't have an account?{{ ' ' }}<a href="#" (click)="$event.preventDefault()" class="text-[#255A90] underline font-medium">Register for a BCeID</a>
              </p>
            </div>
          }
        </div>
      </main>

      <footer class="bg-white text-[#474543] text-sm border-t-2 border-[#fcba19]">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6 flex flex-col sm:flex-row items-center justify-between gap-2">
          <span>A demonstration prototype for the Province of British Columbia</span>
          <span>BCeID and BC Services Card authentication is integrated at implementation</span>
        </div>
      </footer>
    </div>
  `,
})
export class LoginPage {
  private readonly router = inject(Router);
  private readonly session = injectSession();

  protected readonly roles = ROLE_LIST;
  protected readonly icons = ROLE_ICON;
  protected readonly selectedRole = signal<RoleConfig | null>(null);

  protected usernameLabel(role: RoleConfig): string {
    return role.id === 'licensee' ? 'BCeID' : role.id === 'applicant' ? 'BCeID or BC Services Card' : 'Username';
  }

  protected submit(e: Event, role: RoleConfig): void {
    e.preventDefault();
    this.session.login({ role: role.id, ...DEMO_IDENTITY[role.id] });
    // Only the dashboard is built so far, so every role lands there.
    void this.router.navigateByUrl(path('dashboard.home'));
  }
}
