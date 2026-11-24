/** Staff oversight view; redirects to /login once the session has loaded and is empty. */
import { afterNextRender, ChangeDetectionStrategy, Component, effect, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { ActivityConsole } from '../components/activity-console';
import { BCGovFooter } from '@core/layout/footer';
import { BCGovHeader } from '@core/layout/header';
import { type HeroCrumb } from '@shared/ui/bc-gov/hero-breadcrumb';
import { PageHero } from '@shared/ui/bc-gov/page-hero';
import { injectSession } from '@core/session/session-provider';
import { path } from '@/app/route-registry';

@Component({
  selector: 'activity-page',
  imports: [ActivityConsole, BCGovHeader, BCGovFooter, PageHero],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="flex min-h-screen flex-col bg-background">
      <bc-gov-header currentPage="/activity" />
      <main id="main-content" class="flex-1">
        <page-hero
          containerClassName="mx-auto max-w-5xl px-4 sm:px-6 lg:px-8"
          [breadcrumbs]="breadcrumbs"
          title="Activity & Integrations"
          description="A live record of actions across the portal, outbound notifications, and the status of case syncs to MS Dynamics."
        />

        <div class="mx-auto max-w-5xl px-4 py-8 sm:px-6 lg:px-8">
          <activity-console />
        </div>
      </main>
      <bc-gov-footer />
    </div>
  `,
})
export class ActivityPage {
  protected readonly breadcrumbs: HeroCrumb[] = [
    { label: 'Dashboard', href: path('dashboard.home') },
    { label: 'Activity & Integrations' },
  ];

  private readonly router = inject(Router);
  private readonly sessionProvider = injectSession();
  private readonly mounted = signal(false);

  constructor() {
    afterNextRender(() => this.mounted.set(true));
    // Redirect to /login when loading is done and there is no session.
    effect(() => {
      if (!this.mounted()) return;
      if (!this.sessionProvider.loading() && !this.sessionProvider.session()) {
        void this.router.navigateByUrl(path('account.login'), { replaceUrl: true });
      }
    });
  }
}
