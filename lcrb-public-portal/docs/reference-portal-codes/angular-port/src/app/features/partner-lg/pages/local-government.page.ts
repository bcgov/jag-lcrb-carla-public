/** Local Government review dashboard. StatCard, Detail and DueDetail use `a` / `div` attribute selectors because the page's grids style their direct children. */
import { ChangeDetectionStrategy, Component, computed, inject, input } from '@angular/core';
import { Router, RouterLink, UrlTree } from '@angular/router';
import { WorkloadSuggestions } from '@shared/ui/ai/workload-suggestions';
import { BCGovFooter } from '@core/layout/footer';
import { BCGovHeader } from '@core/layout/header';
import { LgStatusBadge } from '../components/lg-status-badge';
import { PAGE_HERO } from '@shared/ui/bc-gov/page-hero';
import { CommentingOffBanner } from '../components/commenting-off-banner';
import { TodaysEventsCard } from '../components/todays-events-card';
import { ALERT } from '@shared/ui/primitives/alert';
import { Button } from '@shared/ui/primitives/button';
import { CARD } from '@shared/ui/primitives/card';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import type { IconName } from '@shared/ui/primitives/icons';
import { daysUntilDue } from '@domain/lg-review/due-dates';
import { formatLgDate } from '@domain/lg-review/due-dates';
import { LG_REVIEW_REPOSITORY } from '@data-access/repositories/lg-review/lg-review.repository';
import { path } from '@/app/route-registry';

