"use client"

/** Shared, reactive store for the licensee's Notices feed. */

import { computed, type Signal } from "@angular/core"
import { injectExternalStore, injectHydrated } from "@shared/util/external-store"
import { createCollection } from "../mock-storage/kv-store"
import { type Entity } from "@domain/models/entity"
import { NOTICES } from "./mock/notices"
import { type Notice } from "@domain/models/notifications"

interface NoticeReadState extends Entity {
  /** String form of the notice id. */
  id: string
  read: boolean
}

const readState = createCollection<NoticeReadState>("notice-read-state")

export type FeedNotice = Notice

/** Notices in display order (newest first), using their built-in read flags. */
function defaultNoticesFeed(): FeedNotice[] {
  return [...NOTICES].sort((a, b) => (a.date < b.date ? 1 : -1))
}

/** Notices with persisted read overrides applied, newest first. */
export function listNoticesFeed(): FeedNotice[] {
  const overrides = new Map(readState.list().map((r) => [r.id, r.read]))
  return NOTICES.map((n) => ({
    ...n,
    read: overrides.has(String(n.id)) ? (overrides.get(String(n.id)) as boolean) : n.read,
  })).sort((a, b) => (a.date < b.date ? 1 : -1))
}

export function noticesUnreadCount(): number {
  return listNoticesFeed().filter((n) => !n.read).length
}

export function markNoticeRead(id: number): void {
  readState.upsert({ id: String(id), read: true })
}

export function markAllNoticesRead(): void {
  for (const n of NOTICES) readState.upsert({ id: String(n.id), read: true })
}

/** Reactive read side for the Notices feed. */
export function injectNoticesFeed(): Signal<{ notices: FeedNotice[]; unread: number; hydrated: boolean }> {
  const version = injectExternalStore(readState.subscribe, readState.snapshot)

  // Persisted read state lives in localStorage, which doesn't exist during SSR.
  const hydratedSignal = injectHydrated()

  return computed(() => {
    version()
    const hydrated = hydratedSignal()
    const notices = hydrated ? listNoticesFeed() : defaultNoticesFeed()
    const unread = notices.filter((n) => !n.read).length
    return { notices, unread, hydrated }
  })
}
