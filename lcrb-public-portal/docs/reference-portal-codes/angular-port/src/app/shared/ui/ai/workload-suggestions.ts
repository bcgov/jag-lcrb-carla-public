/** Portfolio-level "AI Suggest" panel using plain fetch (no cache). Quirk kept: a suggestion's link only renders when its `linkLabel` matches a label in `links`. */
import { ChangeDetectionStrategy, Component, inject, input, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { LucideIcon } from '../primitives/lucide-icon';
import { AI_REPOSITORY } from '@data-access/repositories/ai/ai.repository';

export interface WorkloadStat {
  label: string;
  value: string;
}

export interface WorkloadItem {
  label: string;
  detail: string;
}

export interface WorkloadLink {
  label: string;
  href: string;
}

interface Suggestion {
  title: string;
  reason: string;
  urgency: 'urgent' | 'soon' | 'routine';
  linkLabel?: string;
}

const URGENCY_META: Record<Suggestion['urgency'], { label: string; chip: string; dot: string }> = {
  urgent: { label: 'Urgent', chip: 'bg-red-100 text-red-800 border-red-200', dot: 'bg-red-500' },
  soon: { label: 'Soon', chip: 'bg-amber-100 text-amber-800 border-amber-200', dot: 'bg-amber-500' },
  routine: { label: 'Routine', chip: 'bg-slate-100 text-slate-700 border-slate-200', dot: 'bg-slate-400' },
};

@Component({
  selector: 'workload-suggestions',
  imports: [LucideIcon, RouterLink],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (state() === 'idle' || collapsed()) {
      <button
        type="button"
        (click)="hasCached() ? collapsed.set(false) : suggest()"
        class="flex w-full items-center justify-between gap-2 rounded-lg border border-[#013366]/20 bg-[#013366]/5 px-4 py-2.5 text-left transition-colors hover:bg-[#013366]/10"
      >
        <span class="flex items-center gap-2">
          <svg lucide="Sparkles" class="h-4 w-4 shrink-0 text-[#013366]"></svg>
          <span class="text-sm font-medium text-[#013366]">
            {{ hasCached() ? 'AI: Show suggested priorities' : triggerLabel() }}
          </span>
        </span>
        <svg lucide="ChevronRight" class="h-4 w-4 shrink-0 text-[#013366]"></svg>
      </button>
    } @else {
      <div class="rounded-lg border border-[#013366]/20 bg-card p-4 text-left">
        <div class="flex items-center gap-2">
          <svg lucide="Sparkles" class="h-4 w-4 shrink-0 text-[#013366]"></svg>
          <h4 class="text-sm font-semibold text-foreground">{{ heading() }}</h4>
          <span
            class="ml-auto rounded-full bg-[#013366]/10 px-2 py-0.5 text-[10px] font-medium uppercase tracking-wide text-[#013366]"
          >
            AI
          </span>
          @if (state() === 'done') {
            <button
              type="button"
              (click)="collapsed.set(true)"
              class="-mr-1 rounded-md p-1 text-muted-foreground transition-colors hover:bg-muted hover:text-foreground"
              aria-label="Collapse suggestions"
            >
              <svg lucide="ChevronUp" class="h-4 w-4"></svg>
            </button>
          }
        </div>

        @if (state() === 'loading') {
          <div class="mt-3 flex items-center gap-2 text-sm text-muted-foreground">
            <svg lucide="Loader2" class="h-4 w-4 animate-spin"></svg>
            Reviewing your workload…
          </div>
        }

        @if (state() === 'error') {
          <p class="mt-3 text-sm text-muted-foreground">
            Couldn't generate suggestions right now.
            <button type="button" (click)="suggest()" class="font-medium text-[#013366] hover:underline">
              Retry
            </button>
          </p>
        }

        @if (state() === 'done') {
          @if (suggestions().length === 0) {
            <p class="mt-3 text-sm text-muted-foreground">Nothing needs your attention right now.</p>
          } @else {
            <ul class="mt-3 space-y-2.5">
              @for (s of suggestions(); track $index) {
                @let meta = urgencyMeta[s.urgency];
                @let href = hrefFor(s.linkLabel);
                <li class="rounded-md border border-border bg-background p-3">
                  <div class="flex items-start gap-2">
                    <span class="mt-1.5 h-2 w-2 shrink-0 rounded-full {{ meta.dot }}" aria-hidden="true"></span>
                    <div class="flex-1">
                      <div class="flex flex-wrap items-center gap-2">
                        <span class="text-sm font-medium text-foreground">{{ s.title }}</span>
                        <span
                          class="rounded-full border px-1.5 py-0.5 text-[10px] font-medium uppercase tracking-wide {{ meta.chip }}"
                        >
                          {{ meta.label }}
                        </span>
                      </div>
                      <p class="mt-0.5 text-sm leading-relaxed text-muted-foreground">{{ s.reason }}</p>
                      @if (href) {
                        <a
                          [routerLink]="tree(href)"
                          class="mt-1.5 inline-flex items-center gap-1 text-sm font-medium text-[#013366] hover:underline"
                        >
                          {{ s.linkLabel }}
                          <svg lucide="ArrowRight" class="h-3.5 w-3.5"></svg>
                        </a>
                      }
                    </div>
                  </div>
                </li>
              }
            </ul>
          }

          <div class="mt-3 flex items-center justify-between gap-2">
            <p class="text-[11px] text-muted-foreground">AI-generated guidance · you make the decision.</p>
            <button
              type="button"
              (click)="suggest()"
              class="inline-flex items-center gap-1 text-xs font-medium text-[#013366] hover:underline"
            >
              <svg lucide="RotateCw" class="h-3 w-3"></svg>
              Refresh
            </button>
          </div>
        }
      </div>
    }
  `,
})
export class WorkloadSuggestions {
  private readonly ai = inject(AI_REPOSITORY);
  readonly scope = input.required<'compliance' | 'police' | 'local-government'>();
  /** Heading shown on the expanded panel, e.g. "Suggested priorities". */
  readonly heading = input('Suggested priorities');
  /** Label on the collapsed trigger, e.g. "AI: Suggest priorities". */
  readonly triggerLabel = input('AI: Suggest priorities');
  /** Aggregate snapshot of the workload, computed by the caller. */
  readonly stats = input.required<WorkloadStat[]>();
  readonly items = input.required<WorkloadItem[]>();
  /** Navigation options; a suggestion may deep-link to one of these by label. */
  readonly links = input<WorkloadLink[]>([]);

  protected readonly urgencyMeta = URGENCY_META;
  protected readonly state = signal<'idle' | 'loading' | 'done' | 'error'>('idle');
  protected readonly collapsed = signal(false);
  protected readonly suggestions = signal<Suggestion[]>([]);

  protected hasCached(): boolean {
    return this.state() === 'done';
  }

  private readonly router = inject(Router);
  /** Parsed so a `?query` in the href renders as-is (a string routerLink would encode the `?`). */
  protected tree(href: string) {
    return this.router.parseUrl(href);
  }

  protected hrefFor(label?: string): string | undefined {
    return this.links().find((l) => l.label === label)?.href;
  }

  protected async suggest(): Promise<void> {
    this.collapsed.set(false);
    this.state.set('loading');
    try {
      const res = await this.ai.fetchApi('/api/workload-suggest', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ scope: this.scope(), stats: this.stats(), items: this.items(), links: this.links() }),
      });
      if (!res.ok) throw new Error(String(res.status));
      const data = (await res.json()) as { suggestions: Suggestion[] };
      this.suggestions.set(data.suggestions ?? []);
      this.state.set('done');
    } catch {
      this.state.set('error');
    }
  }
}
