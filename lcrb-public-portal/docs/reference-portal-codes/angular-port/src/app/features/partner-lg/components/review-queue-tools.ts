/** Queue planning tools: .ics deadline calendar, PDF list, or scheduled "emailed" PDF. One effect reads schedules; another fires due ones once records load. */
import { ChangeDetectionStrategy, Component, computed, effect, inject, input, signal, untracked } from '@angular/core';
import { injectSession } from '@core/session/session-provider';
import { ALERT_DIALOG } from '@shared/ui/primitives/alert-dialog';
import { Badge } from '@shared/ui/primitives/badge';
import { Button } from '@shared/ui/primitives/button';
import { CARD } from '@shared/ui/primitives/card';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { SELECT } from '@shared/ui/primitives/select';
import { Switch } from '@shared/ui/primitives/switch';
import type { LgApplication } from '@domain/models/lg-review';
import { generateLgReviewReportPdf } from '@shared/util/pdf/lg-review-report';
import { actorName } from '@domain/roles';
import { openApplications, summarizeDeadlines } from '@domain/lg-review/lg-review-calendar';
import { downloadLgReviewICS } from '@shared/util/lg-review-ics-download';
import { type LgReportSchedule } from '@data-access/repositories/lg-review/lg-report-schedule-service';
import { cadenceLabel, describeFilters } from '@domain/lg-review/report-schedule';
import { type LgReportFilters, type ReportCadence } from '@domain/models/lg-review';
import { LG_REVIEW_REPOSITORY } from '@data-access/repositories/lg-review/lg-review.repository';

function formatDateTime(iso: string): string {
  return new Date(iso).toLocaleString('en-CA', { dateStyle: 'medium', timeStyle: 'short' });
}

