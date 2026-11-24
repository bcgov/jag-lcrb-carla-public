/** Staff view of portal traffic and usage, charted with ECharts. The data is a generated mock; wire the page loader to a real analytics source with the same shape. */
import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { CARD } from '@shared/ui/primitives/card';
import { type BarChartSpec, type ChartConfig, ChartContainer, type PieChartSpec } from '@shared/ui/charts/chart';
import type { IconName } from '@shared/ui/primitives/icons';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import type { SiteAnalytics } from '@data-access/repositories/staff/site-analytics';

const PALETTE = ['var(--chart-1)', 'var(--chart-2)', 'var(--chart-3)', 'var(--chart-4)', 'var(--chart-5)', '#7A9CC6', '#6C7A89'];
const palette = (n: number) => Array.from({ length: n }, (_, i) => PALETTE[i % PALETTE.length]);

/** The file's Kpi, on its own `div` because the grid places its direct children. */
@Component({
  selector: 'div[uiCard][siteKpi]',
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
      <div class="text-xs text-muted-foreground mt-1 flex items-center gap-1">
        @if (trend() === 'up') {
          <svg lucide="TrendingUp" class="h-3.5 w-3.5 text-green-700"></svg>
        }
        @if (trend() === 'down') {
          <svg lucide="TrendingDown" class="h-3.5 w-3.5 text-[#CE3E39]"></svg>
        }
        {{ sub() }}
      </div>
    </div>
  `,
})
export class SiteKpi {
  readonly icon = input.required<IconName>();
  readonly label = input.required<string>();
  readonly value = input.required<number | string>();
  readonly sub = input.required<string>();
  readonly accent = input.required<string>();
  readonly trend = input<'up' | 'down' | undefined>(undefined);
}

@Component({
  selector: 'site-analytics-dashboard',
  imports: [...CARD, LucideIcon, ChartContainer, SiteKpi],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="space-y-8">
      <!-- KPI cards -->
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        <div
          uiCard
          siteKpi
          [icon]="'Users'"
          label="Unique visitors"
          [value]="analytics().uniqueVisitors.toLocaleString()"
          [sub]="(trendUp() ? '+' : '') + analytics().visitorsTrendPct + '% vs prior 30 days'"
          [trend]="trendUp() ? 'up' : 'down'"
          accent="text-[#013366]"
        ></div>
        <div
          uiCard
          siteKpi
          [icon]="'Eye'"
          label="Total page views"
          [value]="analytics().totalViews.toLocaleString()"
          [sub]="analytics().pagesPerSession + ' pages / session'"
          accent="text-[#38598A]"
        ></div>
        <div
          uiCard
          siteKpi
          [icon]="'CalendarDays'"
          label="Avg. visitors / day"
          [value]="analytics().avgVisitorsPerDay.toLocaleString()"
          [sub]="analytics().visitorsToday + ' today'"
          accent="text-green-700"
        ></div>
        <div
          uiCard
          siteKpi
          [icon]="'MousePointerClick'"
          label="Sessions"
          [value]="analytics().sessions.toLocaleString()"
          [sub]="analytics().viewsToday + ' views today'"
          accent="text-orange-600"
        ></div>
      </div>

      <!-- Daily traffic: bar chart -->
      <div uiCard>
        <div uiCardHeader>
          <div uiCardTitle>Daily traffic (last 30 days)</div>
          <div uiCardDescription>
            Unique visitors and page views per day. Weekday peaks and the gradual growth trend are
            visible across the window.
          </div>
        </div>
        <div uiCardContent>
          <div chartContainer [config]="dailyConfig" [spec]="dailySpec()" class="h-[320px] w-full"></div>
        </div>
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <!-- Top pages: horizontal bar -->
        <div uiCard>
          <div uiCardHeader>
            <div uiCardTitle>Most-visited pages</div>
            <div uiCardDescription>Where visitors spend the most attention across the portal.</div>
          </div>
          <div uiCardContent>
            <div chartContainer [config]="viewsConfig" [spec]="topPagesSpec()" class="h-[320px] w-full"></div>
          </div>
        </div>

        <!-- Traffic by section: pie -->
        <div uiCard>
          <div uiCardHeader>
            <div uiCardTitle>Traffic by section</div>
            <div uiCardDescription>Share of page views across major portal areas.</div>
          </div>
          <div uiCardContent>
            <div chartContainer [config]="sectionConfig" [spec]="sectionSpec()" class="h-[320px] w-full"></div>
          </div>
        </div>
      </div>

      <!-- Top applications: horizontal bar -->
      <div uiCard>
        <div uiCardHeader>
          <div uiCardTitle>Most-accessed application types</div>
          <div uiCardDescription>
            Which licensing applications visitors open most — a signal for demand and where to focus
            guidance and process improvements.
          </div>
        </div>
        <div uiCardContent>
          <div chartContainer [config]="opensConfig" [spec]="topAppsSpec()" class="h-[300px] w-full"></div>
        </div>
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <!-- Audience mix: pie -->
        <div uiCard>
          <div uiCardHeader>
            <div uiCardTitle>Audience mix</div>
            <div uiCardDescription>Sessions by portal role.</div>
          </div>
          <div uiCardContent>
            <div chartContainer [config]="sessionsConfig" [spec]="audienceSpec()" class="h-[260px] w-full"></div>
          </div>
        </div>

        <!-- Device split: pie -->
        <div uiCard>
          <div uiCardHeader>
            <div uiCardTitle>Device split</div>
            <div uiCardDescription>Sessions by device class.</div>
          </div>
          <div uiCardContent>
            <div chartContainer [config]="sessionsConfig" [spec]="deviceSpec()" class="h-[260px] w-full"></div>
          </div>
        </div>

        <!-- Acquisition: pie -->
        <div uiCard>
          <div uiCardHeader>
            <div uiCardTitle>How visitors arrive</div>
            <div uiCardDescription>Sessions by referral source.</div>
          </div>
          <div uiCardContent>
            <div chartContainer [config]="sessionsConfig" [spec]="referrerSpec()" class="h-[260px] w-full"></div>
          </div>
        </div>
      </div>

      <!-- New vs returning callout -->
      <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <div uiCard>
          <div uiCardContent class="p-5 flex items-center gap-4">
            <div class="rounded-lg bg-muted p-3">
              <svg lucide="FileText" class="h-6 w-6 text-[#013366]"></svg>
            </div>
            <div>
              <div class="text-2xl font-semibold text-foreground">{{ analytics().newVisitors.toLocaleString() }}</div>
              <div class="text-sm text-muted-foreground">New visitors (first-time)</div>
            </div>
          </div>
        </div>
        <div uiCard>
          <div uiCardContent class="p-5 flex items-center gap-4">
            <div class="rounded-lg bg-muted p-3">
              <svg lucide="Repeat" class="h-6 w-6 text-green-700"></svg>
            </div>
            <div>
              <div class="text-2xl font-semibold text-foreground">{{ analytics().returningVisitors.toLocaleString() }}</div>
              <div class="text-sm text-muted-foreground">Returning visitors</div>
            </div>
          </div>
        </div>
      </div>

      <p class="text-sm text-muted-foreground">
        Traffic is captured anonymously on each page view (rotating visitor id, session, coarse
        device class, and browsing role — no personal data). Figures cover the trailing 30 days.
      </p>
    </div>
  `,
})
export class SiteAnalyticsDashboard {
  readonly analytics = input.required<SiteAnalytics>();
  protected readonly trendUp = computed(() => this.analytics().visitorsTrendPct >= 0);

