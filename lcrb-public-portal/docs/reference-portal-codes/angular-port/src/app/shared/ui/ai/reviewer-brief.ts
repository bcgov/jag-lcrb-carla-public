/** Staff-only AI brief. The cache is read after first render, so it starts idle. Quirk kept: "Regenerate" re-serves the cached result. */
import { afterNextRender, ChangeDetectionStrategy, Component, computed, effect, inject, input, signal } from '@angular/core';
import { LucideIcon } from '../primitives/lucide-icon';
import { AI_REPOSITORY } from '@data-access/repositories/ai/ai.repository';

interface Fact {
  label: string;
  value: string;
}

interface BriefResult {
  summary: string;
  riskLevel: 'low' | 'medium' | 'high';
  attentionItems: string[];
  recommendedDecision: string;
  rationale: string;
}

export interface ReviewerBriefHistoryItem {
  action: string;
  actor?: string;
  date?: string;
  note?: string;
}

const RISK_STYLES: Record<BriefResult['riskLevel'], { wrap: string; badge: string; label: string }> = {
  low: {
    wrap: 'border-[#2E8540]/30 bg-[#2E8540]/5',
    badge: 'bg-[#2E8540]/15 text-[#1E5631]',
    label: 'Low risk',
  },
  medium: {
    wrap: 'border-amber-300 bg-amber-50',
    badge: 'bg-amber-200/70 text-amber-900',
    label: 'Medium risk',
  },
  high: {
    wrap: 'border-red-300 bg-red-50',
    badge: 'bg-red-200/70 text-red-900',
    label: 'High risk',
  },
};

