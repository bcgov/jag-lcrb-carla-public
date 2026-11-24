/** Dropdown menu on Angular CDK Menu with only the parts the prototype uses (trigger, content, item, label, separator). Radix defaults: side "bottom", align "center", sideOffset 4. */
import { FocusMonitor, InputModalityDetector } from '@angular/cdk/a11y';
import { CdkMenu, CdkMenuItem, CdkMenuTrigger } from '@angular/cdk/menu';
import { booleanAttribute, computed, Directive, effect, ElementRef, forwardRef, inject, input, numberAttribute, signal } from '@angular/core';
import {
  createMenuPosition,
  deriveMenuSideFromTransformOrigin,
  MENU_SIDE,
  type MenuAlign,
  type MenuSide,
  updateMenuPosition,
} from '@spartan-ng/brain/core';
import { ClassMerge } from '@shared/util/class-merge';

@Directive({
  selector: '[uiDropdownMenuTrigger]',
  exportAs: 'uiDropdownMenuTrigger',
  providers: [{ provide: MENU_SIDE, useExisting: forwardRef(() => DropdownMenuTrigger) }],
  hostDirectives: [
    {
      directive: CdkMenuTrigger,
      inputs: ['cdkMenuTriggerFor: uiDropdownMenuTrigger'],
      outputs: ['cdkMenuOpened: opened', 'cdkMenuClosed: closed'],
    },
  ],
  host: {
    'data-slot': 'dropdown-menu-trigger',
    '[attr.data-state]': 'cdkTrigger.isOpen() ? "open" : "closed"',
    '(click)': 'focusContentOnPointerOpen()',
  },
})
export class DropdownMenuTrigger {
  protected readonly cdkTrigger = inject(CdkMenuTrigger, { host: true });
  private readonly focusMonitor = inject(FocusMonitor);
  readonly align = input<MenuAlign>('center');
  readonly side = input<MenuSide>('bottom');
  private readonly position = computed(() => createMenuPosition(this.align(), this.side()));

  constructor() {
    (this.cdkTrigger as { transformOriginSelector?: string }).transformOriginSelector = '[data-slot="dropdown-menu-content"]';
    effect(() => updateMenuPosition(this.cdkTrigger, this.position()));
  }

  /** Radix focuses the menu content when a pointer opens it, CDK focuses the first item; keyboard opens never reach here, so they keep CDK's first-item focus, as in Radix. */
  protected focusContentOnPointerOpen(): void {
    const menu = (this.cdkTrigger as unknown as { childMenu?: CdkMenu }).childMenu;
    if (!this.cdkTrigger.isOpen() || !menu) return;
    // So ArrowDown then moves to the first item, as from Radix's focused content.
    (menu as unknown as { keyManager?: { updateActiveItem(index: number): void } }).keyManager?.updateActiveItem(-1);
    this.focusMonitor.focusVia(menu.nativeElement, 'mouse');
  }
}

@Directive({
  selector: '[uiDropdownMenuContent]',
  hostDirectives: [CdkMenu],
  host: {
    'data-slot': 'dropdown-menu-content',
    '[attr.data-state]': 'state()',
    '[attr.data-side]': 'side()',
    '[style.margin-block]': 'sideOffset() + "px"',
  },
})
export class DropdownMenuContent extends ClassMerge {
  private readonly menu = inject(CdkMenu);
  private readonly el = inject<ElementRef<HTMLElement>>(ElementRef).nativeElement;
  private readonly menuSide = inject(MENU_SIDE, { optional: true });
  protected readonly state = signal('open');
  protected readonly side = signal<MenuSide>(this.menuSide?.side() ?? 'bottom');
  readonly sideOffset = input(4, { transform: numberAttribute });

  constructor() {
    super();
    this.menu.closed.subscribe(() => this.state.set('closed'));
    setTimeout(() => this.side.set(deriveMenuSideFromTransformOrigin(this.el.style.transformOrigin, this.side())));
  }

  protected baseClass() {
    return 'bg-popover text-popover-foreground data-[state=open]:animate-in data-[state=closed]:animate-out data-[state=closed]:fade-out-0 data-[state=open]:fade-in-0 data-[state=closed]:zoom-out-95 data-[state=open]:zoom-in-95 data-[side=bottom]:slide-in-from-top-2 data-[side=left]:slide-in-from-right-2 data-[side=right]:slide-in-from-left-2 data-[side=top]:slide-in-from-bottom-2 z-50 max-h-(--radix-dropdown-menu-content-available-height) min-w-[8rem] origin-(--radix-dropdown-menu-content-transform-origin) overflow-x-hidden overflow-y-auto rounded-md border p-1 shadow-md';
  }
}

@Directive({
  selector: '[uiDropdownMenuItem]',
  hostDirectives: [{ directive: CdkMenuItem, inputs: ['cdkMenuItemDisabled: disabled'], outputs: ['cdkMenuItemTriggered: triggered'] }],
  host: {
    'data-slot': 'dropdown-menu-item',
    '[attr.data-variant]': 'variant()',
    '[attr.data-inset]': 'inset() ? "" : null',
    '[attr.data-disabled]': 'disabled() ? "" : null',
    '(mouseenter)': 'focusOnHover()',
  },
})
export class DropdownMenuItem extends ClassMerge {
  private readonly cdkItem = inject(CdkMenuItem, { self: true });
  private readonly parentMenu = inject(CdkMenu, { optional: true });
  private readonly modality = inject(InputModalityDetector);
  readonly variant = input<'default' | 'destructive'>('default');
  readonly inset = input(false, { transform: booleanAttribute });
  readonly disabled = input(false, { transform: booleanAttribute });

  /** Radix highlights the hovered item by focusing it (the classes use focus:). */
  protected focusOnHover(): void {
    if (this.modality.mostRecentModality === 'touch' || this.cdkItem.disabled) return;
    this.parentMenu?.setActiveMenuItem(this.cdkItem);
  }

  protected baseClass() {
    return "focus:bg-accent focus:text-accent-foreground data-[variant=destructive]:text-destructive data-[variant=destructive]:focus:bg-destructive/10 dark:data-[variant=destructive]:focus:bg-destructive/20 data-[variant=destructive]:focus:text-destructive data-[variant=destructive]:*:[svg]:!text-destructive [&_svg:not([class*='text-'])]:text-muted-foreground relative flex cursor-default items-center gap-2 rounded-sm px-2 py-1.5 text-sm outline-hidden select-none data-[disabled]:pointer-events-none data-[disabled]:opacity-50 data-[inset]:pl-8 [&_svg]:pointer-events-none [&_svg]:shrink-0 [&_svg:not([class*='size-'])]:size-4";
  }
}

@Directive({
  selector: '[uiDropdownMenuLabel]',
  host: { 'data-slot': 'dropdown-menu-label', '[attr.data-inset]': 'inset() ? "" : null' },
})
export class DropdownMenuLabel extends ClassMerge {
  readonly inset = input(false, { transform: booleanAttribute });
  protected baseClass() {
    return 'px-2 py-1.5 text-sm font-medium data-[inset]:pl-8';
  }
}

@Directive({ selector: '[uiDropdownMenuSeparator]', host: { 'data-slot': 'dropdown-menu-separator', role: 'separator' } })
export class DropdownMenuSeparator extends ClassMerge {
  protected baseClass() {
    return 'bg-border -mx-1 my-1 h-px';
  }
}

export const DROPDOWN_MENU = [
  DropdownMenuTrigger,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
] as const;
