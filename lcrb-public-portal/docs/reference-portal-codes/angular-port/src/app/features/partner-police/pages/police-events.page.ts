/** Upcoming Special Event Permits: embedded map, filters, one-day PDF and scheduled reports. The map's ssr:false becomes `@defer`; `AuthProvider` is provided here. */
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { BCGovFooter } from '@core/layout/footer';
import { BCGovHeader } from '@core/layout/header';
import { PAGE_HERO } from '@shared/ui/bc-gov/page-hero';
import { SepStatusBadge } from '@shared/ui/bc-gov/sep-status-badge';
import { AuthProvider } from '@shared/ui/map/map-auth';
import { MapExperience } from '@shared/ui/map/map-experience';
import { ScheduledReportsCard } from '../components/scheduled-reports-card';
import { Button } from '@shared/ui/primitives/button';
import { CARD } from '@shared/ui/primitives/card';
import { Input } from '@shared/ui/primitives/input';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { SELECT } from '@shared/ui/primitives/select';
import { distinctCities } from '@domain/special-events/seps';
import { formatEventDate, SEP_STATUS_LABELS, sepOccursOnDate } from '@domain/special-events/seps';
import { type SepRecord, type SepStatus } from '@domain/models/special-events';
import { generateSepDailyReportPdf } from '@shared/util/pdf/sep-daily-report';
import { SPECIAL_EVENTS_REPOSITORY } from '@data-access/repositories/special-events/special-events.repository';
import { path } from '@/app/route-registry';

