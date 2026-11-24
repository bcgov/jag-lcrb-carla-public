/** Root has role "radiogroup", items are buttons with role "radio"; arrow keys move and select. Controlled like Radix (see controllable.ts). */
import { booleanAttribute, ChangeDetectionStrategy, Component, computed, Directive, ElementRef, inject, input, output } from '@angular/core';
import { cn } from '@shared/util/utils';
import { controllable, injectFlushSync } from './controllable';
import { LucideIcon } from './lucide-icon';

@Directive({
  selector: 'div[uiRadioGroup]',
  host: {
    role: 'radiogroup',
    'data-slot': 'radio-group',
    '[class]': 'hostClass()',
    '(keydown)': 'onKeydown($event)',
  },
})
export class RadioGroup {
  readonly value = input<string | undefined>(undefined);
  readonly defaultValue = input<string | undefined>(undefined);
  readonly valueChange = output<string | undefined>();
  /** The selected value: the binding if bound, otherwise the group's own state. */
  readonly state = controllable(this.value, this.defaultValue, (v) => this.valueChange.emit(v));
  readonly userClass = input<string | undefined>(undefined, { alias: 'class' });
  private readonly el = inject<ElementRef<HTMLElement>>(ElementRef).nativeElement;
  protected readonly hostClass = computed(() => cn('grid gap-3', this.userClass()));

  protected onKeydown(event: KeyboardEvent): void {
    const keys = ['ArrowDown', 'ArrowRight', 'ArrowUp', 'ArrowLeft'];
    if (!keys.includes(event.key)) return;
    const items = Array.from(this.el.querySelectorAll<HTMLButtonElement>('button[role="radio"]:not([disabled])'));
    const current = items.indexOf(document.activeElement as HTMLButtonElement);
    if (current === -1) return;
    event.preventDefault();
    const step = event.key === 'ArrowDown' || event.key === 'ArrowRight' ? 1 : -1;
    const next = items[(current + step + items.length) % items.length];
    next.focus();
    next.click();
  }
}

@Component({
  selector: 'button[uiRadioGroupItem]',
  imports: [LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: {
    type: 'button',
    role: 'radio',
    'data-slot': 'radio-group-item',
    '[attr.aria-checked]': 'checked() ? "true" : "false"',
    '[attr.data-state]': 'checked() ? "checked" : "unchecked"',
    '[attr.data-disabled]': 'disabled() ? "" : null',
    '[attr.tabindex]': 'checked() || !group.state.value() ? 0 : -1',
    '[disabled]': 'disabled()',
    '[class]': 'hostClass()',
    '(click)': 'select()',
  },
  template: `@if (checked()) {
    <span data-slot="radio-group-indicator" data-state="checked" class="relative flex items-center justify-center">
      <svg lucide="CircleIcon" class="fill-primary absolute top-1/2 left-1/2 size-2 -translate-x-1/2 -translate-y-1/2"></svg>
    </span>
  }`,
})
export class RadioGroupItem {
  protected readonly group = inject(RadioGroup);
  readonly value = input.required<string>();
  readonly disabled = input(false, { transform: booleanAttribute });
  readonly userClass = input<string | undefined>(undefined, { alias: 'class' });
  protected readonly checked = computed(() => this.group.state.value() === this.value());
  protected readonly hostClass = computed(() =>
    cn(
      'border-input text-primary focus-visible:border-ring focus-visible:ring-ring/50 aria-invalid:ring-destructive/20 dark:aria-invalid:ring-destructive/40 aria-invalid:border-destructive dark:bg-input/30 aspect-square size-4 shrink-0 rounded-full border shadow-xs transition-[color,box-shadow] outline-none focus-visible:ring-[3px] disabled:cursor-not-allowed disabled:opacity-50',
      this.userClass(),
    ),
  );
  private readonly flushSync = injectFlushSync();
  protected select(): void {
    if (this.disabled()) return;
    this.group.state.set(this.value());
    this.flushSync();
  }
}

export const RADIO_GROUP = [RadioGroup, RadioGroupItem] as const;
