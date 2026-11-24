/** Licence Cap engine (deterministic). */

import type { LegalEntityBuild, LEEdge } from "@domain/models/legal-entity"
import { getLicenceById } from "./mock/licences"
import { type Licence } from "@domain/models/licences"
import { getAssociableLicence } from "../tied-house/mock/tied-house"
import { domainForLicenceType } from "@domain/tied-house/tied-house"
import { type ExistingRelationship, type TiedHouseDeclaration, type TiedHouseDomain } from "@domain/models/tied-house"
import { type CapStatus } from "@domain/models/licences"
import { CANNABIS_LICENCE_CAP } from "@domain/licences/licence-cap"
export const LICENCE_CAP = CANNABIS_LICENCE_CAP
/** At/above this count we warn the applicant/staff they are near the cap. */
export const NEAR_CAP_THRESHOLD = 7

/** Which domains actually carry a statutory cap. */
export const CAPPED_DOMAINS: readonly TiedHouseDomain[] = ["cannabis"]

export function isCappedDomain(domain: TiedHouseDomain): boolean {
  return CAPPED_DOMAINS.includes(domain)
}

/** Edge relationship types that indicate control / common ownership. */
const CONTROL_EDGE_TYPES: LEEdge["type"][] = [
  "shareholding",
  "partnership",
  "trustee",
  "director",
]

/** A licence pulled into the effective group, with why it counts. */
export interface CapContributingLicence {
  number: string
  establishmentName: string
  type: string
  domain: TiedHouseDomain
  /** How this licence entered the group. */
  source: "direct" | "legal-entity" | "tied-house"
  /** The party/holder it belongs to. */
  heldBy: string
  /** Certainty this licence truly belongs to the same business (0-100). */
  certainty: number
}

/** A business considered part of the same effective group. */
export interface GroupedParty {
  name: string
  basis: "applicant" | "common-ownership" | "tied-house-association"
  detail: string
}

export interface CapDomainResult {
  domain: TiedHouseDomain
  direct: number
  linked: number
  total: number
  /** `null` when the domain is not capped - display the count, not a ratio. */
  cap: number | null
  status: CapStatus
}

export interface LicenceCapResult {
  /** Liquor holdings in the effective group. */
  liquor: CapDomainResult
  /** Cannabis holdings, scored against CANNABIS_LICENCE_CAP. */
  cannabis: CapDomainResult
  groupedParties: GroupedParty[]
  contributingLicences: CapContributingLicence[]
  /** True when a capped domain is at/over cap OR linked parties create ambiguity. */
  requiresStaffReview: boolean
  /** One-line rationale for the agent log. */
  rationale: string
}

export interface CapEngineInput {
  /** The applicant business legal name. */
  applicantName: string
  /** Licences held directly by the applicant business. */
  ownedLicences: Licence[]
  /** The confirmed/proposed Legal Entity build (ownership graph), if any. */
  leBuild?: LegalEntityBuild | null
  /** Existing tied house relationships on file. */
  existingRelationships?: ExistingRelationship[]
  /** Saved tied house declarations in the current application. */
  declarations?: TiedHouseDeclaration[]
}

function statusFor(domain: TiedHouseDomain, total: number): CapStatus {
  if (!isCappedDomain(domain)) return "none"
  if (total > CANNABIS_LICENCE_CAP) return "over"
  if (total === CANNABIS_LICENCE_CAP) return "at"
  if (total >= NEAR_CAP_THRESHOLD) return "near"
  return "under"
}

/** Nodes connected to the applicant through control edges (any direction) */
function controlConnectedNodeIds(build: LegalEntityBuild): Set<string> {
  const applicant = build.nodes.find((n) => n.type === "applicant") ?? build.nodes[0]
  const connected = new Set<string>()
  if (!applicant) return connected
  connected.add(applicant.id)

  // Simple transitive closure over control edges.
  let changed = true
  while (changed) {
    changed = false
    for (const edge of build.edges) {
      if (!CONTROL_EDGE_TYPES.includes(edge.type)) continue
      const touchesGroup = connected.has(edge.source) || connected.has(edge.target)
      if (!touchesGroup) continue
      for (const id of [edge.source, edge.target]) {
        if (!connected.has(id)) {
          connected.add(id)
          changed = true
        }
      }
    }
  }
  return connected
}

