// Login banner shown only for the simulated One door hand-off (`?from=one-door`).
import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRoute } from '@angular/router';
import { LucideIcon } from '../primitives/lucide-icon';

@Component({
  selector: 'referral-banner',
  imports: [LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    @if (from() === 'one-door') {
      <div
        role="status"
        class="mb-8 flex items-start gap-3 rounded-lg border border-[#013366]/30 bg-white px-4 py-3 text-sm shadow-sm"
      >
        <span class="mt-0.5 rounded-md bg-[#FCBA19] p-1.5 text-[#013366]" aria-hidden="true">
          <svg lucide="ArrowLeftRight" class="h-4 w-4"></svg>
        </span>
        <div>
          <p class="font-semibold text-[#013366]">Transferred from the BC Government assistant</p>
          <p class="text-[#474543]">
            You've been connected to the Liquor and Cannabis Regulation Branch. Sign in to continue — your request will be waiting for you.
          </p>
        </div>
      </div>
    }
  `,
})
export class ReferralBanner {
  private readonly route = inject(ActivatedRoute);
  private readonly params = toSignal(this.route.queryParamMap, { initialValue: this.route.snapshot.queryParamMap });
  protected readonly from = computed(() => this.params().get('from'));
}
