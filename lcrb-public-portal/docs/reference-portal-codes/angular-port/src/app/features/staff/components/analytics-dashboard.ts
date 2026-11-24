/** Staff view of intake analytics charted with ECharts; the data is aggregated from the prototype case store, so back it with a reporting query over cases in Dynamics. */
import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { type BarChartSpec, type ChartConfig, ChartContainer, type PieChartSpec } from '@shared/ui/charts/chart';
import { CARD } from '@shared/ui/primitives/card';
import type { IconName } from '@shared/ui/primitives/icons';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import type { IntakeAnalytics } from '@data-access/repositories/staff/lcrb-analytics';
import { path } from '@/app/route-registry';

const QUEUE_COLOR: Record<string, string> = {
  'beyond-licence-cap': 'var(--chart-5)',
  'section-119-review': 'var(--chart-3)',
  'enhanced-review': 'var(--chart-2)',
  'standard-processing': 'var(--chart-4)',
};

const CONF_COLOR: Record<string, string> = {
  high: 'var(--chart-4)',
  medium: 'var(--chart-3)',
  low: 'var(--chart-5)',
};

/** The file's Kpi, on its own `div` because the grid places its direct children. */
@Component({
  selector: 'div[uiCard][analyticsKpi]',
  imports: [...CARD, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div uiCardContent class="p-5">
      <div class="flex items-center gap-3">
        <div class="rounded-lg bg-muted p-2.5">
          <svg [lucide]="icon()" [class]="'h-5 w-5 ' + accent()"></svg>
        </div>
        <div class="text-sm text-muted-foreground">{{ label() }}</div>
      </div>
      <div class="mt-3 text-3xl font-semibold text-foreground">{{ value() }}</div>
      <div class="text-xs text-muted-foreground mt-1">{{ sub() }}</div>
    </div>
  `,
})
export class AnalyticsKpi {
  readonly icon = input.required<IconName>();
  readonly label = input.required<string>();
  readonly value = input.required<number | string>();
  readonly sub = input.required<string>();
  readonly accent = input.required<string>();
}

@Component({
  selector: 'analytics-dashboard',
  imports: [RouterLink, ...CARD, LucideIcon, ChartContainer, AnalyticsKpi],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="space-y-8">
      <!-- KPI cards -->
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        <div
          uiCard
          analyticsKpi
          [icon]="'Inbox'"
          label="Cases synced"
          [value]="analytics().total"
          [sub]="analytics().syncSuccessRate + '% sync success'"
          accent="text-[#013366]"
        ></div>
        <div
          uiCard
          analyticsKpi
          [icon]="'ShieldAlert'"
          label="Need human review"
          [value]="analytics().requiresReviewCount"
          [sub]="'of ' + analytics().total + ' intake cases'"
          accent="text-orange-600"
        ></div>
        <div
          uiCard
          analyticsKpi
          [icon]="'CheckCircle2'"
          label="Auto-routed to standard"
          [value]="analytics().autoRouteRate + '%'"
          sub="cleared with no elevated signals"
          accent="text-green-700"
        ></div>
        <div
          uiCard
          analyticsKpi
          [icon]="'Gauge'"
          label="Avg. agent confidence"
          [value]="analytics().avgConfidence + '%'"
          sub="across pre-screened cases"
          accent="text-[#38598A]"
        ></div>
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <!-- Cases by queue -->
        <div uiCard>
          <div uiCardHeader>
            <div uiCardTitle>Cases by triage queue</div>
            <div uiCardDescription>Where the pre-screening agent routed incoming cases.</div>
          </div>
          <div uiCardContent>
            <div chartContainer [config]="queueConfig" [spec]="queueSpec()" class="h-[280px] w-full"></div>
          </div>
        </div>

        <!-- Confidence distribution -->
        <div uiCard>
          <div uiCardHeader>
            <div uiCardTitle>Agent confidence distribution</div>
            <div uiCardDescription>How confident the agent was in its overall triage recommendation.</div>
          </div>
          <div uiCardContent>
            <div chartContainer [config]="confidenceConfig" [spec]="confidenceSpec()" class="h-[280px] w-full"></div>
          </div>
        </div>
      </div>

      <!-- Check outcomes stacked -->
      <div uiCard>
        <div uiCardHeader>
          <div uiCardTitle>Pre-screening check outcomes</div>
          <div uiCardDescription>
            Outcomes across every check the agent runs. Flags and attention items are where analysts
            focus first.
          </div>
        </div>
        <div uiCardContent>
          <div chartContainer [config]="outcomeConfig" [spec]="outcomeSpec()" class="h-[300px] w-full"></div>
        </div>
      </div>

      <div class="rounded-lg border border-border bg-muted/40 p-4 flex flex-col sm:flex-row sm:items-center justify-between gap-3">
        <p class="text-sm text-muted-foreground">
          Analytics are derived live from the cases synced to MS Dynamics by the pre-screening agent.
        </p>
        <a
          [routerLink]="path('staff.triage')"
          class="inline-flex items-center gap-1 text-sm font-medium text-[#013366] hover:underline shrink-0"
        >
          Go to case triage
          <svg lucide="ArrowRight" class="h-4 w-4"></svg>
        </a>
      </div>
    </div>
  `,
})
export class AnalyticsDashboard {
  protected readonly path = path;
  readonly analytics = input.required<IntakeAnalytics>();

  protected readonly queueConfig: ChartConfig = { count: { label: 'Cases' } };
  protected readonly confidenceConfig: ChartConfig = {
    high: { label: 'High (80+)', color: 'var(--chart-4)' },
    medium: { label: 'Medium (55–79)', color: 'var(--chart-3)' },
    low: { label: 'Low (<55)', color: 'var(--chart-5)' },
  };
  protected readonly outcomeConfig: ChartConfig = {
    clear: { label: 'Clear', color: 'var(--chart-4)' },
    attention: { label: 'Attention', color: 'var(--chart-3)' },
    flag: { label: 'Flag', color: 'var(--chart-5)' },
  };

  /** BarChart layout="vertical": CartesianGrid horizontal=false, YAxis width 130, Bar radius [0,4,4,0] with Cells. */
  protected readonly queueSpec = computed<BarChartSpec>(() => ({
    type: 'bar',
    layout: 'vertical',
    data: this.analytics().byQueue,
    margin: { left: 8, right: 16, top: 4, bottom: 4 },
    categoryKey: 'label',
    categoryAxis: { width: 130, tickFontSize: 11, tickLine: false, axisLine: false },
    valueAxis: { allowDecimals: false },
    grid: { horizontal: false, vertical: true },
    bars: [{ dataKey: 'count', radius: [0, 4, 4, 0], cellFills: this.analytics().byQueue.map((d) => QUEUE_COLOR[d.queue]) }],
  }));

  /** PieChart: innerRadius 60, outerRadius 100, paddingAngle 2, legend with nameKey "label". */
  protected readonly confidenceSpec = computed<PieChartSpec>(() => ({
    type: 'pie',
    data: this.analytics().byConfidence,
    dataKey: 'count',
    nameKey: 'label',
    innerRadius: 60,
    outerRadius: 100,
    paddingAngle: 2,
    cellFills: this.analytics().byConfidence.map((d) => CONF_COLOR[d.band]),
    legend: { nameKey: 'label' },
    tooltipNameKey: 'label',
  }));

  /** Stacked BarChart: CartesianGrid vertical=false, XAxis (no tick or axis line, 12px), YAxis, legend. */
  protected readonly outcomeSpec = computed<BarChartSpec>(() => ({
    type: 'bar',
    data: this.analytics().byCheckOutcome,
    margin: { left: 8, right: 16, top: 4, bottom: 4 },
    categoryKey: 'label',
    categoryAxis: { tickLine: false, axisLine: false, tickFontSize: 12 },
    valueAxis: { allowDecimals: false },
    grid: { horizontal: true, vertical: false },
    legend: {},
    bars: [
      { dataKey: 'clear', stackId: 'a', fill: 'var(--color-clear)', radius: [0, 0, 0, 0] },
      { dataKey: 'attention', stackId: 'a', fill: 'var(--color-attention)' },
      { dataKey: 'flag', stackId: 'a', fill: 'var(--color-flag)', radius: [4, 4, 0, 0] },
    ],
  }));
}
