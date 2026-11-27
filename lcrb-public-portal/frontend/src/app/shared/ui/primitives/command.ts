// Command menu with cmdk DOM attributes; the consumer filters, arrows and Enter select items.
import {
  booleanAttribute,
  ChangeDetectionStrategy,
  Component,
  computed,
  Directive,
  ElementRef,
  inject,
  input,
  model,
  OnDestroy,
  output,
  signal,
} from '@angular/core';
import { cn } from '@shared/util/utils';
import { ClassMerge } from '@shared/util/class-merge';
import { LucideIcon } from './lucide-icon';

let cmdkId = 0;

@Directive({
  selector: 'div[uiCommand]',
  host: { 'cmdk-root': '', 'data-slot': 'command', tabindex: '-1', '(keydown)': 'onKeydown($event)' },
})
export class Command extends ClassMerge {
  readonly id = `cmdk-${++cmdkId}`;
  private readonly items = signal<CommandItem[]>([]);
  readonly selectedItem = signal<CommandItem | null>(null);
  readonly itemCount = computed(() => this.items().length);

  register(item: CommandItem): void {
    this.items.update((list) => [...list, item]);
    if (!this.selectedItem()) this.selectedItem.set(this.sorted()[0] ?? null);
  }
  unregister(item: CommandItem): void {
    this.items.update((list) => list.filter((i) => i !== item));
    if (this.selectedItem() === item) this.selectedItem.set(this.sorted()[0] ?? null);
  }
  /** Items in DOM order. */
  private sorted(): CommandItem[] {
    return [...this.items()].sort((a, b) =>
      a.el.compareDocumentPosition(b.el) & Node.DOCUMENT_POSITION_FOLLOWING ? -1 : 1,
    );
  }
  /** Reset the selection to the first item (cmdk does this when the search changes). */
  selectFirst(): void {
    this.selectedItem.set(this.sorted()[0] ?? null);
  }

  protected onKeydown(event: KeyboardEvent): void {
    const list = this.sorted().filter((i) => !i.disabled());
    if (!list.length) return;
    const current = list.indexOf(this.selectedItem()!);
    if (event.key === 'ArrowDown' || event.key === 'ArrowUp') {
      event.preventDefault();
      const next = event.key === 'ArrowDown' ? Math.min(list.length - 1, current + 1) : Math.max(0, current - 1);
      this.selectedItem.set(list[next]);
      list[next].el.scrollIntoView?.({ block: 'nearest' });
    } else if (event.key === 'Enter' && current >= 0) {
      event.preventDefault();
      list[current].selected.emit();
    }
  }

  protected baseClass() {
    return 'bg-popover text-popover-foreground flex h-full w-full flex-col overflow-hidden rounded-md';
  }
}

