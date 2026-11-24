/** Marker popup: licence details, role-gated staff intelligence, the police/local-government view, the public complaint button and application intelligence. */
import { ChangeDetectionStrategy, Component, computed, input, output, signal } from '@angular/core';
import { ComplaintDialog } from './complaint-dialog';
import { Badge } from '../primitives/badge';
import { Button } from '../primitives/button';
import type { IconName } from '../primitives/icons';
import { LucideIcon } from '../primitives/lucide-icon';
import { Separator } from '../primitives/separator';
import { Textarea } from '../primitives/textarea';
import { cn } from '@shared/util/utils';
import { calculateRiskScore, calculateSalesTrend, formatApplicationAge, getAnnualSales, getApplicationAgeDays, getApplicationPhaseDisplayName, getApplicationTypeDisplayName, getCurrentMonthSales } from '@domain/map/license-data';
import { type License, type MonthlySales } from '@domain/models/map';

const AI_INSPECTION_SUMMARY = 'Check social responsibility posters and check that the owner has a Selling it Right and Serving it Right.';

const ONLINE_INTELLIGENCE_FINDINGS: Array<{
  id: number;
  type: string;
  severity: 'high' | 'medium' | 'low';
  source: string;
  description: string;
  date: string;
  icon: IconName;
}> = [
  {
    id: 1,
    type: 'website',
    severity: 'high',
    source: 'Business Website',
    description: 'Menu does not display drink quantities as required by regulation',
    date: '2024-01-15',
    icon: 'Globe',
  },
  {
    id: 2,
    type: 'social',
    severity: 'medium',
    source: 'Instagram',
    description: 'Post suggests after-hours event at 3am on Dec 31',
    date: '2024-01-02',
    icon: 'Eye',
  },
  {
    id: 3,
    type: 'review',
    severity: 'low',
    source: 'Google Reviews',
    description: 'Customer mentions being served without ID check',
    date: '2023-12-28',
    icon: 'MessageSquareWarning',
  },
];

function analyzeSalesIntelligence(sales: MonthlySales[] | undefined, contraventions: number, complaints: number) {
  if (!sales || sales.length < 6) return null;

  const currentMonth = getCurrentMonthSales(sales);
  const annualTotal = getAnnualSales(sales);
  const trend = calculateSalesTrend(sales);

  // Calculate historical average (excluding current month)
  const historicalAvg = sales.slice(0, -1).reduce((sum, s) => sum + s.amount, 0) / (sales.length - 1);

  // Calculate standard deviation for anomaly detection
  const variance = sales.slice(0, -1).reduce((sum, s) => sum + Math.pow(s.amount - historicalAvg, 2), 0) / (sales.length - 2);
  const stdDev = Math.sqrt(variance);

  // Detect anomalies (outside 1.5 standard deviations)
  const upperThreshold = historicalAvg + stdDev * 1.5;
  const lowerThreshold = historicalAvg - stdDev * 1.5;

  const findings: Array<{
    type: 'anomaly' | 'correlation' | 'trend' | 'insight';
    severity: 'high' | 'medium' | 'low';
    title: string;
    description: string;
    icon: 'up' | 'down' | 'alert' | 'chart' | 'link';
  }> = [];

  // Check for abnormal current month
  if (currentMonth > upperThreshold) {
    const percentAbove = Math.round(((currentMonth - historicalAvg) / historicalAvg) * 100);
    findings.push({
      type: 'anomaly',
      severity: percentAbove > 50 ? 'high' : 'medium',
      title: 'Abnormally High Purchases',
      description: `Current month is ${percentAbove}% above historical average ($${Math.round(historicalAvg).toLocaleString()})`,
      icon: 'up',
    });
  } else if (currentMonth < lowerThreshold && currentMonth > 0) {
    const percentBelow = Math.round(((historicalAvg - currentMonth) / historicalAvg) * 100);
    findings.push({
      type: 'anomaly',
      severity: percentBelow > 40 ? 'high' : 'medium',
      title: 'Abnormally Low Purchases',
      description: `Current month is ${percentBelow}% below historical average - possible inventory issues or unreported sales`,
      icon: 'down',
    });
  }

  // Check for sudden drops (month over month)
  const lastMonth = sales[sales.length - 2]?.amount || 0;
  if (lastMonth > 0 && currentMonth < lastMonth * 0.6) {
    findings.push({
      type: 'anomaly',
      severity: 'medium',
      title: 'Significant Month-over-Month Drop',
      description: `Sales dropped ${Math.round((1 - currentMonth / lastMonth) * 100)}% from previous month`,
      icon: 'down',
    });
  }

  // Correlation with compliance history
  if (contraventions >= 2 && trend.trend === 'down') {
    findings.push({
      type: 'correlation',
      severity: 'medium',
      title: 'Compliance-Sales Correlation',
      description: `Declining sales correlate with ${contraventions} contraventions - may indicate operational issues`,
      icon: 'link',
    });
  }

  if (complaints >= 3 && currentMonth > upperThreshold) {
    findings.push({
      type: 'correlation',
      severity: 'high',
      title: 'High Volume + Complaints',
      description: `${complaints} complaints alongside unusually high purchases may indicate overcrowding or capacity issues`,
      icon: 'alert',
    });
  }

  // Trend insights
  if (trend.trend === 'up' && trend.percentage > 15) {
    findings.push({
      type: 'trend',
      severity: 'low',
      title: 'Strong Growth Trend',
      description: `Purchases up ${trend.percentage}% over 3 months - business may be expanding`,
      icon: 'chart',
    });
  } else if (trend.trend === 'down' && trend.percentage > 20) {
    findings.push({
      type: 'trend',
      severity: 'medium',
      title: 'Declining Trend',
      description: `Purchases down ${trend.percentage}% over 3 months - monitor for potential closure or license issues`,
      icon: 'chart',
    });
  }

  // Seasonal analysis
  const currentMonthNum = new Date().getMonth();
  const isSummer = currentMonthNum >= 5 && currentMonthNum <= 8;
  const isHoliday = currentMonthNum === 11 || currentMonthNum === 0;

  if ((isSummer || isHoliday) && currentMonth < historicalAvg * 0.8) {
    findings.push({
      type: 'insight',
      severity: 'low',
      title: 'Below Seasonal Expectations',
      description: `Current purchases lower than expected for ${isSummer ? 'summer' : 'holiday'} season`,
      icon: 'chart',
    });
  }

  return {
    currentMonth,
    annualTotal,
    historicalAvg: Math.round(historicalAvg),
    trend,
    findings,
    hasAnomalies: findings.some((f) => f.type === 'anomaly'),
  };
}