@Component({
  selector: 'a[lgStatCard]',
  imports: [LucideIcon, CARD],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div uiCard class="hover:shadow-md transition-shadow h-full">
      <div uiCardContent class="p-5 flex items-center gap-4">
        <div class="rounded-lg bg-muted p-3">
          <svg [lucide]="icon()" [class]="'h-6 w-6 ' + accent()"></svg>
        </div>
        <div>
          <div class="text-2xl font-semibold text-foreground">{{ value() }}</div>
          <div class="text-sm text-muted-foreground">{{ label() }}</div>
        </div>
      </div>
    </div>
  `,
})
export class LgStatCard {
  readonly icon = input.required<IconName>();
  readonly label = input.required<string>();
  readonly value = input.required<number>();
  readonly accent = input.required<string>();
}

@Component({
  selector: 'div[lgDetail]',
  imports: [LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { class: 'flex items-center gap-1.5' },
  template: `
    <svg [lucide]="icon()" class="h-4 w-4 shrink-0"></svg>
    <span class="truncate">{{ label() }}</span>
  `,
})
export class LgDetail {
  readonly icon = input.required<IconName>();
  readonly label = input.required<string>();
}

@Component({
  selector: 'div[lgDueDetail]',
  imports: [LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { '[attr.class]': 'hostClass()' },
  template: `
    <svg lucide="Clock" class="h-4 w-4 shrink-0"></svg>
    <span class="truncate">{{ text() }}{{ withDate() && dueDate() ? ' · due ' + formatLgDate(dueDate()!) : '' }}</span>
  `,
})
export class LgDueDetail {
  readonly days = input.required<number>();
  readonly withDate = input<boolean | undefined>(undefined);
  readonly dueDate = input<string | undefined>(undefined);

  protected readonly hostClass = computed(() => {
    const days = this.days();
    const overdue = days < 0;
    const urgent = days >= 0 && days <= 14;
    const color = overdue ? 'text-red-700 font-medium' : urgent ? 'text-orange-600 font-medium' : 'text-muted-foreground';
    return `flex items-center gap-1.5 ${color}`;
  });
  protected readonly text = computed(() => {
    const days = this.days();
    return days < 0
      ? `${Math.abs(days)} day${Math.abs(days) === 1 ? '' : 's'} overdue`
      : `${days} day${days === 1 ? '' : 's'} to comment`;
  });
  protected readonly formatLgDate = formatLgDate;
}

@Component({
  selector: 'local-government-page',
  imports: [
    RouterLink,
    BCGovHeader,
    BCGovFooter,
    ...PAGE_HERO,
    ...CARD,
    ...ALERT,
    Button,
    LucideIcon,
    LgStatusBadge,
    CommentingOffBanner,
    WorkloadSuggestions,
    TodaysEventsCard,
    LgStatCard,
    LgDetail,
    LgDueDetail,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="min-h-screen flex flex-col bg-background">
      <bc-gov-header role="local-government" currentPage="/local-government" />

      <main class="flex-1">
        <page-hero [breadcrumbs]="[{ label: 'Local Government Review' }]" title="Local Government Review" [description]="description" />

        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
          <!-- Shown only while the jurisdiction has opted out of commenting -->
          <commenting-off-banner className="mb-8" />

          <!-- AI Suggest: workload triage across the comment queue -->
          <div class="mb-8">
            <workload-suggestions scope="local-government" [stats]="workloadStats()" [items]="workloadItems()" [links]="workloadLinks" />
          </div>

          <!-- Stat cards -->
          <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 mb-8">
            <a lgStatCard icon="ClipboardList" label="Pending review" [value]="pending().length" [routerLink]="trees['pending-review']" accent="text-[#013366]"></a>
            <a lgStatCard icon="FileWarning" label="Documents requested" [value]="docsRequested().length" [routerLink]="trees['documents-requested']" accent="text-[#1A5A96]"></a>
            <a lgStatCard icon="Clock" label="Community review" [value]="inCommunity().length" [routerLink]="trees['community-review']" accent="text-teal-700"></a>
            <a lgStatCard icon="CheckCircle2" label="Comment submitted" [value]="completed().length" [routerLink]="trees['completed']" accent="text-green-700"></a>
          </div>

          <!-- Deadline awareness -->
          @if (overdue().length > 0) {
            <div uiAlert class="mb-8 border-l-4 border-l-red-600 bg-red-50">
              <svg lucide="Info" class="h-4 w-4"></svg>
              <div uiAlertTitle class="text-red-800">{{ overdue().length }} application{{ overdue().length === 1 ? '' : 's' }} past the comment deadline</div>
              <div uiAlertDescription class="text-red-800">
                If no comment is received, the LCRB may proceed without local government input. Submit your comment as soon as possible.
              </div>
            </div>
          }

          <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
            <!-- Awaiting review -->
            <section class="lg:col-span-2">
              <div class="flex items-center justify-between mb-4">
                <h2 class="text-xl font-semibold text-foreground">Applications awaiting review</h2>
                <a uiButton variant="ghost" size="sm" [routerLink]="path('lg.queue')" class="gap-1 text-[#1A5A96]">
                  View full queue
                  <svg lucide="ArrowRight" class="h-4 w-4"></svg>
                </a>
              </div>

              <div class="space-y-3">
                @if (pending().length === 0 && docsRequested().length === 0) {
                  <div uiCard>
                    <div uiCardContent class="py-10 text-center text-muted-foreground">No applications awaiting your review right now.</div>
                  </div>
                }
                @for (app of awaiting(); track app.jobNumber) {
                  <a [routerLink]="path('lg.review', { id: app.jobNumber })">
                    <div uiCard class="hover:shadow-md transition-shadow">
                      <div uiCardContent class="p-5">
                        <div class="flex items-start justify-between gap-3">
                          <div>
                            <h3 class="font-semibold text-[#013366]">{{ app.establishmentName }}</h3>
                            <p class="text-sm text-muted-foreground mt-0.5">Job #{{ app.jobNumber }} · {{ app.applicationType }}</p>
                          </div>
                          <lg-status-badge [status]="app.status" />
                        </div>
                        <div class="mt-3 grid grid-cols-2 sm:grid-cols-4 gap-3 text-sm text-muted-foreground">
                          <div lgDetail icon="Building2" [label]="app.licenceType"></div>
                          <div lgDetail icon="MapPin" [label]="app.address + ', ' + app.city"></div>
                          <div lgDetail icon="CalendarClock" [label]="'Submitted ' + formatLgDate(app.submittedDate)"></div>
                          <div lgDueDetail [days]="daysUntilDue(app.commentDueDate)"></div>
                        </div>
                      </div>
                    </div>
                  </a>
                }
              </div>
            </section>

            <!-- Due soon sidebar -->
            <section>
              <h2 class="text-xl font-semibold text-foreground mb-4">Comment deadlines</h2>
              <div uiCard>
                <div uiCardContent class="p-0 divide-y">
                  @if (dueSoon().length === 0) {
                    <div class="p-6 text-center text-muted-foreground text-sm">No open applications.</div>
                  }
                  @for (d of dueSoon().slice(0, 6); track d.app.jobNumber) {
                    <a [routerLink]="path('lg.review', { id: d.app.jobNumber })" class="block p-4 hover:bg-muted/50 transition-colors">
                      <div class="flex items-start justify-between gap-2">
                        <span class="font-medium text-[#013366] leading-snug">{{ d.app.establishmentName }}</span>
                        <lg-status-badge [status]="d.app.status" />
                      </div>
                      <div class="mt-2 text-sm">
                        <div lgDueDetail [days]="d.days" [withDate]="true" [dueDate]="d.app.commentDueDate"></div>
                      </div>
                    </a>
                  }
                </div>
              </div>
            </section>
          </div>

          <!-- Special events happening in the jurisdiction today -->
          <div class="mt-8">
            <todays-events-card />
          </div>
        </div>
      </main>

      <bc-gov-footer />
    </div>
  `,
})
export class LocalGovernmentPage {
  protected readonly path = path;
  private readonly lgReviewRepository = inject(LG_REVIEW_REPOSITORY);
  private readonly applications = this.lgReviewRepository.store().applications;

