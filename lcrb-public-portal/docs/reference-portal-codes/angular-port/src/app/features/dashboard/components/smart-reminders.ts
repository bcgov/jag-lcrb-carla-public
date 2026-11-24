/** The prototype reads the session cache on mount and calls /api/nudges only on "Rewrite with AI"; `shown` freezes the first computed list like `useState(initial)`. */
import {
  afterNextRender,
  ChangeDetectionStrategy,
  Component,
  computed,
  effect,
  inject,
  input,
  signal,
  untracked,
} from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { type IconName } from '@shared/ui/primitives/icons';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { AI_REPOSITORY } from '@data-access/repositories/ai/ai.repository';

export interface ReminderContextItem {
  kind: string;
  label: string;
  detail: string;
  href: string;
}

interface Nudge {
  id: string;
  priority: 'high' | 'medium' | 'low';
  title: string;
  body: string;
  ctaLabel: string;
  ctaHref: string;
}

const PRIORITY_STYLES: Record<Nudge['priority'], { wrap: string; chip: string; icon: IconName }> = {
  high: { wrap: 'border-red-300 bg-red-50', chip: 'bg-red-100 text-red-800', icon: 'AlertTriangle' },
  medium: { wrap: 'border-orange-300 bg-orange-50', chip: 'bg-orange-100 text-orange-800', icon: 'CalendarClock' },
  low: { wrap: 'border-border bg-card', chip: 'bg-muted text-muted-foreground', icon: 'CheckCircle2' },
};

const DAY_MS = 24 * 60 * 60 * 1000;

/** Reminders computed on the client from the same account data the AI sees, so the dashboard never waits on the model; the AI call only refines them and is cached per session. */
function buildDeterministicNudges(items: ReminderContextItem[], today: string): Nudge[] {
  const now = new Date(today).getTime();
  const nudges: Nudge[] = [];

  for (const item of items) {
    if (item.kind === 'licence') {
      const match = item.detail.match(/(\d{4}-\d{2}-\d{2})/);
      if (!match) continue;
      const diffDays = Math.floor((new Date(match[1]).getTime() - now) / DAY_MS);
      if (diffDays < 0) {
        nudges.push({
          id: `renew-${item.href}`,
          priority: 'high',
          title: `${item.label} has expired`,
          body: `This licence expired on ${match[1]}. Renew it right away to avoid an interruption to your operations.`,
          ctaLabel: 'Renew now',
          ctaHref: item.href,
        });
      } else if (diffDays <= 30) {
        nudges.push({
          id: `renew-${item.href}`,
          priority: 'medium',
          title: `${item.label} is due for renewal`,
          body: `This licence expires on ${match[1]} — that's ${diffDays} day${diffDays === 1 ? '' : 's'} away. Renew now to stay compliant.`,
          ctaLabel: 'Renew now',
          ctaHref: item.href,
        });
      }
    } else if (item.kind === 'application') {
      const detail = item.detail.toLowerCase();
      const needsAction = /await|document|action|missing|pending|info/.test(detail);
      nudges.push({
        id: `app-${item.href}`,
        priority: needsAction ? 'high' : 'low',
        title: needsAction ? `Action needed: ${item.label}` : `${item.label} in progress`,
        body: needsAction
          ? `${item.detail}. Open the application to provide what's needed and keep it moving.`
          : `${item.detail}.`,
        ctaLabel: needsAction ? 'Complete now' : 'View application',
        ctaHref: item.href,
      });
    } else if (item.kind === 'notices') {
      const count = Number(item.detail.match(/(\d+)/)?.[1] ?? 0);
      if (count > 0) {
        nudges.push({
          id: `notices-${item.href}`,
          priority: 'medium',
          title: `You have ${count} unread notice${count === 1 ? '' : 's'}`,
          body: 'These may need your attention. Open your inbox to review them.',
          ctaLabel: 'View notices',
          ctaHref: item.href,
        });
      }
    }
  }

  const rank = { high: 0, medium: 1, low: 2 };
  return nudges.sort((a, b) => rank[a.priority] - rank[b.priority]).slice(0, 5);
}

function cacheKey(items: ReminderContextItem[], today: string): string {
  return `lcrb.nudges.${today}.${items.map((i) => `${i.kind}:${i.detail}`).join('|')}`;
}

