/** Licensee Licence Cap indicator; the engine runs after mount with a skeleton first, like the prototype's `useEffect`. */
import { afterNextRender, ChangeDetectionStrategy, Component, computed, inject, input, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import type { IconName } from '@shared/ui/primitives/icons';
import { injectBusinessProfile } from '@core/session/business-profile';
import { gatherLinkContext } from '@core/navigation/link-context';
import { type CapDomainResult, type LicenceCapResult } from '@data-access/repositories/licences/licence-cap-engine';
import { CANNABIS_LICENCE_CAP, CAP_STATUS_META } from '@domain/licences/licence-cap';
import { LICENCES_REPOSITORY } from '@data-access/repositories/licences/licences.repository';
import { path } from '@/app/route-registry';

type Tone = 'positive' | 'caution' | 'danger' | 'neutral';

const TONE_STYLES: Record<Tone, { bar: string; segEmpty: string; chip: string; text: string; ring: string }> = {
  neutral: {
    bar: 'bg-[#38598A]',
    segEmpty: 'bg-slate-100',
    chip: 'bg-slate-100 text-slate-700 border-slate-200',
    text: 'text-[#38598A]',
    ring: 'border-slate-200',
  },
  positive: {
    bar: 'bg-green-600',
    segEmpty: 'bg-green-100',
    chip: 'bg-green-100 text-green-800 border-green-200',
    text: 'text-green-700',
    ring: 'border-green-200',
  },
  caution: {
    bar: 'bg-[#F5A623]',
    segEmpty: 'bg-orange-100',
    chip: 'bg-orange-100 text-orange-800 border-orange-200',
    text: 'text-orange-700',
    ring: 'border-orange-200',
  },
  danger: {
    bar: 'bg-[#D8292F]',
    segEmpty: 'bg-red-100',
    chip: 'bg-red-100 text-red-800 border-red-200',
    text: 'text-red-700',
    ring: 'border-red-200',
  },
};

const SOURCE_LABEL: Record<string, string> = {
  direct: 'Direct holding',
  'legal-entity': 'Common ownership',
  'tied-house': 'Tied house link',
};

@Component({
  selector: 'cap-domain-gauge',
  imports: [LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div [class]="'rounded-lg border ' + styles().ring + ' p-4'">
      <div class="flex items-center justify-between mb-2">
        <div class="flex items-center gap-2">
          <svg [lucide]="icon()" [class]="'h-5 w-5 ' + styles().text" aria-hidden="true"></svg>
          <span class="font-medium text-foreground">{{ label() }}</span>
        </div>
        <span [class]="'rounded-full border px-2.5 py-0.5 text-xs font-semibold ' + styles().chip">{{ statusLabel() }}</span>
      </div>

      <div class="flex items-baseline gap-1.5">
        <span [class]="'text-3xl font-bold ' + styles().text">{{ domain().total }}</span>
        <span class="text-lg text-muted-foreground">/ {{ cap() }}</span>
      </div>

      <!-- Segmented gauge -->
      <div
        class="mt-3 flex gap-1"
        role="img"
        [attr.aria-label]="
          domain().total + ' of ' + cap() + ' ' + label().toLowerCase() + ' used, ' + domain().direct + ' direct and ' + domain().linked + ' linked. Status: ' + statusLabel() + '.'
        "
      >
        @for (cls of segments(); track $index) {
          <span [class]="'h-2.5 flex-1 rounded-sm ' + cls"></span>
        }
      </div>

      <div class="mt-2 flex flex-wrap gap-x-4 gap-y-1 text-xs text-muted-foreground">
        <span><span class="font-medium text-foreground">{{ domain().direct }}</span> direct</span>
        <span><span class="font-medium text-foreground">{{ domain().linked }}</span> linked</span>
        <span><span class="font-medium text-foreground">{{ remaining() }}</span>{{ ' ' }}remaining</span>
      </div>
    </div>
  `,
})
export class CapDomainGauge {
  readonly domain = input.required<CapDomainResult>();
  readonly label = input.required<string>();
  readonly icon = input.required<IconName>();

  private readonly tone = computed(() => CAP_STATUS_META[this.domain().status].tone as Tone);
  protected readonly styles = computed(() => TONE_STYLES[this.tone()]);
  protected readonly statusLabel = computed(() => CAP_STATUS_META[this.domain().status].label);
  protected readonly cap = computed(() => this.domain().cap ?? CANNABIS_LICENCE_CAP);
  protected readonly remaining = computed(() => Math.max(0, this.cap() - this.domain().total));
  protected readonly segments = computed(() => {
    const domain = this.domain();
    const cap = this.cap();
    const styles = this.styles();
    return Array.from({ length: Math.max(cap, domain.total) }).map((_, i) => {
      const overCap = i >= cap;
      const isDirect = i < domain.direct;
      const isLinked = i >= domain.direct && i < domain.total;
      let cls = styles.segEmpty;
      if (overCap && i < domain.total) cls = 'bg-[#D8292F]';
      else if (isDirect) cls = styles.bar;
      else if (isLinked) cls = `${styles.bar} opacity-50`;
      return `${cls} ${isLinked && !overCap ? 'outline-dashed outline-1 outline-offset-[-1px]' : ''}`;
    });
  });
}

/** Uncapped tile: no ratio, no gauge, no "remaining" for liquor. */
@Component({
  selector: 'cap-uncapped-tile',
  imports: [LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div [class]="'rounded-lg border ' + styles.ring + ' p-4'">
      <div class="flex items-center justify-between mb-2">
        <div class="flex items-center gap-2">
          <svg [lucide]="icon()" [class]="'h-5 w-5 ' + styles.text" aria-hidden="true"></svg>
          <span class="font-medium text-foreground">{{ label() }}</span>
        </div>
        <span [class]="'rounded-full border px-2.5 py-0.5 text-xs font-semibold ' + styles.chip">{{ noneLabel }}</span>
      </div>

      <div class="flex items-baseline gap-1.5">
        <span [class]="'text-3xl font-bold ' + styles.text">{{ domain().total }}</span>
        <span class="text-lg text-muted-foreground">in your business group</span>
      </div>

      <div class="mt-2 flex flex-wrap gap-x-4 gap-y-1 text-xs text-muted-foreground">
        <span><span class="font-medium text-foreground">{{ domain().direct }}</span> direct</span>
        <span><span class="font-medium text-foreground">{{ domain().linked }}</span> linked</span>
      </div>
      <p class="mt-3 text-xs leading-relaxed text-muted-foreground">
        There is no limit on the number of liquor licences a business may hold. These are listed because common ownership and
        tied house links still matter for other reviews.
      </p>
    </div>
  `,
})
export class CapUncappedTile {
  readonly domain = input.required<CapDomainResult>();
  readonly label = input.required<string>();
  readonly icon = input.required<IconName>();
  protected readonly styles = TONE_STYLES.neutral;
  protected readonly noneLabel = CAP_STATUS_META.none.label;
}

@Component({
  selector: 'cap-card-inner',
  imports: [LucideIcon, RouterLink, CapDomainGauge, CapUncappedTile],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <section aria-labelledby="licence-cap-heading" class="bg-white border rounded-lg overflow-hidden">
      <h2 id="licence-cap-heading" class="m-0">
        <button
          type="button"
          (click)="open.set(!open())"
          [attr.aria-expanded]="open()"
          aria-controls="licence-cap-panel"
          class="flex w-full items-center gap-3 bg-[#38598A] px-4 py-3 text-left text-white transition-colors hover:bg-[#2f4c78] focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-inset focus-visible:ring-white"
        >
          <svg lucide="ShieldAlert" class="h-5 w-5 shrink-0" aria-hidden="true"></svg>
          <span class="min-w-0 flex-1">
            <span class="block font-semibold leading-tight">Licence cap status</span>
            <span class="block text-xs text-white/80">
              {{
                open()
                  ? 'Maximum ' + cap + ' cannabis retail licences per business group. Liquor licences are not capped.'
                  : 'Cannabis ' + result().cannabis.total + ' of ' + cap + ' · Liquor ' + result().liquor.total + ' (no cap)'
              }}
            </span>
          </span>
          <span [class]="'hidden sm:inline-flex rounded-full border px-2.5 py-0.5 text-xs font-semibold ' + cannabisChip()">
            {{ cannabisLabel() }}
          </span>
          @if (result().requiresStaffReview) {
            <span class="hidden sm:inline-flex rounded-full border border-orange-200 bg-orange-100 px-2.5 py-0.5 text-xs font-semibold text-orange-800">
              Staff review
            </span>
          }
          <svg lucide="ChevronDown" [class]="'h-5 w-5 shrink-0 transition-transform ' + (open() ? 'rotate-180' : '')" aria-hidden="true"></svg>
          <span class="sr-only">{{ open() ? 'Collapse' : 'Expand' }} licence cap details</span>
        </button>
      </h2>

      <div id="licence-cap-panel" [hidden]="!open()" class="p-4 md:p-5">
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <cap-domain-gauge [domain]="result().cannabis" label="Cannabis licences" icon="Leaf" />
          <cap-uncapped-tile [domain]="result().liquor" label="Liquor licences" icon="Wine" />
        </div>

        <!-- Rationale / review banner -->
        <div
          [class]="
            'mt-4 flex items-start gap-3 rounded-md border p-3 text-sm ' +
            (result().requiresStaffReview ? 'bg-orange-50 border-orange-200 text-orange-900' : 'bg-green-50 border-green-200 text-green-900')
          "
        >
          @if (result().requiresStaffReview) {
            <svg lucide="ShieldAlert" class="h-4 w-4 mt-0.5 shrink-0 text-orange-700" aria-hidden="true"></svg>
          } @else {
            <svg lucide="Info" class="h-4 w-4 mt-0.5 shrink-0 text-green-700" aria-hidden="true"></svg>
          }
          <p class="leading-relaxed">
            {{ result().rationale }}{{ ' ' }}
            @if (result().requiresStaffReview) {
              <span class="font-medium">This is an advisory estimate — LCRB staff make the final cap determination.</span>
            }
          </p>
        </div>

        <!-- Toggle for the contributing detail -->
        <button
          type="button"
          (click)="showDetails.set(!showDetails())"
          [attr.aria-expanded]="showDetails()"
          class="mt-4 inline-flex items-center gap-1.5 text-sm font-medium text-[#013366] hover:underline"
        >
          {{ showDetails() ? 'Hide' : 'Show' }} how this is counted ({{ result().contributingLicences.length }} licence{{
            result().contributingLicences.length === 1 ? '' : 's'
          }}{{ linkedTotal() > 0 ? ', ' + linkedTotal() + ' linked' : '' }})
          @if (showDetails()) {
            <svg lucide="ChevronUp" class="h-4 w-4" aria-hidden="true"></svg>
          } @else {
            <svg lucide="ChevronDown" class="h-4 w-4" aria-hidden="true"></svg>
          }
        </button>

        @if (showDetails()) {
          <div class="mt-3 space-y-5">
            <!-- Grouped parties -->
            <div>
              <h3 class="flex items-center gap-1.5 text-xs font-semibold uppercase tracking-wide text-muted-foreground mb-2">
                <svg lucide="Users" class="h-3.5 w-3.5" aria-hidden="true"></svg>
                Effective business group
              </h3>
              <ul class="space-y-1.5">
                @for (party of result().groupedParties; track $index) {
                  <li class="text-sm">
                    <span class="font-medium text-foreground">{{ party.name }}</span>
                    @if (party.name === businessName()) {
                      <span class="ml-2 text-xs text-muted-foreground">(you)</span>
                    }
                    <span class="block text-xs text-muted-foreground">{{ party.detail }}</span>
                  </li>
                }
              </ul>
            </div>

            <!-- Contributing licences table -->
            <div>
              <h3 class="text-xs font-semibold uppercase tracking-wide text-muted-foreground mb-2">Contributing licences</h3>
              <div class="border rounded-md divide-y">
                @for (lic of result().contributingLicences; track lic.number) {
                  <div class="flex flex-wrap items-center gap-x-3 gap-y-1 px-3 py-2 text-sm">
                    <span class="font-medium text-[#013366]">{{ lic.establishmentName }}</span>
                    <span class="text-muted-foreground">#{{ lic.number }}</span>
                    <span class="text-muted-foreground">· {{ lic.type }}</span>
                    <span class="ml-auto flex items-center gap-2">
                      <span
                        [class]="
                          'rounded-full px-2 py-0.5 text-xs font-medium border ' +
                          (lic.domain === 'liquor' ? 'bg-blue-50 text-blue-800 border-blue-200' : 'bg-emerald-50 text-emerald-800 border-emerald-200')
                        "
                      >
                        {{ lic.domain }}
                      </span>
                      <span class="text-xs text-muted-foreground">{{ sourceLabel(lic.source) }}</span>
                    </span>
                  </div>
                }
              </div>
              <p class="mt-2 text-xs text-muted-foreground">
                Linked licences (common ownership or tied house) are provisional and may change once staff confirm the business
                relationships. Review these in the{{ ' ' }}
                <a [routerLink]="path('legal-entity.builder')" class="text-[#013366] underline">Legal Entity Builder</a>{{ ' ' }}
                and{{ ' ' }}
                <a [routerLink]="path('legal-entity.tied-house')" class="text-[#013366] underline">Tied House</a>{{ ' ' }}
                sections.
              </p>
            </div>
          </div>
        }
      </div>
    </section>
  `,
})
export class CapCardInner {
  protected readonly path = path;
  readonly result = input.required<LicenceCapResult>();
  readonly businessName = input.required<string>();
  protected readonly cap = CANNABIS_LICENCE_CAP;
  protected readonly showDetails = signal(false);
  // Collapsed by default: the header still carries cannabis N of 8 + status.
  protected readonly open = signal(false);
  protected readonly linkedTotal = computed(() => this.result().liquor.linked + this.result().cannabis.linked);
  protected readonly cannabisChip = computed(() => TONE_STYLES[CAP_STATUS_META[this.result().cannabis.status].tone as Tone].chip);
  protected readonly cannabisLabel = computed(() => CAP_STATUS_META[this.result().cannabis.status].label);
  protected sourceLabel(source: string) {
    return SOURCE_LABEL[source] ?? source;
  }
}

@Component({
  selector: 'licence-cap-card',
  imports: [CapCardInner],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (result(); as r) {
      <cap-card-inner [result]="r" [businessName]="profile().legalName" />
    } @else {
      <!-- Matches the collapsed header height so the page doesn't jump on mount. -->
      <section class="bg-white border rounded-lg overflow-hidden" aria-busy="true">
        <div class="bg-[#38598A] h-[60px] flex items-center px-4 gap-3">
          <div class="h-5 w-5 rounded bg-white/20 animate-pulse"></div>
          <div class="flex-1">
            <div class="h-4 w-36 rounded bg-white/30 animate-pulse mb-1.5"></div>
            <div class="h-3 w-56 rounded bg-white/20 animate-pulse"></div>
          </div>
        </div>
      </section>
    }
  `,
})
export class LicenceCapCard {
  private readonly licencesRepository = inject(LICENCES_REPOSITORY);
  protected readonly profile = injectBusinessProfile();
  private readonly mounted = signal(false);
  /** The engine reads client-side storage, so it only runs once mounted (skeleton first). */
  protected readonly result = computed<LicenceCapResult | null>(() => {
    if (!this.mounted()) return null;
    const profile = this.profile();
    const link = gatherLinkContext(profile.bceidBusinessNumber);
    return this.licencesRepository.licenceCap({
      applicantName: profile.legalName,
      ownedLicences: this.licencesRepository.licences(),
      leBuild: link.leBuild,
      existingRelationships: link.existingRelationships,
      declarations: link.declarations,
    });
  });

  constructor() {
    afterNextRender(() => this.mounted.set(true));
  }
}
