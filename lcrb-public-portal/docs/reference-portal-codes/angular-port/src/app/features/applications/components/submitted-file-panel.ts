/** Live status, withdraw action, cancelled banner and case conversation for the submitted-application page; a cancellation overrides the seeded status. Also exports SubmittedStatusBadge. */
import { ChangeDetectionStrategy, Component, computed, inject, input } from '@angular/core';
import { toast } from '@spartan-ng/brain/sonner';
import { CancelApplicationDialog } from '@shared/ui/applications/cancel-application-dialog';
import { CaseThread } from '@shared/ui/case-thread/case-thread';
import { injectSession } from '@core/session/session-provider';
import { Badge } from '@shared/ui/primitives/badge';
import { CARD } from '@shared/ui/primitives/card';
import type { IconName } from '@shared/ui/primitives/icons';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { actorName } from '@domain/roles';
import { formatCancelledDate, isCancellable } from '@domain/applications/cancellation';
import { APPLICATIONS_REPOSITORY } from '@data-access/repositories/applications/applications.repository';
import { path } from '@/app/route-registry';

const STATUS_BADGE: Record<'under_review' | 'pending_documents' | 'approved' | 'cancelled', { label: string; color: string; icon: IconName }> = {
  under_review: { label: 'Under Review', color: 'bg-blue-100 text-blue-800 border-blue-200', icon: 'Clock' },
  pending_documents: { label: 'Pending Documents', color: 'bg-yellow-100 text-yellow-800 border-yellow-200', icon: 'AlertCircle' },
  approved: { label: 'Approved', color: 'bg-green-100 text-green-800 border-green-200', icon: 'CheckCircle2' },
  cancelled: { label: 'Cancelled', color: 'bg-zinc-100 text-zinc-600 border-zinc-200', icon: 'Ban' },
};

type BadgeStatus = keyof typeof STATUS_BADGE;

@Component({
  selector: 'submitted-status-badge',
  imports: [Badge, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <span uiBadge variant="outline" [class]="cfg().color + ' flex items-center gap-1'">
      <svg [lucide]="cfg().icon" class="h-3 w-3"></svg>
      {{ cfg().label }}
    </span>
  `,
})
export class SubmittedStatusBadge {
  private readonly applicationsRepository = inject(APPLICATIONS_REPOSITORY);
  readonly id = input.required<string>();
  readonly status = input.required<string>();

  private readonly cancellation = this.applicationsRepository.cancellation(() => this.id());
  protected readonly cfg = computed(() => {
    const status = this.status();
    const key: BadgeStatus = this.cancellation() ? 'cancelled' : (status as BadgeStatus) in STATUS_BADGE ? (status as BadgeStatus) : 'under_review';
    return STATUS_BADGE[key];
  });
}

@Component({
  selector: 'submitted-file-panel',
  imports: [...CARD, CancelApplicationDialog, CaseThread, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  // The page spaces its direct children with `space-y-6`, so the host takes that spacing and re-applies it to the fragment (a display: contents host would drop the margin).
  host: { class: 'block space-y-6' },
  template: `
    @if (cancellation(); as c) {
      <div uiCard class="border-zinc-300 bg-zinc-50">
        <div uiCardContent class="flex gap-3 p-5">
          <svg lucide="Ban" class="mt-0.5 h-5 w-5 shrink-0 text-zinc-600"></svg>
          <div class="text-sm">
            <p class="font-semibold text-zinc-900">Cancelled on {{ cancelledDate() }}</p>
            <p class="mt-1 text-zinc-700">
              You withdrew this application{{ c.reason ? ' — ' + c.reason : '' }}. LCRB{{ lgInvolved() ? ' and the local government' : '' }} have
              been notified. Fees paid are not refunded; you can start a new application at any time.
            </p>
          </div>
        </div>
      </div>
    } @else if (cancellable()) {
      <div uiCard>
        <div uiCardContent class="flex flex-col gap-3 p-5 sm:flex-row sm:items-center sm:justify-between">
          <div class="text-sm">
            <p class="font-semibold text-foreground">No longer proceeding?</p>
            <p class="mt-0.5 text-muted-foreground">
              You can withdraw this application while it is under review. Reviewers who have already started are notified and their work stays on
              file.
            </p>
          </div>
          <cancel-application-dialog [subject]="type() + ' (' + id() + ')'" [consequence]="consequence()" (confirmed)="handleCancel($event)" />
        </div>
      </div>
    }

    <case-thread
      [threadId]="id()"
      kind="application"
      [status]="effectiveStatus()"
      [subjectLabel]="type() + ' ' + id()"
      [hrefFor]="hrefFor()"
      [title]="lgInvolved() ? 'Conversation with LCRB and City of Victoria' : 'Conversation with LCRB'"
    />
  `,
})
export class SubmittedFilePanel {
  private readonly applicationsRepository = inject(APPLICATIONS_REPOSITORY);
  readonly id = input.required<string>();
  readonly type = input.required<string>();
  readonly status = input.required<string>();
  readonly applicant = input.required<string>();

  private readonly sessionProvider = injectSession();
  protected readonly cancellation = this.applicationsRepository.cancellation(() => this.id());
  private readonly referrals = computed(() => this.applicationsRepository.referralsFor(this.id()));
  protected readonly lgInvolved = computed(() => this.referrals().includes('local-government'));
  protected readonly effectiveStatus = computed(() => (this.cancellation() ? 'cancelled' : this.status()));
  protected readonly cancellable = computed(() => isCancellable(this.status()));
  protected readonly cancelledDate = computed(() => formatCancelledDate(this.cancellation()!.cancelledAt));
  protected readonly consequence = computed(() =>
    this.lgInvolved() ? 'The City of Victoria has this file for comment; their reviewer will be told it was withdrawn.' : undefined,
  );
  protected readonly hrefFor = computed(() => ({
    licensee: path('applications.submitted', { id: this.id() }),
    lcrb: `${path('staff.conversations')}?thread=${encodeURIComponent(this.id())}`,
    'local-government': path('lg.review', { id: this.id() }),
  }));

  protected handleCancel(reason: string): void {
    this.applicationsRepository.cancel({
      jobNumber: this.id(),
      applicationType: this.type(),
      applicant: this.applicant(),
      actor: actorName(this.sessionProvider.session()),
      reason,
      priorStatus: this.status(),
      referredTo: this.referrals(),
    });
    toast.success('Application cancelled', { description: `${this.type()} (${this.id()}) has been withdrawn.` });
  }
}