const GREEN_BADGE = 'bg-green-50 text-green-700 border-green-300 dark:bg-green-950 dark:text-green-300 dark:border-green-800';
const AMBER_BADGE = 'bg-amber-50 text-amber-700 border-amber-300 dark:bg-amber-950 dark:text-amber-300 dark:border-amber-800';
const BLUE_BADGE = 'bg-blue-50 text-blue-700 border-blue-300 dark:bg-blue-950 dark:text-blue-300 dark:border-blue-800';
const RED_BADGE = 'bg-red-50 text-red-700 border-red-300 dark:bg-red-950 dark:text-red-300 dark:border-red-800';

@Component({
  selector: 'license-popup',
  imports: [Badge, Button, LucideIcon, Separator, Textarea, ComplaintDialog],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  templateUrl: './license-popup.html',
})
export class LicensePopup {
  readonly license = input.required<License>();
  readonly closed = output<void>();
  readonly isLoggedIn = input(false);
  readonly isStaff = input(false);
  readonly isFollowed = input(false);
  readonly toggleFollow = output<string>();
  readonly isPolice = input(false);
  readonly isLocalGovernment = input(false);

  protected readonly cn = cn;
  protected readonly aiInspectionSummary = AI_INSPECTION_SUMMARY;
  protected readonly onlineFindings = ONLINE_INTELLIGENCE_FINDINGS;
  protected readonly getApplicationTypeDisplayName = getApplicationTypeDisplayName;
  protected readonly getApplicationPhaseDisplayName = getApplicationPhaseDisplayName;
  protected readonly getApplicationAgeDays = getApplicationAgeDays;
  protected readonly formatApplicationAge = formatApplicationAge;

  protected readonly showComplaint = signal(false);
  protected readonly inspectorNotes = signal('');

  protected readonly riskScore = computed(() => calculateRiskScore(this.license().contraventions, this.license().complaints || 0));
  protected readonly riskLabel = computed(() => this.riskScore().charAt(0).toUpperCase() + this.riskScore().slice(1));
  protected readonly hasContraventions = computed(() => this.license().contraventions > 0);
  protected readonly isSpecialEvent = computed(() => this.license().status === 'special-event');
  protected readonly isApplication = computed(() => this.license().status === 'application');

