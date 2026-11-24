import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { Badge } from '@shared/ui/primitives/badge';
import { LG_STATUS_LABELS, type LgReviewStatus } from '@domain/models/lg-review';

const STATUS_STYLES: Record<LgReviewStatus, string> = {
  'pending-review': 'bg-amber-100 text-amber-900 border-amber-200',
  'documents-requested': 'bg-blue-100 text-blue-900 border-blue-200',
  'community-review': 'bg-teal-100 text-teal-900 border-teal-200',
  'no-comment': 'bg-green-100 text-green-900 border-green-200',
  reject: 'bg-red-100 text-red-900 border-red-200',
  completed: 'bg-slate-100 text-slate-800 border-slate-200',
  cancelled: 'bg-zinc-100 text-zinc-600 border-zinc-200 line-through decoration-zinc-400',
};

@Component({
  selector: 'lg-status-badge',
  imports: [Badge],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `<span uiBadge variant="outline" [class]="styles[status()]">{{ labels[status()] }}</span>`,
})
export class LgStatusBadge {
  readonly status = input.required<LgReviewStatus>();
  protected readonly styles = STATUS_STYLES;
  protected readonly labels = LG_STATUS_LABELS;
}
