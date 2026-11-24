/** Staff fit-and-proper pre-screen panel. */
import { ChangeDetectionStrategy, Component, computed, input, signal } from '@angular/core';
import { AdverseMediaScan } from './adverse-media-scan';
import type { IconName } from '@shared/ui/primitives/icons';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { Progress } from '@shared/ui/primitives/progress';
import { FP_RECOMMENDATION_META, FP_TIER_META, fpEscalationHeading, fpProgramLabel, fpRecommendationLabel } from '@domain/screening/fit-and-proper';
import { type FitAndProperResult, type FPDimension, type FPFinding, type FPTier } from '@domain/models/screening';
import { cn } from '@shared/util/utils';
import { OutcomeBadge } from './queue-badge';

const REC_TONE: Record<'positive' | 'caution' | 'danger', { box: string; text: string; icon: IconName }> = {
  positive: { box: 'border-green-200 bg-green-50', text: 'text-green-800', icon: 'ShieldCheck' },
  caution: { box: 'border-orange-200 bg-orange-50', text: 'text-orange-800', icon: 'ShieldAlert' },
  danger: { box: 'border-red-200 bg-red-50', text: 'text-red-800', icon: 'ShieldX' },
};

const TIER_TONE: Record<'positive' | 'caution' | 'danger', string> = {
  positive: 'bg-green-100 text-green-800 border-green-200',
  caution: 'bg-orange-100 text-orange-800 border-orange-200',
  danger: 'bg-red-100 text-red-800 border-red-200',
};

const DIMENSION_META: Record<FPDimension, { label: string; icon: IconName }> = {
  business: { label: 'The business', icon: 'Building2' },
  people: { label: 'Associated people', icon: 'Users' },
};

@Component({
  selector: 'fp-tier-badge',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `<span [attr.title]="meta().description" [class]="spanClass()">{{ meta().short }}</span>`,
})
export class TierBadge {
  readonly tier = input.required<FPTier>();
  protected readonly meta = computed(() => FP_TIER_META[this.tier()]);
  protected readonly spanClass = computed(() =>
    cn('inline-flex items-center rounded-full border px-2 py-0.5 text-[11px] font-medium', TIER_TONE[this.meta().tone]),
  );
}

@Component({
  selector: 'li[fpFindingRow]',
  imports: [LucideIcon, TierBadge, OutcomeBadge],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { class: 'rounded-lg border border-border p-3' },
  template: `
      <button
        type="button"
        (click)="open.set(!open())"
        class="flex w-full items-center justify-between gap-2 text-left"
        [attr.aria-expanded]="open()"
      >
        <span class="text-sm font-medium text-foreground">{{ finding().label }}</span>
        <div class="flex items-center gap-2 shrink-0">
          <fp-tier-badge [tier]="finding().tier" />
          <span class="text-xs text-muted-foreground">{{ finding().confidence }}%</span>
          <outcome-badge [outcome]="finding().outcome" />
          <svg lucide="ChevronDown" [class]="chevronClass()"></svg>
        </div>
      </button>
      <p class="text-sm text-muted-foreground mt-1.5">{{ finding().finding }}</p>
      @if (open() && finding().evidence.length > 0) {
        <ul class="mt-2.5 space-y-1.5 border-t border-border pt-2.5">
          @for (e of finding().evidence; track $index) {
            <li class="flex items-start gap-2 text-xs">
              <svg lucide="Circle" class="h-1.5 w-1.5 fill-current text-muted-foreground/50 shrink-0 mt-1.5"></svg>
              <span class="text-muted-foreground">
                <span class="font-medium text-foreground">{{ e.source }}:</span> {{ e.detail }}
              </span>
            </li>
          }
        </ul>
      }
  `,
})
export class FindingRow {
  readonly finding = input.required<FPFinding>();
  protected readonly open = signal(false);
  protected readonly chevronClass = computed(() => cn('h-4 w-4 text-muted-foreground transition-transform', this.open() && 'rotate-180'));
}

