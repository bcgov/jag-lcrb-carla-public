/** Specs for the ECharts-based chart: options, tooltip, legend and colours. */
import { Component } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { NgxEchartsDirective } from 'ngx-echarts';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import {
  type BarChartSpec,
  buildChartOption,
  type ChartConfig,
  ChartContainer,
  chartLegendItems,
  chartTooltipHtml,
  type ChartTheme,
  getPayloadConfigFromPayload,
  niceTicks,
  type PieChartSpec,
  resolveChartColor,
  withAlpha,
} from './chart';

const theme: ChartTheme = { fontFamily: 'Inter', text: '#474543', grid: 'rgba(217, 217, 217, 0.5)', cursor: '#F7F7F7' };
const root = { getPropertyValue: (n: string) => ({ '--chart-4': '#2E8540', '--chart-3': '#FCBA19', '--chart-5': '#CE3E39' })[n] ?? '' } as unknown as CSSStyleDeclaration;

const outcomeConfig: ChartConfig = {
  clear: { label: 'Clear', color: 'var(--chart-4)' },
  attention: { label: 'Attention', color: 'var(--chart-3)' },
  flag: { label: 'Flag', color: 'var(--chart-5)' },
};
const outcomeSpec: BarChartSpec = {
  type: 'bar',
  data: [
    { label: 'Fit', clear: 3, attention: 1, flag: 0 },
    { label: 'Cap', clear: 2, attention: 0, flag: 2 },
  ],
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
};
const queueSpec: BarChartSpec = {
  type: 'bar',
  layout: 'vertical',
  data: [
    { queue: 'a', label: 'Alpha', count: 4 },
    { queue: 'b', label: 'Beta', count: 0 },
  ],
  margin: { left: 8, right: 16, top: 4, bottom: 4 },
  categoryKey: 'label',
  categoryAxis: { width: 130, tickFontSize: 11, tickLine: false, axisLine: false },
  valueAxis: { allowDecimals: false },
  grid: { horizontal: false, vertical: true },
  bars: [{ dataKey: 'count', radius: [0, 4, 4, 0], cellFills: ['var(--chart-5)', 'var(--chart-3)'] }],
};
const pieConfig: ChartConfig = { high: { label: 'High (80+)', color: 'var(--chart-4)' } };
const pieSpec: PieChartSpec = {
  type: 'pie',
  data: [
    { band: 'high', label: 'High (80+)', count: 5 },
    { band: 'low', label: 'Low (<55)', count: 1 },
  ],
  dataKey: 'count',
  nameKey: 'label',
  innerRadius: 60,
  outerRadius: 100,
  paddingAngle: 2,
  cellFills: ['var(--chart-4)', 'var(--chart-5)'],
  legend: { nameKey: 'label' },
  tooltipNameKey: 'label',
};

describe('colour helpers', () => {
  it('resolves var() chains through the chart config then the page', () => {
    expect(resolveChartColor('var(--color-clear)', outcomeConfig, root)).toBe('#2E8540');
    expect(resolveChartColor('var(--chart-3)', {}, root)).toBe('#FCBA19');
    expect(resolveChartColor('#7A9CC6', {}, root)).toBe('#7A9CC6');
    expect(withAlpha('#D9D9D9', 0.5)).toBe('rgba(217, 217, 217, 0.5)');
  });
});

describe('buildChartOption (bar, horizontal)', () => {
  const opt = buildChartOption(outcomeSpec, outcomeConfig, theme, root) as Record<string, any>;
  it('applies the Recharts margins, YAxis width 60 and XAxis height 30 to the grid', () => {
    expect(opt['grid']).toMatchObject({ left: 68, right: 16, top: 4, bottom: 34 });
    expect(opt['animation']).toBe(false);
  });
  it('builds a stacked series per bar with resolved colours and the flag radius', () => {
    const series = opt['series'] as any[];
    expect(series.map((s) => [s.name, s.stack, s.itemStyle.color])).toEqual([
      ['clear', 'a', '#2E8540'],
      ['attention', 'a', '#FCBA19'],
      ['flag', 'a', '#CE3E39'],
    ]);
    expect(series[2].itemStyle.borderRadius).toEqual([4, 4, 0, 0]);
    expect(series[1].itemStyle.borderRadius).toBeUndefined();
    expect(series[0].data.map((d: any) => d.value)).toEqual([3, 2]);
    expect(series[0].barCategoryGap).toBe('20%');
  });
  it('draws only horizontal dashed grid lines on the value axis, none on the category axis', () => {
    expect(opt['yAxis'].splitLine).toMatchObject({ show: true, lineStyle: { type: [3, 3], color: theme.grid } });
    expect(opt['xAxis'].splitLine).toEqual({ show: false });
    expect(opt['yAxis']).toMatchObject({ min: 0, max: 4, interval: 1 });
    expect(opt['xAxis'].axisLine.show).toBe(false);
    expect(opt['xAxis'].axisTick.show).toBe(false);
    expect(opt['xAxis'].axisLabel.margin).toBe(8);
    expect(opt['yAxis'].axisLine).toMatchObject({ show: true, lineStyle: { color: '#666' } });
    expect(opt['yAxis'].axisLabel.margin).toBe(2);
  });
});

