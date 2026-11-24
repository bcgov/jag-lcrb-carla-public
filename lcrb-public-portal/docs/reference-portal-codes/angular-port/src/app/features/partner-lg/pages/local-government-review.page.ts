/** One referral's review screen. An unknown id shows its own "Application not found", so no redirect. Detail and RiskStat are real elements for parent grids. */
import { ChangeDetectionStrategy, Component, computed, inject, input, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DraftRationaleButton, type DraftRationaleInput } from '@shared/ui/ai/draft-rationale-button';
import { ReviewerBrief } from '@shared/ui/ai/reviewer-brief';
import { BCGovFooter } from '@core/layout/footer';
import { BCGovHeader } from '@core/layout/header';
import { LgStatusBadge } from '../components/lg-status-badge';
import { PAGE_HERO } from '@shared/ui/bc-gov/page-hero';
import { CaseThread } from '@shared/ui/case-thread/case-thread';
import { DocumentManager } from '@shared/ui/documents/document-manager';
import { ApplicantHistory } from '../components/applicant-history';
import { injectSession } from '@core/session/session-provider';
import { Badge } from '@shared/ui/primitives/badge';
import { Button } from '@shared/ui/primitives/button';
import type { IconName } from '@shared/ui/primitives/icons';
import { Label } from '@shared/ui/primitives/label';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { TABS } from '@shared/ui/primitives/tabs';
import { Textarea } from '@shared/ui/primitives/textarea';
import { daysUntilDue } from '@domain/lg-review/due-dates';
import { LG_STATUS_LABELS, LG_TERMINAL_STATUSES, type LgApplication, type LgDecision } from '@domain/models/lg-review';
import { formatLgDate } from '@domain/lg-review/due-dates';
import { COMMENT_HOLD_DAYS, isReferralExempt } from '@domain/lg-review/lg-commenting';
import { downloadLgApplicationPdf, downloadLgDocumentPdf } from '@shared/util/pdf/lg-pdf';
import { actorName } from '@domain/roles';
import { formatCancelledDate } from '@domain/applications/cancellation';
import { LG_REVIEW_REPOSITORY } from '@data-access/repositories/lg-review/lg-review.repository';
import { MESSAGING_REPOSITORY } from '@data-access/repositories/messaging/messaging.repository';
import { path } from '@/app/route-registry';

function decisionToStatus(decision: LgDecision) {
  switch (decision) {
    case 'no-comment':
      return 'no-comment' as const;
    case 'community-review':
      return 'community-review' as const;
    case 'reject':
      return 'reject' as const;
    default:
      return 'documents-requested' as const;
  }
}

