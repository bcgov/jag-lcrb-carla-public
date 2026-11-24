/** Catalogue of applications to start; `?type=` scopes it to one licence type and `?q=` primes the search box (read once). */
import {
  afterNextRender,
  ChangeDetectionStrategy,
  Component,
  computed,
  ElementRef,
  inject,
  input,
  signal,
  viewChild,
} from '@angular/core';
import { NgTemplateOutlet } from '@angular/common';
import { toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { SuggestApplications } from '../components/suggest-applications';
import { BCGovFooter } from '@core/layout/footer';
import { BCGovHeader } from '@core/layout/header';
import { type HeroCrumb } from '@shared/ui/bc-gov/hero-breadcrumb';
import { PageHero } from '@shared/ui/bc-gov/page-hero';
import { ProcessingTimesBanner } from '../components/processing-times-banner';
import { Badge } from '@shared/ui/primitives/badge';
import { Button } from '@shared/ui/primitives/button';
import { CARD } from '@shared/ui/primitives/card';
import { Input } from '@shared/ui/primitives/input';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { LICENCE_TYPE_LABELS, LICENCE_TYPE_ORDER } from '@domain/models/applications';
import { LICENCE_TYPE_ACCENTS } from '@domain/applications/applications';
import { groupByTitle, heldLicenceCounts, heldLicenceTypes, isEntryEligible, type CatalogGroup } from '@domain/applications/application-eligibility';
import { injectBusinessProfile } from '@core/session/business-profile';
import { cn } from '@shared/util/utils';
import { APPLICATIONS_REPOSITORY } from '@data-access/repositories/applications/applications.repository';
import { LICENCES_REPOSITORY } from '@data-access/repositories/licences/licences.repository';
import { path } from '@/app/route-registry';
import type { CatalogEntry } from '@domain/models/applications';

/** A single application; when it exists for several licence types, one card carries a link per type. */
@Component({
  selector: 'catalog-card',
  imports: [NgTemplateOutlet, RouterLink, Badge, LucideIcon, ...CARD],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <ng-template #body>
      <div uiCard [class]="cardClass()">
        <div uiCardContent class="p-5 flex flex-col h-full">
          <div class="flex items-start justify-between gap-3 mb-3">
            <div [attr.class]="cn('p-2 rounded-lg', accent().chip)">
              <svg [lucide]="icon()" class="h-5 w-5" aria-hidden="true"></svg>
            </div>
            <span uiBadge variant="outline" class="text-xs">{{ group().kind === 'application' ? 'Application' : 'Authorization' }}</span>
          </div>
          <h3 class="font-semibold text-foreground text-balance">{{ group().title }}</h3>
          <p class="text-sm text-muted-foreground mt-1 flex-1">{{ group().description }}</p>

          @if (multi()) {
            <div class="mt-4 pt-3 border-t">
              <p class="text-xs font-medium uppercase tracking-wide text-muted-foreground">Choose a licence type</p>
              <div class="mt-2 flex flex-wrap gap-2">
                @for (variant of group().variants; track variant.slug) {
                  <a
                    [routerLink]="path('applications.flow', { slug: variant.slug })"
                    class="inline-flex items-center gap-1 rounded-full border px-3 py-1 text-xs font-medium text-[#013366] transition-colors hover:bg-[#013366] hover:text-white focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-[#013366]"
                  >
                    {{ variant.label }}
                    <svg lucide="ArrowRight" class="h-3 w-3" aria-hidden="true"></svg>
                  </a>
                }
              </div>
              @if (group().fee) {
                <p class="mt-3 text-sm font-medium text-[#013366]">Fee: {{ group().fee }}</p>
              }
            </div>
          } @else {
            <div class="flex items-center justify-between mt-4 pt-3 border-t">
              <span class="text-sm font-medium text-[#013366]">{{ group().fee ? 'Fee: ' + group().fee : 'No application fee' }}</span>
              <span class="inline-flex items-center gap-1 text-sm text-[#013366] group-hover:gap-2 transition-all">
                Start
                <svg lucide="ArrowRight" class="h-4 w-4" aria-hidden="true"></svg>
              </span>
            </div>
          }
        </div>
      </div>
    </ng-template>

    <!-- Multi-variant cards can't be one link: each licence type is its own flow. -->
    @if (multi()) {
      <div class="group"><ng-container [ngTemplateOutlet]="body" /></div>
    } @else {
      <a
        [routerLink]="path('applications.flow', { slug: primary().slug })"
        class="group rounded-lg focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-[#013366]"
      >
        <ng-container [ngTemplateOutlet]="body" />
      </a>
    }
  `,
})
export class CatalogCard {
  protected readonly path = path;
  readonly group = input.required<CatalogGroup>();

  protected readonly cn = cn;
  protected readonly icon = computed(() => (this.group().kind === 'application' ? 'FilePlus2' : 'FileCog'));
  protected readonly primary = computed(() => this.group().variants[0]);
  protected readonly accent = computed(() => LICENCE_TYPE_ACCENTS[this.primary().licenceType]);
  protected readonly multi = computed(() => this.group().variants.length > 1);
  protected readonly cardClass = computed(() => cn('h-full border-l-4 transition-shadow group-hover:shadow-md', this.accent().bar));
}

/** An application the business has no prerequisite for: shown, but not startable. */
@Component({
  selector: 'blocked-card',
  imports: [Badge, LucideIcon, ...CARD],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div uiCard class="h-full border-l-4 border-l-muted bg-muted/30">
      <div uiCardContent class="p-5 flex flex-col h-full">
        <div class="flex items-start justify-between gap-3 mb-3">
          <div class="p-2 rounded-lg bg-muted text-muted-foreground">
            <svg lucide="Lock" class="h-5 w-5" aria-hidden="true"></svg>
          </div>
          <span uiBadge variant="outline" class="text-xs text-muted-foreground">{{ group().kind === 'application' ? 'Application' : 'Authorization' }}</span>
        </div>
        <h3 class="font-semibold text-muted-foreground text-balance">{{ group().title }}</h3>
        <p class="text-sm text-muted-foreground mt-1 flex-1">{{ group().description }}</p>
        <p class="mt-4 pt-3 border-t text-sm text-muted-foreground">
          Requires an active&ngsp;<span class="font-medium">{{ required() }}</span>&ngsp;licence
        </p>
      </div>
    </div>
  `,
})
export class BlockedCard {
  readonly group = input.required<CatalogGroup>();
  protected readonly required = computed(() => this.group().variants.map((v) => v.label).join(' or '));
}

@Component({
  selector: 'applications-new-page',
  imports: [BCGovHeader, BCGovFooter, PageHero, ProcessingTimesBanner, SuggestApplications, CatalogCard, BlockedCard, Badge, Button, Input, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="min-h-screen flex flex-col">
      <bc-gov-header currentPage="/applications" />

      <main id="main-content" class="flex-1">
        <page-hero
          [breadcrumbs]="breadcrumbs"
          title="Start a New Application or Authorization"
          description="Applications available for the licences on your account, most relevant first."
        />

        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
          <!-- Search first: testers could not find a small input under two banners, so it is large, focused on load and clears in one click. -->
          <search role="search" class="mb-8">
            <label for="application-search" class="mb-2 block text-base font-semibold text-[#013366]">What do you need to do?</label>
            <div class="relative">
              <svg
                lucide="Search"
                class="pointer-events-none absolute left-4 top-1/2 h-5 w-5 -translate-y-1/2 text-muted-foreground"
                aria-hidden="true"
              ></svg>
              <input
                #searchInput
                uiInput
                id="application-search"
                type="search"
                placeholder="Describe it in your own words — e.g. add a patio, stay open later, sell my restaurant, renew…"
                [value]="query()"
                (input)="query.set($any($event.target).value)"
                class="h-14 rounded-xl border-2 border-[#013366]/30 bg-white pl-12 pr-28 text-base shadow-sm focus-visible:border-[#013366] focus-visible:ring-[#013366]/20 md:text-lg"
                aria-describedby="application-search-count"
              />
              @if (isSearching()) {
                <button
                  uiButton
                  type="button"
                  variant="ghost"
                  size="sm"
                  (click)="query.set('')"
                  class="absolute right-2 top-1/2 -translate-y-1/2 text-muted-foreground"
                >
                  Clear
                </button>
              }
            </div>
            <p id="application-search-count" class="mt-2 text-sm text-muted-foreground" aria-live="polite">{{ countText() }}</p>
          </search>

          <div class="mb-8">
            <processing-times-banner [focusType]="activeType() ?? undefined" />
          </div>

          <div class="mb-8">
            <suggest-applications [legalName]="profile().legalName" [licences]="heldLicences()" [availableApplications]="catalogItems()" />
          </div>

          @if (!isSearching() && mostUsed().length > 0) {
            <section class="mb-10" aria-labelledby="most-used-heading">
              <div class="mb-4 flex items-center gap-2">
                <svg lucide="TrendingUp" class="h-5 w-5 text-[#013366]" aria-hidden="true"></svg>
                <h2 id="most-used-heading" class="text-lg font-semibold text-[#013366]">Most used</h2>
                <span class="text-sm text-muted-foreground">The applications licensees start most often</span>
              </div>
              <div class="grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-3">
                @for (group of mostUsed(); track group.title) {
                  <catalog-card [group]="group" />
                }
              </div>
            </section>
          }

          @if (!isSearching()) {
            <div class="mb-4 flex items-center gap-2">
              <svg lucide="FileText" class="h-5 w-5 text-[#013366]" aria-hidden="true"></svg>
              <h2 class="text-lg font-semibold text-[#013366]">All applications and authorizations</h2>
              <span uiBadge variant="secondary" class="font-normal">{{ groups().length }}</span>
            </div>
          }

          @if (heldTypeLabels().length > 0 && !activeType()) {
            <p class="mb-6 text-sm text-muted-foreground">
              Showing applications for your&ngsp;<span class="font-medium text-foreground">{{ heldTypeLabels().join(', ') }}</span
              >&ngsp;{{ heldTypeLabels().length === 1 ? 'licence' : 'licences' }}, plus authorizations available to all licensees.
            </p>
          }

          @if (groups().length === 0) {
            <div class="text-center py-12 bg-white border rounded-lg">
              <svg lucide="FileText" class="h-12 w-12 mx-auto text-muted-foreground mb-4" aria-hidden="true"></svg>
              <p class="text-muted-foreground">{{ emptyText() }}</p>
            </div>
          } @else {
            <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
              @for (group of groups(); track group.title) {
                <catalog-card [group]="group" />
              }
            </div>
          }

          @if (blockedGroups().length > 0) {
            <section class="mt-10" aria-labelledby="unavailable-heading">
              <h2 id="unavailable-heading" class="mb-4">
                <button
                  type="button"
                  (click)="showUnavailable.set(!showUnavailable())"
                  [attr.aria-expanded]="showUnavailable()"
                  aria-controls="unavailable-panel"
                  class="flex w-full items-center gap-2 rounded-md py-1 text-left text-lg font-semibold text-[#013366] focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-[#013366]"
                >
                  <svg lucide="Lock" class="h-4 w-4 text-muted-foreground" aria-hidden="true"></svg>
                  Requires a licence you don't hold
                  <span uiBadge variant="secondary" class="ml-1 font-normal">{{ blockedGroups().length }}</span>
                  <svg
                    lucide="ChevronDown"
                    [class]="cn('ml-auto h-5 w-5 text-muted-foreground transition-transform', showUnavailable() ? 'rotate-0' : '-rotate-90')"
                    aria-hidden="true"
                  ></svg>
                </button>
              </h2>
              @if (showUnavailable()) {
                <div id="unavailable-panel">
                  <p class="mb-4 text-sm text-muted-foreground">
                    These applications can only be added to a licence type your business does not currently hold.
                  </p>
                  <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                    @for (group of blockedGroups(); track group.title) {
                      <blocked-card [group]="group" />
                    }
                  </div>
                </div>
              }
            </section>
          }
        </div>
      </main>

      <bc-gov-footer />
    </div>
  `,
})
export class ApplicationsNewPage {
  private readonly applicationsRepository = inject(APPLICATIONS_REPOSITORY);
  private readonly licencesRepository = inject(LICENCES_REPOSITORY);
  protected readonly breadcrumbs: HeroCrumb[] = [
    { label: 'Dashboard', href: path('dashboard.home') },
    { label: 'My Applications', href: path('applications.list') },
    { label: 'Start a new application' },
  ];
  protected readonly cn = cn;

  private readonly route = inject(ActivatedRoute);
  private readonly params = toSignal(this.route.queryParamMap, { initialValue: this.route.snapshot.queryParamMap });
  private readonly searchInput = viewChild<ElementRef<HTMLInputElement>>('searchInput');

  // Only honour a recognised licence type from the URL.
  protected readonly activeType = computed(() => {
    const typeParam = this.params().get('type');
    return LICENCE_TYPE_ORDER.find((t) => t === typeParam) ?? null;
  });
  // `?q=` lets other pages land here with the search primed (e.g. the licence detail page's "Add endorsement" button). Read once.
  protected readonly query = signal(this.route.snapshot.queryParamMap.get('q') ?? '');
  protected readonly showUnavailable = signal(false);
  protected readonly isSearching = computed(() => this.query().trim().length > 0);

  // Business context for the AI "Suggest applications" panel.
  protected readonly profile = injectBusinessProfile();
  protected readonly heldLicences = computed(() =>
    this.profile().licences.map((l) => {
      const full = this.licencesRepository.licence(l.number);
      return {
        type: l.type,
        name: l.establishmentName,
        status: full?.status ?? 'Active',
        expiryDate: full?.expiryDate ?? '',
        flags: full?.flags ?? [],
      };
    }),
  );

  private readonly held = computed(() => heldLicenceTypes(this.profile().licences));
  private readonly counts = computed(() => heldLicenceCounts(this.profile().licences));

  /* Default view shows only what this business can apply for, deduplicated by title and ordered by relevance (applications before authorizations, held licence types weighted up). */
  private readonly split = computed(() => {
    const activeType = this.activeType();
    const held = this.held();
    const scoped = activeType ? this.applicationsRepository.catalogue().filter((e) => e.licenceType === activeType) : this.applicationsRepository.catalogue();
    const eligible: CatalogEntry[] = [];
    const blocked: CatalogEntry[] = [];
    for (const entry of scoped) {
      if (isEntryEligible(entry, held)) eligible.push(entry);
      else blocked.push(entry);
    }
    return { eligible, blocked };
  });

  // The AI suggester is given only the eligible catalogue.
  protected readonly catalogItems = computed(() =>
    this.split().eligible.map((e) => ({
      slug: e.slug,
      title: e.title,
      description: e.description,
      licenceType: LICENCE_TYPE_LABELS[e.licenceType],
      kind: e.kind,
    })),
  );

  // Keyword-aware, ranked search.
  private matches(list: CatalogEntry[]) {
    return this.applicationsRepository.searchCatalogue(list, this.query());
  }
  private rank() {
    const query = this.query();
    return this.isSearching() ? (e: CatalogEntry) => this.applicationsRepository.scoreEntry(e, query) : undefined;
  }

  protected readonly groups = computed(() => groupByTitle(this.matches(this.split().eligible), this.counts(), this.rank()));
  protected readonly blockedGroups = computed(() => groupByTitle(this.matches(this.split().blocked), this.counts(), this.rank()));

  // The six most-started applications this business is eligible for, in LCRB volume order.
  protected readonly mostUsed = computed(() => {
    const all = groupByTitle(this.split().eligible, this.counts());
    const byTitle = new Map(all.map((g) => [g.title, g]));
    const picked: CatalogGroup[] = [];
    for (const title of this.applicationsRepository.mostUsedTitles()) {
      const g = byTitle.get(title);
      if (g) picked.push(g);
      if (picked.length === 6) break;
    }
    return picked;
  });

  // Licence types the business holds, for the context line under the title.
  protected readonly heldTypeLabels = computed(() =>
    LICENCE_TYPE_ORDER.filter((t) => t !== 'common' && this.held().has(t)).map((t) => LICENCE_TYPE_LABELS[t]),
  );

  protected readonly countText = computed(() => {
    const n = this.groups().length;
    const activeType = this.activeType();
    return this.isSearching()
      ? `${n} ${n === 1 ? 'match' : 'matches'} for “${this.query()}”`
      : `${n} available${activeType ? ` for ${LICENCE_TYPE_LABELS[activeType]}` : ''}`;
  });

  protected readonly emptyText = computed(() =>
    this.isSearching() ? `No available applications match “${this.query()}”.` : 'No applications are available for your current licences.',
  );

  constructor() {
    // React's autoFocus focuses on mount without writing the attribute.
    afterNextRender(() => this.searchInput()?.nativeElement.focus());
  }
}
