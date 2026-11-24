/** Establishment Planner: saved plans, plan detail, feasibility, checklist and AI advisor. Local components use `display: contents` hosts to keep layouts. */
import { ChangeDetectionStrategy, Component, computed, effect, inject, input, output, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Button } from '@shared/ui/primitives/button';
import { Checkbox } from '@shared/ui/primitives/checkbox';
import type { IconName } from '@shared/ui/primitives/icons';
import { Input } from '@shared/ui/primitives/input';
import { Label } from '@shared/ui/primitives/label';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { type ChecklistCategory, type FeasibilitySummary, type FlagTone, type PlannerAnswers } from '@data-access/repositories/applications/establishment-feasibility';
import { ADD_ON_LABELS, ESTABLISHMENT_TYPES, getEstablishmentType } from '@domain/applications/establishment-feasibility';
import { type AddOnId, type EstablishmentTypeId } from '@domain/models/applications';
import {
  createPlan,
  deletePlan,
  injectEstablishmentPlan,
  injectEstablishmentPlans,
  renamePlan,
  setChecklistItem,
  updatePlanAnswers,
} from '../state/establishment-plan-store';
import { injectBusinessProfile } from '@core/session/business-profile';
import { getLicenceHoldings } from '@domain/account/business-profile';
import { PlannerAdvisorPanel } from './planner-advisor-panel';
import { APPLICATIONS_REPOSITORY } from '@data-access/repositories/applications/applications.repository';
import { path } from '@/app/route-registry';


function clampInt(value: string): number {
  const n = Number.parseInt(value, 10);
  if (Number.isNaN(n) || n < 0) return 0;
  return Math.min(n, 20);
}

/** Compact plan description sent to the AI advisor as grounding context. */
function buildPlanSummaryText(
  title: string,
  answers: PlannerAnswers,
  summary: FeasibilitySummary | null,
  readiness: number,
): string {
  if (!summary) {
    return `Plan "${title}". The applicant has not chosen an establishment type yet.`;
  }
  const lines: string[] = [];
  lines.push(`Plan: "${title}".`);
  lines.push(`Establishment: ${summary.type.label} (${summary.domain}).`);
  if (answers.city) lines.push(`Location: ${answers.city}${answers.address ? `, ${answers.address}` : ''}.`);
  if (answers.applicantName) lines.push(`Applicant: ${answers.applicantName}.`);
  lines.push(`Estimated application fees: ${summary.feeSubtotalDisplay}. Typical timeline: ${summary.timeline.label}.`);
  lines.push(
    `Applications: ${summary.applications.map((a) => `${a.title} (${a.feeDisplay}${a.feeIsEstimate ? ' est.' : ''})`).join('; ')}.`,
  );
  lines.push(
    `Licence cap (cannabis retail only; liquor is uncapped): cannabis ${summary.cap.cannabis.total}/${summary.cap.cannabis.cap} (${summary.cap.cannabis.status}); liquor in group ${summary.cap.liquor.total}.`,
  );
  if (summary.section119) {
    lines.push(`Section 119: ${summary.section119.status} — ${summary.section119.rationale}`);
  }
  if (summary.flags.length) {
    lines.push(`Flags: ${summary.flags.map((f) => `${f.title} (${f.tone})`).join('; ')}.`);
  }
  lines.push(`Readiness: ${readiness}%.`);
  return lines.join('\n');
}


