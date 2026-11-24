export type UserRole = "licensee" | "applicant" | "police" | "local-government" | "lcrb"

export interface RoleConfig {
  id: UserRole
  label: string
  shortLabel: string
  description: string
  /** Landing route after login for this role */
  home: string
  /** The IAM/BCeID identity provider this role maps to. */
  idp: string
}

/** Role registry. TODO: resolve roles from BCeID / IAM token claims in production; `idp` is the sign-in method shown on the mock login screen (staff use IDIR SSO). */
export const ROLES: Record<UserRole, RoleConfig> = {
  licensee: {
    id: "licensee",
    label: "Licensee",
    shortLabel: "Licensee",
    description:
      "Business owners managing their liquor and cannabis licences, authorizations, and applications.",
    home: "/dashboard",
    idp: "BCeID (Business)",
  },
  applicant: {
    id: "applicant",
    label: "Special Event Permit Applicant",
    shortLabel: "SEP Applicant",
    description:
      "Individuals and organizations applying for and managing Special Event Permits for one-time events.",
    home: "/special-events",
    idp: "BCeID or BC Services Card",
  },
  police: {
    id: "police",
    label: "Police Agency",
    shortLabel: "Police",
    description:
      "Police users review Special Event Permit applications in their jurisdiction, view upcoming events, and approve or deny applications.",
    home: "/police",
    idp: "BCeID (Business)",
  },
  "local-government": {
    id: "local-government",
    label: "Local & Indigenous Government",
    shortLabel: "Local Government",
    description:
      "Local and Indigenous government reviewers provide comment and recommendations on licence applications referred to them.",
    home: "/local-government",
    idp: "BCeID (Business)",
  },
  lcrb: {
    id: "lcrb",
    label: "LCRB Staff",
    shortLabel: "LCRB Staff",
    description:
      "Licensing analysts and adjudicators who triage incoming cases synced from the portal into MS Dynamics and monitor intake performance.",
    home: "/lcrb",
    idp: "IDIR SSO",
  },
}

export const ROLE_LIST: RoleConfig[] = Object.values(ROLES)

export function getRole(id: string | null | undefined): RoleConfig | undefined {
  if (!id) return undefined
  return ROLES[id as UserRole]
}

/** Display name used to attribute reviewer actions; police get a rank-style prefix. TODO: use the verified display name from IAM claims. */
export function actorName(
  session: { role: UserRole; firstName: string; lastName: string } | null,
): string {
  if (!session) return "Unknown user"
  const full = `${session.firstName} ${session.lastName}`.trim()
  if (session.role === "police") return `Cst. ${full}`
  return full
}

/** True for the BCeID account owner (undefined counts as owner). TODO: re-check server-side before saving owner-only settings. */
export function isAccountOwner(
  session: { isAccountOwner?: boolean } | null,
): boolean {
  if (!session) return false
  return session.isAccountOwner !== false
}
