/** Activity & Integrations console: audit trail, notification delivery log (simulated sends) and Dynamics sync status with retry. */
import { NgTemplateOutlet } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, input, signal } from '@angular/core';
import { injectSession } from '@core/session/session-provider';
import { Badge } from '@shared/ui/primitives/badge';
import { Button } from '@shared/ui/primitives/button';
import { Card, CardContent, CardHeader, CardTitle } from '@shared/ui/primitives/card';
import { type IconName } from '@shared/ui/primitives/icons';
import { Input } from '@shared/ui/primitives/input';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { ScrollArea } from '@shared/ui/primitives/scroll-area';
import { SELECT } from '@shared/ui/primitives/select';
import { TABS } from '@shared/ui/primitives/tabs';
import { actorName } from '@domain/roles';
import { type AuditCategory, type AuditEntry } from '@data-access/repositories/activity/audit-service';
import { AI_REPOSITORY } from '@data-access/repositories/ai/ai.repository';
import { ACTIVITY_REPOSITORY } from '@data-access/repositories/activity/activity.repository';
import { NOTIFICATIONS_REPOSITORY } from '@data-access/repositories/notifications/notifications.repository';

const CATEGORY_META: Record<AuditCategory, { label: string; icon: IconName; color: string }> = {
  auth: { label: 'Auth', icon: 'ShieldCheck', color: 'text-[#38598A]' },
  application: { label: 'Application', icon: 'FileText', color: 'text-[#013366]' },
  payment: { label: 'Payment', icon: 'CreditCard', color: 'text-green-700' },
  review: { label: 'Review', icon: 'CheckCircle2', color: 'text-amber-700' },
  document: { label: 'Document', icon: 'FileText', color: 'text-slate-700' },
  notification: { label: 'Notification', icon: 'Bell', color: 'text-blue-700' },
  integration: { label: 'Integration', icon: 'Plug', color: 'text-purple-700' },
  system: { label: 'System', icon: 'Cog', color: 'text-muted-foreground' },
};

function formatTime(iso: string): string {
  return new Date(iso).toLocaleString('en-CA', {
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  });
}

