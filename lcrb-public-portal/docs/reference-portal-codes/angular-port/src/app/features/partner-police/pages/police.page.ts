/** Police SEP review dashboard. StatCard, QueueRow and Detail are attribute components because parents style them. `?query` links use routerLink + queryParams. */
import { ChangeDetectionStrategy, Component, computed, inject, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { WorkloadSuggestions } from '@shared/ui/ai/workload-suggestions';
import { BCGovFooter } from '@core/layout/footer';
import { BCGovHeader } from '@core/layout/header';
import { PAGE_HERO } from '@shared/ui/bc-gov/page-hero';
import { SepStatusBadge } from '@shared/ui/bc-gov/sep-status-badge';
import { AutoApprovalStatusCard } from '../components/auto-approval-status-card';
import { injectSession } from '@core/session/session-provider';
import { ALERT } from '@shared/ui/primitives/alert';
import { Button } from '@shared/ui/primitives/button';
import { CARD } from '@shared/ui/primitives/card';
import type { IconName } from '@shared/ui/primitives/icons';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { AUTO_APPROVAL_THRESHOLD, formatEventDate } from '@domain/special-events/seps';
import { type SepRecord } from '@domain/models/special-events';
import { actorName } from '@domain/roles';
import { SPECIAL_EVENTS_REPOSITORY } from '@data-access/repositories/special-events/special-events.repository';
import { path } from '@/app/route-registry';

/** The link is the root element. */
@Component({
  selector: 'a[policeStatCard]',
  imports: [...CARD, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div uiCard class="hover:shadow-md transition-shadow h-full">
      <div uiCardContent class="p-5 flex items-center gap-4">
        <div class="rounded-lg bg-muted p-3">
          <svg [lucide]="icon()" [attr.class]="'h-6 w-6 ' + accent()"></svg>
        </div>
        <div>
          <div class="text-2xl font-semibold text-foreground">{{ value() }}</div>
          <div class="text-sm text-muted-foreground">{{ label() }}</div>
        </div>
      </div>
    </div>
  `,
})
export class PoliceStatCard {
  readonly icon = input.required<IconName>();
  readonly label = input.required<string>();
  readonly value = input.required<number>();
  readonly accent = input.required<string>();
}

@Component({
  selector: 'div[policeDetail]',
  imports: [LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { '[class]': 'cls()' },
  template: `
    <svg [lucide]="icon()" class="h-4 w-4 shrink-0"></svg>
    <span class="truncate">{{ label() }}</span>
  `,
})
export class PoliceDetail {
  readonly icon = input.required<IconName>();
  readonly label = input.required<string>();
  readonly highlight = input<boolean | undefined>(undefined);
  readonly muted = input<boolean | undefined>(undefined);
  protected readonly cls = computed(
    () =>
      `flex items-center gap-1.5 ${
        this.highlight() ? 'text-[#013366] font-medium' : this.muted() ? 'text-orange-600' : 'text-muted-foreground'
      }`,
  );
}

/** The link is the root element. */
@Component({
  selector: 'a[policeQueueRow]',
  imports: [...CARD, PoliceDetail, SepStatusBadge],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div uiCard class="hover:shadow-md transition-shadow">
      <div uiCardHeader class="pb-3">
        <div class="flex items-start justify-between gap-3">
          <div>
            <div uiCardTitle class="text-base text-[#013366]">{{ sep().eventName }}</div>
            <p class="text-sm text-muted-foreground mt-1">{{ sep().id }} · {{ sep().eventType }}</p>
          </div>
          <sep-status-badge [status]="sep().status" />
        </div>
      </div>
      <div uiCardContent class="pt-0">
        <div class="grid grid-cols-2 sm:grid-cols-4 gap-3 text-sm">
          <div policeDetail icon="CalendarClock" [label]="eventDate()"></div>
          <div policeDetail icon="MapPin" [label]="sep().city"></div>
          <div policeDetail icon="Users" [label]="sep().expectedAttendance.toLocaleString() + ' expected'" [highlight]="overThreshold()"></div>
          <div
            policeDetail
            icon="CheckCircle2"
            [label]="sep().assignedOfficer ? sep().assignedOfficer! : 'Unassigned'"
            [muted]="!sep().assignedOfficer"
          ></div>
        </div>
      </div>
    </div>
  `,
})
export class PoliceQueueRow {
  readonly sep = input.required<SepRecord>();
  protected readonly overThreshold = computed(() => this.sep().expectedAttendance >= AUTO_APPROVAL_THRESHOLD);
  protected readonly eventDate = computed(() => formatEventDate(this.sep()));
}

@Component({
  selector: 'police-page',
  imports: [
    RouterLink,
    BCGovHeader,
    BCGovFooter,
    ...PAGE_HERO,
    ...CARD,
    ...ALERT,
    Button,
    LucideIcon,
    SepStatusBadge,
    WorkloadSuggestions,
    AutoApprovalStatusCard,
    PoliceStatCard,
    PoliceQueueRow,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="min-h-screen flex flex-col bg-background">
      <bc-gov-header role="police" currentPage="/police" />

      <main class="flex-1">
        <page-hero
          [breadcrumbs]="[{ label: 'Special Event Permit Review' }]"
          title="Special Event Permit Review"
          description="Review Special Event Permit applications in your jurisdiction, assign officers, add conditions, and record decisions."
        />

        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
          <!-- AI Suggest: workload triage across the whole review queue -->
          <div class="mb-8">
            <workload-suggestions scope="police" [stats]="workloadStats()" [items]="workloadItems()" [links]="workloadLinks" />
          </div>

          <!-- Stat cards -->
          <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 mb-8">
            <a
              policeStatCard
              icon="ClipboardList"
              label="Needs review"
              [value]="needsReview().length"
              accent="text-[#013366]"
              [routerLink]="path('police.queue')"
            ></a>
            <a
              policeStatCard
              icon="AlertTriangle"
              label="Unassigned"
              [value]="unassigned().length"
              accent="text-orange-600"
              [routerLink]="path('police.queue')"
              [queryParams]="{ filter: 'unassigned' }"
            ></a>
            <a
              policeStatCard
              icon="CheckCircle2"
              label="My jobs"
              [value]="myJobs().length"
              accent="text-green-700"
              [routerLink]="path('police.queue')"
              [queryParams]="{ filter: 'mine' }"
            ></a>
            <a
              policeStatCard
              icon="CalendarClock"
              label="Upcoming events"
              [value]="upcoming().length"
              accent="text-[#38598A]"
              [routerLink]="path('police.events')"
            ></a>
          </div>

          <!-- Auto-approval awareness: a key pain point from the journey -->
          @if (recentAutoApproved().length > 0) {
            <div uiAlert class="mb-8 border-l-4 border-l-[#38598A] bg-[#f0f3f8]">
              <svg lucide="Info" class="h-4 w-4"></svg>
              <div uiAlertTitle class="text-[#013366]">
                {{ recentAutoApproved().length }} event{{ recentAutoApproved().length === 1 ? '' : 's' }} auto-approved by LCRB
              </div>
              <div uiAlertDescription>
                Events under {{ threshold }} expected attendees are auto-approved and shown here for your awareness. You cannot add conditions to an
                auto-approved event, but you can review the details and contact the applicant or LCRB if you have concerns.
                <a [routerLink]="path('police.events')" [queryParams]="{ filter: 'auto-approved' }" class="font-medium text-[#1A5A96] hover:underline">
                  View auto-approved events
                </a>
              </div>
            </div>
          }

          <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
            <!-- Action queue -->
            <section class="lg:col-span-2">
              <div class="flex items-center justify-between mb-4">
                <h2 class="text-xl font-semibold text-foreground">Needs your attention</h2>
                <a uiButton variant="ghost" size="sm" [routerLink]="path('police.queue')" class="gap-1 text-[#1A5A96]">
                  View full queue
                  <svg lucide="ArrowRight" class="h-4 w-4"></svg>
                </a>
              </div>

              <div class="space-y-3">
                @if (needsReview().length === 0) {
                  <div uiCard>
                    <div uiCardContent class="py-10 text-center text-muted-foreground">Nothing in your review queue right now.</div>
                  </div>
                }
                @for (sep of needsReview(); track sep.id) {
                  <a policeQueueRow [sep]="sep" [routerLink]="path('police.review', { id: sep.id })"></a>
                }
              </div>
            </section>

            <!-- Upcoming sidebar -->
            <section>
              <div class="mb-6">
                <auto-approval-status-card />
              </div>
              <div class="flex items-center justify-between mb-4">
                <h2 class="text-xl font-semibold text-foreground">Upcoming events</h2>
                <a uiButton variant="ghost" size="sm" [routerLink]="path('police.events')" class="gap-1 text-[#1A5A96]">
                  All
                  <svg lucide="ArrowRight" class="h-4 w-4"></svg>
                </a>
              </div>
              <div uiCard>
                <div uiCardContent class="p-0 divide-y">
                  @for (sep of upcoming().slice(0, 5); track sep.id) {
                    <a [routerLink]="path('police.review', { id: sep.id })" class="block p-4 hover:bg-muted/50 transition-colors">
                      <div class="flex items-start justify-between gap-2">
                        <span class="font-medium text-[#013366] leading-snug">{{ sep.eventName }}</span>
                        <sep-status-badge [status]="sep.status" />
                      </div>
                      <div class="mt-2 text-sm text-muted-foreground flex items-center gap-1">
                        <svg lucide="CalendarClock" class="h-3.5 w-3.5"></svg>
                        {{ eventDate(sep) }}
                      </div>
                      <div class="text-sm text-muted-foreground flex items-center gap-1">
                        <svg lucide="MapPin" class="h-3.5 w-3.5"></svg>
                        {{ sep.city }}
                      </div>
                    </a>
                  }
                  @if (upcoming().length === 0) {
                    <div class="p-6 text-center text-muted-foreground text-sm">No upcoming events.</div>
                  }
                </div>
              </div>
            </section>
          </div>
        </div>
      </main>

      <bc-gov-footer />
    </div>
  `,
})
export class PolicePage {
  protected readonly path = path;
  private readonly specialEventsRepository = inject(SPECIAL_EVENTS_REPOSITORY);
  private readonly store = this.specialEventsRepository.store();
  private readonly sessionProvider = injectSession();
  protected readonly threshold = AUTO_APPROVAL_THRESHOLD;

  // Attribute "My jobs" to the signed-in officer so it stays consistent with assignments made on the review screen (which also use actorName(session)).
  private readonly officerName = computed(() => actorName(this.sessionProvider.session()));

  protected readonly needsReview = computed(() =>
    this.store.records().filter((r) => ['submitted', 'under-review', 'changes-requested'].includes(r.status)),
  );
  protected readonly unassigned = computed(() => this.store.records().filter((r) => r.status === 'submitted' && !r.assignedOfficer));
  protected readonly myJobs = computed(() => this.store.records().filter((r) => r.assignedOfficer === this.officerName()));
  protected readonly upcoming = computed(() => {
    const today = new Date().toISOString().slice(0, 10);
    return this.store
      .records()
      .filter((r) => r.eventStart >= today && r.status !== 'denied' && r.status !== 'cancelled')
      .sort((a, b) => a.eventStart.localeCompare(b.eventStart));
  });
  protected readonly recentAutoApproved = computed(() => this.store.records().filter((r) => r.status === 'auto-approved'));

  protected readonly workloadStats = computed(() => [
    { label: 'Needs review', value: String(this.needsReview().length) },
    { label: 'Unassigned', value: String(this.unassigned().length) },
    { label: 'My jobs', value: String(this.myJobs().length) },
    { label: 'Upcoming events', value: String(this.upcoming().length) },
  ]);
  protected readonly workloadItems = computed(() =>
    this.needsReview()
      .slice(0, 8)
      .map((r) => ({
        label: r.eventName,
        detail: `${r.status}; ${formatEventDate(r)}; ${r.city}; ${r.expectedAttendance.toLocaleString()} expected${
          r.expectedAttendance >= AUTO_APPROVAL_THRESHOLD ? ' (over review threshold)' : ''
        }; ${r.assignedOfficer ? `assigned to ${r.assignedOfficer}` : 'unassigned'}`,
      })),
  );
  protected readonly workloadLinks = [
    { label: 'Open the review queue', href: path('police.queue') },
    { label: 'Show unassigned files', href: path('police.queue') + '?filter=unassigned' },
    { label: 'Show my jobs', href: path('police.queue') + '?filter=mine' },
    { label: 'View upcoming events', href: path('police.events') },
  ];

  protected eventDate(sep: SepRecord): string {
    return formatEventDate(sep);
  }
}
