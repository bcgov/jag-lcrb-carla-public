/** Staff Insights slide-over (alerts, timeline, peer comparison, following). */
import { ChangeDetectionStrategy, Component, computed, inject, input, output, signal } from '@angular/core';
import { Badge } from '../primitives/badge';
import { Button } from '../primitives/button';
import { CARD } from '../primitives/card';
import { LucideIcon } from '../primitives/lucide-icon';
import { ScrollArea } from '../primitives/scroll-area';
import { SELECT } from '../primitives/select';
import { Separator } from '../primitives/separator';
import { TABS } from '../primitives/tabs';
import { MAP_REPOSITORY } from '@data-access/repositories/map/map.repository';
import { calculateRiskScore, calculateSalesTrend, getCurrentMonthSales } from '@domain/map/license-data';
import { type License } from '@domain/models/map';

// Mock real-time alerts
const mockAlerts = [
  {
    id: '1',
    type: 'high-risk',
    title: 'High Risk Intelligence Detected',
    description: 'Downtown Spirits - New social media post suggests after-hours activity',
    timestamp: '2 hours ago',
    licenseId: 'LIQ-2024-001',
    severity: 'high',
  },
  {
    id: '2',
    type: 'complaint',
    title: 'New Complaint Filed',
    description: 'Gastown Gastropub - Noise complaint received from neighbor',
    timestamp: '4 hours ago',
    licenseId: 'LIQ-2024-002',
    severity: 'medium',
  },
  {
    id: '3',
    type: 'inspection-due',
    title: 'Inspection Overdue',
    description: 'Yaletown Wine Bar - Last inspection was 14 months ago',
    timestamp: '1 day ago',
    licenseId: 'LIQ-2024-003',
    severity: 'medium',
  },
  {
    id: '4',
    type: 'contravention',
    title: 'Contravention Resolved',
    description: 'Coal Harbour Lounge - Previous contravention marked as resolved',
    timestamp: '2 days ago',
    licenseId: 'LIQ-2024-004',
    severity: 'low',
  },
];

type TimelineFilter = 'all' | 'contraventions' | 'complaints' | 'inspections';

