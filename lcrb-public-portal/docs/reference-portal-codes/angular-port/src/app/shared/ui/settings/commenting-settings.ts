/** Setting for LG and Indigenous Nation to opt out of commenting on referrals; owner only, delegates read-only. The Switch moves once the stored value updates. */
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { toast } from '@spartan-ng/brain/sonner';
import { injectSession } from '@core/session/session-provider';
import { ALERT_DIALOG } from '../primitives/alert-dialog';
import { Badge } from '../primitives/badge';
import { Button } from '../primitives/button';
import { CARD } from '../primitives/card';
import { Label } from '../primitives/label';
import { LucideIcon } from '../primitives/lucide-icon';
import { Switch } from '../primitives/switch';
import { formatLgDate } from '@domain/lg-review/due-dates';
import { COMMENT_HOLD_DAYS } from '@domain/lg-review/lg-commenting';
import { actorName, isAccountOwner } from '@domain/roles';
import { LG_REVIEW_REPOSITORY } from '@data-access/repositories/lg-review/lg-review.repository';

@Component({
  selector: 'commenting-settings',
  imports: [ALERT_DIALOG, Badge, Button, CARD, Label, LucideIcon, Switch],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div uiCard>
      <div uiCardHeader>
        <div class="flex flex-wrap items-start justify-between gap-3">
          <div>
            <div uiCardTitle class="flex items-center gap-2">
              @if (optedOut()) {
                <svg lucide="MessageSquareOff" class="h-5 w-5 text-amber-700"></svg>
              } @else {
                <svg lucide="MessageSquare" class="h-5 w-5 text-[#013366]"></svg>
              }
              Commenting on applications
            </div>
            <div uiCardDescription class="mt-1">
              Choose whether {{ jurisdiction() }} is asked to comment on liquor and cannabis licence applications referred to it.
            </div>
          </div>
          <span
            uiBadge
            variant="outline"
            [class]="optedOut() ? 'border-amber-300 bg-amber-50 text-amber-900' : 'border-green-300 bg-green-50 text-green-900'"
          >
            {{ optedOut() ? 'Commenting off' : 'Commenting on' }}
          </span>
        </div>
      </div>

      <div uiCardContent class="space-y-5">
        <div class="flex items-start justify-between gap-4 rounded-lg border border-border bg-muted/40 p-4">
          <div class="flex-1">
            <label uiLabel for="opt-out-commenting" class="text-sm font-medium">Turn off commenting on applications</label>
            <p class="mt-1 text-sm leading-relaxed text-muted-foreground">
              When this is on, we stop sending new applications to you for comment. Applications will go straight to the LCRB for processing without waiting for your input.
            </p>
          </div>
          <button
            uiSwitch
            id="opt-out-commenting"
            [checked]="optedOut()"
            (checkedChange)="handleToggle($event)"
            [disabled]="!owner()"
            [attr.aria-describedby]="!owner() ? 'owner-only-note' : null"
          ></button>
        </div>

        @if (!owner()) {
          <div
            id="owner-only-note"
            class="flex items-start gap-3 rounded-lg border border-border bg-muted/50 p-4 text-sm"
          >
            <svg lucide="Lock" class="mt-0.5 h-4 w-4 shrink-0 text-muted-foreground"></svg>
            <p class="text-muted-foreground">
              Only the account owner can change this setting. You have delegated access to this account, so this is shown to you as view-only. Contact your account owner if this needs to change.
            </p>
          </div>
        }

        <!-- What happens next, the hold-period explanation -->
        <div class="rounded-lg border border-border p-4">
          <h3 class="flex items-center gap-2 text-sm font-semibold text-foreground">
            <svg lucide="Info" class="h-4 w-4 text-[#013366]"></svg>
            What happens when commenting is off
          </h3>
          <ul class="mt-3 space-y-2 text-sm text-muted-foreground">
            <li class="flex gap-2">
              <span aria-hidden="true">&bull;</span>
              <span>New applications in your area are <strong>no longer sent to you</strong> for comment.</span>
            </li>
            <li class="flex gap-2">
              <span aria-hidden="true">&bull;</span>
              <span>The LCRB <strong>does not hold</strong> those applications for the usual {{ holdDays }}-day comment period — they can be processed right away.</span>
            </li>
            <li class="flex gap-2">
              <span aria-hidden="true">&bull;</span>
              <span>Applications <strong>already waiting</strong> for your comment keep their original due date. Nothing in progress changes.</span>
            </li>
            <li class="flex gap-2">
              <span aria-hidden="true">&bull;</span>
              <span>You can turn commenting back on at any time. It will apply to applications referred after that point.</span>
            </li>
          </ul>
        </div>

        @if (optedOut() && grandfathered().length > 0) {
          <div class="flex items-start gap-3 rounded-lg border border-amber-200 bg-amber-50 p-4 text-sm text-amber-900">
            <svg lucide="Clock" class="mt-0.5 h-4 w-4 shrink-0"></svg>
            <div>
              <p class="font-medium">{{ grandfathered().length }} application{{ grandfathered().length === 1 ? '' : 's' }} still awaiting your comment</p>
              <p class="mt-0.5">
                These were referred to you before commenting was turned off, so they keep their original comment deadline. You can still comment on them, or let the deadline pass and they will proceed without your input.
              </p>
            </div>
          </div>
        }

        <!-- Prototype only: every seeded referral is historical, so this lets testers generate a fresh one; remove with simulateIncomingReferral(). -->
        @if (optedOut()) {
          <div class="flex flex-wrap items-center justify-between gap-3 rounded-lg border border-dashed border-border p-4">
            <p class="text-xs text-muted-foreground">
              Prototype testing aid: add a referral dated today to see how a newly arriving application looks while commenting is off.
            </p>
            <button uiButton type="button" variant="outline" size="sm" (click)="simulate()">Simulate new referral</button>
          </div>
        }

        @if (preference().updatedAt) {
          <p class="text-xs text-muted-foreground">Last changed by {{ preference().updatedBy }} on {{ changedAt() }}{{ inEffect() }}</p>
        }
      </div>
    </div>

    <ui-alert-dialog [state]="confirmOpen() ? 'open' : 'closed'" (stateChanged)="confirmOpen.set($event === 'open')">
      <ui-alert-dialog-content *uiAlertDialogPortal>
        <div uiAlertDialogHeader>
          <h2 uiAlertDialogTitle>Turn off commenting on applications?</h2>
          <div uiAlertDialogDescription class="space-y-3 text-sm">
            <p>
              {{ jurisdiction() }} will no longer receive new liquor and cannabis licence applications for comment. Those applications will go straight to the LCRB and will not be held for the {{ holdDays }}-day comment period.
            </p>
            <p>Applications already awaiting your comment are not affected — they keep their existing due dates.</p>
            <p class="font-medium text-foreground">You can turn commenting back on at any time.</p>
          </div>
        </div>
        <div uiAlertDialogFooter>
          <button uiAlertDialogCancel>Cancel</button>
          <button uiAlertDialogAction (click)="confirmOptOut()" class="bg-amber-700 hover:bg-amber-800">Turn off commenting</button>
        </div>
      </ui-alert-dialog-content>
    </ui-alert-dialog>
  `,
})
export class CommentingSettings {
  private readonly lgReviewRepository = inject(LG_REVIEW_REPOSITORY);
  private readonly commenting = this.lgReviewRepository.commenting();
  private readonly store = this.lgReviewRepository.store();
  private readonly session = injectSession().session;

  protected readonly preference = this.commenting.preference;
  protected readonly confirmOpen = signal(false);

  protected readonly holdDays = COMMENT_HOLD_DAYS;
  protected readonly owner = computed(() => isAccountOwner(this.session()));
  private readonly actor = computed(() => actorName(this.session()));
  protected readonly jurisdiction = computed(() => this.session()?.organization ?? 'your jurisdiction');
  protected readonly optedOut = computed(() => this.preference().optedOut);

  // Referrals that were already pending when commenting was switched off keep their original deadline.
  protected readonly grandfathered = computed(() => this.lgReviewRepository.grandfatheredReferrals(this.preference(), this.store.applications()));

  protected readonly changedAt = computed(() =>
    new Date(this.preference().updatedAt as string).toLocaleString('en-CA', { dateStyle: 'medium', timeStyle: 'short' }),
  );
  protected readonly inEffect = computed(() => {
    const p = this.preference();
    return p.optedOut && p.effectiveFrom ? ` · In effect for applications referred on or after ${formatLgDate(p.effectiveFrom)}` : '';
  });

  protected handleToggle(next: boolean): void {
    if (!this.owner()) return;
    if (next) {
      // Turning commenting OFF is consequential: confirm first.
      this.confirmOpen.set(true);
      return;
    }
    this.commenting.setOptedOut(false, this.actor(), this.jurisdiction());
    toast.success('Commenting turned back on', {
      description: 'New referrals will be held for your comment again.',
    });
  }

  protected confirmOptOut(): void {
    this.commenting.setOptedOut(true, this.actor(), this.jurisdiction());
    this.confirmOpen.set(false);
    toast.success('Commenting turned off', {
      description: 'New referrals will flow through to the LCRB without waiting for comment.',
    });
  }

  protected simulate(): void {
    this.store.simulateIncomingReferral(this.actor());
    toast.success('Simulated referral added', {
      description: 'Open it from the comment queue to see the flow-through notice.',
    });
  }
}
