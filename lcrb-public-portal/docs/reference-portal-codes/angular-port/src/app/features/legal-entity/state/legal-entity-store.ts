"use client"

/** Legal Entity build store (prototype): one build per business in the kv-store; refuses licensee writes when `lockedByLCRB`. Swap kv-store for the Dynamics API. */

import { computed, effect, untracked, type Signal } from "@angular/core"
import { injectExternalStore, injectHydrated, read, type Arg } from "@shared/util/external-store"
import { createCollection, makeId } from "@data-access/repositories/mock-storage/kv-store"
import { pushCase } from "@data-access/repositories/activity/dynamics-service"
import { recordAudit } from "@data-access/repositories/activity/audit-service"
import { buildDeterministicAgentLog } from "@data-access/repositories/ai/prescreen-agent"
import {
  collectTiedHouseForBusiness,
  deriveTiedHouseOverlay,
  tiedHouseCollection,
  type TiedHouseOverlay,
} from "@core/navigation/link-context"
import { getBusinessProfile } from "@data-access/repositories/account/mock/business-profile"
import { MOCK_LICENCES } from "@data-access/repositories/licences/mock/licences"
import { createSampleBuild } from "@data-access/repositories/legal-entity/mock/legal-entity"
import { type LegalEntityBuild, type LENode, type LEEdge, type LEPerson, type LERelationshipType, type LEEntityType, type LERole } from "@domain/models/legal-entity"

const collection = createCollection<LegalEntityBuild>("legal-entity-builds")

function buildIdFor(businessNumber: string): string {
  return `build_${businessNumber}`
}

/** Ensure a build exists for the business; create the sample one if not. */
export function ensureBuild(businessLegalName: string, businessNumber: string): LegalEntityBuild {
  const id = buildIdFor(businessNumber)
  const existing = collection.get(id)
  if (existing) return existing
  const seeded = createSampleBuild(businessLegalName, businessNumber)
  collection.upsert(seeded)
  return seeded
}

function touch(build: LegalEntityBuild): LegalEntityBuild {
  return { ...build, updatedAt: new Date().toISOString() }
}

/** Guard: returns the build only if the licensee is allowed to edit it. */
function editable(id: string): LegalEntityBuild | undefined {
  const build = collection.get(id)
  if (!build || build.lockedByLCRB) return undefined
  return build
}

function logSuggestion(
  build: LegalEntityBuild,
  actor: string,
  kind: LegalEntityBuild["suggestions"][number]["kind"],
  summary: string,
): LegalEntityBuild {
  return {
    ...build,
    suggestions: [
      { id: makeId("sug"), at: new Date().toISOString(), actor, kind, summary },
      ...build.suggestions,
    ].slice(0, 50),
  }
}


/** Move a node (persisted after drag). Allowed even when reviewing, but not when locked. */
export function updateNodePosition(id: string, nodeId: string, position: { x: number; y: number }) {
  const build = editable(id)
  if (!build) return
  collection.update(id, {
    nodes: build.nodes.map((n) => (n.id === nodeId ? { ...n, position } : n)),
  })
}

export interface NewNodeInput {
  type: LEEntityType
  legalName: string
  businessNumber?: string
  incorporationNumber?: string
  jurisdiction?: string
  note?: string
}

export function addNode(id: string, actor: string, input: NewNodeInput): LENode | undefined {
  const build = editable(id)
  if (!build) return undefined
  const node: LENode = {
    id: makeId("node"),
    type: input.type,
    legalName: input.legalName,
    businessNumber: input.businessNumber || undefined,
    incorporationNumber: input.incorporationNumber || undefined,
    jurisdiction: input.jurisdiction || undefined,
    linkedLicenceIds: [],
    people: [],
    // Stagger new cards so they don't stack exactly on top of each other.
    position: { x: 200 + build.nodes.length * 24, y: 220 + build.nodes.length * 20 },
    source: "portal",
    locked: false,
    note: input.note || undefined,
  }
  const next = logSuggestion(
    { ...build, nodes: [...build.nodes, node] },
    actor,
    "add-node",
    `Added ${input.type.replace("-", " ")} "${input.legalName}"`,
  )
  collection.upsert(touch(next))
  return node
}

