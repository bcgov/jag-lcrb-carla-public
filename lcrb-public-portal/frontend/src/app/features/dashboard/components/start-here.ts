/** The dismiss-once "New here? Start with the Planner" card. */
import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Button } from '@shared/ui/primitives/button';
import { DismissibleNotice } from '@shared/ui/primitives/dismissible-notice';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { path } from '@/app/route-registry';

const STEPS = [
  {
    title: 'Plan your establishment',
    body: 'Check feasibility, fees and timelines before you commit to anything.',
    href: path('public.establishment-planner'),
  },
  {
    title: 'Find the right licence type',
    body: "Answer a few questions about how you'll operate and get a recommendation.",
    href: path('public.licence-finder'),
  },
  {
    title: 'Start the application',
    body: 'Apply in Guided Mode, one question at a time, with your answers saved as you go.',
    href: path('applications.new'),
  },
] as const;

@Component({
  selector: 'start-here',
  imports: [Button, DismissibleNotice, LucideIcon, RouterLink],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <ui-dismissible-notice noticeId="start-here-v1" title="New here? Start with the Planner" class="mb-8">
      <svg icon #icon lucide="Compass" class="h-5 w-5" aria-hidden="true"></svg>
      <a actions #actions uiButton size="sm" [routerLink]="path('public.establishment-planner')" class="bg-[#013366] text-white hover:bg-[#013366]/90">
        Open the Establishment Planner
        <svg lucide="ArrowRight" class="ml-1.5 h-4 w-4" aria-hidden="true"></svg>
      </a>
      <p>
        If you're opening a new business, the fastest route through licensing is in this order. You can skip
        ahead at any point if you already know what you need.
      </p>
      <ol class="mt-3 grid gap-3 sm:grid-cols-3">
        @for (step of steps; track step.href; let i = $index) {
          <li class="flex gap-3">
            <span
              aria-hidden="true"
              class="flex h-6 w-6 shrink-0 items-center justify-center rounded-full bg-[#013366] text-xs font-semibold text-white"
              >{{ i + 1 }}</span
            >
            <div class="min-w-0">
              <a [routerLink]="step.href" class="font-medium text-[#013366] underline-offset-2 hover:underline">{{
                step.title
              }}</a>
              <p class="text-muted-foreground">{{ step.body }}</p>
            </div>
          </li>
        }
      </ol>
    </ui-dismissible-notice>
  `,
})
export class StartHere {
  protected readonly path = path;
  protected readonly steps = STEPS;
}