/** Compute the provisional licence-cap picture for the applicant's effective business group across direct holdings */
export function computeLicenceCap(input: CapEngineInput): LicenceCapResult {
  const { applicantName, ownedLicences, leBuild, existingRelationships = [], declarations = [] } = input

  const contributing: CapContributingLicence[] = []
  const seen = new Set<string>()
  const groupedParties: GroupedParty[] = [
    { name: applicantName, basis: "applicant", detail: "Applicant business (direct holdings)." },
  ]

  // 1) Direct holdings - full certainty.
  for (const l of ownedLicences) {
    if (seen.has(l.id)) continue
    seen.add(l.id)
    contributing.push({
      number: l.id,
      establishmentName: l.name,
      type: l.type,
      domain: domainForLicenceType(l.type),
      source: "direct",
      heldBy: applicantName,
      certainty: 100,
    })
  }

  // 2) Common ownership via the LE build - high certainty (record-sourced).
  if (leBuild) {
    const connected = controlConnectedNodeIds(leBuild)
    for (const node of leBuild.nodes) {
      if (!connected.has(node.id)) continue
      if (node.type === "applicant") continue
      if (node.linkedLicenceIds.length === 0) continue
      let contributed = false
      for (const licId of node.linkedLicenceIds) {
        if (seen.has(licId)) continue
        const lic = getLicenceById(licId)
        if (!lic) continue
        seen.add(licId)
        contributed = true
        contributing.push({
          number: lic.id,
          establishmentName: lic.name,
          type: lic.type,
          domain: domainForLicenceType(lic.type),
          source: "legal-entity",
          heldBy: node.legalName,
          certainty: node.locked ? 90 : 70,
        })
      }
      if (contributed) {
        groupedParties.push({
          name: node.legalName,
          basis: "common-ownership",
          detail: "Connected through the legal entity build (common ownership/control).",
        })
      }
    }
  }

  // 3) Tied house associations - lower certainty ("potentially one and the same").
  const tiedLicenceRefs: { number: string; holder: string }[] = [
    ...existingRelationships.map((r) => ({ number: r.associatedLicenceNumber, holder: r.associatedEstablishmentName })),
    ...declarations
      .filter((d) => d.status === "saved" && d.changeType !== "remove")
      .map((d) => ({ number: d.associatedLicenceNumber, holder: d.associatedEstablishmentName })),
  ]
  for (const ref of tiedLicenceRefs) {
    if (!ref.number || seen.has(ref.number)) continue
    const assoc = getAssociableLicence(ref.number)
    const type = assoc?.type ?? "Liquor Primary"
    const domain = assoc?.domain ?? domainForLicenceType(type)
    seen.add(ref.number)
    contributing.push({
      number: ref.number,
      establishmentName: ref.holder || assoc?.establishmentName || ref.number,
      type,
      domain,
      source: "tied-house",
      heldBy: assoc?.operatedBy ?? ref.holder,
      certainty: 45,
    })
    if (assoc?.operatedBy) {
      groupedParties.push({
        name: assoc.operatedBy,
        basis: "tied-house-association",
        detail: "Linked through a tied house association — may be one-and-the-same, pending review.",
      })
    }
  }

  // Tally per domain.
  const tally = (domain: TiedHouseDomain): CapDomainResult => {
    const inDomain = contributing.filter((c) => c.domain === domain)
    const direct = inDomain.filter((c) => c.source === "direct").length
    const linked = inDomain.length - direct
    const total = inDomain.length
    const capped = isCappedDomain(domain)
    return {
      domain,
      direct,
      linked,
      total,
      cap: capped ? CANNABIS_LICENCE_CAP : null,
      status: statusFor(domain, total),
    }
  }

  const liquor = tally("liquor")
  const cannabis = tally("cannabis")

  // Only capped domains can trip the cap; liquor is listed for context.
  const atOrOver = cannabis.status === "at" || cannabis.status === "over"
  // Tied house ambiguity only matters for the cap when it touches a capped domain.
  const hasTiedHouseAmbiguity = contributing.some((c) => c.source === "tied-house" && isCappedDomain(c.domain))
  const requiresStaffReview = atOrOver || hasTiedHouseAmbiguity

  const linkedNote = (d: CapDomainResult) => `${d.direct} direct${d.linked ? `, +${d.linked} linked` : ""}`
  const parts: string[] = [
    `Cannabis ${cannabis.total}/${CANNABIS_LICENCE_CAP} (${linkedNote(cannabis)})`,
    `Liquor ${liquor.total} in group (${linkedNote(liquor)}; no cap applies)`,
  ]
  const rationale =
    `${parts.join("; ")}. ` +
    (atOrOver
      ? "At or over the cannabis cap — cap determination required."
      : hasTiedHouseAmbiguity
        ? "Tied house links may expand the effective business group for cannabis — staff confirmation recommended."
        : cannabis.total > 0
          ? "Under the cannabis cap on current connections."
          : "No cannabis licences in the group; the cap does not arise.")

  return { liquor, cannabis, groupedParties, contributingLicences: contributing, requiresStaffReview, rationale }
}
