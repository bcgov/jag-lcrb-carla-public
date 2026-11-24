/** Every saved draft with a one-click Resume; renders nothing when empty and fills in after the first render, as the prototype's localStorage store does. */
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { Button } from '@shared/ui/primitives/button';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { Progress } from '@shared/ui/primitives/progress';
import { APPLICATIONS_REPOSITORY } from '@data-access/repositories/applications/applications.repository';

function relativeDate(iso: string): string {
  const d = new Date(iso);
  if (Number.isNaN(d.getTime())) return '';
  const days = Math.floor((Date.now() - d.getTime()) / 86_400_000);
  if (days === 0) return 'Saved today';
  if (days === 1) return 'Saved yesterday';
  if (days < 7) return `Saved ${days} days ago`;
  return `Saved ${d.toLocaleDateString('en-CA', { month: 'short', day: 'numeric' })}`;
}

/** Best-effort human label for a draft, from whatever the form captured so far. */
function draftSubtitle(formData: Record<string, unknown>): string | null {
  const candidates = ['eventName', 'establishmentName', 'businessName', 'organizationName', 'applicantName'];
  for (const key of candidates) {
    const v = formData[key];
    if (typeof v === 'string' && v.trim()) return v.trim();
  }
  return null;
}

/** One row of tiles on desktop; the rest sit behind a "Show all" toggle. */
const VISIBLE_LIMIT = 3;

@Component({
  selector: 'drafts-panel',
  imports: [Button, LucideIcon, Progress, RouterLink],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (drafts().length > 0) {
      <section aria-labelledby="drafts-heading" class="mb-12">
        <div class="mb-4 flex items-center gap-2">
          <svg lucide="FileClock" class="h-5 w-5 text-[#38598A]" aria-hidden="true"></svg>
          <h2 id="drafts-heading" class="text-lg font-semibold">Pick up where you left off</h2>
          <span class="rounded-full bg-muted px-2 py-0.5 text-xs font-medium text-muted-foreground"
            >{{ drafts().length }} {{ drafts().length === 1 ? 'draft' : 'drafts' }}</span
          >
        </div>

        <ul id="drafts-list" class="grid grid-cols-1 gap-3 md:grid-cols-2 lg:grid-cols-3">
          @for (row of rows(); track row.draft.draftId) {
            <li class="flex flex-col gap-3 rounded-lg border bg-card p-4">
              <div class="min-w-0">
                <p class="truncate font-medium">{{ row.draft.title }}</p>
                <p class="truncate text-sm text-muted-foreground">{{ row.subtitle ?? row.saved }}</p>
              </div>
              <div class="flex items-center gap-3">
                <div uiProgress [value]="row.draft.progress" class="h-1.5 flex-1" aria-label="Draft progress"></div>
                <span class="text-xs tabular-nums text-muted-foreground">{{ round(row.draft.progress) }}%</span>
              </div>
              <div class="flex items-center justify-between gap-2">
                @if (row.subtitle) {
                  <span class="text-xs text-muted-foreground">{{ row.saved }}</span>
                }
                <div class="ml-auto flex items-center gap-1">
                  <button
                    uiButton
                    type="button"
                    variant="ghost"
                    size="sm"
                    class="text-muted-foreground"
                    (click)="store().remove(row.draft.draftId)"
                    [attr.aria-label]="'Delete draft: ' + row.draft.title"
                  >
                    <svg lucide="Trash2" class="h-4 w-4" aria-hidden="true"></svg>
                  </button>
                  <a uiButton size="sm" [routerLink]="parse(row.draft.resumeHref)">
                    Resume
                    <svg lucide="ArrowRight" class="ml-1.5 h-4 w-4" aria-hidden="true"></svg>
                  </a>
                </div>
              </div>
            </li>
          }
        </ul>

        @if (hiddenCount() > 0) {
          <div class="mt-3 flex justify-center">
            <button
              uiButton
              type="button"
              variant="ghost"
              size="sm"
              class="text-[#38598A]"
              [attr.aria-expanded]="expanded()"
              aria-controls="drafts-list"
              (click)="expanded.set(!expanded())"
            >
              @if (expanded()) {
                Show fewer
                <svg lucide="ChevronUp" class="ml-1.5 h-4 w-4" aria-hidden="true"></svg>
              } @else {
                Show {{ hiddenCount() }} more {{ hiddenCount() === 1 ? 'draft' : 'drafts' }}
                <svg lucide="ChevronDown" class="ml-1.5 h-4 w-4" aria-hidden="true"></svg>
              }
            </button>
          </div>
        }
      </section>
    }
  `,
})
export class DraftsPanel {
  private readonly applicationsRepository = inject(APPLICATIONS_REPOSITORY);
  protected readonly store = this.applicationsRepository.drafts();
  private readonly router = inject(Router);
  protected readonly expanded = signal(false);
  protected readonly drafts = computed(() => this.store().drafts);
  protected readonly hiddenCount = computed(() => this.drafts().length - VISIBLE_LIMIT);
  protected readonly rows = computed(() => {
    const all = this.drafts();
    const visible = this.expanded() || this.hiddenCount() <= 0 ? all : all.slice(0, VISIBLE_LIMIT);
    return visible.map((draft) => ({
      draft,
      subtitle: draftSubtitle(draft.formData),
      saved: relativeDate(draft.updatedAt),
    }));
  });
  protected readonly round = Math.round;
  /** Resume links may carry `?draft=`; parsed so the href renders as next/link does. */
  protected parse(href: string) {
    return this.router.parseUrl(href);
  }
}