/** On the grid's own `div`. */
@Component({
  selector: 'div[lgReviewDetail]',
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <dt class="text-xs uppercase tracking-wide text-muted-foreground">{{ label() }}</dt>
    <dd class="mt-0.5 font-medium text-foreground">{{ value() }}</dd>
  `,
})
export class LgReviewDetail {
  readonly label = input.required<string>();
  readonly value = input.required<string>();
}

const TONES: Record<string, string> = {
  red: 'bg-red-50 text-red-800 border-red-200',
  amber: 'bg-amber-50 text-amber-800 border-amber-200',
  green: 'bg-green-50 text-green-800 border-green-200',
  muted: 'bg-muted text-foreground border-border',
};

/** On the grid's own `div`. */
@Component({
  selector: 'div[lgRiskStat]',
  imports: [LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { '[attr.class]': 'hostClass()' },
  template: `
    <svg [lucide]="icon()" class="mx-auto h-5 w-5"></svg>
    <p class="mt-1 text-lg font-semibold">{{ value() }}</p>
    <p class="text-xs">{{ label() }}</p>
  `,
})
export class LgRiskStat {
  readonly icon = input.required<IconName>();
  readonly label = input.required<string>();
  readonly value = input.required<string>();
  readonly tone = input.required<'red' | 'amber' | 'green' | 'muted'>();
  protected readonly hostClass = computed(() => `rounded-md border p-3 text-center ${TONES[this.tone()]}`);
}

@Component({
  selector: 'local-government-review-page',
  imports: [
    RouterLink,
    BCGovHeader,
    BCGovFooter,
    ...PAGE_HERO,
    ...TABS,
    Badge,
    Button,
    Label,
    Textarea,
    LucideIcon,
    LgStatusBadge,
    ReviewerBrief,
    DraftRationaleButton,
    DocumentManager,
    ApplicantHistory,
    CaseThread,
    LgReviewDetail,
    LgRiskStat,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (app(); as a) {
      <div class="flex min-h-screen flex-col bg-background">
        <bc-gov-header role="local-government" currentPage="/local-government/queue" />
        <page-hero
          containerClassName="mx-auto w-full max-w-6xl px-4"
          [breadcrumbs]="[
            { label: 'Local Government Review', href: path('lg.home') },
            { label: 'Applications Awaiting Review', href: path('lg.queue') },
            { label: a.establishmentName },
          ]"
          [title]="a.establishmentName"
        >
          <ng-container heroMeta>Job #{{ a.jobNumber }} · {{ a.applicationType }} · {{ a.licenceType }}</ng-container>
          <lg-status-badge heroBadge [status]="a.status" />
          <span heroDescription class="flex items-center gap-1.5">
            <svg lucide="MapPin" class="h-4 w-4"></svg>
            {{ a.address }}, {{ a.city }} {{ a.postalCode }}
          </span>
          <button
            heroActions
            uiButton
            variant="outline"
            class="gap-2 border-white/40 bg-transparent text-white hover:bg-white/10 hover:text-white"
            (click)="downloadApplication(a)"
          >
            <svg lucide="Download" class="h-4 w-4"></svg>
            Download full application (PDF)
          </button>
        </page-hero>

        <main id="main-content" class="mx-auto w-full max-w-6xl flex-1 px-4 py-8">
          <!-- Comment-due banner: replaced by a flow-through notice when the jurisdiction has opted out of commenting on new referrals. -->
          @if (isCancelled() && a.cancellation) {
            <div class="mt-6 flex items-start gap-3 rounded-lg border border-zinc-300 bg-zinc-50 p-4">
              <svg lucide="Ban" class="mt-0.5 h-5 w-5 shrink-0 text-zinc-600"></svg>
              <div class="text-sm">
                <p class="font-medium text-zinc-900">Cancelled by the applicant on {{ formatCancelledDate(a.cancellation.cancelledAt) }}</p>
                <p class="mt-0.5 text-zinc-700">
                  {{ a.cancellation.cancelledBy }} withdrew this application{{ a.cancellation.reason ? ' — ' + a.cancellation.reason : '' }}. The comment deadline no longer applies and nothing further is needed from {{ a.jurisdiction }}.{{ cancelNote() }}
                </p>
              </div>
            </div>
          } @else if (exemptFromComment()) {
            <div class="mt-6 flex items-start gap-3 rounded-lg border border-border bg-muted/60 p-4">
              <svg lucide="MessageSquareOff" class="mt-0.5 h-5 w-5 shrink-0 text-muted-foreground"></svg>
              <div class="text-sm">
                <p class="font-medium text-foreground">No comment required — commenting is turned off for your jurisdiction</p>
                <p class="mt-0.5 text-muted-foreground">
                  This application was referred after commenting was turned off, so it is not being held for your input. It has gone to the LCRB for processing without the {{ holdDays }}-day comment period.
                </p>
              </div>
            </div>
          } @else {
            <div
              [attr.class]="
                'mt-6 flex items-start gap-3 rounded-lg border p-4 ' +
                (daysLeft() <= 14 ? 'border-red-200 bg-red-50 text-red-900' : 'border-amber-200 bg-amber-50 text-amber-900')
              "
            >
              <svg lucide="CalendarClock" class="mt-0.5 h-5 w-5 shrink-0"></svg>
              <div class="text-sm">
                <p class="font-medium">Comment due {{ formatLgDate(a.commentDueDate) }}{{ daysLeft() >= 0 ? ' — ' + daysLeft() + ' day' + (daysLeft() === 1 ? '' : 's') + ' remaining' : ' — overdue' }}</p>
                <p class="mt-0.5">If no comment is submitted before the due date, the application proceeds without local government input.</p>
              </div>
            </div>
          }

          <reviewer-brief
            [subjectType]="a.licenceType + ' ' + a.applicationType + ' referral'"
            [title]="a.establishmentName"
            reviewerRole="local government"
            [disabled]="isComplete() || exemptFromComment()"
            [decisionOptions]="decisionOptions"
            [facts]="briefFacts()"
            [signals]="signals()"
            [history]="briefHistory()"
            className="mt-6"
          />

          <div class="mt-6 grid gap-6 lg:grid-cols-[1fr_360px]">
            <!-- Left column: tabbed details -->
            <div>
              <div uiTabs value="overview">
                <div uiTabsList class="flex-wrap">
                  <button uiTabsTrigger value="overview">Overview</button>
                  <button uiTabsTrigger value="establishment">Establishment</button>
                  <button uiTabsTrigger value="documents">Documents</button>
                  <button uiTabsTrigger value="history">History</button>
                  <button uiTabsTrigger value="applicant-history" class="gap-1.5">
                    <svg lucide="History" class="h-3.5 w-3.5"></svg>
                    Applicant history
                  </button>
                  <button uiTabsTrigger value="messages" class="gap-1.5">
                    <svg lucide="MessageSquare" class="h-3.5 w-3.5"></svg>
                    Messages
                    @if (unreadMessages() > 0) {
                      <span uiBadge class="h-5 min-w-5 justify-center rounded-full bg-bc-gold px-1.5 text-[11px] text-bc-blue hover:bg-bc-gold">{{ unreadMessages() }}</span>
                    }
                  </button>
                </div>

                <!-- Overview -->
                <div uiTabsContent value="overview" class="mt-4 space-y-6">
                  <section class="rounded-lg border border-border bg-card p-5">
                    <h2 class="text-sm font-semibold uppercase tracking-wide text-muted-foreground">Request details</h2>
                    <p class="mt-2 leading-relaxed text-foreground">{{ a.description }}</p>
                    <dl class="mt-4 grid grid-cols-2 gap-4 text-sm">
                      <div lgReviewDetail label="Application type" [value]="a.applicationType"></div>
                      <div lgReviewDetail label="Licence type" [value]="a.licenceType"></div>
                      <div lgReviewDetail label="Submitted" [value]="formatLgDate(a.submittedDate)"></div>
                      <div lgReviewDetail label="Jurisdiction" [value]="a.jurisdiction"></div>
                    </dl>
                  </section>

                  <section class="rounded-lg border border-border bg-card p-5">
                    <h2 class="text-sm font-semibold uppercase tracking-wide text-muted-foreground">Hours of operation</h2>
                    <ul class="mt-3 divide-y divide-border text-sm">
                      @for (h of a.hours; track h.day) {
                        <li class="flex justify-between py-1.5">
                          <span class="text-muted-foreground">{{ h.day }}</span>
                          <span class="font-medium text-foreground">{{ h.hours }}</span>
                        </li>
                      }
                    </ul>
                  </section>
                </div>

                <!-- Establishment -->
                <div uiTabsContent value="establishment" class="mt-4 space-y-6">
                  <section class="rounded-lg border border-border bg-card p-5">
                    <h2 class="text-sm font-semibold uppercase tracking-wide text-muted-foreground">Compliance snapshot</h2>
                    <div class="mt-3 grid grid-cols-3 gap-3">
                      <div lgRiskStat icon="ShieldAlert" label="Risk level" [value]="a.riskLevel" [tone]="a.riskLevel === 'High' ? 'red' : a.riskLevel === 'Medium' ? 'amber' : 'green'"></div>
                      <div lgRiskStat icon="AlertTriangle" label="Contraventions" [value]="'' + a.contraventions" tone="amber"></div>
                      <div lgRiskStat icon="FileText" label="Incidents" [value]="'' + a.incidents" tone="muted"></div>
                    </div>
                  </section>

                  <section class="rounded-lg border border-border bg-card p-5">
                    <h2 class="text-sm font-semibold uppercase tracking-wide text-muted-foreground">Business &amp; applicant</h2>
                    <dl class="mt-3 grid grid-cols-2 gap-4 text-sm">
                      <div lgReviewDetail label="Business name" [value]="a.businessName"></div>
                      <div lgReviewDetail label="Applicant" [value]="a.applicantName"></div>
                    </dl>
                  </section>

                  <section class="rounded-lg border border-border bg-card p-5">
                    <h2 class="flex items-center gap-2 text-sm font-semibold uppercase tracking-wide text-muted-foreground">
                      <svg lucide="Users2" class="h-4 w-4"></svg> Contacts
                    </h2>
                    <ul class="mt-3 space-y-3">
                      @for (c of a.contacts; track c.email) {
                        <li class="rounded-md border border-border p-3">
                          <div class="flex items-center justify-between">
                            <span class="font-medium text-foreground">{{ c.name }}</span>
                            <span uiBadge variant="secondary">{{ c.role }}</span>
                          </div>
                          <div class="mt-2 flex flex-col gap-1 text-sm text-muted-foreground">
                            <a [attr.href]="'tel:' + c.phone" class="flex items-center gap-2 hover:text-[#013366]">
                              <svg lucide="Phone" class="h-3.5 w-3.5"></svg> {{ c.phone }}
                            </a>
                            <a [attr.href]="'mailto:' + c.email" class="flex items-center gap-2 hover:text-[#013366]">
                              <svg lucide="Mail" class="h-3.5 w-3.5"></svg> {{ c.email }}
                            </a>
                          </div>
                        </li>
                      }
                    </ul>
                  </section>
                </div>

                <!-- Documents -->
                <div uiTabsContent value="documents" class="mt-4">
                  <section class="rounded-lg border border-border bg-card p-5">
                    <h2 class="text-sm font-semibold uppercase tracking-wide text-muted-foreground">Attached documents</h2>
                    <ul class="mt-3 divide-y divide-border">
                      @for (d of a.documents; track d.name) {
                        <li class="flex items-center justify-between py-2.5">
                          <div class="flex items-center gap-3">
                            <svg lucide="FileText" class="h-5 w-5 text-[#013366]"></svg>
                            <div>
                              <p class="text-sm font-medium text-foreground">{{ d.name }}</p>
                              <p class="text-xs text-muted-foreground">{{ d.uploadedBy }} &middot; {{ formatLgDate(d.uploadedDate) }}</p>
                            </div>
                          </div>
                          <button uiButton variant="ghost" size="sm" class="gap-1.5 text-[#013366]" (click)="downloadDocument(a, d)">
                            <svg lucide="Download" class="h-4 w-4"></svg> Download
                          </button>
                        </li>
                      }
                    </ul>
                    <p class="mt-3 text-xs text-muted-foreground">Need something else from the applicant? Use &ldquo;Request documents&rdquo; in the review panel.</p>
                  </section>

                  <section class="mt-4 rounded-lg border border-border bg-card p-5">
                    <h2 class="text-sm font-semibold uppercase tracking-wide text-muted-foreground">Applicant uploads</h2>
                    <p class="mt-1 text-xs text-muted-foreground">Files the applicant attached to this referral. Only virus-scanned files can be opened.</p>
                    <div class="mt-3">
                      <document-manager [subjectId]="a.jobNumber" [actor]="reviewer()" [readOnly]="true" />
                    </div>
                  </section>
                </div>

                <!-- History -->
                <div uiTabsContent value="history" class="mt-4">
                  <section class="rounded-lg border border-border bg-card p-5">
                    <h2 class="text-sm font-semibold uppercase tracking-wide text-muted-foreground">Status history</h2>
                    <ol class="mt-4 space-y-4">
                      @for (h of a.history; track h.label + '-' + $index) {
                        <li class="flex gap-3">
                          <div class="flex flex-col items-center">
                            <span class="mt-1 h-2.5 w-2.5 rounded-full bg-[#013366]"></span>
                            @if ($index < a.history.length - 1) {
                              <span class="my-1 w-px flex-1 bg-border"></span>
                            }
                          </div>
                          <div class="pb-1">
                            <p class="text-sm font-medium text-foreground">{{ h.label }}</p>
                            <p class="text-xs text-muted-foreground">{{ h.actor }} &middot; {{ formatLgDate(h.date) }}</p>
                          </div>
                        </li>
                      }
                    </ol>
                  </section>
                </div>

                <!-- Linked applications + licences for this applicant/business -->
                <div uiTabsContent value="applicant-history" class="mt-4">
                  <applicant-history [current]="a" />
                </div>

                <!-- Shared thread with the applicant and LCRB staff -->
                <div uiTabsContent value="messages" class="mt-4">
                  <case-thread
                    [threadId]="a.jobNumber"
                    kind="application"
                    [status]="a.status"
                    [subjectLabel]="a.licenceType + ' ' + a.jobNumber"
                    title="Conversation with applicant and LCRB"
                    [hrefFor]="hrefFor()"
                  />
                </div>
              </div>
            </div>

            <!-- Right column: review actions -->
            <aside class="lg:sticky lg:top-6 lg:self-start">
              <div class="rounded-lg border border-border bg-card p-5">
                <h2 class="text-base font-semibold text-foreground">Submit your comment</h2>
                <p class="mt-1 text-sm text-muted-foreground">Reviewing as {{ reviewer() }}</p>

                @if (isCancelled()) {
                  <div class="mt-4 flex items-start gap-3 rounded-md border border-zinc-200 bg-zinc-50 p-3 text-sm">
                    <svg lucide="Ban" class="mt-0.5 h-5 w-5 shrink-0 text-zinc-500"></svg>
                    <div>
                      <p class="font-medium text-foreground">Application withdrawn</p>
                      <p class="mt-1 text-muted-foreground">{{ withdrawnNote() }}</p>
                    </div>
                  </div>
                } @else if (submitted()) {
                  <div class="mt-4 flex items-start gap-3 rounded-md border border-green-200 bg-green-50 p-3 text-sm text-green-900">
                    <svg lucide="CheckCircle2" class="mt-0.5 h-5 w-5 shrink-0"></svg>
                    <p>{{ submitted() }}</p>
                  </div>
                } @else if (exemptFromComment()) {
                  <div class="mt-4 space-y-3">
                    <div class="flex items-start gap-3 rounded-md border border-border bg-muted/60 p-3 text-sm">
                      <svg lucide="MessageSquareOff" class="mt-0.5 h-5 w-5 shrink-0 text-muted-foreground"></svg>
                      <div>
                        <p class="font-medium text-foreground">Commenting is turned off</p>
                        <p class="mt-1 text-muted-foreground">
                          Your jurisdiction has opted out of commenting on licence applications, so there is nothing to submit here. This application has already flowed through to the LCRB and is not waiting on you.
                        </p>
                      </div>
                    </div>
                    <p class="text-xs text-muted-foreground">
                      Only the account owner can change this, in
                      <a [routerLink]="path('account.settings')" class="font-medium text-[#013366] underline underline-offset-2 hover:no-underline">Account Settings → Commenting</a>.
                    </p>
                  </div>
                } @else if (isComplete()) {
                  <div class="mt-4 rounded-md border border-border bg-muted/50 p-3 text-sm text-muted-foreground">
                    A comment has already been recorded for this application ({{ statusLabels[a.status] }}).
                  </div>
                } @else {
                  <div class="mt-4 space-y-4">
                    <div>
                      <div class="flex items-center justify-between gap-2">
                        <label uiLabel for="comment" class="text-sm">Comments to LCRB</label>
                        <draft-rationale-button [input]="commentRationale()" (draft)="comment.set($event)" />
                      </div>
                      <textarea
                        uiTextarea
                        id="comment"
                        [value]="comment()"
                        (input)="comment.set($any($event.target).value)"
                        placeholder="Optional notes to accompany your recommendation…"
                        class="mt-1.5"
                        rows="4"
                      ></textarea>
                    </div>

                    <div class="space-y-2">
                      <button uiButton class="w-full justify-start gap-2 bg-green-700 hover:bg-green-800" (click)="submitDecision(a, 'no-comment')">
                        <svg lucide="CheckCircle2" class="h-4 w-4"></svg>
                        No comment / No objection
                      </button>
                      <button uiButton class="w-full justify-start gap-2 bg-teal-700 hover:bg-teal-800" (click)="submitDecision(a, 'community-review')">
                        <svg lucide="MessageSquare" class="h-4 w-4"></svg>
                        Recommend community review
                      </button>
                      <button uiButton variant="outline" class="w-full justify-start gap-2 border-red-300 text-red-700 hover:bg-red-50" (click)="submitDecision(a, 'reject')">
                        <svg lucide="XCircle" class="h-4 w-4"></svg>
                        Recommend rejection
                      </button>
                    </div>

                    <div class="border-t border-border pt-4">
                      <div class="flex items-center justify-between gap-2">
                        <label uiLabel for="docs" class="text-sm">Request documents from applicant</label>
                        <draft-rationale-button [input]="docsRationale()" (draft)="docMessage.set($event)" />
                      </div>
                      <textarea
                        uiTextarea
                        id="docs"
                        [value]="docMessage()"
                        (input)="docMessage.set($any($event.target).value)"
                        placeholder="Describe what additional information is needed…"
                        class="mt-1.5"
                        rows="2"
                      ></textarea>
                      <button uiButton variant="outline" class="mt-2 w-full gap-2" [disabled]="!docMessage().trim()" (click)="submitDecision(a, 'documents-requested')">
                        <svg lucide="Clock" class="h-4 w-4"></svg>
                        Request documents
                      </button>
                    </div>
                  </div>
                }
              </div>
            </aside>
          </div>
        </main>
        <bc-gov-footer />
      </div>
    } @else {
      <div class="flex min-h-screen flex-col bg-background">
        <bc-gov-header role="local-government" currentPage="/local-government/queue" />
        <main id="main-content" class="mx-auto max-w-3xl flex-1 px-4 py-16 text-center">
          <h1 class="text-2xl font-semibold text-foreground">Application not found</h1>
          <p class="mt-2 text-muted-foreground">We couldn&apos;t find an application with job #{{ id() }}.</p>
          <a uiButton class="mt-6 bg-[#013366] hover:bg-[#012a52]" [routerLink]="path('lg.queue')">Back to review queue</a>
        </main>
        <bc-gov-footer />
      </div>
    }
  `,
})
export class LocalGovernmentReviewPage {
  protected readonly path = path;
  private readonly lgReviewRepository = inject(LG_REVIEW_REPOSITORY);
  private readonly messagingRepository = inject(MESSAGING_REPOSITORY);
  readonly id = input.required<string>();

