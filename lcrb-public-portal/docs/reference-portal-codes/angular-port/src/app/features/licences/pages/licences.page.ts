/** Licences list (cap card, overdue banner, search and filters, owned / operated tabs); LicenceCard and EmptyState sit on the real element so the `space-y-4` list keeps its direct children. */
import { ChangeDetectionStrategy, Component, computed, inject, input, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { LicenceSuggestedActions, type AvailableAction } from '../components/licence-suggested-actions';
import { BCGovFooter } from '@core/layout/footer';
import { BCGovHeader } from '@core/layout/header';
import { PAGE_HERO } from '@shared/ui/bc-gov/page-hero';
import { LicenceCapCard } from '../components/licence-cap-card';
import { Badge } from '@shared/ui/primitives/badge';
import { Button } from '@shared/ui/primitives/button';
import { DROPDOWN_MENU } from '@shared/ui/primitives/dropdown-menu';
import { Input } from '@shared/ui/primitives/input';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { SELECT } from '@shared/ui/primitives/select';
import { TABS } from '@shared/ui/primitives/tabs';
import { type LicenceType } from '@domain/models/applications';
import { formatInspectionDateTime } from '@domain/licences/inspections';
import { daysUntilExpiry, isOverdueForRenewal } from '@domain/licences/licences';
import { licenceToRollup } from '@shared/util/pdf/pdf-rollup';
import { type Licence } from '@domain/models/licences';
import { generateApplicationPdf } from '@shared/util/pdf/pdf-rollup';
import type { IconName } from '@shared/ui/primitives/icons';
import { LICENCES_REPOSITORY } from '@data-access/repositories/licences/licences.repository';
import { path } from '@/app/route-registry';
import { APPLICATIONS_REPOSITORY, type ApplicationsRepository } from '@data-access/repositories/applications/applications.repository';

// Maps a human licence type label to the catalog's LicenceType key
const LICENCE_TYPE_TO_KEY: Record<string, LicenceType> = {
  'Liquor Primary': 'liquor-primary',
  'Food Primary': 'food-primary',
  'Cannabis Retail Store': 'cannabis-retail-store',
  'Licensee Retail Store': 'licensee-retail-store',
  Manufacturer: 'manufacturer',
  'Deemed Manufacturer': 'deemed-manufacturer',
};

// Authorizations available to a licence = its type-specific entries + common ones
function getAuthorizationsForLicence(licenceType: string, applications: ApplicationsRepository) {
  const key = LICENCE_TYPE_TO_KEY[licenceType];
  const typeSpecific = key ? applications.entriesByType(key) : [];
  const common = applications.entriesByType('common');
  return { typeSpecific, common };
}

const STATUS_CONFIG: Record<string, { color: string; icon: IconName }> = {
  Active: { color: 'bg-green-100 text-green-800 border-green-200', icon: 'CheckCircle' },
  'Pending Renewal': { color: 'bg-yellow-100 text-yellow-800 border-yellow-200', icon: 'Clock' },
  Expired: { color: 'bg-red-100 text-red-800 border-red-200', icon: 'AlertTriangle' },
};

@Component({
  selector: 'div[licenceCard]',
  imports: [RouterLink, Button, Badge, LucideIcon, ...DROPDOWN_MENU, LicenceSuggestedActions],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { '[class]': 'cardClass()' },
  template: `
    <!-- Card Header -->
    <div [class]="(overdue() ? 'bg-[#D8292F]' : 'bg-[#38598A]') + ' text-white px-4 py-3 flex items-center justify-between'">
      <div class="flex items-center gap-3">
        <span class="font-medium">{{ licence().type }}</span>
        <span class="text-white/70">|</span>
        <span class="text-white/90">Licence #{{ licence().id }}</span>
        @if (overdue()) {
          <span class="inline-flex items-center gap-1 rounded-full bg-white px-2.5 py-0.5 text-xs font-semibold text-[#D8292F]">
            <svg lucide="AlertTriangle" class="h-3 w-3" aria-hidden="true"></svg>
            Renewal overdue
          </span>
        }
      </div>

      <!-- Actions dropdown -->
      <button uiButton variant="ghost" size="sm" class="text-white hover:bg-white/20 gap-2" [uiDropdownMenuTrigger]="actionsMenu" align="end">
        Actions
        <svg lucide="ChevronDown" class="h-4 w-4"></svg>
      </button>
      <ng-template #actionsMenu>
        <div uiDropdownMenuContent class="w-64">
          <a uiDropdownMenuItem [routerLink]="tree(applyHref('licence-renewal'))" class="gap-2 cursor-pointer">
            <svg lucide="RefreshCw" class="h-4 w-4"></svg>
            Renew Licence
          </a>
          <a uiDropdownMenuItem [routerLink]="tree(path('licences.detail', { id: licence().id }) + '?tab=terms')" class="gap-2 cursor-pointer">
            <svg lucide="FileText" class="h-4 w-4"></svg>
            View Terms &amp; Conditions
          </a>
          <div uiDropdownMenuItem (triggered)="handleDownload()" class="gap-2 cursor-pointer">
            <svg lucide="Download" class="h-4 w-4"></svg>
            Download Licence Copy
          </div>

          @if (auths().typeSpecific.length > 0) {
            <div uiDropdownMenuSeparator></div>
            <div uiDropdownMenuLabel class="text-xs text-muted-foreground">{{ licence().type }} authorizations</div>
            @for (entry of auths().typeSpecific; track entry.slug) {
              <a uiDropdownMenuItem [routerLink]="tree(applyHref(entry.slug))" class="gap-2 cursor-pointer">
                <svg lucide="FilePlus2" class="h-4 w-4"></svg>
                {{ entry.title }}
              </a>
            }
          }

          @if (!isOperated()) {
            <div uiDropdownMenuSeparator></div>
            <div uiDropdownMenuLabel class="text-xs text-muted-foreground">Common requests</div>
            @for (entry of commonRequests(); track entry.slug) {
              <a uiDropdownMenuItem [routerLink]="tree(applyHref(entry.slug))" class="gap-2 cursor-pointer">
                <svg lucide="Edit" class="h-4 w-4"></svg>
                {{ entry.title }}
              </a>
            }
          }
        </div>
      </ng-template>
    </div>

    <!-- Card Body -->
    <div class="p-4">
      <div class="flex flex-col lg:flex-row lg:items-start gap-4">
        <!-- Main Info -->
        <div class="flex-1">
          <div class="flex items-start justify-between mb-3">
            <div>
              <h3 class="text-lg font-semibold text-[#013366]">
                <a [routerLink]="path('licences.detail', { id: licence().id })" class="hover:underline">{{ licence().name }}</a>
              </h3>
              <div class="flex items-center gap-2 mt-1">
                <span uiBadge variant="outline" [class]="status().color">
                  <svg [lucide]="status().icon" class="h-3 w-3 mr-1"></svg>
                  {{ licence().status }}
                </span>
                @for (flag of licence().flags; track flag) {
                  <span uiBadge variant="outline" [class]="flagClass(flag)">{{ flag }}</span>
                }
              </div>
            </div>
          </div>

          <!-- Details Grid -->
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4 text-sm">
            <div>
              <div class="text-muted-foreground mb-1">Location</div>
              <div class="flex items-start gap-2">
                <svg lucide="MapPin" class="h-4 w-4 text-muted-foreground mt-0.5 shrink-0"></svg>
                <span>{{ licence().address }}</span>
              </div>
            </div>
            <div>
              <div class="text-muted-foreground mb-1">Contact</div>
              <div class="space-y-1">
                <div class="flex items-center gap-2">
                  <svg lucide="Phone" class="h-4 w-4 text-muted-foreground"></svg>
                  <span>{{ licence().phone }}</span>
                </div>
                <div class="flex items-center gap-2">
                  <svg lucide="Mail" class="h-4 w-4 text-muted-foreground"></svg>
                  <span>{{ licence().email }}</span>
                </div>
              </div>
            </div>
            <div>
              <div class="text-muted-foreground mb-1">Expiry / Renewal Date</div>
              <div [class]="'font-medium ' + (overdue() ? 'text-[#D8292F]' : expiringSoon() ? 'text-orange-700' : '')">
                {{ expiryLabel() }}
                @if (overdue()) {
                  <span class="block text-xs font-semibold">
                    Expired {{ absDays() }} {{ absDays() === 1 ? 'day' : 'days' }} ago — renew now
                  </span>
                }
                @if (expiringSoon()) {
                  <span class="block text-xs font-normal text-orange-700">
                    Expires in {{ daysLeft() }} {{ daysLeft() === 1 ? 'day' : 'days' }}
                  </span>
                }
              </div>
            </div>
            <div>
              <div class="text-muted-foreground mb-1">Operated By</div>
              <div class="text-[#255A90] font-medium">{{ licence().operatedBy }}</div>
              @if (licence().licencedTo) {
                <div class="text-sm text-muted-foreground">Licenced To: {{ licence().licencedTo }}</div>
              }
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- AI: Suggest next actions -->
    <div class="px-4 pb-4">
      <licence-suggested-actions
        [licenceType]="licence().type"
        [licenceName]="licence().name"
        [status]="licence().status"
        [expiryDate]="licence().expiryDate"
        [flags]="licence().flags"
        [isOperated]="isOperated()"
        [hasScheduledInspection]="!!nextInspection()"
        [nextInspectionDate]="nextInspection() ? inspectionDate() : undefined"
        [availableActions]="availableActions()"
        [actionHref]="actionHref"
      />
    </div>

    <!-- Next scheduled inspection -->
    @if (nextInspection()) {
      <a
        [routerLink]="tree(path('licences.detail', { id: licence().id }) + '?tab=inspections')"
        class="flex items-center gap-2 px-4 py-2.5 border-t bg-blue-50/60 text-sm hover:bg-blue-50 transition-colors"
      >
        <svg lucide="CalendarClock" class="h-4 w-4 text-[#013366] shrink-0" aria-hidden="true"></svg>
        <span class="text-[#013366]">
          <span class="font-medium">Inspection scheduled</span>
          {{ ' — ' }}
          {{ inspectionDate() }}
        </span>
        <span class="ml-auto text-xs text-[#255A90] font-medium">View</span>
      </a>
    }

    <!-- Card Footer - Quick Actions -->
    <div class="bg-muted/30 px-4 py-3 border-t flex flex-wrap gap-2">
      <a uiButton variant="outline" size="sm" class="gap-2" [routerLink]="tree(path('licences.detail', { id: licence().id }) + '?tab=terms')">
        <svg lucide="FileText" class="h-4 w-4"></svg>
        Terms &amp; Conditions
      </a>
      <button uiButton variant="outline" size="sm" class="gap-2" (click)="handleDownload()">
        <svg lucide="Download" class="h-4 w-4"></svg>
        Download Copy
      </button>
      @if (isOperated()) {
        <div class="ml-auto text-sm text-muted-foreground italic">3rd-Party Operators cannot request changes to the licence</div>
      }
    </div>
  `,
})
export class LicenceCard {
  protected readonly path = path;
  private readonly applicationsRepository = inject(APPLICATIONS_REPOSITORY);
  private readonly licencesRepository = inject(LICENCES_REPOSITORY);
  private readonly router = inject(Router);
  readonly licence = input.required<Licence>();
  readonly isOperated = input(false);

  protected readonly overdue = computed(() => isOverdueForRenewal(this.licence()));
  protected readonly daysLeft = computed(() => daysUntilExpiry(this.licence()));
  protected readonly absDays = computed(() => Math.abs(this.daysLeft()));
  protected readonly expiringSoon = computed(() => !this.overdue() && this.daysLeft() <= 90);
  // Overdue trumps whatever the record status says.
  protected readonly status = computed(() =>
    this.overdue()
      ? { color: 'bg-red-100 text-red-800 border-red-200', icon: 'AlertTriangle' as IconName }
      : (STATUS_CONFIG[this.licence().status] ?? STATUS_CONFIG['Active']),
  );
  protected readonly cardClass = computed(
    () =>
      `bg-white rounded-lg overflow-hidden hover:shadow-md transition-shadow ${
        this.overdue() ? 'border-2 border-red-500 shadow-[0_0_0_4px_rgba(216,41,47,0.08)]' : 'border'
      }`,
  );
  protected readonly auths = computed(() => getAuthorizationsForLicence(this.licence().type, this.applicationsRepository));
  // The dedicated "Renew Licence" action covers renewals, so keep the generic renewal flow out of "Common requests".
  protected readonly commonRequests = computed(() => this.auths().common.filter((entry) => entry.slug !== 'licence-renewal'));
  protected readonly nextInspection = computed(() => this.licencesRepository.nextInspection(this.licence().id));
  protected readonly inspectionDate = computed(() => {
    const next = this.nextInspection();
    return next ? formatInspectionDateTime(next.scheduledDate) : '';
  });
  protected readonly expiryLabel = computed(() =>
    new Date(`${this.licence().expiryDate}T00:00:00`).toLocaleDateString('en-CA', { year: 'numeric', month: 'long', day: 'numeric' }),
  );

  /** Prefill carried into any application started from this licence's Actions menu. */
  private readonly prefillQuery = computed(() => {
    const licence = this.licence();
    return new URLSearchParams({
      licenceNumber: licence.id,
      establishmentName: licence.name,
      legalEntityName: licence.licencedTo || licence.operatedBy || licence.name,
    }).toString();
  });
  protected readonly actionHref = (slug: string) => `${path('applications.flow', { slug: slug })}?${this.prefillQuery()}`;
  protected applyHref(slug: string) {
    return this.actionHref(slug);
  }

  /** Real request types the AI may recommend; third-party operators get none. */
  protected readonly availableActions = computed<AvailableAction[]>(() =>
    this.isOperated()
      ? []
      : [
          { slug: 'licence-renewal', title: 'Renew Licence', description: 'Renew this licence before it expires to keep it active.' },
          ...this.auths().typeSpecific.map((entry) => ({ slug: entry.slug, title: entry.title, description: entry.description })),
          ...this.commonRequests().map((entry) => ({ slug: entry.slug, title: entry.title, description: entry.description })),
        ],
  );

  protected flagClass(flag: string) {
    return /overdue/i.test(flag) ? 'bg-red-50 text-red-800 border-red-300' : 'bg-orange-50 text-orange-700 border-orange-200';
  }
  /** Parsed so a `?query` in the href renders as-is. */
  protected tree(href: string) {
    return this.router.parseUrl(href);
  }
  protected handleDownload() {
    generateApplicationPdf(licenceToRollup(this.licence()));
  }
}

@Component({
  selector: 'div[licenceEmptyState]',
  imports: [LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { class: 'text-center py-12 bg-white border rounded-lg' },
  template: `
    <svg lucide="FileText" class="h-12 w-12 mx-auto text-muted-foreground mb-4"></svg>
    <p class="text-muted-foreground">{{ message() }}</p>
  `,
})
export class LicenceEmptyState {
  readonly message = input.required<string>();
}

@Component({
  selector: 'licences-page',
  imports: [
    RouterLink,
    BCGovHeader,
    BCGovFooter,
    ...PAGE_HERO,
    ...SELECT,
    ...TABS,
    Button,
    Input,
    LucideIcon,
    LicenceCapCard,
    LicenceCard,
    LicenceEmptyState,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="min-h-screen flex flex-col bg-background">
      <bc-gov-header currentPage="/licences" />

      <main class="flex-1">
        <page-hero
          [breadcrumbs]="breadcrumbs"
          title="Licences & Authorizations"
          description="View and manage all your liquor and cannabis licences"
        >
          <a heroActions uiButton class="gap-2 bg-bc-gold font-semibold text-bc-blue hover:bg-bc-gold/90" [routerLink]="path('applications.new')">
            <svg lucide="FilePlus2" class="h-4 w-4"></svg>
            Start a new application
          </a>
        </page-hero>

        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
          <!-- Licence cap indicator: collapsed accordion; header carries the summary -->
          <div class="mb-6">
            <licence-cap-card />
          </div>

          <!-- Overdue renewals: surfaced above the list so they can't be missed -->
          @if (overdue.length > 0) {
            <div
              role="alert"
              class="mb-6 flex flex-wrap items-center gap-3 rounded-lg border border-red-300 bg-red-50 px-4 py-3 text-sm text-red-900"
            >
              <svg lucide="AlertTriangle" class="h-5 w-5 shrink-0 text-red-700" aria-hidden="true"></svg>
              <p class="flex-1">
                <span class="font-semibold">
                  {{ overdue.length === 1 ? '1 licence is' : overdue.length + ' licences are' }} overdue for renewal.
                </span>{{ ' ' }}
                {{ overdueNames }} expired and must be renewed to keep operating.
              </p>
              <button
                uiButton
                size="sm"
                variant="outline"
                class="border-red-300 bg-white text-red-800 hover:bg-red-100"
                (click)="statusFilter.set('overdue')"
              >
                Show overdue only
              </button>
            </div>
          }

          <!-- Search first, large, with the filters beside it -->
          <search role="search" class="mb-6">
            <label for="licence-search" class="mb-2 block text-base font-semibold text-[#013366]">Find a licence</label>
            <div class="flex flex-col gap-3 lg:flex-row">
              <div class="relative flex-1">
                <svg
                  lucide="Search"
                  class="pointer-events-none absolute left-4 top-1/2 h-5 w-5 -translate-y-1/2 text-muted-foreground"
                  aria-hidden="true"
                ></svg>
                <input
                  uiInput
                  id="licence-search"
                  type="search"
                  placeholder="Search by establishment, licence number, address or type…"
                  [value]="searchQuery()"
                  (input)="searchQuery.set($any($event.target).value)"
                  class="h-12 rounded-lg border-2 border-[#013366]/30 bg-white pl-12 pr-24 text-base shadow-sm focus-visible:border-[#013366] focus-visible:ring-[#013366]/20"
                  aria-describedby="licence-search-count"
                />
                @if (searchQuery()) {
                  <button
                    uiButton
                    type="button"
                    variant="ghost"
                    size="sm"
                    (click)="searchQuery.set('')"
                    class="absolute right-2 top-1/2 -translate-y-1/2 text-muted-foreground"
                  >
                    Clear
                  </button>
                }
              </div>
              <div class="flex gap-2">
                <ui-select [value]="statusFilter()" (valueChange)="statusFilter.set($event ?? '')">
                  <button uiSelectTrigger class="h-12 w-[160px]" aria-label="Filter by status">
                    <svg lucide="Filter" class="mr-2 h-4 w-4" aria-hidden="true"></svg>
                    <span uiSelectValue placeholder="Status"></span>
                  </button>
                  <ng-template uiSelectContent>
                    <ui-select-item value="all">All statuses</ui-select-item>
                    <ui-select-item value="active">Active</ui-select-item>
                    <ui-select-item value="pending">Pending renewal</ui-select-item>
                    <ui-select-item value="overdue">Overdue for renewal</ui-select-item>
                    <ui-select-item value="expired">Expired</ui-select-item>
                  </ng-template>
                </ui-select>
                <ui-select [value]="typeFilter()" (valueChange)="typeFilter.set($event ?? '')">
                  <button uiSelectTrigger class="h-12 w-[190px]" aria-label="Filter by licence type">
                    <span uiSelectValue placeholder="Type"></span>
                  </button>
                  <ng-template uiSelectContent>
                    <ui-select-item value="all">All types</ui-select-item>
                    @for (t of licenceTypes; track t) {
                      <ui-select-item [value]="t">{{ t }}</ui-select-item>
                    }
                  </ng-template>
                </ui-select>
              </div>
            </div>
            <p id="licence-search-count" class="mt-2 text-sm text-muted-foreground" aria-live="polite">
              {{ countText() }}
            </p>
          </search>

          <!-- Tabs for Owned vs Operated -->
          <div uiTabs value="owned" class="w-full">
            <div uiTabsList class="mb-6">
              <button uiTabsTrigger value="owned" class="gap-2">
                <svg lucide="Building2" class="h-4 w-4"></svg>
                My Licences ({{ owned.length }})
              </button>
              <button uiTabsTrigger value="operated" class="gap-2">
                <svg lucide="FileText" class="h-4 w-4"></svg>
                Operated Licences ({{ operated.length }})
              </button>
            </div>

            <div uiTabsContent value="owned">
              <div class="space-y-4">
                @for (licence of filteredOwned(); track licence.id) {
                  <div licenceCard [licence]="licence"></div>
                }
                @if (filteredOwned().length === 0) {
                  <div licenceEmptyState message="No licences found matching your filters"></div>
                }
              </div>
            </div>

            <div uiTabsContent value="operated">
              <div class="space-y-4">
                @for (licence of filteredOperated(); track licence.id) {
                  <div licenceCard [licence]="licence" [isOperated]="true"></div>
                }
                @if (filteredOperated().length === 0) {
                  <div licenceEmptyState message="No operated licences found"></div>
                }
              </div>
            </div>
          </div>
        </div>
      </main>

      <bc-gov-footer />
    </div>
  `,
})
export class LicencesPage {
  protected readonly path = path;
  private readonly licencesRepository = inject(LICENCES_REPOSITORY);
  protected readonly breadcrumbs = [{ label: 'Dashboard', href: path('dashboard.home') }, { label: 'Licences & Authorizations' }];
  protected readonly searchQuery = signal('');
  protected readonly statusFilter = signal('all');
  protected readonly typeFilter = signal('all');

  protected readonly owned = this.licencesRepository.licences();
  protected readonly operated = this.licencesRepository.operatedLicences();
  // Licences past their expiry date without a completed renewal.
  protected readonly overdue = this.licencesRepository.licences().filter((l) => isOverdueForRenewal(l));
  protected readonly overdueNames = this.overdue.map((l) => l.name).join(', ');
  // Type filter options come from the licences actually held.
  protected readonly licenceTypes = Array.from(new Set([...this.licencesRepository.licences(), ...this.licencesRepository.operatedLicences()].map((l) => l.type))).sort();

  private filterLicences(licences: Licence[]) {
    const q = this.searchQuery().trim().toLowerCase();
    const statusFilter = this.statusFilter();
    const typeFilter = this.typeFilter();
    return licences.filter((licence) => {
      const matchesSearch =
        !q ||
        licence.name.toLowerCase().includes(q) ||
        licence.id.includes(q) ||
        licence.address.toLowerCase().includes(q) ||
        licence.type.toLowerCase().includes(q) ||
        licence.operatedBy.toLowerCase().includes(q);

      const matchesStatus =
        statusFilter === 'all' ||
        (statusFilter === 'overdue' ? isOverdueForRenewal(licence) : licence.status.toLowerCase().includes(statusFilter.toLowerCase()));
      const matchesType = typeFilter === 'all' || licence.type === typeFilter;

      return matchesSearch && matchesStatus && matchesType;
    });
  }
  protected readonly filteredOwned = computed(() => this.filterLicences(this.licencesRepository.licences()));
  protected readonly filteredOperated = computed(() => this.filterLicences(this.licencesRepository.operatedLicences()));
  protected readonly countText = computed(() =>
    this.searchQuery() || this.statusFilter() !== 'all' || this.typeFilter() !== 'all'
      ? `${this.filteredOwned().length} of ${this.licencesRepository.licences().length} licences match`
      : `${this.licencesRepository.licences().length} licences on your account`,
  );
}