@Component({
  selector: 'readiness-ring',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="relative shrink-0" [style.width.px]="size()" [style.height.px]="size()">
      <svg [attr.width]="size()" [attr.height]="size()" class="-rotate-90" role="img" [attr.aria-label]="value() + '% ready'">
        <circle [attr.cx]="size() / 2" [attr.cy]="size() / 2" [attr.r]="radius()" fill="none" stroke="var(--border)" [attr.stroke-width]="stroke" />
        <circle
          [attr.cx]="size() / 2"
          [attr.cy]="size() / 2"
          [attr.r]="radius()"
          fill="none"
          [attr.stroke]="color()"
          [attr.stroke-width]="stroke"
          [attr.stroke-dasharray]="circumference()"
          [attr.stroke-dashoffset]="offset()"
          stroke-linecap="round"
        />
      </svg>
      <span class="absolute inset-0 flex items-center justify-center text-xs font-bold text-foreground">{{ value() }}%</span>
    </div>
  `,
})
export class ReadinessRing {
  readonly value = input.required<number>();
  readonly size = input(64);
  protected readonly stroke = 6;
  protected readonly radius = computed(() => (this.size() - this.stroke) / 2);
  protected readonly circumference = computed(() => 2 * Math.PI * this.radius());
  protected readonly offset = computed(() => this.circumference() - (this.value() / 100) * this.circumference());
  protected readonly color = computed(() => (this.value() >= 75 ? '#2E8540' : this.value() >= 40 ? '#F5A623' : '#013366'));
}

@Component({
  selector: 'readiness-badge',
  imports: [LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="flex items-center gap-2 rounded-full border border-border bg-card px-3 py-1.5 shadow-sm">
      <svg lucide="CircleDot" class="h-4 w-4 text-primary"></svg>
      <span class="text-sm font-medium text-foreground">Readiness</span>
      <span class="text-sm font-bold text-primary">{{ value() }}%</span>
    </div>
  `,
})
export class ReadinessBadge {
  readonly value = input.required<number>();
}


const FLAG_STYLES: Record<FlagTone, { wrap: string; icon: IconName }> = {
  positive: { wrap: 'border-[#2E8540]/30 bg-[#2E8540]/5 text-[#1E5631]', icon: 'CheckCircle2' },
  caution: { wrap: 'border-amber-300 bg-amber-50 text-amber-900', icon: 'TriangleAlert' },
  danger: { wrap: 'border-red-300 bg-red-50 text-red-900', icon: 'TriangleAlert' },
  info: { wrap: 'border-primary/30 bg-primary/5 text-primary', icon: 'Info' },
};

