/** Current processing times; `focusType` highlights one licence type, otherwise a portal-wide summary. Computed once per instance. */
import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { Tooltip } from '@shared/ui/primitives/tooltip';
import type { LicenceType } from '@domain/models/applications';
import { computeProcessingTimes, getProcessingTimeFor } from '@domain/applications/processing-times';

const TREND = {
  faster: { icon: 'TrendingDown', text: 'Faster than usual', cls: 'text-green-700' },
  slower: { icon: 'TrendingUp', text: 'Slower than usual', cls: 'text-amber-700' },
  steady: { icon: 'Minus', text: 'Steady', cls: 'text-muted-foreground' },
} as const;

@Component({
  selector: 'processing-times-banner',
  imports: [LucideIcon, Tooltip],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <section aria-label="Current application processing times" class="rounded-lg border border-border bg-muted/40 p-4">
      <div class="flex flex-wrap items-center justify-between gap-x-4 gap-y-2">
        <div class="flex items-center gap-2">
          <span class="flex h-8 w-8 shrink-0 items-center justify-center rounded-md bg-[#013366]/10 text-[#013366]">
            <svg lucide="Clock" class="h-4 w-4" aria-hidden="true"></svg>
          </span>
          <div>
            <h2 class="flex items-center gap-1.5 text-sm font-semibold text-foreground">
              Current processing times
              <button
                type="button"
                class="text-muted-foreground hover:text-foreground"
                aria-label="How processing times are calculated"
                [uiTooltip]="tip"
                [delayDuration]="150"
                tooltipClass="max-w-xs"
              >
                <svg lucide="Info" class="h-3.5 w-3.5" aria-hidden="true"></svg>
              </button>
              <ng-template #tip>
                Live estimates from applications decided in the last {{ months() }} months. Actual times vary with
                completeness and the complexity of your application.
              </ng-template>
            </h2>
            <p class="text-xs text-muted-foreground">From the LCRB analytics engine &middot; updated {{ updated() }}</p>
          </div>
        </div>

        @if (focus(); as f) {
          <div class="flex items-center gap-4">
            <div class="text-right">
              <p class="text-xs text-muted-foreground">{{ f.label }}</p>
              <p class="text-sm">
                <span class="font-semibold text-foreground">Typically ~{{ f.medianDays }} days</span>&ngsp;<span
                  class="text-muted-foreground"
                  >&middot; most within {{ f.p90Days }} days</span
                >
              </p>
            </div>
            <span [attr.class]="'inline-flex items-center gap-1 text-xs font-medium ' + trend().cls">
              <svg [lucide]="trend().icon" class="h-3.5 w-3.5" aria-hidden="true"></svg>
              {{ trend().text }}
            </span>
          </div>
        } @else {
          <p class="text-xs text-muted-foreground">
            Portal-wide median&ngsp;<span class="font-semibold text-foreground">{{ result.overallMedianDays }} days</span>
          </p>
        }
      </div>

      <!-- Representative highlights only when not focused on one type. -->
      @if (!focus()) {
        <ul class="mt-3 grid grid-cols-2 gap-x-4 gap-y-2 sm:grid-cols-4">
          @for (d of highlights; track d.licenceType) {
            <li class="min-w-0">
              <p class="truncate text-xs text-muted-foreground">{{ d.label }}</p>
              <p class="text-sm font-semibold text-foreground">~{{ d.medianDays }} days</p>
            </li>
          }
        </ul>
      }
    </section>
  `,
})
export class ProcessingTimesBanner {
  readonly focusType = input<LicenceType | undefined>(undefined);

  protected readonly result = computeProcessingTimes();
  protected readonly focus = computed(() => {
    const type = this.focusType();
    return type ? getProcessingTimeFor(type, this.result) : undefined;
  });
  protected readonly trend = computed(() => TREND[this.focus()?.trend ?? 'steady']);
  protected readonly months = () => Math.round(this.result.windowDays / 30);
  protected readonly updated = () =>
    new Date(this.result.updatedAt).toLocaleDateString('en-CA', { month: 'short', day: 'numeric' });

  protected readonly highlights = [...this.result.byLicenceType]
    .filter((d) => d.sampleSize >= 10)
    .sort((a, b) => a.medianDays - b.medianDays)
    .slice(0, 4);
}
