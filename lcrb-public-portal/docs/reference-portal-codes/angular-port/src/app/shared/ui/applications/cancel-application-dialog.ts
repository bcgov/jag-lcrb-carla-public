/** Confirmation for withdrawing a submitted application or permit; pass [open] + (openChange), or leave open unset for internal state. A custom trigger is a projected element marked cancelTrigger. */
import { ChangeDetectionStrategy, Component, computed, contentChild, Directive, inject, input, output, signal } from '@angular/core';
import { Button } from '../primitives/button';
import { Checkbox } from '../primitives/checkbox';
import { DIALOG } from '../primitives/dialog';
import { Label } from '../primitives/label';
import { LucideIcon } from '../primitives/lucide-icon';
import { Textarea } from '../primitives/textarea';

const REASONS = [
  'Plans have changed — no longer proceeding',
  'Submitted in error / duplicate application',
  'Will reapply with different details',
  'Other',
];

let ackCounter = 0;

/** A projected custom trigger: clicking it opens the dialog. */
@Directive({ selector: '[cancelTrigger]', host: { '(click)': 'dialog.setOpen(true)' } })
export class CancelApplicationTrigger {
  protected readonly dialog = inject(CancelApplicationDialog);
}

@Component({
  selector: 'cancel-application-dialog',
  imports: [...DIALOG, Button, Checkbox, Label, Textarea, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <ui-dialog [state]="isOpen() ? 'open' : 'closed'" (stateChanged)="onState($event)">
      <!-- Controlled callers (e.g. a dropdown menu item) open the dialog themselves. -->
      <ng-content select="[cancelTrigger]" />
      @if (!isControlled() && !customTrigger()) {
        <button
          uiButton
          variant="outline"
          class="gap-2 border-red-300 text-red-700 hover:bg-red-50 hover:text-red-800"
          (click)="setOpen(true)"
        >
          <svg lucide="Ban" class="h-4 w-4"></svg>
          Cancel {{ noun() }}
        </button>
      }
      <ui-dialog-content *uiDialogPortal class="sm:max-w-lg">
        <div uiDialogHeader>
          <h2 uiDialogTitle>Cancel this {{ noun() }}?</h2>
          <p uiDialogDescription>
            You are withdrawing <span class="font-medium text-foreground">{{ subject() }}</span
            >. This cannot be undone from the portal; you would need to start a new {{ noun() }}.
          </p>
        </div>

        <div class="space-y-4">
          <div class="flex items-start gap-3 rounded-md border border-amber-200 bg-amber-50 p-3 text-sm text-amber-900">
            <svg lucide="AlertTriangle" class="mt-0.5 h-4 w-4 shrink-0"></svg>
            <ul class="space-y-1">
              @if (feePaid()) {
                <li>Application fees already paid are not refunded.</li>
              }
              <li>LCRB and any partner agency reviewing the file will be notified immediately.</li>
              @if (consequence()) {
                <li>{{ consequence() }}</li>
              }
            </ul>
          </div>

          <fieldset class="space-y-2">
            <legend class="text-sm font-medium">Reason for cancelling</legend>
            <div class="grid gap-2">
              @for (r of reasons; track r) {
                <label
                  [attr.class]="
                    'flex cursor-pointer items-center gap-2 rounded-md border px-3 py-2 text-sm transition-colors ' +
                    (choice() === r ? 'border-bc-blue bg-bc-blue/5' : 'border-border hover:bg-muted/50')
                  "
                >
                  <input
                    type="radio"
                    name="cancel-reason"
                    [value]="r"
                    [checked]="choice() === r"
                    (change)="choice.set(r)"
                    class="h-4 w-4 accent-[#013366]"
                  />
                  {{ r }}
                </label>
              }
            </div>
          </fieldset>

          <div>
            <label uiLabel [attr.for]="ackId + '-detail'" class="text-sm">
              {{ choice() === 'Other' ? 'Please describe (required)' : 'Additional details (optional)' }}
            </label>
            <textarea
              uiTextarea
              [attr.id]="ackId + '-detail'"
              [value]="detail()"
              (input)="detail.set($any($event.target).value)"
              rows="2"
              class="mt-1.5"
              placeholder="Anything LCRB should know for the record…"
            ></textarea>
          </div>

          <div class="flex items-start gap-2">
            <button uiCheckbox [attr.id]="ackId" [checked]="acknowledged()" (checkedChange)="acknowledged.set($event === true)"></button>
            <label uiLabel [attr.for]="ackId" class="text-sm font-normal leading-snug">
              I understand this withdrawal is final{{ feePaid() ? ' and that fees paid are not refunded' : '' }}.
            </label>
          </div>
        </div>

        <div uiDialogFooter class="gap-2 sm:gap-0">
          <button uiButton variant="outline" (click)="setOpen(false)">Keep {{ noun() }}</button>
          <button uiButton variant="destructive" [disabled]="!canConfirm()" (click)="confirm()">Cancel {{ noun() }}</button>
        </div>
      </ui-dialog-content>
    </ui-dialog>
  `,
})
export class CancelApplicationDialog {
  /** What is being cancelled, e.g. "Liquor Primary Licence (LP-125998)". */
  readonly subject = input.required<string>();
  /** "application" or "permit": drives the copy. */
  readonly noun = input<'application' | 'permit'>('application');
  /** Extra consequence line, e.g. who has already started reviewing. */
  readonly consequence = input<string | undefined>(undefined);
  /** Whether a fee was paid (controls the non-refund acknowledgement). */
  readonly feePaid = input(true);
  readonly confirmed = output<string>();
  readonly open = input<boolean | undefined>(undefined);
  readonly openChange = output<boolean>();

  protected readonly customTrigger = contentChild(CancelApplicationTrigger);
  protected readonly reasons = REASONS;
  protected readonly ackId = `cancel-ack-${++ackCounter}`;

  private readonly internalOpen = signal(false);
  protected readonly isControlled = computed(() => this.open() !== undefined);
  protected readonly isOpen = computed(() => (this.isControlled() ? this.open()! : this.internalOpen()));

  protected readonly choice = signal('');
  protected readonly detail = signal('');
  protected readonly acknowledged = signal(false);

  private readonly reason = computed(() =>
    this.choice() === 'Other'
      ? this.detail().trim()
      : [this.choice(), this.detail().trim()].filter(Boolean).join(' — '),
  );
  protected readonly canConfirm = computed(
    () => Boolean(this.choice()) && (this.choice() !== 'Other' || this.detail().trim().length > 0) && this.acknowledged(),
  );

  setOpen(v: boolean): void {
    if (!this.isControlled()) this.internalOpen.set(v);
    this.openChange.emit(v);
    if (!v) this.reset();
  }

  /** Radix onOpenChange fires only for user dismissal (Escape, close button, overlay). */
  protected onState(state: string): void {
    if (state === 'closed' && this.isOpen()) this.setOpen(false);
  }

  protected confirm(): void {
    this.confirmed.emit(this.reason());
    this.setOpen(false);
  }

  private reset(): void {
    this.choice.set('');
    this.detail.set('');
    this.acknowledged.set(false);
  }
}
