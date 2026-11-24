/** next/image with `unoptimized` and `priority` renders a plain <img> with these attributes. */
import { booleanAttribute, ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { cn } from '@shared/util/utils';

@Component({
  selector: 'bc-logo',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `<img
    [alt]="'Government of British Columbia'"
    width="1118"
    height="308"
    decoding="async"
    data-nimg="1"
    fetchpriority="high"
    [class]="imgClass()"
    style="color: transparent"
    [src]="reversed() ? '/bc-logo-white.png' : '/bc-logo.png'"
  />`,
})
export class BCLogo {
  readonly userClass = input<string | undefined>(undefined, { alias: 'class' });
  readonly reversed = input(false, { transform: booleanAttribute });
  protected readonly imgClass = computed(() => cn('h-10 w-auto md:h-12', this.userClass()));
}