  private readonly session = injectSession().session;
  private readonly store = this.lgReviewRepository.store();
  private readonly preference = this.lgReviewRepository.commenting().preference;

  protected readonly reviewer = computed(() => actorName(this.session()));
  protected readonly app = computed(() => this.store.getByJobNumber(this.id()));

  // When the jurisdiction has opted out of commenting, referrals received after the opt-out took effect are not held for comment: they flow straight through to the LCRB.
  protected readonly exemptFromComment = computed(() => {
    const app = this.app();
    return app ? isReferralExempt(this.preference(), app.submittedDate) : false;
  });

  protected readonly comment = signal('');
  protected readonly docMessage = signal('');
  protected readonly submitted = signal<string | null>(null);

  protected readonly daysLeft = computed(() => {
    const app = this.app();
    return app ? daysUntilDue(app.commentDueDate) : 0;
  });
  protected readonly unreadMessages = this.messagingRepository.threadUnread(this.id, 'local-government');

  protected readonly isCancelled = computed(() => this.app()?.status === 'cancelled');
  protected readonly isComplete = computed(() => {
    const app = this.app();
    return !!app && (this.isCancelled() || ['no-comment', 'community-review', ...LG_TERMINAL_STATUSES].includes(app.status));
  });
  // What the jurisdiction had recorded before the applicant withdrew, if anything.
  protected readonly priorWork = computed(() => {
    const cancellation = this.app()?.cancellation;
    return cancellation ? (cancellation.priorStatus === 'pending-review' ? null : LG_STATUS_LABELS[cancellation.priorStatus]) : null;
  });
  protected readonly cancelNote = computed(() => {
    const priorWork = this.priorWork();
    return priorWork
      ? ` Your recorded response (${priorWork}) stays on file and has been passed to LCRB with the withdrawal.`
      : ' No comment had been recorded; the referral is closed.';
  });
  protected readonly withdrawnNote = computed(() => {
    const priorWork = this.priorWork();
    return priorWork
      ? `Your response "${priorWork}" was recorded before the applicant cancelled and remains part of the file. No further comment can be added.`
      : 'The applicant cancelled before a comment was recorded. No action is required.';
  });

