/** Local Government review queue. The initial status filter is read once from `?status=`, like the prototype's `useState(initialStatus)`; SortHeader sits on the real `th`. */
import { ChangeDetectionStrategy, Component, computed, inject, input, output, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ReviewerSuggestedActions, type ReviewerFact } from '@shared/ui/ai/reviewer-suggested-actions';
import { BCGovFooter } from '@core/layout/footer';
import { BCGovHeader } from '@core/layout/header';
import { LgStatusBadge } from '../components/lg-status-badge';
import { PAGE_HERO } from '@shared/ui/bc-gov/page-hero';
import { CommentingOffBanner } from '../components/commenting-off-banner';
import { ReviewQueueTools } from '../components/review-queue-tools';
import { Button } from '@shared/ui/primitives/button';
import { CARD } from '@shared/ui/primitives/card';
import { Input } from '@shared/ui/primitives/input';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { SELECT } from '@shared/ui/primitives/select';
import { daysUntilDue } from '@domain/lg-review/due-dates';
import { type LgApplication, LG_STATUS_LABELS, type LgReviewStatus } from '@domain/models/lg-review';
import { formatLgDate } from '@domain/lg-review/due-dates';
import type { LgReportFilters } from '@domain/models/lg-review';
import { LG_REVIEW_REPOSITORY } from '@data-access/repositories/lg-review/lg-review.repository';
import { path } from '@/app/route-registry';

type SortKey = 'establishment' | 'submitted' | 'applicant' | 'status' | 'due';
type SortDir = 'asc' | 'desc';

function lgReviewerFacts(app: LgApplication): ReviewerFact[] {
  const days = daysUntilDue(app.commentDueDate);
  const commentSubmitted = ['no-comment', 'reject', 'completed'].includes(app.status);
  const dueValue = commentSubmitted
    ? 'Comment already submitted'
    : days < 0
      ? `${Math.abs(days)} day${Math.abs(days) === 1 ? '' : 's'} overdue (${formatLgDate(app.commentDueDate)})`
      : `${days} day${days === 1 ? '' : 's'} to comment (${formatLgDate(app.commentDueDate)})`;
  return [
    { label: 'Application type', value: app.applicationType },
    { label: 'Establishment', value: `${app.establishmentName}, ${app.city}` },
    { label: 'Licence type', value: app.licenceType },
    { label: 'Applicant / business', value: `${app.applicantName} (${app.businessName})` },
    { label: 'Submitted', value: formatLgDate(app.submittedDate) },
    { label: 'Comment deadline', value: dueValue },
  ];
}

