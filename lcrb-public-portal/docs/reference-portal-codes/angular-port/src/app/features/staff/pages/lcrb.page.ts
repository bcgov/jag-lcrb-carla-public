/** Staff Intake Overview; content mounts only when the gate shows it, so the case store is not seeded for non-staff visitors. */
import { ChangeDetectionStrategy, Component, computed, inject, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { BCGovFooter } from '@core/layout/footer';
import { BCGovHeader } from '@core/layout/header';
import { PAGE_HERO } from '@shared/ui/bc-gov/page-hero';
import { LcrbGate } from '@core/permissions/lcrb-gate';
import { LgCommentingNotice } from '../components/lg-commenting-notice';
import { QueueBadge } from '../components/queue-badge';
import { injectSession } from '@core/session/session-provider';
import { Button } from '@shared/ui/primitives/button';
import { CARD } from '@shared/ui/primitives/card';
import type { IconName } from '@shared/ui/primitives/icons';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { QUEUE_META } from '@domain/staff/agent-log';
import { type DynamicsQueue } from '@domain/models/staff';
import { relativeAge } from '@domain/staff/lcrb-analytics';
import { STAFF_REPOSITORY } from '@data-access/repositories/staff/staff.repository';
import { path } from '@/app/route-registry';

const PRIORITY_QUEUES: DynamicsQueue[] = ['beyond-licence-cap', 'section-119-review', 'enhanced-review'];

@Component({
  selector: 'div[uiCard][lcrbStat]',
  imports: [...CARD, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div uiCardContent class="p-5 flex items-center gap-4">
      <div class="rounded-lg bg-muted p-3">
        <svg [lucide]="icon()" [class]="'h-6 w-6 ' + accent()"></svg>
      </div>
      <div>
        <div class="text-2xl font-semibold text-foreground">{{ value() }}</div>
        <div class="text-sm text-muted-foreground">{{ label() }}</div>
      </div>
    </div>
  `,
})
export class LcrbStat {
  readonly icon = input.required<IconName>();
  readonly label = input.required<string>();
  readonly value = input.required<number | string>();
  readonly accent = input.required<string>();
}

/** The link wraps a card. */
@Component({
  selector: 'a[lcrbQuickLink]',
  imports: [...CARD, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div uiCard class="hover:shadow-md transition-shadow">
      <div uiCardContent class="p-4 flex items-start gap-3">
        <div class="rounded-lg bg-[#013366] p-2 text-white shrink-0">
          <svg [lucide]="icon()" class="h-4 w-4"></svg>
        </div>
        <div>
          <p class="font-medium text-foreground">{{ title() }}</p>
          <p class="text-sm text-muted-foreground">{{ desc() }}</p>
        </div>
      </div>
    </div>
  `,
})
export class LcrbQuickLink {
  readonly icon = input.required<IconName>();
  readonly title = input.required<string>();
  readonly desc = input.required<string>();
}

@Component({
  selector: 'lcrb-overview-content',
  imports: [RouterLink, ...PAGE_HERO, ...CARD, Button, LucideIcon, QueueBadge, LgCommentingNotice, LcrbStat, LcrbQuickLink],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <page-hero
      [breadcrumbs]="[{ label: 'Intake Overview' }]"
      title="Intake Overview"
      description="Cases submitted through the portal are pre-screened by the AI triage agent and synced to MS Dynamics. Review where they landed, then work the highest-priority queues first."
    />

    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <!-- Stat cards -->
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 mb-8">
        <div uiCard lcrbStat class="h-full" [icon]="'Inbox'" label="Cases in intake" [value]="analytics.total" accent="text-[#013366]"></div>
        <div uiCard lcrbStat class="h-full" [icon]="'ShieldAlert'" label="Need human review" [value]="analytics.requiresReviewCount" accent="text-orange-600"></div>
        <div uiCard lcrbStat class="h-full" [icon]="'Database'" label="Synced to Dynamics" [value]="analytics.syncSuccessRate + '%'" accent="text-[#38598A]"></div>
        <div uiCard lcrbStat class="h-full" [icon]="'CheckCircle2'" label="Auto-routed standard" [value]="analytics.autoRouteRate + '%'" accent="text-green-700"></div>
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
        <!-- Priority cases -->
        <section class="lg:col-span-2">
          <div class="flex items-center justify-between mb-4">
            <h2 class="text-xl font-semibold text-foreground">Priority cases</h2>
            <a uiButton variant="ghost" size="sm" [routerLink]="path('staff.triage')" class="gap-1 text-[#013366]">
              Open full triage
              <svg lucide="ArrowRight" class="h-4 w-4"></svg>
            </a>
          </div>
          <div class="space-y-3">
            @if (priorityCases.length === 0) {
              <div uiCard>
                <div uiCardContent class="py-10 text-center text-muted-foreground">No priority cases in the queue right now.</div>
              </div>
            }
            @for (c of priorityCases; track c.id) {
              <a [routerLink]="path('staff.triage')" [queryParams]="{ case: c.id }">
                <div uiCard class="hover:shadow-md transition-shadow">
                  <div uiCardContent class="p-4">
                    <div class="flex items-start justify-between gap-3">
                      <div class="min-w-0">
                        <p class="font-semibold text-[#013366]">{{ c.reference }}</p>
                        <p class="text-sm text-muted-foreground mt-0.5">{{ c.caseType }} · {{ c.applicant }}</p>
                      </div>
                      <queue-badge [queue]="c.queue ?? 'standard-processing'" />
                    </div>
                    <div class="mt-2 flex flex-wrap items-center gap-x-4 gap-y-1 text-xs text-muted-foreground">
                      @if (c.agentLog) {
                        <span>Agent confidence {{ c.agentLog.overallConfidence }}%</span>
                      }
                      @if (c.dynamicsCaseId) {
                        <span class="inline-flex items-center gap-1">
                          <svg lucide="Database" class="h-3.5 w-3.5"></svg>
                          {{ c.dynamicsCaseId }}
                        </span>
                      }
                      <span>Synced {{ relativeAge(c.lastAttemptAt) }}</span>
                    </div>
                  </div>
                </div>
              </a>
            }
          </div>
        </section>

        <!-- Queue breakdown + quick links -->
        <section>
          <!-- Jurisdictions that opted out of commenting: no comment hold applies -->
          <lg-commenting-notice className="mb-6" />

          <h2 class="text-xl font-semibold text-foreground mb-4">Queues</h2>
          <div uiCard class="mb-4">
            <div uiCardContent class="p-0 divide-y">
              @for (q of analytics.byQueue; track q.queue) {
                <div class="flex items-center justify-between gap-2 p-4">
                  <div class="min-w-0">
                    <queue-badge [queue]="q.queue" />
                    <p class="text-xs text-muted-foreground mt-1.5">{{ queueMeta[q.queue].description }}</p>
                  </div>
                  <span class="text-2xl font-semibold text-foreground shrink-0">{{ q.count }}</span>
                </div>
              }
            </div>
          </div>

          <div class="grid gap-3">
            <a lcrbQuickLink [routerLink]="path('staff.triage')" [icon]="'ListChecks'" title="Case triage" desc="Work cases by queue and review agent findings."></a>
            <a
              lcrbQuickLink
              [routerLink]="path('staff.conversations')"
              [icon]="'MessageSquare'"
              title="Case conversations"
              desc="Live threads with applicants, local governments and police on in-process files."
            ></a>
            <a lcrbQuickLink [routerLink]="path('staff.map')" [icon]="'Map'" title="Licensing map" desc="Licences, applications, and contraventions with the inspection planner."></a>
            <a lcrbQuickLink [routerLink]="path('staff.analytics')" [icon]="'BarChart3'" title="Intake analytics" desc="Trends across queues, confidence, and checks."></a>
            <a lcrbQuickLink [routerLink]="path('staff.site-analytics')" [icon]="'LineChart'" title="Site analytics" desc="Traffic, unique visitors, and popular pages & applications."></a>
          </div>
        </section>
      </div>
    </div>
  `,
})
export class LcrbOverviewContent {
  protected readonly path = path;
  private readonly staffRepository = inject(STAFF_REPOSITORY);
  protected readonly cases = this.staffRepository.cases();
  protected readonly analytics = this.staffRepository.intakeAnalytics(this.cases);
  protected readonly queueMeta = QUEUE_META;
  protected readonly relativeAge = relativeAge;
  protected readonly priorityCases = this.cases
    .filter((c) => PRIORITY_QUEUES.includes((c.queue ?? 'standard-processing') as DynamicsQueue))
    .sort((a, b) => (a.lastAttemptAt < b.lastAttemptAt ? 1 : -1));
}

@Component({
  selector: 'lcrb-page',
  imports: [BCGovHeader, BCGovFooter, LcrbGate, LcrbOverviewContent],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="min-h-screen flex flex-col bg-background">
      <bc-gov-header currentPage="/lcrb" />
      <main class="flex-1">
        <lcrb-gate>
          @if (allowed()) {
            <lcrb-overview-content />
          }
        </lcrb-gate>
      </main>
      <bc-gov-footer />
    </div>
  `,
})
export class LcrbPage {
  private readonly session = injectSession();
  /** What `LcrbGate` lets through: its content is created only then, as in React. */
  protected readonly allowed = computed(() => !this.session.loading() && this.session.session()?.role === 'lcrb');

}
