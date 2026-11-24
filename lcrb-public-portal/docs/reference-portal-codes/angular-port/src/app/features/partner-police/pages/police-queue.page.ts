/** Police review queue. The initial filter is read once from `?filter=`. `QueueCard` and `Meta` are attribute components because their parents style them. */
import { ChangeDetectionStrategy, Component, computed, inject, input, output, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ReviewerSuggestedActions, type ReviewerFact } from '@shared/ui/ai/reviewer-suggested-actions';
import { WorkloadSuggestions } from '@shared/ui/ai/workload-suggestions';
import { BCGovFooter } from '@core/layout/footer';
import { BCGovHeader } from '@core/layout/header';
import { PAGE_HERO } from '@shared/ui/bc-gov/page-hero';
import { SepStatusBadge } from '@shared/ui/bc-gov/sep-status-badge';
import { injectSession } from '@core/session/session-provider';
import { Button } from '@shared/ui/primitives/button';
import { CARD } from '@shared/ui/primitives/card';
import type { IconName } from '@shared/ui/primitives/icons';
import { Input } from '@shared/ui/primitives/input';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { SELECT } from '@shared/ui/primitives/select';
import { AUTO_APPROVAL_THRESHOLD, formatEventDate, SEP_STATUS_LABELS } from '@domain/special-events/seps';
import { type SepRecord, type SepStatus } from '@domain/models/special-events';
import { actorName } from '@domain/roles';
import { SPECIAL_EVENTS_REPOSITORY } from '@data-access/repositories/special-events/special-events.repository';
import { path } from '@/app/route-registry';

