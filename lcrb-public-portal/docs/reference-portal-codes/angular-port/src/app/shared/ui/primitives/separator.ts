/** Radix renders role "none" when decorative (the default), otherwise role "separator" plus aria-orientation (vertical only). */
import { booleanAttribute, computed, Directive, input } from '@angular/core';
import { ClassMerge } from '@shared/util/class-merge';

@Directive({
  selector: 'div[uiSeparator]',
  host: {
    'data-slot': 'separator',
    '[attr.data-orientation]': 'orientation()',
    '[attr.role]': 'decorative() ? "none" : "separator"',
    '[attr.aria-orientation]': 'ariaOrientation()',
  },
})
export class Separator extends ClassMerge {
  readonly orientation = input<'horizontal' | 'vertical'>('horizontal');
  readonly decorative = input(true, { transform: booleanAttribute });
  protected readonly ariaOrientation = computed(() =>
    !this.decorative() && this.orientation() === 'vertical' ? 'vertical' : null,
  );
  protected baseClass() {
    return 'bg-border shrink-0 data-[orientation=horizontal]:h-px data-[orientation=horizontal]:w-full data-[orientation=vertical]:h-full data-[orientation=vertical]:w-px';
  }
}