@Component({
  selector: 'fit-and-proper-panel',
  imports: [LucideIcon, Progress, OutcomeBadge, AdverseMediaScan, FindingRow],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (fp().applicable) {
      <div class="space-y-5">
        <!-- Recommendation banner -->
        <div [class]="bannerClass()">
          <div class="flex items-start gap-3">
            <svg [lucide]="tone().icon" [class]="iconClass()"></svg>
            <div class="min-w-0">
              <div class="flex flex-wrap items-center gap-2">
                <p [class]="titleClass()">Fit &amp; proper: {{ recLabel() }}</p>
                <span class="text-xs text-muted-foreground capitalize">{{ programLabel() }} pre-screen</span>
              </div>
              <p class="text-sm text-foreground/80 mt-1 text-pretty">{{ fp().recommendationRationale }}</p>
            </div>
          </div>

          <div class="mt-4 grid gap-4 sm:grid-cols-2">
            <div>
              <div class="flex items-center justify-between text-xs mb-1">
                <span class="text-muted-foreground">Screening confidence</span>
                <span [class]="titleClass2()">{{ fp().overallConfidence }}%</span>
              </div>
              <div uiProgress [value]="fp().overallConfidence" class="h-2"></div>
            </div>
            <div class="flex flex-col justify-center gap-1 text-xs text-muted-foreground">
              <span>
                <span class="font-medium text-foreground">{{ fp().tierCoverage.tier1 }}%</span> of checks
                resolvable in-portal{{ isCannabis() ? ' (Tier 1)' : '' }}
              </span>
              <span>
                <span class="font-medium text-foreground">{{ fp().people.length }}</span> associated
                individual{{ fp().people.length === 1 ? '' : 's' }} screened
              </span>
            </div>
          </div>
        </div>

        <!-- Escalation ruleset -->
        <div>
          <h4 class="text-sm font-semibold text-foreground mb-1">{{ escalationHeading() }}</h4>
          <p class="text-xs text-muted-foreground mb-3">{{ escalationBlurb() }}</p>
          <ul class="space-y-2">
            @for (c of fp().escalationCriteria; track $index) {
              <li class="flex items-start gap-2.5 rounded-md border border-border p-2.5">
                @if (c.met) {
                  <svg lucide="ShieldAlert" class="h-4 w-4 text-red-600 shrink-0 mt-0.5"></svg>
                } @else {
                  <svg lucide="CheckCircle2" class="h-4 w-4 text-green-600 shrink-0 mt-0.5"></svg>
                }
                <div class="min-w-0">
                  <div class="flex flex-wrap items-center gap-2">
                    <span class="text-sm text-foreground">{{ c.label }}</span>
                    <span [class]="metClass(c.met)">{{ c.met ? 'Triggered' : 'Clear' }}</span>
                  </div>
                  <p class="text-xs text-muted-foreground mt-0.5">{{ c.detail }}</p>
                </div>
              </li>
            }
          </ul>
        </div>

        <!-- Findings by dimension -->
        @for (dim of dimensions; track dim) {
          @if (findingsFor(dim); as items) {
            @if (items.length > 0) {
              <div>
                <h4 class="text-sm font-semibold text-foreground mb-3 flex items-center gap-2">
                  <svg [lucide]="dimensionMeta[dim].icon" class="h-4 w-4 text-[#013366]"></svg>
                  {{ dimensionMeta[dim].label }}
                </h4>
                <ul class="space-y-2">
                  @for (f of items; track f.id) {
                    <li fpFindingRow [finding]="f"></li>
                  }
                </ul>
              </div>
            }
          }
        }

        <!-- People screened -->
        @if (fp().people.length > 0) {
          <div>
            <h4 class="text-sm font-semibold text-foreground mb-3 flex items-center gap-2">
              <svg lucide="Users" class="h-4 w-4 text-[#013366]"></svg>
              People screened
            </h4>
            <ul class="space-y-2">
              @for (p of fp().people; track $index) {
                <li class="rounded-md border border-border p-2.5">
                  <div class="flex items-center justify-between gap-2">
                    <span class="text-sm font-medium text-foreground">{{ p.name }}</span>
                    <outcome-badge [outcome]="p.outcome" />
                  </div>
                  <p class="text-xs text-muted-foreground mt-0.5">{{ p.roles.join(', ') }} · {{ p.entity }}</p>
                  <p class="text-xs text-muted-foreground mt-1">{{ p.note }}</p>
                </li>
              }
            </ul>
          </div>
        }

        <!-- Adverse-media RAG scan: on demand because it does live web retrieval -->
        <div>
          <h4 class="text-sm font-semibold text-foreground mb-1 flex items-center gap-2">
            <svg lucide="Newspaper" class="h-4 w-4 text-[#013366]"></svg>
            Adverse-media scan
          </h4>
          <p class="text-xs text-muted-foreground mb-3">
            Live open-source search for negative media about the business and associated individuals, then an AI
            assessment of what was found. Advisory — verify identity before relying on any result.
          </p>
          <adverse-media-scan
            [program]="fp().program"
            [applicantName]="scanApplicantName()"
            [businessName]="businessSubject()"
            [people]="scanPeople()"
            [autoRun]="true"
          />
        </div>

        <p class="text-xs text-muted-foreground border-t border-border pt-3">{{ footnote() }}</p>
      </div>
    }
  `,
})
export class FitAndProperPanel {
  readonly fp = input.required<FitAndProperResult>();
  /** Business/applicant name, the primary subject of the adverse-media scan. */
  readonly applicantName = input<string | undefined>(undefined);

  protected readonly dimensions: FPDimension[] = ['business', 'people'];
  protected readonly dimensionMeta = DIMENSION_META;

  protected readonly rec = computed(() => FP_RECOMMENDATION_META[this.fp().recommendation]);
  protected readonly tone = computed(() => REC_TONE[this.rec().tone]);
  protected readonly isCannabis = computed(() => this.fp().program === 'cannabis');
  protected readonly recLabel = computed(() => fpRecommendationLabel(this.fp().recommendation, this.fp().program));
  protected readonly programLabel = computed(() => fpProgramLabel(this.fp().program));
  protected readonly bannerClass = computed(() => cn('rounded-lg border p-4', this.tone().box));
  protected readonly iconClass = computed(() => cn('h-5 w-5 shrink-0 mt-0.5', this.tone().text));
  protected readonly titleClass = computed(() => cn('text-sm font-semibold', this.tone().text));
  protected readonly titleClass2 = computed(() => cn('font-semibold', this.tone().text));

  // Business name for the scan: the passed applicant name, else the most common entity among the screened people.
  protected readonly businessSubject = computed(() => {
    const trimmed = this.applicantName()?.trim();
    if (trimmed) return trimmed;
    const counts = new Map<string, number>();
    for (const p of this.fp().people) {
      if (p.entity) counts.set(p.entity, (counts.get(p.entity) ?? 0) + 1);
    }
    return [...counts.entries()].sort((a, b) => b[1] - a[1])[0]?.[0];
  });
  protected readonly scanApplicantName = computed(
    () => this.applicantName() ?? this.businessSubject() ?? this.fp().people[0]?.entity ?? '',
  );
  protected readonly scanPeople = computed(() => this.fp().people.map((p) => ({ name: p.name, roles: p.roles, entity: p.entity })));

  // Program-specific wording for the escalation ruleset + coverage + footnote.
  protected readonly escalationHeading = computed(() => fpEscalationHeading(this.fp().program));
  protected readonly escalationBlurb = computed(() =>
    this.isCannabis()
      ? 'A case is only referred to the Security Programs Division when a criterion below is met. Everything else is cleared in-portal to reduce referral volume.'
      : 'A case is only sent for an enhanced suitability review when a criterion below is met. Everything else is cleared in-portal to keep new-licence and transfer applications moving.',
  );
  protected readonly footnote = computed(() =>
    this.isCannabis()
      ? 'Advisory Tier-1 screening only. The portal does not access restricted Tier-3 systems (CPIC, PRIME BC, or organized-crime intelligence) — those remain with SPD. An LCRB analyst makes the final fit-and-proper determination.'
      : "Advisory screening for liquor new-licence and transfer applications, drawn from the applicant's Personal History Summary, police information check, compliance history, and financial-integrity declarations. There is no organized-crime/SPD nexus. An LCRB analyst makes the final fit-and-proper determination.",
  );

  protected findingsFor(dim: FPDimension): FPFinding[] {
    return this.fp().findings.filter((f) => f.dimension === dim);
  }
  protected metClass(met: boolean): string {
    return cn('text-[11px] font-medium', met ? 'text-red-700' : 'text-green-700');
  }
}
