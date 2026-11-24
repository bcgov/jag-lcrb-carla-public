/** The applicant's Special Event Permits list. */
import { ChangeDetectionStrategy, Component, afterNextRender, computed, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { toast } from '@spartan-ng/brain/sonner';
import { CancelApplicationDialog } from '@shared/ui/applications/cancel-application-dialog';
import { BCGovFooter } from '@core/layout/footer';
import { BCGovHeader } from '@core/layout/header';
import { type HeroCrumb } from '@shared/ui/bc-gov/hero-breadcrumb';
import { PAGE_HERO } from '@shared/ui/bc-gov/page-hero';
import { SepStatusBadge } from '@shared/ui/bc-gov/sep-status-badge';
import { injectSession } from '@core/session/session-provider';
import { SepAccessGuard } from '@core/permissions/sep-access-guard';
import { Button } from '@shared/ui/primitives/button';
import { CARD } from '@shared/ui/primitives/card';
import { DROPDOWN_MENU } from '@shared/ui/primitives/dropdown-menu';
import { Input } from '@shared/ui/primitives/input';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { SELECT } from '@shared/ui/primitives/select';
import { StatTile, StatTileRow } from '@shared/ui/primitives/stat-tile';
import { TABLE } from '@shared/ui/primitives/table';
import { TABS } from '@shared/ui/primitives/tabs';
import { type SepRecord, type SepStatus } from '@domain/models/special-events';
import { actorName } from '@domain/roles';
import { downloadSepPdf } from '@shared/util/pdf/sep-pdf';
import { SPECIAL_EVENTS_REPOSITORY } from '@data-access/repositories/special-events/special-events.repository';
import { APPLICATIONS_REPOSITORY } from '@data-access/repositories/applications/applications.repository';
import { path } from '@/app/route-registry';

@Component({
  selector: 'special-events-page',
  imports: [
    RouterLink,
    SepAccessGuard,
    BCGovHeader,
    BCGovFooter,
    ...PAGE_HERO,
    ...CARD,
    ...DROPDOWN_MENU,
    ...SELECT,
    ...TABLE,
    ...TABS,
    Button,
    Input,
    LucideIcon,
    StatTile,
    StatTileRow,
    SepStatusBadge,
    CancelApplicationDialog,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <sep-access-guard>
      <div class="min-h-screen flex flex-col bg-background">
        <bc-gov-header />

        <main class="flex-1">
          <!-- Hero Section -->
          <page-hero
            containerClassName="container mx-auto px-4"
            [breadcrumbs]="breadcrumbs"
            title="Special Event Permits"
            description="Apply for and manage permits for events where liquor will be served"
          >
            <a heroActions [routerLink]="path('special-events.apply')">
              <button uiButton class="bg-bc-gold hover:bg-bc-gold/90 text-bc-blue font-semibold">
                <svg lucide="Plus" class="h-4 w-4 mr-2"></svg>
                Start New Application
              </button>
            </a>
          </page-hero>

          <div class="container mx-auto px-4 py-8">
            @if (submittedRecord(); as sr) {
              <div
                role="status"
                [attr.class]="
                  'mb-6 flex items-start gap-3 rounded-lg border p-4 ' +
                  (wasAutoApproved() ? 'border-green-200 bg-green-50 text-green-900' : 'border-blue-200 bg-blue-50 text-blue-900')
                "
              >
                <svg lucide="CheckCircle2" class="mt-0.5 h-5 w-5 shrink-0"></svg>
                <div>
                  <p class="font-semibold">
                    {{ wasAutoApproved() ? 'Application ' + sr.id + ' approved instantly' : 'Application ' + sr.id + ' submitted for review' }}
                  </p>
                  <p class="text-sm">
                    {{
                      wasAutoApproved()
                        ? 'No referral triggers applied, so standard conditions were added automatically. Download your permit below.'
                        : "Your application has been routed for review. You'll be notified of the decision."
                    }}
                  </p>
                  @if (!wasAutoApproved() && sr.referralReasons && sr.referralReasons.length > 0) {
                    <ul class="mt-2 list-disc space-y-1 pl-5 text-sm">
                      @for (reason of sr.referralReasons; track reason) {
                        <li>{{ reason }}</li>
                      }
                    </ul>
                  }
                </div>
              </div>
            }
            <div class="grid grid-cols-1 lg:grid-cols-4 gap-6">
              <!-- Main Content -->
              <div class="lg:col-span-3 space-y-6">
                <!-- Quick Stats: each tile filters the table below. -->
                <ui-stat-tile-row class="mb-0">
                  <ui-stat-tile
                    icon="CheckCircle2"
                    tone="bg-green-100 text-green-600"
                    [value]="approvedCount()"
                    label="Approved"
                    hint="Show approved permits"
                    [active]="activeTab() === 'submitted' && statusFilter() === 'approved'"
                    (tileClick)="showStatus('approved')"
                  />
                  <ui-stat-tile
                    icon="Clock"
                    tone="bg-yellow-100 text-yellow-600"
                    [value]="pendingCount()"
                    label="Pending"
                    hint="Show permits under review"
                    [active]="activeTab() === 'submitted' && statusFilter() === 'pending'"
                    (tileClick)="showStatus('pending')"
                  />
                  <ui-stat-tile
                    icon="FileText"
                    tone="bg-gray-100 text-gray-600"
                    [value]="sepDrafts().length"
                    label="Drafts"
                    hint="Show saved drafts"
                    [active]="activeTab() === 'drafts'"
                    (tileClick)="activeTab.set('drafts')"
                  />
                  <ui-stat-tile
                    icon="Calendar"
                    tone="bg-blue-100 text-blue-600"
                    [value]="upcomingCount()"
                    label="Upcoming events"
                    hint="Open the compliance calendar"
                    [href]="path('dashboard.calendar')"
                  />
                </ui-stat-tile-row>

                <!-- Applications Tabs -->
                <div uiCard>
                  <div uiCardHeader class="pb-3">
                    <div class="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
                      <div uiCardTitle>My Applications</div>
                      <div class="flex flex-col sm:flex-row gap-2">
                        <div class="relative">
                          <svg lucide="Search" class="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground"></svg>
                          <input
                            uiInput
                            placeholder="Search applications..."
                            [value]="searchQuery()"
                            (input)="searchQuery.set($any($event.target).value)"
                            class="pl-9 w-full sm:w-64"
                          />
                        </div>
                        <ui-select [value]="statusFilter()" (valueChange)="statusFilter.set($event ?? '')">
                          <button uiSelectTrigger class="w-full sm:w-40">
                            <span uiSelectValue placeholder="Filter by status"></span>
                          </button>
                          <ng-template uiSelectContent>
                            <ui-select-item value="all">All Statuses</ui-select-item>
                            <ui-select-item value="approved">Approved</ui-select-item>
                            <ui-select-item value="pending">Pending</ui-select-item>
                            <ui-select-item value="rejected">Rejected</ui-select-item>
                            <ui-select-item value="cancelled">Cancelled</ui-select-item>
                          </ng-template>
                        </ui-select>
                      </div>
                    </div>
                  </div>
                  <div uiCardContent>
                    <div uiTabs [value]="activeTab()" (valueChange)="activeTab.set($any($event))" class="w-full">
                      <div uiTabsList class="mb-4">
                        <button uiTabsTrigger value="submitted">Submitted ({{ submittedApps().length }})</button>
                        <button uiTabsTrigger value="drafts">Drafts ({{ sepDrafts().length }})</button>
                      </div>

                      <div uiTabsContent value="submitted">
                        <div class="rounded-md border overflow-x-auto">
                          <ui-table>
                            <thead uiTableHeader>
                              <tr uiTableRow>
                                <th uiTableHead>Event Name</th>
                                <th uiTableHead>Date</th>
                                <th uiTableHead class="hidden md:table-cell">Location</th>
                                <th uiTableHead class="hidden sm:table-cell">Guests</th>
                                <th uiTableHead>Status</th>
                                <th uiTableHead class="w-12"></th>
                              </tr>
                            </thead>
                            <tbody uiTableBody>
                              @if (submittedApps().length === 0) {
                                <tr uiTableRow>
                                  <td uiTableCell colspan="6" class="py-8 text-center text-muted-foreground">No applications match your search.</td>
                                </tr>
                              }
                              @for (app of submittedApps(); track app.id) {
                                <tr uiTableRow>
                                  <td uiTableCell>
                                    <div>
                                      <p class="font-medium">{{ app.eventName }}</p>
                                      <p class="text-sm text-muted-foreground">{{ app.id }}</p>
                                    </div>
                                  </td>
                                  <td uiTableCell>
                                    <div class="flex items-center gap-1">
                                      <svg lucide="Calendar" class="h-4 w-4 text-muted-foreground"></svg>
                                      <span class="text-sm">{{ shortDate(app.startDate) }}</span>
                                    </div>
                                  </td>
                                  <td uiTableCell class="hidden md:table-cell">
                                    <div class="flex items-center gap-1">
                                      <svg lucide="MapPin" class="h-4 w-4 text-muted-foreground"></svg>
                                      <span class="text-sm truncate max-w-48">{{ app.location }}</span>
                                    </div>
                                  </td>
                                  <td uiTableCell class="hidden sm:table-cell">
                                    <div class="flex items-center gap-1">
                                      <svg lucide="Users" class="h-4 w-4 text-muted-foreground"></svg>
                                      <span class="text-sm">{{ app.guests.toLocaleString() }}</span>
                                    </div>
                                  </td>
                                  <td uiTableCell>
                                    <sep-status-badge [status]="app.status" />
                                  </td>
                                  <td uiTableCell>
                                    <button uiButton variant="ghost" size="icon" class="h-8 w-8" [uiDropdownMenuTrigger]="appMenu" align="end">
                                      <svg lucide="MoreHorizontal" class="h-4 w-4"></svg>
                                      <span class="sr-only">Actions</span>
                                    </button>
                                    <ng-template #appMenu>
                                      <div uiDropdownMenuContent>
                                        <div uiDropdownMenuItem (triggered)="go(path('special-events.detail', { id: app.id }))">
                                          <svg lucide="Eye" class="h-4 w-4 mr-2"></svg>
                                          View Details
                                        </div>
                                        @if (app.status === 'approved' || app.status === 'auto-approved') {
                                          <div uiDropdownMenuItem (triggered)="handleDownloadPermit(app.id)">
                                            <svg lucide="Download" class="h-4 w-4 mr-2"></svg>
                                            Download Permit
                                          </div>
                                        }
                                        <div uiDropdownMenuItem (triggered)="go(path('special-events.apply'))">
                                          <svg lucide="Copy" class="h-4 w-4 mr-2"></svg>
                                          Copy for New Event
                                        </div>
                                        @if (app.status === 'submitted' || app.status === 'under-review' || app.status === 'changes-requested') {
                                          <div uiDropdownMenuSeparator></div>
                                          <div uiDropdownMenuItem class="text-red-600" (triggered)="chooseCancelTarget(app.id)">
                                            <svg lucide="Trash2" class="h-4 w-4 mr-2"></svg>
                                            Cancel Permit
                                          </div>
                                        }
                                      </div>
                                    </ng-template>
                                  </td>
                                </tr>
                              }
                            </tbody>
                          </ui-table>
                        </div>
                      </div>

                      <div uiTabsContent value="drafts">
                        <div class="rounded-md border overflow-x-auto">
                          <ui-table>
                            <thead uiTableHeader>
                              <tr uiTableRow>
                                <th uiTableHead>Event Name</th>
                                <th uiTableHead>Date</th>
                                <th uiTableHead class="hidden md:table-cell">Location</th>
                                <th uiTableHead>Progress</th>
                                <th uiTableHead class="w-12"></th>
                              </tr>
                            </thead>
                            <tbody uiTableBody>
                              @if (sepDrafts().length === 0) {
                                <tr uiTableRow>
                                  <td uiTableCell colspan="5" class="py-8 text-center text-muted-foreground">
                                    No saved drafts. Start an application and use
                                    <span class="font-medium">Save &amp; Continue Later</span> to resume it here.
                                  </td>
                                </tr>
                              }
                              @for (draft of sepDrafts(); track draft.draftId) {
                                <tr uiTableRow>
                                  <td uiTableCell>
                                    <div>
                                      <p class="font-medium">{{ draftName(draft.formData) }}</p>
                                      <p class="text-sm text-muted-foreground">Last modified: {{ localDate(draft.updatedAt) }}</p>
                                    </div>
                                  </td>
                                  <td uiTableCell>
                                    <div class="flex items-center gap-1">
                                      <svg lucide="Calendar" class="h-4 w-4 text-muted-foreground"></svg>
                                      <span class="text-sm">{{ draftDate(draft.formData) }}</span>
                                    </div>
                                  </td>
                                  <td uiTableCell class="hidden md:table-cell">
                                    <div class="flex items-center gap-1">
                                      <svg lucide="MapPin" class="h-4 w-4 text-muted-foreground"></svg>
                                      <span class="text-sm truncate max-w-48">{{ draftLocation(draft.formData) }}</span>
                                    </div>
                                  </td>
                                  <td uiTableCell>
                                    <div class="flex items-center gap-2">
                                      <div class="w-20 h-2 bg-gray-200 rounded-full overflow-hidden">
                                        <div class="h-full bg-bc-blue rounded-full" [style.width]="draft.progress + '%'"></div>
                                      </div>
                                      <span class="text-sm text-muted-foreground">{{ draft.progress }}%</span>
                                    </div>
                                  </td>
                                  <td uiTableCell>
                                    <button uiButton variant="ghost" size="icon" class="h-8 w-8" [uiDropdownMenuTrigger]="draftMenu" align="end">
                                      <svg lucide="MoreHorizontal" class="h-4 w-4"></svg>
                                      <span class="sr-only">Actions</span>
                                    </button>
                                    <ng-template #draftMenu>
                                      <div uiDropdownMenuContent>
                                        <a uiDropdownMenuItem [routerLink]="tree(draft.resumeHref)" class="cursor-pointer">
                                          <svg lucide="Edit" class="h-4 w-4 mr-2"></svg>
                                          Continue Editing
                                        </a>
                                        <div uiDropdownMenuSeparator></div>
                                        <div uiDropdownMenuItem class="text-red-600" (triggered)="deleteDraft(draft.draftId)">
                                          <svg lucide="Trash2" class="h-4 w-4 mr-2"></svg>
                                          Delete Draft
                                        </div>
                                      </div>
                                    </ng-template>
                                  </td>
                                </tr>
                              }
                            </tbody>
                          </ui-table>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>

                <!-- Information Notice -->
                <div uiCard class="border-blue-200 bg-blue-50">
                  <div uiCardContent class="p-4">
                    <div class="flex gap-3">
                      <svg lucide="AlertCircle" class="h-5 w-5 text-blue-600 shrink-0 mt-0.5"></svg>
                      <div class="text-sm">
                        <p class="font-medium text-blue-900">Most applications are approved instantly</p>
                        <p class="text-blue-700 mt-1">
                          An application will require review if: over 500 guests are expected, you have more than 24 permits per year,
                          your event ends after 10 p.m. (outdoor) or 2 a.m. (indoor), or you are charging over cost recovery price.
                        </p>
                      </div>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Sidebar - Resources -->
              <div class="space-y-4">
                <div uiCard>
                  <div uiCardHeader class="pb-3">
                    <div uiCardTitle class="text-lg">Resources</div>
                  </div>
                  <div uiCardContent class="space-y-3">
                    <a
                      href="https://www2.gov.bc.ca/gov/content/employment-business/business/liquor-regulation-licensing/liquor-licence-permits/apply-for-liquor-licence-permit/apply-for-liquor-special-event-permit"
                      target="_blank"
                      rel="noopener noreferrer"
                      class="flex items-center justify-between p-3 rounded-lg border hover:bg-muted transition-colors"
                    >
                      <div class="flex items-center gap-3">
                        <svg lucide="BookOpen" class="h-5 w-5 text-bc-blue"></svg>
                        <div>
                          <p class="font-medium text-sm">Application Checklist</p>
                          <p class="text-xs text-muted-foreground">What you need before starting</p>
                        </div>
                      </div>
                      <svg lucide="ExternalLink" class="h-4 w-4 text-muted-foreground"></svg>
                    </a>

                    <a
                      href="https://www2.gov.bc.ca/gov/content/employment-business/business/liquor-regulation-licensing/liquor-licence-permits/liquor-resources-information/serving-it-right"
                      target="_blank"
                      rel="noopener noreferrer"
                      class="flex items-center justify-between p-3 rounded-lg border hover:bg-muted transition-colors"
                    >
                      <div class="flex items-center gap-3">
                        <svg lucide="FileText" class="h-5 w-5 text-bc-blue"></svg>
                        <div>
                          <p class="font-medium text-sm">Get Certified</p>
                          <p class="text-xs text-muted-foreground">Serving It Right certification</p>
                        </div>
                      </div>
                      <svg lucide="ExternalLink" class="h-4 w-4 text-muted-foreground"></svg>
                    </a>

                    <a
                      [routerLink]="path('special-events.servings-calculator')"
                      class="flex items-center justify-between p-3 rounded-lg border hover:bg-muted transition-colors"
                    >
                      <div class="flex items-center gap-3">
                        <svg lucide="Calendar" class="h-5 w-5 text-bc-blue"></svg>
                        <div>
                          <p class="font-medium text-sm">Plan Your Drinks</p>
                          <p class="text-xs text-muted-foreground">Event planning guide</p>
                        </div>
                      </div>
                      <svg lucide="ArrowRight" class="h-4 w-4 text-muted-foreground"></svg>
                    </a>

                    <a
                      [routerLink]="path('special-events.servings-calculator')"
                      class="flex items-center justify-between p-3 rounded-lg border hover:bg-muted transition-colors"
                    >
                      <div class="flex items-center gap-3">
                        <svg lucide="Calculator" class="h-5 w-5 text-bc-blue"></svg>
                        <div>
                          <p class="font-medium text-sm">Servings Calculator</p>
                          <p class="text-xs text-muted-foreground">Estimate standard servings</p>
                        </div>
                      </div>
                      <svg lucide="ArrowRight" class="h-4 w-4 text-muted-foreground"></svg>
                    </a>

                    <a
                      href="https://www2.gov.bc.ca/gov/content/employment-business/business/liquor-regulation-licensing/liquor-licence-permits/liquor-resources-information/special-event-serve"
                      target="_blank"
                      rel="noopener noreferrer"
                      class="flex items-center justify-between p-3 rounded-lg border hover:bg-muted transition-colors"
                    >
                      <div class="flex items-center gap-3">
                        <svg lucide="HelpCircle" class="h-5 w-5 text-bc-blue"></svg>
                        <div>
                          <p class="font-medium text-sm">Learn More</p>
                          <p class="text-xs text-muted-foreground">SEP policies and FAQs</p>
                        </div>
                      </div>
                      <svg lucide="ExternalLink" class="h-4 w-4 text-muted-foreground"></svg>
                    </a>
                  </div>
                </div>

                <div uiCard class="border-amber-200 bg-amber-50">
                  <div uiCardContent class="p-4">
                    <div class="flex gap-3">
                      <svg lucide="Clock" class="h-5 w-5 text-amber-600 shrink-0"></svg>
                      <div class="text-sm">
                        <p class="font-medium text-amber-900">Submit Early</p>
                        <p class="text-amber-700 mt-1">
                          Applications requiring police review should be submitted at least 30 days before your event.
                        </p>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </main>

        @if (cancelTarget(); as target) {
          <cancel-application-dialog
            [open]="true"
            (openChange)="!$event && cancelTargetId.set(null)"
            noun="permit"
            [subject]="target.eventName + ' (' + target.id + ')'"
            [feePaid]="false"
            [consequence]="
              target.assignedOfficer
                ? target.assignedOfficer + ' (' + target.jurisdiction + ') has started reviewing this permit and will be told it was withdrawn.'
                : target.jurisdiction + ' will be notified so the event is removed from their review queue.'
            "
            (confirmed)="handleCancel($event)"
          />
        }

        <bc-gov-footer />
      </div>
    </sep-access-guard>
  `,
})
export class SpecialEventsPage {
  protected readonly path = path;
  private readonly specialEventsRepository = inject(SPECIAL_EVENTS_REPOSITORY);
  private readonly applicationsRepository = inject(APPLICATIONS_REPOSITORY);
  protected readonly breadcrumbs: HeroCrumb[] = [{ label: 'Dashboard', href: path('dashboard.home') }, { label: 'Special Event Permits' }];

  private readonly router = inject(Router);
  private readonly sepStore = this.specialEventsRepository.store();
  private readonly sessionProvider = injectSession();
  private readonly drafts = this.applicationsRepository.drafts('sep');

  protected readonly searchQuery = signal('');
  protected readonly statusFilter = signal('all');
  protected readonly activeTab = signal<'submitted' | 'drafts'>('submitted');
  protected readonly submittedId = signal<string | null>(null);
  protected readonly sepDrafts = computed(() => this.drafts().drafts);
  /** Permit the applicant is about to withdraw (drives the reason dialog). */
  protected readonly cancelTargetId = signal<string | null>(null);

  private readonly records = this.sepStore.records;

  protected readonly cancelTarget = computed<SepRecord | undefined>(() => {
    const id = this.cancelTargetId();
    return id ? this.records().find((r) => r.id === id) : undefined;
  });

  protected readonly submittedRecord = computed(() => {
    const id = this.submittedId();
    return id ? this.records().find((r) => r.id === id) : undefined;
  });
  protected readonly wasAutoApproved = computed(() => this.submittedRecord()?.status === 'auto-approved');

  constructor() {
    afterNextRender(() => {
      const id = new URLSearchParams(window.location.search).get('submitted');
      if (id) this.submittedId.set(id);
    });
  }

  // The applicant's own SEPs come from the shared store so wizard submissions appear here and reach reviewers; cancelled permits stay listed (greyed badge) but are excluded from the tile counts.
  protected readonly submittedApps = computed(() => {
    const searchQuery = this.searchQuery();
    const statusFilter = this.statusFilter();
    return this.records()
      .map((r) => ({
        id: r.id,
        eventName: r.eventName,
        startDate: r.eventStart,
        location: `${r.venueName}, ${r.city}`,
        guests: r.expectedAttendance,
        status: r.status as SepStatus,
        submittedDate: r.submittedDate,
      }))
      .filter((app) => {
        const matchesSearch =
          searchQuery === '' ||
          app.eventName.toLowerCase().includes(searchQuery.toLowerCase()) ||
          app.id.toLowerCase().includes(searchQuery.toLowerCase());
        const matchesStatus =
          statusFilter === 'all' ||
          (statusFilter === 'approved' && (app.status === 'approved' || app.status === 'auto-approved')) ||
          (statusFilter === 'pending' &&
            (app.status === 'submitted' || app.status === 'under-review' || app.status === 'changes-requested')) ||
          (statusFilter === 'rejected' && app.status === 'denied') ||
          (statusFilter === 'cancelled' && app.status === 'cancelled');
        return matchesSearch && matchesStatus;
      });
  });

  // Counts come from the UNFILTERED set so the tiles stay stable while the user filters the table below them.
  private readonly allActive = computed(() => this.records().filter((r) => r.status !== 'cancelled'));
  protected readonly approvedCount = computed(
    () => this.allActive().filter((a) => a.status === 'approved' || a.status === 'auto-approved').length,
  );
  protected readonly pendingCount = computed(
    () =>
      this.allActive().filter((a) => a.status === 'submitted' || a.status === 'under-review' || a.status === 'changes-requested')
        .length,
  );
  protected readonly upcomingCount = computed(
    () =>
      this.allActive().filter(
        (a) => (a.status === 'approved' || a.status === 'auto-approved') && new Date(a.eventStart).getTime() > Date.now(),
      ).length,
  );

  protected showStatus(status: string): void {
    this.activeTab.set('submitted');
    this.statusFilter.set(status);
  }

  protected go(url: string): void {
    void this.router.navigateByUrl(url);
  }

  /** Parsed so a `?query` or `#fragment` in the href renders as-is. */
  protected tree(href: string) {
    return this.router.parseUrl(href);
  }

  protected shortDate(value: string): string {
    return new Date(value).toLocaleDateString('en-CA', { month: 'short', day: 'numeric' });
  }

  protected localDate(value: string): string {
    return new Date(value).toLocaleDateString();
  }

  protected draftName(formData: Record<string, unknown>): string {
    return (formData['eventName'] as string) || 'Untitled event';
  }

  protected draftDate(formData: Record<string, unknown>): string {
    const date = (formData['eventDate'] as string) || (formData['eventStartDate'] as string) || '';
    return date ? new Date(date).toLocaleDateString('en-CA', { month: 'short', day: 'numeric' }) : '—';
  }

  protected draftLocation(formData: Record<string, unknown>): string {
    return (formData['locationName'] as string) || (formData['eventCity'] as string) || 'Location not set';
  }

  protected deleteDraft(draftId: string): void {
    this.drafts().remove(draftId);
    toast.success('Draft deleted');
  }

  protected handleDownloadPermit(id: string): void {
    const record = this.records().find((r) => r.id === id);
    if (!record) return;
    downloadSepPdf(record, { variant: 'permit' });
    toast.success('Permit downloaded', { description: `${record.eventName} (${record.id})` });
  }

  /** "Cancel Permit" opens the cancel dialog (checked against the reference: unlike /applications, it opens). */
  protected chooseCancelTarget(id: string): void {
    this.cancelTargetId.set(id);
  }

  protected handleCancel(reason: string): void {
    const target = this.cancelTarget();
    if (!target) return;
    this.sepStore.cancel(target.id, actorName(this.sessionProvider.session()), reason);
    toast.success('Permit cancelled', {
      description: `${target.eventName} (${target.id}) has been withdrawn. ${target.jurisdiction} and LCRB have been notified.`,
    });
    this.cancelTargetId.set(null);
  }
}