export interface DirectoryBusinessInput {
  type: LEEntityType
  legalName: string
  /** Public registration / incorporation number or BN. */
  registrationNumber: string
  jurisdiction?: string
  linkedLicenceIds: string[]
}

/** Adds a directory business as a record-sourced card with locked identity; de-duplicates on registration number and does not copy people in. */
export function linkDirectoryBusiness(
  id: string,
  actor: string,
  input: DirectoryBusinessInput,
): LENode | undefined {
  const build = editable(id)
  if (!build) return undefined
  const already = build.nodes.find(
    (n) =>
      (n.businessNumber && n.businessNumber === input.registrationNumber) ||
      (n.incorporationNumber && n.incorporationNumber === input.registrationNumber),
  )
  if (already) return already
  const isBn = /^\d{9}$|^BC/i.test(input.registrationNumber)
  const node: LENode = {
    id: makeId("node"),
    type: input.type,
    legalName: input.legalName,
    businessNumber: isBn ? input.registrationNumber : undefined,
    incorporationNumber: input.registrationNumber,
    jurisdiction: input.jurisdiction,
    linkedLicenceIds: input.linkedLicenceIds,
    people: [],
    position: { x: 200 + build.nodes.length * 24, y: 240 + build.nodes.length * 20 },
    source: "record",
    locked: true,
  }
  const next = logSuggestion(
    { ...build, nodes: [...build.nodes, node] },
    actor,
    "add-node",
    `Added business "${input.legalName}" (${input.registrationNumber}) from records`,
  )
  collection.upsert(touch(next))
  return node
}

/** Adds a directory person as an individual card; only the name is carried over and the identity is locked. */
export function linkDirectoryIndividual(id: string, actor: string, name: string): LENode | undefined {
  const build = editable(id)
  if (!build) return undefined
  const already = build.nodes.find(
    (n) => n.type === "individual" && n.legalName.toLowerCase() === name.trim().toLowerCase(),
  )
  if (already) return already
  const node: LENode = {
    id: makeId("node"),
    type: "individual",
    legalName: name.trim(),
    linkedLicenceIds: [],
    people: [],
    position: { x: 200 + build.nodes.length * 24, y: 260 + build.nodes.length * 20 },
    source: "record",
    locked: true,
  }
  const next = logSuggestion(
    { ...build, nodes: [...build.nodes, node] },
    actor,
    "add-node",
    `Added individual "${name.trim()}" from records`,
  )
  collection.upsert(touch(next))
  return node
}

export function updateNode(id: string, actor: string, nodeId: string, patch: Partial<LENode>) {
  const build = editable(id)
  if (!build) return
  const target = build.nodes.find((n) => n.id === nodeId)
  if (!target) return
  // Never allow editing locked identity fields on record-sourced cards.
  const safePatch: Partial<LENode> = { ...patch }
  if (target.locked) {
    delete safePatch.legalName
    delete safePatch.businessNumber
    delete safePatch.incorporationNumber
    delete safePatch.linkedLicenceIds
  }
  const next = logSuggestion(
    { ...build, nodes: build.nodes.map((n) => (n.id === nodeId ? { ...n, ...safePatch } : n)) },
    actor,
    "update-node",
    `Updated "${target.legalName}"`,
  )
  collection.upsert(touch(next))
}

export function removeNode(id: string, actor: string, nodeId: string) {
  const build = editable(id)
  if (!build) return
  const target = build.nodes.find((n) => n.id === nodeId)
  // Record-sourced cards and the applicant cannot be removed in the portal.
  if (!target || target.locked || target.type === "applicant") return
  const next = logSuggestion(
    {
      ...build,
      nodes: build.nodes.filter((n) => n.id !== nodeId),
      edges: build.edges.filter((e) => e.source !== nodeId && e.target !== nodeId),
    },
    actor,
    "remove",
    `Removed "${target.legalName}"`,
  )
  collection.upsert(touch(next))
}