  protected readonly dailyConfig: ChartConfig = {
    visitors: { label: 'Visitors', color: 'var(--chart-1)' },
    pageviews: { label: 'Page views', color: 'var(--chart-3)' },
  };
  protected readonly viewsConfig: ChartConfig = { views: { label: 'Views', color: 'var(--chart-1)' } };
  protected readonly sectionConfig: ChartConfig = { views: { label: 'Views' } };
  protected readonly opensConfig: ChartConfig = { views: { label: 'Opens', color: 'var(--chart-2)' } };
  protected readonly sessionsConfig: ChartConfig = { sessions: { label: 'Sessions' } };

  protected readonly dailySpec = computed<BarChartSpec>(() => ({
    type: 'bar',
    data: this.analytics().daily,
    margin: { left: 8, right: 12, top: 8, bottom: 4 },
    categoryKey: 'label',
    categoryAxis: { tickLine: false, axisLine: false, tickFontSize: 11, interval: 3 },
    valueAxis: { allowDecimals: false, tickFontSize: 11, tickLine: false, axisLine: false },
    grid: { horizontal: true, vertical: false },
    legend: {},
    bars: [
      { dataKey: 'visitors', fill: 'var(--color-visitors)', radius: [3, 3, 0, 0] },
      { dataKey: 'pageviews', fill: 'var(--color-pageviews)', radius: [3, 3, 0, 0] },
    ],
  }));