describe('niceTicks (recharts-scale)', () => {
  it('reproduces the Recharts domains seen in the prototype', () => {
    expect(niceTicks(2, 5, false)).toEqual({ min: 0, max: 4, step: 1 });
    expect(niceTicks(4, 5, false)).toEqual({ min: 0, max: 4, step: 1 });
    expect(niceTicks(150, 5, false)).toEqual({ min: 0, max: 160, step: 40 });
    expect(niceTicks(310, 5, false)).toEqual({ min: 0, max: 320, step: 80 });
    expect(niceTicks(134, 5, false)).toEqual({ min: 0, max: 140, step: 35 });
  });
});

describe('buildChartOption (bar, vertical layout)', () => {
  const opt = buildChartOption(queueSpec, {}, theme, root) as Record<string, any>;
  it('puts the category axis on the left with its width, first row at the top', () => {
    expect(opt['grid']).toMatchObject({ left: 138, right: 16, top: 4, bottom: 34 });
    expect(opt['yAxis']).toMatchObject({ type: 'category', inverse: true, data: ['Alpha', 'Beta'] });
    expect(opt['yAxis'].axisLabel.fontSize).toBe(11);
    expect(opt['xAxis']).toMatchObject({ type: 'value', min: 0, max: 4, interval: 1 });
    expect(opt['yAxis'].axisLabel).toMatchObject({ width: 89, overflow: 'break', lineHeight: 11, align: 'right' });
    expect(opt['xAxis'].splitLine.show).toBe(true);
    expect(opt['yAxis'].splitLine.show).toBe(false);
  });
  it('colours each bar from its cell with the right-hand radius', () => {
    const [s] = opt['series'] as any[];
    expect(s.data.map((d: any) => d.itemStyle.color)).toEqual(['#CE3E39', '#FCBA19']);
    expect(s.data[0].itemStyle.borderRadius).toEqual([0, 4, 4, 0]);
  });
});

describe('buildChartOption (pie)', () => {
  const opt = buildChartOption(pieSpec, pieConfig, theme, root) as Record<string, any>;
  it('uses the donut radii, padding angle, counter-clockwise sweep from 3 o clock and no labels', () => {
    const [s] = opt['series'] as any[];
    expect(s).toMatchObject({ type: 'pie', radius: [60, 100], padAngle: 2, startAngle: 0, clockwise: false, label: { show: false } });
    expect(s.data).toEqual([
      { name: 'High (80+)', value: 5, itemStyle: { color: '#2E8540' } },
      { name: 'Low (<55)', value: 1, itemStyle: { color: '#CE3E39' } },
    ]);
  });
});

