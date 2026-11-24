/** Horizontal slider; values are an array like Radix, one thumb per value, with Radix's in-bounds offset formula, pointer drag and keyboard support. */
import { booleanAttribute, ChangeDetectionStrategy, Component, computed, ElementRef, inject, input, model, numberAttribute } from '@angular/core';
import { cn } from '@shared/util/utils';

const THUMB_SIZE = 16; // size-4

/** Radix getThumbInBoundsOffset: keeps the thumb inside the track at both ends. */
function thumbOffset(percent: number): number {
  const half = THUMB_SIZE / 2;
  return half - (percent / 50) * half;
}

@Component({
  selector: 'span[uiSlider]',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: {
    'data-slot': 'slider',
    dir: 'ltr',
    'data-orientation': 'horizontal',
    '[attr.aria-disabled]': 'disabled() ? "true" : null',
    '[attr.data-disabled]': 'disabled() ? "" : null',
    style: '--radix-slider-thumb-transform: translateX(-50%)',
    '[class]': 'hostClass()',
    '(pointerdown)': 'onPointerDown($event)',
  },
  template: `
    <span data-slot="slider-track" data-orientation="horizontal" class="bg-muted relative grow overflow-hidden rounded-full data-[orientation=horizontal]:h-1.5 data-[orientation=horizontal]:w-full data-[orientation=vertical]:h-full data-[orientation=vertical]:w-1.5">
      <span data-slot="slider-range" data-orientation="horizontal" class="bg-primary absolute data-[orientation=horizontal]:h-full data-[orientation=vertical]:w-full" [style.left.%]="rangeStart()" [style.right.%]="100 - rangeEnd()"></span>
    </span>
    @for (v of values(); track $index) {
      <span style="transform: var(--radix-slider-thumb-transform); position: absolute" [style.left]="'calc(' + percent(v) + '% + ' + offset(v) + 'px)'">
        <span
          role="slider"
          data-slot="slider-thumb"
          data-orientation="horizontal"
          tabindex="0"
          aria-orientation="horizontal"
          [attr.aria-valuemin]="min()"
          [attr.aria-valuemax]="max()"
          [attr.aria-valuenow]="v"
          [attr.id]="$index === 0 ? id() : null"
          class="border-primary ring-ring/50 block size-4 shrink-0 rounded-full border bg-white shadow-sm transition-[color,box-shadow] hover:ring-4 focus-visible:ring-4 focus-visible:outline-hidden disabled:pointer-events-none disabled:opacity-50"
          (keydown)="onKeydown($event, $index)"
        ></span>
      </span>
    }
  `,
})
export class Slider {
  readonly value = model<number[] | undefined>(undefined);
  readonly min = input(0, { transform: numberAttribute });
  readonly max = input(100, { transform: numberAttribute });
  readonly step = input(1, { transform: numberAttribute });
  readonly disabled = input(false, { transform: booleanAttribute });
  readonly id = input<string | undefined>(undefined);
  readonly userClass = input<string | undefined>(undefined, { alias: 'class' });
  private readonly el = inject<ElementRef<HTMLElement>>(ElementRef).nativeElement;

  protected readonly values = computed(() => this.value() ?? [this.min(), this.max()]);
  protected readonly rangeStart = computed(() => (this.values().length > 1 ? this.percent(Math.min(...this.values())) : 0));
  protected readonly rangeEnd = computed(() => this.percent(Math.max(...this.values())));
  protected readonly hostClass = computed(() =>
    cn(
      'relative flex w-full touch-none items-center select-none data-[disabled]:opacity-50 data-[orientation=vertical]:h-full data-[orientation=vertical]:min-h-44 data-[orientation=vertical]:w-auto data-[orientation=vertical]:flex-col',
      this.userClass(),
    ),
  );

  protected percent(v: number): number {
    return ((v - this.min()) / (this.max() - this.min())) * 100;
  }
  protected offset(v: number): number {
    return thumbOffset(this.percent(v));
  }

  private setAt(index: number, raw: number): void {
    const stepped = Math.round((raw - this.min()) / this.step()) * this.step() + this.min();
    const clamped = Math.min(this.max(), Math.max(this.min(), stepped));
    const next = [...this.values()];
    if (next[index] === clamped) return;
    next[index] = clamped;
    this.value.set(next);
  }

  protected onKeydown(event: KeyboardEvent, index: number): void {
    if (this.disabled()) return;
    const v = this.values()[index];
    const big = this.step() * 10;
    const map: Record<string, number> = {
      ArrowRight: v + this.step(),
      ArrowUp: v + this.step(),
      ArrowLeft: v - this.step(),
      ArrowDown: v - this.step(),
      PageUp: v + big,
      PageDown: v - big,
      Home: this.min(),
      End: this.max(),
    };
    if (!(event.key in map)) return;
    event.preventDefault();
    this.setAt(index, map[event.key]);
  }

  protected onPointerDown(event: PointerEvent): void {
    if (this.disabled()) return;
    const rect = this.el.getBoundingClientRect();
    const toValue = (x: number) => this.min() + ((x - rect.left) / rect.width) * (this.max() - this.min());
    const start = toValue(event.clientX);
    const vals = this.values();
    const index = vals.reduce((best, v, i) => (Math.abs(v - start) < Math.abs(vals[best] - start) ? i : best), 0);
    this.setAt(index, start);
    (this.el.querySelectorAll<HTMLElement>('[role="slider"]')[index])?.focus();
    const move = (e: PointerEvent) => this.setAt(index, toValue(e.clientX));
    const up = () => {
      window.removeEventListener('pointermove', move);
      window.removeEventListener('pointerup', up);
    };
    window.addEventListener('pointermove', move);
    window.addEventListener('pointerup', up);
  }
}
