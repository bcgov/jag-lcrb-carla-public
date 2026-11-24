"use client"

/** Saved establishment plans (answers, checklist progress); feasibility is in `establishment-feasibility.ts`. Prototype: localStorage via `kv-store`. */

import { computed, type Signal } from "@angular/core"
import { injectExternalStore, injectHydrated, read, type Arg } from "@shared/util/external-store"
import { createCollection } from "@data-access/repositories/mock-storage/kv-store"
import { type Entity } from "@domain/models/entity"
import type { PlannerAnswers } from "@data-access/repositories/applications/establishment-feasibility"

export interface EstablishmentPlan extends Entity {
  id: string
  title: string
  createdAt: string
  updatedAt: string
  answers: PlannerAnswers
  /** Checklist item id → completed. */
  checklistDone: Record<string, boolean>
}

const collection = createCollection<EstablishmentPlan>("establishment-plans")

function uid(): string {
  return `plan-${Date.now().toString(36)}-${Math.random().toString(36).slice(2, 7)}`
}

/** Create a plan and return it; optionally seed answers already known from the signed-in business account (legal name, existing licence holdings). */
export function createPlan(title = "Untitled establishment", seed?: Partial<PlannerAnswers>): EstablishmentPlan {
  const now = new Date().toISOString()
  const plan: EstablishmentPlan = {
    id: uid(),
    title,
    createdAt: now,
    updatedAt: now,
    answers: seed ? { ...seed } : {},
    checklistDone: {},
  }
  collection.upsert(plan)
  return plan
}

export function getPlan(id: string): EstablishmentPlan | undefined {
  return collection.get(id)
}

/** Patch a plan's answers (merges) and bump updatedAt. */
export function updatePlanAnswers(id: string, patch: Partial<PlannerAnswers>): void {
  collection.update(id, (prev) => ({
    ...prev,
    answers: { ...prev.answers, ...patch },
    updatedAt: new Date().toISOString(),
  }))
}

/** Rename a plan. */
export function renamePlan(id: string, title: string): void {
  collection.update(id, { title, updatedAt: new Date().toISOString() })
}

/** Toggle a checklist item. */
export function setChecklistItem(id: string, itemId: string, done: boolean): void {
  collection.update(id, (prev) => ({
    ...prev,
    checklistDone: { ...prev.checklistDone, [itemId]: done },
    updatedAt: new Date().toISOString(),
  }))
}

export function deletePlan(id: string): void {
  collection.remove(id)
}

/** Reactive list of all saved plans (newest first). */
export function injectEstablishmentPlans(): Signal<EstablishmentPlan[]> {
  const jsonSignal = injectExternalStore(collection.subscribe, collection.snapshot)
  const hydrated = injectHydrated()
  return computed(() => {
    const json = jsonSignal()
    if (!hydrated()) return []
    try {
      const map = JSON.parse(json) as Record<string, EstablishmentPlan>
      return Object.values(map).sort((a, b) => b.updatedAt.localeCompare(a.updatedAt))
    } catch {
      return []
    }
  })
}

/** Reactive single plan by id. */
export function injectEstablishmentPlan(idArg: Arg<string | undefined>): Signal<EstablishmentPlan | undefined> {
  const jsonSignal = injectExternalStore(collection.subscribe, collection.snapshot)
  const hydrated = injectHydrated()
  return computed(() => {
    const json = jsonSignal()
    const id = read(idArg)
    if (!hydrated() || !id) return undefined
    try {
      const map = JSON.parse(json) as Record<string, EstablishmentPlan>
      return map[id]
    } catch {
      return undefined
    }
  })
}
