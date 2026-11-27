// Sheet on spartan brain's dialog, with `side` on the content.
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
import { ClassMerge } from '@shared/util/class-merge';
import { LucideIcon } from './lucide-icon';

const SHEET_OVERLAY_CLASS =
  'data-[state=open]:animate-in data-[state=closed]:animate-out data-[state=closed]:fade-out-0 data-[state=open]:fade-in-0 fixed inset-0 z-50 bg-black/50';

export type SheetSide = 'top' | 'right' | 'bottom' | 'left';

@Component({
  selector: 'ui-sheet',
  exportAs: 'uiSheet',
  providers: [
    { provide: BrnDialog, useExisting: forwardRef(() => Sheet) },
    provideBrnDialogDefaultOptions({ backdropClass: SHEET_OVERLAY_CLASS }),
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `<ng-content />`,
})
export class Sheet extends BrnDialog {}

@Directive({
  selector: 'button[uiSheetTrigger]',
  hostDirectives: [{ directive: BrnDialogTrigger, inputs: ['id', 'type'] }],
  host: { 'data-slot': 'sheet-trigger' },
})
export class SheetTrigger {}

@Directive({
  selector: '[uiSheetPortal]',
  hostDirectives: [{ directive: BrnDialogContent, inputs: ['context: uiSheetPortalContext'] }],
})
export class SheetPortal {}

@Component({
  selector: 'ui-sheet-content',
  imports: [BrnDialogClose, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  // No role here: the CDK dialog container already has role="dialog" and aria-modal.
  host: {
    'data-slot': 'sheet-content',
    '[attr.data-state]': 'state()',
    '[class]': 'hostClass()',
  },
  template: `
    <ng-content />
    <button
      brnDialogClose
      [attr.data-state]="state()"
      class="ring-offset-background focus:ring-ring data-[state=open]:bg-secondary absolute top-4 right-4 rounded-xs opacity-70 transition-opacity hover:opacity-100 focus:ring-2 focus:ring-offset-2 focus:outline-hidden disabled:pointer-events-none"
    >
      <svg lucide="XIcon" class="size-4"></svg>
      <span class="sr-only">Close</span>
    </button>
  `,
})
export class SheetContent {
  private readonly dialogRef = inject(BrnDialogRef);
  readonly userClass = input<string | undefined>(undefined, { alias: 'class' });
  readonly side = input<SheetSide>('right');
  protected readonly state = computed(() => this.dialogRef?.state() ?? 'closed');
  protected readonly hostClass = computed(() => {
    const side = this.side();
    return cn(
      'bg-background data-[state=open]:animate-in data-[state=closed]:animate-out fixed z-50 flex flex-col gap-4 shadow-lg transition ease-in-out data-[state=closed]:duration-300 data-[state=open]:duration-500',
      side === 'right' &&
        'data-[state=closed]:slide-out-to-right data-[state=open]:slide-in-from-right inset-y-0 right-0 h-full w-3/4 border-l sm:max-w-sm',
      side === 'left' &&
        'data-[state=closed]:slide-out-to-left data-[state=open]:slide-in-from-left inset-y-0 left-0 h-full w-3/4 border-r sm:max-w-sm',
      side === 'top' &&
        'data-[state=closed]:slide-out-to-top data-[state=open]:slide-in-from-top inset-x-0 top-0 h-auto border-b',
      side === 'bottom' &&
        'data-[state=closed]:slide-out-to-bottom data-[state=open]:slide-in-from-bottom inset-x-0 bottom-0 h-auto border-t',
      this.userClass(),
    );
  });
}

@Directive({ selector: 'button[uiSheetClose]', hostDirectives: [BrnDialogClose], host: { 'data-slot': 'sheet-close' } })
export class SheetClose {}

@Directive({ selector: '[uiSheetHeader]', host: { 'data-slot': 'sheet-header' } })
export class SheetHeader extends ClassMerge {
  protected baseClass() {
    return 'flex flex-col gap-1.5 p-4';
  }
}

@Directive({ selector: '[uiSheetFooter]', host: { 'data-slot': 'sheet-footer' } })
export class SheetFooter extends ClassMerge {
  protected baseClass() {
    return 'mt-auto flex flex-col gap-2 p-4';
  }
}

@Directive({ selector: '[uiSheetTitle]', hostDirectives: [BrnDialogTitle], host: { 'data-slot': 'sheet-title' } })
export class SheetTitle extends ClassMerge {
  protected baseClass() {
    return 'text-foreground font-semibold';
  }
}

@Directive({ selector: '[uiSheetDescription]', hostDirectives: [BrnDialogDescription], host: { 'data-slot': 'sheet-description' } })
export class SheetDescription extends ClassMerge {
  protected baseClass() {
    return 'text-muted-foreground text-sm';
  }
}

export const SHEET = [
  Sheet,
  SheetTrigger,
  SheetPortal,
  SheetContent,
  SheetClose,
  SheetHeader,
  SheetFooter,
  SheetTitle,
  SheetDescription,
] as const;
