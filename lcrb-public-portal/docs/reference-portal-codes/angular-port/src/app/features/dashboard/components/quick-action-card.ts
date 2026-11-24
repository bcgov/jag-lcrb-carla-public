/** `icon` is a lucide-react name from ui/icons. */
import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { cn } from '@shared/util/utils';
import type { IconName } from '@shared/ui/primitives/icons';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { path } from '@/app/route-registry';

@Component({
  selector: 'quick-action-card',
  imports: [RouterLink, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `<a [routerLink]="path()" [queryParams]="query()" [class]="linkClass()">
    <div>
      <h3 class="text-lg font-medium leading-tight mb-2">{{ title() }}</h3>
      @if (description()) {
        <p [class]="descriptionClass()">{{ description() }}</p>
      }
    </div>
    <div class="flex justify-end mt-4">
      <div [class]="iconWrapClass()">
        <svg [lucide]="icon()" [class]="iconClass()"></svg>
      </div>
    </div>
  </a>`,
})
export class QuickActionCard {
  readonly title = input.required<string>();
  readonly description = input<string | undefined>(undefined);
  readonly href = input.required<string>();
  readonly icon = input<IconName>('Globe');
  readonly variant = input<'default' | 'primary'>('default');

  protected readonly path = computed(() => this.href().split('?')[0]);
  protected readonly query = computed(() => {
    const q = this.href().split('?')[1];
    return q ? Object.fromEntries(new URLSearchParams(q)) : null;
  });
  protected readonly linkClass = computed(() =>
    cn(
      'flex flex-col justify-between p-6 rounded-lg transition-all duration-200 group min-h-[160px]',
      this.variant() === 'default' ? 'bg-[#E8E8E8] hover:bg-[#D9D9D9] text-foreground' : 'bg-[#013366] hover:bg-[#38598A] text-white',
    ),
  );
  protected readonly descriptionClass = computed(() => cn('text-sm', this.variant() === 'default' ? 'text-muted-foreground' : 'text-white/80'));
  protected readonly iconWrapClass = computed(() =>
    cn(
      'w-10 h-10 rounded-full flex items-center justify-center transition-colors',
      this.variant() === 'default' ? 'bg-white/80 group-hover:bg-white' : 'bg-white/20 group-hover:bg-white/30',
    ),
  );
  protected readonly iconClass = computed(() => cn('h-5 w-5', this.variant() === 'default' ? 'text-[#013366]' : 'text-white'));
}

// Pre-configured quick actions for common use cases
export const quickActions = {
  findLicenceType: {
    title: 'Find out what licence type I need',
    href: path('public.licence-finder'),
    icon: 'Search' as IconName,
  },
  applyFoodPrimary: {
    title: 'Apply for a food primary liquor licence',
    href: path('applications.food-primary'),
    icon: 'Plus' as IconName,
  },
  checkStatus: {
    title: 'Check the status of an application',
    href: path('applications.list'),
    icon: 'Clock' as IconName,
  },
  renewLicence: {
    title: 'Renew an existing licence',
    href: path('licences.list') + '?action=renew',
    icon: 'RefreshCw' as IconName,
  },
};
