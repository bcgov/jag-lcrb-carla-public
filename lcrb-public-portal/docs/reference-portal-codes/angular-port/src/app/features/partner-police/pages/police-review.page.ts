/** Police review of one SEP. An unknown id shows the inline "Permit not found", so no guard. `Detail` is `div[policeReviewDetail]` for the `dl` grid. */
import { ChangeDetectionStrategy, Component, computed, inject, input, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DraftRationaleButton, type DraftRationaleInput } from '@shared/ui/ai/draft-rationale-button';
import { ReviewerBrief } from '@shared/ui/ai/reviewer-brief';
import { BCGovFooter } from '@core/layout/footer';
import { BCGovHeader } from '@core/layout/header';
import { PAGE_HERO } from '@shared/ui/bc-gov/page-hero';
import { SepStatusBadge } from '@shared/ui/bc-gov/sep-status-badge';
import { CaseThread } from '@shared/ui/case-thread/case-thread';
import { RelatedPermits } from '../components/related-permits';
import { injectSession } from '@core/session/session-provider';
import { injectFlushSync } from '@shared/ui/primitives/controllable';
import { Button } from '@shared/ui/primitives/button';
import { Checkbox } from '@shared/ui/primitives/checkbox';
import { DIALOG } from '@shared/ui/primitives/dialog';
import type { IconName } from '@shared/ui/primitives/icons';
import { Input } from '@shared/ui/primitives/input';
import { Label } from '@shared/ui/primitives/label';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { Separator } from '@shared/ui/primitives/separator';
import { Textarea } from '@shared/ui/primitives/textarea';
import { AUTO_APPROVAL_THRESHOLD, formatEventDate, formatServiceWindow, isAutoApproved, STANDARD_CONDITIONS } from '@domain/special-events/seps';
import { type SepCondition, type SepRecord } from '@domain/models/special-events';
import { actorName } from '@domain/roles';
import { downloadSepPdf, type SepPdfOptions } from '@shared/util/pdf/sep-pdf';
import { MIN_NOTICE_DAYS } from '@domain/special-events/auto-approval';
import { SPECIAL_EVENTS_REPOSITORY } from '@data-access/repositories/special-events/special-events.repository';
import { path } from '@/app/route-registry';

