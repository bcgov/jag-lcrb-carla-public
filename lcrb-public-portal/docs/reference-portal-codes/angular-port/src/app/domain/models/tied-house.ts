import type { Entity } from "./entity"


/** The two shapes a declaration can take. The declaration box reshapes on this. */
export type TiedHouseType = "individual" | "legal-entity"

/** Where the declaration is being made from (drives copy + fee context). */
export type TiedHouseSource = "le-review" | "voluntary" | "new-application"

/** Liquor and cannabis fees are calculated and PAID SEPARATELY. */
export type TiedHouseDomain = "liquor" | "cannabis"

/** The kind of change a declaration represents (drives the fee rate). */
export type TiedHouseChangeType = "add-individual" | "add-legal-entity" | "change-details" | "remove"

/** Lifecycle of a single declaration within an application. */
export type DeclarationStatus = "draft" | "saved"

/** Lifecycle of the whole Tied House application. */
export type TiedHouseAppStatus = "draft" | "awaiting-payment" | "submitted"


export interface Option {
  value: string
  label: string
}


export interface AssociableLicence {
  /** 6-digit licence number. */
  number: string
  /** Establishment / trading name. */
  establishmentName: string
  /** Licence type label, e.g. "Liquor Primary". */
  type: string
  /** Which fee stream this associates with. */
  domain: TiedHouseDomain
  /** Business that holds the licence (context only). */
  operatedBy: string
}


export interface RequiredDoc {
  id: string
  label: string
  description: string
}


export interface IndividualDetails {
  dateOfBirth: string
  firstName: string
  middleName?: string
  lastName: string
  /** Value from INDIVIDUAL_RELATIONSHIPS. */
  relationship: string
  /** Free text when relationship === "other". */
  relationshipOther?: string
}

export interface LegalEntityDetails {
  businessType: string
  legalEntityName: string
  /** Value from LEGAL_ENTITY_RELATIONSHIPS. */
  relationshipType: string
  relationshipOther?: string
}

export interface TiedHouseDeclaration {
  id: string
  type: TiedHouseType
  status: DeclarationStatus
  /** The change this declaration represents (drives fee). */
  changeType: TiedHouseChangeType
  /** Present for individual declarations. */
  individual?: IndividualDetails
  /** Present for legal-entity declarations. */
  entity?: LegalEntityDetails
  /** Associated licence number (6 digits). */
  associatedLicenceNumber: string
  /** Denormalised for display. */
  associatedEstablishmentName: string
  domain: TiedHouseDomain
  /** Documents this addition requires (computed on save). */
  requiredDocs: RequiredDoc[]
  /** When the connection was consented to / "date tied". */
  dateTied?: string
  createdAt: string
}

/** A pre-existing tied house relationship on the licensee's file. */
export interface ExistingRelationship {
  id: string
  type: TiedHouseType
  /** Display label: `Name - DOB` for individuals, legal name for entities. */
  displayName: string
  relationshipLabel: string
  associatedLicenceNumber: string
  associatedEstablishmentName: string
  domain: TiedHouseDomain
  dateTied: string
}

export interface TiedHouseApplication extends Entity {
  businessLegalName: string
  businessNumber: string
  source: TiedHouseSource
  /** Reference shown on the fee panel, e.g. "Legal Entity Review Job No. 123456". */
  jobNumber: string
  status: TiedHouseAppStatus
  declarations: TiedHouseDeclaration[]
  existingRelationships: ExistingRelationship[]
  /** Mock payment receipt numbers, once each stream is paid. */
  liquorReceipt?: string
  cannabisReceipt?: string
  submittedAt?: string
  createdAt: string
  updatedAt: string
}

export interface FeeLine {
  declarationId: string
  label: string
  licencesImpacted: number
  rate: number
  amount: number
  domain: TiedHouseDomain
}

export interface FeeBreakdown {
  liquor: { lines: FeeLine[]; total: number }
  cannabis: { lines: FeeLine[]; total: number }
  grandTotal: number
}
