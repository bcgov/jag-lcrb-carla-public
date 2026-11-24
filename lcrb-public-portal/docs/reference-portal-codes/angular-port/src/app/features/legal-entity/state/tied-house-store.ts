"use client"

/** Tied House store (prototype): one application per source and business in the kv-store; submit needs both mock fees paid. Swap in Dynamics and real payments. */

import { computed, effect, untracked, type Signal } from "@angular/core"
import { injectExternalStore, injectHydrated, read, type Arg } from "@shared/util/external-store"
import { createCollection, makeId } from "@data-access/repositories/mock-storage/kv-store"
import { createPayment, processPayment } from "@data-access/repositories/payments/payment-service"
import { pushCase } from "@data-access/repositories/activity/dynamics-service"
import { recordAudit } from "@data-access/repositories/activity/audit-service"
import { buildDeterministicAgentLog } from "@data-access/repositories/ai/prescreen-agent"
import { getBuildForBusiness, collectTiedHouseForBusiness } from "@core/navigation/link-context"
import { getBusinessProfile } from "@data-access/repositories/account/mock/business-profile"
import { MOCK_LICENCES } from "@data-access/repositories/licences/mock/licences"
import { sampleExistingRelationships } from "@data-access/repositories/tied-house/mock/tied-house"
import { requiredDocsFor, relationshipLabel } from "@domain/tied-house/tied-house"
import { computeFees } from "@data-access/repositories/tied-house/mock/tied-house"
import { type TiedHouseApplication, type TiedHouseDeclaration, type TiedHouseSource, type TiedHouseType, type TiedHouseDomain, type IndividualDetails, type LegalEntityDetails, type ExistingRelationship } from "@domain/models/tied-house"

const collection = createCollection<TiedHouseApplication>("tied-house-applications")

function appIdFor(source: TiedHouseSource, businessNumber: string): string {
  return `th_${source}_${businessNumber}`
}

function jobNumberFor(source: TiedHouseSource): string {
  const seq = 120000 + Math.floor(Math.random() * 9999)
  if (source === "le-review") return `Legal Entity Review Job No. ${seq}`
  if (source === "new-application") return `New Application Job No. ${seq}`
  return `Tied House Update Job No. ${seq}`
}

/** Ensure an application exists for the business + source. */
export function ensureApplication(
  source: TiedHouseSource,
  businessLegalName: string,
  businessNumber: string,
): TiedHouseApplication {
  const id = appIdFor(source, businessNumber)
  const existing = collection.get(id)
  if (existing) return existing
  const seeded: TiedHouseApplication = {
    id,
    businessLegalName,
    businessNumber,
    source,
    jobNumber: jobNumberFor(source),
    status: "draft",
    declarations: [],
    existingRelationships: sampleExistingRelationships(),
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString(),
  }
  collection.upsert(seeded)
  return seeded
}

function touch(app: TiedHouseApplication): TiedHouseApplication {
  return { ...app, updatedAt: new Date().toISOString() }
}

/** Recompute the application status from its declarations + payment state. */
function withStatus(app: TiedHouseApplication): TiedHouseApplication {
  if (app.submittedAt) return { ...app, status: "submitted" }
  const fees = computeFees(app.declarations, app.jobNumber)
  const hasSaved = app.declarations.some((d) => d.status === "saved")
  const owesLiquor = fees.liquor.total > 0 && !app.liquorReceipt
  const owesCannabis = fees.cannabis.total > 0 && !app.cannabisReceipt
  const status = hasSaved && (owesLiquor || owesCannabis) ? "awaiting-payment" : "draft"
  return { ...app, status }
}


/** Add a new DRAFT declaration (unsaved, editable). Returns the new id. */
export function addDeclaration(id: string, type: TiedHouseType): string | undefined {
  const app = collection.get(id)
  if (!app || app.submittedAt) return undefined
  const decl: TiedHouseDeclaration = {
    id: makeId("decl"),
    type,
    status: "draft",
    changeType: type === "individual" ? "add-individual" : "add-legal-entity",
    associatedLicenceNumber: "",
    associatedEstablishmentName: "",
    domain: "liquor",
    requiredDocs: [],
    createdAt: new Date().toISOString(),
  }
  collection.upsert(touch({ ...app, declarations: [...app.declarations, decl] }))
  return decl.id
}

export interface SaveDeclarationInput {
  type: TiedHouseType
  individual?: IndividualDetails
  entity?: LegalEntityDetails
  associatedLicenceNumber: string
  associatedEstablishmentName: string
  domain: TiedHouseDomain
}

