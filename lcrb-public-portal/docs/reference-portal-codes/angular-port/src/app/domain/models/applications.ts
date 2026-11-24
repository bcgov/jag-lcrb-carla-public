/** Application catalogue model: licence types, catalogue entries and their labels. */
import type { FlowConfig } from "../forms/flow-config"
import type { FlowStep } from "@domain/forms/flow-config"

export type LicenceType =
  | "common"
  | "food-primary"
  | "liquor-primary"
  | "cannabis-retail-store"
  | "licensee-retail-store"
  | "manufacturer"
  | "deemed-manufacturer"
  | "agent"
  | "catering"
  | "ubrew-uvin"

export interface CatalogEntry {
  slug: string
  title: string
  description: string
  licenceType: LicenceType
  kind: "application" | "authorization"
  fee?: string
  config: FlowConfig
  /** Extra search terms (synonyms and plain words like "patio", "renew") missing from the official title; title-keyed defaults live in `application-search.ts`. */
  keywords?: string[]
}

export const LICENCE_TYPE_LABELS: Record<LicenceType, string> = {
  common: "Common Authorizations",
  "food-primary": "Food Primary",
  "liquor-primary": "Liquor Primary",
  "cannabis-retail-store": "Cannabis Retail Store",
  "licensee-retail-store": "Licensee Retail Store",
  manufacturer: "Manufacturer",
  "deemed-manufacturer": "Deemed Manufacturer",
  agent: "Agent",
  catering: "Catering",
  "ubrew-uvin": "UBrew & UVin",
}

export const LICENCE_TYPE_ORDER: LicenceType[] = [
  "common",
  "food-primary",
  "liquor-primary",
  "cannabis-retail-store",
  "licensee-retail-store",
  "manufacturer",
  "deemed-manufacturer",
  "agent",
  "catering",
  "ubrew-uvin",
]

/** A supporting document definition, including the criteria the AI completeness check validates an upload against. */
export interface DocumentSpec {
  name: string
  label: string
  required?: boolean
  hint?: string
  /** Checkable requirements the uploaded document must satisfy. */
  criteria?: string[]
  /** Document-intelligence hint driving extraction and cross-checks: floor-plan, director-info, incorporation, lease, lg-approval, certificate; defaults to "generic" (completeness only). */
  docType?: "floor-plan" | "director-info" | "incorporation" | "lease" | "lg-approval" | "certificate" | "generic"
  /** For floor plans: add a companion occupancy-load field auto-filled from the stamp reading. */
  occupancyField?: boolean
}


export interface FlowSpec {
  slug: string
  title: string
  description: string
  licenceType: LicenceType
  kind?: "application" | "authorization"
  fee?: string
  /** Intro shown on the Before Starting step. Omit to skip that step. */
  intro?: string
  /** Include the standard establishment + location step (default true). */
  establishment?: boolean
  /** The application-specific middle steps. */
  steps: FlowStep[]
  /** Supporting document uploads (emits a Documents step when present). */
  documents?: DocumentSpec[]
  /** Include the standard contact step (default true). */
  contact?: boolean
  /** Nav item to highlight (default "/applications"). */
  currentNav?: string
  /** Return path after submit/cancel (default "/applications"). */
  returnTo?: string
}

/** Per-licence-type colour accents for the "Start a new application" catalog (card border, icon chip, heading dot); class strings are written in full so Tailwind's JIT finds them. */
export interface LicenceTypeAccent {
  /** Left-border accent on each card (used with `border-l-4`). */
  bar: string
  /** Icon chip background + foreground. */
  chip: string
  /** Small dot shown beside the group heading. */
  dot: string
}

/** Glossary for licensing terms, referenced by key from field `help`; paraphrases LCRB policy, not legal text. */

export interface GlossaryEntry {
  term: string
  /** One or two plain sentences; what the applicant needs in order to answer. */
  definition: string
  /** Optional concrete example to make the abstraction land. */
  example?: string
}

/** Submitted application details. TODO: replace with the submission API; `requiredDocuments` maps to Dynamics information requests. */

export interface SubmittedSummaryField {
  label: string
  value: string
}

export interface SubmittedSummarySection {
  heading: string
  fields: SubmittedSummaryField[]
}

export interface RequiredDocument {
  /** Stable id, used to scope uploads to this request. */
  id: string
  name: string
  /** Why LCRB is asking for it. */
  reason: string
  /** Whether the applicant must provide it before review can continue. */
  required: boolean
}

export interface SubmittedTimelineEntry {
  label: string
  date: string
  state: "done" | "current" | "upcoming"
}

export type SubmittedStatus = "under_review" | "pending_documents" | "approved" | "cancelled"

export interface SubmittedApplication {
  id: string
  type: string
  reference: string
  status: SubmittedStatus
  applicant: string
  submittedDate: string
  expectedCompletion: string
  /** Free-form note from LCRB shown at the top when documents are pending. */
  reviewNote?: string
  fee: { label: string; amount: number }[]
  currency: string
  paymentMethod: string
  confirmationNumber: string
  summary: SubmittedSummarySection[]
  timeline: SubmittedTimelineEntry[]
  requiredDocuments: RequiredDocument[]
}

/** The customer's applications, used by My Applications and the assistant. TODO: load from the licensing system for the BCeID business. */

export type MyApplicationStatus =
  | "not_submitted"
  | "under_review"
  | "pending_documents"
  | "approved"
  | "rejected"
  | "cancelled"

export interface MyApplicationSection {
  name: string
  status: string
}

export interface MyApplicationCapture {
  label: string
  value: string
}

interface MyApplicationBase {
  id: string
  type: string
  reference: string
  status: MyApplicationStatus
}

export interface InProgressApplication extends MyApplicationBase {
  lastModified: string
  completionPercent: number
  sections: MyApplicationSection[]
  captured: MyApplicationCapture[]
}

export interface SubmittedApplicationEntry extends MyApplicationBase {
  submittedDate: string
  expectedCompletion: string
}

export interface CompletedApplicationEntry extends MyApplicationBase {
  completedDate: string
}

/** Union of every stage, for flattening across stages. */
export type MyApplication = InProgressApplication | SubmittedApplicationEntry | CompletedApplicationEntry

export interface MyApplications {
  inProgress: InProgressApplication[]
  submitted: SubmittedApplicationEntry[]
  completed: CompletedApplicationEntry[]
}

export type EstablishmentDomain = "liquor" | "cannabis"

/** The kinds of establishment an applicant can plan for. */
export type EstablishmentTypeId =
  | "restaurant"
  | "bar"
  | "nightclub"
  | "private-club"
  | "manufacturer"
  | "catering"
  | "rural-store"
  | "cannabis-store"

export interface EstablishmentTypeDef {
  id: EstablishmentTypeId
  label: string
  blurb: string
  domain: EstablishmentDomain
  licenceType: LicenceType
  /** Real catalog slug for the "apply for a new licence" flow, when one exists. */
  primarySlug?: string
  /** Representative base application fee (may be an estimate). */
  baseFee: string
  baseFeeIsEstimate: boolean
  /** Optional note about recurring/annual fees. */
  annualNote?: string
  /** Provisional processing-time band in weeks. */
  minWeeks: number
  maxWeeks: number
  /** Which optional add-ons are commonly relevant to this establishment. */
  relevantAddOns: AddOnId[]
}

export type AddOnId = "patio" | "entertainment" | "extended-hours" | "catered-event" | "structural"

export interface AddOnDef {
  id: AddOnId
  label: string
  /** Catalog title keywords used to find the matching real application entry. */
  match: RegExp
}
