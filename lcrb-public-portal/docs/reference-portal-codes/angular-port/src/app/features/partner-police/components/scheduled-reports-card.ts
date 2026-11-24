/** Scheduled SEP report subscriptions. Deliveries are simulated; "Send now" makes the real PDF. Reads run after first render, gated by `ready`. */
import { afterNextRender, ChangeDetectionStrategy, Component, computed, effect, inject, input, signal, untracked } from '@angular/core';
import { injectSession } from '@core/session/session-provider';
import { ALERT_DIALOG } from '@shared/ui/primitives/alert-dialog';
import { Badge } from '@shared/ui/primitives/badge';
import { Button } from '@shared/ui/primitives/button';
import { CARD } from '@shared/ui/primitives/card';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { SELECT } from '@shared/ui/primitives/select';
import { Switch } from '@shared/ui/primitives/switch';
import type { SepRecord } from '@domain/models/special-events';
import { generateSepReportPdf } from '@shared/util/pdf/sep-daily-report';
import { actorName } from '@domain/roles';
import { type ReportSchedule } from '@data-access/repositories/police-review/sep-report-schedule-service';
import { cadenceLabel, describeFilters } from '@domain/police-review/report-schedule';
import { type ReportCadence, type ReportScheduleFilters } from '@domain/models/police-review';
import { POLICE_REVIEW_REPOSITORY } from '@data-access/repositories/police-review/police-review.repository';

function formatDateTime(iso: string): string {
  return new Date(iso).toLocaleString('en-CA', { dateStyle: 'medium', timeStyle: 'short' });
}

function formatDate(iso: string): string {
  return new Date(iso).toLocaleDateString('en-CA', { month: 'short', day: 'numeric', weekday: 'short' });
}

