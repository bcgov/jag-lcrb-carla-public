/** Licensee compliance and enforcement view; `#fragment` hrefs link within the page. Data is mocked: replace with a licensee-scoped API and a server audit trail. */
import { ChangeDetectionStrategy, Component, computed, inject, input, output, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { WorkloadSuggestions } from '@shared/ui/ai/workload-suggestions';
import { RespondDialog } from './respond-dialog';
import { Badge } from '@shared/ui/primitives/badge';
import { Button } from '@shared/ui/primitives/button';
import type { IconName } from '@shared/ui/primitives/icons';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { TABLE } from '@shared/ui/primitives/table';
import { OUTCOME_META, daysUntil, formatCurrency, formatEnforcementDate } from '@domain/compliance/enforcement';
import { type EnforcementView } from '@data-access/repositories/compliance/enforcement-store';
import { STATUS_META } from '@domain/compliance/enforcement';
import { COMPLIANCE_REPOSITORY } from '@data-access/repositories/compliance/compliance.repository';
import { path } from '@/app/route-registry';

const TONE_STAT: Record<string, { wrap: string; icon: string; value: string }> = {
  danger: { wrap: 'border-red-200 bg-red-50', icon: 'text-red-700', value: 'text-red-800' },
  caution: {
    wrap: 'border-orange-200 bg-orange-50',
    icon: 'text-orange-700',
    value: 'text-orange-800',
  },
  positive: {
    wrap: 'border-green-200 bg-green-50',
    icon: 'text-green-700',
    value: 'text-green-800',
  },
  neutral: { wrap: 'border-border bg-white', icon: 'text-[#013366]', value: 'text-[#013366]' },
};

/** On the card's own `div` because the parent uses `space-y-4`. */
@Component({
  selector: 'div[enforcementCard]',
  imports: [Badge, Button, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { class: 'bg-white border rounded-lg overflow-hidden' },
  template: `
    <div class="flex flex-wrap items-center justify-between gap-2 px-4 py-3 border-b bg-muted/30">
      <div class="flex items-center gap-3">
        <span class="font-semibold text-[#013366]">{{ action().referenceNumber }}</span>
        <span class="text-muted-foreground text-sm">{{ action().section }}</span>
      </div>
      <span uiBadge variant="outline" [class]="meta().badge">{{ meta().label }}</span>
    </div>

    <div class="p-4">
      <h3 class="text-base font-semibold text-foreground">{{ action().allegation }}</h3>
      <div class="text-sm text-muted-foreground mt-0.5">
        {{ action().establishmentName }} · #{{ action().licenceId }} · {{ action().licenceType }}
      </div>
      <p class="text-sm mt-3 leading-relaxed">{{ action().details }}</p>

      <div class="grid grid-cols-1 sm:grid-cols-2 gap-4 mt-4 text-sm">
        <div>
          <div class="text-muted-foreground">Issued</div>
          <div class="font-medium">{{ formatEnforcementDate(action().issuedDate) }}</div>
        </div>
        <div>
          <div class="text-muted-foreground">Proposed action</div>
          <div class="font-medium">{{ action().proposedAction }}</div>
        </div>
      </div>

      <!-- Deadline / status detail -->
      @if (isOpen()) {
        <div [attr.class]="deadlineClass()">
          <svg lucide="CalendarClock" class="h-4 w-4 shrink-0" aria-hidden="true"></svg>
          <span>
            Response due <strong>{{ formatEnforcementDate(action().responseDeadline) }}</strong
            >{{ dueText() }}
          </span>
        </div>
      } @else {
        <div class="mt-4 rounded-md border bg-blue-50 border-blue-200 p-3 text-sm text-blue-900">
          @if (action().response?.election === 'request-hearing' && action().response?.hearingDate) {
            <span class="flex items-center gap-2">
              <svg lucide="Gavel" class="h-4 w-4 shrink-0" aria-hidden="true"></svg>
              Hearing scheduled for <strong>{{ formatEnforcementDate(action().response!.hearingDate!) }}</strong>
            </span>
          } @else {
            <span class="flex items-center gap-2">
              <svg lucide="CheckCircle2" class="h-4 w-4 shrink-0" aria-hidden="true"></svg>
              Response submitted{{
                action().response?.submittedAt ? ' on ' + formatEnforcementDate(action().response!.submittedAt!) : ''
              }}. LCRB compliance staff will review it.
            </span>
          }
          @if (action().response?.statement) {
            <p class="mt-2 text-blue-800 italic">&ldquo;{{ action().response!.statement }}&rdquo;</p>
          }
        </div>
      }
    </div>

    @if (isOpen()) {
      <div class="bg-muted/30 px-4 py-3 border-t flex justify-end">
        <button uiButton (click)="respond.emit()" class="bg-[#013366] hover:bg-[#012a52] gap-2">
          <svg lucide="Gavel" class="h-4 w-4" aria-hidden="true"></svg>
          Respond to this action
        </button>
      </div>
    }
  `,
})
export class EnforcementCard {
  readonly action = input.required<EnforcementView>();
  readonly respond = output<void>();

  protected readonly formatEnforcementDate = formatEnforcementDate;
  protected readonly meta = computed(() => STATUS_META[this.action().status]);
  protected readonly isOpen = computed(() => this.action().status === 'awaiting-response');
  private readonly days = computed(() => daysUntil(this.action().responseDeadline));

  protected readonly deadlineClass = computed(() => {
    const days = this.days();
    const overdue = days < 0;
    const urgent = days >= 0 && days <= 7;
    return `mt-4 flex items-center gap-2 rounded-md border p-3 text-sm ${
      overdue
        ? 'bg-red-50 border-red-200 text-red-900'
        : urgent
          ? 'bg-orange-50 border-orange-200 text-orange-900'
          : 'bg-muted/40 border-border text-foreground'
    }`;
  });
  protected readonly dueText = computed(() => {
    const days = this.days();
    return days < 0
      ? ` — overdue by ${Math.abs(days)} day${Math.abs(days) === 1 ? '' : 's'}`
      : ` — ${days} day${days === 1 ? '' : 's'} remaining`;
  });
}

/** one clickable tile. */
@Component({
  selector: 'stat-card',
  imports: [RouterLink, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <a
      [routerLink]="link().path"
      [fragment]="link().fragment"
      [class]="
        'group block rounded-lg border p-4 transition-all hover:-translate-y-0.5 hover:shadow-md focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[#013366] focus-visible:ring-offset-2 ' +
        styles().wrap
      "
    >
      <div class="flex items-center gap-2">
        <svg [lucide]="icon()" [class]="'h-5 w-5 ' + styles().icon" aria-hidden="true"></svg>
        <span class="flex-1 text-sm text-muted-foreground">{{ label() }}</span>
        <svg lucide="ChevronRight" class="h-4 w-4 text-muted-foreground/60 transition-transform group-hover:translate-x-0.5" aria-hidden="true"></svg>
      </div>
      <div [class]="'mt-2 font-bold ' + styles().value + ' ' + (small() ? 'text-lg' : 'text-2xl')">{{ value() }}</div>
    </a>
  `,
})
export class StatCard {
  readonly icon = input.required<IconName>();
  readonly label = input.required<string>();
  readonly value = input.required<string>();
  readonly tone = input.required<'danger' | 'caution' | 'positive' | 'neutral'>();
  readonly small = input(false);
  /** Where the tile takes you: a `#fragment` on this page or a route. */
  readonly href = input.required<string>();

  protected readonly styles = computed(() => TONE_STAT[this.tone()]);
  protected readonly link = computed(() => {
    const href = this.href();
    return href.startsWith('#') ? { path: path('licences.compliance'), fragment: href.slice(1) } : { path: href, fragment: undefined };
  });
}

@Component({
  selector: 'empty-panel',
  imports: [LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="text-center py-12 bg-white border rounded-lg">
      <svg [lucide]="icon()" class="h-12 w-12 mx-auto text-green-600 mb-3" aria-hidden="true"></svg>
      <p class="font-medium text-foreground">{{ title() }}</p>
      <p class="text-muted-foreground text-sm mt-1">{{ message() }}</p>
    </div>
  `,
})
export class EmptyPanel {
  readonly icon = input.required<IconName>();
  readonly title = input.required<string>();
  readonly message = input.required<string>();
}

@Component({
  selector: 'enforcement-hub',
  imports: [WorkloadSuggestions, Badge, LucideIcon, ...TABLE, EnforcementCard, StatCard, EmptyPanel, RespondDialog],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="space-y-10">
      <!-- AI Suggest: prioritize compliance work -->
      <!-- A direct child of space-y-10: a block host takes the margin (its root element varies by state). -->
      <workload-suggestions
        [style.display]="'block'"
        scope="compliance"
        [stats]="[
          { label: 'Open actions', value: '' + stats().open },
          { label: 'Contraventions (12mo)', value: '' + stats().contraventions },
          { label: 'Penalties (12mo)', value: formatCurrency(stats().penalties) },
        ]"
        [items]="suggestItems()"
        [links]="links"
      />

      <!-- Overview -->
      <section aria-labelledby="overview-heading">
        <div class="mb-3 flex items-baseline gap-2">
          <h2 id="overview-heading" class="text-sm font-semibold uppercase tracking-wide text-[#013366]">At a glance</h2>
          <p class="text-sm text-muted-foreground">Select a tile to jump to the details.</p>
        </div>
        <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
          <stat-card
            icon="AlertTriangle"
            label="Open enforcement actions"
            [value]="'' + stats().open"
            [tone]="stats().open > 0 ? 'danger' : 'positive'"
            href="#active-actions-heading"
          />
          <stat-card
            icon="DollarSign"
            label="Penalties (last 12 months)"
            [value]="formatCurrency(stats().penalties)"
            [tone]="stats().penalties > 0 ? 'caution' : 'positive'"
            href="#history-heading"
          />
          <stat-card
            icon="Scale"
            label="Contraventions (last 12 months)"
            [value]="'' + stats().contraventions"
            [tone]="stats().contraventions > 0 ? 'caution' : 'positive'"
            href="#history-heading"
          />
          <stat-card
            icon="ClipboardCheck"
            label="Last inspection"
            [value]="stats().lastInspection ? formatEnforcementDate(stats().lastInspection!.date) : '—'"
            tone="neutral"
            [small]="true"
            [href]="path('dashboard.calendar')"
          />
        </div>
      </section>

      <!-- Active enforcement actions -->
      <section aria-labelledby="active-actions-heading">
        <div class="flex items-center gap-2 mb-4">
          <svg lucide="Gavel" class="h-5 w-5 text-[#013366]" aria-hidden="true"></svg>
          <h2 id="active-actions-heading" class="text-lg font-semibold text-foreground">Enforcement actions</h2>
        </div>

        @if (actions().length === 0) {
          <empty-panel icon="CheckCircle2" title="No enforcement actions" message="There are no enforcement actions against your licences." />
        } @else {
          <div class="space-y-4">
            @for (action of actions(); track action.id) {
              <div enforcementCard [action]="action" (respond)="respondTo.set(action)"></div>
            }
          </div>
        }
      </section>

      <!-- Compliance history -->
      <section aria-labelledby="history-heading">
        <div class="flex items-center gap-2 mb-4">
          <svg lucide="FileText" class="h-5 w-5 text-[#013366]" aria-hidden="true"></svg>
          <h2 id="history-heading" class="text-lg font-semibold text-foreground">Compliance &amp; inspection history</h2>
        </div>
        <div class="border rounded-lg overflow-x-auto bg-white">
          <ui-table class="min-w-[900px]">
            <thead uiTableHeader>
              <tr uiTableRow class="bg-muted/50">
                <th uiTableHead class="w-[130px]">Date</th>
                <th uiTableHead class="w-[200px]">Establishment</th>
                <th uiTableHead class="w-[110px]">Type</th>
                <th uiTableHead>Details</th>
                <th uiTableHead class="w-[160px]">Outcome</th>
              </tr>
            </thead>
            <tbody uiTableBody>
              @for (rec of history; track rec.id) {
                <tr uiTableRow>
                  <td uiTableCell class="whitespace-nowrap align-top text-sm">
                    {{ formatEnforcementDate(rec.date) }}
                    <div class="text-xs text-muted-foreground">{{ rec.id }}</div>
                  </td>
                  <td uiTableCell class="align-top">
                    <div class="font-medium text-foreground">{{ rec.establishmentName }}</div>
                    <div class="text-xs text-muted-foreground">#{{ rec.licenceId }} · {{ rec.licenceType }}</div>
                  </td>
                  <td uiTableCell class="align-top capitalize text-sm">{{ rec.kind }}</td>
                  <td uiTableCell class="align-top text-sm whitespace-normal break-words">
                    @if (rec.section) {
                      <span class="font-medium text-foreground">{{ rec.section }} — </span>
                    }
                    {{ rec.description }}
                    <div class="text-xs text-muted-foreground mt-0.5">{{ rec.officer }}</div>
                  </td>
                  <td uiTableCell class="align-top whitespace-nowrap">
                    <span uiBadge variant="outline" [class]="outcomeMeta[rec.outcome].badge">{{ outcomeMeta[rec.outcome].label }}</span>
                    @if (rec.penaltyAmount != null) {
                      <div class="text-xs text-muted-foreground mt-1">{{ formatCurrency(rec.penaltyAmount) }}</div>
                    }
                    @if (rec.suspensionDays != null) {
                      <div class="text-xs text-muted-foreground mt-1">{{ rec.suspensionDays }}-day suspension</div>
                    }
                  </td>
                </tr>
              }
            </tbody>
          </ui-table>
        </div>
      </section>

      @if (respondTo(); as target) {
        <respond-dialog [action]="target" [open]="true" (openChange)="onRespondOpenChange($event)" />
      }
    </div>
  `,
})
export class EnforcementHub {
  protected readonly path = path;
  private readonly complianceRepository = inject(COMPLIANCE_REPOSITORY);
  private readonly feed = this.complianceRepository.enforcementViews();
  protected readonly actions = computed(() => this.feed().actions);
  protected readonly respondTo = signal<EnforcementView | null>(null);

  protected readonly history = this.complianceRepository.complianceHistory();
  protected readonly outcomeMeta = OUTCOME_META;
  protected readonly formatCurrency = formatCurrency;
  protected readonly formatEnforcementDate = formatEnforcementDate;
  protected readonly links = [
    { label: 'Review enforcement actions', href: path('licences.compliance') + '#active-actions-heading' },
    { label: 'View compliance history', href: path('licences.compliance') },
  ];

  protected readonly stats = computed(() => {
    const open = this.actions().filter((a) => a.status === 'awaiting-response').length;
    const yearAgo = Date.now() - 365 * 24 * 60 * 60 * 1000;
    const recent = this.complianceRepository.complianceHistory().filter((c) => new Date(c.date).getTime() >= yearAgo);
    const penalties = recent.reduce((sum, c) => sum + (c.penaltyAmount ?? 0), 0);
    const contraventions = recent.filter((c) => c.outcome !== 'no-contravention').length;
    const lastInspection = [...this.complianceRepository.complianceHistory()]
      .filter((c) => c.kind === 'inspection')
      .sort((a, b) => (a.date < b.date ? 1 : -1))[0];
    return { open, penalties, contraventions, lastInspection };
  });

  // Context for the AI "Suggest what to prioritize" panel: outstanding enforcement actions with deadlines plus recent contraventions to follow up.
  protected readonly suggestItems = computed(() => {
    const actionItems = this.actions().map((a) => {
      const due = daysUntil(a.responseDeadline);
      const deadline = due < 0 ? `; response ${Math.abs(due)} day(s) overdue` : `; ${due} day(s) left to respond`;
      return {
        label: a.allegation,
        detail: `${STATUS_META[a.status].label}; ${a.proposedAction}; ${a.establishmentName}${deadline}`,
      };
    });
    const historyItems = this.complianceRepository.complianceHistory().filter((c) => c.outcome !== 'no-contravention')
      .slice(0, 5)
      .map((c) => ({
        label: c.description,
        detail: `${OUTCOME_META[c.outcome].label}; ${formatEnforcementDate(c.date)}${
          c.penaltyAmount ? `; penalty ${formatCurrency(c.penaltyAmount)}` : ''
        }`,
      }));
    return [...actionItems, ...historyItems];
  });

  protected onRespondOpenChange(open: boolean): void {
    if (!open) this.respondTo.set(null);
  }
}
