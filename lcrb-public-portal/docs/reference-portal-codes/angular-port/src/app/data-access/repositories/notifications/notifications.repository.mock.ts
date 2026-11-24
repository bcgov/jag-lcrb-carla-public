import { Injectable } from '@angular/core';
import { signal } from '@angular/core';
import { ANNOUNCEMENTS, latestAnnouncementId, injectUnreadAnnouncements } from './mock/announcements';
import { NOTICES } from './mock/notices';
import { injectInboxFeed } from './inbox';
import { markNoticeRead } from './notices-store';
import { listDeliveryLog } from './notification-service';
import { injectNotifications } from './notifications.signals';
import type { Announcement, Notice } from '@domain/models/notifications';
import type { NotificationsRepository } from './notifications.repository';

/** Mock NotificationsRepository over the prototype's seed data and mock services. */
@Injectable()
export class MockNotificationsRepository implements NotificationsRepository {
  readonly announcements = signal(ANNOUNCEMENTS).asReadonly();

  readonly latestAnnouncementId = latestAnnouncementId;

  readonly unreadAnnouncements = injectUnreadAnnouncements;

  readonly notices = signal(NOTICES).asReadonly();

  readonly inboxFeed = injectInboxFeed;

  readonly markNoticeRead = markNoticeRead;

  readonly notifications = injectNotifications;

  readonly deliveryLog = listDeliveryLog;
}
