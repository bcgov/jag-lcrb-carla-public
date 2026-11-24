/** Flippable month view; each day with events is a popover trigger controlled by the selected day. */
import { NgTemplateOutlet } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, input, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import {
  addMonths,
  eachDayOfInterval,
  endOfMonth,
  endOfWeek,
  format,
  isSameDay,
  isSameMonth,
  isToday,
  parseISO,
  startOfMonth,
  startOfWeek,
} from 'date-fns';
import { Badge } from '@shared/ui/primitives/badge';
import { Button } from '@shared/ui/primitives/button';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { POPOVER } from '@shared/ui/primitives/popover';
import { downloadICS } from '@shared/util/compliance-ics-download';
import { EVENT_META, URGENCY_META, urgencyFor } from '@domain/compliance/compliance-calendar';
import { type CalendarEvent } from '@domain/models/compliance';
import { cn } from '@shared/util/utils';

const WEEKDAYS = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];

/** Solid dot colour per event type so the grid stays legible at a glance. */
const TYPE_DOT: Record<CalendarEvent['type'], string> = {
  renewal: 'bg-[#013366]',
  fee: 'bg-emerald-600',
  inspection: 'bg-sky-600',
  'enforcement-deadline': 'bg-[#D8292F]',
  hearing: 'bg-purple-600',
};

interface Cell {
  day: Date;
  key: string;
  dayEvents: CalendarEvent[];
  inMonth: boolean;
  today: boolean;
  overdue: boolean;
  isSelected: boolean;
  shown: CalendarEvent[];
  extra: number;
}

