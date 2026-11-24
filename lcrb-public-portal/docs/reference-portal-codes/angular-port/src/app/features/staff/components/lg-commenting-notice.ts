/** LCRB view of LG and Indigenous Nation commenting opt-outs, read from the client preference the LG portal writes. In production read it per jurisdiction. */
import { ChangeDetectionStrategy, Component, computed, inject, input } from '@angular/core';
import { Badge } from '@shared/ui/primitives/badge';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { formatLgDate } from '@domain/lg-review/due-dates';
import { COMMENT_HOLD_DAYS } from '@domain/lg-review/lg-commenting';
import { LG_REVIEW_REPOSITORY } from '@data-access/repositories/lg-review/lg-review.repository';

@Component({
  selector: 'lg-commenting-notice',
  imports: [Badge, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (lg.loaded()) {
      <section [class]="'rounded-lg border border-border bg-card p-5 ' + className()">
        <div class="flex items-start justify-between gap-3">
          <h2 class="flex items-center gap-2 text-sm font-semibold uppercase tracking-wide text-muted-foreground">
            @if (optedOut()) {
              <svg lucide="MessageSquareOff" class="h-4 w-4 text-amber-700"></svg>
            } @else {
              <svg lucide="MessageSquare" class="h-4 w-4 text-[#013366]"></svg>
            }
            Local government commenting
          </h2>
          <span
            uiBadge
            variant="outline"
            [class]="optedOut() ? 'border-amber-300 bg-amber-50 text-amber-900' : 'border-green-300 bg-green-50 text-green-900'"
          >
            {{ optedOut() ? '1 opted out' : 'All commenting' }}
          </span>
        </div>

        @if (optedOut()) {
          <div class="mt-3 rounded-md border border-amber-200 bg-amber-50 p-3 text-sm text-amber-900">
            <p class="font-medium">
              {{ lg.preference().updatedBy ? lg.preference().updatedBy + "'s jurisdiction" : 'One jurisdiction' }} has
              opted out of commenting
            </p>
            <p class="mt-1">
              New applications referred there are <strong>not held</strong> for the{{ ' ' }}{{ holdDays }}-day comment period — no comment is coming, so these cases can be
              moved along in Dynamics right away.
            </p>
            @if (lg.preference().effectiveFrom) {
              <p class="mt-1.5 text-xs">
                In effect for applications referred on or after {{ formatLgDate(lg.preference().effectiveFrom!) }}.
                Applications referred before then are still open for comment on their original
                deadline.
              </p>
            }
          </div>
        } @else {
          <p class="mt-3 text-sm text-muted-foreground">
            All jurisdictions are currently participating in commenting. Applications are held for up
            to {{ holdDays }} days awaiting local government or Indigenous Nation input before
            they can be moved along.
          </p>
        }
      </section>
    }
  `,
})
export class LgCommentingNotice {
  private readonly lgReviewRepository = inject(LG_REVIEW_REPOSITORY);
  readonly className = input('');
  protected readonly lg = this.lgReviewRepository.commenting();
  protected readonly holdDays = COMMENT_HOLD_DAYS;
  protected readonly formatLgDate = formatLgDate;
  protected readonly optedOut = computed(() => this.lg.preference().optedOut);
}
