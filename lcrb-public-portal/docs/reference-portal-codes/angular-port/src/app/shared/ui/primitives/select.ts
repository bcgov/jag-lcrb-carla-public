/** Select on an Angular CDK overlay (popper position). Items render once off-screen so the closed trigger can show the selected label, and again in the overlay when open. */
import { Overlay, type OverlayRef } from '@angular/cdk/overlay';
import { TemplatePortal } from '@angular/cdk/portal';
import { NgTemplateOutlet } from '@angular/common';
import {
  afterNextRender,
  booleanAttribute,
  ChangeDetectionStrategy,
  Component,
  computed,
  contentChild,
  DestroyRef,
  Directive,
  ElementRef,
  inject,
  Injector,
  input,
  output,
  signal,
  TemplateRef,
  viewChild,
  ViewContainerRef,
} from '@angular/core';
import { cn } from '@shared/util/utils';
import { controllable } from './controllable';
import { LucideIcon } from './lucide-icon';

@Directive({ selector: 'ng-template[uiSelectContent]' })
export class SelectContent {
  readonly template = inject(TemplateRef);
  readonly userClass = input<string | undefined>(undefined, { alias: 'class' });
}

@Component({
  selector: 'ui-select',
  imports: [NgTemplateOutlet],
  changeDetection: ChangeDetectionStrategy.OnPush,
  // A real box, not display: contents: the trigger is the prototype's direct child of its parent, so parent child-selectors (Tailwind 4 space-y-* margins) must land on this host.
  host: { 'data-slot': 'select', style: 'display: block' },
  template: `
    <ng-content />
    <div aria-hidden="true" style="position: absolute; width: 1px; height: 1px; overflow: hidden; clip: rect(0 0 0 0); white-space: nowrap">
      @if (content(); as c) { <ng-container [ngTemplateOutlet]="c.template" /> }
    </div>
    <ng-template #panel>
      <div
        data-slot="select-content"
        role="listbox"
        data-state="open"
        data-side="bottom"
        [class]="contentClass()"
        style="display: flex; flex-direction: column; outline: none; box-sizing: border-box"
        [style.--radix-select-trigger-width.px]="triggerWidth()"
        [style.--radix-select-trigger-height.px]="triggerHeight()"
        (keydown)="onListKeydown($event)"
      >
        <div
          data-radix-select-viewport=""
          class="p-1 h-[var(--radix-select-trigger-height)] w-full min-w-[var(--radix-select-trigger-width)] scroll-my-1"
          style="position: relative; flex: 1; overflow: hidden auto"
        >
          @if (content(); as c) { <ng-container [ngTemplateOutlet]="c.template" /> }
        </div>
      </div>
    </ng-template>
  `,
})
export class Select {
  readonly value = input<string | undefined>(undefined);
  readonly defaultValue = input<string | undefined>(undefined);
  readonly valueChange = output<string | undefined>();
  /** The selected value: the binding if bound, otherwise the select's own state. */
  readonly state = controllable(this.value, this.defaultValue, (v) => this.valueChange.emit(v));
  readonly disabled = input(false, { transform: booleanAttribute });
  readonly open = signal(false);
  readonly labels = signal<Record<string, string>>({});
  protected readonly content = contentChild(SelectContent);
  private readonly panel = viewChild.required<TemplateRef<unknown>>('panel');
  private readonly overlay = inject(Overlay);
  private readonly vcr = inject(ViewContainerRef);
  private readonly injector = inject(Injector);
  private overlayRef: OverlayRef | null = null;
  private triggerEl: HTMLElement | null = null;
  protected readonly triggerWidth = signal(0);
  protected readonly triggerHeight = signal(0);

  protected readonly contentClass = computed(() =>
    cn(
      'bg-popover text-popover-foreground data-[state=open]:animate-in data-[state=closed]:animate-out data-[state=closed]:fade-out-0 data-[state=open]:fade-in-0 data-[state=closed]:zoom-out-95 data-[state=open]:zoom-in-95 data-[side=bottom]:slide-in-from-top-2 data-[side=left]:slide-in-from-right-2 data-[side=right]:slide-in-from-left-2 data-[side=top]:slide-in-from-bottom-2 relative z-50 max-h-(--radix-select-content-available-height) min-w-[8rem] origin-(--radix-select-content-transform-origin) overflow-x-hidden overflow-y-auto rounded-md border shadow-md',
      'data-[side=bottom]:translate-y-1 data-[side=left]:-translate-x-1 data-[side=right]:translate-x-1 data-[side=top]:-translate-y-1',
      this.content()?.userClass(),
    ),
  );