@Component({
  selector: 'insights-panel',
  imports: [Badge, Button, ...CARD, LucideIcon, ScrollArea, ...SELECT, Separator, ...TABS],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (isOpen()) {
      <div class="fixed inset-y-0 right-0 w-full sm:w-[480px] bg-background border-l shadow-xl z-50 flex flex-col mt-14">
        <div class="flex items-center justify-between p-4 border-b shrink-0">
          <div class="flex items-center gap-2">
            <svg lucide="TrendingUp" class="w-5 h-5 text-primary"></svg>
            <h2 class="font-semibold text-lg">Insights</h2>
          </div>
          <button uiButton variant="ghost" size="icon" (click)="closed.emit()">
            <svg lucide="X" class="w-4 h-4"></svg>
          </button>
        </div>

        <div uiTabs value="alerts" class="flex-1 flex flex-col overflow-hidden">
          <div uiTabsList class="grid w-full grid-cols-4 shrink-0 mx-4 mt-2" style="width: calc(100% - 2rem)">
            <button uiTabsTrigger value="alerts" class="text-xs">
              <svg lucide="Bell" class="w-3 h-3 mr-1"></svg>
              Alerts
            </button>
            <button uiTabsTrigger value="timeline" class="text-xs">
              <svg lucide="Clock" class="w-3 h-3 mr-1"></svg>
              Timeline
            </button>
            <button uiTabsTrigger value="compare" class="text-xs">
              <svg lucide="BarChart3" class="w-3 h-3 mr-1"></svg>
              Compare
            </button>
            <button uiTabsTrigger value="watching" class="text-xs">
              <svg lucide="Eye" class="w-3 h-3 mr-1"></svg>
              Following
            </button>
          </div>

          <div uiTabsContent value="alerts" class="flex-1 overflow-hidden m-0">
            <div uiScrollArea class="h-full">
              <div class="p-4 space-y-3">
                <div class="flex items-center justify-between mb-2">
                  <p class="text-sm text-muted-foreground">Real-time alerts prioritized by AI</p>
                  <span uiBadge variant="outline" class="text-xs">
                    <svg lucide="Sparkles" class="w-3 h-3 mr-1"></svg>
                    AI Prioritized
                  </span>
                </div>
                @for (alert of alerts; track alert.id) {
                  <div
                    uiCard
                    [class]="
                      'cursor-pointer hover:bg-muted/50 transition-colors ' +
                      (alert.severity === 'high'
                        ? 'border-l-4 border-l-red-500'
                        : alert.severity === 'medium'
                          ? 'border-l-4 border-l-amber-500'
                          : 'border-l-4 border-l-green-500')
                    "
                  >
                    <div uiCardContent class="p-3">
                      <div class="flex items-start gap-3">
                        @if (alert.severity === 'high') {
                          <svg lucide="AlertCircle" class="w-5 h-5 text-red-500 shrink-0 mt-0.5"></svg>
                        } @else if (alert.severity === 'medium') {
                          <svg lucide="AlertTriangle" class="w-5 h-5 text-amber-500 shrink-0 mt-0.5"></svg>
                        } @else {
                          <svg lucide="CheckCircle" class="w-5 h-5 text-green-500 shrink-0 mt-0.5"></svg>
                        }
                        <div class="flex-1 min-w-0">
                          <h4 class="font-medium text-sm">{{ alert.title }}</h4>
                          <p class="text-xs text-muted-foreground mt-1">{{ alert.description }}</p>
                          <div class="flex items-center gap-2 mt-2">
                            <span class="text-xs text-muted-foreground">{{ alert.timestamp }}</span>
                            <svg lucide="ChevronRight" class="w-3 h-3 text-muted-foreground"></svg>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                }
              </div>
            </div>
          </div>

          <div uiTabsContent value="timeline" class="flex-1 overflow-hidden m-0">
            <div class="p-4 space-y-4 h-full flex flex-col">
              <div class="space-y-2 shrink-0">
                <label class="text-sm font-medium">Select Licensee</label>
                <ui-select [value]="selectedLicenseId()" (valueChange)="selectedLicenseId.set($event ?? '')">
                  <button uiSelectTrigger>
                    <span uiSelectValue placeholder="Choose a licensee..."></span>
                  </button>
                  <ng-template uiSelectContent>
                    @for (license of licensedOptions; track license.id) {
                      <ui-select-item [value]="license.id">{{ license.businessName }}</ui-select-item>
                    }
                  </ng-template>
                </ui-select>
              </div>

              @if (selectedLicense(); as selected) {
                <div uiCard class="bg-amber-50 dark:bg-amber-950/30 border-amber-200 dark:border-amber-800 shrink-0">
                  <div uiCardContent class="p-3">
                    <div class="flex items-center gap-2 mb-2">
                      <svg lucide="Sparkles" class="w-4 h-4 text-amber-600"></svg>
                      <span class="text-sm font-medium text-amber-800 dark:text-amber-200"> AI Pattern Analysis </span>
                    </div>
                    <ul class="text-xs text-amber-700 dark:text-amber-300 space-y-1">
                      @for (pattern of patternSummary(); track $index) {
                        <li>• {{ pattern }}</li>
                      }
                    </ul>
                  </div>
                </div>

                <div class="flex gap-2 shrink-0">
                  <button uiButton [variant]="timelineFilter() === 'all' ? 'default' : 'outline'" size="sm" (click)="timelineFilter.set('all')" class="text-xs">
                    All
                  </button>
                  <button
                    uiButton
                    [variant]="timelineFilter() === 'contraventions' ? 'default' : 'outline'"
                    size="sm"
                    (click)="timelineFilter.set('contraventions')"
                    class="text-xs"
                  >
                    Contraventions
                  </button>
                  <button
                    uiButton
                    [variant]="timelineFilter() === 'complaints' ? 'default' : 'outline'"
                    size="sm"
                    (click)="timelineFilter.set('complaints')"
                    class="text-xs"
                  >
                    Complaints
                  </button>
                  <button
                    uiButton
                    [variant]="timelineFilter() === 'inspections' ? 'default' : 'outline'"
                    size="sm"
                    (click)="timelineFilter.set('inspections')"
                    class="text-xs"
                  >
                    Inspections
                  </button>
                </div>

                <div uiScrollArea class="flex-1">
                  <div class="space-y-3 pr-4">
                    @if (timelineEvents().length === 0) {
                      <p class="text-sm text-muted-foreground text-center py-8">No events found</p>
                    } @else {
                      @for (event of timelineEvents(); track $index; let i = $index) {
                        <div class="flex gap-3">
                          <div class="flex flex-col items-center">
                            <div
                              [class]="
                                'w-8 h-8 rounded-full flex items-center justify-center shrink-0 ' +
                                (event.type === 'contravention'
                                  ? 'bg-red-100 dark:bg-red-900'
                                  : event.type === 'complaint'
                                    ? 'bg-amber-100 dark:bg-amber-900'
                                    : 'bg-blue-100 dark:bg-blue-900')
                              "
                            >
                              @if (event.type === 'contravention') {
                                <svg lucide="XCircle" class="w-4 h-4 text-red-600"></svg>
                              } @else if (event.type === 'complaint') {
                                <svg lucide="MessageSquare" class="w-4 h-4 text-amber-600"></svg>
                              } @else {
                                <svg lucide="ClipboardCheck" class="w-4 h-4 text-blue-600"></svg>
                              }
                            </div>
                            @if (i < timelineEvents().length - 1) {
                              <div class="w-px h-full bg-border mt-2"></div>
                            }
                          </div>
                          <div class="flex-1 pb-4">
                            <div class="flex items-center gap-2">
                              <span class="font-medium text-sm">{{ event.title }}</span>
                              @if (event.severity) {
                                <span
                                  uiBadge
                                  variant="outline"
                                  [class]="
                                    'text-xs ' +
                                    (event.severity === 'high' || event.severity === 'critical'
                                      ? 'border-red-300 text-red-700'
                                      : event.severity === 'medium' || event.severity === 'major'
                                        ? 'border-amber-300 text-amber-700'
                                        : 'border-green-300 text-green-700')
                                  "
                                >
                                  {{ event.severity }}
                                </span>
                              }
                            </div>
                            <p class="text-xs text-muted-foreground mt-1">{{ event.description }}</p>
                            <p class="text-xs text-muted-foreground mt-1">
                              <svg lucide="Calendar" class="w-3 h-3 inline mr-1"></svg>
                              {{ event.date }}
                            </p>
                          </div>
                        </div>
                      }
                    }
                  </div>
                </div>
              }

              @if (!selectedLicense()) {
                <div class="flex-1 flex items-center justify-center">
                  <p class="text-sm text-muted-foreground">Select a licensee to view their history</p>
                </div>
              }
            </div>
          </div>

          <div uiTabsContent value="compare" class="flex-1 overflow-hidden m-0">
            <div uiScrollArea class="h-full">
              <div class="p-4 space-y-4">
                <div class="space-y-2">
                  <label class="text-sm font-medium">Select Licensee to Compare</label>
                  <ui-select [value]="selectedLicenseId()" (valueChange)="selectedLicenseId.set($event ?? '')">
                    <button uiSelectTrigger>
                      <span uiSelectValue placeholder="Choose a licensee..."></span>
                    </button>
                    <ng-template uiSelectContent>
                      @for (license of licensedOptions; track license.id) {
                        <ui-select-item [value]="license.id">{{ license.businessName }}</ui-select-item>
                      }
                    </ng-template>
                  </ui-select>
                </div>

                @if (selectedLicense(); as selected) {
                  @if (peerComparison(); as peer) {
                    @if (peer.isOutlier) {
                      <div uiCard class="bg-red-50 dark:bg-red-950/30 border-red-200 dark:border-red-800">
                        <div uiCardContent class="p-3">
                          <div class="flex items-center gap-2">
                            <svg lucide="AlertTriangle" class="w-4 h-4 text-red-600"></svg>
                            <span class="text-sm font-medium text-red-800 dark:text-red-200"> Outlier Detected - Above average for region </span>
                          </div>
                        </div>
                      </div>
                    }

                    <div uiCard>
                      <div uiCardHeader class="pb-2">
                        <div uiCardTitle class="text-sm flex items-center gap-2">
                          <svg lucide="Users" class="w-4 h-4"></svg>
                          Peer Comparison
                        </div>
                        <p class="text-xs text-muted-foreground">
                          Compared against {{ peer.peerCount }} similar {{ selected.licenseType === 'liquor' ? 'liquor' : 'cannabis' }} establishments in
                          {{ selected.region }}
                        </p>
                      </div>
                      <div uiCardContent class="space-y-4">
                        <div>
                          <div class="flex justify-between text-sm mb-1">
                            <span>Contraventions</span>
                            <span class="font-medium"> {{ peer.licenseContraventions }} vs {{ peer.avgContraventions }} avg </span>
                          </div>
                          <div class="h-2 bg-muted rounded-full overflow-hidden">
                            <div
                              [class]="'h-full ' + (peer.licenseContraventions > toNumber(peer.avgContraventions) ? 'bg-red-500' : 'bg-green-500')"
                              [style.width]="barWidth(peer.licenseContraventions, toNumber(peer.avgContraventions))"
                            ></div>
                          </div>
                        </div>

                        <div>
                          <div class="flex justify-between text-sm mb-1">
                            <span>Complaints</span>
                            <span class="font-medium"> {{ peer.licenseComplaints }} vs {{ peer.avgComplaints }} avg </span>
                          </div>
                          <div class="h-2 bg-muted rounded-full overflow-hidden">
                            <div
                              [class]="'h-full ' + (peer.licenseComplaints > toNumber(peer.avgComplaints) ? 'bg-amber-500' : 'bg-green-500')"
                              [style.width]="barWidth(peer.licenseComplaints, toNumber(peer.avgComplaints))"
                            ></div>
                          </div>
                        </div>

                        <div class="flex items-center justify-between pt-2 border-t">
                          <span class="text-sm">Risk Score</span>
                          <span
                            uiBadge
                            variant="outline"
                            [class]="
                              peer.riskScore === 'high'
                                ? 'bg-red-50 text-red-700 border-red-300'
                                : peer.riskScore === 'medium'
                                  ? 'bg-amber-50 text-amber-700 border-amber-300'
                                  : 'bg-green-50 text-green-700 border-green-300'
                            "
                          >
                            {{ peer.riskScore.toUpperCase() }}
                          </span>
                        </div>

                        <div class="flex items-center justify-between">
                          <span class="text-sm">Performance Percentile</span>
                          <span class="text-sm font-medium">{{ peer.percentile }}th</span>
                        </div>

                        <div class="flex items-center justify-between">
                          <span class="text-sm">High Risk Peers in Region</span>
                          <span class="text-sm font-medium">{{ peer.highRiskPeers }}</span>
                        </div>

                        @if (peer.hasSalesData) {
                          <div uiSeparator class="my-2"></div>
                          <div class="pt-2">
                            <div class="flex items-center gap-2 mb-2">
                              <svg lucide="DollarSign" class="w-4 h-4 text-blue-600"></svg>
                              <span class="text-sm font-medium">BCLDB Sales Comparison</span>
                            </div>
                            <div class="flex justify-between text-sm mb-1">
                              <span>Monthly Sales</span>
                              <span class="font-medium"> \${{ peer.licenseMonthlySales.toLocaleString() }} vs \${{ peer.avgMonthlySales.toLocaleString() }} avg </span>
                            </div>
                            <div class="h-2 bg-muted rounded-full overflow-hidden">
                              <div
                                [class]="'h-full ' + (peer.licenseMonthlySales > peer.avgMonthlySales ? 'bg-blue-500' : 'bg-slate-400')"
                                [style.width]="barWidth(peer.licenseMonthlySales, peer.avgMonthlySales)"
                              ></div>
                            </div>
                            @if (peer.salesTrend; as trend) {
                              <div class="flex items-center justify-between mt-2">
                                <span class="text-sm">3-Month Trend</span>
                                <div class="flex items-center gap-1">
                                  @if (trend.trend === 'up') {
                                    <svg lucide="TrendingUp" class="w-3 h-3 text-green-600"></svg>
                                  } @else if (trend.trend === 'down') {
                                    <svg lucide="TrendingDown" class="w-3 h-3 text-red-600"></svg>
                                  } @else {
                                    <svg lucide="Minus" class="w-3 h-3 text-muted-foreground"></svg>
                                  }
                                  <span
                                    [class]="'text-sm font-medium ' + (trend.trend === 'up' ? 'text-green-600' : trend.trend === 'down' ? 'text-red-600' : '')"
                                  >
                                    {{ trend.trend === 'stable' ? 'Stable' : (trend.trend === 'up' ? '+' : '-') + trend.percentage + '%' }}
                                  </span>
                                </div>
                              </div>
                            }
                          </div>
                        }
                      </div>
                    </div>
                  }
                }

                @if (!selectedLicense()) {
                  <div class="flex items-center justify-center py-12">
                    <p class="text-sm text-muted-foreground">Select a licensee to compare with peers</p>
                  </div>
                }
              </div>
            </div>
          </div>

          <div uiTabsContent value="watching" class="flex-1 overflow-hidden m-0">
            <div uiScrollArea class="h-full">
              <div class="p-4 space-y-3">
                <p class="text-sm text-muted-foreground mb-4">Licensees you are following will appear here. Follow a licensee from their popup window.</p>
                @if (followedLicenseData().length === 0) {
                  <div class="flex flex-col items-center justify-center py-12 text-center">
                    <svg lucide="Eye" class="w-12 h-12 text-muted-foreground/30 mb-4"></svg>
                    <p class="text-sm text-muted-foreground">You are not following any licensees</p>
                    <p class="text-xs text-muted-foreground mt-1">Click the Follow button on a licensee popup to add them here</p>
                  </div>
                } @else {
                  @for (license of followedLicenseData(); track license.id) {
                    <div uiCard class="cursor-pointer hover:bg-muted/50 transition-colors" (click)="selectLicense.emit(license)">
                      <div uiCardContent class="p-3">
                        <div class="flex items-start justify-between gap-2">
                          <div class="flex-1 min-w-0">
                            <h4 class="font-medium text-sm truncate">{{ license.businessName }}</h4>
                            <div class="flex items-center gap-2 mt-1">
                              <span uiBadge variant="outline" class="text-xs">
                                {{ license.licenseType }}
                              </span>
                              <span
                                uiBadge
                                variant="outline"
                                [class]="
                                  'text-xs ' +
                                  (risk(license) === 'high'
                                    ? 'border-red-300 text-red-700'
                                    : risk(license) === 'medium'
                                      ? 'border-amber-300 text-amber-700'
                                      : 'border-green-300 text-green-700')
                                "
                              >
                                {{ risk(license) }} risk
                              </span>
                            </div>
                            <p class="text-xs text-muted-foreground mt-1 flex items-center gap-1">
                              <svg lucide="MapPin" class="w-3 h-3"></svg>
                              {{ license.city }}
                            </p>
                          </div>
                          <svg lucide="ChevronRight" class="w-4 h-4 text-muted-foreground shrink-0"></svg>
                        </div>
                      </div>
                    </div>
                  }
                }
              </div>
            </div>
          </div>
        </div>
      </div>
    }
  `,
})
export class InsightsPanel {
  private readonly map = inject(MAP_REPOSITORY);
  readonly isOpen = input.required<boolean>();
  readonly closed = output<void>();
  readonly followedLicenses = input.required<string[]>();
  readonly selectLicense = output<License>();

  protected readonly alerts = mockAlerts;
  protected readonly licensedOptions = this.map.licenses().filter((l) => l.status === 'licensed').slice(0, 50);
  protected readonly selectedLicenseId = signal<string>('');
  protected readonly timelineFilter = signal<TimelineFilter>('all');

  protected readonly selectedLicense = computed(() => this.map.licenses().find((l) => l.id === this.selectedLicenseId()));

  protected readonly followedLicenseData = computed(() => this.map.licenses().filter((l) => this.followedLicenses().includes(l.id)));

  // Generate timeline events for selected license
  protected readonly timelineEvents = computed(() => {
    const selectedLicense = this.selectedLicense();
    const timelineFilter = this.timelineFilter();
    if (!selectedLicense) return [];

    const events: { date: string; type: string; title: string; description: string; severity?: string }[] = [];

    // Add contraventions
    if (selectedLicense.contraventionHistory && (timelineFilter === 'all' || timelineFilter === 'contraventions')) {
      selectedLicense.contraventionHistory.forEach((c) => {
        events.push({
          date: c.date,
          type: 'contravention',
          title: 'Contravention',
          description: c.description,
          severity: c.severity,
        });
      });
    }

    // Add complaints
    if (selectedLicense.complaintHistory && (timelineFilter === 'all' || timelineFilter === 'complaints')) {
      selectedLicense.complaintHistory.forEach((c) => {
        events.push({
          date: c.date,
          type: 'complaint',
          title: 'Complaint',
          description: c.description,
          severity: c.status === 'investigating' ? 'medium' : 'low',
        });
      });
    }

    // Add inspections
    if (selectedLicense.inspectionHistory && (timelineFilter === 'all' || timelineFilter === 'inspections')) {
      selectedLicense.inspectionHistory.forEach((i) => {
        events.push({
          date: i.date,
          type: 'inspection',
          title: 'Inspection',
          description: `${i.result.toUpperCase()} - ${i.inspector}`,
          severity: i.result === 'fail' ? 'high' : i.result === 'warning' ? 'medium' : 'low',
        });
      });
    }

    return events.sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime());
  });

  // AI Pattern Summary
  protected readonly patternSummary = computed(() => {
    const selectedLicense = this.selectedLicense();
    if (!selectedLicense) return null;

    const complaints = selectedLicense.complaintHistory || [];
    const contraventions = selectedLicense.contraventionHistory || [];

    const noiseComplaints = complaints.filter((c) => c.category === 'noise').length;
    const weekendIssues = complaints.filter((c) => {
      const day = new Date(c.date).getDay();
      return day === 0 || day === 5 || day === 6;
    }).length;

    const patterns: string[] = [];
    if (noiseComplaints >= 2) {
      patterns.push(`${noiseComplaints} noise complaints in the past 12 months`);
    }
    if (weekendIssues >= 2) {
      patterns.push(`${weekendIssues} issues occurred on weekends`);
    }
    if (contraventions.length >= 2) {
      const recentYear = contraventions.filter((c) => {
        const date = new Date(c.date);
        const yearAgo = new Date();
        yearAgo.setFullYear(yearAgo.getFullYear() - 1);
        return date >= yearAgo;
      }).length;
      if (recentYear >= 2) {
        patterns.push(`${recentYear} contraventions in the past year - trending upward`);
      }
    }

    return patterns.length > 0 ? patterns : ['No significant patterns detected'];
  });

  // Peer comparison data
  protected readonly peerComparison = computed(() => {
    const selectedLicense = this.selectedLicense();
    if (!selectedLicense) return null;

    const peers = this.map.licenses().filter(
      (l) =>
        l.licenseType === selectedLicense.licenseType &&
        l.region === selectedLicense.region &&
        l.status === 'licensed' &&
        l.id !== selectedLicense.id,
    );

    const avgContraventions = peers.reduce((sum, p) => sum + p.contraventions, 0) / peers.length || 0;
    const avgComplaints = peers.reduce((sum, p) => sum + (p.complaints || 0), 0) / peers.length || 0;

    const peersWithSales = peers.filter((p) => p.monthlySales && p.monthlySales.length > 0);
    const avgMonthlySales =
      peersWithSales.length > 0 ? peersWithSales.reduce((sum, p) => sum + getCurrentMonthSales(p.monthlySales!), 0) / peersWithSales.length : 0;
    const licenseMonthlySales = selectedLicense.monthlySales ? getCurrentMonthSales(selectedLicense.monthlySales) : 0;
    const salesTrend = selectedLicense.monthlySales ? calculateSalesTrend(selectedLicense.monthlySales) : null;

    const licenseRisk = calculateRiskScore(selectedLicense.contraventions, selectedLicense.complaints || 0);
    const highRiskPeers = peers.filter((p) => calculateRiskScore(p.contraventions, p.complaints || 0) === 'high').length;
    const percentile =
      (peers.filter((p) => p.contraventions + (p.complaints || 0) > selectedLicense.contraventions + (selectedLicense.complaints || 0)).length /
        peers.length) *
      100;

    return {
      peerCount: peers.length,
      avgContraventions: avgContraventions.toFixed(1),
      avgComplaints: avgComplaints.toFixed(1),
      licenseContraventions: selectedLicense.contraventions,
      licenseComplaints: selectedLicense.complaints || 0,
      riskScore: licenseRisk,
      highRiskPeers,
      percentile: Math.round(percentile),
      isOutlier: selectedLicense.contraventions > avgContraventions * 1.5 || (selectedLicense.complaints || 0) > avgComplaints * 1.5,
      avgMonthlySales: Math.round(avgMonthlySales),
      licenseMonthlySales,
      salesTrend,
      hasSalesData: !!selectedLicense.monthlySales,
    };
  });

  protected toNumber(value: string): number {
    return Number(value);
  }

  /** The comparison bars' width: `Math.min((value / (avg * 2 || 1)) * 100, 100)%`. */
  protected barWidth(value: number, avg: number): string {
    return `${Math.min((value / (avg * 2 || 1)) * 100, 100)}%`;
  }

  protected risk(license: License): 'high' | 'medium' | 'low' {
    return calculateRiskScore(license.contraventions, license.complaints || 0);
  }
}
