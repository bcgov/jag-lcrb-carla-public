/** On-demand status explainer; the inputs are POSTed as the body, with undefined optional props omitted by JSON.stringify as in the prototype. */
import { ChangeDetectionStrategy, Component, inject, input, signal } from '@angular/core';
import { LucideIcon } from '../primitives/lucide-icon';
import { AI_REPOSITORY } from '@data-access/repositories/ai/ai.repository';

interface HistoryItem {
  action: string;
  actor?: string;
  date?: string;
  note?: string;
}

interface StatusExplanation {
  meaning: string;
  whatsNext: string[];
  actionRequired: boolean;
}

@Component({
  selector: 'status-explainer',
  imports: [LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (state() === 'idle') {
      <button
        type="button"
        (click)="explain()"
        class="flex w-full items-center justify-between gap-2 rounded-lg border border-[#013366]/20 bg-[#013366]/5 px-4 py-3 text-left transition-colors hover:bg-[#013366]/10"
      >
        <span class="flex items-center gap-2">
          <svg lucide="Sparkles" class="h-4 w-4 shrink-0 text-[#013366]"></svg>
          <span class="text-sm font-medium text-[#013366]">What does this status mean?</span>
        </span>
        <svg lucide="ChevronRight" class="h-4 w-4 shrink-0 text-[#013366]"></svg>
      </button>
    } @else {
      <div class="rounded-lg border border-[#013366]/20 bg-card p-4">
        <div class="flex items-center gap-2">
          <svg lucide="Sparkles" class="h-4 w-4 shrink-0 text-[#013366]"></svg>
          <h3 class="text-sm font-semibold text-foreground">What this means</h3>
          <span
            class="ml-auto rounded-full bg-[#013366]/10 px-2 py-0.5 text-[10px] font-medium uppercase tracking-wide text-[#013366]"
          >
            AI
          </span>
        </div>

        @if (state() === 'loading') {
          <div class="mt-3 flex items-center gap-2 text-sm text-muted-foreground">
            <svg lucide="Loader2" class="h-4 w-4 animate-spin"></svg>
            Reading your status and history…
          </div>
        }

        @if (state() === 'error') {
          <p class="mt-3 text-sm text-muted-foreground">
            Couldn't generate an explanation right now. Please try again in a moment.
            <button type="button" (click)="explain()" class="font-medium text-[#013366] hover:underline">Retry</button>
          </p>
        }

        @if (state() === 'done' && result(); as r) {
          <div class="mt-3 space-y-3">
            <p class="text-sm leading-relaxed text-foreground">{{ r.meaning }}</p>

            @if (r.actionRequired) {
              <div
                class="flex items-start gap-2 rounded-md border border-orange-200 bg-orange-50 p-2.5 text-sm text-orange-900"
              >
                <svg lucide="AlertTriangle" class="mt-0.5 h-4 w-4 shrink-0"></svg>
                <span>This needs an action from you — see the steps below.</span>
              </div>
            } @else {
              <div
                class="flex items-start gap-2 rounded-md border border-green-200 bg-green-50 p-2.5 text-sm text-green-900"
              >
                <svg lucide="CheckCircle2" class="mt-0.5 h-4 w-4 shrink-0"></svg>
                <span>No action needed from you right now.</span>
              </div>
            }

            @if (r.whatsNext.length > 0) {
              <div>
                <p class="text-xs font-semibold uppercase tracking-wide text-muted-foreground">What happens next</p>
                <ul class="mt-1.5 space-y-1.5">
                  @for (step of r.whatsNext; track $index) {
                    <li class="flex items-start gap-2 text-sm text-foreground">
                      <svg lucide="ChevronRight" class="mt-0.5 h-4 w-4 shrink-0 text-[#013366]"></svg>
                      <span>{{ step }}</span>
                    </li>
                  }
                </ul>
              </div>
            }

            <p class="text-[11px] text-muted-foreground">AI-generated guidance · verify with official policy.</p>
          </div>
        }
      </div>
    }
  `,
})
export class StatusExplainer {
  private readonly ai = inject(AI_REPOSITORY);
  readonly subjectType = input.required<string>();
  readonly title = input.required<string>();
  readonly status = input.required<string>();
  readonly statusLabel = input<string | undefined>(undefined);
  readonly history = input<HistoryItem[] | undefined>(undefined);
  readonly extra = input<string | undefined>(undefined);

  protected readonly state = signal<'idle' | 'loading' | 'done' | 'error'>('idle');
  protected readonly result = signal<StatusExplanation | null>(null);

  protected async explain() {
    this.state.set('loading');
    try {
      const res = await this.ai.fetchApi('/api/status-explainer', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          subjectType: this.subjectType(),
          title: this.title(),
          status: this.status(),
          statusLabel: this.statusLabel(),
          history: this.history(),
          extra: this.extra(),
        }),
      });
      if (!res.ok) throw new Error(String(res.status));
      const data = (await res.json()) as StatusExplanation;
      this.result.set(data);
      this.state.set('done');
    } catch {
      this.state.set('error');
    }
  }
}