  constructor() {
    inject(DestroyRef).onDestroy(() => this.overlayRef?.dispose());
  }

  registerTrigger(el: HTMLElement): void {
    this.triggerEl = el;
  }

  registerLabel(value: string, label: string): void {
    if (this.labels()[value] !== label) this.labels.update((l) => ({ ...l, [value]: label }));
  }

  show(): void {
    if (this.open() || this.disabled() || !this.triggerEl) return;
    const rect = this.triggerEl.getBoundingClientRect();
    this.triggerWidth.set(rect.width);
    this.triggerHeight.set(rect.height);
    this.overlayRef ??= this.overlay.create({
      positionStrategy: this.overlay
        .position()
        .flexibleConnectedTo(this.triggerEl)
        .withPositions([
          { originX: 'start', originY: 'bottom', overlayX: 'start', overlayY: 'top' },
          { originX: 'start', originY: 'top', overlayX: 'start', overlayY: 'bottom' },
        ])
        // Natural width; the horizontal shift is applied in shiftIntoView(), as floating-ui does.
        .withFlexibleDimensions(false)
        .withPush(false),
      scrollStrategy: this.overlay.scrollStrategies.reposition(),
      // Measured at its natural width, as the fixed-position Radix popper is.
      width: 'max-content',
      hasBackdrop: true,
      backdropClass: 'cdk-overlay-transparent-backdrop',
    });
    this.overlayRef.backdropClick().subscribe(() => this.hide());
    this.overlayRef.attach(new TemplatePortal(this.panel(), this.vcr));
    this.open.set(true);
    // The options render after attach: position again with the real size, then shift.
    afterNextRender(
      () => {
        this.overlayRef?.updatePosition();
        this.shiftIntoView();
      },
      { injector: this.injector },
    );
    // Focus the selected item (or the first), as Radix does on open.
    setTimeout(() => {
      const items = this.items();
      (items.find((i) => i.getAttribute('data-state') === 'checked') ?? items[0])?.focus();
    });
  }

  /** floating-ui `shift` with Radix Select's 10px collision padding; if the list is wider than the space, left padding wins and it overflows right, as in Radix. */
  private shiftIntoView(): void {
    const pane = this.overlayRef?.overlayElement;
    if (!pane) return;
    const rect = pane.getBoundingClientRect();
    const padding = 10;
    const viewport = document.documentElement.clientWidth;
    const x = Math.max(padding, Math.min(rect.left, viewport - padding - rect.width));
    if (x !== rect.left) pane.style.left = `${parseFloat(pane.style.left || '0') + (x - rect.left)}px`;
  }

  hide(focusTrigger = true): void {
    if (!this.open()) return;
    this.overlayRef?.detach();
    this.open.set(false);
    if (focusTrigger) this.triggerEl?.focus();
  }

  choose(value: string): void {
    this.state.set(value);
    this.hide();
  }

  private items(): HTMLElement[] {
    return Array.from(this.overlayRef?.overlayElement?.querySelectorAll<HTMLElement>('[role="option"]:not([data-disabled])') ?? []);
  }

  protected onListKeydown(event: KeyboardEvent): void {
    const items = this.items();
    const current = items.indexOf(document.activeElement as HTMLElement);
    if (event.key === 'ArrowDown' || event.key === 'ArrowUp') {
      event.preventDefault();
      const next = event.key === 'ArrowDown' ? Math.min(items.length - 1, current + 1) : Math.max(0, current - 1);
      items[next]?.focus();
    } else if (event.key === 'Escape' || event.key === 'Tab') {
      event.preventDefault();
      this.hide();
    }
  }
}