@Component({
  selector: 'reviewer-brief',
  imports: [LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (!disabled()) {
      <section [attr.class]="rootClass()" aria-label="AI reviewer brief">
        <div class="flex items-center gap-2">
          <svg lucide="Sparkles" class="h-4 w-4 shrink-0 text-[#013366]"></svg>
          <h2 class="text-sm font-semibold text-foreground">Reviewer brief</h2>
          <span
            class="rounded-full bg-[#013366]/10 px-2 py-0.5 text-[10px] font-medium uppercase tracking-wide text-[#013366]"
          >
            AI · staff only
          </span>
          @if (state() === 'done') {
            <button
              type="button"
              (click)="run()"
              class="ml-auto inline-flex items-center gap-1 text-xs font-medium text-muted-foreground hover:text-[#013366]"
            >
              <svg lucide="RefreshCw" class="h-3 w-3"></svg>
              Regenerate
            </button>
          }
        </div>

        @if (state() === 'idle') {
          <div class="mt-3 flex flex-wrap items-center gap-3">
            <button
              type="button"
              (click)="run()"
              class="inline-flex items-center gap-1.5 rounded-md bg-[#013366] px-3 py-1.5 text-sm font-medium text-white hover:bg-[#013366]/90"
            >
              <svg lucide="Sparkles" class="h-3.5 w-3.5"></svg>
              Generate brief
            </button>
            <span class="text-sm text-muted-foreground">
              Summarizes the risk signals and history for this submission.
            </span>
          </div>
        }

        @if (state() === 'loading') {
          <div class="mt-3 flex items-center gap-2 text-sm text-muted-foreground">
            <svg lucide="Loader2" class="h-4 w-4 animate-spin"></svg>
            Assessing this submission…
          </div>
        }

        @if (state() === 'error') {
          <p class="mt-3 text-sm text-muted-foreground">
            Couldn't generate the brief.
            <button type="button" (click)="run()" class="font-medium text-[#013366] hover:underline">Retry</button>
          </p>
        }

        @if (state() === 'done' && result(); as r) {
          <div class="mt-3 space-y-4">
            <div class="flex flex-wrap items-center gap-2">
              <span [attr.class]="badgeClass()">
                @if (r.riskLevel === 'low') {
                  <svg lucide="ShieldCheck" class="h-3.5 w-3.5"></svg>
                } @else {
                  <svg lucide="AlertTriangle" class="h-3.5 w-3.5"></svg>
                }
                {{ risk().label }}
              </span>
            </div>

            <p class="text-sm leading-relaxed text-foreground">{{ r.summary }}</p>

            @if (r.attentionItems.length > 0) {
              <div>
                <p
                  class="flex items-center gap-1.5 text-xs font-semibold uppercase tracking-wide text-muted-foreground"
                >
                  <svg lucide="ListChecks" class="h-3.5 w-3.5"></svg>
                  Check before deciding
                </p>
                <ul class="mt-1.5 space-y-1">
                  @for (item of r.attentionItems; track $index) {
                    <li class="flex items-start gap-2 text-sm text-foreground">
                      <span class="mt-1.5 h-1.5 w-1.5 shrink-0 rounded-full bg-current opacity-50"></span>
                      <span>{{ item }}</span>
                    </li>
                  }
                </ul>
              </div>
            }

            <div class="rounded-md border border-border bg-card/80 p-3">
              <p class="text-xs font-semibold uppercase tracking-wide text-muted-foreground">Suggested action</p>
              <p class="mt-1 text-sm font-medium text-foreground">{{ r.recommendedDecision }}</p>
              <p class="mt-0.5 text-sm text-muted-foreground">{{ r.rationale }}</p>
            </div>

            <p class="text-[11px] text-muted-foreground">
              AI suggestion only — you make the decision. Verify against policy.
            </p>
          </div>
        }
      </section>
    }
  `,
})
export class ReviewerBrief {
  private readonly aiRepository = inject(AI_REPOSITORY);
  readonly subjectType = input.required<string>();
  readonly title = input.required<string>();
  readonly reviewerRole = input.required<string>();
  readonly facts = input.required<Fact[]>();
  readonly signals = input<string[] | undefined>(undefined);
  readonly decisionOptions = input.required<string[]>();
  readonly history = input<ReviewerBriefHistoryItem[] | undefined>(undefined);
  /** Skip auto-generation (e.g. for already-decided items). */
  readonly disabled = input<boolean | undefined>(undefined);
  readonly className = input<string | undefined>(undefined);

  protected readonly state = signal<'idle' | 'loading' | 'done' | 'error'>('idle');
  protected readonly result = signal<BriefResult | null>(null);
  private readonly mounted = signal(false);

  // Serialize the inputs (same key order as the prototype).
  private readonly payload = computed(() =>
    JSON.stringify({
      subjectType: this.subjectType(),
      title: this.title(),
      reviewerRole: this.reviewerRole(),
      facts: this.facts(),
      signals: this.signals(),
      decisionOptions: this.decisionOptions(),
      history: this.history(),
    }),
  );

  protected readonly risk = computed(() => {
    const r = this.result();
    return r ? RISK_STYLES[r.riskLevel] : RISK_STYLES.low;
  });

  protected readonly rootClass = computed(
    () =>
      `rounded-lg border p-5 ${this.state() === 'done' ? this.risk().wrap : 'border-[#013366]/20 bg-[#013366]/5'} ${this.className() ?? ''}`,
  );

  protected readonly badgeClass = computed(
    () => `inline-flex items-center gap-1 rounded-full px-2.5 py-0.5 text-xs font-semibold ${this.risk().badge}`,
  );

  constructor() {
    afterNextRender(() => this.mounted.set(true));
    effect(() => {
      if (!this.mounted()) return;
      if (this.disabled() || this.state() !== 'idle') return;
      const cached = this.aiRepository.readJsonCache<BriefResult>(`lcrb.brief.${this.aiRepository.hashKey(this.payload())}`);
      if (cached) {
        this.result.set(cached);
        this.state.set('done');
      }
    });
  }

  protected async run() {
    const payload = this.payload();
    this.state.set('loading');
    try {
      // Deduped + cached + concurrency-gated by requestAiJson.
      const data = await this.aiRepository.requestJson<BriefResult>({
        url: '/api/reviewer-brief',
        body: payload,
        key: `lcrb.brief.${this.aiRepository.hashKey(payload)}`,
      });
      this.result.set(data);
      this.state.set('done');
    } catch {
      this.state.set('error');
    }
  }
}
