// Progress bar with role "progressbar" and an indicator translated by (100 - value)%.
import { ChangeDetectionStrategy, Component, computed, input, numberAttribute } from '@angular/core';
import { cn } from '@shared/util/utils';

@Component({
  selector: 'div[uiProgress]',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: {
    role: 'progressbar',
    'data-slot': 'progress',
    'aria-valuemin': '0',
    '[attr.aria-valuemax]': 'max()',
    '[attr.aria-valuenow]': 'value()',
    '[attr.data-state]': 'state()',
    '[attr.data-value]': 'value()',
    '[attr.data-max]': 'max()',
    '[class]': 'hostClass()',
  },
  template: `<div
    data-slot="progress-indicator"
    [attr.data-state]="state()"
    [attr.data-value]="value()"
    [attr.data-max]="max()"
    class="bg-primary h-full w-full flex-1 transition-all"
    [style.transform]="'translateX(-' + (100 - (value() || 0)) + '%)'"
  ></div>`,
})
export class Progress {
  readonly value = input(0, { transform: numberAttribute });
  readonly max = input(100, { transform: numberAttribute });
  readonly userClass = input<string | undefined>(undefined, { alias: 'class' });
  protected readonly state = computed(() => (this.value() >= this.max() ? 'complete' : 'loading'));
  protected readonly hostClass = computed(() => cn('bg-primary/20 relative h-2 w-full overflow-hidden rounded-full', this.userClass()));
}
