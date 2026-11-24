/** Rolls a full application up into a single PDF and downloads it client-side. */
import { ChangeDetectionStrategy, Component, input, signal } from '@angular/core';
import { Button } from '../primitives/button';
import { LucideIcon } from '../primitives/lucide-icon';
import { generateApplicationPdf, type RollupData } from '@shared/util/pdf/pdf-rollup';

@Component({
  selector: 'download-pdf-button',
  imports: [Button, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `<button uiButton type="button" [variant]="variant()" [size]="size()" [class]="userClass()" (click)="handleClick()" [disabled]="busy()">
    @if (busy()) {
      <svg lucide="Loader2" class="h-4 w-4 animate-spin" aria-hidden="true"></svg>
    } @else {
      <svg lucide="Download" class="h-4 w-4" aria-hidden="true"></svg>
    }
    {{ busy() ? 'Preparing PDF…' : label() }}
  </button>`,
})
export class DownloadPdfButton {
  readonly data = input.required<RollupData>();
  readonly label = input('Download as PDF');
  readonly variant = input<'default' | 'outline' | 'secondary' | 'ghost'>('outline');
  readonly size = input<'default' | 'sm' | 'lg'>('default');
  readonly userClass = input<string | undefined>(undefined, { alias: 'class' });
  protected readonly busy = signal(false);

  protected async handleClick(): Promise<void> {
    this.busy.set(true);
    try {
      // Yield to the browser so the spinner can paint before the (sync) render.
      await new Promise((r) => setTimeout(r, 30));
      generateApplicationPdf(this.data());
    } finally {
      this.busy.set(false);
    }
  }
}