  protected readonly topPagesSpec = computed<BarChartSpec>(() => ({
    type: 'bar',
    layout: 'vertical',
    data: this.analytics().topPages,
    margin: { left: 8, right: 24, top: 4, bottom: 4 },
    categoryKey: 'title',
    categoryAxis: { width: 150, tickFontSize: 11, tickLine: false, axisLine: false },
    valueAxis: { allowDecimals: false, tickFontSize: 11 },
    grid: { horizontal: false, vertical: true },
    bars: [{ dataKey: 'views', radius: [0, 4, 4, 0], cellFills: palette(this.analytics().topPages.length) }],
  }));

  protected readonly topAppsSpec = computed<BarChartSpec>(() => ({
    type: 'bar',
    layout: 'vertical',
    data: this.analytics().topApplications,
    margin: { left: 8, right: 24, top: 4, bottom: 4 },
    categoryKey: 'title',
    categoryAxis: { width: 220, tickFontSize: 11, tickLine: false, axisLine: false },
    valueAxis: { allowDecimals: false, tickFontSize: 11 },
    grid: { horizontal: false, vertical: true },
    bars: [{ dataKey: 'views', radius: [0, 4, 4, 0], cellFills: palette(this.analytics().topApplications.length) }],
  }));

  /** Pie without innerRadius (outerRadius 110) and a wrapping legend. */
  protected readonly sectionSpec = computed<PieChartSpec>(() => ({
    type: 'pie',
    data: this.analytics().bySection,
    dataKey: 'views',
    nameKey: 'label',
    outerRadius: 110,
    paddingAngle: 2,
    cellFills: palette(this.analytics().bySection.length),
    legend: { nameKey: 'label', className: 'flex-wrap' },
    tooltipNameKey: 'label',
  }));

  protected readonly audienceSpec = computed<PieChartSpec>(() => this.donut(this.analytics().byAudience));
  protected readonly deviceSpec = computed<PieChartSpec>(() => this.donut(this.analytics().byDevice));
  protected readonly referrerSpec = computed<PieChartSpec>(() => this.donut(this.analytics().byReferrer));

  /** The three sessions donuts: innerRadius 45, outerRadius 90, paddingAngle 2. */
  private donut(data: readonly object[]): PieChartSpec {
    return {
      type: 'pie',
      data,
      dataKey: 'sessions',
      nameKey: 'label',
      innerRadius: 45,
      outerRadius: 90,
      paddingAngle: 2,
      cellFills: palette(data.length),
      legend: { nameKey: 'label', className: 'flex-wrap' },
      tooltipNameKey: 'label',
    };
  }
}
