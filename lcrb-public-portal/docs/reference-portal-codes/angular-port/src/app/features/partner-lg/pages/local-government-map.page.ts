/** Local Government regional map: client-only, so it loads lazily behind the "Loading map…" fallback (ssr:false becomes @defer). `AuthProvider` is provided here. */
import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { BCGovFooter } from '@core/layout/footer';
import { BCGovHeader } from '@core/layout/header';
import { AuthProvider } from '@shared/ui/map/map-auth';
import { MapExperience } from '@shared/ui/map/map-experience';
import { CARD } from '@shared/ui/primitives/card';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { formatLgDate } from '@domain/lg-review/due-dates';
import { RISK_COLOR } from '@domain/map/map-markers';
import { formatEventDate } from '@domain/special-events/seps';
import { SPECIAL_EVENTS_REPOSITORY } from '@data-access/repositories/special-events/special-events.repository';
import { LG_REVIEW_REPOSITORY } from '@data-access/repositories/lg-review/lg-review.repository';
import { path } from '@/app/route-registry';

@Component({
  selector: 'local-government-map-page',
  imports: [RouterLink, BCGovHeader, BCGovFooter, MapExperience, LucideIcon, ...CARD],
  providers: [AuthProvider],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="flex min-h-screen flex-col bg-background">
      <bc-gov-header role="local-government" currentPage="/local-government/map" />

      <main id="main-content" class="flex-1">
        <div class="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
          <div class="mb-6">
            <h1 class="text-2xl font-semibold text-foreground md:text-3xl">Regional Map</h1>
            <p class="mt-2 text-muted-foreground">
              Explore the full licensing &amp; events map for {{ reviewerRole }} — toggle overlays, search and filter licences, follow locations of interest, and open licence popups — then jump into the applications and events under review listed below.
            </p>
          </div>

          <!-- Full role-aware map experience: overlays, search, filters, following queue, popups -->
          <div class="mb-8 overflow-hidden rounded-lg border border-border shadow-sm">
            @defer (on immediate) {
              <map-experience heightClassName="h-[640px]" />
            } @loading {
              <div class="flex h-[640px] w-full items-center justify-center rounded-lg border border-border bg-muted/30 text-sm text-muted-foreground">Loading map…</div>
            } @placeholder {
              <div class="flex h-[640px] w-full items-center justify-center rounded-lg border border-border bg-muted/30 text-sm text-muted-foreground">Loading map…</div>
            }
          </div>

          <div class="grid gap-8 lg:grid-cols-2">
            <!-- Establishments -->
            <section>
              <h2 class="mb-4 flex items-center gap-2 text-xl font-semibold text-foreground">
                <svg lucide="Building2" class="h-5 w-5 text-[#013366]"></svg>
                Establishments ({{ applications.length }})
              </h2>
              <div class="space-y-3">
                @for (a of applications; track a.jobNumber) {
                  <div uiCard class="transition-shadow hover:shadow-md">
                    <div uiCardContent class="p-4">
                      <div class="flex items-start justify-between gap-2">
                        <a [routerLink]="path('lg.review', { id: a.jobNumber })" class="font-semibold text-[#013366] hover:underline">{{ a.establishmentName }}</a>
                        <span class="flex items-center gap-1 text-xs font-medium" [style.color]="riskColor[a.riskLevel]">
                          <svg lucide="ShieldAlert" class="h-3.5 w-3.5"></svg>
                          {{ a.riskLevel }} risk
                        </span>
                      </div>
                      <div class="mt-2 grid grid-cols-1 gap-1.5 text-sm text-muted-foreground sm:grid-cols-2">
                        <span class="flex items-center gap-1.5">
                          <svg lucide="MapPin" class="h-4 w-4 shrink-0"></svg>
                          {{ a.city }}
                        </span>
                        <span class="truncate">{{ a.licenceType }}</span>
                        <span class="truncate">{{ a.applicationType }}</span>
                        <span class="flex items-center gap-1.5">
                          <svg lucide="CalendarClock" class="h-4 w-4 shrink-0"></svg>
                          Due {{ formatLgDate(a.commentDueDate) }}
                        </span>
                      </div>
                      <a
                        [routerLink]="path('lg.review', { id: a.jobNumber })"
                        class="mt-3 inline-flex items-center gap-1 text-sm font-medium text-[#013366] hover:underline"
                      >
                        Open application
                        <svg lucide="ArrowRight" class="h-4 w-4"></svg>
                      </a>
                    </div>
                  </div>
                }
              </div>
            </section>

            <!-- Special events -->
            <section>
              <h2 class="mb-4 flex items-center gap-2 text-xl font-semibold text-foreground">
                <svg lucide="CalendarClock" class="h-5 w-5 text-[#013366]"></svg>
                Special events ({{ localSeps().length }})
              </h2>
              <div class="space-y-3">
                @for (r of localSeps(); track r.id) {
                  <div uiCard class="transition-shadow hover:shadow-md">
                    <div uiCardContent class="p-4">
                      <div class="font-semibold text-[#013366]">{{ r.eventName }}</div>
                      <div class="mt-2 grid grid-cols-1 gap-1.5 text-sm text-muted-foreground sm:grid-cols-2">
                        <span class="flex items-center gap-1.5">
                          <svg lucide="CalendarClock" class="h-4 w-4 shrink-0"></svg>
                          {{ formatEventDate(r) }}
                        </span>
                        <span class="flex items-center gap-1.5">
                          <svg lucide="MapPin" class="h-4 w-4 shrink-0"></svg>
                          {{ r.venueName }}, {{ r.city }}
                        </span>
                      </div>
                    </div>
                  </div>
                }
                @if (localSeps().length === 0) {
                  <div uiCard>
                    <div uiCardContent class="py-10 text-center text-muted-foreground">No special events in your jurisdiction right now.</div>
                  </div>
                }
              </div>
            </section>
          </div>
        </div>
      </main>

      <bc-gov-footer />
    </div>
  `,
})
export class LocalGovernmentMapPage {
  protected readonly path = path;
  private readonly specialEventsRepository = inject(SPECIAL_EVENTS_REPOSITORY);
  private readonly lgReviewRepository = inject(LG_REVIEW_REPOSITORY);
  private readonly records = this.specialEventsRepository.store().records;

  protected readonly applications = this.lgReviewRepository.applications();
  protected readonly reviewerRole = this.lgReviewRepository.reviewer().role;
  protected readonly riskColor = RISK_COLOR;
  protected readonly formatLgDate = formatLgDate;
  protected readonly formatEventDate = formatEventDate;

  // The jurisdiction's cities drive what shows in the lists below. LG applications are already scoped to the jurisdiction; SEP events are filtered to those cities.
  private readonly jurisdictionCities = new Set(this.lgReviewRepository.applications().map((a) => a.city.toLowerCase()));
  protected readonly localSeps = computed(() => this.records().filter((r) => this.jurisdictionCities.has(r.city.toLowerCase())));
}
