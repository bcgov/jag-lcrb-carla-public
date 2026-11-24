/** Screening card for one associated person: send the request, upload the completed form, submit it to LCRB. */
import { ChangeDetectionStrategy, Component, computed, inject, input, signal } from '@angular/core';
import { toast } from '@spartan-ng/brain/sonner';
import { DocumentManager } from '@shared/ui/documents/document-manager';
import { Badge } from '@shared/ui/primitives/badge';
import { Button } from '@shared/ui/primitives/button';
import type { IconName } from '@shared/ui/primitives/icons';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { activateScreening, resetScreening, type ScreeningPerson, screeningSubjectId, submitScreening } from '../state/screening-store';
import { DOCUMENTS_REPOSITORY } from '@data-access/repositories/documents/documents.repository';

const STATUS: Record<ScreeningPerson['status'], { label: string; className: string; icon: IconName }> = {
  not_started: { label: 'Not started', className: 'bg-gray-100 text-gray-800', icon: 'AlertCircle' },
  invited: { label: 'Invitation sent', className: 'bg-blue-100 text-blue-800', icon: 'Mail' },
  submitted: { label: 'Uploaded — syncing', className: 'bg-yellow-100 text-yellow-800', icon: 'Clock' },
  synced: { label: 'Submitted to LCRB', className: 'bg-green-100 text-green-800', icon: 'CheckCircle2' },
};

function formatDate(iso?: string) {
  if (!iso) return null;
  return new Date(iso).toLocaleDateString('en-CA', { year: 'numeric', month: 'short', day: 'numeric' });
}

@Component({
  selector: 'div[screeningPersonCard]',
  imports: [Badge, Button, LucideIcon, DocumentManager],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { class: 'rounded-lg border p-4' },
  template: `
    <div class="flex flex-wrap items-start justify-between gap-3">
      <div>
        <div class="flex items-center gap-2">
          <p class="font-semibold text-foreground">{{ person().name }}</p>
          <span uiBadge variant="outline" [class]="status().className + ' flex items-center gap-1'">
            <svg [lucide]="status().icon" class="h-3 w-3"></svg>
            {{ status().label }}
          </span>
        </div>
        <p class="text-sm text-muted-foreground">{{ person().role }}</p>
        <p class="mt-0.5 flex items-center gap-1.5 text-xs text-muted-foreground">
          <svg lucide="Mail" class="h-3 w-3"></svg>
          {{ person().email }} · {{ person().formType }}
        </p>
      </div>

      @if (person().status === 'not_started') {
        <button uiButton (click)="handleActivate()" class="shrink-0 bg-bc-blue hover:bg-bc-blue/90">
          <svg lucide="Send" class="mr-2 h-4 w-4"></svg>
          Send screening request
        </button>
      }
    </div>

    <!-- Timeline hints -->
    @if (person().status === 'invited') {
      <div class="mt-3 flex items-start gap-2 rounded-md bg-blue-50 p-3 text-sm text-blue-800">
        <svg lucide="Mail" class="mt-0.5 h-4 w-4 shrink-0"></svg>
        <p>
          Invitation emailed{{ invitedOn() }}. Once {{ person().name }} returns the completed {{ person().formType }}, upload it below and submit it to LCRB.
        </p>
      </div>
    }

    @if (person().status === 'synced') {
      <div class="mt-3 flex items-start gap-2 rounded-md bg-green-50 p-3 text-sm text-green-800">
        <svg lucide="Database" class="mt-0.5 h-4 w-4 shrink-0"></svg>
        <p>Submitted to LCRB for review{{ person().dynamicsCaseId ? ' — case ' + person().dynamicsCaseId : '' }}{{ submittedOn() }}.</p>
      </div>
    }

    <!-- Upload area: available once invited -->
    @if (person().status === 'invited' || person().status === 'submitted' || person().status === 'synced') {
      <div class="mt-4 space-y-3">
        <p class="text-sm font-medium text-foreground">Completed {{ person().formType }}</p>
        <document-manager
          [subjectId]="subjectId()"
          [actor]="actor()"
          [category]="'Security screening — ' + person().name"
          [readOnly]="person().status === 'synced'"
        />

        @if (person().status === 'invited' || person().status === 'submitted') {
          <div class="flex flex-wrap items-center gap-3">
            <button uiButton (click)="handleSubmit()" [disabled]="!hasCleanDoc() || submitting()" class="bg-bc-blue hover:bg-bc-blue/90">
              <svg lucide="ShieldCheck" class="mr-2 h-4 w-4"></svg>
              {{ submitting() ? 'Submitting…' : 'Submit completed form to LCRB' }}
            </button>
            @if (!hasCleanDoc()) {
              <span class="text-xs text-muted-foreground">Upload the completed form (and wait for the virus scan) to enable submission.</span>
            }
            @if (person().status === 'submitted') {
              <span class="flex items-center gap-1.5 text-xs text-yellow-700">
                <svg lucide="RefreshCw" class="h-3 w-3"></svg>
                Sync to LCRB pending — you can retry submission.
              </span>
            }
          </div>
        }
      </div>
    }

    <!-- Dev/reset affordance for the prototype -->
    @if (person().status !== 'not_started') {
      <button type="button" (click)="reset()" class="mt-3 text-xs text-muted-foreground underline underline-offset-2 hover:text-foreground">Reset (demo)</button>
    }
  `,
})
export class ScreeningPersonCard {
  private readonly documentsRepository = inject(DOCUMENTS_REPOSITORY);
  readonly person = input.required<ScreeningPerson>();
  readonly actor = input.required<string>();

  protected readonly subjectId = computed(() => screeningSubjectId(this.person().id));
  private readonly docs = this.documentsRepository.documents(() => this.subjectId());
  protected readonly submitting = signal(false);
  protected readonly hasCleanDoc = computed(() => this.docs().some((d) => d.scanState === 'clean'));
  protected readonly status = computed(() => STATUS[this.person().status]);
  protected readonly invitedOn = computed(() => {
    const d = formatDate(this.person().invitedAt);
    return d ? ` on ${d}` : '';
  });
  protected readonly submittedOn = computed(() => {
    const d = formatDate(this.person().submittedAt);
    return d ? ` on ${d}` : '';
  });

  protected handleActivate(): void {
    const person = this.person();
    activateScreening(person.id, this.actor());
    toast.success('Screening request sent', {
      description: `An email with instructions was sent to ${person.name} at ${person.email}.`,
    });
  }

  protected handleSubmit(): void {
    const person = this.person();
    this.submitting.set(true);
    submitScreening(person.id, this.actor());
    // submitScreening updates the store synchronously; give the UI a beat.
    setTimeout(() => {
      this.submitting.set(false);
      toast.success('Completed form submitted', {
        description: `${person.name}'s screening was sent to LCRB for review.`,
      });
    }, 400);
  }

  protected reset(): void {
    resetScreening(this.person().id);
  }
}
