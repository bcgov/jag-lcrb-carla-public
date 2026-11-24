import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { Badge } from '../primitives/badge';
import { SEP_STATUS_LABELS, SEP_STATUS_STYLES } from '@domain/special-events/seps';
import { type SepStatus } from '@domain/models/special-events';

@Component({
  selector: 'sep-status-badge',
  imports: [Badge],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `<span uiBadge variant="outline" [class]="styles[status()]">{{ labels[status()] }}</span>`,
})
export class SepStatusBadge {
  readonly status = input.required<SepStatus>();
  protected readonly styles = SEP_STATUS_STYLES;
  protected readonly labels = SEP_STATUS_LABELS;
}
