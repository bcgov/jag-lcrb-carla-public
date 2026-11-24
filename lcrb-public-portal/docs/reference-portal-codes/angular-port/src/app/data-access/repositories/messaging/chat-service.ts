"use client"

/** Case conversation threads (prototype). */

import { createCollection, makeId } from "../mock-storage/kv-store"
import { type Entity } from "@domain/models/entity"
import { notify } from "../notifications/notification-service"
import type { UserRole } from "@domain/roles"
import { type ThreadKind } from "@domain/models/messaging"
import { ROLE_LABELS, THREAD_PARTICIPANTS } from "@domain/messaging/chat"

export interface ChatMessage extends Entity {
  threadId: string
  kind: ThreadKind
  authorRole: UserRole
  authorName: string
  /** Organization shown under the author, e.g. "City of Victoria". */
  authorOrg?: string
  body: string
  createdAt: string
}

interface ThreadRead extends Entity {
  /** `${threadId}:${role}` */
  id: string
  threadId: string
  role: UserRole
  lastReadAt: string
}

export const chatCollection = createCollection<ChatMessage>("case-chat")
export const chatReadCollection = createCollection<ThreadRead>("case-chat-reads")

export function listMessages(threadId: string): ChatMessage[] {
  return chatCollection
    .list()
    .filter((m) => m.threadId === threadId)
    .sort((a, b) => a.createdAt.localeCompare(b.createdAt))
}

export interface ThreadSummary {
  threadId: string
  kind: ThreadKind
  messageCount: number
  lastMessage: ChatMessage
  unread: number
}

/** Every thread, newest activity first, with the unread count for `role`. */
export function listThreads(role: UserRole, kind?: ThreadKind): ThreadSummary[] {
  const byThread = new Map<string, ChatMessage[]>()
  for (const m of chatCollection.list()) {
    if (kind && m.kind !== kind) continue
    const arr = byThread.get(m.threadId) ?? []
    arr.push(m)
    byThread.set(m.threadId, arr)
  }
  const out: ThreadSummary[] = []
  for (const [threadId, msgs] of byThread) {
    msgs.sort((a, b) => a.createdAt.localeCompare(b.createdAt))
    out.push({
      threadId,
      kind: msgs[0].kind,
      messageCount: msgs.length,
      lastMessage: msgs[msgs.length - 1],
      unread: countUnread(threadId, role, msgs),
    })
  }
  return out.sort((a, b) => b.lastMessage.createdAt.localeCompare(a.lastMessage.createdAt))
}

function countUnread(threadId: string, role: UserRole, msgs?: ChatMessage[]): number {
  const list = msgs ?? listMessages(threadId)
  const watermark = chatReadCollection.get(`${threadId}:${role}`)?.lastReadAt ?? ""
  return list.filter((m) => m.authorRole !== role && m.createdAt > watermark).length
}

export function unreadForThread(threadId: string, role: UserRole): number {
  return countUnread(threadId, role)
}

export function markThreadRead(threadId: string, role: UserRole): void {
  const id = `${threadId}:${role}`
  const now = new Date().toISOString()
  const prev = chatReadCollection.get(id)
  // Avoid a write (and a re-render storm) when nothing new has arrived.
  const latest = listMessages(threadId).at(-1)?.createdAt ?? ""
  if (prev && prev.lastReadAt >= latest) return
  chatReadCollection.upsert({ id, threadId, role, lastReadAt: now })
}

export interface PostMessageInput {
  threadId: string
  kind: ThreadKind
  authorRole: UserRole
  authorName: string
  authorOrg?: string
  body: string
  /** Shown in the notification sent to the other participants. */
  subjectLabel: string
  /** Per-role deep links so each participant lands on their own view of the file. */
  hrefFor: Partial<Record<UserRole, string>>
}

export function postMessage(input: PostMessageInput): ChatMessage {
  const body = input.body.trim()
  const message: ChatMessage = {
    id: makeId("msg"),
    threadId: input.threadId,
    kind: input.kind,
    authorRole: input.authorRole,
    authorName: input.authorName,
    authorOrg: input.authorOrg,
    body,
    createdAt: new Date().toISOString(),
  }
  chatCollection.upsert(message)
  // The author has obviously read their own message.
  chatReadCollection.upsert({
    id: `${input.threadId}:${input.authorRole}`,
    threadId: input.threadId,
    role: input.authorRole,
    lastReadAt: message.createdAt,
  })

  // Ping the other participant groups.
  const recipients = new Set<UserRole>(THREAD_PARTICIPANTS[input.kind])
  recipients.delete(input.authorRole)
  if (input.authorRole === "licensee" || input.authorRole === "applicant") {
    recipients.delete("licensee")
    recipients.delete("applicant")
  }
  const preview = body.length > 140 ? `${body.slice(0, 137)}…` : body
  for (const r of recipients) {
    const href = input.hrefFor[r]
    if (!href) continue
    notify({
      recipient: r,
      category: "application",
      title: `New message on ${input.subjectLabel}`,
      body: `${input.authorName} (${ROLE_LABELS[input.authorRole]}): ${preview}`,
      href,
      subjectId: input.threadId,
    })
  }
  return message
}

/** Demo conversations so the feature is visible before anyone types. */
export function seedChatIfEmpty(): void {
  const hoursAgo = (h: number) => new Date(Date.now() - h * 3600_000).toISOString()
  chatCollection.seedIfEmpty([
    {
      id: "msg_seed_lp1",
      threadId: "LP-125998",
      kind: "application",
      authorRole: "lcrb",
      authorName: "Kasey Poole",
      authorOrg: "LCRB Licensing",
      body: "Hi Dana — your Liquor Primary file for The Cedar Room has been referred to the City of Victoria for comment. I've added their reviewer to this thread so we can resolve questions in one place.",
      createdAt: hoursAgo(52),
    },
    {
      id: "msg_seed_lp2",
      threadId: "LP-125998",
      kind: "application",
      authorRole: "local-government",
      authorName: "Ricardo Tubbs",
      authorOrg: "City of Victoria",
      body: "Thanks. One question on the patio: the site plan shows 28 seats but the occupant-load letter says 24. Which is correct? We can comment as soon as that's confirmed.",
      createdAt: hoursAgo(27),
    },
    {
      id: "msg_seed_sep1",
      threadId: "SEP-2026-04788",
      kind: "sep",
      authorRole: "police",
      authorName: "Cst. Dan Whitmore",
      authorOrg: "West Shore RCMP",
      body: "Reviewing your Rotary fundraiser permit. Can you confirm whether the beer garden will be fenced with a single controlled entry? That affects the security staffing condition we apply.",
      createdAt: hoursAgo(20),
    },
  ])
}
