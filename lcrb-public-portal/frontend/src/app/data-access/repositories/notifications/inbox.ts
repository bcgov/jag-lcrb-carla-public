// Unified inbox feed shared by the header bell and the notifications page: service notifications plus licensee notices.

import { computed, type Signal } from "@angular/core"
import { read, type Arg } from "@shared/util/external-store"
import { injectNotifications } from "./notifications.signals"
import {
  markAllRead as markAllServiceRead,
  markNotificationRead,
  type NotificationCategory,
} from "./notification-service"
import {
  injectNoticesFeed,
  markNoticeRead,
  markAllNoticesRead,
} from "./notices-store"
import type { Notice, NoticeType } from "@domain/models/notifications"

export const CATEGORY_LABEL: Record<NotificationCategory, string> = {
  application: "Application",
  payment: "Payment",
  decision: "Decision",
  reminder: "Reminder",
  "request-for-info": "Action required",
  system: "System",
}

export const NOTICE_CATEGORY_LABEL: Record<NoticeType, string> = {
  action_required: "Action required",
  info: "Information",
  success: "Approved",
}

/** A unified inbox item rendered by both the bell and the notifications page. */
export interface InboxItem {
  key: string
  source: "notification" | "notice"
  categoryLabel: string
  title: string
  body: string
  createdAt: string
  read: boolean
  href?: string
  markRead: () => void
  /** The full Notice record when `source === "notice"` (drives the document viewer). */
  notice?: Notice
}

export interface InboxFeed {
  items: InboxItem[]
  unread: number
  /** Unread notices only — used by the bell's audible chime. */
  noticesUnread: number
  /** Whether the notices read-state has hydrated from storage (bell chime gate). */
  hydrated: boolean
  /** True when notices are part of this recipient's feed (licensee). */
  showNotices: boolean
  markAll: () => void
}

// Reactive inbox for a recipient; notices appear only in the licensee's feed.
export function injectInboxFeed(recipient: Arg<string>): Signal<InboxFeed> {
  const notificationsFeed = injectNotifications(recipient)
  const noticesFeed = injectNoticesFeed()
  return computed(() => inboxFeed(read(recipient), notificationsFeed(), noticesFeed()))
}

/** Merges the injected notification and notice feeds into one inbox. */
function inboxFeed(
  recipient: string,
  { notifications, unread: serviceUnread }: ReturnType<ReturnType<typeof injectNotifications>>,
  { notices, unread: noticesUnread, hydrated }: ReturnType<ReturnType<typeof injectNoticesFeed>>,
): InboxFeed {

  const showNotices = recipient === "licensee"

  const items: InboxItem[] = [
    ...notifications.map(
      (n): InboxItem => ({
        key: `ntf:${n.id}`,
        source: "notification",
        categoryLabel: CATEGORY_LABEL[n.category],
        title: n.title,
        body: n.body,
        createdAt: n.createdAt,
        read: n.read,
        href: n.href,
        markRead: () => markNotificationRead(n.id),
      }),
    ),
    ...(showNotices
      ? notices.map(
          (n): InboxItem => ({
            key: `notice:${n.id}`,
            source: "notice",
            categoryLabel: NOTICE_CATEGORY_LABEL[n.type],
            title: n.title,
            body: n.message,
            createdAt: n.date,
            read: n.read,
            href: "/notices",
            markRead: () => markNoticeRead(n.id),
            notice: n,
          }),
        )
      : []),
  ].sort((a, b) => (a.createdAt < b.createdAt ? 1 : -1))

  const unread = serviceUnread + (showNotices ? noticesUnread : 0)

  const markAll = () => {
    markAllServiceRead(recipient)
    if (showNotices) markAllNoticesRead()
  }

  return { items, unread, noticesUnread, hydrated, showNotices, markAll }
}