@Component({
  selector: 'smart-reminders',
  imports: [LucideIcon, RouterLink],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (shown().length === 0) {
      <div class="flex items-center gap-4 rounded-lg border border-green-300 bg-green-50 p-5 text-left">
        <div
          class="flex h-12 w-12 shrink-0 items-center justify-center rounded-full bg-green-100 text-green-700"
        >
          <svg lucide="CheckCircle2" class="h-6 w-6" aria-hidden="true"></svg>
        </div>
        <div class="flex-1">
          <div class="font-semibold text-green-900">You're all caught up</div>
          <div class="mt-0.5 text-sm text-green-800">
            Nothing needs your attention right now. We'll let you know when something comes up.
          </div>
        </div>
      </div>
    } @else {
      <div class="space-y-3 text-left">
        <div class="flex items-center gap-1.5">
          <svg lucide="Sparkles" class="h-4 w-4 text-[#013366]" aria-hidden="true"></svg>
          <span class="text-sm font-semibold text-[#013366]">Smart reminders</span>
          @if (refined()) {
            <span
              class="rounded-full bg-[#013366]/10 px-2 py-0.5 text-[10px] font-medium uppercase tracking-wide text-[#013366]"
            >
              AI
            </span>
          }
          @if (!refined()) {
            <button
              type="button"
              (click)="refine()"
              [disabled]="refining()"
              class="ml-auto inline-flex items-center gap-1 text-xs font-medium text-muted-foreground hover:text-[#013366] disabled:opacity-70"
            >
              @if (refining()) {
                <svg lucide="Loader2" class="h-3 w-3 animate-spin" aria-hidden="true"></svg>
                Rewriting…
              } @else {
                <svg lucide="Sparkles" class="h-3 w-3" aria-hidden="true"></svg>
                Rewrite with AI
              }
            </button>
          }
        </div>
        @for (n of shown(); track n.id) {
          @let styles = priorityStyles[n.priority] ?? priorityStyles.low;
          <div class="flex items-start gap-4 rounded-lg border p-4 {{ styles.wrap }}">
            <div class="mt-0.5 shrink-0">
              <svg [lucide]="styles.icon" class="h-5 w-5 text-foreground/70" aria-hidden="true"></svg>
            </div>
            <div class="min-w-0 flex-1">
              <div class="flex items-center gap-2">
                <span class="font-semibold text-foreground">{{ n.title }}</span>
                <span
                  class="rounded-full px-2 py-0.5 text-[10px] font-medium uppercase tracking-wide {{ styles.chip }}"
                >
                  {{ n.priority }}
                </span>
              </div>
              <p class="mt-0.5 text-sm text-muted-foreground">{{ n.body }}</p>
              <a
                [routerLink]="tree(n.ctaHref)"
                class="mt-2 inline-flex items-center gap-1 text-sm font-medium text-[#013366] hover:underline"
              >
                {{ n.ctaLabel }}
                <svg lucide="ArrowRight" class="h-3.5 w-3.5" aria-hidden="true"></svg>
              </a>
            </div>
          </div>
        }
      </div>
    }
  `,
})
export class SmartReminders {
  private readonly aiRepository = inject(AI_REPOSITORY);
  readonly items = input.required<ReminderContextItem[]>();

  protected readonly priorityStyles = PRIORITY_STYLES;
  private readonly today = new Date().toISOString().slice(0, 10);
  private readonly initial = computed(() => buildDeterministicNudges(this.items(), this.today));
  private readonly override = signal<Nudge[] | null>(null);
  private frozen: Nudge[] | undefined;
  /** `useState(initial)`: the first computed list is kept even if `items` changes later. */
  protected readonly shown = computed(() => this.override() ?? (this.frozen ??= this.initial()));

  protected readonly refining = signal(false);
  protected readonly refined = signal(false);
  private readonly mounted = signal(false);

  constructor() {
    afterNextRender(() => this.mounted.set(true));
    // Mount effect: reuse a cached refinement without calling the API.
    effect(() => {
      if (!this.mounted()) return;
      const items = this.items();
      untracked(() => {
        const cached = this.aiRepository.readJsonCache<{ nudges: Nudge[] }>(cacheKey(items, this.today));
        if (cached?.nudges?.length) {
          this.override.set(cached.nudges);
          this.refined.set(true);
        }
      });
    });
  }

  private readonly router = inject(Router);
  /** Parsed so a `?query` in the href renders as-is (a string routerLink would encode the `?`). */
  protected tree(href: string) {
    return this.router.parseUrl(href);
  }

  protected refine(): void {
    this.refining.set(true);
    this.aiRepository.requestJson<{ nudges: Nudge[] }>({
      url: '/api/nudges',
      body: { items: this.items(), today: this.today },
      key: cacheKey(this.items(), this.today),
    })
      .then((data) => {
        if (data?.nudges?.length) this.override.set(data.nudges);
        this.refined.set(true);
      })
      .catch(() => {
        // Keep the deterministic list visible on any failure.
      })
      .finally(() => this.refining.set(false));
  }
}