@Component({
  selector: 'police-events-page',
  imports: [
    RouterLink,
    BCGovHeader,
    BCGovFooter,
    ...PAGE_HERO,
    ...CARD,
    ...SELECT,
    Button,
    Input,
    LucideIcon,
    SepStatusBadge,
    MapExperience,
    ScheduledReportsCard,
  ],
  providers: [AuthProvider],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="flex min-h-screen flex-col bg-background">
      <bc-gov-header role="police" currentPage="/police/events" />

      <main id="main-content" class="flex-1">
        <page-hero
          [breadcrumbs]="[{ label: 'Special Event Permit Review', href: path('police.home') }, { label: 'Upcoming Events' }]"
          title="Upcoming Events"
          description="Explore the full licensing & events map — toggle overlays, apply filters, and follow locations of interest — then review upcoming Special Event Permits listed by date below."
        />

        <div class="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
          <!-- Full police map experience: overlays, filters, following queue, popups -->
          <div class="mb-8 overflow-hidden rounded-lg border border-border shadow-sm">
            @defer (on immediate) {
              <map-experience heightClassName="h-[640px]" />
            } @loading {
              <div class="flex h-[640px] w-full items-center justify-center rounded-lg border border-border bg-muted/30 text-sm text-muted-foreground">
                Loading map…
              </div>
            } @placeholder {
              <div class="flex h-[640px] w-full items-center justify-center rounded-lg border border-border bg-muted/30 text-sm text-muted-foreground">
                Loading map…
              </div>
            }
          </div>

          <!-- List filters -->
          <div class="mb-4 flex flex-col gap-3 lg:flex-row">
            <div class="relative flex-1">
              <svg lucide="Search" class="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground"></svg>
              <input
                uiInput
                placeholder="Search events, applicants, venues, jurisdictions…"
                [value]="query()"
                (input)="query.set($any($event.target).value)"
                class="pl-10"
              />
            </div>
            <ui-select [value]="cityFilter()" (valueChange)="cityFilter.set($event ?? '')">
              <button uiSelectTrigger class="w-full lg:w-[200px]"><span uiSelectValue placeholder="City"></span></button>
              <ng-template uiSelectContent>
                <ui-select-item value="all">All cities</ui-select-item>
                @for (c of cities(); track c) {
                  <ui-select-item [value]="c">{{ c }}</ui-select-item>
                }
              </ng-template>
            </ui-select>
            <ui-select [value]="statusFilter()" (valueChange)="statusFilter.set($event ?? '')">
              <button uiSelectTrigger class="w-full lg:w-[200px]"><span uiSelectValue placeholder="Status"></span></button>
              <ng-template uiSelectContent>
                <ui-select-item value="all">All statuses</ui-select-item>
                @for (s of statuses; track s) {
                  <ui-select-item [value]="s">{{ statusLabels[s] }}</ui-select-item>
                }
              </ng-template>
            </ui-select>
          </div>

          <!-- Date-range filter -->
          <div class="mb-6 flex flex-col gap-3 sm:flex-row sm:items-end">
            <div class="flex flex-col gap-1.5">
              <label for="from-date" class="text-xs font-medium text-muted-foreground">From date</label>
              <input
                uiInput
                id="from-date"
                type="date"
                [attr.min]="todayIso"
                [attr.max]="toDate() || null"
                [value]="fromDate()"
                (input)="fromDate.set($any($event.target).value)"
                class="w-full sm:w-[180px]"
              />
            </div>
            <div class="flex flex-col gap-1.5">
              <label for="to-date" class="text-xs font-medium text-muted-foreground">To date</label>
              <input
                uiInput
                id="to-date"
                type="date"
                [attr.min]="fromDate() || todayIso"
                [value]="toDate()"
                (input)="toDate.set($any($event.target).value)"
                class="w-full sm:w-[180px]"
              />
            </div>
            @if (rangeActive()) {
              <button
                type="button"
                (click)="clearDates()"
                class="inline-flex items-center gap-1 self-start text-sm font-medium text-[#013366] hover:underline sm:self-auto sm:pb-2.5"
              >
                <svg lucide="X" class="h-4 w-4"></svg>
                Clear dates
              </button>
            }
          </div>

          <!-- One-day PDF report -->
          <div uiCard class="mb-6 border-[#013366]/20 bg-[#013366]/[0.03]">
            <div uiCardContent class="flex flex-col gap-3 p-4 sm:flex-row sm:items-end sm:justify-between">
              <div class="flex flex-col gap-3 sm:flex-row sm:items-end">
                <div class="flex flex-col gap-1.5">
                  <label for="report-date" class="text-xs font-medium text-foreground">Daily permit report (PDF)</label>
                  <input
                    uiInput
                    id="report-date"
                    type="date"
                    [value]="reportDate()"
                    (input)="reportDate.set($any($event.target).value)"
                    class="w-full bg-background sm:w-[180px]"
                  />
                </div>
                <p class="text-xs text-muted-foreground sm:pb-2.5">{{ reportSummary() }}</p>
              </div>
              <button
                uiButton
                type="button"
                (click)="handleDownloadPdf()"
                [disabled]="!reportDate() || generating()"
                class="bg-[#013366] text-white hover:bg-[#012a52]"
              >
                @if (generating()) {
                  <svg lucide="Loader2" class="h-4 w-4 animate-spin"></svg>
                } @else {
                  <svg lucide="FileDown" class="h-4 w-4"></svg>
                }
                {{ generating() ? 'Preparing…' : 'Download PDF' }}
              </button>
            </div>
          </div>

          <!-- Automatic PDF report delivery (daily / weekly email) -->
          <scheduled-reports-card [records]="records()" [filters]="scheduleFilters()" />

          <p class="mb-4 text-sm text-muted-foreground">
            {{ upcoming().length }} upcoming event{{ upcoming().length === 1 ? '' : 's' }}{{ rangeActive() ? ' in selected range' : '' }}
          </p>

          <!-- List -->
          <div class="grid gap-3 md:grid-cols-2">
            @for (sep of upcoming(); track sep.id) {
              <div uiCard class="transition-shadow hover:shadow-md">
                <div uiCardContent class="p-4">
                  <div class="flex items-start justify-between gap-2">
                    <a [routerLink]="path('police.review', { id: sep.id })" class="font-semibold text-[#013366] hover:underline">{{ sep.eventName }}</a>
                    <sep-status-badge [status]="sep.status" />
                  </div>
                  <div class="mt-2 grid grid-cols-1 gap-1.5 text-sm text-muted-foreground sm:grid-cols-2">
                    <span class="flex items-center gap-1.5">
                      <svg lucide="CalendarClock" class="h-4 w-4 shrink-0"></svg>
                      {{ eventDate(sep) }}
                    </span>
                    <span class="flex items-center gap-1.5">
                      <svg lucide="MapPin" class="h-4 w-4 shrink-0"></svg>
                      {{ sep.city }}
                    </span>
                    <span class="flex items-center gap-1.5">
                      <svg lucide="Users" class="h-4 w-4 shrink-0"></svg>
                      {{ sep.expectedAttendance.toLocaleString() }} expected
                    </span>
                    <span class="truncate">{{ sep.jurisdiction }}</span>
                  </div>
                  <a
                    [routerLink]="path('police.review', { id: sep.id })"
                    class="mt-3 inline-flex items-center gap-1 text-sm font-medium text-[#013366] hover:underline"
                  >
                    Open review
                    <svg lucide="ArrowRight" class="h-4 w-4"></svg>
                  </a>
                </div>
              </div>
            }
            @if (upcoming().length === 0) {
              <div uiCard class="md:col-span-2">
                <div uiCardContent class="py-12 text-center text-muted-foreground">No upcoming events match your search.</div>
              </div>
            }
          </div>
        </div>
      </main>

      <bc-gov-footer />
    </div>
  `,
})
export class PoliceEventsPage {
  protected readonly path = path;
  private readonly specialEventsRepository = inject(SPECIAL_EVENTS_REPOSITORY);
  private readonly store = this.specialEventsRepository.store();
  protected readonly records = this.store.records;

  protected readonly statuses = Object.keys(SEP_STATUS_LABELS) as SepStatus[];
  protected readonly statusLabels = SEP_STATUS_LABELS;

  protected readonly query = signal('');
  protected readonly statusFilter = signal('all');
  protected readonly cityFilter = signal('all');
  protected readonly fromDate = signal('');
  protected readonly toDate = signal('');
  protected readonly reportDate = signal('');
  protected readonly generating = signal(false);

  protected readonly todayIso = new Date().toISOString().slice(0, 10);
  // The date range only makes sense within the upcoming window, so clamp the "from" bound to today at the earliest.
  private readonly effectiveFrom = computed(() => (this.fromDate() && this.fromDate() > this.todayIso ? this.fromDate() : this.todayIso));

  // Cities present across all records, for the city filter dropdown.
  protected readonly cities = computed(() => distinctCities(this.records()));

  // Applies the non-date filters (status, city, search) shared by both the on-screen list and the daily PDF report.
  private readonly matchesFilters = computed(() => {
    const statusFilter = this.statusFilter();
    const cityFilter = this.cityFilter();
    const q = this.query().trim().toLowerCase();
    return (r: SepRecord) => {
      if (statusFilter !== 'all' && r.status !== (statusFilter as SepStatus)) return false;
      if (cityFilter !== 'all' && r.city !== cityFilter) return false;
      if (q) {
        return [r.eventName, r.applicantName, r.venueName, r.city, r.jurisdiction].join(' ').toLowerCase().includes(q);
      }
      return true;
    };
  });

  protected readonly upcoming = computed(() => {
    const effectiveFrom = this.effectiveFrom();
    const toDate = this.toDate();
    let list = this.records().filter((r) => r.eventStart >= effectiveFrom);
    if (toDate) {
      // Compare on the event's start day so an event on the "to" date is included.
      list = list.filter((r) => r.eventStart.slice(0, 10) <= toDate);
    }
    list = list.filter(this.matchesFilters());
    return list.sort((a, b) => a.eventStart.localeCompare(b.eventStart));
  });

  protected readonly rangeActive = computed(() => Boolean(this.fromDate() || this.toDate()));

  // SEPs active on the chosen report day, honouring the active filters.
  private readonly reportRecords = computed(() => {
    const reportDate = this.reportDate();
    if (!reportDate) return [];
    const matches = this.matchesFilters();
    return this.records()
      .filter((r) => sepOccursOnDate(r, reportDate) && matches(r))
      .sort((a, b) => a.startTime.localeCompare(b.startTime));
  });

  protected readonly reportSummary = computed(() => {
    if (!this.reportDate()) return 'Pick a day to export all its Special Event Permits.';
    const n = this.reportRecords().length;
    return (
      `${n} permit${n === 1 ? '' : 's'} on this day` +
      (this.cityFilter() !== 'all' || this.statusFilter() !== 'all' || this.query().trim() ? ' (matching filters)' : '')
    );
  });

  protected readonly scheduleFilters = computed(() => ({
    city: this.cityFilter(),
    status: this.statusFilter() as SepStatus | 'all',
    query: this.query(),
  }));

  protected eventDate(sep: SepRecord): string {
    return formatEventDate(sep);
  }

  protected clearDates(): void {
    this.fromDate.set('');
    this.toDate.set('');
  }

  protected async handleDownloadPdf(): Promise<void> {
    if (!this.reportDate() || this.generating()) return;
    const filterBits: string[] = [];
    filterBits.push(this.cityFilter() === 'all' ? 'All cities' : this.cityFilter());
    filterBits.push(this.statusFilter() === 'all' ? 'All statuses' : SEP_STATUS_LABELS[this.statusFilter() as SepStatus]);
    if (this.query().trim()) filterBits.push(`Search: "${this.query().trim()}"`);
    this.generating.set(true);
    try {
      await generateSepDailyReportPdf({
        reportDate: this.reportDate(),
        records: this.reportRecords(),
        filterSummary: filterBits.join('  ·  '),
      });
    } finally {
      this.generating.set(false);
    }
  }
}