export interface NewEdgeInput {
  source: string
  target: string
  type: LERelationshipType
  sharePercent?: number
  label?: string
}

export function addEdge(id: string, actor: string, input: NewEdgeInput): LEEdge | undefined {
  const build = editable(id)
  if (!build) return undefined
  if (input.source === input.target) return undefined
  // Avoid duplicate identical relationships.
  const dup = build.edges.find(
    (e) => e.source === input.source && e.target === input.target && e.type === input.type,
  )
  if (dup) return dup
  const edge: LEEdge = {
    id: makeId("edge"),
    source: input.source,
    target: input.target,
    type: input.type,
    sharePercent: input.sharePercent,
    label: input.label,
    locked: false,
  }
  const sName = build.nodes.find((n) => n.id === input.source)?.legalName ?? "?"
  const tName = build.nodes.find((n) => n.id === input.target)?.legalName ?? "?"
  const next = logSuggestion(
    { ...build, edges: [...build.edges, edge] },
    actor,
    "add-relationship",
    `Linked "${sName}" → "${tName}" (${input.type})`,
  )
  collection.upsert(touch(next))
  return edge
}

export function removeEdge(id: string, actor: string, edgeId: string) {
  const build = editable(id)
  if (!build) return
  const target = build.edges.find((e) => e.id === edgeId)
  if (!target || target.locked) return
  const next = logSuggestion(
    { ...build, edges: build.edges.filter((e) => e.id !== edgeId) },
    actor,
    "remove",
    `Removed a relationship`,
  )
  collection.upsert(touch(next))
}

export function addPerson(id: string, actor: string, nodeId: string, person: { name: string; role: LERole }) {
  const build = editable(id)
  if (!build) return
  const target = build.nodes.find((n) => n.id === nodeId)
  if (!target) return
  const p: LEPerson = { id: makeId("p"), name: person.name, role: person.role }
  const next = logSuggestion(
    {
      ...build,
      nodes: build.nodes.map((n) => (n.id === nodeId ? { ...n, people: [...n.people, p] } : n)),
    },
    actor,
    "update-node",
    `Added ${person.role.replace("-", " ")} "${person.name}" to "${target.legalName}"`,
  )
  collection.upsert(touch(next))
}

export function removePerson(id: string, actor: string, nodeId: string, personId: string) {
  const build = editable(id)
  if (!build) return
  const target = build.nodes.find((n) => n.id === nodeId)
  if (!target) return
  const person = target.people.find((p) => p.id === personId)
  if (!person || person.locked) return
  const next = {
    ...build,
    nodes: build.nodes.map((n) =>
      n.id === nodeId ? { ...n, people: n.people.filter((p) => p.id !== personId) } : n,
    ),
  }
  collection.upsert(touch(next))
}

/** Submit the build to LCRB for review and push a case to Dynamics. */
export function submitToLCRB(id: string, actor: string): LegalEntityBuild | undefined {
  const build = collection.get(id)
  if (!build || build.lockedByLCRB) return undefined

  // Run the deterministic pre-screen (licence cap across this build and the tied house register, plus Section 119) to route the case and attach an agent log. Sync-only, no AI narrative.
  const profile = getBusinessProfile(build.businessLegalName)
  const tied = collectTiedHouseForBusiness(build.businessNumber)
  const { agentLog } = buildDeterministicAgentLog({
    title: "Legal Entity Build",
    applicantName: build.businessLegalName,
    ownedLicences: MOCK_LICENCES,
    leBuild: build,
    existingRelationships: tied.existingRelationships,
    declarations: tied.declarations,
    firstNation: profile.firstNation,
  })

  const sync = pushCase({
    reference: build.id,
    caseType: "Legal Entity Build",
    applicant: build.businessLegalName,
    payloadSummary: `${build.nodes.length} entities, ${build.edges.length} relationships`,
    actor,
    queue: agentLog.recommendedQueue,
    agentLog,
  })
  const next = touch({
    ...build,
    status: "submitted",
    version: build.version + 1,
    lastSyncedAt: sync.state === "synced" ? sync.lastAttemptAt : build.lastSyncedAt,
    dynamicsCaseId: sync.dynamicsCaseId ?? build.dynamicsCaseId,
  })
  collection.upsert(next)
  recordAudit({
    category: "integration",
    action: "legal_entity.submitted",
    summary: `Legal Entity build submitted to LCRB for ${build.businessLegalName}`,
    actor,
    subjectId: build.id,
    meta: { queue: agentLog.recommendedQueue, overallConfidence: agentLog.overallConfidence },
  })
  return next
}

