"use client"

/** Security-screening store (prototype): people go not_started, invited, submitted, synced. TODO: real person list; wire activateScreening to notifications. */

import { computed, type Signal } from "@angular/core"
import { injectExternalStore, injectHydrated, read, type Arg } from "@shared/util/external-store"
import { createCollection } from "@data-access/repositories/mock-storage/kv-store"
import { type Entity } from "@domain/models/entity"
import { pushCase } from "@data-access/repositories/activity/dynamics-service"
import { recordAudit } from "@data-access/repositories/activity/audit-service"

export type ScreeningStatus = "not_started" | "invited" | "submitted" | "synced"
export type ScreeningCategory = "liquor" | "cannabis"

export interface ScreeningPerson extends Entity {
  id: string
  name: string
  role: string
  email: string
  category: ScreeningCategory
  formType: string
  status: ScreeningStatus
  invitedAt?: string
  submittedAt?: string
  syncedAt?: string
  dynamicsCaseId?: string
}

const collection = createCollection<ScreeningPerson>("security-screening")

const SEED: ScreeningPerson[] = [
  {
    id: "person-john-smith",
    name: "John Smith",
    role: "Director",
    email: "john.smith@example.com",
    category: "liquor",
    formType: "Personal History Summary",
    status: "synced",
    invitedAt: "2026-03-01T09:00:00.000Z",
    submittedAt: "2026-03-15T12:00:00.000Z",
    syncedAt: "2026-03-15T12:05:00.000Z",
    dynamicsCaseId: "LCRB-482910",
  },
  {
    id: "person-maria-chan",
    name: "Maria Chan",
    role: "Shareholder (25%+)",
    email: "maria.chan@example.com",
    category: "liquor",
    formType: "Personal History Summary",
    status: "not_started",
  },
  {
    id: "person-jane-doe",
    name: "Jane Doe",
    role: "Director",
    email: "jane.doe@example.com",
    category: "cannabis",
    formType: "Personal History Summary",
    status: "invited",
    invitedAt: "2026-05-10T15:30:00.000Z",
  },
  {
    id: "person-raj-patel",
    name: "Raj Patel",
    role: "Key personnel (General Manager)",
    email: "raj.patel@example.com",
    category: "cannabis",
    formType: "Personal History Summary",
    status: "not_started",
  },
]

function ensureSeed() {
  collection.seedIfEmpty(SEED)
}

/** The document subject id used to store a person's uploaded PHS form. */
export function screeningSubjectId(personId: string): string {
  return `screening:${personId}`
}

/** Mark a person as invited and (simulated) email them their PHS instructions. */
export function activateScreening(personId: string, actor = "Business user"): void {
  const person = collection.get(personId)
  if (!person) return
  collection.update(personId, {
    status: "invited",
    invitedAt: new Date().toISOString(),
  })
  recordAudit({
    category: "notification",
    action: "screening.invited",
    summary: `Screening invitation emailed to ${person.name} (${person.email})`,
    actor,
    subjectId: personId,
  })
}

/** Records that the business user uploaded the person's completed form, then pushes it to Dynamics for staff review. */
export function submitScreening(personId: string, actor = "Business user"): void {
  const person = collection.get(personId)
  if (!person) return
  const now = new Date().toISOString()
  collection.update(personId, { status: "submitted", submittedAt: now })

  const sync = pushCase({
    reference: `PHS-${personId}`,
    caseType: `Security Screening — ${person.formType}`,
    applicant: person.name,
    payloadSummary: `${person.role} · ${person.category} · completed PHS uploaded`,
    actor,
  })

  if (sync.state === "synced") {
    collection.update(personId, {
      status: "synced",
      syncedAt: new Date().toISOString(),
      dynamicsCaseId: sync.dynamicsCaseId,
    })
  }
}

/** Reset a person back to not-started (e.g. to re-send). */
export function resetScreening(personId: string): void {
  collection.update(personId, {
    status: "not_started",
    invitedAt: undefined,
    submittedAt: undefined,
    syncedAt: undefined,
    dynamicsCaseId: undefined,
  })
}

/** Reactive list of screening people. */
export function injectScreeningPeople(): Signal<ScreeningPerson[]> {
  const jsonSignal = injectExternalStore(collection.subscribe, collection.snapshot)
  const hydrated = injectHydrated(() => ensureSeed())
  return computed(() => {
    const json = jsonSignal()
    if (!hydrated()) return SEED
    try {
      const map = JSON.parse(json) as Record<string, ScreeningPerson>
      const values = Object.values(map)
      return values.length > 0 ? values : SEED
    } catch {
      return SEED
    }
  })
}