@Component({
  selector: 'scheduled-reports-card',
  imports: [...ALERT_DIALOG, ...CARD, ...SELECT, Badge, Button, LucideIcon, Switch],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (session()) {
      <div uiCard class="mb-6 border-[#013366]/20">
        <div uiCardContent class="p-4 sm:p-5">
          <div class="flex items-start gap-3">
            <div class="mt-0.5 flex h-9 w-9 shrink-0 items-center justify-center rounded-md bg-[#013366]/10 text-[#013366]">
              <svg lucide="CalendarSync" class="h-5 w-5" aria-hidden="true"></svg>
            </div>
            <div class="min-w-0 flex-1">
              <h2 class="text-base font-semibold text-foreground">Automatic report delivery</h2>
              <p class="mt-1 text-sm text-muted-foreground">
                Have this permit report generated as a PDF and emailed to your account automatically.
              </p>

              @if (email()) {
                <p class="mt-2 inline-flex items-center gap-1.5 text-sm text-foreground">
                  <svg lucide="Mail" class="h-4 w-4 text-muted-foreground" aria-hidden="true"></svg>
                  Sends to <span class="font-medium">{{ email() }}</span>
                </p>
              } @else {
                <p class="mt-2 text-sm text-[#8A6100]">
                  No email is associated with your account, so automatic delivery is unavailable.
                </p>
              }

              <!-- Create controls -->
              <div class="mt-4 flex flex-col gap-3 rounded-md border border-border bg-muted/30 p-3 sm:flex-row sm:items-end">
                <div class="flex flex-col gap-1.5">
                  <label for="cadence" class="text-xs font-medium text-foreground">Frequency</label>
                  <ui-select [value]="cadence()" (valueChange)="cadence.set($any($event))">
                    <button uiSelectTrigger id="cadence" class="w-full bg-background sm:w-[220px]">
                      <span uiSelectValue></span>
                    </button>
                    <ng-template uiSelectContent>
                      <ui-select-item value="daily">Daily — the next day's permits</ui-select-item>
                      <ui-select-item value="weekly">Weekly — the next 7 days</ui-select-item>
                    </ng-template>
                  </ui-select>
                </div>
                <div class="min-w-0 flex-1 text-xs text-muted-foreground sm:pb-2">
                  <p>
                    Filters captured: <span class="text-foreground">{{ describeFilters(filters()) }}</span>
                  </p>
                  <p class="mt-0.5">
                    Next run would cover
                    <span class="text-foreground">{{ previewRange() }}</span>
                    ({{ preview().records.length }} permit{{ preview().records.length === 1 ? '' : 's' }} right now)
                  </p>
                </div>
                <button
                  uiButton
                  type="button"
                  (click)="handleCreate()"
                  [disabled]="!email() || creating()"
                  class="bg-[#013366] text-white hover:bg-[#012a52]"
                >
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
                            [class]="
                              s.active
                                ? 'border-[#2E8540]/30 bg-[#2E8540]/10 text-[#1E5B2C]'
                                : 'border-border bg-muted text-muted-foreground'
                            "
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
                              Last sent {{ formatDateTime(s.lastRunAt) }} ({{ s.lastRunCount }} permit{{ s.lastRunCount === 1 ? '' : 's' }})
                            </span>
                          }
                        </div>
                      </div>
                      <div class="flex items-center gap-2">
                        <button
                          uiButton
                          type="button"
                          variant="outline"
                          size="sm"
                          (click)="handleSendNow(s)"
                          [disabled]="sendingId() === s.id"
                        >
                          @if (sendingId() === s.id) {
                            <svg lucide="Loader2" class="h-4 w-4 animate-spin"></svg>
                          } @else {
                            <svg lucide="Send" class="h-4 w-4"></svg>
                          }
                          Send now
                        </button>
                        <div class="flex items-center gap-1.5">
                          <button
                            uiSwitch
                            [checked]="s.active"
                            (checkedChange)="setScheduleActive(s.id, $event)"
                            [attr.aria-label]="s.active ? 'Pause schedule' : 'Resume schedule'"
                          ></button>
                        </div>
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
                                You will stop receiving the {{ cadenceLabel(s.cadence).toLowerCase() }} email to
                                {{ s.email }}. You can create a new schedule at any time.
                              </p>
                            </div>
                            <div uiAlertDialogFooter>
                              <button uiAlertDialogCancel>Cancel</button>
                              <button uiAlertDialogAction (click)="deleteSchedule(s.id)" class="bg-destructive text-white hover:bg-destructive/90">
                                Remove
                              </button>
                            </div>
                          </ui-alert-dialog-content>
                        </ui-alert-dialog>
                      </div>
                    </li>
                  }
                </ul>
              }

              <p class="mt-3 text-xs text-muted-foreground">
                Prototype: emails are simulated and recorded in the notification delivery log. &ldquo;Send now&rdquo; downloads the actual PDF so you can
                preview what recipients receive.
              </p>
            </div>
          </div>
        </div>
      </div>
    }
  `,
})
export class ScheduledReportsCard {
  private readonly policeReviewRepository = inject(POLICE_REVIEW_REPOSITORY);
  readonly filters = input.required<ReportScheduleFilters>();
  readonly records = input.required<SepRecord[]>();

  private readonly sessionProvider = injectSession();
  protected readonly session = this.sessionProvider.session;
  protected readonly email = computed(() => this.session()?.email ?? '');
  private readonly owner = computed(() => {
    const session = this.session();
    return this.email() || (session ? `${session.role}:anonymous` : '');
  });

  protected readonly schedules = signal<ReportSchedule[]>([]);
  protected readonly cadence = signal<ReportCadence>('daily');
  protected readonly creating = signal(false);
  protected readonly sendingId = signal<string | null>(null);
  // Guards the automatic due-run pass so it only fires once records are loaded.
  private processed = false;
  private readonly ready = signal(false);

  protected readonly cadenceLabel = cadenceLabel;
  protected readonly describeFilters = describeFilters;
  protected readonly formatDateTime = formatDateTime;
  protected readonly setScheduleActive = this.policeReviewRepository.setScheduleActive;
  protected readonly deleteSchedule = this.policeReviewRepository.deleteSchedule;

  // A preview of what the *next* run of the selected cadence would contain, using the filters currently applied on the page.
  protected readonly preview = computed(() => this.policeReviewRepository.filterRecordsForSchedule(this.records(), { cadence: this.cadence(), filters: this.filters() }));
  protected readonly previewRange = computed(() => {
    const p = this.preview();
    return p.fromDate === p.toDate ? formatDate(p.fromDate) : `${formatDate(p.fromDate)} – ${formatDate(p.toDate)}`;
  });

  constructor() {
    afterNextRender(() => this.ready.set(true));

    // Subscribe to the schedule collection so edits from anywhere stay in sync.
    effect((onCleanup) => {
      if (!this.ready()) return;
      this.owner();
      untracked(() => this.refresh());
      const unsub = this.policeReviewRepository.reportScheduleCollection.subscribe(() => untracked(() => this.refresh()));
      onCleanup(unsub);
    });

    // Automatic delivery: on load, once we have records, fire any schedules whose next run is due. recordScheduleRun advances nextRunAt so this won't loop.
    effect(() => {
      if (!this.ready()) return;
      const owner = this.owner();
      const records = this.records();
      if (!owner || this.processed || records.length === 0) return;
      this.processed = true;
      untracked(() => {
        const due = this.policeReviewRepository.getDueSchedules(owner);
        if (due.length === 0) return;
        for (const schedule of due) {
          const { fromDate, toDate, records: matched } = this.policeReviewRepository.filterRecordsForSchedule(records, schedule);
          this.policeReviewRepository.recordScheduleRun(schedule, {
            fromDate,
            toDate,
            count: matched.length,
            filename: fromDate === toDate ? `SEP-daily-report-${fromDate}.pdf` : `SEP-report-${fromDate}_to_${toDate}.pdf`,
          });
        }
        this.refresh();
      });
    });
  }

  private refresh(): void {
    const owner = this.owner();
    if (!owner) return;
    this.schedules.set(this.policeReviewRepository.listSchedules(owner));
  }

  protected async handleCreate(): Promise<void> {
    const session = this.session();
    const email = this.email();
    if (!session || !email || this.creating()) return;
    this.creating.set(true);
    try {
      this.policeReviewRepository.createSchedule({
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

  protected async handleSendNow(schedule: ReportSchedule): Promise<void> {
    if (this.sendingId()) return;
    this.sendingId.set(schedule.id);
    try {
      const { fromDate, toDate, records: matched } = this.policeReviewRepository.filterRecordsForSchedule(this.records(), schedule);
      const result = await generateSepReportPdf({
        fromDate,
        toDate,
        records: matched,
        filterSummary: describeFilters(schedule.filters),
        output: 'save',
      });
      this.policeReviewRepository.recordScheduleRun(schedule, {
        fromDate,
        toDate,
        count: result.count,
        filename: result.filename,
      });
      this.refresh();
    } finally {
      this.sendingId.set(null);
    }
  }
}
