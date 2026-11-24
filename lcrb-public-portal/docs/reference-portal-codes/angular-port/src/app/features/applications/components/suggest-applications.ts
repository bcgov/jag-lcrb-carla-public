/** Collapsed-by-default "AI Suggest Applications" panel using plain fetch (no cache); quirk kept: refresh/retry has no dedupe or cancel. */
import { ChangeDetectionStrategy, Component, inject, input, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { path } from '@/app/route-registry';
import { AI_REPOSITORY } from '@data-access/repositories/ai/ai.repository';

export interface CatalogItem {
  slug: string;
  title: string;
  description: string;
  licenceType: string;
  kind: string;
}

export interface HeldLicence {
  type: string;
  name: string;
  status: string;
  expiryDate: string;
  flags: string[];
}

interface Suggestion {
  title: string;
  reason: string;
  urgency: 'urgent' | 'soon' | 'routine';
  appSlug: string;
}

const URGENCY_META: Record<Suggestion['urgency'], { label: string; chip: string; dot: string }> = {
  urgent: {
    label: 'Urgent',
    chip: 'bg-red-100 text-red-800 border-red-200',
    dot: 'bg-red-500',
  },
  soon: {
    label: 'Soon',
    chip: 'bg-amber-100 text-amber-800 border-amber-200',
    dot: 'bg-amber-500',
  },
  routine: {
    label: 'Routine',
    chip: 'bg-slate-100 text-slate-700 border-slate-200',
    dot: 'bg-slate-400',
  },
};

function daysBetween(from: Date, to: Date) {
  return Math.round((to.getTime() - from.getTime()) / (1000 * 60 * 60 * 24));
}

@Component({
  selector: 'suggest-applications',
  imports: [LucideIcon, RouterLink],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (state() === 'idle' || collapsed()) {
      <button
        type="button"
        (click)="hasCached() ? collapsed.set(false) : suggest()"
        class="flex w-full items-center justify-between gap-2 rounded-lg border border-[#013366]/20 bg-[#013366]/5 px-4 py-3 text-left transition-colors hover:bg-[#013366]/10"
      >
        <span class="flex items-center gap-2">
          <svg lucide="Sparkles" class="h-4 w-4 shrink-0 text-[#013366]"></svg>
          <span class="text-sm font-medium text-[#013366]">
            {{ hasCached() ? 'AI: Show suggested applications' : 'AI: Suggest applications for my business' }}
          </span>
        </span>
        <svg lucide="ChevronRight" class="h-4 w-4 shrink-0 text-[#013366]"></svg>
      </button>
    } @else {
      <div class="rounded-lg border border-[#013366]/20 bg-card p-4">
        <div class="flex items-center gap-2">
          <svg lucide="Sparkles" class="h-4 w-4 shrink-0 text-[#013366]"></svg>
          <h2 class="text-sm font-semibold text-foreground">Suggested applications for your business</h2>
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
            Reviewing your licences…
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
            <p class="mt-3 text-sm text-muted-foreground">
              Nothing stands out for your business right now — browse the full catalog below.
            </p>
          } @else {
            <ul class="mt-3 space-y-2.5">
              @for (s of suggestions(); track $index) {
                @let meta = urgencyMeta[s.urgency];
                @let label = appTitle(s.appSlug);
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
                      <a
                        [routerLink]="path('applications.flow', { slug: s.appSlug })"
                        class="mt-1.5 inline-flex items-center gap-1 text-sm font-medium text-[#013366] hover:underline"
                      >
                        {{ label ? 'Start: ' + label : 'Start this application' }}
                        <svg lucide="ArrowRight" class="h-3.5 w-3.5"></svg>
                      </a>
                    </div>
                  </div>
                </li>
              }
            </ul>
          }

          <div class="mt-3 flex items-center justify-between gap-2">
            <p class="text-[11px] text-muted-foreground">AI-generated guidance · verify with official policy.</p>
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
export class SuggestApplications {
  private readonly aiRepository = inject(AI_REPOSITORY);
  protected readonly path = path;
  readonly legalName = input.required<string>();
  readonly licences = input.required<HeldLicence[]>();
  readonly availableApplications = input.required<CatalogItem[]>();

  protected readonly urgencyMeta = URGENCY_META;
  protected readonly state = signal<'idle' | 'loading' | 'done' | 'error'>('idle');
  protected readonly collapsed = signal(false);
  protected readonly suggestions = signal<Suggestion[]>([]);

  protected hasCached(): boolean {
    return this.state() === 'done';
  }

  protected appTitle(slug: string): string | undefined {
    return this.availableApplications().find((a) => a.slug === slug)?.title;
  }

  protected async suggest(): Promise<void> {
    this.collapsed.set(false);
    this.state.set('loading');
    try {
      const res = await this.aiRepository.fetchApi('/api/suggest-applications', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          legalName: this.legalName(),
          licences: this.licences().map((l) => ({
            type: l.type,
            name: l.name,
            status: l.status,
            expiryDate: l.expiryDate,
            daysToExpiry: daysBetween(new Date(), new Date(l.expiryDate)),
            flags: l.flags,
          })),
          availableApplications: this.availableApplications(),
        }),
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
