/** "My Applications". Quirks kept: search and type filter never filter the lists; Cancel on a draft row removes it with a toast, on a submitted row it opens the dialog. */
import { ChangeDetectionStrategy, Component, computed, inject, input, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { toast } from '@spartan-ng/brain/sonner';
import { CancelApplicationDialog } from '@shared/ui/applications/cancel-application-dialog';
import { DraftSummaryDialog, type DraftSummary } from '../components/draft-summary-dialog';
import { BCGovFooter } from '@core/layout/footer';
import { BCGovHeader } from '@core/layout/header';
import { type HeroCrumb } from '@shared/ui/bc-gov/hero-breadcrumb';
import { PAGE_HERO } from '@shared/ui/bc-gov/page-hero';
import { injectSession } from '@core/session/session-provider';
import { Badge } from '@shared/ui/primitives/badge';
import { Button } from '@shared/ui/primitives/button';
import { CARD } from '@shared/ui/primitives/card';
import { DROPDOWN_MENU } from '@shared/ui/primitives/dropdown-menu';
import type { IconName } from '@shared/ui/primitives/icons';
import { Input } from '@shared/ui/primitives/input';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { SELECT } from '@shared/ui/primitives/select';
import { StatTile, StatTileRow } from '@shared/ui/primitives/stat-tile';
import { TABLE } from '@shared/ui/primitives/table';
import { TABS } from '@shared/ui/primitives/tabs';
import { hrefForApplication } from '@domain/applications/my-applications';
import { type SubmittedApplicationEntry } from '@domain/models/applications';
import { actorName } from '@domain/roles';
import { APPLICATIONS_REPOSITORY } from '@data-access/repositories/applications/applications.repository';
import { path } from '@/app/route-registry';

const statusConfig: Record<string, { label: string; color: string; icon: IconName }> = {
  not_submitted: { label: 'Not Submitted', color: 'bg-gray-100 text-gray-800 border-gray-200', icon: 'FileText' },
  under_review: { label: 'Under Review', color: 'bg-blue-100 text-blue-800 border-blue-200', icon: 'Clock' },
  pending_documents: { label: 'Pending Documents', color: 'bg-yellow-100 text-yellow-800 border-yellow-200', icon: 'AlertCircle' },
  approved: { label: 'Approved', color: 'bg-green-100 text-green-800 border-green-200', icon: 'CheckCircle2' },
  rejected: { label: 'Rejected', color: 'bg-red-100 text-red-800 border-red-200', icon: 'XCircle' },
  cancelled: { label: 'Cancelled', color: 'bg-zinc-100 text-zinc-600 border-zinc-200', icon: 'Ban' },
};

type ActiveTab = 'in-progress' | 'submitted' | 'completed';

@Component({
  selector: 'applications-status-badge',
  imports: [Badge, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <span uiBadge variant="outline" [class]="config().color + ' flex items-center gap-1'">
      <svg [lucide]="config().icon" class="h-3 w-3"></svg>
      {{ config().label }}
    </span>
  `,
})
export class ApplicationsStatusBadge {
  readonly status = input.required<string>();
  protected readonly config = computed(() => statusConfig[this.status()]);
}

@Component({
  selector: 'applications-page',
  imports: [
    RouterLink,
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
    ApplicationsStatusBadge,
    CancelApplicationDialog,
    DraftSummaryDialog,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="min-h-screen flex flex-col bg-background">
      <bc-gov-header />

      <main class="flex-1">
        <!-- Hero Section -->
        <page-hero
          containerClassName="container mx-auto px-4"
          [breadcrumbs]="breadcrumbs"
          title="My Applications"
          description="Track and manage your licence and authorization applications"
        >
          <div heroActions class="flex flex-wrap gap-2">
            <a uiButton variant="outline" class="gap-2 border-white/40 bg-white/10 text-white hover:bg-white/20 hover:text-white" [routerLink]="path('account.receipts')">
              <svg lucide="ReceiptText" class="h-4 w-4"></svg>
              Receipts
            </a>
            <a uiButton class="gap-2 bg-bc-gold font-semibold text-bc-blue hover:bg-bc-gold/90" [routerLink]="path('applications.new')">
              <svg lucide="FilePlus2" class="h-4 w-4"></svg>
              Start a new application
            </a>
          </div>
        </page-hero>

        <div class="container mx-auto px-4 py-8">
          <!-- Quick stats: every tile selects the matching tab below -->
          <ui-stat-tile-row>
            <ui-stat-tile
              icon="FileText"
              tone="bg-gray-100 text-gray-600"
              [value]="apps().inProgress.length"
              label="In Progress"
              hint="Show in-progress applications"
              [active]="activeTab() === 'in-progress' && !actionOnly()"
              (tileClick)="selectTab('in-progress')"
            />
            <ui-stat-tile
              icon="Clock"
              tone="bg-blue-100 text-blue-600"
              [value]="apps().submitted.length"
              label="Under Review"
              hint="Show submitted applications"
              [active]="activeTab() === 'submitted' && !actionOnly()"
              (tileClick)="selectTab('submitted')"
            />
            <ui-stat-tile
              icon="AlertCircle"
              tone="bg-yellow-100 text-yellow-600"
              [value]="actionRequired().length"
              label="Action Required"
              hint="Show applications waiting on you"
              [active]="actionOnly()"
              (tileClick)="selectTab('submitted', true)"
            />
            <ui-stat-tile
              icon="CheckCircle2"
              tone="bg-green-100 text-green-600"
              [value]="apps().completed.length"
              label="Completed"
              hint="Show completed applications"
              [active]="activeTab() === 'completed'"
              (tileClick)="selectTab('completed')"
            />
          </ui-stat-tile-row>

          <!-- Filters -->
          <div uiCard class="mb-6">
            <div uiCardContent class="p-4">
              <div class="flex flex-col md:flex-row gap-4">
                <div class="relative flex-1">
                  <svg lucide="Search" class="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground"></svg>
                  <input
                    uiInput
                    placeholder="Search by application type, reference, or ID..."
                    [value]="searchQuery()"
                    (input)="searchQuery.set($any($event.target).value)"
                    class="pl-9"
                  />
                </div>
                <ui-select [value]="typeFilter()" (valueChange)="typeFilter.set($event ?? '')">
                  <button uiSelectTrigger class="w-full md:w-56">
                    <svg lucide="Filter" class="h-4 w-4 mr-2"></svg>
                    <span uiSelectValue placeholder="Filter by type"></span>
                  </button>
                  <ng-template uiSelectContent>
                    <ui-select-item value="all">All Application Types</ui-select-item>
                    <ui-select-item value="liquor">Liquor Licences</ui-select-item>
                    <ui-select-item value="cannabis">Cannabis Licences</ui-select-item>
                    <ui-select-item value="authorizations">Authorizations</ui-select-item>
                  </ng-template>
                </ui-select>
              </div>
            </div>
          </div>

          <!-- Applications Tabs -->
          <div uiCard>
            <div uiCardContent class="p-6">
              <div uiTabs [value]="activeTab()" (valueChange)="selectTab($any($event))" class="w-full">
                <div uiTabsList class="mb-6">
                  <button uiTabsTrigger value="in-progress">In Progress ({{ apps().inProgress.length }})</button>
                  <button uiTabsTrigger value="submitted">Submitted ({{ apps().submitted.length }})</button>
                  <button uiTabsTrigger value="completed">Completed ({{ apps().completed.length }})</button>
                </div>

                <div uiTabsContent value="in-progress">
                  <div class="rounded-md border overflow-x-auto">
                    <ui-table>
                      <thead uiTableHeader>
                        <tr uiTableRow>
                          <th uiTableHead>Application</th>
                          <th uiTableHead>Status</th>
                          <th uiTableHead class="hidden md:table-cell">Progress</th>
                          <th uiTableHead class="hidden sm:table-cell">Last Modified</th>
                          <th uiTableHead class="w-12"></th>
                        </tr>
                      </thead>
                      <tbody uiTableBody>
                        @for (app of apps().inProgress; track app.id) {
                          <tr uiTableRow>
                            <td uiTableCell>
                              <div>
                                <p class="font-medium">{{ app.type }}</p>
                                <p class="text-sm text-muted-foreground">Job No: {{ app.id }} | Ref: {{ app.reference }}</p>
                              </div>
                            </td>
                            <td uiTableCell>
                              <applications-status-badge [status]="app.status" />
                            </td>
                            <td uiTableCell class="hidden md:table-cell">
                              <div class="flex items-center gap-2">
                                <div class="w-24 h-2 bg-gray-200 rounded-full overflow-hidden">
                                  <div class="h-full bg-bc-blue rounded-full" [style.width]="app.completionPercent + '%'"></div>
                                </div>
                                <span class="text-sm text-muted-foreground">{{ app.completionPercent }}%</span>
                              </div>
                            </td>
                            <td uiTableCell class="hidden sm:table-cell">
                              <span class="text-sm text-muted-foreground">{{ date(app.lastModified) }}</span>
                            </td>
                            <td uiTableCell>
                              <button uiButton variant="ghost" size="icon" class="h-8 w-8" [uiDropdownMenuTrigger]="inProgressMenu" align="end">
                                <svg lucide="MoreHorizontal" class="h-4 w-4"></svg>
                                <span class="sr-only">Actions</span>
                              </button>
                              <ng-template #inProgressMenu>
                                <div uiDropdownMenuContent>
                                  <a uiDropdownMenuItem [routerLink]="tree(hrefFor(app.id))" class="cursor-pointer">
                                    <svg lucide="Edit" class="h-4 w-4 mr-2"></svg>
                                    Complete Application
                                  </a>
                                  <div uiDropdownMenuItem class="cursor-pointer" (triggered)="viewDraft.set(toDraft(app))">
                                    <svg lucide="Eye" class="h-4 w-4 mr-2"></svg>
                                    View Details
                                  </div>
                                  <div uiDropdownMenuSeparator></div>
                                  <div uiDropdownMenuItem class="text-red-600" (triggered)="handleCancel(app.id, app.type)">
                                    <svg lucide="Trash2" class="h-4 w-4 mr-2"></svg>
                                    Cancel Application
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

                <div uiTabsContent value="submitted">
                  @if (actionOnly()) {
                    <div
                      role="status"
                      class="mb-3 flex flex-wrap items-center justify-between gap-2 rounded-md border border-yellow-200 bg-yellow-50 px-3 py-2 text-sm text-yellow-900"
                    >
                      <span>Showing only applications that need something from you ({{ actionRequired().length }}).</span>
                      <button uiButton variant="ghost" size="sm" (click)="actionOnly.set(false)">Show all submitted</button>
                    </div>
                  }
                  <div class="rounded-md border overflow-x-auto">
                    <ui-table>
                      <thead uiTableHeader>
                        <tr uiTableRow>
                          <th uiTableHead>Application</th>
                          <th uiTableHead>Status</th>
                          <th uiTableHead class="hidden md:table-cell">Submitted</th>
                          <th uiTableHead class="hidden sm:table-cell">Expected Completion</th>
                          <th uiTableHead class="w-12"></th>
                        </tr>
                      </thead>
                      <tbody uiTableBody>
                        @for (app of visibleSubmitted(); track app.id) {
                          <tr uiTableRow>
                            <td uiTableCell>
                              <div>
                                <p class="font-medium">{{ app.type }}</p>
                                <p class="text-sm text-muted-foreground">Job No: {{ app.id }} | Ref: {{ app.reference }}</p>
                              </div>
                            </td>
                            <td uiTableCell>
                              <applications-status-badge [status]="app.status" />
                            </td>
                            <td uiTableCell class="hidden md:table-cell">
                              <span class="text-sm">{{ date(app.submittedDate) }}</span>
                            </td>
                            <td uiTableCell class="hidden sm:table-cell">
                              <span class="text-sm text-muted-foreground">{{ date(app.expectedCompletion) }}</span>
                            </td>
                            <td uiTableCell>
                              <button uiButton variant="ghost" size="icon" class="h-8 w-8" [uiDropdownMenuTrigger]="submittedMenu" align="end">
                                <svg lucide="MoreHorizontal" class="h-4 w-4"></svg>
                                <span class="sr-only">Actions</span>
                              </button>
                              <ng-template #submittedMenu>
                                <div uiDropdownMenuContent>
                                  <a uiDropdownMenuItem [routerLink]="path('applications.submitted', { id: app.id })" class="cursor-pointer">
                                    <svg lucide="Eye" class="h-4 w-4 mr-2"></svg>
                                    View Application
                                  </a>
                                  <a uiDropdownMenuItem [routerLink]="path('applications.documents', { id: app.id })" class="cursor-pointer">
                                    <svg lucide="FileText" class="h-4 w-4 mr-2"></svg>
                                    Upload Documents
                                  </a>
                                  <a uiDropdownMenuItem [routerLink]="tree(path('applications.submitted', { id: app.id }) + '#conversation')" class="cursor-pointer">
                                    <svg lucide="MessageSquare" class="h-4 w-4 mr-2"></svg>
                                    Message LCRB
                                  </a>
                                  <div uiDropdownMenuSeparator></div>
                                  <div uiDropdownMenuItem class="text-red-600" (triggered)="chooseCancelTarget(app)">
                                    <svg lucide="Ban" class="h-4 w-4 mr-2"></svg>
                                    Cancel Application
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

                <div uiTabsContent value="completed">
                  <div class="rounded-md border overflow-x-auto">
                    <ui-table>
                      <thead uiTableHeader>
                        <tr uiTableRow>
                          <th uiTableHead>Application</th>
                          <th uiTableHead>Status</th>
                          <th uiTableHead>Completed</th>
                          <th uiTableHead class="w-12"></th>
                        </tr>
                      </thead>
                      <tbody uiTableBody>
                        @for (app of apps().completed; track app.id) {
                          <tr uiTableRow>
                            <td uiTableCell>
                              <div>
                                <p class="font-medium">{{ app.type }}</p>
                                <p class="text-sm text-muted-foreground">Job No: {{ app.id }} | Ref: {{ app.reference }}</p>
                              </div>
                            </td>
                            <td uiTableCell>
                              <applications-status-badge [status]="app.status" />
                            </td>
                            <td uiTableCell>
                              <span class="text-sm">{{ date(app.completedDate) }}</span>
                            </td>
                            <td uiTableCell>
                              <button uiButton variant="ghost" size="icon" class="h-8 w-8" [uiDropdownMenuTrigger]="completedMenu" align="end">
                                <svg lucide="MoreHorizontal" class="h-4 w-4"></svg>
                                <span class="sr-only">Actions</span>
                              </button>
                              <ng-template #completedMenu>
                                <div uiDropdownMenuContent>
                                  <a uiDropdownMenuItem [routerLink]="path('applications.submitted', { id: app.id })" class="cursor-pointer">
                                    <svg lucide="Eye" class="h-4 w-4 mr-2"></svg>
                                    View Application
                                  </a>
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
          <div uiCard class="mt-6 border-blue-200 bg-blue-50">
            <div uiCardContent class="p-4">
              <div class="flex gap-3">
                <svg lucide="AlertCircle" class="h-5 w-5 text-blue-600 shrink-0 mt-0.5"></svg>
                <div class="text-sm">
                  <p class="font-medium text-blue-900">Please note</p>
                  <p class="text-blue-700 mt-1">
                    Due to the high volume of applications, we ask that you limit questions about the status of your application as much as possible. We
                    will contact you regarding your application as soon as we have an update.
                  </p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </main>

      <draft-summary-dialog
        [draft]="viewDraft()"
        [open]="viewDraft() !== null"
        (openChange)="!$event && viewDraft.set(null)"
        [resumeHref]="viewDraft() ? hrefFor(viewDraft()!.id) : path('applications.new')"
      />

      @if (cancelTarget(); as target) {
        <cancel-application-dialog
          [open]="true"
          (openChange)="!$event && cancelTarget.set(null)"
          [subject]="target.type + ' (' + target.id + ')'"
          [consequence]="consequenceFor(target.id)"
          (confirmed)="handleCancelSubmitted($event)"
        />
      }

      <bc-gov-footer />
    </div>
  `,
})
export class ApplicationsPage {
  protected readonly path = path;
  private readonly applicationsRepository = inject(APPLICATIONS_REPOSITORY);
  protected readonly breadcrumbs: HeroCrumb[] = [{ label: 'Dashboard', href: path('dashboard.home') }, { label: 'My Applications' }];

  private readonly router = inject(Router);
  private readonly sessionProvider = injectSession();

  protected readonly searchQuery = signal('');
  protected readonly typeFilter = signal('all');
  protected readonly baseApps = signal(this.applicationsRepository.myApplications());
  /** Draft selected for the read-only "View Details" summary dialog. */
  protected readonly viewDraft = signal<DraftSummary | null>(null);
  /** Submitted application the user is about to withdraw (drives the dialog). */
  protected readonly cancelTarget = signal<SubmittedApplicationEntry | null>(null);

  // Withdrawn submissions come from the shared cancellation service, so they survive reloads, are visible to LCRB and the local government, and move from Submitted to Completed.
  private readonly cancellations = this.applicationsRepository.cancellations();
  protected readonly apps = computed(() => {
    const baseApps = this.baseApps();
    const cancellations = this.cancellations();
    if (cancellations.length === 0) return baseApps;
    const byJob = new Map(cancellations.map((c) => [c.id, c]));
    const cancelledEntries = baseApps.submitted
      .filter((a) => byJob.has(a.id))
      .map((a) => ({
        id: a.id,
        type: a.type,
        reference: a.reference,
        status: 'cancelled' as const,
        completedDate: byJob.get(a.id)!.cancelledAt,
      }));
    return {
      ...baseApps,
      submitted: baseApps.submitted.filter((a) => !byJob.has(a.id)),
      completed: [...cancelledEntries, ...baseApps.completed],
    };
  });

  // Tabs are controlled so the stat tiles can drive them; "Action Required" is a view over the submitted tab for applications waiting on the applicant.
  protected readonly activeTab = signal<ActiveTab>('in-progress');
  protected readonly actionOnly = signal(false);
  protected readonly actionRequired = computed(() => this.apps().submitted.filter((a) => a.status === 'pending_documents'));
  protected readonly visibleSubmitted = computed(() => (this.actionOnly() ? this.actionRequired() : this.apps().submitted));

  protected hrefFor(id: string): string {
    return hrefForApplication(id);
  }

  /** Parsed so a `?query` or `#fragment` in the href renders as-is. */
  protected tree(href: string) {
    return this.router.parseUrl(href);
  }

  protected date(value: string): string {
    return new Date(value).toLocaleDateString();
  }

  protected toDraft(app: unknown): DraftSummary {
    return app as DraftSummary;
  }

  protected consequenceFor(id: string): string | undefined {
    return this.applicationsRepository.referralsFor(id).includes('local-government')
      ? 'The local government has this file for comment; their reviewer will be told it was withdrawn.'
      : undefined;
  }

  protected selectTab(tab: ActiveTab, onlyAction = false): void {
    this.activeTab.set(tab);
    this.actionOnly.set(onlyAction);
  }

  /** "Cancel Application" on a submitted row opens the cancel dialog for it. */
  protected chooseCancelTarget(app: SubmittedApplicationEntry): void {
    this.cancelTarget.set(app);
  }

  protected handleCancelSubmitted(reason: string): void {
    const target = this.cancelTarget();
    if (!target) return;
    this.applicationsRepository.cancel({
      jobNumber: target.id,
      applicationType: target.type,
      applicant: 'LCRB Training Business 5 Ltd.',
      actor: actorName(this.sessionProvider.session()),
      reason,
      priorStatus: target.status,
      referredTo: this.applicationsRepository.referralsFor(target.id),
    });
    toast.success('Application cancelled', {
      description: `${target.type} (${target.id}) has been withdrawn. Reviewers have been notified.`,
    });
    this.cancelTarget.set(null);
  }

  protected handleCancel(id: string, type: string): void {
    this.baseApps.update((prev) => ({ ...prev, inProgress: prev.inProgress.filter((a) => a.id !== id) }));
    toast.success('Application cancelled', {
      description: `${type} (${id}) has been removed from your in-progress applications.`,
    });
  }
}
