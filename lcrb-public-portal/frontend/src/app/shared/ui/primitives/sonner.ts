// Toaster with the system theme; raise toasts with `toast` from '@spartan-ng/brain/sonner'.
import { booleanAttribute, ChangeDetectionStrategy, Component, input } from '@angular/core';
import { BrnSonnerImports, type ToasterProps } from '@spartan-ng/brain/sonner';

@Component({
  selector: 'ui-toaster',
  imports: [BrnSonnerImports],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `<brn-sonner-toaster
    theme="system"
    class="toaster group"
    [position]="position()"
    [richColors]="richColors()"
    [style]="{ '--normal-bg': 'var(--popover)', '--normal-text': 'var(--popover-foreground)', '--normal-border': 'var(--border)' }"
  />`,
})
export class Toaster {
  readonly position = input<ToasterProps['position']>('bottom-right');
  readonly richColors = input(false, { transform: booleanAttribute });
}
