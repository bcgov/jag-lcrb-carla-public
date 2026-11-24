/** "Draft with AI" button above a decision textarea; `onDraft` becomes the `draft` output and nothing is auto-submitted. */
import { ChangeDetectionStrategy, Component, inject, input, output, signal } from '@angular/core';
import { LucideIcon } from '../primitives/lucide-icon';
import { AI_REPOSITORY } from '@data-access/repositories/ai/ai.repository';

export interface DraftRationaleInput {
  decision: string;
  tone: 'deny' | 'changes' | 'approve' | 'neutral';
  subjectType: string;
  title: string;
  reviewerRole: string;
  facts?: { label: string; value: string }[];
  signals?: string[];
  conditions?: string[];
  reviewerNotes?: string;
}

@Component({
  selector: 'draft-rationale-button',
  imports: [LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="flex items-center justify-between gap-2">
      <button
        type="button"
        (click)="handleClick()"
        [disabled]="loading()"
        class="inline-flex items-center gap-1.5 rounded-md border border-[#013366]/30 bg-[#013366]/5 px-2.5 py-1 text-xs font-medium text-[#013366] hover:bg-[#013366]/10 disabled:opacity-60"
      >
        @if (loading()) {
          <svg lucide="Loader2" class="h-3.5 w-3.5 animate-spin"></svg>
        } @else {
          <svg lucide="Sparkles" class="h-3.5 w-3.5"></svg>
        }
        {{ loading() ? 'Drafting…' : label() }}
      </button>
      @if (error()) {
        <span class="text-xs text-destructive">{{ error() }}</span>
      }
    </div>
  `,
})
export class DraftRationaleButton {
  private readonly ai = inject(AI_REPOSITORY);

  readonly input = input.required<DraftRationaleInput>();
  readonly draft = output<string>();
  readonly label = input('Draft with AI');

  protected readonly loading = signal(false);
  protected readonly error = signal<string | null>(null);

  protected async handleClick() {
    this.loading.set(true);
    this.error.set(null);
    try {
      const res = await this.ai.fetchApi('/api/decision-rationale', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(this.input()),
      });
      if (!res.ok) throw new Error('Request failed');
      const data = (await res.json()) as { text?: string };
      if (data.text) this.draft.emit(data.text);
      else throw new Error('Empty response');
    } catch {
      this.error.set('Could not draft. Try again.');
    } finally {
      this.loading.set(false);
    }
  }
}
