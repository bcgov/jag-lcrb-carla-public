/** Shown to LG and Indigenous Nation reviewers while their jurisdiction has opted out of commenting. Renders nothing until the preference loads. */
import { ChangeDetectionStrategy, Component, computed, inject, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { injectSession } from '@core/session/session-provider';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { isAccountOwner } from '@domain/roles';
import { LG_REVIEW_REPOSITORY } from '@data-access/repositories/lg-review/lg-review.repository';
import { path } from '@/app/route-registry';

@Component({
  selector: 'commenting-off-banner',
  imports: [RouterLink, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (loaded() && preference().optedOut) {
      <div [attr.class]="'flex items-start gap-3 rounded-lg border border-amber-200 bg-amber-50 p-4 text-amber-900 ' + className()">
        <svg lucide="MessageSquareOff" class="mt-0.5 h-5 w-5 shrink-0"></svg>
        <div class="text-sm">
          <p class="font-medium">Commenting is turned off for {{ jurisdiction() }}</p>
          <p class="mt-0.5">
            New licence applications are no longer sent to you for comment — they go straight to the LCRB for processing.{{ pendingText() }}
          </p>
          <p class="mt-1.5">
            @if (owner()) {
              <a [routerLink]="path('account.settings')" class="font-medium underline underline-offset-2 hover:no-underline">Change this in Account Settings</a>
            } @else {
              Only the account owner can change this setting.
            }
          </p>
        </div>
      </div>
    }
  `,
})
export class CommentingOffBanner {
  protected readonly path = path;
  private readonly lgReviewRepository = inject(LG_REVIEW_REPOSITORY);
  readonly className = input('');

  private readonly session = injectSession().session;
  private readonly commenting = this.lgReviewRepository.commenting();
  private readonly store = this.lgReviewRepository.store();

  protected readonly preference = this.commenting.preference;
  protected readonly loaded = this.commenting.loaded;
  protected readonly jurisdiction = computed(() => this.session()?.organization ?? 'your jurisdiction');
  protected readonly owner = computed(() => isAccountOwner(this.session()));
  private readonly pending = computed(() => this.lgReviewRepository.grandfatheredReferrals(this.preference(), this.store.applications()));
  protected readonly pendingText = computed(() => {
    const n = this.pending().length;
    return n > 0
      ? ` ${n} application${n === 1 ? '' : 's'} referred before this change ${n === 1 ? 'is' : 'are'} still open for your comment and ${n === 1 ? 'keeps its' : 'keep their'} original due date.`
      : '';
  });
}
