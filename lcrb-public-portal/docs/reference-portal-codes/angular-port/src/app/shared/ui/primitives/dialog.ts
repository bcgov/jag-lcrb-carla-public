/** Dialog on spartan brain's dialog (Angular CDK); overlay and content carry the prototype's classes and data-state, plus a close button unless showCloseButton is false. */
import { booleanAttribute, ChangeDetectionStrategy, Component, computed, Directive, forwardRef, inject, input } from '@angular/core';
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
import { ClassMerge } from '@shared/util/class-merge';
import { LucideIcon } from './lucide-icon';

export const DIALOG_OVERLAY_CLASS =
  'data-[state=open]:animate-in data-[state=closed]:animate-out data-[state=closed]:fade-out-0 data-[state=open]:fade-in-0 fixed inset-0 z-50 bg-black/50';

@Component({
  selector: 'ui-dialog',
  exportAs: 'uiDialog',
  providers: [
    { provide: BrnDialog, useExisting: forwardRef(() => Dialog) },
    provideBrnDialogDefaultOptions({ backdropClass: DIALOG_OVERLAY_CLASS }),
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `<ng-content />`,
})
export class Dialog extends BrnDialog {}

/** The prototype's DialogTrigger (`asChild` on a button). */
@Directive({
  selector: 'button[uiDialogTrigger]',
  hostDirectives: [{ directive: BrnDialogTrigger, inputs: ['id', 'type'] }],
  host: { 'data-slot': 'dialog-trigger' },
})
export class DialogTrigger {}

/** Structural portal for the dialog content: `<ui-dialog-content *uiDialogPortal>`. */
@Directive({
  selector: '[uiDialogPortal]',
  hostDirectives: [{ directive: BrnDialogContent, inputs: ['context: uiDialogPortalContext'] }],
})
export class DialogPortal {}

@Component({
  selector: 'ui-dialog-content',
  imports: [BrnDialogClose, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  // No role here: the CDK dialog container already has role="dialog" and aria-modal, so a second role would duplicate it.
  host: {
    'data-slot': 'dialog-content',
    '[attr.data-state]': 'state()',
    '[class]': 'hostClass()',
  },
  template: `
    <ng-content />
    @if (showCloseButton()) {
      <button
        brnDialogClose
        data-slot="dialog-close"
        [attr.data-state]="state()"
        class="ring-offset-background focus:ring-ring data-[state=open]:bg-accent data-[state=open]:text-muted-foreground absolute top-4 right-4 rounded-xs opacity-70 transition-opacity hover:opacity-100 focus:ring-2 focus:ring-offset-2 focus:outline-hidden disabled:pointer-events-none [&_svg]:pointer-events-none [&_svg]:shrink-0 [&_svg:not([class*='size-'])]:size-4"
      >
        <svg lucide="XIcon"></svg>
        <span class="sr-only">Close</span>
      </button>
    }
  `,
})
export class DialogContent {
  private readonly dialogRef = inject(BrnDialogRef);
  readonly userClass = input<string | undefined>(undefined, { alias: 'class' });
  readonly showCloseButton = input(true, { transform: booleanAttribute });
  protected readonly state = computed(() => this.dialogRef?.state() ?? 'closed');
  protected readonly hostClass = computed(() =>
    cn(
      'bg-background data-[state=open]:animate-in data-[state=closed]:animate-out data-[state=closed]:fade-out-0 data-[state=open]:fade-in-0 data-[state=closed]:zoom-out-95 data-[state=open]:zoom-in-95 fixed top-[50%] left-[50%] z-50 grid w-full max-w-[calc(100%-2rem)] translate-x-[-50%] translate-y-[-50%] gap-4 rounded-lg border p-6 shadow-lg duration-200 sm:max-w-lg',
      this.userClass(),
    ),
  );
}

@Directive({ selector: 'button[uiDialogClose]', hostDirectives: [BrnDialogClose], host: { 'data-slot': 'dialog-close' } })
export class DialogClose {}

@Directive({ selector: '[uiDialogHeader]', host: { 'data-slot': 'dialog-header' } })
export class DialogHeader extends ClassMerge {
  protected baseClass() {
    return 'flex flex-col gap-2 text-center sm:text-left';
  }
}

@Directive({ selector: '[uiDialogFooter]', host: { 'data-slot': 'dialog-footer' } })
export class DialogFooter extends ClassMerge {
  protected baseClass() {
    return 'flex flex-col-reverse gap-2 sm:flex-row sm:justify-end';
  }
}

/** Radix renders the title as an h2 and the description as a p; use those tags. */
@Directive({ selector: '[uiDialogTitle]', hostDirectives: [BrnDialogTitle], host: { 'data-slot': 'dialog-title' } })
export class DialogTitle extends ClassMerge {
  protected baseClass() {
    return 'text-lg leading-none font-semibold';
  }
}

@Directive({
  selector: '[uiDialogDescription]',
  hostDirectives: [BrnDialogDescription],
  host: { 'data-slot': 'dialog-description' },
})
export class DialogDescription extends ClassMerge {
  protected baseClass() {
    return 'text-muted-foreground text-sm';
  }
}

export const DIALOG = [
  Dialog,
  DialogTrigger,
  DialogPortal,
  DialogContent,
  DialogClose,
  DialogHeader,
  DialogFooter,
  DialogTitle,
  DialogDescription,
] as const;
