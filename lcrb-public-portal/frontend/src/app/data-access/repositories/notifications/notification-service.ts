// Mock notification service for in-portal, email and SMS channels; replace the dispatchers and keep `notify()`.

import { createCollection, makeId } from "../mock-storage/kv-store"
import { type Entity } from "@domain/models/entity"
import { recordAudit } from "../activity/audit-service"

export type NotificationChannel = "in-portal" | "email" | "sms"

export type NotificationCategory =
  | "application"
  | "payment"
  | "decision"
  | "reminder"
  | "request-for-info"
  | "system"

export interface PortalNotification extends Entity {
  /** Recipient key — for the mock we scope by role; in prod, user id. */
  recipient: string
  category: NotificationCategory
  title: string
  body: string
  /** Optional deep link into the portal. */
  href?: string
  /** Channels this message was sent on. */
  channels: NotificationChannel[]
  createdAt: string
  read: boolean
  /** Optional related subject (job number / permit id). */
  subjectId?: string
}

/** A file attached to a simulated outbound email (metadata only; the bytes are regenerated on demand). */
export interface EmailAttachment {
  filename: string
  contentType: string
  /** Size in bytes, for the "receipt.pdf (42 KB)" chip. */
  sizeBytes: number
}

/** A simulated outbound email/SMS, surfaced in the dev "delivery log". */
export interface DeliveryLogEntry extends Entity {
  channel: Exclude<NotificationChannel, "in-portal">
  to: string
  subject: string
  preview: string
  /** Full plain-text body when the caller supplied one (e.g. a receipt). */
  body?: string
  attachments?: EmailAttachment[]
  /** Related subject (job number / receipt number) so pages can show "emailed on …". */
  subjectId?: string
  at: string
  status: "sent" | "failed"
}

const inbox = createCollection<PortalNotification>("notifications")
const deliveryLog = createCollection<DeliveryLogEntry>("notification-delivery")

export interface NotifyInput {
  recipient: string
  category: NotificationCategory
  title: string
  body: string
  href?: string
  subjectId?: string
  /** Defaults to ["in-portal"]. Add "email"/"sms" to simulate external sends. */
  channels?: NotificationChannel[]
  /** Contact endpoints for simulated external delivery. */
  email?: string
  phone?: string
  /** Full email text; defaults to `body`. */
  emailBody?: string
  attachments?: EmailAttachment[]
}

function dispatchEmail(
  to: string,
  subject: string,
  preview: string,
  extra: { body?: string; attachments?: EmailAttachment[]; subjectId?: string } = {},
): DeliveryLogEntry {
  const entry: DeliveryLogEntry = {
    id: makeId("eml"),
    channel: "email",
    to,
    subject,
    preview,
    body: extra.body,
    attachments: extra.attachments,
    subjectId: extra.subjectId,
    at: new Date().toISOString(),
    status: "sent",
  }
  deliveryLog.upsert(entry)
  return entry
}

function dispatchSms(to: string, preview: string) {
  deliveryLog.upsert({
    id: makeId("sms"),
    channel: "sms",
    to,
    subject: "SMS",
    preview,
    at: new Date().toISOString(),
    status: "sent",
  })
}

/** Send a notification across the requested channels. Returns the inbox record. */
export function notify(input: NotifyInput): PortalNotification {
  const channels = input.channels ?? ["in-portal"]
  const record: PortalNotification = {
    id: makeId("ntf"),
    recipient: input.recipient,
    category: input.category,
    title: input.title,
    body: input.body,
    href: input.href,
    channels,
    createdAt: new Date().toISOString(),
    read: false,
    subjectId: input.subjectId,
  }
  if (channels.includes("in-portal")) {
    inbox.upsert(record)
  }
  if (channels.includes("email") && input.email) {
    dispatchEmail(input.email, input.title, input.body, {
      body: input.emailBody,
      attachments: input.attachments,
      subjectId: input.subjectId,
    })
  }
  if (channels.includes("sms") && input.phone) {
    dispatchSms(input.phone, input.body)
  }
  recordAudit({
    category: "notification",
    action: "notification.sent",
    summary: `Notification sent to ${input.recipient}: ${input.title}`,
    actor: "System",
    subjectId: input.subjectId,
    meta: { channels: channels.join(",") },
  })
  return record
}

/** Inbox for a recipient, newest first. */
export function listNotifications(recipient: string): PortalNotification[] {
  return inbox
    .list()
    .filter((n) => n.recipient === recipient)
    .sort((a, b) => (a.createdAt < b.createdAt ? 1 : -1))
}

export function unreadCount(recipient: string): number {
  return listNotifications(recipient).filter((n) => !n.read).length
}

export function markNotificationRead(id: string): void {
  inbox.update(id, { read: true })
}

export function markAllRead(recipient: string): void {
  for (const n of listNotifications(recipient)) {
    if (!n.read) inbox.update(n.id, { read: true })
  }
}

/** The simulated external delivery log (dev/debug visibility), newest first. */
export function listDeliveryLog(): DeliveryLogEntry[] {
  return deliveryLog.list().sort((a, b) => (a.at < b.at ? 1 : -1))
}

/** Emails/SMS sent about one subject (e.g. a receipt number), newest first. */
export function listDeliveriesForSubject(subjectId: string): DeliveryLogEntry[] {
  return listDeliveryLog().filter((d) => d.subjectId === subjectId)
}

export const notificationInbox = inbox
export const notificationDeliveryLog = deliveryLog
