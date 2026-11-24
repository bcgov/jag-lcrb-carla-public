/** Alert dialog on spartan brain's dialog: role "alertdialog", no close button, outside clicks do not close it; Action and Cancel close after their own click handlers, like Radix. */
import { ChangeDetectionStrategy, Component, computed, Directive, forwardRef, inject, input } from '@angular/core';
import {
  BrnDialog,
  BrnDialogClose,
  BrnDialogContent,
  BrnDialogDescription,
  BrnDialogRef,
  BrnDialogTitle,
  BrnDialogTrigger,
  provideBrnDialogDefaultOptions,
} from '@spartan-ng/brain/dialog';
import { cn } from '@shared/util/utils';
import { buttonVariants } from './button';
import { ClassMerge } from '@shared/util/class-merge';

const ALERT_DIALOG_OVERLAY_CLASS =
  'data-[state=open]:animate-in data-[state=closed]:animate-out data-[state=closed]:fade-out-0 data-[state=open]:fade-in-0 fixed inset-0 z-50 bg-black/50';

@Component({
  selector: 'ui-alert-dialog',
  exportAs: 'uiAlertDialog',
  providers: [
    { provide: BrnDialog, useExisting: forwardRef(() => AlertDialog) },
    provideBrnDialogDefaultOptions({
      backdropClass: ALERT_DIALOG_OVERLAY_CLASS,
      role: 'alertdialog',
      closeOnOutsidePointerEvents: false,
    }),
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `<ng-content />`,
})
export class AlertDialog extends BrnDialog {}

@Directive({
  selector: 'button[uiAlertDialogTrigger]',
  hostDirectives: [{ directive: BrnDialogTrigger, inputs: ['id', 'type'] }],
  host: { 'data-slot': 'alert-dialog-trigger' },
})
export class AlertDialogTrigger {}

@Directive({
  selector: '[uiAlertDialogPortal]',
  hostDirectives: [{ directive: BrnDialogContent, inputs: ['context: uiAlertDialogPortalContext'] }],
})
export class AlertDialogPortal {}

@Component({
  selector: 'ui-alert-dialog-content',
  changeDetection: ChangeDetectionStrategy.OnPush,
  // No role here: the CDK dialog container already has role="alertdialog" and aria-modal, so a second role would duplicate it.
  host: {
    'data-slot': 'alert-dialog-content',
    '[attr.data-state]': 'state()',
    '[class]': 'hostClass()',
  },
  template: `<ng-content />`,
})
export class AlertDialogContent {
  private readonly dialogRef = inject(BrnDialogRef);
  readonly userClass = input<string | undefined>(undefined, { alias: 'class' });
  protected readonly state = computed(() => this.dialogRef?.state() ?? 'closed');
  protected readonly hostClass = computed(() =>
    cn(
      'bg-background data-[state=open]:animate-in data-[state=closed]:animate-out data-[state=closed]:fade-out-0 data-[state=open]:fade-in-0 data-[state=closed]:zoom-out-95 data-[state=open]:zoom-in-95 fixed top-[50%] left-[50%] z-50 grid w-full max-w-[calc(100%-2rem)] translate-x-[-50%] translate-y-[-50%] gap-4 rounded-lg border p-6 shadow-lg duration-200 sm:max-w-lg',
      this.userClass(),
    ),
  );
}

@Directive({ selector: '[uiAlertDialogHeader]', host: { 'data-slot': 'alert-dialog-header' } })
export class AlertDialogHeader extends ClassMerge {
  protected baseClass() {
    return 'flex flex-col gap-2 text-center sm:text-left';
  }
}

@Directive({ selector: '[uiAlertDialogFooter]', host: { 'data-slot': 'alert-dialog-footer' } })
export class AlertDialogFooter extends ClassMerge {
  protected baseClass() {
    return 'flex flex-col-reverse gap-2 sm:flex-row sm:justify-end';
  }
}

@Directive({ selector: '[uiAlertDialogTitle]', hostDirectives: [BrnDialogTitle], host: { 'data-slot': 'alert-dialog-title' } })
export class AlertDialogTitle extends ClassMerge {
  protected baseClass() {
    return 'text-lg font-semibold';
  }
}

@Directive({
  selector: '[uiAlertDialogDescription]',
  hostDirectives: [BrnDialogDescription],
  host: { 'data-slot': 'alert-dialog-description' },
})
export class AlertDialogDescription extends ClassMerge {
  protected baseClass() {
    return 'text-muted-foreground text-sm';
  }
}

@Directive({ selector: 'button[uiAlertDialogAction]', hostDirectives: [BrnDialogClose] })
export class AlertDialogAction extends ClassMerge {
  protected baseClass() {
    return buttonVariants();
  }
}

@Directive({ selector: 'button[uiAlertDialogCancel]', hostDirectives: [BrnDialogClose] })
export class AlertDialogCancel extends ClassMerge {
  protected baseClass() {
    return buttonVariants({ variant: 'outline' });
  }
}

export const ALERT_DIALOG = [
  AlertDialog,
  AlertDialogTrigger,
  AlertDialogPortal,
  AlertDialogContent,
  AlertDialogHeader,
  AlertDialogFooter,
  AlertDialogTitle,
  AlertDialogDescription,
  AlertDialogAction,
  AlertDialogCancel,
] as const;
