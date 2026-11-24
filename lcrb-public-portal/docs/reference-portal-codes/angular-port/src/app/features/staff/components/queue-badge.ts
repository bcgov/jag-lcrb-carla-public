/** Queue and check-outcome pills. */
import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { CHECK_OUTCOME_META, QUEUE_META } from '@domain/staff/agent-log';
import { type CheckOutcome, type DynamicsQueue } from '@domain/models/staff';
import { cn } from '@shared/util/utils';

const TONE_CLASSES: Record<'danger' | 'caution' | 'info' | 'neutral', string> = {
  danger: 'bg-red-100 text-red-800 border-red-200',
  caution: 'bg-orange-100 text-orange-800 border-orange-200',
  info: 'bg-blue-100 text-blue-800 border-blue-200',
  neutral: 'bg-slate-100 text-slate-700 border-slate-200',
};

@Component({
  selector: 'queue-badge',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `<span [class]="spanClass()">{{ meta().label }}</span>`,
})
export class QueueBadge {
  readonly queue = input.required<DynamicsQueue>();
  readonly className = input<string | undefined>(undefined);
  protected readonly meta = computed(() => QUEUE_META[this.queue()]);
  protected readonly spanClass = computed(() =>
    cn('inline-flex items-center rounded-full border px-2.5 py-0.5 text-xs font-medium', TONE_CLASSES[this.meta().tone], this.className()),
  );
}

const OUTCOME_CLASSES: Record<'positive' | 'caution' | 'danger', string> = {
  positive: 'bg-green-100 text-green-800 border-green-200',
  caution: 'bg-orange-100 text-orange-800 border-orange-200',
  danger: 'bg-red-100 text-red-800 border-red-200',
};

@Component({
  selector: 'outcome-badge',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `<span [class]="spanClass()">{{ meta().label }}</span>`,
})
export class OutcomeBadge {
  readonly outcome = input.required<CheckOutcome>();
  readonly className = input<string | undefined>(undefined);
  protected readonly meta = computed(() => CHECK_OUTCOME_META[this.outcome()]);
  protected readonly spanClass = computed(() =>
    cn('inline-flex items-center rounded-full border px-2 py-0.5 text-xs font-medium', OUTCOME_CLASSES[this.meta().tone], this.className()),
  );
}
