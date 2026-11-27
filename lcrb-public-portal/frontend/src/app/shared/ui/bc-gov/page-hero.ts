// Blue page header band with optional breadcrumb, title, description, and heroBadge and heroActions slots.
import { ChangeDetectionStrategy, Component, computed, contentChild, Directive, input } from '@angular/core';
import { cn } from '@shared/util/utils';
import { HeroBreadcrumb, type HeroCrumb } from './hero-breadcrumb';

@Directive({ selector: '[heroBadge]' })
export class HeroBadge {}
@Directive({ selector: '[heroMeta]' })
export class HeroMeta {}
@Directive({ selector: '[heroActions]' })
export class HeroActions {}
@Directive({ selector: '[heroDescription]' })
export class HeroDescription {}
@Directive({ selector: '[heroTitle]' })
export class HeroTitle {}

@Component({
  selector: 'page-hero',
  imports: [HeroBreadcrumb],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `<div [class]="outerClass()">
    <div [class]="innerClass()">
      @if (breadcrumbs() && breadcrumbs()!.length > 0) {
        <hero-breadcrumb [items]="breadcrumbs()!" />
      }
      <div class="flex flex-col gap-4 sm:flex-row sm:items-end sm:justify-between">
        <div class="min-w-0">
          <div class="flex flex-wrap items-center gap-3">
            <h1 class="text-2xl font-bold text-balance md:text-3xl">{{ title() }}<ng-content select="[heroTitle]" /></h1>
            <ng-content select="[heroBadge]" />
          </div>
          @if (description() || descriptionSlot()) {
            <p class="mt-2 max-w-2xl text-white/80">{{ description() }}<ng-content select="[heroDescription]" /></p>
          }
          @if (metaSlot()) {
            <div class="mt-2 text-sm text-white/80"><ng-content select="[heroMeta]" /></div>
          }
        </div>
        @if (actionsSlot()) {
          <div class="flex shrink-0 flex-wrap items-center gap-3"><ng-content select="[heroActions]" /></div>
        }
      </div>
    </div>
  </div>`,
})
export class PageHero {
  /** Page title rendered as the h1 inside the blue band (or use a [heroTitle] slot). */
  readonly title = input('');
  /** Optional supporting sentence shown beneath the title. */
  readonly description = input<string | undefined>(undefined);
  /** Breadcrumb trail. Omit on top-level landing pages. */
  readonly breadcrumbs = input<HeroCrumb[] | undefined>(undefined);
  /** Width + horizontal padding of the inner content wrapper. */
  readonly containerClassName = input<string | undefined>(undefined);
  readonly userClass = input<string | undefined>(undefined, { alias: 'class' });

  protected readonly descriptionSlot = contentChild(HeroDescription);
  protected readonly metaSlot = contentChild(HeroMeta);
  protected readonly actionsSlot = contentChild(HeroActions);

  protected readonly outerClass = computed(() => cn('bg-bc-blue text-white', this.userClass()));
  protected readonly innerClass = computed(() =>
    cn('py-8 md:py-10', this.containerClassName() ?? 'mx-auto max-w-7xl px-4 sm:px-6 lg:px-8'),
  );
}

export const PAGE_HERO = [PageHero, HeroBadge, HeroMeta, HeroActions, HeroDescription, HeroTitle] as const;