/** Reset a submitted/approved build back to draft so the licensee can revise. */
export function reopenDraft(id: string, actor: string) {
  const build = collection.get(id)
  if (!build || build.lockedByLCRB) return
  collection.upsert(touch({ ...build, status: "draft" }))
  recordAudit({
    category: "integration",
    action: "legal_entity.reopened",
    summary: `Legal Entity build reopened for editing (${build.businessLegalName})`,
    actor,
    subjectId: build.id,
  })
}


/** Simulates LCRB staff locking or unlocking the build in Dynamics; when locked the portal is read-only. */
export function lcrbSetLock(id: string, locked: boolean, staff = "LCRB Case Manager") {
  const build = collection.get(id)
  if (!build) return
  collection.upsert(
    touch({
      ...build,
      lockedByLCRB: locked,
      status: locked ? "under-review" : build.status,
      reviewNotes: [
        {
          id: makeId("rev"),
          at: new Date().toISOString(),
          actor: staff,
          decision: "comment",
          message: locked
            ? "Build locked by LCRB — under staff review in Dynamics. Portal editing is disabled."
            : "Build unlocked by LCRB — you can suggest changes again.",
        },
        ...build.reviewNotes,
      ],
    }),
  )
}

/** Simulate an LCRB review decision from Dynamics. */
export function lcrbReview(
  id: string,
  decision: "approved" | "changes-requested",
  message: string,
  staff = "LCRB Case Manager",
) {
  const build = collection.get(id)
  if (!build) return
  collection.upsert(
    touch({
      ...build,
      status: decision,
      lockedByLCRB: decision === "approved" ? true : build.lockedByLCRB,
      reviewNotes: [
        { id: makeId("rev"), at: new Date().toISOString(), actor: staff, decision, message },
        ...build.reviewNotes,
      ],
    }),
  )
}


/** Reactive access to the build; seeds a sample on first use, with a hydration guard so the first client render matches the server. */
export function injectLegalEntityBuild(
  businessLegalNameArg: Arg<string>,
  businessNumberArg: Arg<string>,
): Signal<LegalEntityBuild> {
  const version = injectExternalStore(collection.subscribe, collection.snapshot)
  const hydrated = injectHydrated(() => ensureBuild(read(businessLegalNameArg), read(businessNumberArg)))
  // Re-ensure when the business changes after hydration.
  effect(() => {
    const name = read(businessLegalNameArg)
    const number = read(businessNumberArg)
    if (untracked(hydrated)) ensureBuild(name, number)
  })

  return computed(() => {
    version()
    const businessLegalName = read(businessLegalNameArg)
    const businessNumber = read(businessNumberArg)
    const id = buildIdFor(businessNumber)
    const stored = hydrated() ? collection.get(id) : undefined
    return stored ?? createSampleBuild(businessLegalName, businessNumber)
  })
}

const EMPTY_OVERLAY: TiedHouseOverlay = { nodes: [], edges: [] }

/** Live read-only overlay of Tied House associations as nodes and edges; empty until hydrated. */
export function injectTiedHouseOverlay(businessNumber: Arg<string>, applicantNodeId: Arg<string>): Signal<TiedHouseOverlay> {
  const snapshot = injectExternalStore(tiedHouseCollection.subscribe, tiedHouseCollection.snapshot)
  const hydrated = injectHydrated()

  return computed(() => {
    // `snapshot` changes on every tied-house mutation, refreshing the overlay.
    snapshot()
    return hydrated() ? deriveTiedHouseOverlay(read(businessNumber), read(applicantNodeId)) : EMPTY_OVERLAY
  })
}