  protected readonly salesTrend = computed(() => {
    const sales = this.license().monthlySales;
    return sales ? calculateSalesTrend(sales) : null;
  });
  protected readonly currentMonthSales = computed(() => {
    const sales = this.license().monthlySales;
    return sales ? getCurrentMonthSales(sales) : 0;
  });
  protected readonly annualSales = computed(() => {
    const sales = this.license().monthlySales;
    return sales ? getAnnualSales(sales) : 0;
  });

  protected readonly salesIntelligence = computed(() => {
    const license = this.license();
    if (!license.monthlySales) return null;
    return analyzeSalesIntelligence(license.monthlySales, license.contraventions, license.complaints || 0);
  });

  protected readonly eventDateLabel = computed(() =>
    new Date(this.license().eventDate as string).toLocaleDateString('en-CA', {
      weekday: 'long',
      year: 'numeric',
      month: 'long',
      day: 'numeric',
    }),
  );

  protected readonly headerClass = computed(() => {
    const type = this.license().licenseType;
    return `p-4 ${type === 'liquor' ? 'bg-[#FFF4ED]' : type === 'cannabis' ? 'bg-[#E8F5E9]' : 'bg-[#EDE7F6]'} rounded-t-lg shrink-0`;
  });

  protected readonly phaseBadgeClass = computed(() => {
    const phase = this.license().applicationPhase;
    return phase === 'approved-in-principle' ? GREEN_BADGE : phase === 'incomplete-info-requested' ? AMBER_BADGE : BLUE_BADGE;
  });
  protected readonly ageClass = computed(() => {
    const days = getApplicationAgeDays(this.license().applicationReceivedDate);
    return cn(days > 180 ? 'text-red-600 font-medium' : days > 90 ? 'text-amber-600' : 'text-muted-foreground');
  });
  protected readonly statusBadgeClass = computed(() => (this.license().activeStatus === 'active' ? GREEN_BADGE : RED_BADGE));
  protected readonly riskIconClass = computed(
    () => `w-4 h-4 shrink-0 ${this.riskScore() === 'high' ? 'text-red-600' : this.riskScore() === 'medium' ? 'text-amber-600' : 'text-green-600'}`,
  );
  protected readonly riskBadgeClass = computed(() =>
    this.riskScore() === 'high' ? RED_BADGE : this.riskScore() === 'medium' ? AMBER_BADGE : GREEN_BADGE,
  );
  protected readonly trendTextClass = computed(
    () => `font-medium ${this.salesTrend()?.trend === 'up' ? 'text-green-600' : this.salesTrend()?.trend === 'down' ? 'text-red-600' : 'text-muted-foreground'}`,
  );
  protected readonly intelTrendClass = computed(
    () =>
      `text-sm font-semibold flex items-center justify-center gap-1 ${
        this.salesIntelligence()?.trend.trend === 'up' ? 'text-green-600' : this.salesIntelligence()?.trend.trend === 'down' ? 'text-red-600' : ''
      }`,
  );

