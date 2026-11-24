/** Button with role "switch", aria-checked, data-state and a thumb span; controlled like Radix (see controllable.ts). */
import { booleanAttribute, ChangeDetectionStrategy, Component, computed, input, output } from '@angular/core';
import { cn } from '@shared/util/utils';
import { controllable, injectFlushSync } from './controllable';

@Component({
  selector: 'button[uiSwitch]',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: {
    type: 'button',
    role: 'switch',
    'data-slot': 'switch',
    '[attr.aria-checked]': 'controlled.value() ? "true" : "false"',
    '[attr.data-state]': 'state()',
    '[attr.data-disabled]': 'disabled() ? "" : null',
    '[disabled]': 'disabled()',
    '[class]': 'hostClass()',
    '(click)': 'toggle()',
  },
  template: `<span
    data-slot="switch-thumb"
    [attr.data-state]="state()"
    class="bg-background dark:data-[state=unchecked]:bg-foreground dark:data-[state=checked]:bg-primary-foreground pointer-events-none block size-4 rounded-full ring-0 transition-transform data-[state=checked]:translate-x-[calc(100%-2px)] data-[state=unchecked]:translate-x-0"
  ></span>`,
})
export class Switch {
  readonly checked = input<boolean | undefined>(undefined);
  readonly defaultChecked = input(false, { transform: booleanAttribute });
  readonly checkedChange = output<boolean>();
  protected readonly controlled = controllable(this.checked, this.defaultChecked, (v) => this.checkedChange.emit(v));
  readonly disabled = input(false, { transform: booleanAttribute });
  readonly userClass = input<string | undefined>(undefined, { alias: 'class' });
  protected readonly state = computed(() => (this.controlled.value() ? 'checked' : 'unchecked'));
  protected readonly hostClass = computed(() =>
    cn(
      'peer data-[state=checked]:bg-primary data-[state=unchecked]:bg-input focus-visible:border-ring focus-visible:ring-ring/50 dark:data-[state=unchecked]:bg-input/80 inline-flex h-[1.15rem] w-8 shrink-0 items-center rounded-full border border-transparent shadow-xs transition-all outline-none focus-visible:ring-[3px] disabled:cursor-not-allowed disabled:opacity-50',
      this.userClass(),
    ),
  );
  private readonly flushSync = injectFlushSync();
  protected toggle(): void {
    if (this.disabled()) return;
    this.controlled.set(!this.controlled.value());
    this.flushSync();
  }
}
