import { InjectionToken } from '@angular/core';
import type { Signal } from '@angular/core';
import type { Announcement, Notice } from '@domain/models/notifications';
import type { latestAnnouncementId, injectUnreadAnnouncements } from './mock/announcements';
import type { injectInboxFeed } from './inbox';
import type { markNoticeRead } from './notices-store';
import type { listDeliveryLog } from './notification-service';
import type { injectNotifications } from './notifications.signals';
import { MockNotificationsRepository } from './notifications.repository.mock';

// Notifications repository token; defaults to the mock, provide a real implementation in app.config.ts.
export interface NotificationsRepository {
  readonly announcements: Signal<Announcement[]>;
  readonly latestAnnouncementId: typeof latestAnnouncementId;
  readonly unreadAnnouncements: typeof injectUnreadAnnouncements;
  readonly notices: Signal<Notice[]>;
  readonly inboxFeed: typeof injectInboxFeed;
  readonly markNoticeRead: typeof markNoticeRead;
  readonly notifications: typeof injectNotifications;
  readonly deliveryLog: typeof listDeliveryLog;
}

export const NOTIFICATIONS_REPOSITORY = new InjectionToken<NotificationsRepository>('NotificationsRepository', {
  providedIn: 'root',
  factory: () => new MockNotificationsRepository(),
});
