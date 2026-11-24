"use client"

/** Portal announcements: policy items, regulatory updates, and Portal release notes (with changelogs). */

import { computed, type Signal } from "@angular/core"
import { injectExternalStore } from "@shared/util/external-store"
import { type Announcement } from "@domain/models/notifications"

/** Newest first. */
export const ANNOUNCEMENTS: Announcement[] = [
  {
    id: "2026-06-portal-2-4",
    date: "2026-06-18",
    category: "release",
    title: "Portal 2.4: AI document checks and a smarter assistant",
    version: "2.4.0",
    summary:
      "Uploads are now automatically checked for completeness, and the licensing assistant can start applications for you from plain-language requests.",
    body: [
      "This release focuses on reducing back-and-forth during the application process. When you upload a supporting document, the Portal now checks it against the requirements and tells you if something looks missing before you submit.",
      "The licensing assistant can also recognise what you are trying to do and offer one-tap buttons to start the right application.",
    ],
    changelog: [
      { type: "added", text: "AI completeness check on every document upload, with inline guidance." },
      { type: "added", text: "Assistant action buttons to start applications from natural language." },
      { type: "added", text: "Redesigned payment confirmation and printable receipt." },
      { type: "improved", text: "Assistant now recovers automatically from brief network interruptions." },
      { type: "fixed", text: "Corrected navigation links for police event pages." },
    ],
  },
  {
    id: "2026-06-serving-it-right",
    date: "2026-06-02",
    category: "policy",
    title: "Serving It Right certificates required at submission",
    summary:
      "Liquor Primary and Food Primary applicants must now provide a valid Serving It Right certificate number when applying.",
    body: [
      "Under the Liquor Control and Licensing Act, licensees and their staff must hold a valid Serving It Right certificate. To reduce processing delays, applicants are now asked to provide their certificate number at the time of application.",
      "If you do not yet have a certificate, you can complete the Serving It Right course online before submitting your application.",
    ],
  },
  {
    id: "2026-05-cannabis-fee-update",
    date: "2026-05-15",
    category: "regulatory",
    title: "Updated cannabis retail licence fees effective July 1",
    summary:
      "Annual licence fees for cannabis retail stores will be updated effective July 1, 2026. Review the new fee schedule.",
    body: [
      "Following the annual review under the Cannabis Control and Licensing Act, the fee schedule for cannabis retail store licences has been updated. The new fees apply to applications submitted on or after July 1, 2026.",
      "Applications submitted before this date will be assessed under the current fee schedule.",
    ],
  },
  {
    id: "2026-04-scheduled-maintenance",
    date: "2026-04-20",
    category: "service",
    title: "Scheduled maintenance — Sunday, April 27",
    summary:
      "The Portal will be unavailable from 1:00 AM to 5:00 AM PT on Sunday, April 27 for scheduled maintenance.",
    body: [
      "To improve performance and reliability, the Portal will undergo scheduled maintenance on Sunday, April 27 between 1:00 AM and 5:00 AM Pacific Time. During this window, you will not be able to sign in or submit applications.",
      "Any in-progress applications are saved as drafts and will be available when the Portal returns.",
    ],
  },
]

/** The id of the most recent announcement (used for the "new" indicator). */
export function latestAnnouncementId(): string {
  return ANNOUNCEMENTS[0]?.id ?? ""
}

const STORAGE_KEY = "lcrb.announcements.lastSeen"
const listeners = new Set<() => void>()

function emit() {
  listeners.forEach((l) => l())
}

function subscribe(cb: () => void) {
  listeners.add(cb)
  const onStorage = (e: StorageEvent) => {
    if (e.key === STORAGE_KEY) cb()
  }
  window.addEventListener("storage", onStorage)
  return () => {
    listeners.delete(cb)
    window.removeEventListener("storage", onStorage)
  }
}

function readLastSeen(): string {
  if (typeof window === "undefined") return ""
  try {
    return localStorage.getItem(STORAGE_KEY) ?? ""
  } catch {
    return ""
  }
}

/** Count of announcements published after the last one the user viewed. */
export function injectUnreadAnnouncements(): Signal<{ count: number; markAllSeen: () => void }> {
  const lastSeenSignal = injectExternalStore(subscribe, readLastSeen, () => "")

  return computed(() => {
  const lastSeen = lastSeenSignal()
  const count = (() => {
    if (!lastSeen) return ANNOUNCEMENTS.length
    const seenIndex = ANNOUNCEMENTS.findIndex((a) => a.id === lastSeen)
    // Everything before the seen index is newer (list is newest-first).
    return seenIndex === -1 ? ANNOUNCEMENTS.length : seenIndex
  })()

  return { count, markAllSeen }
  })
}

function markAllSeen(): void {
    try {
      localStorage.setItem(STORAGE_KEY, latestAnnouncementId())
      emit()
    } catch {
      // ignore
    }
}
