/** Shadcn chart on ngx-echarts: the Recharts JSX children become a declarative `spec` translated into an ECharts option (SVG renderer, no animation) with the Recharts defaults applied explicitly. */
import { afterNextRender, ChangeDetectionStrategy, Component, computed, effect, ElementRef, inject, input, signal } from '@angular/core';
import type { EChartsCoreOption } from 'echarts/core';
import { NgxEchartsDirective, provideEchartsCore } from 'ngx-echarts';
import { cn } from '@shared/util/utils';


/** The prototype's ChartConfig (the `icon` and `theme` variants are not used by any chart). */
export type ChartConfig = Record<string, { label?: string; color?: string }>;

type Row = Record<string, unknown>;
const asRow = (r: object) => r as Row;

export interface ChartMargin {
  left: number;
  right: number;
  top: number;
  bottom: number;
}

export interface BarSeries {
  dataKey: string;
  stackId?: string;
  /** The `<Bar fill>`; omitted when each bar takes its colour from a `<Cell>`. */
  fill?: string;
  /** Recharts `radius` (top-left, top-right, bottom-right, bottom-left). */
  radius?: [number, number, number, number];
  /** One `<Cell fill>` per data row. */
  cellFills?: string[];
}

export interface AxisSpec {
  /** `tick={{ fontSize }}`; the container's text-xs (12) when omitted. */
  tickFontSize?: number;
  tickLine?: boolean;
  axisLine?: boolean;
  allowDecimals?: boolean;
  /** YAxis `width` (category axis of a vertical chart; default 60). */
  width?: number;
  /** XAxis `interval`. */
  interval?: number;
}

export interface BarChartSpec {
  type: 'bar';
  data: readonly object[];
  /** `layout="vertical"` draws horizontal bars with the category axis on the left. */
  layout?: 'horizontal' | 'vertical';
  margin: ChartMargin;
  /** The category axis `dataKey`. */
  categoryKey: string;
  categoryAxis?: AxisSpec;
  valueAxis?: AxisSpec;
  /** CartesianGrid `horizontal` / `vertical`. */
  grid: { horizontal: boolean; vertical: boolean };
  bars: BarSeries[];
  /** ChartLegend content (`className` and `nameKey`); omitted when the chart has no legend. */
  legend?: { className?: string; nameKey?: string };
  /** ChartTooltipContent `nameKey`. */
  tooltipNameKey?: string;
}

export interface PieChartSpec {
  type: 'pie';
  data: readonly object[];
  dataKey: string;
  nameKey: string;
  innerRadius?: number;
  outerRadius: number;
  paddingAngle: number;
  cellFills: string[];
  legend?: { className?: string; nameKey?: string };
  tooltipNameKey?: string;
}

export type ChartSpec = BarChartSpec | PieChartSpec;

/** A Recharts tooltip / legend payload entry. */
export interface PayloadItem {
  dataKey?: string;
  name?: string;
  value?: unknown;
  color?: string;
  payload?: Row;
  [key: string]: unknown;
}

export interface ChartTheme {
  fontFamily: string;
  /** `fill-muted-foreground` */
  text: string;
  /** `stroke-border/50` */
  grid: string;
  /** `fill-muted` (tooltip cursor) */
  cursor: string;
}

// Ports of getPayloadConfigFromPayload and the tooltip / legend markup.

/** Helper to extract item config from a payload (verbatim logic of the prototype). */
export function getPayloadConfigFromPayload(config: ChartConfig, payload: unknown, key: string) {
  if (typeof payload !== 'object' || payload === null) return undefined;
  const p = payload as Record<string, unknown>;
  const payloadPayload = typeof p['payload'] === 'object' && p['payload'] !== null ? (p['payload'] as Record<string, unknown>) : undefined;
  let configLabelKey: string = key;
  if (key in p && typeof p[key] === 'string') {
    configLabelKey = p[key] as string;
  } else if (payloadPayload && key in payloadPayload && typeof payloadPayload[key] === 'string') {
    configLabelKey = payloadPayload[key] as string;
  }
  return configLabelKey in config ? config[configLabelKey] : config[key];
}