  protected sevBox(severity: string): string {
    return `p-2 rounded-md border ${
      severity === 'high'
        ? 'bg-red-50 dark:bg-red-950/50 border-red-200 dark:border-red-800'
        : severity === 'medium'
          ? 'bg-amber-50 dark:bg-amber-950/50 border-amber-200 dark:border-amber-800'
          : 'bg-slate-50 dark:bg-slate-900/50 border-slate-200 dark:border-slate-700'
    }`;
  }
  protected sevIcon(severity: string): string {
    return `w-4 h-4 shrink-0 mt-0.5 ${severity === 'high' ? 'text-red-600' : severity === 'medium' ? 'text-amber-600' : 'text-slate-500'}`;
  }
  protected sevBadge(severity: string): string {
    return `text-[10px] px-1 py-0 ${
      severity === 'high'
        ? 'border-red-400 text-red-700 dark:text-red-400'
        : severity === 'medium'
          ? 'border-amber-400 text-amber-700 dark:text-amber-400'
          : 'border-slate-400 text-slate-600 dark:text-slate-400'
    }`;
  }
  protected arrowIconClass(severity: string): string {
    return `w-4 h-4 shrink-0 mt-0.5 ${severity === 'high' ? 'text-red-600' : 'text-amber-600'}`;
  }
  protected chartIconClass(severity: string): string {
    return `w-4 h-4 shrink-0 mt-0.5 ${severity === 'medium' ? 'text-amber-600' : 'text-slate-500'}`;
  }
  protected aiScoreBar(score: number): string {
    return `h-full rounded-full ${score > 60 ? 'bg-red-500' : score > 30 ? 'bg-amber-500' : 'bg-green-500'}`;
  }
  protected aiScoreText(score: number): string {
    return `text-xs font-bold ${score > 60 ? 'text-red-600' : score > 30 ? 'text-amber-600' : 'text-green-600'}`;
  }
  protected tiedBox(level: string): string {
    return `rounded p-2 text-xs ${
      level === 'high'
        ? 'bg-red-50 dark:bg-red-950 border border-red-200'
        : level === 'medium'
          ? 'bg-amber-50 dark:bg-amber-950 border border-amber-200'
          : 'bg-white dark:bg-indigo-900/50'
    }`;
  }
  protected tiedBadge(level: string): string {
    return `text-[10px] capitalize ${level === 'high' ? 'bg-red-100 text-red-700' : level === 'medium' ? 'bg-amber-100 text-amber-700' : 'bg-green-100 text-green-700'}`;
  }
  protected fitBox(status: string): string {
    return `rounded p-2 text-xs ${
      status === 'concerns' || status === 'failed'
        ? 'bg-red-50 dark:bg-red-950 border border-red-200'
        : status === 'cleared'
          ? 'bg-green-50 dark:bg-green-950 border border-green-200'
          : 'bg-white dark:bg-indigo-900/50'
    }`;
  }
  protected fitBadge(status: string): string {
    return `text-[10px] capitalize ${
      status === 'cleared' ? 'bg-green-100 text-green-700' : status === 'concerns' || status === 'failed' ? 'bg-red-100 text-red-700' : 'bg-amber-100 text-amber-700'
    }`;
  }
  protected researchBox(sentiment: string): string {
    return `p-2 rounded border text-xs ${
      sentiment === 'concerning'
        ? 'bg-red-50 dark:bg-red-950/30 border-red-200 dark:border-red-800'
        : sentiment === 'positive'
          ? 'bg-green-50 dark:bg-green-950/30 border-green-200 dark:border-green-800'
          : 'bg-gray-50 dark:bg-gray-900/50 border-gray-200 dark:border-gray-700'
    }`;
  }
  protected researchTypeBadge(type: string): string {
    return `text-[10px] px-1 py-0 ${
      type === 'news'
        ? 'bg-blue-100 text-blue-700 border-blue-300'
        : type === 'social-media'
          ? 'bg-purple-100 text-purple-700 border-purple-300'
          : type === 'public-records'
            ? 'bg-amber-100 text-amber-700 border-amber-300'
            : type === 'corporate-registry'
              ? 'bg-cyan-100 text-cyan-700 border-cyan-300'
              : type === 'court-records'
                ? 'bg-red-100 text-red-700 border-red-300'
                : type === 'business-history'
                  ? 'bg-slate-100 text-slate-700 border-slate-300'
                  : 'bg-gray-100 text-gray-700 border-gray-300'
    }`;
  }
  protected researchTypeLabel(type: string): string {
    return type === 'social-media'
      ? 'Social'
      : type === 'public-records'
        ? 'Records'
        : type === 'corporate-registry'
          ? 'Corp'
          : type === 'court-records'
            ? 'Court'
            : type === 'business-history'
              ? 'History'
              : 'News';
  }
  protected researchDot(sentiment: string): string {
    return `w-2 h-2 rounded-full flex-shrink-0 mt-1 ${
      sentiment === 'concerning' ? 'bg-red-500' : sentiment === 'positive' ? 'bg-green-500' : sentiment === 'negative' ? 'bg-amber-500' : 'bg-gray-400'
    }`;
  }
  protected findingBox(severity: string): string {
    return `rounded p-2 text-xs ${
      severity === 'high'
        ? 'bg-red-50 dark:bg-red-950 border-l-2 border-red-500'
        : severity === 'medium'
          ? 'bg-amber-50 dark:bg-amber-950 border-l-2 border-amber-500'
          : severity === 'low'
            ? 'bg-blue-50 dark:bg-blue-950 border-l-2 border-blue-500'
            : 'bg-gray-50 dark:bg-gray-900 border-l-2 border-gray-300'
    }`;
  }
  protected findingBadge(severity: string): string {
    return `text-[10px] capitalize ${
      severity === 'high'
        ? 'bg-red-100 text-red-700'
        : severity === 'medium'
          ? 'bg-amber-100 text-amber-700'
          : severity === 'low'
            ? 'bg-blue-100 text-blue-700'
            : 'bg-gray-100 text-gray-700'
    }`;
  }
  protected shortDate(date: string): string {
    return new Date(date).toLocaleDateString();
  }
}
