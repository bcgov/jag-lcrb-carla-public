"use client"

/** Joins legal entity and tied house data per business; reads the kv-store collections by name to avoid import cycles. TODO: read from the real API, keep the return shapes. */

import { createCollection } from "@data-access/repositories/mock-storage/kv-store"
import type { LegalEntityBuild, LENode, LEEdge, LERelationshipType } from "@domain/models/legal-entity"
import { relationshipLabel } from "@domain/tied-house/tied-house"
import type { TiedHouseApplication, TiedHouseDeclaration, ExistingRelationship } from "@domain/models/tied-house"

// Same names the domain stores use; shared underlying storage.
const leBuilds = createCollection<LegalEntityBuild>("legal-entity-builds")
const tiedHouseApps = createCollection<TiedHouseApplication>("tied-house-applications")

/** The tied-house collection, exposed so the Legal Entity builder can react to declaration changes and mirror them live. */
export const tiedHouseCollection = tiedHouseApps

/** Read-only access to a business's LE build (does not seed a sample). */
export function getBuildForBusiness(businessNumber: string): LegalEntityBuild | undefined {
  return leBuilds.get(`build_${businessNumber}`)
}

/** Aggregates a business's tied-house associations across all sources, de-duplicated by associated licence number. */
export function collectTiedHouseForBusiness(businessNumber: string): {
  existingRelationships: ExistingRelationship[]
  declarations: TiedHouseDeclaration[]
} {
  const apps = tiedHouseApps.list().filter((a) => a.businessNumber === businessNumber)
  const relByNumber = new Map<string, ExistingRelationship>()
  const declByNumber = new Map<string, TiedHouseDeclaration>()
  for (const app of apps) {
    for (const r of app.existingRelationships) {
      if (r.associatedLicenceNumber) relByNumber.set(r.associatedLicenceNumber, r)
    }
    for (const d of app.declarations) {
      if (d.status === "saved" && d.changeType !== "remove" && d.associatedLicenceNumber) {
        declByNumber.set(d.associatedLicenceNumber, d)
      }
    }
  }
  return {
    existingRelationships: [...relByNumber.values()],
    declarations: [...declByNumber.values()],
  }
}

export interface LinkContext {
  leBuild: LegalEntityBuild | null
  existingRelationships: ExistingRelationship[]
  declarations: TiedHouseDeclaration[]
}

/** Best-effort map of a tied-house relationship label to an LE edge type (for colour). */
function toLERelType(label: string): LERelationshipType {
  const l = label.toLowerCase()
  if (l.includes("sharehold")) return "shareholding"
  if (l.includes("director")) return "director"
  if (l.includes("partner")) return "partnership"
  if (l.includes("trustee")) return "trustee"
  if (["spouse", "parent", "sibling", "child", "family"].some((k) => l.includes(k))) return "family"
  return "business-agreement"
}

export interface TiedHouseOverlay {
  nodes: LENode[]
  edges: LEEdge[]
}

/** Mirrors tied-house associations into read-only legal entity overlay nodes/edges (`external: "tied-house"`, `locked: true`); never persisted into the build. */
export function deriveTiedHouseOverlay(
  businessNumber: string,
  applicantNodeId: string,
): TiedHouseOverlay {
  const { existingRelationships, declarations } = collectTiedHouseForBusiness(businessNumber)

  // Collapse both sources onto the associated licence number so a relationship declared twice shows as one card.
  const byLicence = new Map<
    string,
    { establishment: string; type: "individual" | "legal-entity"; label: string; domain: string }
  >()

  for (const r of existingRelationships) {
    if (!r.associatedLicenceNumber) continue
    byLicence.set(r.associatedLicenceNumber, {
      establishment: r.associatedEstablishmentName || `Licence ${r.associatedLicenceNumber}`,
      type: r.type,
      label: r.relationshipLabel,
      domain: r.domain,
    })
  }
  for (const d of declarations) {
    if (!d.associatedLicenceNumber) continue
    const label =
      d.type === "individual"
        ? relationshipLabel("individual", d.individual?.relationship ?? "")
        : relationshipLabel("legal-entity", d.entity?.relationshipType ?? "")
    byLicence.set(d.associatedLicenceNumber, {
      establishment: d.associatedEstablishmentName || `Licence ${d.associatedLicenceNumber}`,
      type: d.type,
      label,
      domain: d.domain,
    })
  }

  const nodes: LENode[] = []
  const edges: LEEdge[] = []
  let i = 0
  for (const [licence, info] of byLicence) {
    const nodeId = `th_overlay_${licence}`
    nodes.push({
      id: nodeId,
      type: info.type === "individual" ? "individual" : "private-corporation",
      legalName: info.establishment,
      linkedLicenceIds: [],
      people: [],
      // A distinct band below/around the build; fitView keeps it in frame.
      position: { x: 120 + (i % 4) * 320, y: 1040 + Math.floor(i / 4) * 220 },
      source: "record",
      locked: true,
      external: "tied-house",
      note: `Associated licence ${licence} (${info.domain}). Declared in Tied House.`,
    })
    edges.push({
      id: `th_overlay_edge_${licence}`,
      source: applicantNodeId,
      target: nodeId,
      type: toLERelType(info.label),
      label: `Tied House · ${info.label}`,
      locked: true,
      external: "tied-house",
    })
    i++
  }

  return { nodes, edges }
}

/** One call to gather every graph the cap engine + agent need for a business. */
export function gatherLinkContext(businessNumber: string): LinkContext {
  const tied = collectTiedHouseForBusiness(businessNumber)
  return {
    leBuild: getBuildForBusiness(businessNumber) ?? null,
    existingRelationships: tied.existingRelationships,
    declarations: tied.declarations,
  }
}