function esc(value: unknown): string {
  return String(value)
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;')
    .replace(/"/g, '&quot;');
}

/** ChartTooltipContent (indicator "dot", no formatter), as an HTML string for the ECharts tooltip. */
export function chartTooltipHtml(config: ChartConfig, payload: PayloadItem[], label: unknown, nameKey?: string): string {
  if (!payload.length) return '';
  const first = payload[0];
  const labelKey = undefined as string | undefined;
  const lookupKey = `${labelKey || first.dataKey || first.name || 'value'}`;
  const labelConfig = getPayloadConfigFromPayload(config, first, lookupKey);
  const labelValue = typeof label === 'string' ? config[label]?.label || label : labelConfig?.label;
  const tooltipLabel = labelValue ? `<div class="font-medium">${esc(labelValue)}</div>` : '';

  const rows = payload
    .map((item) => {
      const key = `${nameKey || item.name || item.dataKey || 'value'}`;
      const itemConfig = getPayloadConfigFromPayload(config, item, key);
      const indicatorColor = (item.payload?.['fill'] as string | undefined) || item.color;
      const rowClass = cn('[&>svg]:text-muted-foreground flex w-full flex-wrap items-stretch gap-2 [&>svg]:h-2.5 [&>svg]:w-2.5', 'items-center');
      const indicatorClass = cn('shrink-0 rounded-[2px] border-(--color-border) bg-(--color-bg)', 'h-2.5 w-2.5');
      const innerClass = cn('flex flex-1 justify-between leading-none', 'items-center');
      const value = item.value
        ? `<span class="text-foreground font-mono font-medium tabular-nums">${esc(Number(item.value).toLocaleString())}</span>`
        : '';
      return (
        `<div class="${rowClass}">` +
        `<div class="${indicatorClass}" style="--color-bg:${esc(indicatorColor ?? '')};--color-border:${esc(indicatorColor ?? '')}"></div>` +
        `<div class="${innerClass}"><div class="grid gap-1.5"><span class="text-muted-foreground">${esc(itemConfig?.label || item.name || '')}</span></div>${value}</div>` +
        `</div>`
      );
    })
    .join('');

  return (
    `<div class="border-border/50 bg-background grid min-w-[8rem] items-start gap-1.5 rounded-lg border px-2.5 py-1.5 text-xs shadow-xl">` +
    tooltipLabel +
    `<div class="grid gap-1.5">${rows}</div></div>`
  );
}

export interface LegendEntry {
  value: string;
  color: string;
  label: string | undefined;
}

/** ChartLegendContent entries: one per series (bar) or per slice (pie). */
export function chartLegendItems(spec: ChartSpec, config: ChartConfig): LegendEntry[] {
  if (!spec.legend) return [];
  const nameKey = spec.legend.nameKey;
  let payload: PayloadItem[];
  if (spec.type === 'bar') {
    payload = spec.bars.map((b) => ({ value: b.dataKey, dataKey: b.dataKey, color: b.fill ?? '', payload: { dataKey: b.dataKey } }));
  } else {
    payload = spec.data.map((r, i) => ({
      value: String(asRow(r)[spec.nameKey]),
      color: spec.cellFills[i],
      payload: { ...asRow(r), fill: spec.cellFills[i] },
    }));
  }
  return payload.map((item) => {
    const key = `${nameKey || item.dataKey || 'value'}`;
    const itemConfig = getPayloadConfigFromPayload(config, item, key);
    return { value: String(item.value), color: item.color ?? '', label: itemConfig?.label };
  });
}

// Recharts paints `var(--chart-1)` and `var(--color-x)` through CSS; ECharts needs concrete colours, so the variables are resolved against the page.

export function resolveChartColor(value: string, config: ChartConfig, root: CSSStyleDeclaration | null, depth = 0): string {
  const match = /^var\((--[\w-]+)\)$/.exec(value.trim());
  if (!match || depth > 6) return value;
  const name = match[1];
  if (name.startsWith('--color-')) {
    const configured = config[name.slice('--color-'.length)]?.color;
    if (configured) return resolveChartColor(configured, config, root, depth + 1);
  }
  const raw = root?.getPropertyValue(name).trim();
  return raw ? resolveChartColor(raw, config, root, depth + 1) : value;
}

/** `stroke-border/50`: the colour at 50% alpha. */
export function withAlpha(color: string, alpha: number): string {
  const hex = /^#([0-9a-f]{6})$/i.exec(color.trim());
  if (!hex) return color;
  const n = parseInt(hex[1], 16);
  return `rgba(${(n >> 16) & 255}, ${(n >> 8) & 255}, ${n & 255}, ${alpha})`;
}



// Port of recharts-scale getNiceTickValues for a number axis with domain [0, 'auto'] and tickCount 5 (Recharts 2.15).

const digitCount = (v: number) => (v === 0 ? 1 : Math.floor(Math.log10(Math.abs(v))) + 1);
const round = (v: number) => Math.round(v * 1e10) / 1e10;

function formatStep(rough: number, allowDecimals: boolean, correction: number): number {
  if (rough <= 0) return 0;
  const digits = digitCount(rough);
  const base = Math.pow(10, digits);
  const ratio = rough / base;
  const scale = digits !== 1 ? 0.05 : 0.1;
  const amended = round((Math.ceil(round(ratio / scale)) + correction) * scale);
  const step = round(amended * base);
  return allowDecimals ? step : Math.ceil(step);
}

/** Ticks (min, max, step) for the domain [0, dataMax]. */
export function niceTicks(dataMax: number, tickCount = 5, allowDecimals = true): { min: number; max: number; step: number } {
  const count = Math.max(tickCount, 2);
  const min = 0;
  const max = dataMax;
  if (!Number.isFinite(max) || max <= min) return { min: 0, max: 1, step: 1 };
  for (let correction = 0; correction < 50; correction++) {
    const step = formatStep((max - min) / (count - 1), allowDecimals, correction);
    const middle = 0; // min <= 0 && max >= 0
    const below = Math.ceil((middle - min) / step);
    let up = Math.ceil(round((max - middle) / step));
    const scaleCount = below + up + 1;
    if (scaleCount > count) continue;
    if (scaleCount < count) up += count - scaleCount;
    return { min: round((middle - below) * step), max: round((middle + up) * step), step };
  }
  return { min: 0, max, step: max };
}

/** The data maximum of a bar chart's value axis (stacked series add up). */
function barDataMax(spec: BarChartSpec): number {
  let max = 0;
  for (const raw of spec.data) {
    const row = asRow(raw);
    const stacks = new Map<string, number>();
    for (const b of spec.bars) {
      const v = Number(row[b.dataKey]) || 0;
      if (b.stackId) stacks.set(b.stackId, (stacks.get(b.stackId) ?? 0) + v);
      else max = Math.max(max, v);
    }
    for (const v of stacks.values()) max = Math.max(max, v);
  }
  return max;
}


let measureCtx: CanvasRenderingContext2D | null | undefined;
/** Width of a label in px: canvas measurement in browsers, a rough estimate elsewhere (jsdom). */
function textWidth(text: string, fontSize: number, fontFamily: string): number {
  if (measureCtx === undefined) {
    measureCtx = typeof navigator !== 'undefined' && /jsdom/i.test(navigator.userAgent) ? null : document.createElement('canvas').getContext('2d');
  }
  if (!measureCtx) return text.length * fontSize * 0.55;
  measureCtx.font = `${fontSize}px ${fontFamily}`;
  return measureCtx.measureText(text).width;
}

/** Recharts' default XAxis `interval="preserveEnd"`: keep a tick only if its label clears the last kept one by `minTickGap` (5px); returns the step. */
function preserveEndStep(labels: string[], band: number, fontSize: number, fontFamily: string): number {
  const widths = labels.map((l) => textWidth(l, fontSize, fontFamily));
  for (let step = 1; step < labels.length; step++) {
    let ok = true;
    for (let i = labels.length - 1; i - step >= 0; i -= step) {
      if (widths[i] / 2 + widths[i - step] / 2 + 5 > step * band) {
        ok = false;
        break;
      }
    }
    if (ok) return step;
  }
  return labels.length;
}

const AXIS_STROKE = '#666'; // Recharts CartesianAxis default
const TICK_SIZE = 6; // Recharts default tickSize
const TICK_MARGIN = 2; // Recharts default tickMargin
const X_AXIS_HEIGHT = 30; // Recharts default XAxis height
const Y_AXIS_WIDTH = 60; // Recharts default YAxis width
const DEFAULT_FONT_SIZE = 12; // text-xs on the container

function axisCommon(axis: AxisSpec | undefined, theme: ChartTheme) {
  const tickLine = axis?.tickLine ?? true;
  return {
    axisLine: { show: axis?.axisLine ?? true, lineStyle: { color: AXIS_STROKE, width: 1 } },
    axisTick: {
      show: tickLine,
      length: TICK_SIZE,
      alignWithLabel: true,
      lineStyle: { color: AXIS_STROKE, width: 1 },
    },
    axisLabel: {
      color: theme.text,
      fontFamily: theme.fontFamily,
      fontSize: axis?.tickFontSize ?? DEFAULT_FONT_SIZE,
      // Recharts offsets the label by tickSize + tickMargin whether or not the tick line is drawn.
      margin: tickLine ? TICK_MARGIN : TICK_SIZE + TICK_MARGIN,
      hideOverlap: false,
    },
  };
}

export function buildChartOption(
  spec: ChartSpec,
  config: ChartConfig,
  theme: ChartTheme,
  root: CSSStyleDeclaration | null,
  /** The chart's rendered width in px, used for the category tick interval. */
  width?: number,
): EChartsCoreOption {
  const resolve = (c: string | undefined) => (c ? resolveChartColor(c, config, root) : '#8884d8');
  const base = {
    animation: false,
    textStyle: { fontFamily: theme.fontFamily, fontSize: DEFAULT_FONT_SIZE, color: theme.text },
  };
  const tooltipBase = {
    transitionDuration: 0,
    backgroundColor: 'transparent',
    borderWidth: 0,
    padding: 0,
    extraCssText: 'box-shadow:none;font-family:inherit;',
    position: (pt: number[]) => [pt[0] + 10, pt[1] + 10],
  };

  if (spec.type === 'pie') {
    return {
      ...base,
      tooltip: {
        ...tooltipBase,
        trigger: 'item',
        formatter: (p: { dataIndex: number }) => {
          const row = asRow(spec.data[p.dataIndex]);
          const fill = resolve(spec.cellFills[p.dataIndex]);
          return chartTooltipHtml(
            config,
            [{ dataKey: spec.dataKey, name: String(row[spec.nameKey]), value: row[spec.dataKey], color: fill, payload: { ...row, fill } }],
            undefined,
            spec.tooltipNameKey,
          );
        },
      },
      series: [
        {
          type: 'pie',
          center: ['50%', '50%'],
          radius: [spec.innerRadius ?? 0, spec.outerRadius],
          // Recharts starts at 3 o'clock and sweeps counter-clockwise.
          startAngle: 0,
          clockwise: false,
          padAngle: spec.paddingAngle,
          label: { show: false },
          labelLine: { show: false },
          itemStyle: { borderWidth: 0 },
          emphasis: { disabled: true },
          selectedMode: false,
          animation: false,
          data: spec.data.map((r, i) => ({
            name: String(asRow(r)[spec.nameKey]),
            value: asRow(r)[spec.dataKey] as number,
            itemStyle: { color: resolve(spec.cellFills[i]) },
          })),
        },
      ],
    };
  }

  const vertical = spec.layout === 'vertical';
  const categories = spec.data.map((r) => String(asRow(r)[spec.categoryKey]));
  const grid = vertical
    ? { left: spec.margin.left + (spec.categoryAxis?.width ?? Y_AXIS_WIDTH), right: spec.margin.right, top: spec.margin.top, bottom: spec.margin.bottom + X_AXIS_HEIGHT }
    : { left: spec.margin.left + (spec.valueAxis?.width ?? Y_AXIS_WIDTH), right: spec.margin.right, top: spec.margin.top, bottom: spec.margin.bottom + X_AXIS_HEIGHT };

  const catCommon = axisCommon(spec.categoryAxis, theme);
  const valCommon = axisCommon(spec.valueAxis, theme);
  const dashed = { show: true, lineStyle: { color: theme.grid, type: [3, 3], width: 1 } };
  const categoryAxis = {
    type: 'category',
    data: categories,
    inverse: vertical,
    boundaryGap: true,
    axisLine: catCommon.axisLine,
    axisTick: catCommon.axisTick,
    axisLabel: {
      ...catCommon.axisLabel,
      ...(spec.categoryAxis?.interval !== undefined
        ? { interval: spec.categoryAxis.interval }
        : !vertical && width
          ? (() => {
              const n = categories.length;
              const plot = width - grid.left - grid.right;
              const step = preserveEndStep(categories, plot / n, spec.categoryAxis?.tickFontSize ?? DEFAULT_FONT_SIZE, theme.fontFamily);
              return { interval: (i: number) => (n - 1 - i) % step === 0 };
            })()
          : {}),
      // Recharts wraps a YAxis tick to the axis width but measures the words at the 16px default font while painting them at `fontSize`, so the effective wrap width is width * fontSize / 16.
      ...(vertical
        ? {
            width: Math.round(((spec.categoryAxis?.width ?? Y_AXIS_WIDTH) * (spec.categoryAxis?.tickFontSize ?? DEFAULT_FONT_SIZE)) / 16),
            overflow: 'break',
            lineHeight: spec.categoryAxis?.tickFontSize ?? DEFAULT_FONT_SIZE,
            align: 'right',
          }
        : {}),
    },
    splitLine: { show: false },
  };
  const nice = niceTicks(barDataMax(spec), 5, spec.valueAxis?.allowDecimals !== false);
  const valueAxis = {
    type: 'value',
    min: nice.min,
    max: nice.max,
    interval: nice.step,
    axisLine: valCommon.axisLine,
    axisTick: valCommon.axisTick,
    axisLabel: valCommon.axisLabel,
    splitLine: vertical ? (spec.grid.vertical ? dashed : { show: false }) : spec.grid.horizontal ? dashed : { show: false },
  };

  return {
    ...base,
    grid: { ...grid, outerBoundsMode: 'none' },
    xAxis: vertical ? valueAxis : categoryAxis,
    yAxis: vertical ? categoryAxis : valueAxis,
    tooltip: {
      ...tooltipBase,
      trigger: 'axis',
      axisPointer: { type: 'shadow', shadowStyle: { color: theme.cursor, opacity: 1 } },
      formatter: (params: { dataIndex: number } | { dataIndex: number }[]) => {
        const idx = (Array.isArray(params) ? params[0] : params).dataIndex;
        const row = asRow(spec.data[idx]);
        const items: PayloadItem[] = spec.bars.map((b) => ({
          dataKey: b.dataKey,
          name: b.dataKey,
          value: row[b.dataKey],
          color: resolve(b.cellFills ? b.cellFills[idx] : b.fill),
          payload: row,
        }));
        return chartTooltipHtml(config, items, String(row[spec.categoryKey]), spec.tooltipNameKey);
      },
    },
    series: spec.bars.map((b) => ({
      type: 'bar',
      name: b.dataKey,
      stack: b.stackId,
      barCategoryGap: '20%',
      // Recharts barGap is 4px between series (about a third of a bar at the page's chart widths).
      barGap: '33%',
      emphasis: { disabled: true },
      animation: false,
      itemStyle: { color: resolve(b.fill), ...(b.radius ? { borderRadius: b.radius } : {}) },
      data: spec.data.map((r, i) => ({
        value: asRow(r)[b.dataKey] as number,
        ...(b.cellFills ? { itemStyle: { color: resolve(b.cellFills[i]), ...(b.radius ? { borderRadius: b.radius } : {}) } } : {}),
      })),
    })),
  };
}


let chartSeq = 0;

@Component({
  selector: 'div[chartContainer]',
  imports: [NgxEchartsDirective],
  // Lazy: echarts draws Math.random at module load (see chart-echarts.ts).
  providers: [provideEchartsCore({ echarts: () => import('./chart-echarts').then((m) => m.echarts) })],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: {
    'data-slot': 'chart',
    '[attr.data-chart]': 'chartId',
    '[class]': 'hostClass()',
  },
  template: `
    <div style="width: 100%; height: 100%; min-width: 0; display: flex; flex-direction: column">
      @if (option(); as opt) {
        <div echarts [options]="opt" [initOpts]="initOpts" style="flex: 1 1 0%; min-height: 0; width: 100%"></div>
      }
      @if (legend().length > 0) {
        <div [class]="legendClass()" [style.transform]="'translateY(-' + legendOffset() + 'px)'">
          @for (item of legend(); track item.value) {
            <div class="[&>svg]:text-muted-foreground flex items-center gap-1.5 [&>svg]:h-3 [&>svg]:w-3">
              <div class="h-2 w-2 shrink-0 rounded-[2px]" [style.backgroundColor]="item.color"></div>
              {{ item.label }}
            </div>
          }
        </div>
      }
    </div>
  `,
})
export class ChartContainer {
  readonly config = input.required<ChartConfig>();
  readonly spec = input.required<ChartSpec>();
  readonly userClass = input<string | undefined>(undefined, { alias: 'class' });

  protected readonly chartId = `chart-${++chartSeq}`;
  protected readonly initOpts = { renderer: 'svg' as const };
  private readonly el = inject<ElementRef<HTMLElement>>(ElementRef).nativeElement;
  private readonly ready = signal(false);

  protected readonly hostClass = computed(() =>
    cn(
      "[&_.recharts-cartesian-axis-tick_text]:fill-muted-foreground [&_.recharts-cartesian-grid_line[stroke='#ccc']]:stroke-border/50 [&_.recharts-curve.recharts-tooltip-cursor]:stroke-border [&_.recharts-polar-grid_[stroke='#ccc']]:stroke-border [&_.recharts-radial-bar-background-sector]:fill-muted [&_.recharts-rectangle.recharts-tooltip-cursor]:fill-muted [&_.recharts-reference-line_[stroke='#ccc']]:stroke-border flex aspect-video justify-center text-xs [&_.recharts-dot[stroke='#fff']]:stroke-transparent [&_.recharts-layer]:outline-hidden [&_.recharts-sector]:outline-hidden [&_.recharts-sector[stroke='#fff']]:stroke-transparent [&_.recharts-surface]:outline-hidden",
      this.userClass(),
    ),
  );

  protected readonly legend = computed(() => chartLegendItems(this.spec(), this.config()));
  /** Recharts places the legend wrapper at `bottom: margin.bottom` (pie charts have the 5px default margin). */
  protected readonly legendOffset = computed(() => {
    const spec = this.spec();
    return spec.type === 'pie' ? 5 : spec.margin.bottom;
  });
  protected readonly legendClass = computed(() => cn('flex items-center justify-center gap-4', 'pt-3', this.spec().legend?.className));

  /** The ECharts option, built after the first render so the page's CSS variables can be resolved. */
  protected readonly option = computed<EChartsCoreOption | null>(() => {
    if (!this.ready()) return null;
    const root = getComputedStyle(document.documentElement);
    const text = root.getPropertyValue('--muted-foreground').trim() || '#474543';
    const border = root.getPropertyValue('--border').trim() || '#D9D9D9';
    const muted = root.getPropertyValue('--muted').trim() || '#F7F7F7';
    const theme: ChartTheme = {
      fontFamily: getComputedStyle(this.el).fontFamily || 'sans-serif',
      text,
      grid: withAlpha(border, 0.5),
      cursor: muted,
    };
    return buildChartOption(this.spec(), this.config(), theme, root, this.el.clientWidth || undefined);
  });

  constructor() {
    // ChartStyle: `--color-<key>` for every configured colour.
    effect(() => {
      for (const [key, item] of Object.entries(this.config())) {
        if (item.color) this.el.style.setProperty(`--color-${key}`, item.color);
      }
    });
    afterNextRender(() => this.ready.set(true));
  }
}