describe('tooltip markup (ChartTooltipContent)', () => {
  it('bar tooltips: category header, series label from config, value, indicator colour', () => {
    const opt = buildChartOption(outcomeSpec, outcomeConfig, theme, root) as any;
    const html: string = opt.tooltip.formatter([{ dataIndex: 1 }]);
    expect(html).toContain('<div class="font-medium">Cap</div>');
    expect(html).toContain('>Clear</span>');
    expect(html).toContain('>Flag</span>');
    expect(html).toContain('--color-bg:#2E8540');
    expect(html).toContain('text-foreground font-mono font-medium tabular-nums">2</span>');
    // A zero value shows no number (the prototype's `item.value &&`).
    expect(html.match(/tabular-nums/g)!.length).toBe(2);
    expect(html).toContain('border-border/50 bg-background grid min-w-[8rem] items-start gap-1.5 rounded-lg border px-2.5 py-1.5 text-xs shadow-xl');
  });
  it('pie tooltips: slice name, config-less header, formatted number', () => {
    const opt = buildChartOption(pieSpec, pieConfig, theme, root) as any;
    const html: string = opt.tooltip.formatter({ dataIndex: 0 });
    expect(html).toContain('>High (80+)</span>');
    expect(html).toContain('--color-bg:#2E8540');
    expect(html).not.toContain('font-medium">');
  });
  it('escapes markup and falls back to the item name', () => {
    expect(chartTooltipHtml({}, [{ dataKey: 'x', name: '<b>', value: 1234 }], undefined)).toContain('&lt;b&gt;');
    expect(chartTooltipHtml({}, [{ dataKey: 'x', name: 'n', value: 1234 }], undefined)).toContain('1,234');
    expect(chartTooltipHtml({}, [], 'x')).toBe('');
  });
  it('getPayloadConfigFromPayload follows the prototype lookup order', () => {
    const cfg: ChartConfig = { count: { label: 'Cases' }, Alpha: { label: 'A' } };
    expect(getPayloadConfigFromPayload(cfg, { name: 'count' }, 'name')?.label).toBe('Cases');
    expect(getPayloadConfigFromPayload(cfg, { payload: { label: 'Alpha' } }, 'label')?.label).toBe('A');
    expect(getPayloadConfigFromPayload(cfg, { payload: { label: 'Zed' } }, 'label')).toBeUndefined();
    expect(getPayloadConfigFromPayload(cfg, null, 'x')).toBeUndefined();
  });
});

describe('legend entries (ChartLegendContent)', () => {
  it('bar legends take the label from the config for each series', () => {
    expect(chartLegendItems(outcomeSpec, outcomeConfig)).toEqual([
      { value: 'clear', color: 'var(--color-clear)', label: 'Clear' },
      { value: 'attention', color: 'var(--color-attention)', label: 'Attention' },
      { value: 'flag', color: 'var(--color-flag)', label: 'Flag' },
    ]);
  });
  it('pie legends keep the prototype lookup (nameKey "label" resolves to the slice label, not a config key)', () => {
    const items = chartLegendItems(pieSpec, pieConfig);
    expect(items.map((i) => i.value)).toEqual(['High (80+)', 'Low (<55)']);
    expect(items.map((i) => i.color)).toEqual(['var(--chart-4)', 'var(--chart-5)']);
    expect(items.map((i) => i.label)).toEqual([undefined, undefined]);
  });
  it('no legend when the spec has none', () => {
    expect(chartLegendItems(queueSpec, {})).toEqual([]);
  });
});

@Component({
  imports: [ChartContainer],
  template: `<div chartContainer [config]="config" [spec]="spec" class="h-[300px] w-full"></div>`,
})
class Host {
  config = outcomeConfig;
  spec = outcomeSpec;
}

describe('ChartContainer', () => {
  beforeEach(() => {
    vi.stubGlobal('ResizeObserver', class { observe() {} unobserve() {} disconnect() {} });
    // Keep the specs about the data and options: jsdom cannot draw (no canvas text measuring).
    vi.spyOn(NgxEchartsDirective.prototype as any, 'initChart').mockResolvedValue(undefined);
    vi.spyOn(NgxEchartsDirective.prototype as any, 'onOptionsChange').mockResolvedValue(undefined);
  });
  afterEach(() => {
    vi.unstubAllGlobals();
    vi.restoreAllMocks();
  });

  it('renders the container classes, colour variables, legend and an SVG-renderer echarts host', async () => {
    const fixture = TestBed.createComponent(Host);
    for (let i = 0; i < 3; i++) {
      fixture.detectChanges();
      await fixture.whenStable();
    }
    const el = (fixture.nativeElement as HTMLElement).querySelector('div[chartContainer]') as HTMLElement;
    expect(el.getAttribute('data-slot')).toBe('chart');
    expect(el.getAttribute('data-chart')).toMatch(/^chart-\d+$/);
    expect(el.className).toMatch(/aspect-video flex justify-center text-xs/);
    expect(el.className).toContain('h-[300px] w-full');
    expect(el.style.getPropertyValue('--color-clear')).toBe('var(--chart-4)');
    expect([...el.querySelectorAll('.rounded-\\[2px\\]')].map((d) => (d as HTMLElement).style.backgroundColor)).toEqual([
      'var(--color-clear)',
      'var(--color-attention)',
      'var(--color-flag)',
    ]);
    expect(el.textContent).toContain('Clear');
    expect(el.textContent).toContain('Flag');
    const directive = fixture.debugElement.query(By.directive(NgxEchartsDirective)).injector.get(NgxEchartsDirective);
    expect(directive.initOpts()).toEqual({ renderer: 'svg' });
    expect((directive.options() as any).series).toHaveLength(3);
    fixture.destroy();
  });
});
