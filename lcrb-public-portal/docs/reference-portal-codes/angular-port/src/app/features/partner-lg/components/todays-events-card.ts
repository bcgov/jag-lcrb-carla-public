/** Awareness card: how many Special Event Permits are scheduled today in the reviewer's jurisdiction. Reads the live SEP store and scopes to the jurisdiction's cities, like the regional map. */
import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CARD } from '@shared/ui/primitives/card';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { formatTimeOfDay, isAutoApproved, sepOccursOnDate, todayIso } from '@domain/special-events/seps';
import { SPECIAL_EVENTS_REPOSITORY } from '@data-access/repositories/special-events/special-events.repository';
import { LG_REVIEW_REPOSITORY } from '@data-access/repositories/lg-review/lg-review.repository';
import { path } from '@/app/route-registry';

@Component({
  selector: 'todays-events-card',
  imports: [RouterLink, LucideIcon, CARD],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div uiCard class="border-l-4 border-l-[#013366] overflow-hidden">
      <div uiCardContent class="p-0">
        <div class="flex flex-col gap-4 p-5 sm:flex-row sm:items-center sm:justify-between">
          <div class="flex items-center gap-4">
            <div class="rounded-lg bg-[#013366]/10 p-3 shrink-0">
              <svg lucide="PartyPopper" class="h-6 w-6 text-[#013366]" aria-hidden="true"></svg>
            </div>
            <div>
              <div class="flex items-baseline gap-2">
                <span class="text-3xl font-semibold text-foreground tabular-nums">{{ count() }}</span>
                <span class="text-base font-medium text-foreground">special event{{ count() === 1 ? '' : 's' }} today</span>
              </div>
              <p class="text-sm text-muted-foreground mt-0.5">Scheduled in {{ place }} for {{ dateLabel }}</p>
            </div>
          </div>
          <a [routerLink]="path('lg.map')" class="inline-flex items-center gap-1 text-sm font-medium text-[#1A5A96] hover:underline shrink-0">
            View on map
            <svg lucide="ArrowRight" class="h-4 w-4"></svg>
          </a>
        </div>

        @if (count() > 0) {
          <ul class="divide-y border-t">
            @for (event of events(); track event.id) {
              <li>
                <div class="flex flex-wrap items-center gap-x-6 gap-y-1.5 px-5 py-3">
                  <div class="min-w-0 flex-1">
                    <div class="font-medium text-[#013366] truncate">{{ event.eventName }}</div>
                    <div class="text-sm text-muted-foreground truncate">{{ event.eventType }}</div>
                  </div>
                  <div class="flex items-center gap-1.5 text-sm text-muted-foreground">
                    <svg lucide="Clock" class="h-4 w-4 shrink-0" aria-hidden="true"></svg>
                    {{ formatTimeOfDay(event.startTime) }}&ndash;{{ formatTimeOfDay(event.endTime) }}
                  </div>
                  <div class="flex items-center gap-1.5 text-sm text-muted-foreground min-w-0 max-w-[16rem]">
                    <svg lucide="MapPin" class="h-4 w-4 shrink-0" aria-hidden="true"></svg>
                    <span class="truncate">{{ event.venueName }}</span>
                  </div>
                  <div class="flex items-center gap-1.5 text-sm text-muted-foreground">
                    <svg lucide="Users" class="h-4 w-4 shrink-0" aria-hidden="true"></svg>
                    {{ event.expectedAttendance.toLocaleString() }}
                  </div>
                  <span
                    [attr.class]="
                      'rounded-full border px-2 py-0.5 text-xs font-medium ' +
                      (isAutoApproved(event) ? 'bg-slate-100 text-slate-700 border-slate-200' : 'bg-green-100 text-green-800 border-green-200')
                    "
                  >
                    {{ isAutoApproved(event) ? 'Auto-approved' : 'Approved' }}
                  </span>
                </div>
              </li>
            }
          </ul>
        }

        @if (count() === 0) {
          <div class="border-t px-5 py-4 text-sm text-muted-foreground flex items-center gap-2">
            <svg lucide="CalendarDays" class="h-4 w-4 shrink-0" aria-hidden="true"></svg>
            No special events are scheduled in {{ place }} today.
          </div>
        }
      </div>
    </div>
  `,
})
export class TodaysEventsCard {
  protected readonly path = path;
  private readonly specialEventsRepository = inject(SPECIAL_EVENTS_REPOSITORY);
  private readonly lgReviewRepository = inject(LG_REVIEW_REPOSITORY);
  private readonly records = this.specialEventsRepository.store().records;

  // Cities that belong to this reviewer's jurisdiction.
  private readonly jurisdictionCities = new Set(this.lgReviewRepository.applications().map((a) => a.city.toLowerCase()));

  private readonly today = todayIso();
  // Filter the LIVE store so session changes (submissions, cancellations) are reflected. Exclude cancelled/denied events: they are not taking place.
  protected readonly events = computed(() =>
    this.records()
      .filter(
        (r) =>
          r.status !== 'cancelled' &&
          r.status !== 'denied' &&
          this.jurisdictionCities.has(r.city.toLowerCase()) &&
          sepOccursOnDate(r, this.today),
      )
      .sort((a, b) => a.startTime.localeCompare(b.startTime)),
  );
  protected readonly count = computed(() => this.events().length);

  // Derive a friendly jurisdiction label ("City of Victoria" -> "Victoria").
  protected readonly place = this.lgReviewRepository.reviewer().role.replace(/^City of\s+/i, '');
  protected readonly dateLabel = new Date(this.today + 'T00:00:00').toLocaleDateString('en-CA', {
    weekday: 'long',
    month: 'long',
    day: 'numeric',
  });

  protected readonly formatTimeOfDay = formatTimeOfDay;
  protected readonly isAutoApproved = isAutoApproved;
}