@Component({
  selector: 'month-grid',
  imports: [NgTemplateOutlet, RouterLink, Badge, Button, LucideIcon, ...POPOVER],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="rounded-lg border bg-white">
      <!-- Month header -->
      <div class="flex flex-wrap items-center justify-between gap-3 border-b px-4 py-3">
        <div class="flex items-center gap-1">
          <button uiButton variant="outline" size="icon" class="h-8 w-8" (click)="shift(-1)" aria-label="Previous month">
            <svg lucide="ChevronLeft" class="h-4 w-4"></svg>
          </button>
          <button uiButton variant="outline" size="icon" class="h-8 w-8" (click)="shift(1)" aria-label="Next month">
            <svg lucide="ChevronRight" class="h-4 w-4"></svg>
          </button>
          <h2 class="ml-2 text-lg font-semibold text-[#013366]" aria-live="polite">{{ monthLabel() }}</h2>
          <span class="ml-2 text-sm text-muted-foreground">{{ monthCount() }} {{ monthCount() === 1 ? 'item' : 'items' }}</span>
        </div>
        <div class="flex items-center gap-2">
          <button uiButton variant="ghost" size="sm" (click)="nextWithEvents(-1)">Previous with events</button>
          <button uiButton variant="ghost" size="sm" (click)="goToday()">Today</button>
          <button uiButton variant="ghost" size="sm" (click)="nextWithEvents(1)">Next with events</button>
        </div>
      </div>

      <!-- Weekday header -->
      <div class="grid grid-cols-7 border-b bg-muted/40 text-center text-xs font-medium uppercase tracking-wide text-muted-foreground">
        @for (d of weekdays; track d) {
          <div class="py-2">{{ d }}</div>
        }
      </div>

      <!-- Day cells -->
      <div role="grid" [attr.aria-label]="monthLabel() + ' compliance calendar'" class="grid grid-cols-7">
        @for (c of cells(); track c.key) {
          @if (c.dayEvents.length === 0 || !c.inMonth) {
            <div><ng-container [ngTemplateOutlet]="cellTpl" [ngTemplateOutletContext]="{ $implicit: c }" /></div>
          } @else {
            <ui-popover align="start" [state]="c.isSelected ? 'open' : 'closed'" (stateChanged)="onPopover($event, c.day)">
              <button
                uiPopoverTrigger
                class="block text-left focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-inset focus-visible:ring-[#013366]"
                [attr.aria-label]="dayLabel(c)"
              >
                <ng-container [ngTemplateOutlet]="cellTpl" [ngTemplateOutletContext]="{ $implicit: c }" />
              </button>
              <div uiPopoverContent *uiPopoverPortal class="w-96 p-0">
                <div class="flex items-center justify-between border-b px-4 py-2.5">
                  <h3 class="font-semibold text-[#013366]">{{ fullDate(c.day) }}</h3>
                  <button uiButton variant="ghost" size="icon" class="h-7 w-7" (click)="selected.set(null)" aria-label="Close">
                    <svg lucide="X" class="h-4 w-4"></svg>
                  </button>
                </div>
                <ul class="max-h-80 divide-y overflow-y-auto">
                  @for (e of c.dayEvents; track e.id) {
                    @let meta = eventMeta[e.type];
                    @let urg = urgencyMeta[urgencyFor(e.date)];
                    <li class="px-4 py-3">
                      <div class="flex flex-wrap items-center gap-2">
                        <span uiBadge variant="outline" [class]="meta.chip">{{ meta.label }}</span>
                        <span [attr.class]="cn('text-xs font-medium', urg.text)">{{ urg.label }}</span>
                      </div>
                      <p class="mt-1 text-sm font-medium text-foreground text-pretty">{{ e.title }}</p>
                      <div class="mt-2 flex items-center gap-4">
                        <a [routerLink]="tree(e.href)" class="inline-flex items-center gap-1 text-sm font-medium text-[#013366] hover:underline">
                          View details
                          <svg lucide="ArrowRight" class="h-3.5 w-3.5" aria-hidden="true"></svg>
                        </a>
                        <button type="button" (click)="download(e)" class="inline-flex items-center gap-1 text-sm text-muted-foreground hover:text-foreground">
                          <svg lucide="CalendarPlus" class="h-3.5 w-3.5" aria-hidden="true"></svg>
                          Add to calendar
                        </button>
                      </div>
                    </li>
                  }
                </ul>
              </div>
            </ui-popover>
          }
        }
      </div>

      <!-- Legend -->
      <div class="flex flex-wrap items-center gap-x-4 gap-y-1 border-t px-4 py-2 text-xs text-muted-foreground">
        @for (t of dotTypes; track t) {
          <span class="inline-flex items-center gap-1.5">
            <span [attr.class]="cn('h-2 w-2 rounded-full', typeDot[t])" aria-hidden="true"></span>
            {{ eventMeta[t].label }}
          </span>
        }
        <span class="inline-flex items-center gap-1.5">
          <span class="h-2 w-2 rounded-sm bg-red-100 ring-1 ring-red-300" aria-hidden="true"></span>
          Overdue
        </span>
      </div>
    </div>

    <ng-template #cellTpl let-c>
      <div role="gridcell" [attr.aria-selected]="c.isSelected" [attr.class]="cellClass(c)">
        <div class="flex items-center justify-between">
          <span [attr.class]="numberClass(c)">{{ dayNumber(c.day) }}</span>
          @if (c.dayEvents.length > 0) {
            <span class="sr-only">{{ c.dayEvents.length }} {{ c.dayEvents.length === 1 ? 'item' : 'items' }}</span>
          }
        </div>
        <div class="mt-1 flex flex-col gap-1">
          @for (e of c.shown; track e.id) {
            <span [attr.class]="chipClass(e)" [attr.title]="e.title">
              <span [attr.class]="dotClass(e)" aria-hidden="true"></span>
              <span class="truncate">{{ e.title }}</span>
            </span>
          }
          @if (c.extra > 0) {
            <span class="px-1 text-[11px] font-medium text-[#013366]">+{{ c.extra }} more</span>
          }
        </div>
      </div>
    </ng-template>
  `,
})
export class MonthGrid {
  readonly events = input.required<CalendarEvent[]>();
  readonly initialMonth = input<Date | undefined>(undefined);

  private readonly router = inject(Router);
  protected readonly weekdays = WEEKDAYS;
  protected readonly typeDot = TYPE_DOT;
  protected readonly dotTypes = Object.keys(TYPE_DOT) as CalendarEvent['type'][];
  protected readonly eventMeta = EVENT_META;
  protected readonly urgencyMeta = URGENCY_META;
  protected readonly urgencyFor = urgencyFor;
  protected readonly cn = cn;

  // The prototype's `useState(() => startOfMonth(initialMonth ?? new Date()))`: read once.
  private readonly monthState = signal<Date | undefined>(undefined);
  private initial?: Date;
  protected readonly month = computed(() => this.monthState() ?? (this.initial ??= startOfMonth(this.initialMonth() ?? new Date())));
  protected readonly selected = signal<Date | null>(null);

  protected readonly monthLabel = computed(() => format(this.month(), 'MMMM yyyy'));

  private readonly byDay = computed(() => {
    const map = new Map<string, CalendarEvent[]>();
    for (const e of this.events()) {
      const key = e.date.slice(0, 10);
      const list = map.get(key) ?? [];
      list.push(e);
      map.set(key, list);
    }
    return map;
  });

  private readonly days = computed(() =>
    eachDayOfInterval({
      start: startOfWeek(startOfMonth(this.month())),
      end: endOfWeek(endOfMonth(this.month())),
    }),
  );

  protected readonly monthCount = computed(
    () => this.events().filter((e) => isSameMonth(parseISO(e.date), this.month())).length,
  );

  protected readonly cells = computed((): Cell[] => {
    const month = this.month();
    const selected = this.selected();
    return this.days().map((day) => {
      const key = format(day, 'yyyy-MM-dd');
      const dayEvents = this.byDay().get(key) ?? [];
      const shown = dayEvents.slice(0, 2);
      return {
        day,
        key,
        dayEvents,
        inMonth: isSameMonth(day, month),
        today: isToday(day),
        overdue: dayEvents.some((e) => urgencyFor(e.date) === 'overdue'),
        isSelected: Boolean(selected && isSameDay(day, selected)),
        shown,
        extra: dayEvents.length - shown.length,
      };
    });
  });

  protected shift(by: number): void {
    this.monthState.set(addMonths(this.month(), by));
  }

  protected goToday(): void {
    this.monthState.set(startOfMonth(new Date()));
  }

  // Jump to the nearest month that actually has something in it.
  protected nextWithEvents(dir: 1 | -1): void {
    const month = this.month();
    const sorted = [...this.events()].map((e) => parseISO(e.date)).sort((a, b) => a.getTime() - b.getTime());
    const target =
      dir === 1 ? sorted.find((d) => d > endOfMonth(month)) : [...sorted].reverse().find((d) => d < startOfMonth(month));
    if (target) this.monthState.set(startOfMonth(target));
  }

  /** Radix onOpenChange(o) sets the selected day, or clears it. */
  protected onPopover(state: string, day: Date): void {
    if (state === 'open') this.selected.set(day);
    else if (this.selected() && isSameDay(day, this.selected()!)) this.selected.set(null);
  }

  protected cellClass(c: Cell): string {
    return cn(
      'min-h-24 border-b border-r p-1.5 text-left align-top transition-colors',
      !c.inMonth && 'bg-muted/30 text-muted-foreground',
      c.inMonth && c.dayEvents.length > 0 && 'cursor-pointer hover:bg-[#013366]/5',
      c.overdue && c.inMonth && 'bg-red-50/70',
      c.isSelected && 'ring-2 ring-inset ring-[#013366]',
    );
  }

  protected numberClass(c: Cell): string {
    return cn(
      'inline-flex h-6 w-6 items-center justify-center rounded-full text-sm',
      c.today && 'bg-[#013366] font-semibold text-white',
      !c.today && c.inMonth && 'font-medium text-foreground',
    );
  }

  protected chipClass(e: CalendarEvent): string {
    return cn(
      'flex items-center gap-1 truncate rounded px-1 py-0.5 text-[11px] leading-tight',
      urgencyFor(e.date) === 'overdue' ? 'bg-red-100 text-red-900' : 'bg-slate-100 text-slate-800',
    );
  }

  protected dotClass(e: CalendarEvent): string {
    return cn('h-1.5 w-1.5 shrink-0 rounded-full', TYPE_DOT[e.type]);
  }

  protected dayNumber(day: Date): string {
    return format(day, 'd');
  }

  protected fullDate(day: Date): string {
    return format(day, 'EEEE, MMMM d, yyyy');
  }

  protected dayLabel(c: Cell): string {
    return `${format(c.day, 'EEEE, MMMM d')}: ${c.dayEvents.length} ${c.dayEvents.length === 1 ? 'item' : 'items'}`;
  }

  /** Parsed so a `?query` in the href renders as-is (a string routerLink would encode the `?`). */
  protected tree(href: string) {
    return this.router.parseUrl(href);
  }

  protected download(e: CalendarEvent): void {
    downloadICS([e], `${e.id}.ics`);
  }
}
