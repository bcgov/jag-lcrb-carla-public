/** Button with role "checkbox", aria-checked and data-state, showing a check indicator; controlled like Radix (see controllable.ts) or uncontrolled via defaultChecked. */
import { booleanAttribute, ChangeDetectionStrategy, Component, computed, input, output } from '@angular/core';
import { cn } from '@shared/util/utils';
import { controllable, injectFlushSync } from './controllable';
import { LucideIcon } from './lucide-icon';

@Component({
  selector: 'button[uiCheckbox]',
  imports: [LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: {
    type: 'button',
    role: 'checkbox',
    'data-slot': 'checkbox',
    '[attr.aria-checked]': 'state.value() ? "true" : "false"',
    '[attr.data-state]': 'state.value() ? "checked" : "unchecked"',
    '[attr.data-disabled]': 'disabled() ? "" : null',
    '[disabled]': 'disabled()',
    '[class]': 'hostClass()',
    '(click)': 'toggle()',
  },
  template: `@if (state.value()) {
    <span data-slot="checkbox-indicator" data-state="checked" class="flex items-center justify-center text-current transition-none" style="pointer-events: none">
      <svg lucide="CheckIcon" class="size-3.5"></svg>
    </span>
  }`,
})
export class Checkbox {
  readonly checked = input<boolean | undefined>(undefined);
  readonly defaultChecked = input(false, { transform: booleanAttribute });
  readonly checkedChange = output<boolean>();
  protected readonly state = controllable(this.checked, this.defaultChecked, (v) => this.checkedChange.emit(v));
  readonly disabled = input(false, { transform: booleanAttribute });
  readonly userClass = input<string | undefined>(undefined, { alias: 'class' });
  protected readonly hostClass = computed(() =>
    cn(
      'peer border-input dark:bg-input/30 data-[state=checked]:bg-primary data-[state=checked]:text-primary-foreground dark:data-[state=checked]:bg-primary data-[state=checked]:border-primary focus-visible:border-ring focus-visible:ring-ring/50 aria-invalid:ring-destructive/20 dark:aria-invalid:ring-destructive/40 aria-invalid:border-destructive size-4 shrink-0 rounded-[4px] border shadow-xs transition-shadow outline-none focus-visible:ring-[3px] disabled:cursor-not-allowed disabled:opacity-50',
      this.userClass(),
    ),
  );
  private readonly flushSync = injectFlushSync();
  protected toggle(): void {
    if (this.disabled()) return;
    this.state.set(!this.state.value());
    this.flushSync();
  }
}
