import { Directive, input } from '@angular/core';
import { cva, type VariantProps } from 'class-variance-authority';
import { ClassMerge } from '@shared/util/class-merge';

const alertVariants = cva(
  'relative w-full rounded-lg border px-4 py-3 text-sm grid has-[>svg]:grid-cols-[calc(var(--spacing)*4)_1fr] grid-cols-[0_1fr] has-[>svg]:gap-x-3 gap-y-0.5 items-start [&>svg]:size-4 [&>svg]:translate-y-0.5 [&>svg]:text-current',
  {
    variants: {
      variant: {
        default: 'bg-card text-card-foreground',
        destructive:
          'text-destructive bg-card [&>svg]:text-current *:data-[slot=alert-description]:text-destructive/90',
      },
    },
    defaultVariants: {
      variant: 'default',
    },
  },
)

export type AlertVariant = NonNullable<VariantProps<typeof alertVariants>['variant']>;

@Directive({ selector: 'div[uiAlert]', host: { 'data-slot': 'alert', role: 'alert' } })
export class Alert extends ClassMerge {
  readonly variant = input<AlertVariant | null | undefined>('default');
  protected baseClass() {
    return alertVariants({ variant: this.variant() });
  }
}

@Directive({ selector: 'div[uiAlertTitle]', host: { 'data-slot': 'alert-title' } })
export class AlertTitle extends ClassMerge {
  protected baseClass() {
    return 'col-start-2 line-clamp-1 min-h-4 font-medium tracking-tight';
  }
}

@Directive({ selector: 'div[uiAlertDescription]', host: { 'data-slot': 'alert-description' } })
export class AlertDescription extends ClassMerge {
  protected baseClass() {
    return 'text-muted-foreground col-start-2 grid justify-items-start gap-1 text-sm [&_p]:leading-relaxed';
  }
}

export const ALERT = [Alert, AlertTitle, AlertDescription] as const;
