/** Queue tabs, case list and agent-log drawer. */
import { ChangeDetectionStrategy, Component, computed, inject, input, linkedSignal, output, signal } from '@angular/core';
import { injectSession } from '@core/session/session-provider';
import { Button } from '@shared/ui/primitives/button';
import { CARD } from '@shared/ui/primitives/card';
import { Input } from '@shared/ui/primitives/input';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { SHEET } from '@shared/ui/primitives/sheet';
import { QUEUE_META } from '@domain/staff/agent-log';
import { type DynamicsQueue } from '@domain/models/staff';
import { actorName } from '@domain/roles';
import { relativeAge } from '@domain/staff/lcrb-analytics';
import { type CaseSyncRecord } from '@data-access/repositories/activity/dynamics-service';
import { CaseAgentLog } from './case-agent-log';
import { QueueBadge } from './queue-badge';
import { ACTIVITY_REPOSITORY } from '@data-access/repositories/activity/activity.repository';

const ALL_QUEUES: DynamicsQueue[] = ['beyond-licence-cap', 'section-119-review', 'enhanced-review', 'standard-processing'];

@Component({
  selector: 'sync-pill',
  imports: [LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (record().state === 'synced') {
      <span class="inline-flex items-center gap-1 text-green-700">
        <svg lucide="CheckCircle2" class="h-3.5 w-3.5"></svg>
        Synced to Dynamics
      </span>
    } @else if (record().state === 'failed') {
      <span class="inline-flex items-center gap-1 text-red-700">
        <svg lucide="XCircle" class="h-3.5 w-3.5"></svg>
        Sync failed
      </span>
    } @else {
      <span class="inline-flex items-center gap-1 text-muted-foreground">
        <svg lucide="RefreshCw" class="h-3.5 w-3.5"></svg>
        Sync pending
      </span>
    }
  `,
})
export class SyncPill {
  readonly record = input.required<CaseSyncRecord>();
}

/** The prototype's QueueTab, on its own `button`. */
@Component({
  selector: 'button[triageQueueTab]',
  imports: [LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { type: 'button', '[class]': 'hostClass()' },
  template: `
    @if (tone() === 'danger' && !active()) {
      <svg lucide="ShieldAlert" class="h-3.5 w-3.5 text-red-600"></svg>
    }
    {{ label() }}
    <span [class]="countClass()">{{ count() }}</span>
  `,
})
export class QueueTab {
  readonly label = input.required<string>();
  readonly count = input.required<number>();
  readonly active = input.required<boolean>();
  readonly tone = input<'danger' | 'caution' | 'info' | 'neutral' | undefined>(undefined);
  protected readonly hostClass = computed(
    () =>
      `inline-flex items-center gap-2 rounded-full border px-3 py-1.5 text-sm font-medium transition-colors ${
        this.active() ? 'bg-[#013366] text-white border-[#013366]' : 'bg-background text-foreground border-border hover:border-[#013366]'
      }`,
  );
  protected readonly countClass = computed(() => `rounded-full px-1.5 text-xs ${this.active() ? 'bg-white/20' : 'bg-muted text-muted-foreground'}`);
}

/** The prototype's CaseRow: a card, on its own `div` because the list styles its direct children. */
@Component({
  selector: 'div[uiCard][triageCaseRow]',
  imports: [...CARD, Button, LucideIcon, QueueBadge, SyncPill],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div uiCardContent class="p-4">
      <div class="flex flex-col lg:flex-row lg:items-center gap-4">
        <div class="flex-1 min-w-0">
          <div class="flex flex-wrap items-center gap-2">
            <button type="button" (click)="opened.emit()" class="font-semibold text-[#013366] hover:underline text-left">
              {{ record().reference }}
            </button>
            <queue-badge [queue]="record().queue ?? 'standard-processing'" />
            @if (record().agentLog?.requiresHumanReview) {
              <span class="inline-flex items-center gap-1 text-xs text-orange-700">
                <svg lucide="ShieldAlert" class="h-3.5 w-3.5"></svg>
                Review required
              </span>
            }
          </div>
          <p class="text-sm text-muted-foreground mt-1">{{ record().caseType }} · {{ record().applicant }}</p>
          <div class="mt-2 flex flex-wrap items-center gap-x-4 gap-y-1 text-xs text-muted-foreground">
            <sync-pill [record]="record()" />
            @if (record().agentLog; as log) {
              <span>Agent confidence {{ log.overallConfidence }}%</span>
            }
            <span>Synced {{ relativeAge(record().lastAttemptAt) }}</span>
          </div>
        </div>

        <div class="flex items-center gap-2 shrink-0">
          @if (record().state === 'failed') {
            <button uiButton variant="outline" size="sm" class="gap-1" (click)="retried.emit()">
              <svg lucide="RefreshCw" class="h-4 w-4"></svg>
              Retry sync
            </button>
          }
          <button uiButton size="sm" class="bg-[#013366] hover:bg-[#012a52] gap-1" (click)="opened.emit()">
            Open
            <svg lucide="ArrowRight" class="h-4 w-4"></svg>
          </button>
        </div>
      </div>
    </div>
  `,
})
export class CaseRow {
  readonly record = input.required<CaseSyncRecord>();
  readonly opened = output<void>();
  readonly retried = output<void>();
  protected readonly relativeAge = relativeAge;
}

@Component({
  selector: 'triage-view',
  imports: [...CARD, ...SHEET, Input, LucideIcon, QueueTab, CaseRow, SyncPill, CaseAgentLog],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div>
      <!-- Queue tabs -->
      <div class="flex flex-wrap gap-2 mb-5">
        <button triageQueueTab label="All cases" [count]="counts()['all']" [active]="activeQueue() === 'all'" (click)="activeQueue.set('all')"></button>
        @for (q of allQueues; track q) {
          <button
            triageQueueTab
            [label]="queueMeta[q].label"
            [count]="counts()[q] ?? 0"
            [active]="activeQueue() === q"
            [tone]="queueMeta[q].tone"
            (click)="activeQueue.set(q)"
          ></button>
        }
      </div>

      <!-- Active queue description -->
      @if (activeQueueDescription(); as description) {
        <p class="text-sm text-muted-foreground mb-4">{{ description }}</p>
      }

      <!-- Search -->
      <div class="relative mb-5 max-w-md">
        <svg lucide="Search" class="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground"></svg>
        <input
          uiInput
          placeholder="Search by reference, applicant, or Dynamics case #..."
          [value]="query()"
          (input)="query.set($any($event.target).value)"
          class="pl-10"
        />
      </div>

      <p class="text-sm text-muted-foreground mb-4">
        Showing {{ filtered().length }} case{{ filtered().length === 1 ? '' : 's' }}
      </p>

      <div class="space-y-3">
        @for (c of filtered(); track c.id) {
          <div uiCard triageCaseRow class="hover:shadow-md transition-shadow" [record]="c" (opened)="selectedId.set(c.id)" (retried)="handleRetry(c.id)"></div>
        }
        @if (filtered().length === 0) {
          <div uiCard>
            <div uiCardContent class="py-12 text-center text-muted-foreground">No cases in this queue.</div>
          </div>
        }
      </div>

      <!-- Detail drawer -->
      <ui-sheet [state]="selected() !== null ? 'open' : 'closed'" (stateChanged)="$event === 'closed' && selectedId.set(null)">
        <ui-sheet-content *uiSheetPortal class="w-full sm:max-w-xl overflow-y-auto">
          @if (selected(); as sel) {
            <div uiSheetHeader>
              <h2 uiSheetTitle class="text-left">{{ sel.reference }}</h2>
              <p uiSheetDescription class="text-left">{{ sel.caseType }} · {{ sel.applicant }}</p>
            </div>
            <div class="mt-4 px-4 pb-6">
              <div class="flex flex-wrap items-center gap-2 mb-4 text-xs text-muted-foreground">
                <sync-pill [record]="sel" />
                @if (sel.dynamicsCaseId) {
                  <span class="inline-flex items-center gap-1">
                    <svg lucide="Database" class="h-3.5 w-3.5"></svg>
                    {{ sel.dynamicsCaseId }}
                  </span>
                }
                <span>Synced {{ relativeAge(sel.lastAttemptAt) }}</span>
              </div>
              @if (sel.agentLog; as log) {
                <case-agent-log [log]="log" [applicantName]="sel.applicant" />
              } @else {
                <p class="text-sm text-muted-foreground">No pre-screening log was attached to this case.</p>
              }
            </div>
          }
        </ui-sheet-content>
      </ui-sheet>
    </div>
  `,
})
export class TriageView {
  private readonly activityRepository = inject(ACTIVITY_REPOSITORY);
  readonly cases = input.required<CaseSyncRecord[]>();
  readonly initialQueue = input<DynamicsQueue | 'all' | undefined>(undefined);
  /** When set, the matching case drawer opens automatically on mount. */
  readonly initialCaseId = input<string | null | undefined>(undefined);

  private readonly session = injectSession();
  protected readonly allQueues = ALL_QUEUES;
  protected readonly queueMeta = QUEUE_META;
  protected readonly relativeAge = relativeAge;

  // The prototype's useState initial values: seeded from the props once, then local.
  protected readonly caseList = linkedSignal(() => this.cases());
  protected readonly activeQueue = linkedSignal<DynamicsQueue | 'all'>(() => this.initialQueue() ?? 'all');
  protected readonly query = signal('');
  protected readonly selectedId = linkedSignal<string | null>(() => {
    const id = this.initialCaseId();
    return id && this.cases().some((c) => c.id === id) ? id : null;
  });

  protected readonly counts = computed(() => {
    const list = this.caseList();
    const map: Record<string, number> = { all: list.length };
    for (const q of ALL_QUEUES) map[q] = 0;
    for (const c of list) map[c.queue ?? 'standard-processing']++;
    return map;
  });

  protected readonly filtered = computed(() => {
    let list = this.caseList();
    const active = this.activeQueue();
    if (active !== 'all') {
      list = list.filter((c) => (c.queue ?? 'standard-processing') === active);
    }
    const q = this.query().trim().toLowerCase();
    if (q) {
      list = list.filter((c) => [c.reference, c.caseType, c.applicant, c.dynamicsCaseId ?? ''].join(' ').toLowerCase().includes(q));
    }
    // Highest-risk queues first, then most recent.
    const order: Record<DynamicsQueue, number> = {
      'beyond-licence-cap': 0,
      'section-119-review': 1,
      'enhanced-review': 2,
      'standard-processing': 3,
    };
    return [...list].sort((a, b) => {
      const qa = order[a.queue ?? 'standard-processing'];
      const qb = order[b.queue ?? 'standard-processing'];
      if (qa !== qb) return qa - qb;
      return a.lastAttemptAt < b.lastAttemptAt ? 1 : -1;
    });
  });

  protected readonly selected = computed(() => this.caseList().find((c) => c.id === this.selectedId()) ?? null);
  protected readonly activeQueueDescription = computed(() => {
    const q = this.activeQueue();
    return q === 'all' ? null : QUEUE_META[q].description;
  });

  protected handleRetry(id: string): void {
    const updated = this.activityRepository.retrySync(id, actorName(this.session.session()));
    if (updated) this.caseList.update((prev) => prev.map((c) => (c.id === updated.id ? updated : c)));
  }

}
