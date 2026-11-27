// Scroll area using native overflow with the scrollbar hidden at rest.
import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { cn } from '@shared/util/utils';

@Component({
  selector: 'div[uiScrollArea]',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { 'data-slot': 'scroll-area', dir: 'ltr', style: 'position: relative; --radix-scroll-area-corner-width: 0px; --radix-scroll-area-corner-height: 0px', '[class]': 'hostClass()' },
  styles: `.viewport { scrollbar-width: none; } .viewport::-webkit-scrollbar { display: none; }`,
  template: `<div
    data-slot="scroll-area-viewport"
    data-radix-scroll-area-viewport=""
    class="viewport focus-visible:ring-ring/50 size-full rounded-[inherit] transition-[color,box-shadow] outline-none focus-visible:ring-[3px] focus-visible:outline-1"
    style="overflow: hidden scroll"
  >
    <div style="min-width: 100%; display: table"><ng-content /></div>
  </div>`,
})
export class ScrollArea {
  readonly userClass = input<string | undefined>(undefined, { alias: 'class' });
  protected readonly hostClass = computed(() => cn('relative', this.userClass()));
}
