/** Breadcrumb trail for the dark-blue page heroes; the last item is the current page (no link). */
import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { cn } from '@shared/util/utils';
import { LucideIcon } from '../primitives/lucide-icon';

export interface HeroCrumb {
  label: string;
  /** Omit href on the final (current-page) crumb. */
  href?: string;
}

@Component({
  selector: 'hero-breadcrumb',
  imports: [RouterLink, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `<nav aria-label="Breadcrumb" [class]="navClass()">
    <ol class="flex flex-wrap items-center gap-1.5 text-sm">
      @for (item of items(); track item.label + '-' + $index; let isLast = $last) {
        <li class="inline-flex items-center gap-1.5">
          @if (item.href && !isLast) {
            <a [routerLink]="item.href" class="text-white/70 transition-colors hover:text-white hover:underline">{{ item.label }}</a>
          } @else {
            <span [class]="isLast ? 'font-medium text-white' : 'text-white/70'" [attr.aria-current]="isLast ? 'page' : null">{{ item.label }}</span>
          }
          @if (!isLast) {
            <svg lucide="ChevronRight" class="h-3.5 w-3.5 text-white/50" aria-hidden="true"></svg>
          }
        </li>
      }
    </ol>
  </nav>`,
})
export class HeroBreadcrumb {
  readonly items = input.required<HeroCrumb[]>();
  readonly userClass = input<string | undefined>(undefined, { alias: 'class' });
  protected readonly navClass = computed(() => cn('mb-2', this.userClass()));
}