@Component({
  selector: 'ui-command-input',
  imports: [LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `<div data-slot="command-input-wrapper" class="flex h-9 items-center gap-2 border-b px-3">
    <svg lucide="SearchIcon" class="size-4 shrink-0 opacity-50"></svg>
    <input
      cmdk-input=""
      data-slot="command-input"
      autocomplete="off"
      autocorrect="off"
      spellcheck="false"
      aria-autocomplete="list"
      role="combobox"
      aria-expanded="true"
      [attr.aria-controls]="command.id + '-list'"
      type="text"
      [placeholder]="placeholder()"
      [value]="value()"
      (input)="onInput($event)"
      [class]="inputClass()"
    />
  </div>`,
})
export class CommandInput {
  protected readonly command = inject(Command);
  readonly value = model('');
  readonly placeholder = input('');
  readonly userClass = input<string | undefined>(undefined, { alias: 'class' });
  protected readonly inputClass = computed(() =>
    cn(
      'placeholder:text-muted-foreground flex h-10 w-full rounded-md bg-transparent py-3 text-sm outline-hidden disabled:cursor-not-allowed disabled:opacity-50',
      this.userClass(),
    ),
  );
  protected onInput(event: Event): void {
    this.value.set((event.target as HTMLInputElement).value);
    queueMicrotask(() => this.command.selectFirst());
  }
}

@Directive({
  selector: 'div[uiCommandList]',
  host: { 'cmdk-list': '', 'data-slot': 'command-list', role: 'listbox', 'aria-label': 'Suggestions', '[attr.id]': 'command.id + "-list"' },
})
export class CommandList extends ClassMerge {
  protected readonly command = inject(Command);
  protected baseClass() {
    return 'max-h-[300px] scroll-py-1 overflow-x-hidden overflow-y-auto';
  }
}

/** Rendered only when the list has no items, like cmdk's Command.Empty. */
@Component({
  selector: 'div[uiCommandEmpty]',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { 'cmdk-empty': '', 'data-slot': 'command-empty', role: 'presentation', class: 'py-6 text-center text-sm', '[hidden]': 'command.itemCount() > 0' },
  template: `@if (command.itemCount() === 0) { <ng-content /> }`,
})
export class CommandEmpty {
  protected readonly command = inject(Command);
}

@Component({
  selector: 'div[uiCommandGroup]',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { 'cmdk-group': '', 'data-slot': 'command-group', role: 'presentation', '[attr.data-value]': 'heading()', '[class]': 'hostClass()' },
  template: `@if (heading()) {
      <div cmdk-group-heading="" aria-hidden="true" [attr.id]="headingId">{{ heading() }}</div>
    }
    <div cmdk-group-items="" role="group" [attr.aria-labelledby]="heading() ? headingId : null"><ng-content /></div>`,
})
export class CommandGroup {
  readonly heading = input<string | undefined>(undefined);
  readonly userClass = input<string | undefined>(undefined, { alias: 'class' });
  protected readonly headingId = `cmdk-group-heading-${++cmdkId}`;
  protected readonly hostClass = computed(() =>
    cn(
      'text-foreground [&_[cmdk-group-heading]]:text-muted-foreground overflow-hidden p-1 [&_[cmdk-group-heading]]:px-2 [&_[cmdk-group-heading]]:py-1.5 [&_[cmdk-group-heading]]:text-xs [&_[cmdk-group-heading]]:font-medium',
      this.userClass(),
    ),
  );
}

@Directive({
  selector: 'div[uiCommandItem]',
  host: {
    'cmdk-item': '',
    'data-slot': 'command-item',
    role: 'option',
    '[attr.aria-disabled]': 'disabled() ? "true" : "false"',
    '[attr.aria-selected]': 'isSelected() ? "true" : "false"',
    '[attr.data-disabled]': 'disabled() ? "true" : "false"',
    '[attr.data-selected]': 'isSelected() ? "true" : "false"',
    '(pointermove)': 'select()',
    '(click)': 'onClick()',
  },
})
export class CommandItem extends ClassMerge implements OnDestroy {
  private readonly command = inject(Command);
  readonly el = inject<ElementRef<HTMLElement>>(ElementRef).nativeElement;
  readonly disabled = input(false, { transform: booleanAttribute });
  readonly selected = output<void>();
  protected readonly isSelected = computed(() => this.command.selectedItem() === this);

  constructor() {
    super();
    queueMicrotask(() => this.command.register(this));
  }
  ngOnDestroy(): void {
    this.command.unregister(this);
  }
  protected select(): void {
    if (!this.disabled()) this.command.selectedItem.set(this);
  }
  protected onClick(): void {
    if (!this.disabled()) this.selected.emit();
  }
  protected baseClass() {
    return "data-[selected=true]:bg-accent data-[selected=true]:text-accent-foreground [&_svg:not([class*='text-'])]:text-muted-foreground relative flex cursor-default items-center gap-2 rounded-sm px-2 py-1.5 text-sm outline-hidden select-none data-[disabled=true]:pointer-events-none data-[disabled=true]:opacity-50 [&_svg]:pointer-events-none [&_svg]:shrink-0 [&_svg:not([class*='size-'])]:size-4";
  }
}

export const COMMAND = [Command, CommandInput, CommandList, CommandEmpty, CommandGroup, CommandItem] as const;
