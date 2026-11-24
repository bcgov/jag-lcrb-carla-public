import type { Entity } from "./entity"

/** Kinds of nodes (cards) that can appear in a build. */
export type LEEntityType =
  | "applicant"
  | "private-corporation"
  | "public-corporation"
  | "partnership"
  | "limited-partnership"
  | "trust"
  | "society"
  | "sole-proprietor"
  | "individual"

/** Role a person/entity holds relative to the entity they are attached to. */
export type LERole =
  | "director"
  | "shareholder"
  | "key-personnel"
  | "partner"
  | "trustee"
  | "beneficiary"
  | "officer"

/** Relationship (edge) types drawn between two nodes. */
export type LERelationshipType =
  | "shareholding"
  | "partnership"
  | "trustee"
  | "director"
  | "key-personnel"
  | "family"
  | "business-agreement"
  | "dba"
  | "connection"

/** A person listed inside an entity card's Directors/Shareholders/Key Personnel. */
export interface LEPerson {
  id: string
  name: string
  role: LERole
  /** True when pulled from a record (name not editable in the portal). */
  locked?: boolean
}

/** A node/card in the build. */
export interface LENode {
  id: string
  type: LEEntityType
  /** Legal / registered name. Locked when sourced from a record. */
  legalName: string
  /** Business number (BN). Locked when sourced from a record. */
  businessNumber?: string
  /** Incorporation / registration number. Locked when sourced from a record. */
  incorporationNumber?: string
  jurisdiction?: string
  /** Licence numbers tied to this entity (from records; not editable). */
  linkedLicenceIds: string[]
  /** People attached to this entity. */
  people: LEPerson[]
  position: { x: number; y: number }
  /** Where the card came from: a system record (locked identity) or portal-added. */
  source: "record" | "portal"
  /** True when identity fields (name/number/licence) are read-only. */
  locked: boolean
  /** Optional licensee note / context for LCRB. */
  note?: string
  /** Set when this card is mirrored in read-only from another register (e.g. tied house declarations); never persisted into the build and cannot be edited, moved or removed. */
  external?: "tied-house"
}

/** A relationship between two nodes. */
export interface LEEdge {
  id: string
  /** Source node id (the owned/parent side). */
  source: string
  /** Target node id. */
  target: string
  type: LERelationshipType
  /** Ownership percentage for shareholding/partnership edges. */
  sharePercent?: number
  label?: string
  /** True when the relationship comes from a record and is not editable. */
  locked: boolean
  /** Set when this relationship is mirrored in read-only from another register (e.g. a tied house declaration); not part of the build. */
  external?: "tied-house"
}

export type LEBuildStatus =
  | "draft"
  | "submitted"
  | "under-review"
  | "changes-requested"
  | "approved"

/** A licensee-proposed change awaiting LCRB confirmation. */
export interface LESuggestion {
  id: string
  at: string
  actor: string
  summary: string
  kind: "add-node" | "update-node" | "add-relationship" | "remove" | "note"
}

/** An LCRB (Dynamics) review entry. */
export interface LEReviewNote {
  id: string
  at: string
  actor: string
  decision: "comment" | "approved" | "changes-requested"
  message: string
}

/** The whole build for a business (one per business account). */
export interface LegalEntityBuild extends Entity {
  businessLegalName: string
  businessNumber: string
  status: LEBuildStatus
  /** When true, LCRB owns the build in Dynamics and the portal is read-only. */
  lockedByLCRB: boolean
  version: number
  nodes: LENode[]
  edges: LEEdge[]
  suggestions: LESuggestion[]
  reviewNotes: LEReviewNote[]
  lastSyncedAt?: string
  dynamicsCaseId?: string
  updatedAt: string
}


export interface EntityMeta {
  label: string
  /** Short label for compact chips. */
  short: string
  /** Header colour (hex), a legend category colour accessible on white text. */
  color: string
  /** Whether this entity type can hold sub-people (directors etc.). */
  hasPeople: boolean
}
