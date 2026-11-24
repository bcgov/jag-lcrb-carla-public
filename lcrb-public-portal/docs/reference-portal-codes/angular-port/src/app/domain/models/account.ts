import { type FirstNationProfile } from "./section-119"

/** Account Settings config per role. TODO: values are fixtures; load organization details from BCeID, BC Registries or IDIR. */

/** Which icon the Organization tab should render. Mapped to lucide in the page. */
export type OrgIcon = "building" | "landmark" | "shield" | "briefcase"

export interface OrgField {
  label: string
  /** Fixture value. The primary/name field is overridden by the session org. */
  value: string
  /** Marks the field that should display the signed-in user's organization. */
  isOrgName?: boolean
}

export interface OrgTabConfig {
  /** Tab label, e.g. "Business", "Jurisdiction", "Agency". */
  label: string
  icon: OrgIcon
  cardTitle: string
  cardDescription: string
  /** Small badge showing where the record is mastered. */
  sourceBadge: string
  fields: OrgField[]
  /** Explains where to go to change these details. */
  footnote: string
  /** Optional external link shown under the footnote. */
  externalLink?: { label: string; href: string }
  /** Address block labels; omitted when the role has no service address. */
  addresses?: { label: string; value: string }[]
}

export interface NotificationTypeDef {
  id: string
  label: string
  description: string
  defaultOn: boolean
}

export interface DelegatesConfig {
  /** Tab label, e.g. "Delegates", "Agency Users". */
  label: string
  cardTitle: string
  cardDescription: string
  addButtonLabel: string
  addDialogTitle: string
  addDialogDescription: string
  /** Copy shown above the table explaining the trust model. */
  note: string
}

export interface IdentityConfig {
  /** Card title, e.g. "BCeID Account", "IDIR Account". */
  cardTitle: string
  cardDescription: string
  /** Description for the "Personal Information" card above it. */
  profileDescription: string
  usernameLabel: string
  username: string
  /** Label for the external "manage credentials" link. */
  manageLabel: string
}

export interface AccountConfig {
  /** Sub-heading under the page title. */
  description: string
  /** Breadcrumb back to this role's landing page. */
  home: { label: string; href: string }
  identity: IdentityConfig
  /** Null when the role has no organization record to show. */
  org: OrgTabConfig | null
  notificationTypes: NotificationTypeDef[]
  /** Null when delegation does not apply; LCRB staff accounts are provisioned through IDIR and cannot invite delegates. */
  delegates: DelegatesConfig | null
  /** Explains why the Delegates tab is absent, shown on the Profile tab. */
  delegatesUnavailableNote?: string
  /** Local Government / Indigenous Nation commenting opt-out. */
  showCommenting: boolean
  /** Police agency SEP auto-approval policy. */
  showSepAutoApproval?: boolean
}

export interface BusinessLicence {
  /** Licence number (the value stored on the form). */
  number: string
  /** Establishment / trading name tied to the licence. */
  establishmentName: string
  /** Licence type label, e.g. "Liquor Primary". */
  type: string
  /** Establishment address, for display. */
  address: string
}

export interface BusinessProfile {
  /** BCeID business legal name; auto-filled, not editable. */
  legalName: string
  /** BCeID business number; auto-filled, not editable. */
  bceidBusinessNumber: string
  /** Canonical legal-entity type from BC Registries (read-only); each form maps it onto its own options. */
  legalEntityType: string
  /** Incorporation / registration number from BC Registries, auto-filled; empty for unincorporated types such as a sole proprietorship. */
  incorporationNumber: string
  /** Every licence/establishment held under this business account. */
  licences: BusinessLicence[]
  /** First Nations affiliation + s.119 agreement status, if on file. */
  firstNation?: FirstNationProfile
}

export interface LicenceHoldings {
  /** Count of liquor licences held under the business account. */
  liquor: number
  /** Count of cannabis licences held under the business account. */
  cannabis: number
}
