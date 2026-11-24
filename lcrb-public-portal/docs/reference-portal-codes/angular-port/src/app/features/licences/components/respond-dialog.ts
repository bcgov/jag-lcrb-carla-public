/** Licensee response to an enforcement action (accept, written response or hearing request); the prototype's `open` / `onOpenChange` props are the `open` input and `openChange` output. */
import { ChangeDetectionStrategy, Component, computed, inject, input, output, signal } from '@angular/core';
import { injectSession } from '@core/session/session-provider';
import { Button } from '@shared/ui/primitives/button';
import { DIALOG } from '@shared/ui/primitives/dialog';
import type { IconName } from '@shared/ui/primitives/icons';
import { Label } from '@shared/ui/primitives/label';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { RADIO_GROUP } from '@shared/ui/primitives/radio-group';
import { Textarea } from '@shared/ui/primitives/textarea';
import type { EnforcementAction } from '@domain/models/compliance';
import { actorName } from '@domain/roles';
import { type ResponseElection } from '@data-access/repositories/compliance/enforcement-store';
import { cn } from '@shared/util/utils';
import { LICENCES_REPOSITORY } from '@data-access/repositories/licences/licences.repository';
import { COMPLIANCE_REPOSITORY } from '@data-access/repositories/compliance/compliance.repository';

const ELECTIONS: {
  value: ResponseElection;
  title: string;
  description: string;
  Icon: IconName;
}[] = [
  {
    value: 'accept',
    title: 'Accept the proposed action',
    description: 'Acknowledge the contravention and accept the penalty proposed by the LCRB.',
    Icon: 'ShieldCheck',
  },
  {
    value: 'dispute',
    title: 'Submit a written response',
    description: 'Provide context or dispute the allegation in writing for staff to consider.',
    Icon: 'FileText',
  },
  {
    value: 'request-hearing',
    title: 'Request an enforcement hearing',
    description: "Ask for a hearing before a general manager's delegate to present your case.",
    Icon: 'Gavel',
  },
];

@Component({
  selector: 'respond-dialog',
  imports: [...DIALOG, ...RADIO_GROUP, Button, Label, LucideIcon, Textarea],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <ui-dialog [state]="open() ? 'open' : 'closed'" (stateChanged)="onState($event)">
      <ui-dialog-content *uiDialogPortal class="max-w-lg max-h-[90vh] overflow-y-auto">
        <div uiDialogHeader>
          <h2 uiDialogTitle>Respond to {{ action().referenceNumber }}</h2>
          <p uiDialogDescription>{{ action().allegation }} · {{ action().establishmentName }}</p>
        </div>

        <div class="rounded-md border bg-muted/40 p-3 text-sm">
          <div class="text-muted-foreground">Proposed action</div>
          <div class="font-medium text-foreground">{{ action().proposedAction }}</div>
        </div>

        <fieldset class="space-y-3">
          <legend class="text-sm font-medium text-foreground mb-1">How would you like to respond?</legend>
          <div uiRadioGroup [value]="election()" (valueChange)="election.set($any($event))" class="gap-2">
            @for (e of elections; track e.value) {
              <label uiLabel [attr.for]="'election-' + e.value" [class]="labelClass(e.value)">
                <button uiRadioGroupItem [value]="e.value" [attr.id]="'election-' + e.value" class="mt-1"></button>
                <span class="flex-1">
                  <span class="flex items-center gap-2 font-medium text-foreground">
                    <svg [lucide]="e.Icon" class="h-4 w-4 text-[#013366]" aria-hidden="true"></svg>
                    {{ e.title }}
                  </span>
                  <span class="block text-sm text-muted-foreground mt-0.5">{{ e.description }}</span>
                </span>
              </label>
            }
          </div>
        </fieldset>

        @if (needsStatement()) {
          <div class="space-y-1.5">
            <label uiLabel for="statement"
              >Your statement <span class="text-muted-foreground font-normal">(required, min. 10 characters)</span></label
            >
            <textarea
              uiTextarea
              id="statement"
              [value]="statement()"
              (input)="statement.set($any($event.target).value)"
              rows="5"
              [placeholder]="
                election() === 'request-hearing'
                  ? 'Explain why you are requesting a hearing and what you intend to present...'
                  : 'Provide any context, mitigating factors, or grounds for dispute...'
              "
            ></textarea>
          </div>
        }

        @if (election() === 'accept') {
          <p class="text-sm text-muted-foreground">
            By submitting, you accept the proposed action. Payment or suspension instructions will follow in your Notices.
          </p>
        }

        <div uiDialogFooter>
          <button uiButton variant="outline" (click)="openChange.emit(false)" [disabled]="submitting()">Cancel</button>
          <button uiButton (click)="handleSubmit()" [disabled]="!canSubmit()" class="bg-[#013366] hover:bg-[#012a52] gap-2">
            @if (submitting()) {
              <svg lucide="Loader2" class="h-4 w-4 animate-spin" aria-hidden="true"></svg>
            }
            Submit response
          </button>
        </div>
      </ui-dialog-content>
    </ui-dialog>
  `,
})
export class RespondDialog {
  private readonly licencesRepository = inject(LICENCES_REPOSITORY);
  private readonly complianceRepository = inject(COMPLIANCE_REPOSITORY);
  readonly action = input.required<EnforcementAction>();
  readonly open = input.required<boolean>();
  readonly openChange = output<boolean>();

  private readonly session = injectSession().session;
  protected readonly elections = ELECTIONS;
  protected readonly election = signal<ResponseElection>('dispute');
  protected readonly statement = signal('');
  protected readonly submitting = signal(false);

  protected readonly needsStatement = computed(() => this.election() !== 'accept');
  protected readonly canSubmit = computed(
    () => !this.submitting() && (!this.needsStatement() || this.statement().trim().length >= 10),
  );

  protected labelClass(value: ResponseElection): string {
    return cn(
      'flex items-start gap-3 rounded-lg border p-3 cursor-pointer transition-colors',
      this.election() === value ? 'border-[#013366] bg-[#013366]/5' : 'hover:bg-muted/50',
    );
  }

  /** Radix onOpenChange fires only for user dismissal (Escape, close button, overlay). */
  protected onState(state: string): void {
    if (state === 'closed' && this.open()) this.openChange.emit(false);
  }

  protected handleSubmit(): void {
    if (!this.canSubmit()) return;
    this.submitting.set(true);
    const action = this.action();
    this.complianceRepository.submitResponse({
      action,
      election: this.election(),
      statement: this.statement().trim(),
      actor: actorName(this.session()),
      email: this.licencesRepository.licence(action.licenceId)?.email,
    });
    // Small delay so the confirming state is visible, then close.
    setTimeout(() => {
      this.submitting.set(false);
      this.openChange.emit(false);
      this.statement.set('');
      this.election.set('dispute');
    }, 500);
  }
}
