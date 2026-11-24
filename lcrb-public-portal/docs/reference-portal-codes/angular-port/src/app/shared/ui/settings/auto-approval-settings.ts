/** Auto-approval account setting (police agency, owner only): changes are confirmed in a dialog and go through the lib store; the Switch only moves once the policy changes. */
import { ChangeDetectionStrategy, Component, computed, inject, input, signal } from '@angular/core';
import { toast } from '@spartan-ng/brain/sonner';
import { injectSession } from '@core/session/session-provider';
import { Badge } from '../primitives/badge';
import { Button } from '../primitives/button';
import { DIALOG } from '../primitives/dialog';
import { type IconName } from '../primitives/icons';
import { Label } from '../primitives/label';
import { LucideIcon } from '../primitives/lucide-icon';
import { Switch } from '../primitives/switch';
import { AUTO_APPROVAL_THRESHOLD, REFERRAL_RULES } from '@domain/special-events/seps';
import { actorName, isAccountOwner } from '@domain/roles';
import { MIN_NOTICE_DAYS } from '@domain/special-events/auto-approval';
import { SPECIAL_EVENTS_REPOSITORY } from '@data-access/repositories/special-events/special-events.repository';

/** The prototype's local `Rule` component. */
@Component({
  selector: 'auto-approval-rule',
  imports: [LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="flex gap-3 rounded-md border border-border p-3">
      <svg [lucide]="icon()" class="mt-0.5 h-4 w-4 shrink-0 text-[#013366]"></svg>
      <div>
        <p class="text-sm font-medium text-foreground">{{ title() }}</p>
        <p class="mt-0.5 text-xs leading-relaxed text-muted-foreground">{{ body() }}</p>
      </div>
    </div>
  `,
})
export class AutoApprovalRule {
  readonly icon = input.required<IconName>();
  readonly title = input.required<string>();
  readonly body = input.required<string>();
}

@Component({
  selector: 'auto-approval-settings',
  imports: [Badge, Button, DIALOG, Label, LucideIcon, Switch, AutoApprovalRule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="space-y-6">
      <section class="rounded-lg border border-border bg-card p-6">
        <div class="flex flex-wrap items-start justify-between gap-4">
          <div class="flex gap-3">
            <div [attr.class]="'rounded-lg p-2 ' + (policy().enabled ? 'bg-green-100 text-green-800' : 'bg-muted text-muted-foreground')">
              @if (policy().enabled) {
                <svg lucide="ShieldCheck" class="h-5 w-5"></svg>
              } @else {
                <svg lucide="ShieldOff" class="h-5 w-5"></svg>
              }
            </div>
            <div>
              <h2 class="text-lg font-semibold text-foreground">Special Event Permit auto-approval</h2>
              <p class="mt-1 max-w-prose text-sm text-muted-foreground">
                When on, low-risk permits in your jurisdiction are approved automatically with the standard
                conditions, so officers only see events that warrant review. When off, every permit is referred to
                an officer regardless of size.
              </p>
            </div>
          </div>
          <span
            uiBadge
            variant="outline"
            [class]="policy().enabled ? 'border-green-200 bg-green-50 text-green-800' : 'border-zinc-200 bg-zinc-100 text-zinc-700'"
          >
            {{ policy().enabled ? 'On' : 'Off' }}
          </span>
        </div>

        <div class="mt-6 flex items-center justify-between gap-4 rounded-md border border-border bg-muted/40 p-4">
          <div>
            <label uiLabel for="auto-approval" class="text-sm font-medium">Enable auto-approval for {{ agency() }}</label>
            <p class="mt-0.5 text-xs text-muted-foreground">
              {{ owner() ? 'Only the account owner can change this.' : 'Read-only — only the account owner can change this.' }}
            </p>
          </div>
          <div class="flex items-center gap-2">
            @if (!owner()) {
              <svg lucide="Lock" class="h-4 w-4 text-muted-foreground" aria-hidden="true"></svg>
            }
            <button
              uiSwitch
              id="auto-approval"
              [checked]="policy().enabled"
              [disabled]="!owner()"
              (checkedChange)="onSwitch($event)"
              aria-describedby="auto-approval-rules"
            ></button>
          </div>
        </div>

        <div id="auto-approval-rules" class="mt-6 grid gap-3 sm:grid-cols-2">
          <auto-approval-rule
            icon="CalendarClock"
            [title]="'At least ' + minNoticeDays + ' days\\' notice'"
            body="Permits submitted closer to the event than this are always referred to an officer, even when auto-approval is on."
          />
          <auto-approval-rule
            icon="Info"
            title="Standard eligibility still applies"
            [body]="eligibilityBody"
          />
        </div>

        @if (policy().updatedAt) {
          <p class="mt-4 text-xs text-muted-foreground">
            Last changed by {{ policy().updatedBy }} on {{ changedAt() }}.
          </p>
        }
      </section>

      <ui-dialog [state]="confirming() !== null ? 'open' : 'closed'" (stateChanged)="onDialogState($event)">
        <ui-dialog-content *uiDialogPortal>
          <div uiDialogHeader>
            <h2 uiDialogTitle>{{ confirming() ? 'Turn auto-approval on?' : 'Turn auto-approval off?' }}</h2>
            <p uiDialogDescription>{{ dialogDescription() }}</p>
          </div>
          <div uiDialogFooter>
            <button uiButton variant="outline" (click)="confirming.set(null)">Keep current setting</button>
            <button uiButton class="bg-[#013366] hover:bg-[#012a52]" (click)="apply(confirming() === true)">
              {{ confirming() ? 'Turn on' : 'Turn off' }}
            </button>
          </div>
        </ui-dialog-content>
      </ui-dialog>
    </div>
  `,
})
export class AutoApprovalSettings {
  private readonly specialEventsRepository = inject(SPECIAL_EVENTS_REPOSITORY);
  private readonly sessionProvider = injectSession();
  protected readonly policy = this.specialEventsRepository.autoApprovalPolicy();
  private readonly session = this.sessionProvider.session;
  protected readonly owner = computed(() => isAccountOwner(this.session()));
  protected readonly agency = computed(() => this.session()?.organization ?? 'your agency');
  protected readonly confirming = signal<boolean | null>(null);

  protected readonly minNoticeDays = MIN_NOTICE_DAYS;
  protected readonly eligibilityBody = `Under ${AUTO_APPROVAL_THRESHOLD} attendees, fewer than ${REFERRAL_RULES.drinksThreshold.toLocaleString()} drinks, service ending by 2:00 am, not on public property, and security in place where required.`;

  protected readonly changedAt = computed(() =>
    new Date(this.policy().updatedAt as string).toLocaleString('en-CA', { dateStyle: 'medium', timeStyle: 'short' }),
  );

  protected readonly dialogDescription = computed(() =>
    this.confirming()
      ? `Permits for ${this.agency()} that meet the standard criteria and are submitted at least ${MIN_NOTICE_DAYS} days before the event will be approved automatically from now on. Permits already in your queue are not affected.`
      : `Every new Special Event Permit for ${this.agency()} will be referred to an officer for review. Permits already auto-approved stay approved.`,
  );

  protected onSwitch(next: boolean): void {
    this.confirming.set(next);
  }

  /** Radix onOpenChange fires only for user dismissal (Escape, the close button), not for our own state changes. */
  protected onDialogState(state: string): void {
    if (state === 'closed' && this.confirming() !== null) this.confirming.set(null);
  }

  protected apply(enabled: boolean): void {
    this.specialEventsRepository.setAutoApprovalEnabled(enabled, actorName(this.session()), this.agency());
    this.confirming.set(null);
    toast.success(enabled ? 'Auto-approval turned on' : 'Auto-approval turned off', {
      description: enabled
        ? `Eligible permits submitted at least ${MIN_NOTICE_DAYS} days before the event will be approved automatically.`
        : 'Every Special Event Permit will now be referred to an officer.',
    });
  }
}