  protected readonly decisionOptions = ['No comment / No objection', 'Recommend community review', 'Recommend rejection'];

  protected readonly signals = computed(() => {
    const app = this.app();
    if (!app) return [];
    return [
      app.riskLevel === 'High' ? 'High compliance risk level' : '',
      app.contraventions > 0 ? `${app.contraventions} prior contravention(s)` : '',
      app.incidents > 0 ? `${app.incidents} prior incident(s)` : '',
    ].filter(Boolean);
  });
  protected readonly briefFacts = computed(() => {
    const app = this.app();
    if (!app) return [];
    return [
      { label: 'Application type', value: app.applicationType },
      { label: 'Licence type', value: app.licenceType },
      { label: 'Jurisdiction', value: app.jurisdiction },
      { label: 'Address', value: `${app.address}, ${app.city} ${app.postalCode}` },
      { label: 'Compliance risk level', value: app.riskLevel },
      { label: 'Contraventions on record', value: String(app.contraventions) },
      { label: 'Incidents on record', value: String(app.incidents) },
      { label: 'Hours of operation', value: app.hours.map((h) => `${h.day}: ${h.hours}`).join('; ') },
      { label: 'Request details', value: app.description || '—' },
    ];
  });
  protected readonly briefHistory = computed(() =>
    (this.app()?.history ?? []).map((h) => ({ action: h.label, actor: h.actor, date: formatLgDate(h.date) })),
  );
  protected readonly hrefFor = computed(() => {
    const jobNumber = this.app()?.jobNumber ?? '';
    return {
      licensee: path('applications.submitted', { id: jobNumber }),
      lcrb: `${path('staff.conversations')}?thread=${encodeURIComponent(jobNumber)}`,
      'local-government': path('lg.review', { id: jobNumber }),
    };
  });

