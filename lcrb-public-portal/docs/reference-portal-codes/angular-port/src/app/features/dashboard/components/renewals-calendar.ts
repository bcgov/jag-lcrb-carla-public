/** Compliance calendar (summary pills, type filter chips, agenda or month view, ICS export); EventRow sits on its own `div` because the parent uses `space-y-3`. */
import { ChangeDetectionStrategy, Component, computed, inject, input, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { injectExternalStore, injectHydrated } from '@shared/util/external-store';
import { ExportCalendarDialog } from './export-calendar-dialog';
import { MonthGrid } from './month-grid';
import { Badge } from '@shared/ui/primitives/badge';
import type { IconName } from '@shared/ui/primitives/icons';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { daysUntil, formatEnforcementDate } from '@domain/compliance/enforcement';
import { downloadICS } from '@shared/util/compliance-ics-download';
import { EVENT_META, URGENCY_META, groupByMonth, urgencyFor } from '@domain/compliance/compliance-calendar';
import { type CalendarEvent, type CalendarEventType } from '@domain/models/compliance';
import { COMPLIANCE_REPOSITORY } from '@data-access/repositories/compliance/compliance.repository';

const TYPE_ICON: Record<CalendarEventType, IconName> = {
  renewal: 'RefreshCw',
  fee: 'DollarSign',
  'enforcement-deadline': 'AlertTriangle',
  hearing: 'Gavel',
  inspection: 'ClipboardCheck',
};

const FILTERS: { value: CalendarEventType | 'all'; label: string }[] = [
  { value: 'all', label: 'All' },
  { value: 'renewal', label: 'Renewals' },
  { value: 'fee', label: 'Fees' },
  { value: 'inspection', label: 'Inspections' },
  { value: 'enforcement-deadline', label: 'Enforcement' },
  { value: 'hearing', label: 'Hearings' },
];

const SUMMARY_TONE: Record<string, { icon: string; value: string }> = {
  neutral: { icon: 'text-[#013366]', value: 'text-[#013366]' },
  info: { icon: 'text-blue-700', value: 'text-blue-800' },
  danger: { icon: 'text-red-700', value: 'text-red-800' },
};

@Component({
  selector: 'div[eventRow]',
  imports: [RouterLink, Badge, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { '[class]': 'hostClass()' },
  template: `
    <!-- Date block -->
    <div class="shrink-0 w-14 text-center" aria-hidden="true">
      <div class="text-xs font-medium uppercase text-muted-foreground">{{ monthShort() }}</div>
      <div class="text-2xl font-bold text-[#013366] leading-none">{{ dayOfMonth() }}</div>
    </div>

    <div class="flex-1 min-w-0">
      <div class="flex flex-wrap items-center gap-2">
        <span uiBadge variant="outline" [class]="meta().chip">
          <svg [lucide]="icon()" class="h-3 w-3 mr-1" aria-hidden="true"></svg>
          {{ meta().label }}
        </span>
        <span [class]="'text-xs font-medium ' + urgMeta().text">{{ urgencyText() }}</span>
      </div>
      <h3 class="font-semibold text-foreground mt-1.5 text-pretty">{{ event().title }}</h3>
      <p class="text-sm text-muted-foreground mt-0.5">{{ event().description }}</p>
      <div class="flex flex-wrap items-center gap-4 mt-2">
        <a [routerLink]="tree()" class="inline-flex items-center gap-1 text-sm font-medium text-[#013366] hover:underline">
          View details
          <svg lucide="ArrowRight" class="h-3.5 w-3.5" aria-hidden="true"></svg>
        </a>
        <button type="button" (click)="download()" class="inline-flex items-center gap-1 text-sm font-medium text-muted-foreground hover:text-foreground">
          <svg lucide="CalendarPlus" class="h-3.5 w-3.5" aria-hidden="true"></svg>
          Add to calendar
        </button>
      </div>
    </div>

    <div class="hidden sm:block shrink-0 text-right text-sm text-muted-foreground">{{ formatEnforcementDate(event().date) }}</div>
  `,
})
export class EventRow {
  readonly event = input.required<CalendarEvent>();
  private readonly router = inject(Router);

  protected readonly formatEnforcementDate = formatEnforcementDate;
  protected readonly meta = computed(() => EVENT_META[this.event().type]);
  protected readonly urgMeta = computed(() => URGENCY_META[urgencyFor(this.event().date)]);
  protected readonly icon = computed(() => TYPE_ICON[this.event().type]);
  private readonly days = computed(() => daysUntil(this.event().date));
  private readonly d = computed(() => new Date(this.event().date));
  protected readonly monthShort = computed(() => this.d().toLocaleDateString('en-CA', { month: 'short' }));
  protected readonly dayOfMonth = computed(() => this.d().getDate());
  protected readonly tree = computed(() => this.router.parseUrl(this.event().href));
  protected readonly hostClass = computed(() => `flex gap-4 rounded-lg border border-l-4 ${this.urgMeta().row} bg-white p-4`);
  protected readonly urgencyText = computed(() => {
    const days = this.days();
    const label = this.urgMeta().label;
    return days < 0
      ? `${label} · ${Math.abs(days)} day${Math.abs(days) === 1 ? '' : 's'} ago`
      : `${label} · in ${days} day${days === 1 ? '' : 's'}`;
  });

  protected download(): void {
    downloadICS([this.event()], `${this.event().id}.ics`);
  }
}

@Component({
  selector: 'summary-pill',
  imports: [LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="flex items-center gap-2">
      <svg [lucide]="icon()" [class]="'h-5 w-5 ' + styles().icon" aria-hidden="true"></svg>
      <span [class]="'text-xl font-bold ' + styles().value">{{ value() }}</span>
      <span class="text-muted-foreground">{{ label() }}</span>
    </div>
  `,
})
export class SummaryPill {
  readonly icon = input.required<IconName>();
  readonly label = input.required<string>();
  readonly value = input.required<number>();
  readonly tone = input.required<'neutral' | 'info' | 'danger'>();
  protected readonly styles = computed(() => SUMMARY_TONE[this.tone()]);
}

@Component({
  selector: 'renewals-calendar',
  imports: [LucideIcon, MonthGrid, ExportCalendarDialog, EventRow, SummaryPill],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (!hydrated()) {
      <div class="h-64 rounded-lg border bg-white animate-pulse" aria-busy="true" aria-label="Loading calendar"></div>
    } @else {
      <div class="space-y-6">
        <!-- Toolbar -->
        <div class="flex flex-col lg:flex-row lg:items-center lg:justify-between gap-4">
          <div class="flex flex-wrap items-center gap-4 text-sm">
            <summary-pill icon="CalendarDays" label="Total tracked" [value]="counts().total" tone="neutral" />
            <summary-pill icon="CalendarPlus" label="Next 90 days" [value]="counts().next90" tone="info" />
            <summary-pill icon="AlertTriangle" label="Overdue" [value]="counts().overdue" tone="danger" />
          </div>
          <export-calendar-dialog [events]="allEvents()" [defaultTypes]="defaultTypes()" />
        </div>

        <!-- Filter chips + view toggle -->
        <div class="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
          <div class="flex flex-wrap gap-2" role="group" aria-label="Filter events by type">
            @for (f of filters; track f.value) {
              <button
                type="button"
                (click)="filter.set(f.value)"
                [attr.aria-pressed]="filter() === f.value"
                [attr.class]="
                  'rounded-full border px-3 py-1 text-sm font-medium transition-colors ' +
                  (filter() === f.value ? 'bg-[#013366] text-white border-[#013366]' : 'bg-white text-foreground hover:bg-muted')
                "
              >
                {{ f.label }} ({{ countFor(f.value) }})
              </button>
            }
          </div>
          <div role="group" aria-label="Calendar view" class="inline-flex shrink-0 self-start rounded-md border bg-white p-0.5 sm:self-auto">
            <button
              type="button"
              (click)="view.set('agenda')"
              [attr.aria-pressed]="view() === 'agenda'"
              [attr.class]="
                'inline-flex items-center gap-1.5 rounded px-3 py-1.5 text-sm font-medium transition-colors ' +
                (view() === 'agenda' ? 'bg-[#013366] text-white' : 'text-foreground hover:bg-muted')
              "
            >
              <svg lucide="List" class="h-4 w-4" aria-hidden="true"></svg>
              Agenda
            </button>
            <button
              type="button"
              (click)="view.set('month')"
              [attr.aria-pressed]="view() === 'month'"
              [attr.class]="
                'inline-flex items-center gap-1.5 rounded px-3 py-1.5 text-sm font-medium transition-colors ' +
                (view() === 'month' ? 'bg-[#013366] text-white' : 'text-foreground hover:bg-muted')
              "
            >
              <svg lucide="LayoutGrid" class="h-4 w-4" aria-hidden="true"></svg>
              Month
            </button>
          </div>
        </div>

        <!-- Month grid -->
        @if (view() === 'month') {
          <month-grid [events]="filtered()" />
        }

        <!-- Agenda -->
        @if (view() !== 'month') {
          @if (groups().length === 0) {
            <div class="text-center py-12 bg-white border rounded-lg">
              <svg lucide="CalendarDays" class="h-12 w-12 mx-auto text-muted-foreground mb-3" aria-hidden="true"></svg>
              <p class="text-muted-foreground">No {{ filter() === 'all' ? '' : filter() + ' ' }}events to show.</p>
            </div>
          } @else {
            <div class="space-y-8">
              @for (group of groups(); track group.key) {
                <section [attr.aria-label]="group.label">
                  <h2 class="text-sm font-semibold uppercase tracking-wide text-muted-foreground mb-3">{{ group.label }}</h2>
                  <div class="space-y-3">
                    @for (event of group.events; track event.id) {
                      <div eventRow [event]="event"></div>
                    }
                  </div>
                </section>
              }
            </div>
          }
        }
      </div>
    }
  `,
})
export class RenewalsCalendar {
  private readonly complianceRepository = inject(COMPLIANCE_REPOSITORY);
  // React to enforcement store changes (deadlines/hearings can move).
  private readonly storeVersion = injectExternalStore(
    this.complianceRepository.enforcementCollection.subscribe,
    this.complianceRepository.enforcementCollection.snapshot,
    () => '{}',
  );
  protected readonly hydrated = injectHydrated();
  protected readonly filters = FILTERS;
  protected readonly filter = signal<CalendarEventType | 'all'>('all');
  // Agenda = chronological list (default, best for "what's next"); Month = a flippable grid for "what's happening in October".
  protected readonly view = signal<'agenda' | 'month'>('agenda');

  protected readonly allEvents = computed(() => (this.hydrated() ? this.complianceRepository.buildCalendarEvents() : []));
  protected readonly filtered = computed(() =>
    this.filter() === 'all' ? this.allEvents() : this.allEvents().filter((e) => e.type === this.filter()),
  );
  protected readonly groups = computed(() => groupByMonth(this.filtered()));
  protected readonly defaultTypes = computed(() =>
    this.filter() === 'all' ? (Object.keys(EVENT_META) as CalendarEventType[]) : [this.filter() as CalendarEventType],
  );

  protected readonly counts = computed(() => {
    const events = this.allEvents();
    const next90 = events.filter((e) => {
      const d = daysUntil(e.date);
      return d >= 0 && d <= 90;
    });
    const overdue = events.filter((e) => daysUntil(e.date) < 0).length;
    return { total: events.length, next90: next90.length, overdue };
  });

  protected countFor(value: CalendarEventType | 'all'): number {
    const events = this.allEvents();
    return value === 'all' ? events.length : events.filter((e) => e.type === value).length;
  }
}
