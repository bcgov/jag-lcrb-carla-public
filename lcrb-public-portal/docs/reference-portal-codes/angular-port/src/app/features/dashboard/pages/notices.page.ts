/** The single inbox (official notices plus status alerts) with filters. */
import { NgTemplateOutlet } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { BCGovFooter } from '@core/layout/footer';
import { BCGovHeader } from '@core/layout/header';
import { PAGE_HERO } from '@shared/ui/bc-gov/page-hero';
import { NoticeDocumentViewer } from '../components/notice-document-viewer';
import { injectSession } from '@core/session/session-provider';
import { Badge } from '@shared/ui/primitives/badge';
import { Button } from '@shared/ui/primitives/button';
import { CARD } from '@shared/ui/primitives/card';
import { type IconName } from '@shared/ui/primitives/icons';
import { LucideIcon } from '@shared/ui/primitives/lucide-icon';
import { formatNoticeDate } from '@domain/notifications/notices';
import { type NoticeType } from '@domain/models/notifications';
import { cn } from '@shared/util/utils';
import { NOTIFICATIONS_REPOSITORY } from '@data-access/repositories/notifications/notifications.repository';
import { path } from '@/app/route-registry';

const noticeTypeConfig: Record<
  NoticeType,
  { icon: IconName; color: string; bgColor: string; badge: string; badgeLabel: string }
> = {
  action_required: {
    icon: 'AlertCircle',
    color: 'text-yellow-600',
    bgColor: 'bg-yellow-50 border-yellow-200',
    badge: 'bg-yellow-100 text-yellow-800',
    badgeLabel: 'Action Required',
  },
  info: {
    icon: 'Bell',
    color: 'text-blue-600',
    bgColor: 'bg-blue-50 border-blue-200',
    badge: 'bg-blue-100 text-blue-800',
    badgeLabel: 'Information',
  },
  success: {
    icon: 'CheckCircle2',
    color: 'text-green-600',
    bgColor: 'bg-green-50 border-green-200',
    badge: 'bg-green-100 text-green-800',
    badgeLabel: 'Approved',
  },
};

type Filter = 'all' | 'unread' | 'notice' | 'notification';

function formatWhen(iso: string): string {
  return new Date(iso).toLocaleString('en-CA', {
    month: 'short',
    day: 'numeric',
    hour: 'numeric',
    minute: '2-digit',
  });
}

