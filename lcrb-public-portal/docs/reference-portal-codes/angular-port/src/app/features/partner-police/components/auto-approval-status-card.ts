/** Dashboard glance at the agency's SEP auto-approval policy, linking to Account Settings, Auto-approval, where it can be changed. */
import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Badge } from '@shared/ui/primitives/badge';
import { CARD } from '@shared/ui/primitives/card';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { MIN_NOTICE_DAYS } from '@domain/special-events/auto-approval';
import { SPECIAL_EVENTS_REPOSITORY } from '@data-access/repositories/special-events/special-events.repository';
import { path } from '@/app/route-registry';

@Component({
  selector: 'auto-approval-status-card',
  imports: [RouterLink, Badge, ...CARD, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div uiCard>
      <div uiCardContent class="p-4">
        <div class="flex items-start justify-between gap-3">
          <div class="flex items-start gap-3">
            <div [attr.class]="'rounded-lg p-2 ' + (policy().enabled ? 'bg-green-100 text-green-800' : 'bg-muted text-muted-foreground')">
              @if (policy().enabled) {
                <svg lucide="ShieldCheck" class="h-4 w-4"></svg>
              } @else {
                <svg lucide="ShieldOff" class="h-4 w-4"></svg>
              }
            </div>
            <div>
              <p class="font-medium text-foreground">SEP auto-approval</p>
              <p class="mt-0.5 text-sm text-muted-foreground">
                {{
                  policy().enabled
                    ? 'On — eligible permits filed ' + minNoticeDays + '+ days ahead are approved automatically.'
                    : 'Off — every permit is referred to an officer.'
                }}
              </p>
            </div>
          </div>
          <span
            uiBadge
            variant="outline"
            [class]="policy().enabled ? 'border-green-200 bg-green-50 text-green-800' : 'border-zinc-200 bg-zinc-100 text-zinc-700'"
          >
            {{ policy().enabled ? 'On' : 'Off' }}
          </span>
        </div>
        <a
          [routerLink]="path('account.settings')"
          class="mt-3 inline-flex items-center gap-1 text-sm font-medium text-[#1A5A96] underline-offset-2 hover:underline"
        >
          Manage in Account Settings
          <svg lucide="ArrowRight" class="h-3.5 w-3.5"></svg>
        </a>
      </div>
    </div>
  `,
})
export class AutoApprovalStatusCard {
  protected readonly path = path;
  private readonly specialEventsRepository = inject(SPECIAL_EVENTS_REPOSITORY);
  protected readonly policy = this.specialEventsRepository.autoApprovalPolicy();
  protected readonly minNoticeDays = MIN_NOTICE_DAYS;
}
