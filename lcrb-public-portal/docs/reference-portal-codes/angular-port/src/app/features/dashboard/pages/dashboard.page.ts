/** Licensee home. Stat cards, renewals box and activity rows are plain `<a href>` links, as in the prototype (full page loads). */
import { ChangeDetectionStrategy, Component, computed, inject, input } from '@angular/core';
import { AIAssistant } from '../components/ai-assistant';
import { BCGovFooter } from '@core/layout/footer';
import { BCGovHeader } from '@core/layout/header';
import { PageHero } from '@shared/ui/bc-gov/page-hero';
import { QuickActionCard } from '../components/quick-action-card';
import { type ReminderContextItem, SmartReminders } from '../components/smart-reminders';
import { DraftsPanel } from '../components/drafts-panel';
import { StartHere } from '../components/start-here';
import type { IconName } from '@shared/ui/primitives/icons';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { NOTIFICATIONS_REPOSITORY } from '@data-access/repositories/notifications/notifications.repository';
import { path } from '@/app/route-registry';


// Mock licence renewal data; would come from the database in a real app.
const mockLicences = [
  { id: '326739', name: 'Liquor Primary - OneStop Test 1', expiryDate: '2026-06-20' },
  { id: '326740', name: 'Food Primary - Downtown Bistro', expiryDate: '2026-12-01' },
  { id: '326741', name: 'Cannabis Retail - East Side', expiryDate: '2027-03-15' },
];

// In-flight applications used to surface "action needed" reminders.
const mockApplications = [{ id: 'FP-126418', name: 'Food Primary Licence Application', status: 'Awaiting documents from you' }];

// The structured context the AI nudge engine reasons over.
/** The smart-reminder context; the unread count comes from the notifications repository. */
const buildReminderItems = (unreadNoticeCount: number): ReminderContextItem[] => [
  ...mockLicences.map((l) => ({
    kind: 'licence',
    label: l.name,
    detail: `Expires ${l.expiryDate}`,
    href: path('licences.detail', { id: l.id }),
  })),
  ...mockApplications.map((a) => ({
    kind: 'application',
    label: a.name,
    detail: a.status,
    href: path('applications.food-primary'),
  })),
  {
    kind: 'notices',
    label: 'Unread notices',
    detail: `${unreadNoticeCount} unread notice${unreadNoticeCount === 1 ? '' : 's'} in your inbox`,
    href: path('dashboard.notices'),
  },
];

type RenewalStatus = 'expired' | 'soon' | 'ok';

function getRenewalSummary(licences: { expiryDate: string }[]) {
  const now = new Date();
  const thirtyDaysMs = 30 * 24 * 60 * 60 * 1000;

  let expired = 0;
  let soon = 0;

  for (const licence of licences) {
    const expiry = new Date(licence.expiryDate);
    const diff = expiry.getTime() - now.getTime();
    if (diff < 0) {
      expired += 1;
    } else if (diff <= thirtyDaysMs) {
      soon += 1;
    }
  }

  const status: RenewalStatus = expired > 0 ? 'expired' : soon > 0 ? 'soon' : 'ok';
  return { expired, soon, status, total: expired + soon };
}

