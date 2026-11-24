/** One-time onboarding card: once dismissed, `lcrb.notice.<id>` is set in localStorage and it never renders again. Nothing renders on first paint, as in the prototype (the flag is read after it). */
import { afterNextRender, ChangeDetectionStrategy, Component, computed, contentChild, ElementRef, input, signal } from '@angular/core';
import { cn } from '@shared/util/utils';
import { Button } from './button';
import { LucideIcon } from './lucide-icon';

const PREFIX = 'lcrb.notice.';

function isDismissed(id: string): boolean {
  if (typeof window === 'undefined') return true;
  try {
    return window.localStorage.getItem(PREFIX + id) === '1';
  } catch {
    return false;
  }
}

@Component({
  selector: 'ui-dismissible-notice',
  imports: [Button, LucideIcon],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `@if (visible()) {
    <aside [attr.aria-label]="title()" [class]="asideClass()">
      <button
        uiButton
        type="button"
        variant="ghost"
        size="icon"
        (click)="dismiss()"
        [attr.aria-label]="'Dismiss: ' + title()"
        class="absolute right-2 top-2 h-8 w-8 text-muted-foreground hover:text-foreground"
      >
        <svg lucide="X" class="h-4 w-4" aria-hidden="true"></svg>
      </button>
      <div class="flex items-start gap-3 pr-8">
        @if (iconSlot()) {
          <div class="mt-0.5 shrink-0 text-[#013366]"><ng-content select="[icon]" /></div>
        }
        <div class="flex min-w-0 flex-col gap-2">
          <p class="font-semibold text-[#013366]">{{ title() }}</p>
          <div class="text-foreground/90"><ng-content /></div>
          @if (actionsSlot()) {
            <div class="mt-1 flex flex-wrap items-center gap-2"><ng-content select="[actions]" /></div>
          }
        </div>
      </div>
    </aside>
  }`,
})
export class DismissibleNotice {
  /** Stable key; changing it re-shows the card to everyone. */
  readonly noticeId = input.required<string>();
  readonly title = input.required<string>();
  readonly userClass = input<string | undefined>(undefined, { alias: 'class' });
  protected readonly iconSlot = contentChild('icon', { read: ElementRef });
  protected readonly actionsSlot = contentChild('actions', { read: ElementRef });
  protected readonly visible = signal(false);
  protected readonly asideClass = computed(() =>
    cn('relative rounded-xl border border-[#013366]/20 bg-[#F1F8FE] p-5 text-sm leading-relaxed', this.userClass()),
  );

  constructor() {
    afterNextRender(() => this.visible.set(!isDismissed(this.noticeId())));
  }

  protected dismiss(): void {
    this.visible.set(false);
    try {
      window.localStorage.setItem(PREFIX + this.noticeId(), '1');
    } catch {
      /* fine, it will show again next visit */
    }
  }
}
