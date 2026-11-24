"use client"

/** Save-and-return draft store (prototype). */

import { computed, type Signal } from "@angular/core"
import { injectExternalStore, read, type Arg } from "@shared/util/external-store"

export interface ApplicationDraft {
  draftId: string
  /** Flow/application identifier, e.g. "sep" or "licensee-representative". */
  flowId: string
  title: string
  /** Where "Resume" should navigate. */
  resumeHref: string
  /** Saved form values. */
  formData: Record<string, unknown>
  /** Step the user was on when they saved. */
  currentStep: number
  /** 0–100 rough completion for display. */
  progress: number
  updatedAt: string
}

const STORAGE_KEY = "lcrb.app.drafts"
const listeners = new Set<() => void>()

function readAll(): Record<string, ApplicationDraft> {
  if (typeof window === "undefined") return {}
  try {
    const raw = localStorage.getItem(STORAGE_KEY)
    return raw ? (JSON.parse(raw) as Record<string, ApplicationDraft>) : {}
  } catch {
    return {}
  }
}

function writeAll(map: Record<string, ApplicationDraft>) {
  try {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(map))
  } catch {
    // ignore quota / availability errors
  }
  listeners.forEach((l) => l())
}

export function saveDraft(draft: ApplicationDraft) {
  const map = readAll()
  map[draft.draftId] = { ...draft, updatedAt: new Date().toISOString() }
  writeAll(map)
}

export function getDraft(draftId: string): ApplicationDraft | undefined {
  return readAll()[draftId]
}

export function deleteDraft(draftId: string) {
  const map = readAll()
  delete map[draftId]
  writeAll(map)
}

/** All saved drafts, newest first. Plain getter (safe outside React). */
export function getAllDrafts(): ApplicationDraft[] {
  return Object.values(readAll()).sort((a, b) => (a.updatedAt < b.updatedAt ? 1 : -1))
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

/** Live list of drafts, optionally filtered to a flow, newest first. */
export function injectDrafts(flowIdArg?: Arg<string | undefined>): Signal<{
  drafts: ApplicationDraft[]
  remove: (id: string) => void
}> {
  const getSnapshot = () => {
    const json = JSON.stringify(readAll())
    return json
  }

  const jsonSignal = injectExternalStore(subscribe, getSnapshot, () => "{}")
  const remove = (id: string) => deleteDraft(id)
  return computed(() => {
  const flowId = flowIdArg === undefined ? undefined : read(flowIdArg)
  const json = jsonSignal()
  const map = JSON.parse(json) as Record<string, ApplicationDraft>
  const drafts = Object.values(map)
    .filter((d) => (flowId ? d.flowId === flowId : true))
    .sort((a, b) => (a.updatedAt < b.updatedAt ? 1 : -1))

  return { drafts, remove }
  })
}
