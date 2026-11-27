import { Injectable } from '@angular/core';
import { computed, signal } from '@angular/core';
import { ANNOUNCEMENTS, latestAnnouncementId, injectUnreadAnnouncements } from './mock/announcements';
import { injectInboxFeed } from './inbox';
import { injectNoticesFeed, markNoticeRead } from './notices-store';
import { listDeliveryLog } from './notification-service';
import { injectNotifications } from './notifications.signals';
import type { NotificationsRepository } from './notifications.repository';

/** Mock NotificationsRepository over the seed data and mock services. */
@Injectable()
export class MockNotificationsRepository implements NotificationsRepository {
  readonly announcements = signal(ANNOUNCEMENTS).asReadonly();

  readonly latestAnnouncementId = latestAnnouncementId;

  readonly unreadAnnouncements = injectUnreadAnnouncements;

  private readonly noticesFeed = injectNoticesFeed();

  readonly notices = computed(() => this.noticesFeed().notices);

  readonly inboxFeed = injectInboxFeed;

  readonly markNoticeRead = markNoticeRead;

  readonly notifications = injectNotifications;

  readonly deliveryLog = listDeliveryLog;
}
