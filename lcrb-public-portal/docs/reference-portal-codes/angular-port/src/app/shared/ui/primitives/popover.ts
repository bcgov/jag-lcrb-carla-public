/** Popover on spartan brain's popover with Radix defaults (align "center", sideOffset 4); set `align`/`sideOffset` on <ui-popover>. */
import { ChangeDetectionStrategy, Component, computed, Directive, inject } from '@angular/core';
import { BrnOverlayRef } from '@spartan-ng/brain/overlay';
import { BrnPopover, BrnPopoverContent, BrnPopoverTrigger, provideBrnPopoverConfig } from '@spartan-ng/brain/popover';
import { ClassMerge } from '@shared/util/class-merge';

@Component({
  selector: 'ui-popover',
  exportAs: 'uiPopover',
  providers: [provideBrnPopoverConfig({ align: 'center', sideOffset: 4 })],
  hostDirectives: [
    {
      directive: BrnPopover,
      inputs: ['align', 'sideOffset', 'state', 'closeOnOutsidePointerEvents', 'offsetX', 'attachTo'],
      outputs: ['stateChanged', 'closed'],
    },
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { 'data-slot': 'popover', style: 'display: contents' },
  template: `<ng-content />`,
})
export class Popover {}

@Directive({
  selector: 'button[uiPopoverTrigger]',
  hostDirectives: [{ directive: BrnPopoverTrigger, inputs: ['id', 'type'] }],
  host: { 'data-slot': 'popover-trigger' },
})
export class PopoverTrigger {}

@Directive({
  selector: '[uiPopoverPortal]',
  hostDirectives: [{ directive: BrnPopoverContent, inputs: ['context: uiPopoverPortalContext'] }],
})
export class PopoverPortal {}

@Directive({
  selector: '[uiPopoverContent]',
  host: { 'data-slot': 'popover-content', '[attr.data-state]': 'state()' },
})
export class PopoverContent extends ClassMerge {
  private readonly overlayRef = inject(BrnOverlayRef, { optional: true });
  protected readonly state = computed(() => this.overlayRef?.state?.() ?? 'open');
  protected baseClass() {
    return 'bg-popover text-popover-foreground data-[state=open]:animate-in data-[state=closed]:animate-out data-[state=closed]:fade-out-0 data-[state=open]:fade-in-0 data-[state=closed]:zoom-out-95 data-[state=open]:zoom-in-95 data-[side=bottom]:slide-in-from-top-2 data-[side=left]:slide-in-from-right-2 data-[side=right]:slide-in-from-left-2 data-[side=top]:slide-in-from-bottom-2 z-50 w-72 origin-(--radix-popover-content-transform-origin) rounded-md border p-4 shadow-md outline-hidden';
  }
}

export const POPOVER = [Popover, PopoverTrigger, PopoverPortal, PopoverContent] as const;