/** On the real `th`. */
@Component({
  selector: 'th[lgSortHeader]',
  imports: [LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { class: 'px-4 py-3 font-medium text-muted-foreground' },
  template: `
    <button
      type="button"
      (click)="sort.emit()"
      class="inline-flex items-center gap-1.5 hover:text-foreground transition-colors"
      [attr.aria-label]="'Sort by ' + label()"
    >
      {{ label() }}
      @if (active()) {
        @if (dir() === 'asc') {
          <svg lucide="ArrowUp" class="h-3.5 w-3.5"></svg>
        } @else {
          <svg lucide="ArrowDown" class="h-3.5 w-3.5"></svg>
        }
      } @else {
        <svg lucide="ArrowUpDown" class="h-3.5 w-3.5 opacity-40"></svg>
      }
    </button>
  `,
})
export class LgSortHeader {
  readonly label = input.required<string>();
  readonly active = input.required<boolean>();
  readonly dir = input.required<SortDir>();
  readonly sort = output<void>();
}

@Component({
  selector: 'lg-due-label',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (status() === 'cancelled') {
      <span class="text-xs text-muted-foreground">No action needed</span>
    } @else if (submitted()) {
      <span class="text-xs text-muted-foreground">Comment submitted</span>
    } @else {
      <span [attr.class]="'text-xs ' + color()">{{ text() }}</span>
    }
  `,
})
export class LgDueLabel {
  readonly days = input.required<number>();
  readonly status = input.required<LgReviewStatus>();

  protected readonly submitted = computed(() => ['no-comment', 'reject', 'completed'].includes(this.status()));
  protected readonly color = computed(() => {
    const days = this.days();
    return days < 0 ? 'text-red-700 font-medium' : days >= 0 && days <= 14 ? 'text-orange-600 font-medium' : 'text-muted-foreground';
  });
  protected readonly text = computed(() => {
    const days = this.days();
    return days < 0
      ? `${Math.abs(days)} day${Math.abs(days) === 1 ? '' : 's'} overdue`
      : `${days} day${days === 1 ? '' : 's'} to comment`;
  });
}

@Component({
  selector: 'local-government-queue-page',
  imports: [
    RouterLink,
    BCGovHeader,
    BCGovFooter,
    ...PAGE_HERO,
    ...CARD,
    ...SELECT,
    Button,
    Input,
    LucideIcon,
    LgStatusBadge,
    CommentingOffBanner,
    ReviewQueueTools,
    ReviewerSuggestedActions,
    LgSortHeader,
    LgDueLabel,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="min-h-screen flex flex-col bg-background">
      <bc-gov-header role="local-government" currentPage="/local-government/queue" />

      <main class="flex-1">
        <page-hero
          [breadcrumbs]="[{ label: 'Local Government Review', href: path('lg.home') }, { label: 'Applications Awaiting Review' }]"
          title="Applications Awaiting Review"
          description="Applications routed to your jurisdiction for comment. Sort by clicking a column heading, or search by job number, applicant, business, or establishment name."
        />

        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
          <!-- Shown only while the jurisdiction has opted out of commenting -->
          <commenting-off-banner className="mb-6" />

          <!-- Search + filter -->
          <div class="flex flex-col lg:flex-row gap-3 mb-6">
            <div class="relative flex-1">
              <svg lucide="Search" class="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground"></svg>
              <input
                uiInput
                placeholder="Search by job #, applicant, business, or establishment..."
                [value]="query()"
                (input)="query.set($any($event.target).value)"
                class="pl-10"
              />
            </div>
            <ui-select [value]="statusFilter()" (valueChange)="statusFilter.set($any($event))">
              <button uiSelectTrigger class="w-full lg:w-[220px]"><span uiSelectValue placeholder="Status"></span></button>
              <ng-template uiSelectContent>
                <ui-select-item value="all">All statuses</ui-select-item>
                <ui-select-item value="pending-review">Pending Review</ui-select-item>
                <ui-select-item value="documents-requested">Documents Requested</ui-select-item>
                <ui-select-item value="community-review">Community Review</ui-select-item>
                <ui-select-item value="completed">Comment submitted</ui-select-item>
                <ui-select-item value="cancelled">Cancelled by applicant</ui-select-item>
              </ng-template>
            </ui-select>
            @if (query() || statusFilter() !== 'all') {
              <button uiButton variant="outline" (click)="clearAll()">Clear all</button>
            }
          </div>

          <review-queue-tools
            [filteredApplications]="filtered()"
            [allApplications]="applications()"
            [filters]="scheduleFilters()"
            [filterSummary]="filterSummary()"
          />

          <p class="text-sm text-muted-foreground mb-4">Showing {{ filtered().length }} application{{ filtered().length === 1 ? '' : 's' }}</p>

          <!-- Desktop table -->
          <div uiCard class="hidden md:block overflow-hidden">
            <div class="overflow-x-auto">
              <table class="w-full text-sm">
                <thead>
                  <tr class="border-b bg-muted/50 text-left">
                    <th lgSortHeader label="Application / Submit Date" [active]="sortKey() === 'submitted'" [dir]="sortDir()" (sort)="toggleSort('submitted')"></th>
                    <th lgSortHeader label="Applicant / Business" [active]="sortKey() === 'applicant'" [dir]="sortDir()" (sort)="toggleSort('applicant')"></th>
                    <th lgSortHeader label="Establishment" [active]="sortKey() === 'establishment'" [dir]="sortDir()" (sort)="toggleSort('establishment')"></th>
                    <th lgSortHeader label="Status / Due" [active]="sortKey() === 'status'" [dir]="sortDir()" (sort)="toggleSort('status')"></th>
                    <th class="px-4 py-3 font-medium text-muted-foreground text-right">Action</th>
                  </tr>
                </thead>
                <tbody class="divide-y">
                  @for (row of rows(); track row.app.jobNumber) {
                    <tr class="hover:bg-muted/30 transition-colors align-top">
                      <td class="px-4 py-3">
                        <div class="font-medium text-[#013366]">{{ row.app.applicationType }}</div>
                        <div class="text-muted-foreground">Job #{{ row.app.jobNumber }}</div>
                        <div class="text-muted-foreground">{{ formatLgDate(row.app.submittedDate) }}</div>
                      </td>
                      <td class="px-4 py-3">
                        <div class="text-foreground">{{ row.app.applicantName }}</div>
                        <div class="text-muted-foreground">{{ row.app.businessName }}</div>
                      </td>
                      <td class="px-4 py-3">
                        <a [routerLink]="path('lg.review', { id: row.app.jobNumber })" class="font-medium text-[#013366] hover:underline">{{ row.app.establishmentName }}</a>
                        <div class="text-muted-foreground">{{ row.app.address }}</div>
                        <div class="text-muted-foreground">{{ row.app.city }} · {{ row.app.licenceType }}</div>
                      </td>
                      <td class="px-4 py-3">
                        <lg-status-badge [status]="row.app.status" />
                        <div class="mt-1">
                          <lg-due-label [days]="row.days" [status]="row.app.status" />
                        </div>
                      </td>
                      <td class="px-4 py-3 text-right">
                        <div class="flex flex-col items-end gap-2">
                          <a uiButton size="sm" class="bg-[#013366] hover:bg-[#012a52] gap-1" [routerLink]="path('lg.review', { id: row.app.jobNumber })">
                            Review
                            <svg lucide="ArrowRight" class="h-4 w-4"></svg>
                          </a>
                          <div class="w-full max-w-xs">
                            <reviewer-suggested-actions
                              role="local-government"
                              [itemTitle]="row.app.establishmentName"
                              [itemId]="'Job #' + row.app.jobNumber"
                              [status]="row.statusLabel"
                              [facts]="row.facts"
                              [reviewHref]="path('lg.review', { id: row.app.jobNumber })"
                              [dense]="true"
                            />
                          </div>
                        </div>
                      </td>
                    </tr>
                  }
                </tbody>
              </table>
            </div>
            @if (filtered().length === 0) {
              <div class="py-12 text-center text-muted-foreground">No applications match your search.</div>
            }
          </div>

          <!-- Mobile cards -->
          <div class="md:hidden space-y-3">
            @for (row of rows(); track row.app.jobNumber) {
              <div uiCard class="hover:shadow-md transition-shadow">
                <a [routerLink]="path('lg.review', { id: row.app.jobNumber })" class="block">
                  <div uiCardContent class="p-4">
                    <div class="flex items-start justify-between gap-2">
                      <div>
                        <h3 class="font-semibold text-[#013366]">{{ row.app.establishmentName }}</h3>
                        <p class="text-sm text-muted-foreground">Job #{{ row.app.jobNumber }}</p>
                      </div>
                      <lg-status-badge [status]="row.app.status" />
                    </div>
                    <p class="text-sm text-muted-foreground mt-2">{{ row.app.applicationType }}</p>
                    <p class="text-sm text-muted-foreground">{{ row.app.applicantName }} · {{ row.app.businessName }}</p>
                    <p class="text-sm text-muted-foreground">{{ row.app.licenceType }} · Submitted {{ formatLgDate(row.app.submittedDate) }}</p>
                    <div class="mt-2">
                      <lg-due-label [days]="row.days" [status]="row.app.status" />
                    </div>
                  </div>
                </a>
                <div class="px-4 pb-4">
                  <reviewer-suggested-actions
                    role="local-government"
                    [itemTitle]="row.app.establishmentName"
                    [itemId]="'Job #' + row.app.jobNumber"
                    [status]="row.statusLabel"
                    [facts]="row.facts"
                    [reviewHref]="path('lg.review', { id: row.app.jobNumber })"
                  />
                </div>
              </div>
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
export class LocalGovernmentQueuePage {
  protected readonly path = path;
  private readonly lgReviewRepository = inject(LG_REVIEW_REPOSITORY);
  protected readonly applications = this.lgReviewRepository.store().applications;
  private readonly route = inject(ActivatedRoute);

  protected readonly query = signal('');
  protected readonly statusFilter = signal<string>(this.route.snapshot.queryParamMap.get('status') ?? 'all');
  protected readonly sortKey = signal<SortKey>('submitted');
  protected readonly sortDir = signal<SortDir>('desc');

  protected toggleSort(key: SortKey): void {
    if (this.sortKey() === key) {
      this.sortDir.update((d) => (d === 'asc' ? 'desc' : 'asc'));
    } else {
      this.sortKey.set(key);
      this.sortDir.set('asc');
    }
  }

  protected clearAll(): void {
    this.query.set('');
    this.statusFilter.set('all');
  }

  protected readonly filtered = computed(() => {
    let list = [...this.applications()];
    const statusFilter = this.statusFilter();

    if (statusFilter === 'completed') {
      list = list.filter((a) => ['no-comment', 'reject', 'completed'].includes(a.status));
    } else if (statusFilter !== 'all') {
      list = list.filter((a) => a.status === (statusFilter as LgReviewStatus));
    }

    const q = this.query().trim().toLowerCase();
    if (q) {
      list = list.filter((a) =>
        [a.jobNumber, a.establishmentName, a.applicantName, a.businessName, a.address, a.city, a.licenceType, a.applicationType]
          .join(' ')
          .toLowerCase()
          .includes(q),
      );
    }

    const dir = this.sortDir() === 'asc' ? 1 : -1;
    const sortKey = this.sortKey();
    list.sort((a, b) => {
      switch (sortKey) {
        case 'establishment':
          return a.establishmentName.localeCompare(b.establishmentName) * dir;
        case 'submitted':
          return a.submittedDate.localeCompare(b.submittedDate) * dir;
        case 'applicant':
          return a.applicantName.localeCompare(b.applicantName) * dir;
        case 'status':
          return LG_STATUS_LABELS[a.status].localeCompare(LG_STATUS_LABELS[b.status]) * dir;
        case 'due':
          return a.commentDueDate.localeCompare(b.commentDueDate) * dir;
        default:
          return 0;
      }
    });

    return list;
  });

  /** Per-row derived values (days to deadline, status label, AI facts), built once per change. */
  protected readonly rows = computed(() =>
    this.filtered().map((app) => ({
      app,
      days: daysUntilDue(app.commentDueDate),
      statusLabel: LG_STATUS_LABELS[app.status],
      facts: lgReviewerFacts(app),
    })),
  );

  // Capture active filters for the deadline tools. "completed" and "all" become "all" since scheduled digests only cover applications awaiting comment.
  private readonly scheduleStatus = computed<LgReportFilters['status']>(() => {
    const statusFilter = this.statusFilter();
    return ['pending-review', 'documents-requested', 'community-review'].includes(statusFilter)
      ? (statusFilter as LgReviewStatus)
      : 'all';
  });
  protected readonly scheduleFilters = computed<LgReportFilters>(() => ({ status: this.scheduleStatus(), query: this.query() }));
  protected readonly filterSummary = computed(() => {
    const scheduleStatus = this.scheduleStatus();
    const query = this.query();
    return [scheduleStatus === 'all' ? 'All statuses' : LG_STATUS_LABELS[scheduleStatus], query.trim() ? `Search: "${query.trim()}"` : null]
      .filter(Boolean)
      .join('  ·  ');
  });

  protected readonly formatLgDate = formatLgDate;
}