  // Shared context handed to the AI decision-rationale drafter.
  private readonly rationaleBase = computed(() => {
    const app = this.app()!;
    return {
      subjectType: `${app.licenceType} ${app.applicationType} referral`,
      title: app.establishmentName,
      reviewerRole: 'local government',
      facts: [
        { label: 'Application type', value: app.applicationType },
        { label: 'Licence type', value: app.licenceType },
        { label: 'Jurisdiction', value: app.jurisdiction },
        { label: 'Address', value: `${app.address}, ${app.city}` },
        { label: 'Compliance risk level', value: app.riskLevel },
        { label: 'Contraventions on record', value: String(app.contraventions) },
        { label: 'Incidents on record', value: String(app.incidents) },
      ],
      signals: this.signals(),
    };
  });
  protected readonly commentRationale = computed<DraftRationaleInput>(() => ({
    ...this.rationaleBase(),
    decision: 'Local government comment',
    tone: 'neutral',
    reviewerNotes: this.comment(),
  }));
  protected readonly docsRationale = computed<DraftRationaleInput>(() => ({
    ...this.rationaleBase(),
    decision: 'Request documents',
    tone: 'changes',
    reviewerNotes: this.docMessage(),
  }));

  protected readonly holdDays = COMMENT_HOLD_DAYS;
  protected readonly statusLabels = LG_STATUS_LABELS;
  protected readonly formatLgDate = formatLgDate;
  protected readonly formatCancelledDate = formatCancelledDate;

  protected downloadApplication(app: LgApplication): void {
    downloadLgApplicationPdf(app, this.reviewer());
  }

  protected downloadDocument(app: LgApplication, d: LgApplication['documents'][number]): void {
    downloadLgDocumentPdf(app, d, this.reviewer());
  }

  protected submitDecision(app: LgApplication, decision: LgDecision): void {
    if (decision === 'documents-requested') {
      this.store.requestDocuments(app.jobNumber, this.docMessage(), this.reviewer());
      this.submitted.set('Documents requested from the applicant. The comment clock is paused.');
      return;
    }
    this.store.recordDecision(app.jobNumber, decision, this.comment(), this.reviewer());
    this.submitted.set(`Comment recorded: ${LG_STATUS_LABELS[decisionToStatus(decision)]}.`);
  }
}