@Component({
  selector: 'notices-page',
  imports: [
    NgTemplateOutlet,
    RouterLink,
    BCGovHeader,
    BCGovFooter,
    ...PAGE_HERO,
    ...CARD,
    Badge,
    Button,
    LucideIcon,
    NoticeDocumentViewer,
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: { style: 'display: contents' },
  template: `
    <div class="min-h-screen flex flex-col bg-background">
      <bc-gov-header currentPage="/notices" />

      <main id="main-content" class="flex-1">
        <page-hero
          [breadcrumbs]="breadcrumbs"
          title="Notices"
          description="Everything the LCRB sends you in one place: official correspondence about your licences and applications — approvals, renewal letters and requests for information, each a formal document you can open and download — alongside status alerts about your applications, payments and deadlines."
        >
          @if (feed().unread > 0) {
            <span uiBadge heroBadge class="bg-bc-gold text-bc-blue hover:bg-bc-gold">{{ feed().unread }} unread</span>
          }
          @if (feed().unread > 0) {
            <button
              uiButton
              heroActions
              variant="outline"
              class="gap-2 border-white/40 bg-transparent text-white hover:bg-white/10 hover:text-white"
              (click)="feed().markAll()"
            >
              <svg lucide="CheckCheck" class="h-4 w-4" aria-hidden="true"></svg>
              Mark all as read
            </button>
          }
        </page-hero>

        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
          <!-- Filters -->
          <div class="mb-6 flex flex-wrap items-center gap-2" role="group" aria-label="Filter notices">
            @for (f of filters(); track f.value) {
              <button
                type="button"
                (click)="filter.set(f.value)"
                [attr.aria-pressed]="filter() === f.value"
                [attr.class]="
                  cn(
                    'rounded-full border px-3.5 py-1.5 text-sm font-medium transition-colors',
                    filter() === f.value
                      ? 'border-[#013366] bg-[#013366] text-white'
                      : 'bg-white text-foreground hover:bg-muted'
                  )
                "
              >
                {{ f.label }} ({{ f.count }})
              </button>
            }
            @if (feed().unread > 0) {
              <button uiButton variant="ghost" size="sm" (click)="feed().markAll()" class="ml-auto gap-1.5 text-[#013366]">
                <svg lucide="CheckCheck" class="h-4 w-4" aria-hidden="true"></svg>
                Mark all as read
              </button>
            }
          </div>

          @if (visible().length === 0) {
            <div uiCard>
              <div uiCardContent class="p-12 text-center">
                <div class="w-16 h-16 rounded-full bg-muted flex items-center justify-center mx-auto mb-4">
                  <svg lucide="Inbox" class="h-8 w-8 text-muted-foreground" aria-hidden="true"></svg>
                </div>
                <h3 class="text-lg font-semibold mb-2">{{ filter() === 'unread' ? "You're all caught up" : 'Nothing here yet' }}</h3>
                <p class="text-muted-foreground">{{ emptyText() }}</p>
              </div>
            </div>
          } @else {
            <div class="space-y-4">
              @for (item of visible(); track item.key) {
                @if (item.source === 'notice' && item.notice) {
                  <!-- Official LCRB correspondence: full card with the document viewer. -->
                  @let notice = item.notice;
                  @let config = noticeConfig[notice.type];
                  <div uiCard [class]="cn(config.bgColor, !item.read && 'ring-2 ring-[#013366]/20')">
                    <div uiCardContent class="p-4">
                      <div class="flex gap-4">
                        <div [attr.class]="cn('shrink-0', config.color)">
                          <svg [lucide]="config.icon" class="h-6 w-6" aria-hidden="true"></svg>
                        </div>
                        <div class="flex-1 min-w-0">
                          <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-2 mb-2">
                            <div class="flex items-center gap-2">
                              <span class="text-[11px] font-medium uppercase tracking-wide text-[#38598A]">
                                Official notice
                              </span>
                              @if (!item.read) {
                                <span class="w-2 h-2 rounded-full bg-[#013366]" aria-label="Unread"></span>
                              }
                            </div>
                            <div class="flex items-center gap-2">
                              <span uiBadge variant="outline" [class]="config.badge">{{ config.badgeLabel }}</span>
                              <span class="text-sm text-muted-foreground flex items-center gap-1">
                                <svg lucide="Calendar" class="h-3 w-3" aria-hidden="true"></svg>
                                {{ formatNoticeDate(notice.date) }}
                              </span>
                            </div>
                          </div>
                          <h3 [attr.class]="cn('font-semibold', !item.read && 'text-[#013366]')">{{ notice.title }}</h3>
                          <p class="text-sm text-muted-foreground mt-1">{{ notice.message }}</p>
                          <notice-document-viewer [notice]="notice" (opened)="markNoticeRead($event)" />
                        </div>
                      </div>
                    </div>
                  </div>
                } @else {
                  <!-- Lightweight status alert (application / payment / decision / reminder). -->
                  <div [attr.class]="cn('rounded-lg border bg-white hover:bg-muted/40 transition-colors', !item.read && 'border-[#013366]/30')">
                    @if (item.href) {
                      <a [routerLink]="tree(item.href)" (click)="item.markRead()" class="block">
                        <ng-container *ngTemplateOutlet="alertInner; context: { $implicit: item }" />
                      </a>
                    } @else {
                      <button type="button" (click)="item.markRead()" class="block w-full text-left">
                        <ng-container *ngTemplateOutlet="alertInner; context: { $implicit: item }" />
                      </button>
                    }
                  </div>
                }
              }
            </div>
          }

          <!-- Information -->
          <div uiCard class="mt-6">
            <div uiCardContent class="p-4">
              <div class="flex gap-3">
                <svg lucide="FileText" class="h-5 w-5 text-muted-foreground shrink-0" aria-hidden="true"></svg>
                <p class="text-sm text-muted-foreground">
                  Notices attached to your account are shown here and are retained as long as you have an active account with LCRB. If you have any questions, please contact LCRB at 1-866-209-2111.
                </p>
              </div>
            </div>
          </div>
        </div>
      </main>

      <bc-gov-footer />
    </div>

    <ng-template #alertInner let-item>
      <div class="flex gap-3 px-4 py-3">
        <span
          [attr.class]="cn('mt-2 h-2 w-2 shrink-0 rounded-full', item.read ? 'bg-transparent' : 'bg-[#D8292F]')"
          aria-hidden="true"
        ></span>
        <div class="min-w-0 flex-1">
          <div class="flex items-center justify-between gap-2">
            <span class="text-[11px] font-medium uppercase tracking-wide text-[#38598A]">
              Alert · {{ item.categoryLabel }}
            </span>
            <span class="shrink-0 text-xs text-muted-foreground">{{ formatWhen(item.createdAt) }}</span>
          </div>
          <p [attr.class]="cn('text-sm text-foreground', !item.read && 'font-semibold')">{{ item.title }}</p>
          <p class="mt-0.5 text-sm leading-relaxed text-muted-foreground">{{ item.body }}</p>
        </div>
        @if (item.href) {
          <svg lucide="ArrowRight" class="mt-2 h-4 w-4 shrink-0 text-muted-foreground" aria-hidden="true"></svg>
        }
      </div>
    </ng-template>
  `,
})
export class NoticesPage {
  private readonly notificationsRepository = inject(NOTIFICATIONS_REPOSITORY);
  private readonly router = inject(Router);
  private readonly session = injectSession().session;
  private readonly recipient = computed(() => this.session()?.role ?? 'licensee');

  protected readonly feed = this.notificationsRepository.inboxFeed(this.recipient);
  protected readonly filter = signal<Filter>('all');

  protected readonly breadcrumbs = [{ label: 'Dashboard', href: path('dashboard.home') }, { label: 'Notices' }];
  protected readonly noticeConfig = noticeTypeConfig;
  protected readonly cn = cn;
  protected readonly formatNoticeDate = formatNoticeDate;
  protected readonly formatWhen = formatWhen;
  protected readonly markNoticeRead = this.notificationsRepository.markNoticeRead;

  private readonly counts = computed(() => {
    const { items, unread } = this.feed();
    return {
      all: items.length,
      unread,
      notice: items.filter((i) => i.source === 'notice').length,
      notification: items.filter((i) => i.source === 'notification').length,
    };
  });

  protected readonly visible = computed(() => {
    const { items } = this.feed();
    switch (this.filter()) {
      case 'unread':
        return items.filter((i) => !i.read);
      case 'notice':
        return items.filter((i) => i.source === 'notice');
      case 'notification':
        return items.filter((i) => i.source === 'notification');
      default:
        return items;
    }
  });

  protected readonly filters = computed<{ value: Filter; label: string; count: number }[]>(() => {
    const counts = this.counts();
    return [
      { value: 'all', label: 'All', count: counts.all },
      { value: 'unread', label: 'Unread', count: counts.unread },
      { value: 'notice', label: 'Official notices', count: counts.notice },
      { value: 'notification', label: 'Alerts', count: counts.notification },
    ];
  });

  protected readonly emptyText = computed(() => {
    const filter = this.filter();
    return filter === 'unread'
      ? 'Every notice and alert has been read.'
      : filter === 'notice'
        ? 'There are currently no official notices attached to this account.'
        : filter === 'notification'
          ? 'No status alerts right now.'
          : 'There are currently no notices or alerts attached to this account.';
  });

  /** Parsed so a `?query` in the href renders as-is (a string routerLink would encode the `?`). */
  protected tree(href: string) {
    return this.router.parseUrl(href);
  }
}
