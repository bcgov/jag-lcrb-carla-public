/** Text size and contrast controls. */
import { ChangeDetectionStrategy, Component } from '@angular/core';
import { injectA11y } from '@core/a11y/a11y-provider';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';

@Component({
  selector: 'bc-accessibility-bar',
  imports: [LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="bg-[#f2f2f2] text-[#2d2d2d] text-sm border-b border-[#dbdcdc]">
      <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex items-center justify-end gap-1 h-9">
          <span class="sr-only" id="a11y-label">Accessibility options</span>
          <span class="hidden sm:inline text-[#606060] mr-1 text-xs uppercase tracking-wide">Text size</span>
          <button
            type="button"
            (click)="a11y.decreaseText()"
            [disabled]="a11y.textScale() === 'normal'"
            class="inline-flex items-center gap-1 px-2 py-1 rounded hover:bg-[#dbdcdc] focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-[#255A90] disabled:opacity-40 disabled:cursor-not-allowed"
            aria-label="Decrease text size"
          >
            <svg lucide="ZoomOut" class="h-4 w-4" aria-hidden="true"></svg>
            <span aria-hidden="true" class="text-xs">A-</span>
          </button>
          <button
            type="button"
            (click)="a11y.increaseText()"
            [disabled]="a11y.textScale() === 'x-large'"
            class="inline-flex items-center gap-1 px-2 py-1 rounded hover:bg-[#dbdcdc] focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-[#255A90] disabled:opacity-40 disabled:cursor-not-allowed"
            aria-label="Increase text size"
          >
            <svg lucide="ZoomIn" class="h-4 w-4" aria-hidden="true"></svg>
            <span aria-hidden="true" class="text-xs">A+</span>
          </button>
          <button
            type="button"
            (click)="a11y.resetText()"
            [disabled]="a11y.textScale() === 'normal'"
            class="inline-flex items-center gap-1 px-2 py-1 rounded hover:bg-[#dbdcdc] focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-[#255A90] disabled:opacity-40 disabled:cursor-not-allowed"
            aria-label="Reset text size"
          >
            <svg lucide="RotateCcw" class="h-3.5 w-3.5" aria-hidden="true"></svg>
          </button>
          <div class="w-px h-5 bg-[#dbdcdc] mx-1" aria-hidden="true"></div>
          <button
            type="button"
            (click)="a11y.toggleContrast()"
            [attr.aria-pressed]="a11y.highContrast()"
            class="inline-flex items-center gap-1.5 px-2 py-1 rounded hover:bg-[#dbdcdc] focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-[#255A90]"
          >
            <svg lucide="Contrast" class="h-4 w-4" aria-hidden="true"></svg>
            <span class="text-xs">{{ a11y.highContrast() ? 'Standard contrast' : 'High contrast' }}</span>
          </button>
        </div>
      </div>
    </div>
  `,
})
export class AccessibilityBar {
  protected readonly a11y = injectA11y();
}
