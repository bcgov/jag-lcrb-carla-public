/** Licence detail page. An unknown id shows an inline "Licence not found", so no guard is needed. `?tab=` is read once, like the prototype's `useState`. */
import { ChangeDetectionStrategy, Component, computed, inject, input, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { toast } from '@spartan-ng/brain/sonner';
import { StatusExplainer } from '@shared/ui/ai/status-explainer';
import { BCGovFooter } from '@core/layout/footer';
import { BCGovHeader } from '@core/layout/header';
import { PAGE_HERO } from '@shared/ui/bc-gov/page-hero';
import { LicenceInspections } from '../components/licence-inspections';
import { Badge } from '@shared/ui/primitives/badge';
import { Button } from '@shared/ui/primitives/button';
import type { IconName } from '@shared/ui/primitives/icons';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { TABS } from '@shared/ui/primitives/tabs';
import { licenceTypeFromLabel } from '@domain/applications/application-eligibility';
import { formatInspectionDateTime } from '@domain/licences/inspections';
import { formatLicenceDate, getLicenceTerms } from '@domain/licences/licences';
import { licenceToRollup } from '@shared/util/pdf/pdf-rollup';
import { generateApplicationPdf } from '@shared/util/pdf/pdf-rollup';
import { LICENCES_REPOSITORY } from '@data-access/repositories/licences/licences.repository';
import { ACTIVITY_REPOSITORY } from '@data-access/repositories/activity/activity.repository';
import { path } from '@/app/route-registry';

const STATUS_CONFIG: Record<string, { color: string; icon: IconName }> = {
  Active: { color: 'bg-green-100 text-green-800 border-green-200', icon: 'CheckCircle' },
  'Pending Renewal': { color: 'bg-yellow-100 text-yellow-800 border-yellow-200', icon: 'Clock' },
  Expired: { color: 'bg-red-100 text-red-800 border-red-200', icon: 'AlertTriangle' },
};

// Map the licence type to the matching transfer flow slug.
const TRANSFER_SLUG: Record<string, string> = {
  'Food Primary': 'fp-transfer',
  'Liquor Primary': 'lp-transfer',
  'Cannabis Retail Store': 'crs-transfer',
  'Liquor Retail Store': 'lrs-transfer',
};

@Component({
  selector: 'licence-detail-page',
  imports: [RouterLink, BCGovHeader, BCGovFooter, ...PAGE_HERO, ...TABS, Button, Badge, LucideIcon, StatusExplainer, LicenceInspections],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (licence(); as l) {
      <div class="min-h-screen flex flex-col bg-background">
        <bc-gov-header currentPage="/licences" />

        <main class="flex-1">
          <page-hero containerClassName="mx-auto max-w-5xl px-4 sm:px-6 lg:px-8" [breadcrumbs]="breadcrumbs()" [title]="l.name">
            <ng-container heroMeta>{{ l.type }} | Licence #{{ l.id }}</ng-container>
            <span heroBadge uiBadge variant="outline" [class]="status().color">
              <svg [lucide]="status().icon" class="h-3 w-3 mr-1"></svg>
              {{ l.status }}
            </span>
            @for (flag of l.flags; track flag) {
              <span heroBadge uiBadge variant="outline" class="bg-orange-50 text-orange-700 border-orange-200">{{ flag }}</span>
            }
            <a heroActions uiButton class="bg-bc-gold font-semibold text-bc-blue hover:bg-bc-gold/90 gap-2" [routerLink]="tree(renewHref())">
              <svg lucide="RefreshCw" class="h-4 w-4"></svg>
              Renew Licence
            </a>
            <a
              heroActions
              uiButton
              variant="outline"
              class="gap-2 border-white/40 bg-transparent text-white hover:bg-white/10 hover:text-white"
              [routerLink]="tree(transferHref())"
            >
              <svg lucide="ArrowLeftRight" class="h-4 w-4"></svg>
              Transfer Licence
            </a>
            <button
              heroActions
              uiButton
              variant="outline"
              class="gap-2 border-white/40 bg-transparent text-white hover:bg-white/10 hover:text-white"
              (click)="handleDownload()"
            >
              <svg lucide="Download" class="h-4 w-4"></svg>
              Download Certificate
            </button>
          </page-hero>

          <div class="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
            <div uiTabs [value]="activeTab()" (valueChange)="activeTab.set($event ?? 'overview')" class="w-full">
              <div uiTabsList class="mb-6">
                <button uiTabsTrigger value="overview">Overview</button>
                <button uiTabsTrigger value="inspections" class="gap-1.5">
                  Inspections
                  @if (nextInspection()) {
                    <span
                      class="ml-1 inline-flex h-5 min-w-5 items-center justify-center rounded-full bg-[#013366] px-1.5 text-xs font-medium text-white"
                    >
                      {{ scheduledInspectionCount() }}
                    </span>
                  }
                </button>
                <button uiTabsTrigger value="terms">Terms &amp; Conditions</button>
              </div>

              <div uiTabsContent value="overview">
                <div class="bg-white border rounded-lg p-6 grid grid-cols-1 md:grid-cols-2 gap-6 text-sm">
                  <div>
                    <div class="text-muted-foreground mb-1">Location</div>
                    <div class="flex items-start gap-2">
                      <svg lucide="MapPin" class="h-4 w-4 text-muted-foreground mt-0.5 shrink-0"></svg>
                      <span>{{ l.address }}</span>
                    </div>
                  </div>
                  <div>
                    <div class="text-muted-foreground mb-1">Contact</div>
                    <div class="space-y-1">
                      <div class="flex items-center gap-2">
                        <svg lucide="Phone" class="h-4 w-4 text-muted-foreground"></svg>
                        <span>{{ l.phone }}</span>
                      </div>
                      <div class="flex items-center gap-2">
                        <svg lucide="Mail" class="h-4 w-4 text-muted-foreground"></svg>
                        <span>{{ l.email }}</span>
                      </div>
                    </div>
                  </div>
                  <div>
                    <div class="text-muted-foreground mb-1">Expiry / Renewal Date</div>
                    <div class="font-medium">{{ formatDate(l.expiryDate) }}</div>
                  </div>
                  <div>
                    <div class="text-muted-foreground mb-1">Operated By</div>
                    <div class="text-[#255A90] font-medium">{{ l.operatedBy }}</div>
                    @if (l.licencedTo) {
                      <div class="text-muted-foreground">Licenced To: {{ l.licencedTo }}</div>
                    }
                  </div>
                </div>

                <!-- What this licence already carries: permanent vs. time-limited. -->
                <section aria-labelledby="endorsements-heading" class="mt-6 rounded-lg border bg-white p-6">
                  <div class="flex items-center justify-between gap-4">
                    <h2 id="endorsements-heading" class="font-semibold text-[#013366]">Endorsements</h2>
                    <a uiButton variant="outline" size="sm" [routerLink]="tree(endorsementHref())">Add endorsement</a>
                  </div>
                  @if (l.endorsements && l.endorsements.length > 0) {
                    <ul class="mt-4 divide-y">
                      @for (e of l.endorsements; track e.name) {
                        @let soon = expiringSoon(e.expires);
                        <li
                          class="flex flex-col gap-1 py-3 first:pt-0 last:pb-0 sm:flex-row sm:items-start sm:justify-between sm:gap-6"
                        >
                          <div class="flex-1">
                            <div class="flex flex-wrap items-center gap-2">
                              <span class="font-medium">{{ e.name }}</span>
                              <span uiBadge variant="outline" [class]="endorsementClass(e.kind, soon)">
                                {{ e.kind === 'permanent' ? 'Permanent' : 'Temporary' }}
                              </span>
                            </div>
                            <p class="mt-1 text-sm text-muted-foreground">{{ e.summary }}</p>
                          </div>
                          <div class="text-sm text-muted-foreground sm:text-right sm:shrink-0">
                            <div>Granted {{ formatDate(e.granted) }}</div>
                            @if (e.expires) {
                              <div [class]="soon ? 'font-medium text-orange-700' : ''">Expires {{ formatDate(e.expires) }}</div>
                            }
                          </div>
                        </li>
                      }
                    </ul>
                  } @else {
                    <p class="mt-3 text-sm text-muted-foreground">
                      This licence has no endorsements. Endorsements extend what a licence permits — for example allowing minors
                      with a guardian, adding a patio, or hosting patron-participation entertainment.
                    </p>
                  }
                </section>

                @if (nextInspection(); as next) {
                  <button
                    type="button"
                    (click)="activeTab.set('inspections')"
                    class="mt-6 w-full flex items-start gap-3 rounded-lg border border-l-4 border-l-[#013366] bg-white p-4 text-left hover:bg-muted/40 transition-colors"
                  >
                    <svg lucide="CalendarClock" class="h-5 w-5 text-[#013366] mt-0.5 shrink-0" aria-hidden="true"></svg>
                    <div class="flex-1">
                      <div class="text-sm text-muted-foreground">Next scheduled inspection</div>
                      <div class="font-semibold text-foreground mt-0.5">{{ inspectionDate(next.scheduledDate) }}</div>
                      <div class="text-sm text-muted-foreground mt-0.5">{{ next.window }} · Compliance &amp; Enforcement</div>
                    </div>
                    <span class="text-sm font-medium text-[#013366] shrink-0 self-center">View inspections</span>
                  </button>
                }
                <div class="mt-6">
                  <status-explainer
                    [subjectType]="l.type + ' licence'"
                    [title]="l.type + ' — ' + l.operatedBy"
                    [status]="l.status"
                    [extra]="extra()"
                  />
                </div>
              </div>

              <div uiTabsContent value="inspections">
                <licence-inspections [licenceId]="l.id" />
              </div>

              <div uiTabsContent value="terms">
                <div class="bg-white border rounded-lg divide-y">
                  @for (term of terms(); track term.heading) {
                    <div class="p-6">
                      <h2 class="font-semibold text-[#013366] mb-1">{{ term.heading }}</h2>
                      <p class="text-sm text-muted-foreground leading-relaxed">{{ term.body }}</p>
                    </div>
                  }
                  <div class="p-6 bg-muted/30">
                    <button uiButton variant="outline" class="gap-2" (click)="handleDownload()">
                      <svg lucide="Download" class="h-4 w-4"></svg>
                      Download Licence Copy (PDF)
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </main>

        <bc-gov-footer />
      </div>
    } @else {
      <div class="min-h-screen flex flex-col bg-background">
        <bc-gov-header currentPage="/licences" />
        <main class="flex-1">
          <div class="max-w-3xl mx-auto px-4 sm:px-6 lg:px-8 py-16 text-center">
            <svg lucide="FileText" class="h-12 w-12 mx-auto text-muted-foreground mb-4"></svg>
            <h1 class="text-2xl font-semibold text-foreground">Licence not found</h1>
            <p class="text-muted-foreground mt-2">We couldn't find a licence with number {{ id() }}.</p>
            <a uiButton class="mt-6 bg-[#013366] hover:bg-[#012a52]" [routerLink]="path('licences.list')">Back to Licences</a>
          </div>
        </main>
        <bc-gov-footer />
      </div>
    }
  `,
})
export class LicenceDetailPage {
  protected readonly path = path;
  private readonly licencesRepository = inject(LICENCES_REPOSITORY);
  private readonly activityRepository = inject(ACTIVITY_REPOSITORY);
  private readonly route = inject(ActivatedRoute);
  readonly id = input.required<string>();

  private readonly tabParam = this.route.snapshot.queryParamMap.get('tab');
  protected readonly activeTab = signal<string>(
    this.tabParam === 'terms' ? 'terms' : this.tabParam === 'inspections' ? 'inspections' : 'overview',
  );

  protected readonly licence = computed(() => this.licencesRepository.licence(this.id()));
  protected readonly status = computed(() => STATUS_CONFIG[this.licence()!.status] ?? STATUS_CONFIG['Active']);
  protected readonly terms = computed(() => getLicenceTerms(this.licence()!.type));
  protected readonly nextInspection = computed(() => this.licencesRepository.nextInspection(this.id()));
  protected readonly scheduledInspectionCount = computed(() => this.licencesRepository.scheduledInspections(this.id()).length);
  protected readonly breadcrumbs = computed(() => [
    { label: 'Dashboard', href: path('dashboard.home') },
    { label: 'Licences & Authorizations', href: path('licences.list') },
    { label: this.licence()!.name },
  ]);

  /** Carry the licence identity into the renewal/transfer wizards via prefill. */
  private readonly prefillQuery = computed(() => {
    const licence = this.licence()!;
    return new URLSearchParams({
      licenceNumber: licence.id,
      establishmentName: licence.name,
      legalEntityName: licence.operatedBy,
    }).toString();
  });
  protected readonly renewHref = computed(() => `${path('applications.flow', { slug: 'licence-renewal' })}?${this.prefillQuery()}`);
  protected readonly transferHref = computed(
    () => `${path('applications.flow', { slug: TRANSFER_SLUG[this.licence()!.type] ?? 'lp-transfer' })}?${this.prefillQuery()}`,
  );
  // Land on the tile page scoped to this licence type with "endorsement" already in the search box.
  protected readonly endorsementHref = computed(() => {
    const endorsementType = licenceTypeFromLabel(this.licence()!.type);
    return `${path('applications.new')}?q=endorsement${endorsementType ? `&type=${endorsementType}` : ''}`;
  });
  protected readonly extra = computed(() => {
    const l = this.licence()!;
    return `Expiry / renewal date: ${formatLicenceDate(l.expiryDate)}.${l.flags?.length ? ` Flags: ${l.flags.join(', ')}.` : ''}`;
  });

  protected formatDate(value: string) {
    return formatLicenceDate(value);
  }
  protected inspectionDate(iso: string) {
    return formatInspectionDateTime(iso);
  }
  protected expiringSoon(expires?: string) {
    return expires && new Date(expires).getTime() - Date.now() < 60 * 86_400_000;
  }
  protected endorsementClass(kind: string, soon: unknown) {
    return kind === 'permanent'
      ? 'bg-emerald-50 text-emerald-800 border-emerald-200'
      : soon
        ? 'bg-orange-50 text-orange-700 border-orange-200'
        : 'bg-sky-50 text-sky-800 border-sky-200';
  }
  protected tree(href: string) {
    return this.router.parseUrl(href);
  }
  private readonly router = inject(Router);

  protected handleDownload() {
    const licence = this.licence()!;
    generateApplicationPdf(licenceToRollup(licence));
    this.activityRepository.recordAudit({
      category: 'document',
      action: 'licence.certificate-downloaded',
      summary: `Downloaded licence certificate for ${licence.name} (${licence.id})`,
      actor: licence.operatedBy,
      actorRole: 'licensee',
      subjectId: licence.id,
      subjectType: 'licence',
    });
    toast.success('Certificate downloaded', { description: `${licence.type} — ${licence.id}` });
  }
}
