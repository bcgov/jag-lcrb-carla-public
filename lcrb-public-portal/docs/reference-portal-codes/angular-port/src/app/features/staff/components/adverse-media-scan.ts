/** On-demand adverse-media RAG scan; the auto-run effect is gated on a flag set in afterNextRender so it fires once. */
import { afterNextRender, ChangeDetectionStrategy, Component, effect, inject, input, signal, untracked } from '@angular/core';
import { type IconName } from '@shared/ui/primitives/icons';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { AI_REPOSITORY } from '@data-access/repositories/ai/ai.repository';

type Outcome = 'clear' | 'attention' | 'flag';
type MatchConfidence = 'confirmed' | 'possible' | 'unclear';

interface Article {
  title: string;
  publisher: string | null;
  date: string | null;
  url: string;
  relevance: string;
}

interface SubjectFinding {
  name: string;
  type: 'person' | 'business';
  outcome: Outcome;
  matchConfidence: MatchConfidence;
  summary: string;
  articles: Article[];
}

interface ScanResult {
  program: 'cannabis' | 'liquor';
  escalationTarget: string;
  scannedSubjects: { name: string; type: 'person' | 'business' }[];
  overall: { outcome: Outcome; escalates: boolean; summary: string };
  subjects: SubjectFinding[];
}

export interface AdverseMediaPerson {
  name: string;
  roles?: string[];
  entity?: string;
}

const OUTCOME_META: Record<Outcome, { label: string; chip: string; icon: IconName; iconColor: string }> = {
  clear: {
    label: 'Clear',
    chip: 'bg-green-100 text-green-800 border-green-200',
    icon: 'ShieldCheck',
    iconColor: 'text-green-600',
  },
  attention: {
    label: 'Attention',
    chip: 'bg-amber-100 text-amber-800 border-amber-200',
    icon: 'AlertTriangle',
    iconColor: 'text-amber-600',
  },
  flag: {
    label: 'Flag',
    chip: 'bg-red-100 text-red-800 border-red-200',
    icon: 'ShieldAlert',
    iconColor: 'text-red-600',
  },
};

const MATCH_META: Record<MatchConfidence, { label: string; chip: string }> = {
  confirmed: { label: 'Match confirmed', chip: 'bg-slate-200 text-slate-800 border-slate-300' },
  possible: { label: 'Possible match', chip: 'bg-slate-100 text-slate-700 border-slate-200' },
  unclear: { label: 'Unverified match', chip: 'bg-slate-100 text-slate-600 border-slate-200' },
};