@Component({
  selector: 'renewals-box',
  imports: [LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <a [href]="path('licences.list') + '?filter=renewal'" [attr.class]="'flex items-center gap-4 text-left p-5 rounded-lg border transition-shadow hover:shadow-md ' + view().config.container">
      <div [attr.class]="'shrink-0 w-12 h-12 rounded-full flex items-center justify-center ' + view().config.iconWrap">
        <svg [lucide]="view().config.icon" class="h-6 w-6"></svg>
      </div>
      <div class="flex-1">
        <div [attr.class]="'font-semibold ' + view().config.title">{{ view().config.heading }}</div>
        <div [attr.class]="'text-sm mt-0.5 ' + view().config.body">{{ view().config.message }}</div>
      </div>
      @if (view().total > 0) {
        <div [attr.class]="'shrink-0 text-3xl font-bold ' + view().config.title">{{ view().total }}</div>
      }
    </a>
  `,
})
export class RenewalsBox {
  protected readonly path = path;
  readonly licences = input.required<{ expiryDate: string }[]>();

  protected readonly view = computed(() => {
    const { expired, soon, status, total } = getRenewalSummary(this.licences());
    const config: Record<
      RenewalStatus,
      { container: string; iconWrap: string; title: string; body: string; icon: IconName; heading: string; message: string }
    > = {
      expired: {
        container: 'bg-red-50 border-red-300',
        iconWrap: 'bg-red-100 text-red-700',
        title: 'text-red-900',
        body: 'text-red-800',
        icon: 'AlertTriangle',
        heading: `${expired} licence${expired === 1 ? '' : 's'} ${expired === 1 ? 'has' : 'have'} expired`,
        message:
          soon > 0
            ? `Renew immediately. ${soon} more ${soon === 1 ? 'is' : 'are'} due within 30 days.`
            : 'Renew immediately to avoid an interruption to your operations.',
      },
      soon: {
        container: 'bg-orange-50 border-orange-300',
        iconWrap: 'bg-orange-100 text-orange-700',
        title: 'text-orange-900',
        body: 'text-orange-800',
        icon: 'CalendarClock',
        heading: `${soon} licence${soon === 1 ? '' : 's'} up for renewal`,
        message: 'Due within the next 30 days. Renew now to stay compliant.',
      },
      ok: {
        container: 'bg-green-50 border-green-300',
        iconWrap: 'bg-green-100 text-green-700',
        title: 'text-green-900',
        body: 'text-green-800',
        icon: 'CheckCircle2',
        heading: 'No renewals needed',
        message: 'All of your licences are current. Nothing is nearing expiry.',
      },
    };
    return { config: config[status], total };
  });
}

@Component({
  selector: 'dashboard-stat-card',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <a [attr.href]="href()" [attr.class]="linkClass()">
      <div class="text-3xl font-bold text-[#013366]">{{ value() }}</div>
      <div class="text-sm text-muted-foreground mt-1">{{ label() }}</div>
    </a>
  `,
})
export class StatCard {
  readonly label = input.required<string>();
  readonly value = input.required<string>();
  readonly href = input.required<string>();
  readonly highlight = input(false);
  protected readonly linkClass = computed(
    () =>
      `block p-6 rounded-lg border transition-colors hover:border-[#013366] ${
        this.highlight() ? 'bg-[#FEF1D8] border-[#F5A623]' : 'bg-white'
      }`,
  );
}

const STATUS_COLORS: Record<string, string> = {
  'In Review': 'bg-blue-100 text-blue-800',
  'Renewal Due': 'bg-yellow-100 text-yellow-800',
  Draft: 'bg-gray-100 text-gray-800',
  Approved: 'bg-green-100 text-green-800',
  Pending: 'bg-orange-100 text-orange-800',
};

/** On the `<a>` itself so the link stays a direct child of the parent's `divide-y` list. */
@Component({
  selector: 'a[dashboardActivityItem]',
  imports: [LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { '[attr.href]': 'href()', class: 'flex items-center justify-between p-4 hover:bg-muted/50 transition-colors' },
  template: `
      <div class="flex items-center gap-4">
        <div [attr.class]="iconClass()">
          @if (type() === 'application') {
            <svg lucide="FileText" class="h-5 w-5"></svg>
          }
          @if (type() === 'licence') {
            <svg lucide="FileText" class="h-5 w-5"></svg>
          }
        </div>
        <div>
          <div class="font-medium text-foreground">{{ title() }}</div>
          <div class="text-sm text-muted-foreground">{{ date() }}</div>
        </div>
      </div>
      <span [attr.class]="'text-xs font-medium px-2.5 py-1 rounded-full ' + (statusColors[status()] || 'bg-gray-100')">{{
        status()
      }}</span>
  `,
})
export class ActivityItem {
  readonly type = input.required<'application' | 'licence'>();
  readonly title = input.required<string>();
  readonly status = input.required<string>();
  readonly date = input.required<string>();
  readonly href = input.required<string>();
  protected readonly statusColors = STATUS_COLORS;
  protected readonly iconClass = computed(
    () =>
      `w-10 h-10 rounded-full flex items-center justify-center ${
        this.type() === 'application'
          ? 'bg-[#013366]/10 text-[#013366]'
          : this.type() === 'licence'
            ? 'bg-[#2E8540]/10 text-[#2E8540]'
            : 'bg-[#FCBA19]/20 text-[#013366]'
      }`,
  );
}

@Component({
  selector: 'dashboard-page',
  imports: [
    ActivityItem,
    AIAssistant,
    BCGovFooter,
    BCGovHeader,
    DraftsPanel,
    LucideIcon,
    PageHero,
    QuickActionCard,
    RenewalsBox,
    SmartReminders,
    StartHere,
    StatCard,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="min-h-screen flex flex-col">
      <bc-gov-header currentPage="/dashboard" />

      <main class="flex-1">
        <!-- Welcome Banner -->
        <page-hero
          title="Welcome to the LCRB Portal"
          description="Manage your liquor and cannabis licences, submit applications, and access all your licensing needs in one place."
        />

        <!-- Main Content -->
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8 md:py-12">
          <!-- Recommended first step for new licensees; dismiss-once. -->
          <start-here />

          <!-- saved drafts first. Renders nothing when empty. -->
          <drafts-panel />

          <!-- AI Prompt Section -->
          <div class="text-center mb-12">
            <h2 class="text-2xl md:text-3xl font-medium text-[#38598A] italic mb-8">What would you like to do today?</h2>

            <!-- Quick Action Cards -->
            <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 mb-8">
              <quick-action-card
                title="Start a new application"
                description="Browse all licence applications & authorizations"
                [href]="path('applications.new')"
                icon="FilePlus2"
                variant="primary"
              />
              <quick-action-card
                title="Plan a new establishment"
                description="Feasibility, fees, timeline & checklist"
                [href]="path('public.establishment-planner')"
                icon="Building2"
              />
              <quick-action-card title="Find out what licence type I need" [href]="path('public.licence-finder')" icon="Search" />
              <quick-action-card
                title="Go to My Applications"
                description="View, track & continue your applications"
                [href]="path('applications.list')"
                icon="ClipboardList"
              />
            </div>

            <!-- AI Search Input -->
            <div class="max-w-2xl mx-auto">
              <bc-ai-assistant variant="inline" placeholder="Enter a prompt here" />
            </div>

            <!-- Renewals Status Box -->
            <div class="max-w-2xl mx-auto mt-6">
              <renewals-box [licences]="licences" />
            </div>

            <!-- AI-generated proactive reminders -->
            <div class="max-w-2xl mx-auto mt-6">
              <smart-reminders [items]="reminderItems" />
            </div>
          </div>

          <!-- Quick Stats / Overview Cards -->
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 mt-12">
            <dashboard-stat-card label="Active Licences" value="3" [href]="path('licences.list')" />
            <dashboard-stat-card label="Pending Applications" value="2" [href]="path('applications.list') + '?status=pending'" [highlight]="true" />
            <dashboard-stat-card label="Upcoming Renewals" value="1" [href]="path('licences.list') + '?filter=renewal'" />
            <dashboard-stat-card label="Unread Notices" [value]="unreadNotices" [href]="path('dashboard.notices')" [highlight]="unreadNoticeCount > 0" />
          </div>

          <!-- Recent Activity Section -->
          <div class="mt-12">
            <div class="mb-4 flex items-center justify-between">
              <h3 class="text-lg font-semibold text-foreground">Recent Activity</h3>
              <a [href]="path('applications.list')" class="inline-flex items-center gap-1 text-sm font-medium text-[#013366] hover:underline">
                View all applications
                <svg lucide="ArrowRight" class="h-4 w-4"></svg>
              </a>
            </div>
            <div class="bg-white border rounded-lg divide-y">
              <a
                dashboardActivityItem
                type="application"
                title="Liquor Primary Licence"
                status="In Review"
                date="May 25, 2026"
                [href]="path('applications.submitted', { id: 'LP-125998' })"
              ></a>
              <a
                dashboardActivityItem
                type="licence"
                title="Liquor Primary - OneStop Test 1"
                status="Renewal Due"
                date="Apr 30, 2027"
                [href]="path('licences.detail', { id: '326739' })"
              ></a>
            </div>
          </div>
        </div>
      </main>

      <bc-gov-footer />
    </div>
  `,
})
export class DashboardPage {
  protected readonly path = path;
  protected readonly licences = mockLicences;
  private readonly notificationsRepository = inject(NOTIFICATIONS_REPOSITORY);
  // Unread notices are derived from the shared notices data so the dashboard stat stays consistent with the Notices page.
  protected readonly unreadNoticeCount = this.notificationsRepository.notices().filter((n) => !n.read).length;
  protected readonly reminderItems = buildReminderItems(this.unreadNoticeCount);
  protected readonly unreadNotices = String(this.unreadNoticeCount);
}
