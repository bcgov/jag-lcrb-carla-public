/** Prior permits from the same applicant or venue, each linking to its review. `Row` is `li[relatedPermitRow]` because the parent `ul` styles its children. */
import { ChangeDetectionStrategy, Component, computed, inject, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { SepStatusBadge } from '@shared/ui/bc-gov/sep-status-badge';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { formatEventDate } from '@domain/special-events/seps';
import { type SepRecord } from '@domain/models/special-events';
import { SPECIAL_EVENTS_REPOSITORY } from '@data-access/repositories/special-events/special-events.repository';
import { path } from '@/app/route-registry';

function key(s: string | undefined): string {
  return (s ?? '').trim().toLowerCase();
}

@Component({
  selector: 'li[relatedPermitRow]',
  imports: [RouterLink, SepStatusBadge, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { class: 'flex flex-wrap items-center justify-between gap-3 py-3' },
  template: `
    <div class="min-w-0">
      <a
        [routerLink]="path('police.review', { id: r().id })"
        class="inline-flex items-center gap-1 font-medium text-[#013366] underline-offset-2 hover:underline"
      >
        {{ r().eventName }}
        <svg lucide="ArrowUpRight" class="h-3.5 w-3.5"></svg>
      </a>
      <p class="flex flex-wrap items-center gap-x-3 text-xs text-muted-foreground">
        <span class="inline-flex items-center gap-1">
          <svg lucide="CalendarDays" class="h-3 w-3"></svg>
          {{ eventDate() }}
        </span>
        <span class="inline-flex items-center gap-1">
          <svg lucide="MapPin" class="h-3 w-3"></svg>
          {{ r().venueName }}, {{ r().city }}
        </span>
        <span>{{ r().expectedAttendance.toLocaleString() }} expected</span>
        @if (r().assignedOfficer) {
          <span>Reviewed by {{ r().assignedOfficer }}</span>
        }
      </p>
    </div>
    <sep-status-badge [status]="r().status" />
  `,
})
export class RelatedPermitRow {
  protected readonly path = path;
  readonly r = input.required<SepRecord>();
  protected readonly eventDate = computed(() => formatEventDate(this.r()));
}

@Component({
  selector: 'related-permits',
  imports: [RelatedPermitRow, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <section class="rounded-lg border border-border bg-card p-6">
      <h2 class="text-lg font-semibold text-foreground">Permit history</h2>
      <p class="mt-1 text-sm text-muted-foreground">
        Earlier permits linked to this applicant and this location, across all officers.
      </p>

      <h3 class="mt-5 flex items-center gap-2 text-sm font-semibold text-foreground">
        <svg lucide="User" class="h-4 w-4 text-[#013366]"></svg>
        Same applicant{{ current().applicantOrg ? ' / ' + current().applicantOrg : '' }}
      </h3>
      @if (related().byApplicant.length === 0) {
        <p class="mt-2 text-sm text-muted-foreground">First permit from this applicant.</p>
      } @else {
        <ul class="mt-1 divide-y divide-border">
          @for (r of related().byApplicant; track r.id) {
            <li relatedPermitRow [r]="r"></li>
          }
        </ul>
      }

      <h3 class="mt-5 flex items-center gap-2 text-sm font-semibold text-foreground">
        <svg lucide="MapPin" class="h-4 w-4 text-[#013366]"></svg>
        Same venue — {{ current().venueName }}
      </h3>
      @if (related().byVenue.length === 0) {
        <p class="mt-2 text-sm text-muted-foreground">No other permits on record at this venue.</p>
      } @else {
        <ul class="mt-1 divide-y divide-border">
          @for (r of related().byVenue; track r.id) {
            <li relatedPermitRow [r]="r"></li>
          }
        </ul>
      }
    </section>
  `,
})
export class RelatedPermits {
  private readonly specialEventsRepository = inject(SPECIAL_EVENTS_REPOSITORY);
  readonly current = input.required<SepRecord>();
  private readonly store = this.specialEventsRepository.store();

  protected readonly related = computed(() => {
    const current = this.current();
    const others = this.store.records().filter((r) => r.id !== current.id);
    const applicant = key(current.applicantName);
    const org = key(current.applicantOrg);
    const venue = key(current.venueName);
    const address = key(current.address);
    const byApplicant = others
      .filter((r) => key(r.applicantName) === applicant || (org && key(r.applicantOrg) === org))
      .sort((a, b) => b.eventStart.localeCompare(a.eventStart));
    const byVenue = others
      .filter((r) => !byApplicant.includes(r))
      .filter((r) => key(r.venueName) === venue || (address && key(r.address) === address))
      .sort((a, b) => b.eventStart.localeCompare(a.eventStart));
    return { byApplicant, byVenue };
  });
}
