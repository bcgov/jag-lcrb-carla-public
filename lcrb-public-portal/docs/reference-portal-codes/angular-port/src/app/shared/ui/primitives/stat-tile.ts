/** Stat tile that is always an action: a link with `href`, else a button (aria-pressed as a filter toggle). Testers kept clicking the tiles. */
import { booleanAttribute, ChangeDetectionStrategy, Component, computed, input, output } from '@angular/core';
import { RouterLink } from '@angular/router';
import { NgTemplateOutlet } from '@angular/common';
import { cn } from '@shared/util/utils';
import type { IconName } from './icons';
import { LucideIcon } from './lucide-icon';

@Component({
  selector: 'ui-stat-tile',
  imports: [RouterLink, NgTemplateOutlet, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <ng-template #body>
      <div class="flex items-center gap-3">
        <div [class]="toneClass()">
          <svg [lucide]="icon()" class="h-5 w-5" aria-hidden="true"></svg>
        </div>
        <div class="min-w-0 flex-1 text-left">
          <p class="text-2xl font-bold leading-none text-foreground">{{ value() }}</p>
          <p class="mt-1 truncate text-sm text-muted-foreground">{{ label() }}</p>
        </div>
        <svg
          lucide="ChevronRight"
          class="h-4 w-4 shrink-0 text-muted-foreground/60 transition-transform group-hover:translate-x-0.5 group-hover:text-[#013366]"
          aria-hidden="true"
        ></svg>
      </div>
      <span class="sr-only">. {{ hint() }}</span>
    </ng-template>
    @if (href(); as link) {
      <a [routerLink]="link" [class]="classes()"><ng-container [ngTemplateOutlet]="body" /></a>
    } @else {
      <button type="button" (click)="tileClick.emit()" [attr.aria-pressed]="active()" [class]="classes()">
        <ng-container [ngTemplateOutlet]="body" />
      </button>
    }
  `,
})
export class StatTile {
  readonly label = input.required<string>();
  readonly value = input.required<string | number>();
  readonly icon = input.required<IconName>();
  /** Tailwind classes for the icon badge, e.g. "bg-blue-100 text-blue-600". */
  readonly tone = input('bg-slate-100 text-slate-600');
  readonly href = input<string | undefined>(undefined);
  /** When the tile acts as a filter toggle, whether it's the active one. */
  readonly active = input(false, { transform: booleanAttribute });
  /** Short verb shown under the label on hover/focus, e.g. "View list". */
  readonly hint = input('View');
  readonly userClass = input<string | undefined>(undefined, { alias: 'class' });
  readonly tileClick = output<void>();

  protected readonly toneClass = computed(() => cn('rounded-lg p-2', this.tone()));
  protected readonly classes = computed(() =>
    cn(
      'group block w-full rounded-lg border bg-card p-4 text-card-foreground shadow-sm transition-all',
      'hover:-translate-y-0.5 hover:border-[#013366]/40 hover:shadow-md',
      'focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[#013366] focus-visible:ring-offset-2',
      this.active() && 'border-[#013366] ring-1 ring-[#013366]/30',
      this.userClass(),
    ),
  );
}

/** A row of StatTiles with a heading that names them as a summary. */
@Component({
  selector: 'ui-stat-tile-row',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `<section [attr.aria-label]="title()" [class]="sectionClass()">
    <div class="mb-3 flex items-baseline gap-2">
      <h2 class="text-sm font-semibold uppercase tracking-wide text-[#013366]">{{ title() }}</h2>
      <p class="text-sm text-muted-foreground">{{ description() }}</p>
    </div>
    <div class="grid grid-cols-2 gap-4 md:grid-cols-4"><ng-content /></div>
  </section>`,
})
export class StatTileRow {
  readonly title = input('At a glance');
  readonly description = input('Select a tile to filter the list below.');
  readonly userClass = input<string | undefined>(undefined, { alias: 'class' });
  protected readonly sectionClass = computed(() => cn('mb-6', this.userClass()));
}
