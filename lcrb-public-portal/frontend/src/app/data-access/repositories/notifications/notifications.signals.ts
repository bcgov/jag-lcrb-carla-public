import { computed, type Signal } from "@angular/core"
import { read, type Arg } from "@shared/util/external-store"
import { notificationInbox, listNotifications, unreadCount, type PortalNotification } from "./notification-service"
import { injectCollectionVersion } from "../mock-storage/collection-version"

export function injectNotifications(recipient: Arg<string>): Signal<{
  notifications: PortalNotification[]
  unread: number
}> {
  const version = injectCollectionVersion(notificationInbox)
  return computed(() => {
    version()
    const r = read(recipient)
    return { notifications: listNotifications(r), unread: unreadCount(r) }
  })
}
