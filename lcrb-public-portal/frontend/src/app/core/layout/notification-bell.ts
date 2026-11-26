// Header bell: unified inbox feed, mark-all-read, chime toggle and a link to all notices.
import { NgTemplateOutlet } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { cn } from '@shared/util/utils';
import { injectSession } from '@core/session/session-provider';
import { Button } from '@shared/ui/primitives/button';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { POPOVER } from '@shared/ui/primitives/popover';
import { ScrollArea } from '@shared/ui/primitives/scroll-area';
import { NOTIFICATIONS_REPOSITORY } from '@data-access/repositories/notifications/notifications.repository';
import { injectNoticeChime, isNoticeChimeMuted, setNoticeChimeMuted } from '@core/notifications/notice-chime';
import { path } from '@/app/route-registry';

function timeAgo(iso: string): string {
  const diff = Date.now() - new Date(iso).getTime();
  const mins = Math.floor(diff / 60000);
  if (mins < 1) return 'just now';
  if (mins < 60) return `${mins}m ago`;
  const hrs = Math.floor(mins / 60);
  if (hrs < 24) return `${hrs}h ago`;
  const days = Math.floor(hrs / 24);
  return `${days}d ago`;
}

@Component({
  selector: 'notification-bell',
  imports: [RouterLink, NgTemplateOutlet, Button, LucideIcon, ...POPOVER, ScrollArea],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <ui-popover align="end">
      <button
        uiButton
        uiPopoverTrigger
        variant="ghost"
        size="icon"
        class="relative text-[#013366] hover:bg-[#f2f2f2]"
        [attr.aria-label]="'Notifications' + (feed().unread > 0 ? ', ' + feed().unread + ' unread' : '')"
      >
        <svg lucide="Bell" class="h-5 w-5"></svg>
        @if (feed().unread > 0) {
          <span class="absolute -right-0.5 -top-0.5 flex h-5 min-w-5 items-center justify-center rounded-full bg-[#D8292F] px-1 text-[11px] font-semibold text-white">
            {{ feed().unread > 9 ? '9+' : feed().unread }}
          </span>
        }
      </button>
      <div uiPopoverContent *uiPopoverPortal class="w-96 p-0">
        <div class="flex items-center justify-between border-b px-4 py-3">
          <h2 class="text-sm font-semibold text-foreground">Notifications</h2>
          <div class="flex items-center gap-3">
            @if (feed().unread > 0) {
              <button type="button" (click)="feed().markAll()" class="flex items-center gap-1 text-xs font-medium text-[#013366] hover:underline">
                <svg lucide="CheckCheck" class="h-3.5 w-3.5"></svg> Mark all read
              </button>
            }
            @if (feed().showNotices) {
              <button
                type="button"
                (click)="toggleMuted()"
                [attr.aria-pressed]="muted()"
                [title]="muted() ? 'Notice chime is off' : 'Notice chime is on'"
                class="text-muted-foreground transition-colors hover:text-[#013366]"
              >
                @if (muted()) { <svg lucide="VolumeX" class="h-4 w-4"></svg> } @else { <svg lucide="Volume2" class="h-4 w-4"></svg> }
                <span class="sr-only">{{ muted() ? 'Turn notice chime on' : 'Turn notice chime off' }}</span>
              </button>
            }
          </div>
        </div>
        <div uiScrollArea class="max-h-96 overflow-hidden">
          @if (feed().items.length === 0) {
            <p class="px-4 py-10 text-center text-sm text-muted-foreground">You have no notifications yet.</p>
          } @else {
            <ul class="divide-y">
              @for (item of feed().items; track item.key) {
                <ng-template #body>
                  <div class="flex gap-3 px-4 py-3">
                    <span [class]="dotClass(item.read)" aria-hidden="true"></span>
                    <div class="min-w-0 flex-1">
                      <div class="flex items-center justify-between gap-2">
                        <span class="text-[11px] font-medium uppercase tracking-wide text-[#38598A]">{{ item.categoryLabel }}</span>
                        <span class="shrink-0 text-xs text-muted-foreground">{{ timeAgo(item.createdAt) }}</span>
                      </div>
                      <p [class]="titleClass(item.read)">{{ item.title }}</p>
                      <p class="mt-0.5 text-sm leading-relaxed text-muted-foreground">{{ item.body }}</p>
                    </div>
                  </div>
                </ng-template>
                <li class="hover:bg-muted/50">
                  @if (item.href) {
                    <a [routerLink]="item.href" (click)="item.markRead()" class="block"><ng-container [ngTemplateOutlet]="body" /></a>
                  } @else {
                    <button type="button" (click)="item.markRead()" class="block w-full text-left"><ng-container [ngTemplateOutlet]="body" /></button>
                  }
                </li>
              }
            </ul>
          }
        </div>
        <div class="border-t bg-popover px-4 py-2.5">
          <a [routerLink]="path('dashboard.notices')" class="block text-center text-xs font-medium text-[#013366] hover:underline">View all notices</a>
        </div>
      </div>
    </ui-popover>
  `,
})
export class NotificationBell {
  protected readonly path = path;
  private readonly session = injectSession().session;
  protected readonly feed = inject(NOTIFICATIONS_REPOSITORY).inboxFeed(() => this.session()?.role ?? 'licensee');
  protected readonly muted = signal<boolean>(isNoticeChimeMuted());
  protected readonly timeAgo = timeAgo;

  constructor() {
    // Chime once on login when unread notices exist, and on each new notice, for roles that receive notices.
    injectNoticeChime(
      () => this.feed().noticesUnread,
      () => this.feed().hydrated,
      () => this.feed().showNotices,
    );
  }

  protected toggleMuted(): void {
    const next = !this.muted();
    this.muted.set(next);
    setNoticeChimeMuted(next);
  }

  protected dotClass(read: boolean): string {
    return cn('mt-1.5 h-2 w-2 shrink-0 rounded-full', read ? 'bg-transparent' : 'bg-[#D8292F]');
  }

  protected titleClass(read: boolean): string {
    return cn('text-sm text-foreground', !read && 'font-semibold');
  }
}