@Component({
  selector: 'div[policeMeta]',
  imports: [LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { '[class]': 'cls()' },
  template: `
    <svg [lucide]="icon()" class="h-4 w-4 shrink-0"></svg>
    <span class="truncate">{{ text() }}</span>
  `,
})
export class PoliceMeta {
  readonly icon = input.required<IconName>();
  readonly text = input.required<string>();
  readonly strong = input<boolean | undefined>(undefined);
  readonly warn = input<boolean | undefined>(undefined);
  protected readonly cls = computed(
    () =>
      `flex items-center gap-1.5 min-w-0 ${
        this.strong() ? 'text-[#013366] font-medium' : this.warn() ? 'text-orange-600' : 'text-muted-foreground'
      }`,
  );
}

/** Used on a `div[uiCard]` element. */
@Component({
  selector: 'div[policeQueueCard]',
  imports: [...CARD, RouterLink, Button, LucideIcon, SepStatusBadge, PoliceMeta, ReviewerSuggestedActions],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div uiCardContent class="p-4">
      <div class="flex flex-col lg:flex-row lg:items-center gap-4">
        <div class="flex-1 min-w-0">
          <div class="flex items-center gap-2 flex-wrap">
            <a [routerLink]="path('police.review', { id: sep().id })" class="font-semibold text-[#013366] hover:underline">{{ sep().eventName }}</a>
            <sep-status-badge [status]="sep().status" />
            @if (overThreshold()) {
              <span class="text-xs rounded bg-[#013366] text-white px-2 py-0.5">Requires review</span>
            } @else {
              <span class="text-xs rounded bg-slate-200 text-slate-700 px-2 py-0.5">Under {{ threshold }}</span>
            }
          </div>
          <p class="text-sm text-muted-foreground mt-1">
            {{ sep().id }} · {{ sep().eventType }} · Applicant: {{ sep().applicantName }}{{ sep().applicantOrg ? ' (' + sep().applicantOrg + ')' : '' }}
          </p>

          <div class="grid grid-cols-2 sm:grid-cols-4 gap-3 text-sm mt-3">
            <div policeMeta icon="CalendarClock" [text]="eventDate()"></div>
            <div policeMeta icon="MapPin" [text]="sep().venueName + ', ' + sep().city"></div>
            <div policeMeta icon="Users" [text]="sep().expectedAttendance.toLocaleString() + ' expected'" [strong]="overThreshold()"></div>
            <div policeMeta icon="User" [text]="sep().assignedOfficer ?? 'Unassigned'" [warn]="!sep().assignedOfficer"></div>
          </div>
        </div>

        <div class="flex items-center gap-2 shrink-0">
          @if (canAssign()) {
            <button uiButton variant="outline" size="sm" (click)="assign.emit()">Assign to me</button>
          }
          <a uiButton size="sm" class="bg-[#013366] hover:bg-[#012a52] gap-1" [routerLink]="path('police.review', { id: sep().id })">
            Review
            <svg lucide="ArrowRight" class="h-4 w-4"></svg>
          </a>
        </div>
      </div>

      <div class="mt-4 border-t pt-4">
        <reviewer-suggested-actions
          role="police"
          [itemTitle]="sep().eventName"
          [itemId]="sep().id"
          [status]="statusLabel()"
          [facts]="facts()"
          [reviewHref]="path('police.review', { id: sep().id })"
        />
      </div>
    </div>
  `,
})
export class PoliceQueueCard {
  protected readonly path = path;
  readonly sep = input.required<SepRecord>();
  readonly assign = output<void>();
  protected readonly threshold = AUTO_APPROVAL_THRESHOLD;
  protected readonly overThreshold = computed(() => this.sep().expectedAttendance >= AUTO_APPROVAL_THRESHOLD);
  protected readonly canAssign = computed(() => !this.sep().assignedOfficer && this.sep().status === 'submitted');
  protected readonly eventDate = computed(() => formatEventDate(this.sep()));
  protected readonly statusLabel = computed(() => SEP_STATUS_LABELS[this.sep().status]);

  protected readonly facts = computed<ReviewerFact[]>(() => {
    const sep = this.sep();
    const overThreshold = this.overThreshold();
    const daysToEvent = Math.round((new Date(sep.eventStart).getTime() - Date.now()) / (1000 * 60 * 60 * 24));
    return [
      {
        label: 'Event date',
        value: `${formatEventDate(sep)} (${daysToEvent < 0 ? `${Math.abs(daysToEvent)} days ago` : `${daysToEvent} days away`})`,
      },
      {
        label: 'Expected attendance',
        value: `${sep.expectedAttendance.toLocaleString()} (${
          overThreshold ? `over the ${AUTO_APPROVAL_THRESHOLD} review threshold` : `under the ${AUTO_APPROVAL_THRESHOLD} threshold`
        })`,
      },
      { label: 'Event type', value: sep.eventType },
      { label: 'Venue', value: `${sep.venueName}, ${sep.city}` },
      { label: 'Assignment', value: sep.assignedOfficer ? `Assigned to ${sep.assignedOfficer}` : 'Unassigned' },
    ];
  });
}

@Component({
  selector: 'police-queue-page',
  imports: [
    BCGovHeader,
    BCGovFooter,
    ...PAGE_HERO,
    ...CARD,
    ...SELECT,
    Input,
    LucideIcon,
    WorkloadSuggestions,
    PoliceQueueCard,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="min-h-screen flex flex-col bg-background">
      <bc-gov-header role="police" currentPage="/police/queue" />

      <main class="flex-1">
        <page-hero
          [breadcrumbs]="[{ label: 'Special Event Permit Review', href: path('police.home') }, { label: 'Review Queue' }]"
          title="Review Queue"
          description="Search and filter Special Event Permit applications by event, applicant, location, or date."
        />

        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
          <!-- AI Suggest: triage the current queue view -->
          <div class="mb-6">
            <workload-suggestions scope="police" [stats]="workloadStats()" [items]="workloadItems()" [links]="workloadLinks()" />
          </div>

          <!-- Search + filters -->
          <div class="flex flex-col lg:flex-row gap-3 mb-6">
            <div class="relative flex-1">
              <svg lucide="Search" class="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground"></svg>
              <input
                uiInput
                placeholder="Search by event name, applicant, venue, address, or permit #..."
                [value]="query()"
                (input)="query.set($any($event.target).value)"
                class="pl-10"
              />
            </div>
            <ui-select [value]="statusFilter()" (valueChange)="statusFilter.set($event ?? '')">
              <button uiSelectTrigger class="w-full lg:w-[200px]"><span uiSelectValue placeholder="View"></span></button>
              <ng-template uiSelectContent>
                <ui-select-item value="needs-review">Needs review</ui-select-item>
                <ui-select-item value="unassigned">Unassigned</ui-select-item>
                <ui-select-item value="mine">My jobs</ui-select-item>
                <ui-select-item value="all">All applications</ui-select-item>
                @for (s of statuses; track s) {
                  <ui-select-item [value]="s">{{ statusLabels[s] }}</ui-select-item>
                }
              </ng-template>
            </ui-select>
            <ui-select [value]="sortBy()" (valueChange)="sortBy.set($event ?? '')">
              <button uiSelectTrigger class="w-full lg:w-[180px]"><span uiSelectValue placeholder="Sort by"></span></button>
              <ng-template uiSelectContent>
                <ui-select-item value="event-date">Event date</ui-select-item>
                <ui-select-item value="submitted">Recently submitted</ui-select-item>
                <ui-select-item value="attendance">Attendance (high–low)</ui-select-item>
                <ui-select-item value="name">Event name (A–Z)</ui-select-item>
              </ng-template>
            </ui-select>
          </div>

          <p class="text-sm text-muted-foreground mb-4">Showing {{ filtered().length }} application{{ filtered().length === 1 ? '' : 's' }}</p>

          <div class="space-y-3">
            @for (sep of filtered(); track sep.id) {
              <div uiCard policeQueueCard class="hover:shadow-md transition-shadow" [sep]="sep" (assign)="assignToMe(sep.id)"></div>
            }
            @if (filtered().length === 0) {
              <div uiCard>
                <div uiCardContent class="py-12 text-center text-muted-foreground">No applications match your search.</div>
              </div>
            }
          </div>
        </div>
      </main>

      <bc-gov-footer />
    </div>
  `,
})
export class PoliceQueuePage {
  protected readonly path = path;
  private readonly specialEventsRepository = inject(SPECIAL_EVENTS_REPOSITORY);
  private readonly store = this.specialEventsRepository.store();
  private readonly sessionProvider = injectSession();
  private readonly officerName = computed(() => actorName(this.sessionProvider.session()));

  protected readonly statuses = Object.keys(SEP_STATUS_LABELS) as SepStatus[];
  protected readonly statusLabels = SEP_STATUS_LABELS;

  protected readonly query = signal('');
  protected readonly statusFilter = signal<string>(inject(ActivatedRoute).snapshot.queryParamMap.get('filter') ?? 'needs-review');
  protected readonly sortBy = signal('event-date');

  protected readonly filtered = computed(() => {
    let list = [...this.store.records()];
    const statusFilter = this.statusFilter();

    // View filters
    if (statusFilter === 'needs-review') {
      list = list.filter((r) => ['submitted', 'under-review', 'changes-requested'].includes(r.status));
    } else if (statusFilter === 'unassigned') {
      list = list.filter((r) => r.status === 'submitted' && !r.assignedOfficer);
    } else if (statusFilter === 'mine') {
      list = list.filter((r) => r.assignedOfficer === this.officerName());
    } else if (statusFilter !== 'all') {
      list = list.filter((r) => r.status === (statusFilter as SepStatus));
    }

    // Text search across event name, applicant, venue, city, address, id
    const q = this.query().trim().toLowerCase();
    if (q) {
      list = list.filter((r) =>
        [r.eventName, r.applicantName, r.applicantOrg ?? '', r.venueName, r.address, r.city, r.id].join(' ').toLowerCase().includes(q),
      );
    }

    // Sort
    const sortBy = this.sortBy();
    list.sort((a, b) => {
      switch (sortBy) {
        case 'event-date':
          return a.eventStart.localeCompare(b.eventStart);
        case 'submitted':
          return b.submittedDate.localeCompare(a.submittedDate);
        case 'attendance':
          return b.expectedAttendance - a.expectedAttendance;
        case 'name':
          return a.eventName.localeCompare(b.eventName);
        default:
          return 0;
      }
    });

    return list;
  });

  protected readonly workloadStats = computed(() => [{ label: 'In this view', value: String(this.filtered().length) }]);
  protected readonly workloadItems = computed(() =>
    this.filtered()
      .slice(0, 10)
      .map((r) => ({
        label: r.eventName,
        detail: `${SEP_STATUS_LABELS[r.status]}; ${formatEventDate(r)}; ${r.city}; ${r.expectedAttendance.toLocaleString()} expected${
          r.expectedAttendance >= AUTO_APPROVAL_THRESHOLD ? ' (over review threshold)' : ''
        }; ${r.assignedOfficer ? `assigned to ${r.assignedOfficer}` : 'unassigned'}`,
      })),
  );
  protected readonly workloadLinks = computed(() =>
    this.filtered()
      .slice(0, 5)
      .map((r) => ({ label: `Review: ${r.eventName}`, href: path('police.review', { id: r.id }) })),
  );

  protected assignToMe(id: string): void {
    this.store.assignOfficer(id, this.officerName());
  }
}
