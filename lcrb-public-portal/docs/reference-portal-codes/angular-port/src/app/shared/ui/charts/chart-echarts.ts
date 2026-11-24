/** Tree-shaken echarts, loaded lazily: echarts and zrender call Math.random at load, which would shift the seeded cases on /lcrb/analytics. */
import { BarChart, PieChart } from 'echarts/charts';
import { GridComponent, TooltipComponent } from 'echarts/components';
import * as echarts from 'echarts/core';
import { SVGRenderer } from 'echarts/renderers';

echarts.use([BarChart, PieChart, GridComponent, TooltipComponent, SVGRenderer]);

export { echarts };