  protected readonly description = `Review liquor and cannabis licence applications for ${this.lgReviewRepository.reviewer().role}. Submit your comment to the LCRB before each application's deadline.`;

  protected readonly pending = computed(() => this.applications().filter((a) => a.status === 'pending-review'));
  protected readonly docsRequested = computed(() => this.applications().filter((a) => a.status === 'documents-requested'));
  protected readonly inCommunity = computed(() => this.applications().filter((a) => a.status === 'community-review'));
  protected readonly completed = computed(() =>
    this.applications().filter((a) => ['no-comment', 'reject', 'completed'].includes(a.status)),
  );

  // Applications approaching their comment deadline (the 90-day clock)
  protected readonly dueSoon = computed(() =>
    this.applications()
      .filter((a) => ['pending-review', 'documents-requested', 'community-review'].includes(a.status))
      .map((a) => ({ app: a, days: daysUntilDue(a.commentDueDate) }))
      .sort((x, y) => x.days - y.days),
  );
  protected readonly overdue = computed(() => this.dueSoon().filter((d) => d.days < 0));
  protected readonly awaiting = computed(() => [...this.pending(), ...this.docsRequested()]);

  protected readonly workloadStats = computed(() => [
    { label: 'Pending review', value: String(this.pending().length) },
    { label: 'Documents requested', value: String(this.docsRequested().length) },
    { label: 'Community review', value: String(this.inCommunity().length) },
    { label: 'Comment submitted', value: String(this.completed().length) },
    { label: 'Past deadline', value: String(this.overdue().length) },
  ]);
  protected readonly workloadItems = computed(() =>
    this.dueSoon()
      .slice(0, 8)
      .map(({ app, days }) => ({
        label: app.establishmentName,
        detail: `${app.status}; ${app.applicationType}; ${app.city}; ${
          days < 0
            ? `${Math.abs(days)} day(s) overdue for comment`
            : `${days} day(s) until comment deadline (${formatLgDate(app.commentDueDate)})`
        }`,
      })),
  );
  protected readonly workloadLinks = [
    { label: 'Open the full queue', href: path('lg.queue') },
    { label: 'Show pending reviews', href: path('lg.queue') + '?status=pending-review' },
    { label: 'Show documents requested', href: path('lg.queue') + '?status=documents-requested' },
    { label: 'Show community review', href: path('lg.queue') + '?status=community-review' },
  ];

  protected readonly daysUntilDue = daysUntilDue;
  protected readonly formatLgDate = formatLgDate;
  /** Links with a query string (the prototype's `href` with `?status=`) as stable UrlTrees. */
  protected readonly trees: Record<string, UrlTree> = Object.fromEntries(
    ['pending-review', 'documents-requested', 'community-review', 'completed'].map((s) => [
      s,
      inject(Router).parseUrl(`${path('lg.queue')}?status=${s}`),
    ]),
  );
}
