/** Triggers have role "tab", aria-selected and data-state; only the active content renders (Radix unmounts inactive panels). */
import { ChangeDetectionStrategy, Component, computed, Directive, ElementRef, inject, input, model } from '@angular/core';
import { cn } from '@shared/util/utils';
import { ClassMerge } from '@shared/util/class-merge';

let tabsId = 0;

@Directive({
  selector: 'div[uiTabs]',
  host: { 'data-slot': 'tabs', 'data-orientation': 'horizontal', dir: 'ltr', '[class]': 'hostClass()' },
})
export class Tabs {
  readonly value = model<string | undefined>(undefined);
  readonly userClass = input<string | undefined>(undefined, { alias: 'class' });
  readonly baseId = `ui-tabs-${++tabsId}`;
  protected readonly hostClass = computed(() => cn('flex flex-col gap-2', this.userClass()));
}

@Directive({
  selector: 'div[uiTabsList]',
  host: {
    role: 'tablist',
    'data-slot': 'tabs-list',
    'aria-orientation': 'horizontal',
    'data-orientation': 'horizontal',
    tabindex: '-1',
    '(keydown)': 'onKeydown($event)',
  },
})
export class TabsList extends ClassMerge {
  private readonly el = inject<ElementRef<HTMLElement>>(ElementRef).nativeElement;
  protected baseClass() {
    return 'bg-muted text-muted-foreground inline-flex h-9 w-fit items-center justify-center rounded-lg p-[3px]';
  }
  /** Radix: arrow keys move focus and activate (automatic activation). */
  protected onKeydown(event: KeyboardEvent): void {
    if (event.key !== 'ArrowRight' && event.key !== 'ArrowLeft') return;
    const tabs = Array.from(this.el.querySelectorAll<HTMLButtonElement>('button[role="tab"]:not([disabled])'));
    const current = tabs.indexOf(document.activeElement as HTMLButtonElement);
    if (current === -1) return;
    event.preventDefault();
    const next = tabs[(current + (event.key === 'ArrowRight' ? 1 : -1) + tabs.length) % tabs.length];
    next.focus();
    next.click();
  }
}

@Directive({
  selector: 'button[uiTabsTrigger]',
  host: {
    type: 'button',
    role: 'tab',
    'data-slot': 'tabs-trigger',
    'data-orientation': 'horizontal',
    '[attr.id]': 'id() ?? tabs.baseId + "-trigger-" + value()',
    '[attr.aria-controls]': 'tabs.baseId + "-content-" + value()',
    '[attr.aria-selected]': 'active() ? "true" : "false"',
    '[attr.data-state]': 'active() ? "active" : "inactive"',
    '[attr.tabindex]': 'active() ? null : -1',
    '(mousedown)': 'onMousedown($event)',
    '(click)': 'tabs.value.set(value())',
  },
})
export class TabsTrigger extends ClassMerge {
  protected readonly tabs = inject(Tabs);
  readonly value = input.required<string>();
  /** A consumer id overrides the generated one, as Radix spreads props after it. */
  readonly id = input<string | undefined>(undefined);
  protected readonly active = computed(() => this.tabs.value() === this.value());
  protected onMousedown(event: MouseEvent): void {
    // Radix activates on mousedown for the primary button.
    if (event.button === 0 && !event.ctrlKey) this.tabs.value.set(this.value());
  }
  protected baseClass() {
    return "data-[state=active]:bg-background dark:data-[state=active]:text-foreground focus-visible:border-ring focus-visible:ring-ring/50 focus-visible:outline-ring dark:data-[state=active]:border-input dark:data-[state=active]:bg-input/30 text-foreground dark:text-muted-foreground inline-flex h-[calc(100%-1px)] flex-1 items-center justify-center gap-1.5 rounded-md border border-transparent px-2 py-1 text-sm font-medium whitespace-nowrap transition-[color,box-shadow] focus-visible:ring-[3px] focus-visible:outline-1 disabled:pointer-events-none disabled:opacity-50 data-[state=active]:shadow-sm [&_svg]:pointer-events-none [&_svg]:shrink-0 [&_svg:not([class*='size-'])]:size-4";
  }
}

/** `<div uiTabsContent value="x">`, rendered only while active, like Radix. */
@Component({
  selector: 'div[uiTabsContent]',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: {
    role: 'tabpanel',
    'data-slot': 'tabs-content',
    'data-orientation': 'horizontal',
    tabindex: '0',
    '[attr.id]': 'id() ?? tabs.baseId + "-content-" + value()',
    '[attr.aria-labelledby]': 'tabs.baseId + "-trigger-" + value()',
    '[attr.data-state]': 'active() ? "active" : "inactive"',
    '[hidden]': '!active()',
    '[class]': 'hostClass()',
  },
  template: `@if (active()) { <ng-content /> }`,
})
export class TabsContent {
  protected readonly tabs = inject(Tabs);
  readonly value = input.required<string>();
  readonly id = input<string | undefined>(undefined);
  readonly userClass = input<string | undefined>(undefined, { alias: 'class' });
  protected readonly active = computed(() => this.tabs.value() === this.value());
  protected readonly hostClass = computed(() => cn('flex-1 outline-none', this.userClass()));
}

export const TABS = [Tabs, TabsList, TabsTrigger, TabsContent] as const;
