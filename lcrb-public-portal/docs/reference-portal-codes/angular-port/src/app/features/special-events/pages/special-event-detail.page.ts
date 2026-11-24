/** Applicant-facing Special Event Permit detail page; `id` is a route-param input and `Detail` is `div[sepDetail]` because the `dl` grid places its direct children. */
import { ChangeDetectionStrategy, Component, computed, inject, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { toast } from '@spartan-ng/brain/sonner';
import { StatusExplainer } from '@shared/ui/ai/status-explainer';
import { CancelApplicationDialog, CancelApplicationTrigger } from '@shared/ui/applications/cancel-application-dialog';
import { BCGovFooter } from '@core/layout/footer';
import { BCGovHeader } from '@core/layout/header';
import { PAGE_HERO } from '@shared/ui/bc-gov/page-hero';
import { SepStatusBadge } from '@shared/ui/bc-gov/sep-status-badge';
import { CaseThread } from '@shared/ui/case-thread/case-thread';
import { injectSession } from '@core/session/session-provider';
import { Button } from '@shared/ui/primitives/button';
import type { IconName } from '@shared/ui/primitives/icons';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { Separator } from '@shared/ui/primitives/separator';
import { formatEventDate, formatServiceWindow } from '@domain/special-events/seps';
import { actorName } from '@domain/roles';
import { downloadSepPdf } from '@shared/util/pdf/sep-pdf';
import { SPECIAL_EVENTS_REPOSITORY } from '@data-access/repositories/special-events/special-events.repository';
import { path } from '@/app/route-registry';

@Component({
  selector: 'div[sepDetail]',
  imports: [LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { class: 'flex items-start gap-3' },
  template: `
    <svg [lucide]="icon()" class="mt-0.5 h-5 w-5 shrink-0 text-[#013366]" aria-hidden="true"></svg>
    <div>
      <dt class="text-xs font-medium uppercase tracking-wide text-muted-foreground">{{ label() }}</dt>
      <dd class="mt-0.5 text-sm text-foreground">{{ value() }}</dd>
    </div>
  `,
})
export class SepDetail {
  readonly icon = input.required<IconName>();
  readonly label = input.required<string>();
  readonly value = input.required<string>();
}

@Component({
  selector: 'special-event-detail-page',
  imports: [
    RouterLink,
    BCGovHeader,
    BCGovFooter,
    ...PAGE_HERO,
    Button,
    LucideIcon,
    Separator,
    SepStatusBadge,
    CancelApplicationDialog,
    CancelApplicationTrigger,
    CaseThread,
    StatusExplainer,
    SepDetail,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (record(); as r) {
      <div class="flex min-h-screen flex-col bg-background">
        <bc-gov-header currentPage="/special-events" />
        <page-hero
          containerClassName="mx-auto w-full max-w-5xl px-4"
          [breadcrumbs]="breadcrumbs()"
          [title]="r.eventName"
        >
          <sep-status-badge heroBadge [status]="r.status" />
          <ng-container heroMeta>{{ r.id }} · {{ r.eventType }} · {{ r.jurisdiction }}</ng-container>
          <ng-container heroActions>
            @if (isApproved()) {
              <button uiButton class="gap-2 bg-bc-gold font-semibold text-bc-blue hover:bg-bc-gold/90" (click)="downloadPermit()">
                <svg lucide="Download" class="h-4 w-4"></svg>
                Download permit
              </button>
            }
            @if (isPending()) {
              <cancel-application-dialog
                noun="permit"
                [subject]="r.eventName + ' (' + r.id + ')'"
                [feePaid]="false"
                [consequence]="consequence()"
                (confirmed)="handleCancel($event)"
              >
                <button
                  cancelTrigger
                  uiButton
                  variant="outline"
                  class="gap-2 border-white/40 bg-transparent text-white hover:bg-white/10 hover:text-white"
                >
                  <svg lucide="XCircle" class="h-4 w-4"></svg>
                  Cancel permit
                </button>
              </cancel-application-dialog>
            }
          </ng-container>
        </page-hero>

        <main id="main-content" class="mx-auto w-full max-w-5xl flex-1 px-4 py-8">
          <!-- Status banner -->
          @if (isApproved()) {
            <div class="mt-6 flex items-start gap-3 rounded-lg border border-green-200 bg-green-50 p-4 text-green-900">
              <svg lucide="CheckCircle2" class="mt-0.5 h-5 w-5 shrink-0"></svg>
              <div class="text-sm">
                <p class="font-semibold">Your permit is approved</p>
                <p class="mt-0.5">Download your permit and display it prominently at your event. Review the conditions below.</p>
              </div>
            </div>
          }
          @if (r.status === 'changes-requested') {
            <div class="mt-6 flex items-start gap-3 rounded-lg border border-orange-200 bg-orange-50 p-4 text-orange-900">
              <svg lucide="AlertTriangle" class="mt-0.5 h-5 w-5 shrink-0"></svg>
              <div class="text-sm">
                <p class="font-semibold">Changes requested</p>
                <p class="mt-0.5">A reviewer has asked for changes. See the latest note in the history below, then resubmit.</p>
              </div>
            </div>
          }
          @if (r.status === 'denied') {
            <div class="mt-6 flex items-start gap-3 rounded-lg border border-red-200 bg-red-50 p-4 text-red-900">
              <svg lucide="XCircle" class="mt-0.5 h-5 w-5 shrink-0"></svg>
              <div class="text-sm">
                <p class="font-semibold">Permit denied</p>
                <p class="mt-0.5">See the reason in the history below.</p>
              </div>
            </div>
          }
          @if (r.status === 'cancelled') {
            <div class="mt-6 flex items-start gap-3 rounded-lg border border-zinc-300 bg-zinc-50 p-4 text-zinc-800">
              <svg lucide="Ban" class="mt-0.5 h-5 w-5 shrink-0 text-zinc-600"></svg>
              <div class="text-sm">
                <p class="font-semibold">You cancelled this permit{{ cancellationEntry() ? ' on ' + cancellationEntry()!.date : '' }}</p>
                <p class="mt-0.5">
                  {{ (cancellationEntry()?.note ? cancellationEntry()!.note + '. ' : '') + r.jurisdiction }} and LCRB were notified. This permit cannot be reinstated; apply again if your
                  plans change.
                </p>
              </div>
            </div>
          }

          <div class="mt-6 grid gap-6 lg:grid-cols-3">
            <div class="space-y-6 lg:col-span-2">
              <section class="rounded-lg border border-border bg-card p-6">
                <h2 class="text-lg font-semibold text-foreground">Event details</h2>
                <dl class="mt-4 grid gap-4 sm:grid-cols-2">
                  <div sepDetail icon="Calendar" label="Date(s)" [value]="eventDate()"></div>
                  <div sepDetail icon="Clock" label="Time" [value]="serviceWindow()"></div>
                  <div sepDetail icon="MapPin" label="Venue" [value]="r.venueName + ', ' + r.address + ', ' + r.city"></div>
                  <div sepDetail icon="Users" label="Expected attendance" [value]="'' + r.expectedAttendance"></div>
                  <div sepDetail icon="FileText" label="Liquor service" [value]="r.serviceType"></div>
                  @if (r.liquorTypes && r.liquorTypes.length > 0) {
                    <div sepDetail icon="Wine" label="Liquor types" [value]="liquorTypes()"></div>
                  }
                  @if (isNumber(r.estimatedDrinks)) {
                    <div sepDetail icon="Wine" label="Estimated drinks served" [value]="'' + r.estimatedDrinks"></div>
                  }
                  @if (isBoolean(r.hasSecurity)) {
                    <div sepDetail icon="ShieldCheck" label="Security staff" [value]="securityLabel()"></div>
                  }
                </dl>
                @if (r.description) {
                  <div uiSeparator class="my-4"></div>
                  <p class="text-sm leading-relaxed text-muted-foreground">{{ r.description }}</p>
                }
              </section>

              <section class="rounded-lg border border-border bg-card p-6">
                <div class="flex items-center justify-between">
                  <h2 class="text-lg font-semibold text-foreground">Permit conditions</h2>
                  <span class="text-sm text-muted-foreground">{{ conditions().length }} applied</span>
                </div>
                <ul class="mt-4 space-y-2">
                  @for (c of conditions(); track c.id) {
                    <li class="flex items-start gap-2 rounded-md border border-border bg-muted/30 p-3">
                      <svg lucide="CheckCircle2" class="mt-0.5 h-4 w-4 shrink-0 text-green-600"></svg>
                      <span class="text-sm text-foreground">{{ c.text }}</span>
                    </li>
                  }
                  @if (conditions().length === 0) {
                    <li class="rounded-md border border-dashed border-border p-4 text-center text-sm text-muted-foreground">
                      No conditions have been applied yet.
                    </li>
                  }
                </ul>
              </section>
            </div>

            <div class="space-y-6">
              <!-- A direct child of space-y-6: a block host takes the margin (its root element varies by state). -->
              <status-explainer
                [style.display]="'block'"
                subjectType="Special Event Permit"
                [title]="r.eventName"
                [status]="r.status"
                [history]="explainerHistory()"
              />
              <section class="rounded-lg border border-border bg-card p-6">
                <h2 class="text-lg font-semibold text-foreground">History</h2>
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
            </div>
          </div>

          <!-- Shared thread with police and LCRB: open while the permit is in process -->
          <div class="mt-6" id="conversation">
            <case-thread
              [threadId]="r.id"
              kind="sep"
              [status]="r.status"
              [subjectLabel]="'Special Event Permit ' + r.id"
              [title]="'Conversation with ' + r.jurisdiction + ' and LCRB'"
              [hrefFor]="threadHrefs()"
            />
          </div>
        </main>
        <bc-gov-footer />
      </div>
    } @else {
      <div class="flex min-h-screen flex-col bg-background">
        <bc-gov-header currentPage="/special-events" />
        <main id="main-content" class="mx-auto max-w-3xl flex-1 px-4 py-16 text-center">
          <h1 class="text-2xl font-semibold text-foreground">Permit not found</h1>
          <p class="mt-2 text-muted-foreground">We couldn't find a Special Event Permit with ID {{ id() }}.</p>
          <a uiButton class="mt-6 bg-[#013366] hover:bg-[#012a52]" [routerLink]="path('special-events.list')">Back to my permits</a>
        </main>
        <bc-gov-footer />
      </div>
    }
  `,
})
export class SpecialEventDetailPage {
  protected readonly path = path;
  private readonly specialEventsRepository = inject(SPECIAL_EVENTS_REPOSITORY);
  readonly id = input.required<string>();

  private readonly sepStore = this.specialEventsRepository.store();
  private readonly sessionProvider = injectSession();

  protected readonly record = computed(() => this.sepStore.records().find((r) => r.id === this.id()));
  private readonly applicant = computed(() => actorName(this.sessionProvider.session()));

  protected readonly isApproved = computed(() => {
    const s = this.record()?.status;
    return s === 'approved' || s === 'auto-approved';
  });
  protected readonly isPending = computed(() => {
    const s = this.record()?.status;
    return s === 'submitted' || s === 'under-review' || s === 'changes-requested';
  });
  protected readonly conditions = computed(() => this.record()?.conditions ?? []);
  protected readonly cancellationEntry = computed(() => {
    const r = this.record();
    return r?.status === 'cancelled' ? [...r.history].reverse().find((h) => h.action === 'Cancelled by applicant') : undefined;
  });
  private readonly underPoliceReview = computed(() => {
    const s = this.record()?.status;
    return s === 'under-review' || s === 'changes-requested';
  });

  protected readonly breadcrumbs = computed(() => [
    { label: 'Dashboard', href: path('dashboard.home') },
    { label: 'Special Event Permits', href: path('special-events.list') },
    { label: this.record()?.eventName ?? '' },
  ]);

  protected readonly consequence = computed(() => {
    const r = this.record()!;
    return this.underPoliceReview()
      ? `${r.jurisdiction} has started reviewing this permit${r.assignedOfficer ? ` (${r.assignedOfficer})` : ''}; they will be told it was withdrawn.`
      : `${r.jurisdiction} will be notified so the event is removed from their review queue.`;
  });

  protected readonly eventDate = computed(() => formatEventDate(this.record()!));
  protected readonly serviceWindow = computed(() => formatServiceWindow(this.record()!));
  protected readonly liquorTypes = computed(() =>
    (this.record()!.liquorTypes ?? []).map((t) => t.charAt(0).toUpperCase() + t.slice(1)).join(', '),
  );
  protected readonly securityLabel = computed(() => {
    const r = this.record()!;
    return r.hasSecurity ? (r.securityStaffCount ? `Yes — ${r.securityStaffCount} on site` : 'Yes') : 'No';
  });
  protected readonly explainerHistory = computed(() =>
    this.record()!.history.map((h) => ({ action: h.action, actor: h.actor, date: h.date, note: h.note })),
  );
  protected readonly threadHrefs = computed(() => {
    const id = this.record()!.id;
    return {
      applicant: path('special-events.detail', { id: id }),
      licensee: path('special-events.detail', { id: id }),
      lcrb: `${path('staff.conversations')}?thread=${encodeURIComponent(id)}`,
      police: path('police.review', { id: id }),
    };
  });

  protected isNumber(v: unknown): boolean {
    return typeof v === 'number';
  }
  protected isBoolean(v: unknown): boolean {
    return typeof v === 'boolean';
  }

  protected downloadPermit(): void {
    const r = this.record()!;
    downloadSepPdf(r, 'permit', { preparedFor: r.applicantName });
  }

  protected handleCancel(reason: string): void {
    const r = this.record()!;
    this.sepStore.cancel(r.id, this.applicant(), reason);
    toast.success('Permit cancelled', {
      description: `${r.eventName} (${r.id}) has been withdrawn. Police and LCRB have been notified.`,
    });
  }
}