@Component({
  selector: 'adverse-media-scan',
  imports: [LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (state() === 'idle' || collapsed()) {
      <button
        type="button"
        (click)="hasCached() ? collapsed.set(false) : scan()"
        class="flex w-full items-center justify-between gap-2 rounded-lg border border-[#013366]/20 bg-[#013366]/5 px-4 py-2.5 text-left transition-colors hover:bg-[#013366]/10"
      >
        <span class="flex items-center gap-2">
          <svg lucide="Newspaper" class="h-4 w-4 shrink-0 text-[#013366]"></svg>
          <span class="text-sm font-medium text-[#013366]">
            {{ hasCached() ? 'Show adverse-media scan' : 'Run adverse-media scan' }}
          </span>
        </span>
        <svg lucide="ChevronRight" class="h-4 w-4 shrink-0 text-[#013366]"></svg>
      </button>
    } @else {
      <div class="rounded-lg border border-[#013366]/20 bg-card p-4 text-left">
        <div class="flex items-center gap-2">
          <svg lucide="Newspaper" class="h-4 w-4 shrink-0 text-[#013366]"></svg>
          <h4 class="text-sm font-semibold text-foreground">Adverse-media scan</h4>
          <span
            class="rounded-full bg-[#013366]/10 px-2 py-0.5 text-[10px] font-medium uppercase tracking-wide text-[#013366]"
          >
            RAG
          </span>
          @if (state() === 'done') {
            <button
              type="button"
              (click)="collapsed.set(true)"
              class="ml-auto -mr-1 rounded-md p-1 text-muted-foreground transition-colors hover:bg-muted hover:text-foreground"
              aria-label="Collapse adverse-media scan"
            >
              <svg lucide="ChevronUp" class="h-4 w-4"></svg>
            </button>
          }
        </div>

        @if (state() === 'loading') {
          <div class="mt-3 flex items-center gap-2 text-sm text-muted-foreground">
            <svg lucide="Loader2" class="h-4 w-4 animate-spin"></svg>
            Searching open sources for the business and associated individuals…
          </div>
        }

        @if (state() === 'error') {
          <p class="mt-3 text-sm text-muted-foreground">
            Couldn't complete the scan right now.
            <button type="button" (click)="scan()" class="font-medium text-[#013366] hover:underline">
              Retry
            </button>
          </p>
        }

        @if (state() === 'done' && result(); as r) {
          <!-- Overall banner -->
          @let overall = outcomeMeta[r.overall.outcome];
          <div class="mt-3 flex items-start gap-2 rounded-md border border-border bg-muted/40 p-3">
            <svg
              [lucide]="overall.icon"
              class="mt-0.5 h-4 w-4 shrink-0 {{ overall.iconColor }}"
              aria-hidden="true"
            ></svg>
            <div class="min-w-0">
              <div class="flex flex-wrap items-center gap-2">
                <span class="text-sm font-medium text-foreground">Overall: {{ overall.label }}</span>
                @if (r.overall.escalates) {
                  <span
                    class="rounded-full border border-red-200 bg-red-100 px-1.5 py-0.5 text-[10px] font-medium uppercase tracking-wide text-red-800"
                  >
                    May warrant {{ r.escalationTarget }}
                  </span>
                }
              </div>
              <p class="mt-0.5 text-sm leading-relaxed text-muted-foreground">{{ r.overall.summary }}</p>
            </div>
          </div>

          <!-- Per-subject findings -->
          <ul class="mt-3 space-y-2.5">
            @for (s of r.subjects; track $index) {
              @let meta = outcomeMeta[s.outcome];
              <li class="rounded-md border border-border bg-background p-3">
                <div class="flex flex-wrap items-center gap-2">
                  <svg
                    [lucide]="s.type === 'business' ? 'Building2' : 'User'"
                    class="h-4 w-4 shrink-0 text-muted-foreground"
                    aria-hidden="true"
                  ></svg>
                  <span class="text-sm font-medium text-foreground">{{ s.name }}</span>
                  <span
                    class="rounded-full border px-1.5 py-0.5 text-[10px] font-medium uppercase tracking-wide {{ meta.chip }}"
                  >
                    {{ meta.label }}
                  </span>
                  @if (s.outcome !== 'clear') {
                    <span
                      class="rounded-full border px-1.5 py-0.5 text-[10px] font-medium {{ matchMeta[s.matchConfidence].chip }}"
                    >
                      {{ matchMeta[s.matchConfidence].label }}
                    </span>
                  }
                </div>
                <p class="mt-1 text-sm leading-relaxed text-muted-foreground">{{ s.summary }}</p>
                @if (s.articles.length > 0) {
                  <ul class="mt-2 space-y-1.5">
                    @for (a of s.articles; track $index) {
                      <li class="text-xs">
                        <a
                          [href]="a.url"
                          target="_blank"
                          rel="noopener noreferrer"
                          class="inline-flex items-start gap-1 font-medium text-[#013366] hover:underline"
                        >
                          <svg lucide="ExternalLink" class="mt-0.5 h-3 w-3 shrink-0" aria-hidden="true"></svg>
                          <span>{{ a.title }}{{ a.publisher ? ' — ' + a.publisher : '' }}{{ a.date ? ' (' + a.date + ')' : '' }}</span>
                        </a>
                        <p class="ml-4 text-muted-foreground">{{ a.relevance }}</p>
                      </li>
                    }
                  </ul>
                }
              </li>
            }
          </ul>

          <div class="mt-3 flex items-center justify-between gap-2 border-t border-border pt-3">
            <p class="text-[11px] leading-relaxed text-muted-foreground">
              AI open-source scan · results are unverified and may include name collisions — confirm identity before
              relying on any finding. An analyst makes the determination.
            </p>
            <button
              type="button"
              (click)="scan()"
              class="inline-flex shrink-0 items-center gap-1 text-xs font-medium text-[#013366] hover:underline"
            >
              <svg lucide="RotateCw" class="h-3 w-3"></svg>
              Rescan
            </button>
          </div>
        }
      </div>
    }
  `,
})
export class AdverseMediaScan {
  private readonly aiRepository = inject(AI_REPOSITORY);
  readonly program = input.required<'cannabis' | 'liquor'>();
  readonly applicantName = input.required<string>();
  readonly businessName = input<string | undefined>(undefined);
  readonly people = input.required<AdverseMediaPerson[]>();
  /** Automatically run the scan once when the component first mounts. */
  readonly autoRun = input(false);

  protected readonly outcomeMeta = OUTCOME_META;
  protected readonly matchMeta = MATCH_META;
  protected readonly state = signal<'idle' | 'loading' | 'done' | 'error'>('idle');
  protected readonly collapsed = signal(false);
  protected readonly result = signal<ScanResult | null>(null);
  private autoRanRef = false;
  private readonly mounted = signal(false);

  constructor() {
    afterNextRender(() => this.mounted.set(true));
    // When mounted with autoRun, kick off the scan exactly once.
    effect(() => {
      if (!this.mounted()) return;
      if (this.autoRun() && !this.autoRanRef) {
        this.autoRanRef = true;
        untracked(() => void this.scan());
      }
    });
  }

  protected hasCached(): boolean {
    return this.state() === 'done';
  }

  protected async scan(): Promise<void> {
    this.collapsed.set(false);
    this.state.set('loading');
    try {
      const body = {
        program: this.program(),
        applicantName: this.applicantName(),
        businessName: this.businessName(),
        people: this.people(),
      };
      // Deduped, cached and concurrency-gated, so autoRun mounts and repeat opens for the same subject reuse one model call.
      const data = await this.aiRepository.requestJson<ScanResult>({
        url: '/api/adverse-media',
        body,
        key: `lcrb.adverse.${this.aiRepository.hashKey(JSON.stringify(body))}`,
      });
      this.result.set(data);
      this.state.set('done');
    } catch {
      this.state.set('error');
    }
  }
}