/** Saves (locks) a declaration: captures details, computes required documents and the date tied, and recomputes status so the payment panel reveals. */
export function saveDeclaration(id: string, declarationId: string, input: SaveDeclarationInput) {
  const app = collection.get(id)
  if (!app || app.submittedAt) return
  const next = app.declarations.map((d) =>
    d.id === declarationId
      ? {
          ...d,
          type: input.type,
          status: "saved" as const,
          changeType:
            d.changeType === "change-details"
              ? d.changeType
              : input.type === "individual"
                ? ("add-individual" as const)
                : ("add-legal-entity" as const),
          individual: input.individual,
          entity: input.entity,
          associatedLicenceNumber: input.associatedLicenceNumber,
          associatedEstablishmentName: input.associatedEstablishmentName,
          domain: input.domain,
          requiredDocs: requiredDocsFor(input.type, input.domain),
          dateTied: new Date().toISOString(),
        }
      : d,
  )
  collection.upsert(touch(withStatus({ ...app, declarations: next })))
  recordAudit({
    category: "application",
    action: "tied_house.declaration_saved",
    summary: `Tied house declaration saved for ${app.businessLegalName}`,
    actor: app.businessLegalName,
    subjectId: app.id,
  })
}

/** CANCEL a draft (never saved) declaration: clears the fields entirely. */
export function cancelDeclaration(id: string, declarationId: string) {
  const app = collection.get(id)
  if (!app || app.submittedAt) return
  collection.upsert(
    touch(withStatus({ ...app, declarations: app.declarations.filter((d) => d.id !== declarationId) })),
  )
}

/** REMOVE a saved declaration: clears it and hides the payment panel if empty. */
export function removeDeclaration(id: string, declarationId: string) {
  const app = collection.get(id)
  if (!app || app.submittedAt) return
  // Removing a saved declaration invalidates any payment already made against the changed fee set, so clear receipts.
  collection.upsert(
    touch(
      withStatus({
        ...app,
        declarations: app.declarations.filter((d) => d.id !== declarationId),
        liquorReceipt: undefined,
        cannabisReceipt: undefined,
      }),
    ),
  )
}

/** Turn an existing relationship into an editable "change details" declaration. */
export function startChangeFromExisting(id: string, existing: ExistingRelationship): string | undefined {
  const app = collection.get(id)
  if (!app || app.submittedAt) return undefined
  const decl: TiedHouseDeclaration = {
    id: makeId("decl"),
    type: existing.type,
    status: "draft",
    changeType: "change-details",
    associatedLicenceNumber: existing.associatedLicenceNumber,
    associatedEstablishmentName: existing.associatedEstablishmentName,
    domain: existing.domain,
    requiredDocs: [],
    createdAt: new Date().toISOString(),
  }
  collection.upsert(touch({ ...app, declarations: [...app.declarations, decl] }))
  return decl.id
}

/** Remove an existing relationship as a zero-fee "remove" declaration. */
export function removeExisting(id: string, existing: ExistingRelationship) {
  const app = collection.get(id)
  if (!app || app.submittedAt) return
  const decl: TiedHouseDeclaration = {
    id: makeId("decl"),
    type: existing.type,
    status: "saved",
    changeType: "remove",
    associatedLicenceNumber: existing.associatedLicenceNumber,
    associatedEstablishmentName: existing.associatedEstablishmentName,
    domain: existing.domain,
    requiredDocs: [],
    dateTied: existing.dateTied,
    createdAt: new Date().toISOString(),
    individual: existing.type === "individual" ? {
      dateOfBirth: "",
      firstName: existing.displayName.split(" – ")[0] ?? existing.displayName,
      lastName: "",
      relationship: existing.relationshipLabel.toLowerCase(),
    } : undefined,
    entity: existing.type === "legal-entity" ? {
      businessType: "",
      legalEntityName: existing.displayName,
      relationshipType: existing.relationshipLabel.toLowerCase(),
    } : undefined,
  }
  collection.upsert(
    touch(
      withStatus({
        ...app,
        declarations: [...app.declarations, decl],
        existingRelationships: app.existingRelationships.filter((e) => e.id !== existing.id),
      }),
    ),
  )
}


/** Mock-pay the liquor fee stream; government uses its own provider, no charge. */
export function payLiquor(id: string) {
  const app = collection.get(id)
  if (!app || app.liquorReceipt) return
  const fees = computeFees(app.declarations, app.jobNumber)
  if (fees.liquor.total <= 0) return
  const tx = createPayment({
    description: `Tied House — liquor (${app.jobNumber})`,
    subjectId: app.id,
    payerName: app.businessLegalName,
    lineItems: fees.liquor.lines.map((l) => ({ label: l.label, amount: l.amount })),
  })
  const paid = processPayment({ transactionId: tx.id, method: "Government payment provider", actor: app.businessLegalName })
  collection.upsert(touch(withStatus({ ...app, liquorReceipt: paid?.receiptNumber ?? tx.receiptNumber })))
}