@Component({
  selector: 'metric-card',
  imports: [LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="rounded-lg border border-border bg-card p-4 shadow-sm">
      <div class="flex items-center gap-2 text-muted-foreground">
        <svg [lucide]="icon()" class="h-4 w-4"></svg>
        <span class="text-xs font-medium uppercase tracking-wide">{{ label() }}</span>
      </div>
      <p class="mt-2 text-lg font-bold text-foreground">{{ value() }}</p>
      <p class="text-xs text-muted-foreground">{{ sub() }}</p>
    </div>
  `,
})
export class MetricCard {
  readonly icon = input.required<IconName>();
  readonly label = input.required<string>();
  readonly value = input.required<string>();
  readonly sub = input.required<string>();
}

@Component({
  selector: 'feasibility-summary-view',
  imports: [Button, LucideIcon, MetricCard],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <section class="flex flex-col gap-4">
      <h2 class="text-base font-semibold text-foreground">Your feasibility summary</h2>

      <!-- Key metrics -->
      <div class="grid gap-3 sm:grid-cols-3">
        <metric-card icon="Coins" label="Estimated fees" [value]="summary().feeSubtotalDisplay" sub="Application fees" />
        <metric-card icon="CalendarClock" label="Typical timeline" [value]="summary().timeline.label" sub="From complete application" />
        <metric-card
          icon="Building2"
          label="Licence type"
          [value]="summary().type.label"
          [sub]="summary().domain === 'cannabis' ? 'Cannabis' : 'Liquor'"
        />
      </div>

      <!-- Flags -->
      <div class="flex flex-col gap-2">
        @for (flag of summary().flags; track $index) {
          <div [class]="'flex gap-3 rounded-lg border px-4 py-3 ' + flagStyles[flag.tone].wrap">
            <svg [lucide]="flagStyles[flag.tone].icon" class="mt-0.5 h-5 w-5 shrink-0"></svg>
            <div>
              <p class="text-sm font-semibold">{{ flag.title }}</p>
              <p class="text-sm leading-relaxed opacity-90">{{ flag.detail }}</p>
            </div>
          </div>
        }
      </div>

      <!-- Applications / handoff -->
      <div class="rounded-lg border border-border bg-card p-4 shadow-sm">
        <h3 class="flex items-center gap-2 text-sm font-semibold text-foreground">
          <svg lucide="ShieldCheck" class="h-4 w-4 text-primary"></svg>
          Applications you'll likely file
        </h3>
        <ul class="mt-3 flex flex-col gap-2">
          @for (app of summary().applications; track (app.slug ?? 'primary') + '-' + $index) {
            <li class="flex flex-wrap items-center justify-between gap-2 rounded-lg border border-border bg-background p-3">
              <div class="min-w-0">
                <p class="flex items-center gap-2 text-sm font-medium text-foreground">
                  {{ app.title }}
                  @if (app.kind === 'primary') {
                    <span class="rounded-full bg-primary/10 px-2 py-0.5 text-[10px] font-semibold uppercase tracking-wide text-primary">Primary</span>
                  }
                </p>
                <p class="text-xs text-muted-foreground">{{ app.feeDisplay }}{{ app.feeIsEstimate ? ' (estimate)' : '' }}</p>
              </div>
              <button uiButton size="sm" [variant]="app.kind === 'primary' ? 'default' : 'outline'" (click)="launch(app.slug)">
                {{ app.slug ? 'Start application' : 'Browse applications' }}
              </button>
            </li>
          }
        </ul>
        @if (summary().type.annualNote) {
          <p class="mt-3 text-xs text-muted-foreground">{{ summary().type.annualNote }}</p>
        }
        <p class="mt-3 text-[11px] text-muted-foreground">
          Fees and timelines are provisional estimates for planning. The LCRB confirms the exact requirements for your
          application.
        </p>
      </div>
    </section>
  `,
})
export class FeasibilitySummaryView {
  private readonly router = inject(Router);
  readonly summary = input.required<FeasibilitySummary>();
  readonly answers = input.required<PlannerAnswers>();
  protected readonly flagStyles = FLAG_STYLES;

  protected launch(slug: string | undefined): void {
    const answers = this.answers();
    const params = new URLSearchParams();
    if (answers.establishmentName) params.set('establishmentName', answers.establishmentName);
    if (answers.applicantName) {
      params.set('legalName', answers.applicantName);
      params.set('legalEntityName', answers.applicantName);
    }
    const qs = params.toString();
    if (slug) {
      void this.router.navigateByUrl(qs ? `${path('applications.flow', { slug: slug })}?${qs}` : path('applications.flow', { slug: slug }));
    } else {
      void this.router.navigateByUrl(path('applications.list'));
    }
  }
}


const CATEGORY_LABELS: Record<ChecklistCategory, string> = {
  business: 'Business & legal',
  location: 'Location & zoning',
  premises: 'Premises',
  people: 'People & screening',
  compliance: 'Compliance',
};

const CATEGORY_ORDER: ChecklistCategory[] = ['business', 'location', 'premises', 'people', 'compliance'];

@Component({
  selector: 'checklist-view',
  imports: [Checkbox, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <section class="rounded-lg border border-border bg-card p-5 shadow-sm">
      <div class="flex items-center justify-between">
        <h2 class="flex items-center gap-2 text-base font-semibold text-foreground">
          <svg lucide="ClipboardList" class="h-5 w-5 text-primary"></svg>
          Document &amp; task checklist
        </h2>
        <span class="text-sm font-medium text-muted-foreground">{{ completed() }} / {{ total() }} done</span>
      </div>

      <div class="mt-4 flex flex-col gap-5">
        @for (group of grouped(); track group.cat) {
          <div>
            <h3 class="text-xs font-semibold uppercase tracking-wide text-muted-foreground">{{ categoryLabels[group.cat] }}</h3>
            <ul class="mt-2 flex flex-col gap-2">
              @for (item of group.items; track item.id) {
                <li class="flex gap-3 rounded-lg border border-border bg-background p-3">
                  <button
                    uiCheckbox
                    [id]="'chk-' + item.id"
                    [checked]="!!done()[item.id]"
                    (checkedChange)="toggle.emit({ itemId: item.id, value: $event })"
                    class="mt-0.5"
                  ></button>
                  <label [attr.for]="'chk-' + item.id" class="flex-1 cursor-pointer">
                    <span
                      [class]="'block text-sm font-medium ' + (done()[item.id] ? 'text-muted-foreground line-through' : 'text-foreground')"
                      >{{ item.label }}</span
                    >
                    <span class="block text-xs leading-relaxed text-muted-foreground">{{ item.detail }}</span>
                  </label>
                </li>
              }
            </ul>
          </div>
        }
      </div>
    </section>
  `,
})
export class ChecklistView {
  readonly summary = input.required<FeasibilitySummary>();
  readonly done = input.required<Record<string, boolean>>();
  readonly toggle = output<{ itemId: string; value: boolean }>();
  protected readonly categoryLabels = CATEGORY_LABELS;
  protected readonly completed = computed(() => this.summary().checklist.filter((i) => this.done()[i.id]).length);
  protected readonly total = computed(() => this.summary().checklist.length);
  protected readonly grouped = computed(() =>
    CATEGORY_ORDER.map((cat) => ({
      cat,
      items: this.summary().checklist.filter((i) => i.category === cat),
    })).filter((g) => g.items.length > 0),
  );
}


@Component({
  selector: 'planner-interview',
  imports: [Checkbox, Input, Label, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <section class="rounded-lg border border-border bg-card p-5 shadow-sm">
      <h2 class="flex items-center gap-2 text-base font-semibold text-foreground">
        <svg lucide="ClipboardList" class="h-5 w-5 text-primary"></svg>
        Tell us about your establishment
      </h2>

      <!-- Establishment type -->
      <fieldset class="mt-4">
        <legend class="text-sm font-medium text-foreground">What are you planning to open?</legend>
        <div class="mt-3 grid gap-2 sm:grid-cols-2">
          @for (t of types; track t.id) {
            <button
              type="button"
              (click)="patch.emit({ establishmentType: $any(t.id) })"
              [attr.aria-pressed]="answers().establishmentType === t.id"
              [class]="
                'flex flex-col rounded-lg border p-3 text-left transition-colors ' +
                (answers().establishmentType === t.id
                  ? 'border-primary bg-primary/5 ring-1 ring-primary'
                  : 'border-border bg-background hover:border-primary/50 hover:bg-muted/40')
              "
            >
              <span class="flex items-center justify-between gap-2">
                <span class="text-sm font-semibold text-foreground">{{ t.label }}</span>
                <span
                  [class]="
                    'rounded-full px-2 py-0.5 text-[10px] font-medium uppercase tracking-wide ' +
                    (t.domain === 'cannabis' ? 'bg-[#2E8540]/10 text-[#1E5631]' : 'bg-primary/10 text-primary')
                  "
                  >{{ t.domain }}</span
                >
              </span>
              <span class="mt-1 text-xs leading-relaxed text-muted-foreground">{{ t.blurb }}</span>
            </button>
          }
        </div>
      </fieldset>

      <!-- Basic identity -->
      <div class="mt-5 grid gap-4 sm:grid-cols-2">
        <div class="flex flex-col gap-1.5">
          <label uiLabel for="applicantName">Business / legal name</label>
          <input
            uiInput
            id="applicantName"
            [value]="profile().legalName"
            readonly
            aria-describedby="applicantName-hint"
            class="bg-muted/50 text-muted-foreground"
          />
          <p id="applicantName-hint" class="text-xs text-muted-foreground">From your signed-in business account.</p>
        </div>
        <div class="flex flex-col gap-1.5">
          <label uiLabel for="establishmentName">Establishment name</label>
          <input
            uiInput
            id="establishmentName"
            [value]="answers().establishmentName ?? ''"
            (input)="patch.emit({ establishmentName: $any($event.target).value })"
            placeholder="e.g. The Blue Anchor"
          />
          <p class="text-xs text-muted-foreground">The trading name for this new establishment.</p>
        </div>
        <div class="flex flex-col gap-1.5">
          <label uiLabel for="city">City / community</label>
          <input
            uiInput
            id="city"
            [value]="answers().city ?? ''"
            (input)="patch.emit({ city: $any($event.target).value })"
            placeholder="e.g. Victoria"
          />
        </div>
        <div class="flex flex-col gap-1.5">
          <label uiLabel for="address">Proposed address (optional)</label>
          <input
            uiInput
            id="address"
            [value]="answers().address ?? ''"
            (input)="patch.emit({ address: $any($event.target).value })"
            placeholder="Street address"
          />
        </div>
      </div>

      <!-- Add-ons -->
      @if (typeDef(); as td) {
        @if (td.relevantAddOns.length > 0) {
          <fieldset class="mt-5">
            <legend class="text-sm font-medium text-foreground">
              Planning any of these? <span class="font-normal text-muted-foreground">(optional)</span>
            </legend>
            <div class="mt-3 flex flex-col gap-2">
              @for (addOn of td.relevantAddOns; track addOn) {
                <label class="flex items-center gap-3 text-sm text-foreground">
                  <button uiCheckbox [checked]="(answers().addOns ?? []).includes(addOn)" (checkedChange)="toggleAddOn(addOn, $event)"></button>
                  {{ addOnLabels[addOn] }}
                </label>
              }
            </div>
          </fieldset>
        }
      }

      <!-- Existing holdings -->
      <div class="mt-5">
        <p class="text-sm font-medium text-foreground">Licences you already hold</p>
        <p class="text-xs text-muted-foreground">
          Pre-filled from your account. Adjust if you hold licences under associated businesses that count toward the
          same cap.
        </p>
        <div class="mt-3 grid gap-4 sm:grid-cols-2">
          <div class="flex flex-col gap-1.5">
            <label uiLabel for="existingLiquor">Liquor licences</label>
            <input
              uiInput
              id="existingLiquor"
              type="number"
              min="0"
              [value]="answers().existingLiquorLicences ?? 0"
              (input)="patchInt('existingLiquorLicences', $any($event.target))"
            />
          </div>
          <div class="flex flex-col gap-1.5">
            <label uiLabel for="existingCannabis">Cannabis licences</label>
            <input
              uiInput
              id="existingCannabis"
              type="number"
              min="0"
              [value]="answers().existingCannabisLicences ?? 0"
              (input)="patchInt('existingCannabisLicences', $any($event.target))"
            />
          </div>
        </div>
      </div>

      <!-- Cannabis / First Nations -->
      @if (isCannabis()) {
        <div class="mt-5 flex flex-col gap-3 rounded-lg border border-border bg-muted/30 p-4">
          <label class="flex items-center gap-3 text-sm text-foreground">
            <button uiCheckbox [checked]="!!answers().firstNationAffiliated" (checkedChange)="patch.emit({ firstNationAffiliated: $event })"></button>
            The business is First Nation owned or affiliated
          </label>
          @if (answers().firstNationAffiliated) {
            <div class="flex flex-col gap-1.5">
              <label uiLabel for="nationName">Nation name (optional)</label>
              <input
                uiInput
                id="nationName"
                [value]="answers().firstNationName ?? ''"
                (input)="patch.emit({ firstNationName: $any($event.target).value })"
                placeholder="Name of the Indigenous nation"
              />
            </div>
            <label class="flex items-center gap-3 text-sm text-foreground">
              <button uiCheckbox [checked]="!!answers().onFirstNationLands" (checkedChange)="patch.emit({ onFirstNationLands: $event })"></button>
              The premises will be on the nation's lands
            </label>
          }
        </div>
      }
    </section>
  `,
})
export class Interview {
  readonly answers = input.required<PlannerAnswers>();
  readonly patch = output<Partial<PlannerAnswers>>();
  protected readonly profile = injectBusinessProfile();
  protected readonly types = ESTABLISHMENT_TYPES;
  protected readonly addOnLabels = ADD_ON_LABELS;
  /** A controlled number input: emit the clamped value and write it back, as React re-syncs the DOM. */
  protected patchInt(key: 'existingLiquorLicences' | 'existingCannabisLicences', el: HTMLInputElement): void {
    const value = clampInt(el.value);
    this.patch.emit({ [key]: value });
    // ReactDOMInput's number rule: rewrite only on a loose mismatch, or 0 over an empty field.
    // eslint-disable-next-line eqeqeq
    if ((value === 0 && el.value === '') || el.value != (value as unknown as string)) el.value = String(value);
  }
  protected readonly typeDef = computed(() => getEstablishmentType(this.answers().establishmentType));
  protected readonly isCannabis = computed(() => this.typeDef()?.domain === 'cannabis');

  constructor() {
    // The business / legal name is authoritative from the signed-in BCeID business account: keep the stored answer in sync (and read-only in the UI).
    effect(() => {
      const legalName = this.profile().legalName;
      if (legalName && this.answers().applicantName !== legalName) {
        this.patch.emit({ applicantName: legalName });
      }
    });
  }

  protected toggleAddOn(addOn: AddOnId, v: boolean): void {
    const current = new Set(this.answers().addOns ?? []);
    if (v) current.add(addOn);
    else current.delete(addOn);
    this.patch.emit({ addOns: Array.from(current) as AddOnId[] });
  }
}


@Component({
  selector: 'plan-header',
  imports: [Button, Input],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div>
      @if (editing()) {
        <div class="flex items-center gap-2">
          <input
            uiInput
            [value]="value()"
            (input)="value.set($any($event.target).value)"
            aria-label="Plan name"
            class="max-w-sm"
          />
          <button uiButton size="sm" (click)="save()">Save</button>
        </div>
      } @else {
        <div class="flex items-center gap-3">
          <h1 class="text-xl font-bold text-foreground">{{ plan().title }}</h1>
          <button type="button" (click)="startEditing()" class="text-sm font-medium text-primary hover:underline">Rename</button>
        </div>
      }
    </div>
  `,
})
export class PlanHeader {
  readonly plan = input.required<{ id: string; title: string }>();
  protected readonly editing = signal(false);
  protected readonly value = signal('');

  constructor() {
    // useState(plan.title): seeded once from the plan the header mounts with.
    let seeded = false;
    effect(() => {
      const title = this.plan().title;
      if (!seeded) {
        seeded = true;
        this.value.set(title);
      }
    });
  }

  protected startEditing(): void {
    this.value.set(this.plan().title);
    this.editing.set(true);
  }

  protected save(): void {
    renamePlan(this.plan().id, this.value().trim() || 'Untitled establishment');
    this.editing.set(false);
  }
}

@Component({
  selector: 'plan-detail',
  imports: [
    Button,
    ChecklistView,
    FeasibilitySummaryView,
    Interview,
    LucideIcon,
    PlanHeader,
    PlannerAdvisorPanel,
    ReadinessBadge,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (plan(); as p) {
      <div class="mx-auto w-full max-w-6xl">
        <div class="flex items-center justify-between gap-3">
          <button uiButton variant="ghost" (click)="back.emit()" class="gap-1">
            <svg lucide="ArrowLeft" class="h-4 w-4"></svg> All plans
          </button>
          <readiness-badge [value]="readiness()" />
        </div>

        <div class="mt-4 grid gap-6 lg:grid-cols-[1fr_360px]">
          <!-- Main column -->
          <div class="flex flex-col gap-6">
            <plan-header [plan]="p" />
            <planner-interview [answers]="p.answers" (patch)="update($event)" />
            @if (summary(); as s) {
              <feasibility-summary-view [summary]="s" [answers]="p.answers" />
              <checklist-view [summary]="s" [done]="p.checklistDone" (toggle)="onToggle($event)" />
            } @else {
              <div class="rounded-lg border border-dashed border-border bg-muted/30 p-6 text-center text-sm text-muted-foreground">
                Choose an establishment type above to see your feasibility summary, fees, timeline, and document
                checklist.
              </div>
            }
          </div>

          <!-- Sidebar -->
          <aside class="flex flex-col gap-4">
            <div class="rounded-lg border border-border bg-card shadow-sm lg:sticky lg:top-4 lg:h-[calc(100vh-2rem)]">
              <planner-advisor-panel [planSummary]="advisorSummary()" />
            </div>
          </aside>
        </div>
      </div>
    } @else {
      <div class="mx-auto w-full max-w-4xl">
        <button uiButton variant="ghost" (click)="back.emit()" class="gap-1">
          <svg lucide="ArrowLeft" class="h-4 w-4"></svg> Back
        </button>
        <p class="mt-6 text-muted-foreground">This plan could not be found.</p>
      </div>
    }
  `,
})
export class PlanDetail {
  private readonly applicationsRepository = inject(APPLICATIONS_REPOSITORY);
  readonly planId = input.required<string>();
  readonly back = output<void>();
  protected readonly plan = injectEstablishmentPlan(() => this.planId());
  protected readonly summary = computed(() => {
    const p = this.plan();
    return p ? this.applicationsRepository.computeFeasibility(p.answers) : null;
  });
  protected readonly readiness = computed(() => {
    const p = this.plan();
    return p ? this.applicationsRepository.computeReadiness(p.answers, this.summary(), p.checklistDone) : 0;
  });
  protected readonly advisorSummary = computed(() => {
    const p = this.plan();
    return p ? buildPlanSummaryText(p.title, p.answers, this.summary(), this.readiness()) : '';
  });

  protected update(patch: Partial<PlannerAnswers>): void {
    updatePlanAnswers(this.planId(), patch);
  }

  protected onToggle(e: { itemId: string; value: boolean }): void {
    setChecklistItem(this.planId(), e.itemId, e.value);
  }
}


@Component({
  selector: 'establishment-planner',
  imports: [Button, LucideIcon, PlanDetail, ReadinessRing],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (activeId(); as id) {
      <plan-detail [planId]="id" (back)="activeId.set(null)" />
    } @else {
      <div class="mx-auto w-full max-w-4xl">
        <header class="flex flex-col gap-2">
          <span class="flex h-12 w-12 items-center justify-center rounded-full bg-primary/10 text-primary">
            <svg lucide="Building2" class="h-6 w-6"></svg>
          </span>
          <h1 class="mt-2 text-2xl font-bold text-balance text-foreground">Establishment Planner</h1>
          <p class="max-w-2xl text-pretty leading-relaxed text-muted-foreground">
            Planning to open a new liquor or cannabis establishment? Answer a few questions and get a personalized
            feasibility check — the licence you'll need, estimated fees and timeline, what could hold you up, and a
            document checklist to get you application-ready.
          </p>
        </header>

        <div class="mt-8">
          <button uiButton (click)="startNewPlan()" class="gap-2">
            <svg lucide="Plus" class="h-4 w-4"></svg>
            Start a new plan
          </button>
        </div>

        @if (plans().length > 0) {
          <section class="mt-10">
            <h2 class="text-sm font-semibold uppercase tracking-wide text-muted-foreground">Your saved plans</h2>
            <ul class="mt-3 flex flex-col gap-3">
              @for (row of rows(); track row.plan.id) {
                <li class="flex items-center gap-4 rounded-lg border border-border bg-card p-4 shadow-sm">
                  <readiness-ring [value]="row.readiness" [size]="52" />
                  <button type="button" (click)="activeId.set(row.plan.id)" class="flex flex-1 flex-col text-left">
                    <span class="font-semibold text-foreground">{{ row.plan.title }}</span>
                    <span class="text-sm text-muted-foreground">{{ row.typeLabel }} · {{ row.date }}</span>
                  </button>
                  <button uiButton variant="ghost" size="sm" class="gap-1" (click)="activeId.set(row.plan.id)">
                    Open <svg lucide="ArrowRight" class="h-4 w-4"></svg>
                  </button>
                  <button
                    uiButton
                    variant="ghost"
                    size="icon"
                    [attr.aria-label]="'Delete ' + row.plan.title"
                    (click)="remove(row.plan.id)"
                  >
                    <svg lucide="Trash2" class="h-4 w-4 text-muted-foreground"></svg>
                  </button>
                </li>
              }
            </ul>
          </section>
        }
      </div>
    }
  `,
})
export class EstablishmentPlanner {
  private readonly applicationsRepository = inject(APPLICATIONS_REPOSITORY);
  protected readonly plans = injectEstablishmentPlans();
  private readonly profile = injectBusinessProfile();
  protected readonly activeId = signal<string | null>(null);

  protected readonly rows = computed(() =>
    this.plans().map((plan) => {
      const summary = this.applicationsRepository.computeFeasibility(plan.answers);
      const readiness = this.applicationsRepository.computeReadiness(plan.answers, summary, plan.checklistDone);
      const typeDef = getEstablishmentType(plan.answers.establishmentType);
      return {
        plan,
        readiness,
        typeLabel: typeDef ? typeDef.label : 'Not started',
        date: new Date(plan.updatedAt).toLocaleDateString('en-CA', { month: 'short', day: 'numeric', year: 'numeric' }),
      };
    }),
  );

  // Pre-populate what we already know from the signed-in business account: the legal name and the liquor/cannabis licences currently held.
  protected startNewPlan(): void {
    const profile = this.profile();
    const holdings = getLicenceHoldings(profile);
    const plan = createPlan('Untitled establishment', {
      applicantName: profile.legalName,
      existingLiquorLicences: holdings.liquor,
      existingCannabisLicences: holdings.cannabis,
    });
    this.activeId.set(plan.id);
  }

  protected remove(id: string): void {
    deletePlan(id);
  }
}