/** Attribute component on the real element, so `space-y-*` on the parent still applies to it. */
@Component({
  selector: 'div[activityEmptyState]',
  imports: [LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { class: 'flex flex-col items-center justify-center gap-3 rounded-lg border border-dashed py-16 text-center' },
  template: `
    <svg [lucide]="icon()" class="h-8 w-8 text-muted-foreground" aria-hidden="true"></svg>
    <p class="max-w-sm text-sm text-muted-foreground">{{ text() }}</p>
  `,
})
class EmptyState {
  readonly icon = input.required<IconName>();
  readonly text = input.required<string>();
}

interface AiResult {
  answer: string;
  interpretation?: string;
  matchIds: string[];
}

@Component({
  selector: 'activity-audit-tab',
  imports: [Button, EmptyState, Input, LucideIcon, NgTemplateOutlet, ScrollArea, SELECT],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <ng-template #row let-e>
      <li class="flex gap-3 px-4 py-3">
        <svg [lucide]="meta(e).icon" [class]="'mt-0.5 h-4 w-4 shrink-0 ' + meta(e).color" aria-hidden="true"></svg>
        <div class="min-w-0 flex-1">
          <div class="flex items-center justify-between gap-2">
            <span class="text-[11px] font-medium uppercase tracking-wide text-muted-foreground">
              {{ meta(e).label }}
            </span>
            <span class="shrink-0 text-xs text-muted-foreground">{{ time(e.at) }}</span>
          </div>
          <p class="text-sm text-foreground">{{ e.summary }}</p>
          <p class="mt-0.5 text-xs text-muted-foreground">
            {{ e.actor }}{{ e.subjectId ? ' · ' + e.subjectId : '' }}
          </p>
        </div>
      </li>
    </ng-template>

    <div class="space-y-4">
      <!-- AI natural-language search -->
      <div class="rounded-lg border border-[#013366]/20 bg-[#013366]/5 p-3">
        <div class="flex items-center gap-1.5">
          <svg lucide="Sparkles" class="h-4 w-4 text-[#013366]" aria-hidden="true"></svg>
          <span class="text-sm font-medium text-[#013366]">Ask about activity</span>
          <span
            class="rounded-full bg-[#013366]/10 px-2 py-0.5 text-[10px] font-medium uppercase tracking-wide text-[#013366]"
          >
            AI
          </span>
        </div>
        <div class="mt-2 flex flex-col gap-2 sm:flex-row">
          <input
            uiInput
            [value]="aiQuery()"
            (input)="aiQuery.set($any($event.target).value)"
            (keydown)="onAiKeydown($event)"
            placeholder='e.g. "SEP denials this month" or "payments by Dana Lee"'
            class="flex-1 bg-background"
            aria-label="Ask about activity in natural language"
          />
          <div class="flex gap-2">
            <button
              uiButton
              (click)="runAiSearch()"
              [disabled]="aiLoading() || !aiQuery().trim()"
              class="gap-1.5 bg-[#013366] hover:bg-[#012a52]"
            >
              @if (aiLoading()) {
                <svg lucide="Loader2" class="h-4 w-4 animate-spin"></svg>
              } @else {
                <svg lucide="Sparkles" class="h-4 w-4"></svg>
              }
              {{ aiLoading() ? 'Searching' : 'Search' }}
            </button>
            @if (aiResult()) {
              <button uiButton variant="outline" (click)="clearAi()">Clear</button>
            }
          </div>
        </div>
        @if (aiError()) {
          <p class="mt-2 text-sm text-destructive">{{ aiError() }}</p>
        }
        @if (aiResult(); as result) {
          <div class="mt-3 rounded-md border border-border bg-background p-3">
            <p class="text-sm text-foreground">{{ result.answer }}</p>
            @if (result.interpretation) {
              <p class="mt-1 text-xs text-muted-foreground">{{ result.interpretation }}</p>
            }
            <p class="mt-1 text-xs font-medium text-muted-foreground">
              {{ aiEntries()?.length ?? 0 }} matching {{ aiEntries()?.length === 1 ? 'entry' : 'entries' }} shown
              below.
            </p>
          </div>
        }
      </div>

      @if (aiEntries(); as ai) {
        @if (ai.length === 0) {
          <div activityEmptyState icon="Activity" text="No activity matched your question. Try rephrasing or clearing the AI search."></div>
        } @else {
          <div uiScrollArea class="h-[24rem] rounded-lg border">
            <ul class="divide-y">
              @for (e of ai; track e.id) {
                <ng-container [ngTemplateOutlet]="row" [ngTemplateOutletContext]="{ $implicit: e }" />
              }
            </ul>
          </div>
        }
      } @else {
        <div class="flex flex-col gap-3 sm:flex-row sm:items-center">
          <div class="relative flex-1">
            <svg
              lucide="Search"
              class="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground"
            ></svg>
            <input
              uiInput
              [value]="search()"
              (input)="search.set($any($event.target).value)"
              placeholder="Search actions, actors, references..."
              class="pl-9"
              aria-label="Search audit trail"
            />
          </div>
          <ui-select [value]="category()" (valueChange)="category.set($any($event))">
            <button uiSelectTrigger class="sm:w-52" aria-label="Filter by category">
              <span uiSelectValue></span>
            </button>
            <ng-template uiSelectContent>
              <ui-select-item value="all">All categories</ui-select-item>
              @for (c of categories; track c[0]) {
                <ui-select-item [value]="c[0]">{{ c[1].label }}</ui-select-item>
              }
            </ng-template>
          </ui-select>
        </div>

        @if (entries().length === 0) {
          <div activityEmptyState icon="Activity" text="No activity recorded yet. Actions across the portal will appear here."></div>
        } @else {
          <div uiScrollArea class="h-[28rem] rounded-lg border">
            <ul class="divide-y">
              @for (e of entries(); track e.id) {
                <ng-container [ngTemplateOutlet]="row" [ngTemplateOutletContext]="{ $implicit: e }" />
              }
            </ul>
          </div>
        }
      }
    </div>
  `,
})
class AuditTab {
  private readonly activityRepository = inject(ACTIVITY_REPOSITORY);
  private readonly aiRepository = inject(AI_REPOSITORY);
  private readonly all = this.activityRepository.auditTrail();
  private readonly allLength = computed(() => this.all().length);
  protected readonly category = signal<AuditCategory | 'all'>('all');
  protected readonly search = signal('');
  protected readonly categories = Object.entries(CATEGORY_META);

  // AI natural-language search state
  protected readonly aiQuery = signal('');
  protected readonly aiLoading = signal(false);
  protected readonly aiError = signal<string | null>(null);
  protected readonly aiResult = signal<AiResult | null>(null);

  protected readonly meta = (e: AuditEntry) => CATEGORY_META[e.category];
  protected readonly time = formatTime;

  protected async runAiSearch(): Promise<void> {
    if (!this.aiQuery().trim()) return;
    this.aiLoading.set(true);
    this.aiError.set(null);
    try {
      const payload = this.activityRepository.listAudit()
        .slice(0, 200)
        .map((e) => ({
          id: e.id,
          at: e.at,
          category: e.category,
          action: e.action,
          summary: e.summary,
          actor: e.actor,
          subjectId: e.subjectId ?? null,
        }));
      const res = await this.aiRepository.fetchApi('/api/activity-search', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ query: this.aiQuery(), entries: payload, today: new Date().toISOString().slice(0, 10) }),
      });
      if (!res.ok) throw new Error('failed');
      this.aiResult.set(await res.json());
    } catch {
      this.aiError.set('Search failed. Try rephrasing your question.');
    } finally {
      this.aiLoading.set(false);
    }
  }

  protected onAiKeydown(e: KeyboardEvent): void {
    if (e.key === 'Enter') {
      e.preventDefault();
      void this.runAiSearch();
    }
  }

  protected clearAi(): void {
    this.aiResult.set(null);
    this.aiQuery.set('');
    this.aiError.set(null);
  }

  // When an AI result is present, show only the matched entries (in match order).
  protected readonly aiEntries = computed(() => {
    const result = this.aiResult();
    if (!result) return null;
    this.allLength();
    const byId = new Map(this.activityRepository.listAudit().map((e) => [e.id, e]));
    return result.matchIds.map((id) => byId.get(id)).filter((e): e is AuditEntry => Boolean(e));
  });

  protected readonly entries = computed(() => {
    // re-run when the underlying trail changes
    this.allLength();
    return this.activityRepository.filterAudit({ category: this.category(), search: this.search() });
  });
}

@Component({
  selector: 'activity-delivery-tab',
  imports: [Badge, EmptyState, LucideIcon, ScrollArea],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (log().length === 0) {
      <div activityEmptyState icon="Mail" text="No email or SMS messages have been sent yet."></div>
    } @else {
      <div uiScrollArea class="h-[28rem] rounded-lg border">
        <ul class="divide-y">
          @for (d of log(); track d.id) {
            <li class="flex gap-3 px-4 py-3">
              <svg lucide="Mail" class="mt-0.5 h-4 w-4 shrink-0 text-blue-700" aria-hidden="true"></svg>
              <div class="min-w-0 flex-1">
                <div class="flex items-center justify-between gap-2">
                  <span uiBadge variant="outline" class="text-[10px] uppercase">
                    {{ d.channel }}
                  </span>
                  <span class="shrink-0 text-xs text-muted-foreground">{{ time(d.at) }}</span>
                </div>
                <p class="text-sm font-medium text-foreground">{{ d.subject }}</p>
                <p class="text-xs text-muted-foreground">To: {{ d.to }}</p>
                <p class="mt-0.5 text-sm text-muted-foreground">{{ d.preview }}</p>
              </div>
            </li>
          }
        </ul>
      </div>
    }
  `,
})
class DeliveryTab {
  private readonly activityRepository = inject(ACTIVITY_REPOSITORY);
  private readonly notificationsRepository = inject(NOTIFICATIONS_REPOSITORY);
  // The prototype reads the delivery log on render, so re-read it whenever audit or sync data changes.
  private readonly audit = this.activityRepository.auditTrail();
  private readonly sync = this.activityRepository.syncRecords();
  protected readonly log = computed(() => (this.audit(), this.sync(), this.notificationsRepository.deliveryLog()));
  protected readonly time = formatTime;
}

@Component({
  selector: 'activity-sync-tab',
  imports: [Button, EmptyState, LucideIcon, ScrollArea],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (records().length === 0) {
      <div activityEmptyState icon="Plug" text="No cases have been pushed to MS Dynamics yet."></div>
    } @else {
      <div uiScrollArea class="h-[28rem] rounded-lg border">
        <ul class="divide-y">
          @for (r of records(); track r.id) {
            @let ok = r.state === 'synced';
            @let pending = r.state === 'pending';
            <li class="flex items-start gap-3 px-4 py-3">
              @if (ok) {
                <svg lucide="CheckCircle2" class="mt-0.5 h-4 w-4 shrink-0 text-green-700" aria-hidden="true"></svg>
              } @else if (pending) {
                <svg lucide="Clock" class="mt-0.5 h-4 w-4 shrink-0 text-amber-600" aria-hidden="true"></svg>
              } @else {
                <svg lucide="AlertTriangle" class="mt-0.5 h-4 w-4 shrink-0 text-[#D8292F]" aria-hidden="true"></svg>
              }
              <div class="min-w-0 flex-1">
                <div class="flex items-center justify-between gap-2">
                  <span class="text-sm font-medium text-foreground">{{ r.reference }}</span>
                  <span class="shrink-0 text-xs text-muted-foreground">{{ time(r.lastAttemptAt) }}</span>
                </div>
                <p class="text-xs text-muted-foreground">{{ r.caseType }} · {{ r.applicant }}</p>
                @if (ok) {
                  <p class="mt-0.5 text-xs text-green-700">Synced as {{ r.dynamicsCaseId }}</p>
                } @else {
                  <div class="mt-1 flex items-center gap-2">
                    <p class="text-xs text-[#D8292F]">{{ r.error }}</p>
                    <button uiButton size="sm" variant="outline" class="h-7 gap-1 text-xs" (click)="retry(r.id)">
                      <svg lucide="RefreshCw" class="h-3 w-3"></svg> Retry
                    </button>
                  </div>
                }
              </div>
            </li>
          }
        </ul>
      </div>
    }
  `,
})
class SyncTab {
  private readonly activityRepository = inject(ACTIVITY_REPOSITORY);
  protected readonly records = this.activityRepository.syncRecords();
  private readonly sessionProvider = injectSession();
  protected readonly time = formatTime;

  protected retry(id: string): void {
    this.activityRepository.retrySync(id, actorName(this.sessionProvider.session()));
  }
}

@Component({
  selector: 'activity-console',
  imports: [AuditTab, Card, CardContent, CardHeader, CardTitle, DeliveryTab, LucideIcon, SyncTab, TABS],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div uiCard>
      <div uiCardHeader>
        <div uiCardTitle class="flex items-center gap-2 text-[#013366]">
          <svg lucide="Activity" class="h-5 w-5" aria-hidden="true"></svg> Activity &amp; Integrations
        </div>
      </div>
      <div uiCardContent>
        <div uiTabs value="audit">
          <div uiTabsList class="mb-4">
            <button uiTabsTrigger value="audit">Audit trail ({{ audit().length }})</button>
            <button uiTabsTrigger value="delivery">Notifications</button>
            <button uiTabsTrigger value="sync" class="gap-1.5">
              Dynamics sync
              @if (failedSync() > 0) {
                <span
                  class="flex h-4 min-w-4 items-center justify-center rounded-full bg-[#D8292F] px-1 text-[10px] font-semibold text-white"
                >
                  {{ failedSync() }}
                </span>
              }
            </button>
          </div>
          <div uiTabsContent value="audit"><activity-audit-tab /></div>
          <div uiTabsContent value="delivery"><activity-delivery-tab /></div>
          <div uiTabsContent value="sync"><activity-sync-tab /></div>
        </div>
      </div>
    </div>
  `,
})
export class ActivityConsole {
  private readonly activityRepository = inject(ACTIVITY_REPOSITORY);
  protected readonly audit = this.activityRepository.auditTrail();
  private readonly sync = this.activityRepository.syncRecords();
  protected readonly failedSync = computed(() => this.sync().filter((s) => s.state === 'failed').length);
}
