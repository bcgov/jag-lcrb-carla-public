"use client"

/** How the applicant fills in an application: "form" (classic multi-step) or "guided" (one question at a time); stored per user, not per application. */

import { afterNextRender, DestroyRef, inject, signal, type Signal } from "@angular/core"

export type ApplicationMode = "form" | "guided"

const STORAGE_KEY = "lcrb.app.mode"
/** Same-tab broadcast: `storage` events only fire in OTHER tabs. */
const MODE_EVENT = "lcrb:app-mode-change"

export function readStoredMode(): ApplicationMode {
  if (typeof window === "undefined") return "form"
  try {
    return window.localStorage.getItem(STORAGE_KEY) === "guided" ? "guided" : "form"
  } catch {
    // Private browsing or storage disabled: fall back to the classic form.
    return "form"
  }
}

/** Read/write the preferred mode; `hydrated` is false on first paint, so render the classic form until it is true. */
export function injectApplicationMode(): {
  mode: Signal<ApplicationMode>
  setMode: (next: ApplicationMode) => void
  hydrated: Signal<boolean>
} {
  const mode = signal<ApplicationMode>("form")
  const hydrated = signal(false)

  afterNextRender(() => {
    mode.set(readStoredMode())
    hydrated.set(true)
  })

  // Keep every mounted toggle in sync — across tabs and within this one.
  const sync = () => mode.set(readStoredMode())
  const onStorage = (e: StorageEvent) => {
    if (e.key === STORAGE_KEY) sync()
  }
  window.addEventListener("storage", onStorage)
  window.addEventListener(MODE_EVENT, sync)
  inject(DestroyRef).onDestroy(() => {
    window.removeEventListener("storage", onStorage)
    window.removeEventListener(MODE_EVENT, sync)
  })

  const update = (next: ApplicationMode) => {
    mode.set(next)
    try {
      window.localStorage.setItem(STORAGE_KEY, next)
    } catch {
      /* preference simply won't persist */
    }
    window.dispatchEvent(new Event(MODE_EVENT))
  }

  return { mode: mode.asReadonly(), setMode: update, hydrated: hydrated.asReadonly() }
}
