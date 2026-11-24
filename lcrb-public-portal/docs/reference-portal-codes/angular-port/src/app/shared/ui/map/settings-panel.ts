/** Admin System Settings overlay; every setting is local state and nothing is persisted, as in the prototype. */
import { ChangeDetectionStrategy, Component, output, signal } from '@angular/core';
import { Badge } from '../primitives/badge';
import { Button } from '../primitives/button';
import { CARD } from '../primitives/card';
import { Input } from '../primitives/input';
import { Label } from '../primitives/label';
import { LucideIcon } from '../primitives/lucide-icon';
import { ScrollArea } from '../primitives/scroll-area';
import { SELECT } from '../primitives/select';
import { Separator } from '../primitives/separator';
import { Slider } from '../primitives/slider';
import { Switch } from '../primitives/switch';
import { TABS } from '../primitives/tabs';

interface OnlineResearchSources {
  news: boolean;
  socialMedia: boolean;
  publicRecords: boolean;
  courtRecords: boolean;
  corporateRegistry: boolean;
}

@Component({
  selector: 'settings-panel',
  imports: [Badge, Button, ...CARD, Input, Label, LucideIcon, ScrollArea, ...SELECT, Separator, Slider, Switch, ...TABS],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  templateUrl: './settings-panel.html',
})
export class SettingsPanel {
  readonly closed = output<void>();

  protected readonly mapDefaultZoom = signal('7');
  protected readonly mapDefaultRegion = signal('all');
  protected readonly showApplicationsByDefault = signal(true);
  protected readonly showSpecialEventsByDefault = signal(true);

  protected readonly emailNotifications = signal(true);
  protected readonly newApplicationAlerts = signal(true);
  protected readonly contraventionAlerts = signal(true);
  protected readonly inspectionReminders = signal(true);
  protected readonly reminderLeadTime = signal('24');

  protected readonly allowSelfRegistration = signal(false);
  protected readonly requireMFA = signal(true);
  protected readonly sessionTimeout = signal('60');

  protected readonly dataRetentionPeriod = signal('36');
  protected readonly autoArchiveCompleted = signal(true);
  protected readonly enableAuditLog = signal(true);

  protected readonly dynamicsIntegration = signal(false);
  protected readonly dynamicsEndpoint = signal('');
  protected readonly aiWebCrawlEnabled = signal(true);
  protected readonly crawlFrequency = signal('daily');
  protected readonly customWebsites = signal<string[]>(['https://example-cannabis-review.ca', 'https://bc-nightlife-blog.com']);
  protected readonly newWebsite = signal('');

  protected readonly workforceEnabled = signal(true);
  protected readonly maxHoursPerWeek = signal('37.5');
  protected readonly maxInspectionsPerDay = signal('4');
  protected readonly breakBetweenInspections = signal('30');
  protected readonly travelTimeBuffer = signal('45');
  protected readonly uploadedDocuments = signal<{ name: string; type: string; uploadDate: string }[]>([
    { name: 'BCGEU Collective Agreement 2023-2025.pdf', type: 'collective-agreement', uploadDate: '2024-01-15' },
  ]);
  protected readonly respectSeniority = signal(true);
  protected readonly allowOvertime = signal(false);
  protected readonly vacationBlackout = signal(true);

  protected readonly riskScoreHighThreshold = signal(70);
  protected readonly riskScoreMediumThreshold = signal(40);
  protected readonly enableSalesAnomalyDetection = signal(true);
  protected readonly salesAnomalySensitivity = signal('medium');
  protected readonly applicationAgeWarningDays = signal('90');
  protected readonly applicationAgeCriticalDays = signal('180');
  protected readonly enableTiedHouseDetection = signal(true);
  protected readonly tiedHouseSensitivity = signal('high');
  protected readonly enableFitProperAlerts = signal(true);
  protected readonly defaultSearchSort = signal('default');
  protected readonly searchResultsPerPage = signal('20');
  protected readonly enableOnlineResearch = signal(true);
  protected readonly onlineResearchSources = signal<OnlineResearchSources>({
    news: true,
    socialMedia: true,
    publicRecords: true,
    courtRecords: true,
    corporateRegistry: true,
  });
  protected readonly bcldSyncFrequency = signal('daily');
  protected readonly enablePeerComparison = signal(true);

  protected readonly userRoles = [
    { role: 'Administrator', count: 2, permissions: 'Full system access' },
    { role: 'Inspector', count: 12, permissions: 'Inspection management, basic reports' },
    {
      role: 'Licensing Officer',
      count: 8,
      permissions: 'Application review, license management',
    },
    { role: 'Analyst', count: 4, permissions: 'Reports, analytics, read-only data access' },
    { role: 'Public User', count: 156, permissions: 'View own license information only' },
  ];

  protected setSource(key: keyof OnlineResearchSources, value: boolean): void {
    this.onlineResearchSources.set({ ...this.onlineResearchSources(), [key]: value });
  }

  protected addWebsite(): void {
    const newWebsite = this.newWebsite();
    if (newWebsite.trim() && !this.customWebsites().includes(newWebsite.trim())) {
      this.customWebsites.set([...this.customWebsites(), newWebsite.trim()]);
      this.newWebsite.set('');
    }
  }

  protected removeWebsite(url: string): void {
    this.customWebsites.set(this.customWebsites().filter((w) => w !== url));
  }
}