@Component({
  selector: 'button[uiSelectTrigger]',
  imports: [LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: {
    type: 'button',
    role: 'combobox',
    'data-slot': 'select-trigger',
    'aria-autocomplete': 'none',
    dir: 'ltr',
    '[attr.aria-expanded]': 'select.open() ? "true" : "false"',
    '[attr.data-state]': 'select.open() ? "open" : "closed"',
    '[attr.data-size]': 'size()',
    '[attr.data-placeholder]': 'select.state.value() === undefined || select.state.value() === "" ? "" : null',
    '[attr.data-disabled]': 'select.disabled() ? "" : null',
    '[disabled]': 'select.disabled()',
    '[class]': 'hostClass()',
    '(click)': 'select.open() ? select.hide() : select.show()',
    '(keydown)': 'onKeydown($event)',
  },
  template: `<ng-content /><svg lucide="ChevronDownIcon" class="size-4 opacity-50" aria-hidden="true"></svg>`,
})
export class SelectTrigger {
  protected readonly select = inject(Select);
  readonly size = input<'sm' | 'default'>('default');
  readonly userClass = input<string | undefined>(undefined, { alias: 'class' });
  protected readonly hostClass = computed(() =>
    cn(
      "border-input data-[placeholder]:text-muted-foreground [&_svg:not([class*='text-'])]:text-muted-foreground focus-visible:border-ring focus-visible:ring-ring/50 aria-invalid:ring-destructive/20 dark:aria-invalid:ring-destructive/40 aria-invalid:border-destructive dark:bg-input/30 dark:hover:bg-input/50 flex w-fit items-center justify-between gap-2 rounded-md border bg-transparent px-3 py-2 text-sm whitespace-nowrap shadow-xs transition-[color,box-shadow] outline-none focus-visible:ring-[3px] disabled:cursor-not-allowed disabled:opacity-50 data-[size=default]:h-9 data-[size=sm]:h-8 *:data-[slot=select-value]:line-clamp-1 *:data-[slot=select-value]:flex *:data-[slot=select-value]:items-center *:data-[slot=select-value]:gap-2 [&_svg]:pointer-events-none [&_svg]:shrink-0 [&_svg:not([class*='size-'])]:size-4",
      this.userClass(),
    ),
  );
  constructor() {
    this.select.registerTrigger(inject<ElementRef<HTMLElement>>(ElementRef).nativeElement);
  }
  protected onKeydown(event: KeyboardEvent): void {
    if (['Enter', ' ', 'ArrowDown', 'ArrowUp'].includes(event.key)) {
      event.preventDefault();
      this.select.show();
    }
  }
}

/** `<span uiSelectValue placeholder="…">` shows the selected item's label (or the placeholder). */
@Component({
  selector: 'span[uiSelectValue]',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { 'data-slot': 'select-value', style: 'pointer-events: none' },
  template: `{{ text() }}`,
})
export class SelectValue {
  private readonly select = inject(Select);
  readonly placeholder = input('');
  protected readonly text = computed(() => {
    const v = this.select.state.value();
    return v !== undefined && v !== '' ? (this.select.labels()[v] ?? '') : this.placeholder();
  });
}

@Component({
  selector: 'ui-select-item',
  imports: [LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: {
    role: 'option',
    'data-slot': 'select-item',
    tabindex: '-1',
    '[attr.aria-selected]': 'selected() ? "true" : "false"',
    '[attr.data-state]': 'selected() ? "checked" : "unchecked"',
    '[attr.data-disabled]': 'disabled() ? "" : null',
    '[attr.aria-disabled]': 'disabled() ? "true" : null',
    '[class]': 'hostClass()',
    '(click)': 'choose()',
    '(keydown.enter)': 'choose(); $event.preventDefault()',
    '(keydown.space)': 'choose(); $event.preventDefault()',
    '(mousemove)': 'focusOnHover()',
  },
  template: `<span class="absolute right-2 flex size-3.5 items-center justify-center">
      @if (selected()) { <svg lucide="CheckIcon" class="size-4"></svg> }
    </span>
    <span #text><ng-content /></span>`,
})
export class SelectItem {
  private readonly select = inject(Select);
  private readonly el = inject<ElementRef<HTMLElement>>(ElementRef).nativeElement;
  private readonly textEl = viewChild.required<ElementRef<HTMLElement>>('text');
  readonly value = input.required<string>();
  readonly disabled = input(false, { transform: booleanAttribute });
  readonly userClass = input<string | undefined>(undefined, { alias: 'class' });
  protected readonly selected = computed(() => this.select.state.value() === this.value());
  protected readonly hostClass = computed(() =>
    cn(
      "focus:bg-accent focus:text-accent-foreground [&_svg:not([class*='text-'])]:text-muted-foreground relative flex w-full cursor-default items-center gap-2 rounded-sm py-1.5 pr-8 pl-2 text-sm outline-hidden select-none data-[disabled]:pointer-events-none data-[disabled]:opacity-50 [&_svg]:pointer-events-none [&_svg]:shrink-0 [&_svg:not([class*='size-'])]:size-4 *:[span]:last:flex *:[span]:last:items-center *:[span]:last:gap-2",
      this.userClass(),
    ),
  );

  constructor() {
    afterNextRender(() => this.select.registerLabel(this.value(), this.textEl().nativeElement.textContent?.trim() ?? ''));
  }

  protected choose(): void {
    if (!this.disabled()) this.select.choose(this.value());
  }

  protected focusOnHover(): void {
    if (!this.disabled() && document.activeElement !== this.el) this.el.focus();
  }
}

export const SELECT = [Select, SelectTrigger, SelectValue, SelectContent, SelectItem] as const;