/** Mock-pay the cannabis fee stream (separate from liquor). */
export function payCannabis(id: string) {
  const app = collection.get(id)
  if (!app || app.cannabisReceipt) return
  const fees = computeFees(app.declarations, app.jobNumber)
  if (fees.cannabis.total <= 0) return
  const tx = createPayment({
    description: `Tied House — cannabis (${app.jobNumber})`,
    subjectId: app.id,
    payerName: app.businessLegalName,
    lineItems: fees.cannabis.lines.map((l) => ({ label: l.label, amount: l.amount })),
  })
  const paid = processPayment({ transactionId: tx.id, method: "Government payment provider", actor: app.businessLegalName })
  collection.upsert(touch(withStatus({ ...app, cannabisReceipt: paid?.receiptNumber ?? tx.receiptNumber })))
}

/** Whether every owed stream has been paid (submission gate). */
export function canSubmit(app: TiedHouseApplication): boolean {
  const fees = computeFees(app.declarations, app.jobNumber)
  const hasSaved = app.declarations.some((d) => d.status === "saved")
  if (!hasSaved) return false
  const liquorOk = fees.liquor.total === 0 || Boolean(app.liquorReceipt)
  const cannabisOk = fees.cannabis.total === 0 || Boolean(app.cannabisReceipt)
  return liquorOk && cannabisOk
}

/** Submit the declarations to the Branch (pushes a case to Dynamics). */
export function submitToBranch(id: string): TiedHouseApplication | undefined {
  const app = collection.get(id)
  if (!app || app.submittedAt || !canSubmit(app)) return undefined

  // A declaration can pull more licences into the business group, so pre-screen against the build and tied house register, route to a queue and attach the agent log. Sync-only, no AI narrative.
  const profile = getBusinessProfile(app.businessLegalName)
  const tied = collectTiedHouseForBusiness(app.businessNumber)
  const { agentLog } = buildDeterministicAgentLog({
    title: "Tied House Declaration",
    applicantName: app.businessLegalName,
    ownedLicences: MOCK_LICENCES,
    leBuild: getBuildForBusiness(app.businessNumber),
    existingRelationships: tied.existingRelationships,
    declarations: tied.declarations,
    firstNation: profile.firstNation,
  })

  const sync = pushCase({
    reference: app.id,
    caseType: "Tied House Declaration",
    applicant: app.businessLegalName,
    payloadSummary: `${app.declarations.filter((d) => d.status === "saved").length} declaration(s)`,
    actor: app.businessLegalName,
    queue: agentLog.recommendedQueue,
    agentLog,
  })
  const next = touch({ ...app, status: "submitted", submittedAt: new Date().toISOString() })
  collection.upsert(next)
  recordAudit({
    category: "integration",
    action: "tied_house.submitted",
    summary: `Tied house declarations submitted to the Branch for ${app.businessLegalName}`,
    actor: app.businessLegalName,
    subjectId: app.id,
    meta: {
      dynamicsCaseId: sync.dynamicsCaseId ?? null,
      queue: agentLog.recommendedQueue,
      overallConfidence: agentLog.overallConfidence,
    },
  })
  return next
}

export function getApplication(id: string): TiedHouseApplication | undefined {
  return collection.get(id)
}


/** Reactive access to the Tied House application for a business + source. */
export function injectTiedHouseApplication(
  sourceArg: Arg<TiedHouseSource>,
  businessLegalNameArg: Arg<string>,
  businessNumberArg: Arg<string>,
): Signal<TiedHouseApplication | undefined> {
  const version = injectExternalStore(collection.subscribe, collection.snapshot)
  const hydrated = injectHydrated(() =>
    ensureApplication(read(sourceArg), read(businessLegalNameArg), read(businessNumberArg)),
  )
  // Re-ensure when the inputs change after hydration (the prototype's effect dependencies).
  effect(() => {
    const source = read(sourceArg)
    const name = read(businessLegalNameArg)
    const number = read(businessNumberArg)
    if (untracked(hydrated)) ensureApplication(source, name, number)
  })

  return computed(() => {
    version()
    const id = appIdFor(read(sourceArg), read(businessNumberArg))
    return hydrated() ? collection.get(id) : undefined
  })
}

// Re-export for convenience in components.
export { relationshipLabel }