@Component({
  selector: 'div[policeReviewDetail]',
  imports: [LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { class: 'flex items-start gap-3' },
  template: `
    <svg [lucide]="icon()" class="mt-0.5 h-4 w-4 shrink-0 text-[#013366]"></svg>
    <div>
      <dt class="text-xs uppercase tracking-wide text-muted-foreground">{{ label() }}</dt>
      <dd class="text-sm text-foreground">{{ value() }}</dd>
    </div>
  `,
})
export class PoliceReviewDetail {
  readonly icon = input.required<IconName>();
  readonly label = input.required<string>();
  readonly value = input.required<string>();
}

type PdfKey = 'applicant' | 'logistics' | 'liquor' | 'conditions' | 'history';

@Component({
  selector: 'police-review-page',
  imports: [
    RouterLink,
    BCGovHeader,
    BCGovFooter,
    ...PAGE_HERO,
    ...DIALOG,
    Button,
    Checkbox,
    Input,
    Label,
    LucideIcon,
    Separator,
    Textarea,
    SepStatusBadge,
    ReviewerBrief,
    DraftRationaleButton,
    CaseThread,
    RelatedPermits,
    PoliceReviewDetail,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (record(); as r) {
      <div class="flex min-h-screen flex-col bg-background">
        <bc-gov-header role="police" currentPage="/police/queue" />
        <page-hero
          containerClassName="mx-auto w-full max-w-6xl px-4"
          [breadcrumbs]="[{ label: 'Special Event Permit Review', href: path('police.home') }, { label: 'Review Queue', href: path('police.queue') }, { label: r.eventName }]"
          [title]="r.eventName"
        >
          <sep-status-badge heroBadge [status]="r.status" />
          <ng-container heroMeta>{{ r.id }} · {{ r.eventType }} · {{ r.jurisdiction }}</ng-container>
          <ng-container heroActions>
            <button
              uiButton
              variant="outline"
              class="gap-2 border-white/40 bg-transparent text-white hover:bg-white/10 hover:text-white"
              (click)="downloadFull(r)"
            >
              <svg lucide="Download" class="h-4 w-4"></svg>
              Full PDF
            </button>
            @if (!r.assignedOfficer && !decided()) {
              <button uiButton class="gap-2 bg-bc-gold font-semibold text-bc-blue hover:bg-bc-gold/90" (click)="assignToMe(r)">
                <svg lucide="UserCheck" class="h-4 w-4"></svg>
                Assign to me
              </button>
            }
          </ng-container>
        </page-hero>

        <main id="main-content" class="mx-auto w-full max-w-6xl flex-1 px-4 py-8">
          <!-- Cancelled-by-applicant banner: takes precedence over everything else -->
          @if (isCancelled()) {
            <div class="mt-6 flex items-start gap-3 rounded-lg border border-zinc-300 bg-zinc-50 p-4">
              <svg lucide="Ban" class="mt-0.5 h-5 w-5 shrink-0 text-zinc-600"></svg>
              <div class="text-sm text-zinc-800">
                <p class="font-semibold">Cancelled by the applicant{{ cancellationEntry() ? ' on ' + cancellationEntry()!.date : '' }}</p>
                <p class="mt-0.5">{{ cancelledText() }}</p>
              </div>
            </div>
          }

          <!-- Auto-approval awareness banner -->
          @if (autoApproved() && !isCancelled()) {
            <div class="mt-6 flex items-start gap-3 rounded-lg border border-slate-200 bg-slate-50 p-4">
              <svg lucide="ShieldAlert" class="mt-0.5 h-5 w-5 shrink-0 text-slate-500"></svg>
              <div class="text-sm text-slate-700">
                <p class="font-semibold">For awareness only — auto-approved</p>
                <p class="mt-0.5">
                  This event met every auto-approval criterion (under {{ threshold }} attendees, no referral triggers, submitted at least
                  {{ minNoticeDays }} days ahead) and your agency has auto-approval turned on, so it was approved with the standard conditions. You can
                  still review the details and download a copy for situational awareness.
                </p>
              </div>
            </div>
          }

          <!-- Why a small event is in the queue at all: the policy layer referred it. -->
          @if (!autoApproved() && !decided() && r.expectedAttendance < threshold) {
            <div class="mt-6 flex items-start gap-3 rounded-lg border border-amber-200 bg-amber-50 p-4">
              <svg lucide="AlertTriangle" class="mt-0.5 h-5 w-5 shrink-0 text-amber-700"></svg>
              <div class="text-sm text-amber-900">
                <p class="font-semibold">Referred for review despite being under the attendance threshold</p>
                <p class="mt-0.5">{{ referredText() }}</p>
              </div>
            </div>
          }

          <div class="mt-6 grid gap-6 lg:grid-cols-3">
            <!-- Left: details -->
            <div class="space-y-6 lg:col-span-2">
              <!-- AI reviewer brief -->
              <!-- A direct child of space-y-6: a block host takes the margin while the brief renders (it renders nothing once decided). -->
              <reviewer-brief
                [style.display]="decided() ? 'contents' : 'block'"
                subjectType="Special Event Permit"
                [title]="r.eventName"
                reviewerRole="police"
                [disabled]="decided()"
                [decisionOptions]="decisionOptions"
                [facts]="briefFacts()"
                [signals]="briefSignals()"
                [history]="briefHistory()"
              />
              <!-- Logistics -->
              <section class="rounded-lg border border-border bg-card p-6">
                <h2 class="text-lg font-semibold text-foreground">Event details</h2>
                <dl class="mt-4 grid gap-4 sm:grid-cols-2">
                  <div policeReviewDetail icon="Calendar" label="Date(s)" [value]="eventDate()"></div>
                  <div policeReviewDetail icon="Clock" label="Time" [value]="serviceWindow()"></div>
                  <div policeReviewDetail icon="MapPin" label="Venue" [value]="r.venueName + ', ' + r.address + ', ' + r.city"></div>
                  <div policeReviewDetail icon="Users" label="Expected attendance" [value]="String(r.expectedAttendance)"></div>
                  <div policeReviewDetail icon="FileText" label="Liquor service" [value]="r.serviceType"></div>
                  <div policeReviewDetail icon="AlertTriangle" label="Minors expected" [value]="r.minorsExpected ? 'Yes' : 'No'"></div>
                </dl>
                <div uiSeparator class="my-4"></div>
                <p class="text-sm leading-relaxed text-muted-foreground">{{ r.description }}</p>
                @if (!r.siteplanProvided) {
                  <p class="mt-3 inline-flex items-center gap-2 rounded-md bg-orange-50 px-3 py-1.5 text-sm text-orange-800">
                    <svg lucide="AlertTriangle" class="h-4 w-4"></svg>
                    No site plan provided
                  </p>
                }
              </section>

              <!-- Applicant -->
              <section class="rounded-lg border border-border bg-card p-6">
                <h2 class="text-lg font-semibold text-foreground">Applicant</h2>
                <dl class="mt-4 grid gap-4 sm:grid-cols-2">
                  <div policeReviewDetail icon="UserCheck" label="Name" [value]="r.applicantName"></div>
                  <div policeReviewDetail icon="Building2" label="Organization" [value]="r.applicantOrg || '—'"></div>
                  <div policeReviewDetail icon="Phone" label="Phone" [value]="r.applicantPhone"></div>
                  <div policeReviewDetail icon="Mail" label="Email" [value]="r.applicantEmail"></div>
                </dl>
              </section>

              <!-- Conditions -->
              <section class="rounded-lg border border-border bg-card p-6">
                <div class="flex items-center justify-between">
                  <h2 class="text-lg font-semibold text-foreground">Permit conditions</h2>
                  <span class="text-sm text-muted-foreground">{{ conditions().length }} applied</span>
                </div>

                <ul class="mt-4 space-y-2">
                  @for (c of conditions(); track c.id) {
                    <li class="flex items-start justify-between gap-3 rounded-md border border-border bg-muted/30 p-3">
                      <div class="flex items-start gap-2">
                        <svg lucide="CheckCircle2" class="mt-0.5 h-4 w-4 shrink-0 text-green-600"></svg>
                        <span class="text-sm text-foreground">{{ c.text }}</span>
                      </div>
                      <div class="flex items-center gap-2">
                        <span class="shrink-0 rounded-full bg-background px-2 py-0.5 text-xs text-muted-foreground">
                          {{ c.standard ? 'Standard' : 'Custom' }}
                        </span>
                        @if (!decided()) {
                          <button (click)="removeCondition(r, c.id)" class="text-muted-foreground hover:text-destructive" aria-label="Remove condition">
                            <svg lucide="Trash2" class="h-4 w-4"></svg>
                          </button>
                        }
                      </div>
                    </li>
                  }
                  @if (conditions().length === 0) {
                    <li class="rounded-md border border-dashed border-border p-4 text-center text-sm text-muted-foreground">No conditions applied yet.</li>
                  }
                </ul>

                @if (!decided()) {
                  <div class="mt-4 space-y-3">
                    @if (unusedStandard().length > 0) {
                      <div>
                        <p class="text-sm font-medium text-foreground">Add a standard condition</p>
                        <div class="mt-2 flex flex-wrap gap-2">
                          @for (text of unusedStandard(); track text) {
                            <button
                              (click)="addStandardCondition(r, text)"
                              class="inline-flex items-center gap-1 rounded-full border border-[#013366]/30 bg-[#013366]/5 px-3 py-1 text-xs text-[#013366] hover:bg-[#013366]/10"
                            >
                              <svg lucide="Plus" class="h-3 w-3"></svg>
                              {{ text.length > 48 ? text.slice(0, 48) + '…' : text }}
                            </button>
                          }
                        </div>
                      </div>
                    }
                    <div class="flex gap-2">
                      <input
                        uiInput
                        [value]="newCondition()"
                        (input)="newCondition.set($any($event.target).value)"
                        placeholder="Add a custom condition…"
                        (keydown)="onConditionKey($event, r)"
                      />
                      <button uiButton (click)="addCustomCondition(r)" variant="outline" class="shrink-0 gap-1">
                        <svg lucide="Plus" class="h-4 w-4"></svg>
                        Add
                      </button>
                    </div>
                  </div>
                }
              </section>

              <!-- History -->
              <section class="rounded-lg border border-border bg-card p-6">
                <h2 class="text-lg font-semibold text-foreground">Review history</h2>
                <ol class="mt-4 space-y-4">
                  @for (h of r.history; track $index; let i = $index) {
                    <li class="relative pl-6">
                      <span class="absolute left-0 top-1.5 h-2.5 w-2.5 rounded-full bg-[#013366]"></span>
                      @if (i < r.history.length - 1) {
                        <span class="absolute left-[4.5px] top-4 h-full w-px bg-border"></span>
                      }
                      <div class="flex flex-wrap items-baseline gap-x-2">
                        <span class="text-sm font-medium text-foreground">{{ h.action }}</span>
                        <span class="text-xs text-muted-foreground">{{ h.actor }} &middot; {{ h.date }}</span>
                      </div>
                      @if (h.note) {
                        <p class="mt-1 text-sm text-muted-foreground">{{ h.note }}</p>
                      }
                    </li>
                  }
                </ol>
              </section>

              <!-- Shared thread with the applicant and LCRB (a block host takes the space-y-6 margin) -->
              <case-thread
                [style.display]="'block'"
                [threadId]="r.id"
                kind="sep"
                [status]="r.status"
                [subjectLabel]="'Special Event Permit ' + r.id"
                title="Conversation with applicant and LCRB"
                [hrefFor]="hrefFor()"
              />

              <!-- Prior permits by applicant / venue -->
              <related-permits [current]="r" />
            </div>

            <!-- Right: decision rail -->
            <div class="space-y-6">
              <!-- Decision actions -->
              <section class="rounded-lg border border-border bg-card p-6">
                <h2 class="text-lg font-semibold text-foreground">Decision</h2>

                @if (isCancelled()) {
                  <div class="mt-3 flex items-start gap-2 rounded-md bg-zinc-100 p-3 text-sm text-zinc-700">
                    <svg lucide="Ban" class="mt-0.5 h-4 w-4 shrink-0"></svg>
                    <p>Cancelled by the applicant. No decision can be recorded; the history below is kept for the record.</p>
                  </div>
                } @else if (decided()) {
                  <div class="mt-3 rounded-md bg-muted/40 p-3 text-sm text-muted-foreground">
                    This permit is <span class="font-medium text-foreground">{{ r.status }}</span
                    >. No further action is required.
                  </div>
                } @else if (autoApproved()) {
                  <div class="mt-3 rounded-md bg-muted/40 p-3 text-sm text-muted-foreground">Auto-approved events don't require a police decision.</div>
                } @else {
                  <!-- Radix Dialog renders no element, so each trigger is a direct child of space-y-3: the ui-dialog hosts are blocks. -->
                  <div class="mt-4 space-y-3">
                    @if (!isAssignedToMe()) {
                      <div class="rounded-md bg-amber-50 p-3 text-sm text-amber-800">
                        <p>{{ r.assignedOfficer ? 'Assigned to ' + r.assignedOfficer + '.' : 'Assign this permit to yourself to record a decision.' }}</p>
                        <button
                          uiButton
                          variant="outline"
                          size="sm"
                          class="mt-2 gap-2 border-amber-300 bg-white text-amber-900 hover:bg-amber-100"
                          (click)="assignToMe(r)"
                        >
                          <svg lucide="UserCheck" class="h-4 w-4"></svg>
                          {{ r.assignedOfficer ? 'Reassign to me' : 'Assign to me' }}
                        </button>
                      </div>
                    }

                    <!-- Approve -->
                    <ui-dialog [style.display]="'block'">
                      <button uiButton uiDialogTrigger [disabled]="!isAssignedToMe()" class="w-full gap-2 bg-green-700 hover:bg-green-800">
                        <svg lucide="CheckCircle2" class="h-4 w-4"></svg>
                        Approve permit
                      </button>
                      <ui-dialog-content *uiDialogPortal>
                        <div uiDialogHeader>
                          <h2 uiDialogTitle>Approve permit</h2>
                          <p uiDialogDescription>
                            Approving with {{ conditions().length }} condition{{ conditions().length === 1 ? '' : 's' }}. Add an optional note for the
                            record.
                          </p>
                        </div>
                        <draft-rationale-button [input]="approveInput()" (draft)="approveNote.set($event)" />
                        <textarea
                          uiTextarea
                          [value]="approveNote()"
                          (input)="approveNote.set($any($event.target).value)"
                          placeholder="Optional note…"
                        ></textarea>
                        <div uiDialogFooter>
                          <button uiButton (click)="handleApprove(r)" class="bg-green-700 hover:bg-green-800">Confirm approval</button>
                        </div>
                      </ui-dialog-content>
                    </ui-dialog>

                    <!-- Request changes -->
                    <ui-dialog [style.display]="'block'">
                      <button
                        uiButton
                        uiDialogTrigger
                        [disabled]="!isAssignedToMe()"
                        variant="outline"
                        class="w-full gap-2 border-orange-300 text-orange-800 hover:bg-orange-50"
                      >
                        <svg lucide="RotateCcw" class="h-4 w-4"></svg>
                        Request changes
                      </button>
                      <ui-dialog-content *uiDialogPortal>
                        <div uiDialogHeader>
                          <h2 uiDialogTitle>Request changes</h2>
                          <p uiDialogDescription>Send the application back to the applicant without denying it. Explain what's needed.</p>
                        </div>
                        <draft-rationale-button [input]="changesInput()" (draft)="changesNote.set($event)" />
                        <textarea
                          uiTextarea
                          [value]="changesNote()"
                          (input)="changesNote.set($any($event.target).value)"
                          placeholder="e.g. Please provide a site plan and security staffing plan."
                          required
                        ></textarea>
                        <div uiDialogFooter>
                          <button uiButton (click)="handleRequestChanges(r)" [disabled]="!changesNote().trim()" class="bg-orange-600 hover:bg-orange-700">
                            Send request
                          </button>
                        </div>
                      </ui-dialog-content>
                    </ui-dialog>

                    <!-- Deny -->
                    <ui-dialog [style.display]="'block'">
                      <button
                        uiButton
                        uiDialogTrigger
                        [disabled]="!isAssignedToMe()"
                        variant="outline"
                        class="w-full gap-2 border-red-300 text-red-700 hover:bg-red-50"
                      >
                        <svg lucide="XCircle" class="h-4 w-4"></svg>
                        Deny permit
                      </button>
                      <ui-dialog-content *uiDialogPortal>
                        <div uiDialogHeader>
                          <h2 uiDialogTitle>Deny permit</h2>
                          <p uiDialogDescription>A reason is required and will be shared with the applicant and recorded in the permit history.</p>
                        </div>
                        <draft-rationale-button [input]="denyInput()" (draft)="denyReason.set($event)" />
                        <textarea
                          uiTextarea
                          [value]="denyReason()"
                          (input)="denyReason.set($any($event.target).value)"
                          placeholder="Reason for denial (required)…"
                          required
                          aria-label="Reason for denial"
                        ></textarea>
                        <div uiDialogFooter>
                          <button uiButton (click)="handleDeny(r)" [disabled]="!denyReason().trim()" variant="destructive">Confirm denial</button>
                        </div>
                      </ui-dialog-content>
                    </ui-dialog>
                  </div>
                }
              </section>

              <!-- Customizable field PDF -->
              <section class="rounded-lg border border-border bg-card p-6">
                <h2 class="text-lg font-semibold text-foreground">Officer field copy</h2>
                <p class="mt-1 text-sm text-muted-foreground">Generate a PDF to carry on patrol. Choose what to include.</p>
                <div class="mt-4 space-y-2">
                  @for (opt of pdfOptions; track opt[0]) {
                    <div class="flex items-center gap-2">
                      <button
                        uiCheckbox
                        [attr.id]="'pdf-' + opt[0]"
                        [checked]="pdfInclude()[opt[0]]"
                        (checkedChange)="setPdf(opt[0], $event === true)"
                      ></button>
                      <label uiLabel [attr.for]="'pdf-' + opt[0]" class="font-normal">{{ opt[1] }}</label>
                    </div>
                  }
                </div>
                <button uiButton (click)="handleFieldPdf(r)" variant="outline" class="mt-4 w-full gap-2">
                  <svg lucide="Download" class="h-4 w-4"></svg>
                  Download field copy
                </button>
              </section>
            </div>
          </div>
        </main>
        <bc-gov-footer />
      </div>
    } @else {
      <div class="flex min-h-screen flex-col bg-background">
        <bc-gov-header role="police" currentPage="/police/queue" />
        <main id="main-content" class="mx-auto max-w-3xl flex-1 px-4 py-16 text-center">
          <h1 class="text-2xl font-semibold text-foreground">Permit not found</h1>
          <p class="mt-2 text-muted-foreground">We couldn't find a Special Event Permit with ID {{ id() }}.</p>
          <a uiButton [routerLink]="path('police.queue')" class="mt-6 bg-[#013366] hover:bg-[#012a52]">Back to review queue</a>
        </main>
        <bc-gov-footer />
      </div>
    }
  `,
})
export class PoliceReviewPage {
  protected readonly path = path;
  private readonly specialEventsRepository = inject(SPECIAL_EVENTS_REPOSITORY);
  /** Route param `:id`. */
  readonly id = input.required<string>();

  private readonly store = this.specialEventsRepository.store();
  private readonly sessionProvider = injectSession();
  protected readonly policy = this.specialEventsRepository.autoApprovalPolicy();

  protected readonly String = String;
  protected readonly threshold = AUTO_APPROVAL_THRESHOLD;
  protected readonly minNoticeDays = MIN_NOTICE_DAYS;
  protected readonly decisionOptions = ['Approve permit', 'Request changes', 'Deny permit'];
  protected readonly pdfOptions: [PdfKey, string][] = [
    ['applicant', 'Applicant & organizer'],
    ['logistics', 'Event logistics'],
    ['liquor', 'Liquor service & description'],
    ['conditions', 'Permit conditions'],
    ['history', 'Review history'],
  ];

  protected readonly record = computed(() => this.store.getById(this.id()));
  private readonly officerName = computed(() => actorName(this.sessionProvider.session()));

  protected readonly newCondition = signal('');
  protected readonly denyReason = signal('');
  protected readonly changesNote = signal('');
  protected readonly approveNote = signal('');

  // Field-copy section toggles
  protected readonly pdfInclude = signal<Record<PdfKey, boolean>>({
    applicant: true,
    logistics: true,
    liquor: true,
    conditions: true,
    history: false,
  });

  protected readonly conditions = computed<SepCondition[]>(() => this.record()?.conditions ?? []);
  protected readonly unusedStandard = computed(() => STANDARD_CONDITIONS.filter((text) => !this.conditions().some((c) => c.text === text)));

  protected readonly autoApproved = computed(() => isAutoApproved(this.record()!));
  protected readonly isAssignedToMe = computed(() => this.record()!.assignedOfficer === this.officerName());
  protected readonly decided = computed(() => ['approved', 'denied', 'auto-approved', 'cancelled'].includes(this.record()!.status));
  protected readonly isCancelled = computed(() => this.record()!.status === 'cancelled');
  protected readonly cancellationEntry = computed(() => {
    const record = this.record()!;
    return this.isCancelled() ? [...record.history].reverse().find((h) => h.action === 'Cancelled by applicant') : undefined;
  });

  protected readonly cancelledText = computed(() => {
    const record = this.record()!;
    const entry = this.cancellationEntry();
    // Was an officer already into this file when the applicant pulled it?
    const workedByPolice = Boolean(record.assignedOfficer) || record.conditions.some((c) => !c.standard);
    const previouslyDecided = record.history.some((h) => ['Approved', 'Denied', 'Requested changes'].includes(h.action));
    return (
      `${entry?.actor ?? record.applicantName} withdrew this permit${entry?.note ? ` — ${entry.note}` : ''}. ` +
      'The event will not proceed under this permit and no decision is required.' +
      (previouslyDecided
        ? ' The decision you had already recorded stays in the history for the record.'
        : workedByPolice
          ? ` It was assigned to ${record.assignedOfficer ?? 'an officer'}${
              record.conditions.length ? ` with ${record.conditions.length} condition(s) drafted` : ''
            }; that work is retained but no longer actionable.`
          : ' No officer had started on it.')
    );
  });

  protected readonly referredText = computed(() => {
    const record = this.record()!;
    return record.referralReasons?.length
      ? record.referralReasons.join(' ')
      : this.policy().enabled
        ? `Auto-approval only applies to permits submitted at least ${MIN_NOTICE_DAYS} days before the event.`
        : 'Auto-approval is turned off for your agency, so every permit comes to an officer.';
  });

  protected readonly eventDate = computed(() => formatEventDate(this.record()!));
  protected readonly serviceWindow = computed(() => formatServiceWindow(this.record()!));

  protected readonly briefFacts = computed(() => {
    const record = this.record()!;
    return [
      { label: 'Event type', value: record.eventType },
      { label: 'Jurisdiction', value: record.jurisdiction },
      { label: 'Date(s)', value: formatEventDate(record) },
      { label: 'Time', value: formatServiceWindow(record) },
      { label: 'Venue', value: `${record.venueName}, ${record.address}, ${record.city}` },
      { label: 'Expected attendance', value: String(record.expectedAttendance) },
      { label: 'Liquor service', value: record.serviceType },
      { label: 'Minors expected', value: record.minorsExpected ? 'Yes' : 'No' },
      { label: 'Site plan provided', value: record.siteplanProvided ? 'Yes' : 'No' },
      { label: 'Description', value: record.description || '—' },
    ];
  });
  protected readonly briefSignals = computed(() => {
    const record = this.record()!;
    return [
      !record.siteplanProvided ? 'No site plan provided' : '',
      record.minorsExpected ? 'Minors expected at a liquor event' : '',
      record.expectedAttendance >= AUTO_APPROVAL_THRESHOLD
        ? `Attendance (${record.expectedAttendance}) at or above the ${AUTO_APPROVAL_THRESHOLD}-person threshold`
        : '',
    ].filter(Boolean);
  });
  protected readonly briefHistory = computed(() =>
    this.record()!.history.map((h) => ({ action: h.action, actor: h.actor, date: h.date, note: h.note })),
  );

  // Shared context handed to the AI decision-rationale drafter.
  private readonly rationaleBase = computed(() => {
    const record = this.record()!;
    return {
      subjectType: 'Special Event Permit',
      title: record.eventName,
      reviewerRole: 'police',
      facts: [
        { label: 'Event type', value: record.eventType },
        { label: 'Jurisdiction', value: record.jurisdiction },
        { label: 'Venue', value: `${record.venueName}, ${record.city}` },
        { label: 'Expected attendance', value: String(record.expectedAttendance) },
        { label: 'Liquor service', value: record.serviceType },
        { label: 'Minors expected', value: record.minorsExpected ? 'Yes' : 'No' },
        { label: 'Site plan provided', value: record.siteplanProvided ? 'Yes' : 'No' },
      ],
      signals: [
        !record.siteplanProvided ? 'No site plan provided' : '',
        record.minorsExpected ? 'Minors expected at a liquor event' : '',
      ].filter(Boolean),
    };
  });
  protected readonly approveInput = computed<DraftRationaleInput>(() => ({
    ...this.rationaleBase(),
    decision: 'Approve permit',
    tone: 'approve',
    conditions: this.conditions().map((c) => c.text),
    reviewerNotes: this.approveNote(),
  }));
  protected readonly changesInput = computed<DraftRationaleInput>(() => ({
    ...this.rationaleBase(),
    decision: 'Request changes',
    tone: 'changes',
    reviewerNotes: this.changesNote(),
  }));
  protected readonly denyInput = computed<DraftRationaleInput>(() => ({
    ...this.rationaleBase(),
    decision: 'Deny permit',
    tone: 'deny',
    reviewerNotes: this.denyReason(),
  }));

  protected readonly hrefFor = computed(() => {
    const id = this.record()!.id;
    return {
      applicant: path('special-events.detail', { id: id }),
      licensee: path('special-events.detail', { id: id }),
      lcrb: `${path('staff.conversations')}?thread=${encodeURIComponent(id)}`,
      police: path('police.review', { id: id }),
    };
  });

  private readonly flushSync = injectFlushSync();

  protected assignToMe(r: SepRecord): void {
    this.store.assignOfficer(r.id, this.officerName());
    // React applies a click's state before the handler returns, so the decision buttons are enabled at once (the approve-dialog state clicks them straight after).
    this.flushSync();
  }

  protected downloadFull(r: SepRecord): void {
    downloadSepPdf(r, 'full', { preparedFor: this.officerName() });
  }

  protected addStandardCondition(r: SepRecord, text: string): void {
    this.store.setConditions(r.id, [...this.conditions(), { id: `std-${Date.now()}`, text, standard: true }]);
  }

  protected addCustomCondition(r: SepRecord): void {
    if (!this.newCondition().trim()) return;
    const next: SepCondition = {
      id: `custom-${Date.now()}`,
      text: this.newCondition().trim(),
      standard: false,
    };
    this.store.setConditions(r.id, [...this.conditions(), next]);
    this.newCondition.set('');
  }

  protected onConditionKey(e: KeyboardEvent, r: SepRecord): void {
    if (e.key === 'Enter') {
      e.preventDefault();
      this.addCustomCondition(r);
    }
  }

  protected removeCondition(r: SepRecord, condId: string): void {
    this.store.setConditions(
      r.id,
      this.conditions().filter((c) => c.id !== condId),
    );
  }

  protected handleApprove(r: SepRecord): void {
    this.store.decide(r.id, 'approved', this.officerName(), this.approveNote());
  }
  protected handleDeny(r: SepRecord): void {
    if (!this.denyReason().trim()) return;
    this.store.decide(r.id, 'denied', this.officerName(), this.denyReason().trim());
  }
  protected handleRequestChanges(r: SepRecord): void {
    if (!this.changesNote().trim()) return;
    this.store.decide(r.id, 'changes-requested', this.officerName(), this.changesNote().trim());
  }

  protected setPdf(key: PdfKey, checked: boolean): void {
    this.pdfInclude.update((prev) => ({ ...prev, [key]: checked }));
  }

  protected handleFieldPdf(r: SepRecord): void {
    const options: SepPdfOptions = { include: this.pdfInclude(), preparedFor: this.officerName() };
    downloadSepPdf(r, 'field', options);
  }
}