@Component({
  selector: 'review-queue-tools',
  imports: [ALERT_DIALOG, Badge, Button, CARD, LucideIcon, SELECT, Switch],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div uiCard class="mb-6 border-[#013366]/20">
      <div uiCardContent class="p-4 sm:p-5">
        <div class="flex items-start gap-3">
          <div class="mt-0.5 flex h-9 w-9 shrink-0 items-center justify-center rounded-md bg-[#013366]/10 text-[#013366]">
            <svg lucide="CalendarSync" class="h-5 w-5" aria-hidden="true"></svg>
          </div>
          <div class="min-w-0 flex-1">
            <h2 class="text-base font-semibold text-foreground">Deadline tools</h2>
            <p class="mt-1 text-sm text-muted-foreground">
              {{ stats().open }} application{{ stats().open === 1 ? '' : 's' }} awaiting your comment@if (stats().overdue > 0) {<span class="text-red-700 font-medium"> · {{ stats().overdue }} overdue</span>}@if (stats().dueSoon > 0) {<span class="text-orange-600 font-medium"> · {{ stats().dueSoon }} due within 14 days</span>}. Export your deadlines, download a due-date list, or have it emailed on a schedule.
            </p>

            <!-- One-off downloads -->
            <div class="mt-3 flex flex-wrap gap-2">
              <button uiButton type="button" variant="outline" (click)="handleDownloadCalendar()" [disabled]="openForCalendar().length === 0">
                <svg lucide="CalendarPlus" class="h-4 w-4"></svg>
                Download calendar (.ics)
              </button>
              <button uiButton type="button" variant="outline" (click)="handleDownloadPdf()" [disabled]="downloadingPdf() || filteredApplications().length === 0">
                @if (downloadingPdf()) {
                  <svg lucide="Loader2" class="h-4 w-4 animate-spin"></svg>
                } @else {
                  <svg lucide="FileDown" class="h-4 w-4"></svg>
                }
                Download PDF list
              </button>
            </div>

            @if (email()) {
              <p class="mt-4 inline-flex items-center gap-1.5 text-sm text-foreground">
                <svg lucide="Mail" class="h-4 w-4 text-muted-foreground" aria-hidden="true"></svg>
                Scheduled emails send to <span class="font-medium">{{ email() }}</span>
              </p>
            } @else {
              <p class="mt-4 text-sm text-[#8A6100]">No email is associated with your account, so automatic delivery is unavailable.</p>
            }

            <!-- Create schedule controls -->
            <div class="mt-2 flex flex-col gap-3 rounded-md border border-border bg-muted/30 p-3 sm:flex-row sm:items-end">
              <div class="flex flex-col gap-1.5">
                <label for="lg-cadence" class="text-xs font-medium text-foreground">Frequency</label>
                <ui-select [value]="cadence()" (valueChange)="cadence.set($any($event))">
                  <button uiSelectTrigger id="lg-cadence" class="w-full bg-background sm:w-[240px]"><span uiSelectValue></span></button>
                  <ng-template uiSelectContent>
                    <ui-select-item value="daily">Daily — deadlines due within 1 day</ui-select-item>
                    <ui-select-item value="weekly">Weekly — deadlines due within 7 days</ui-select-item>
                  </ng-template>
                </ui-select>
              </div>
              <div class="min-w-0 flex-1 text-xs text-muted-foreground sm:pb-2">
                <p>Filters captured: <span class="text-foreground">{{ describeFilters(filters()) }}</span></p>
                <p class="mt-0.5">
                  Next run would include <span class="text-foreground">{{ preview().applications.length }} application{{ preview().applications.length === 1 ? '' : 's' }}</span> due by <span class="text-foreground">{{ preview().horizon }}</span> (plus any overdue)
                </p>
              </div>
              <button uiButton type="button" (click)="handleCreate()" [disabled]="!email() || creating()" class="bg-[#013366] text-white hover:bg-[#012a52]">
                @if (creating()) {
                  <svg lucide="Loader2" class="h-4 w-4 animate-spin"></svg>
                } @else {
                  <svg lucide="CalendarSync" class="h-4 w-4"></svg>
                }
                Schedule delivery
              </button>
            </div>

            <!-- Existing schedules -->
            @if (schedules().length > 0) {
              <ul class="mt-4 flex flex-col gap-2">
                @for (s of schedules(); track s.id) {
                  <li class="flex flex-col gap-3 rounded-md border border-border p-3 sm:flex-row sm:items-center sm:justify-between">
                    <div class="min-w-0 flex-1">
                      <div class="flex flex-wrap items-center gap-2">
                        <span class="font-medium text-foreground">{{ cadenceLabel(s.cadence) }} report</span>
                        <span
                          uiBadge
                          variant="outline"
                          [class]="s.active ? 'border-[#2E8540]/30 bg-[#2E8540]/10 text-[#1E5B2C]' : 'border-border bg-muted text-muted-foreground'"
                        >
                          {{ s.active ? 'Active' : 'Paused' }}
                        </span>
                      </div>
                      <p class="mt-1 truncate text-xs text-muted-foreground">{{ describeFilters(s.filters) }}</p>
                      <div class="mt-1 flex flex-wrap items-center gap-x-4 gap-y-1 text-xs text-muted-foreground">
                        <span class="inline-flex items-center gap-1">
                          <svg lucide="Clock" class="h-3.5 w-3.5" aria-hidden="true"></svg>
                          {{ s.active ? 'Next: ' + formatDateTime(s.nextRunAt) : 'Paused' }}
                        </span>
                        @if (s.lastRunAt) {
                          <span class="inline-flex items-center gap-1">
                            <svg lucide="CheckCircle2" class="h-3.5 w-3.5 text-[#2E8540]" aria-hidden="true"></svg>
                            Last sent {{ formatDateTime(s.lastRunAt) }} ({{ s.lastRunCount }} application{{ s.lastRunCount === 1 ? '' : 's' }})
                          </span>
                        }
                      </div>
                    </div>
                    <div class="flex items-center gap-2">
                      <button uiButton type="button" variant="outline" size="sm" (click)="handleSendNow(s)" [disabled]="sendingId() === s.id">
                        @if (sendingId() === s.id) {
                          <svg lucide="Loader2" class="h-4 w-4 animate-spin"></svg>
                        } @else {
                          <svg lucide="Send" class="h-4 w-4"></svg>
                        }
                        Send now
                      </button>
                      <button
                        uiSwitch
                        [checked]="s.active"
                        (checkedChange)="setScheduleActive(s.id, $event)"
                        [attr.aria-label]="s.active ? 'Pause schedule' : 'Resume schedule'"
                      ></button>
                      <ui-alert-dialog>
                        <button
                          uiButton
                          uiAlertDialogTrigger
                          type="button"
                          variant="ghost"
                          size="icon"
                          class="text-muted-foreground hover:text-destructive"
                          aria-label="Delete schedule"
                        >
                          <svg lucide="Trash2" class="h-4 w-4"></svg>
                        </button>
                        <ui-alert-dialog-content *uiAlertDialogPortal>
                          <div uiAlertDialogHeader>
                            <h2 uiAlertDialogTitle>Remove this scheduled report?</h2>
                            <p uiAlertDialogDescription>
                              You will stop receiving the {{ cadenceLabel(s.cadence).toLowerCase() }} email to {{ s.email }}. You can create a new schedule at any time.
                            </p>
                          </div>
                          <div uiAlertDialogFooter>
                            <button uiAlertDialogCancel>Cancel</button>
                            <button uiAlertDialogAction (click)="deleteSchedule(s.id)" class="bg-destructive text-white hover:bg-destructive/90">Remove</button>
                          </div>
                        </ui-alert-dialog-content>
                      </ui-alert-dialog>
                    </div>
                  </li>
                }
              </ul>
            }

            <p class="mt-3 text-xs text-muted-foreground">
              Prototype: emails are simulated and recorded in the notification delivery log. &ldquo;Send now&rdquo; downloads the actual PDF so you can preview what recipients receive.
            </p>
          </div>
        </div>
      </div>
    </div>
  `,
})
export class ReviewQueueTools {
  private readonly lgReviewRepository = inject(LG_REVIEW_REPOSITORY);
  /** The applications currently shown (search + status applied). */
  readonly filteredApplications = input.required<LgApplication[]>();
  /** The full jurisdiction set (used for schedule runs / previews). */
  readonly allApplications = input.required<LgApplication[]>();
  /** Filters to capture into a new schedule. */
  readonly filters = input.required<LgReportFilters>();
  /** Human description of the active filters, shown in the PDF + schedule copy. */
  readonly filterSummary = input.required<string>();

  private readonly session = injectSession().session;
  protected readonly email = computed(() => this.session()?.email ?? '');
  private readonly owner = computed(() => {
    const session = this.session();
    return this.email() || (session ? `${session.role}:anonymous` : '');
  });

  protected readonly schedules = signal<LgReportSchedule[]>([]);
  protected readonly cadence = signal<ReportCadence>('weekly');
  protected readonly creating = signal(false);
  protected readonly sendingId = signal<string | null>(null);
  protected readonly downloadingPdf = signal(false);
  private processed = false;

  protected readonly stats = computed(() => summarizeDeadlines(this.filteredApplications()));
  protected readonly openForCalendar = computed(() => openApplications(this.filteredApplications()));
  protected readonly preview = computed(() =>
    this.lgReviewRepository.filterApplicationsForSchedule(this.allApplications(), { cadence: this.cadence(), filters: this.filters() }),
  );

  protected readonly cadenceLabel = cadenceLabel;
  protected readonly describeFilters = describeFilters;
  protected readonly formatDateTime = formatDateTime;
  protected readonly setScheduleActive = this.lgReviewRepository.setScheduleActive;
  protected readonly deleteSchedule = this.lgReviewRepository.deleteSchedule;

  constructor() {
    effect((onCleanup) => {
      const owner = this.owner();
      const refresh = () => {
        if (!owner) return;
        this.schedules.set(this.lgReviewRepository.listSchedules(owner));
      };
      untracked(refresh);
      const unsub = this.lgReviewRepository.reportScheduleCollection.subscribe(refresh);
      onCleanup(unsub);
    });

    // Automatic delivery: fire any schedules whose next run is due once records are available. recordScheduleRun advances nextRunAt so this won't loop.
    effect(() => {
      const owner = this.owner();
      const all = this.allApplications();
      untracked(() => {
        if (!owner || this.processed || all.length === 0) return;
        this.processed = true;
        const due = this.lgReviewRepository.getDueSchedules(owner);
        if (due.length === 0) return;
        for (const schedule of due) {
          const { horizon, applications } = this.lgReviewRepository.filterApplicationsForSchedule(all, schedule);
          this.lgReviewRepository.recordScheduleRun(
            schedule,
            { horizon, count: applications.length, filename: `LG-review-due-dates-${horizon}.pdf` },
            { automatic: true },
          );
        }
        this.refresh();
      });
    });
  }

  private refresh(): void {
    const owner = this.owner();
    if (!owner) return;
    this.schedules.set(this.lgReviewRepository.listSchedules(owner));
  }

  protected async handleDownloadPdf(): Promise<void> {
    if (this.downloadingPdf()) return;
    this.downloadingPdf.set(true);
    try {
      await generateLgReviewReportPdf({
        applications: this.filteredApplications(),
        filterSummary: this.filterSummary(),
        output: 'save',
      });
    } finally {
      this.downloadingPdf.set(false);
    }
  }

  protected handleDownloadCalendar(): void {
    downloadLgReviewICS(this.filteredApplications());
  }

  protected async handleCreate(): Promise<void> {
    const session = this.session();
    const email = this.email();
    if (!session || !email || this.creating()) return;
    this.creating.set(true);
    try {
      this.lgReviewRepository.createSchedule({
        owner: this.owner(),
        email,
        ownerName: actorName(session),
        role: session.role,
        cadence: this.cadence(),
        filters: this.filters(),
      });
      this.refresh();
    } finally {
      this.creating.set(false);
    }
  }

  protected async handleSendNow(schedule: LgReportSchedule): Promise<void> {
    if (this.sendingId()) return;
    this.sendingId.set(schedule.id);
    try {
      const { horizon, applications } = this.lgReviewRepository.filterApplicationsForSchedule(this.allApplications(), schedule);
      const result = await generateLgReviewReportPdf({
        applications,
        filterSummary: describeFilters(schedule.filters),
        output: 'save',
        fileBaseName: `LG-review-due-dates-${horizon}`,
      });
      this.lgReviewRepository.recordScheduleRun(schedule, { horizon, count: result.count, filename: result.filename });
      this.refresh();
    } finally {
      this.sendingId.set(null);
    }
  }
}
