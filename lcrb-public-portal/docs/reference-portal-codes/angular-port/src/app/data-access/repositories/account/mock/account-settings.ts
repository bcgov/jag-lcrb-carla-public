import type { UserRole } from "@domain/roles"
import { type NotificationTypeDef, type AccountConfig } from "@domain/models/account"

/** Shared "news and updates" toggle offered to every role. */
const NEWS_UPDATES: NotificationTypeDef = {
  id: "newsUpdates",
  label: "News & Updates",
  description: "Policy changes, new features, and LCRB announcements",
  defaultOn: false,
}

const ACCOUNT_CONFIG: Record<UserRole, AccountConfig> = {
  licensee: {
    description: "Manage your profile, business details, delegated users, and notification preferences",
    home: { label: "Dashboard", href: "/dashboard" },
    identity: {
      cardTitle: "BCeID Account",
      cardDescription: "Your BCeID Business credentials and login information",
      profileDescription: "Your personal details linked to your BCeID account",
      usernameLabel: "BCeID Username",
      username: "harding_hospitality",
      manageLabel: "Manage BCeID",
    },
    org: {
      label: "Business",
      icon: "building",
      cardTitle: "Business Details",
      cardDescription: "Your registered business information",
      sourceBadge: "Linked from BCeID",
      fields: [
        { label: "Legal Business Name", value: "Harding Hospitality Group", isOrgName: true },
        { label: "Business Type", value: "Private Corporation" },
        { label: "CRA Business Number", value: "123456789" },
        { label: "Incorporation Number", value: "BC1234567" },
      ],
      footnote:
        "Business details are linked from your BCeID account. To make changes, update your information with BC Registries or contact BCeID support.",
      externalLink: { label: "Go to BC Registries", href: "https://www.bcregistry.gov.bc.ca/" },
      addresses: [
        { label: "Mailing Address", value: "1125 Douglas St, Victoria, BC V8W 2E7" },
        { label: "Physical Address", value: "1125 Douglas St, Victoria, BC V8W 2E7" },
      ],
    },
    notificationTypes: [
      {
        id: "renewalReminders",
        label: "Licence Renewal Reminders",
        description: "Get notified 90, 60, and 30 days before licence expiry",
        defaultOn: true,
      },
      {
        id: "applicationUpdates",
        label: "Application Updates",
        description: "Status changes, document requests, and approval notifications",
        defaultOn: true,
      },
      {
        id: "complianceNotices",
        label: "Compliance & Enforcement",
        description: "Inspection results, contraventions, and enforcement notices",
        defaultOn: true,
      },
      NEWS_UPDATES,
    ],
    delegates: {
      label: "Delegates",
      cardTitle: "Delegate Access",
      cardDescription: "Manage who can access your account and what they can do",
      addButtonLabel: "Add Delegate",
      addDialogTitle: "Add Delegate",
      addDialogDescription:
        "Invite someone to access your business account. They'll appear as pending until they accept.",
      note: "Delegates sign in with their own BCeID and act on behalf of your business. You remain responsible for everything submitted under this account.",
    },
    showCommenting: false,
  },

  applicant: {
    description: "Manage your profile, organization details, and notification preferences",
    home: { label: "My Special Event Permits", href: "/special-events" },
    identity: {
      cardTitle: "Sign-in Account",
      cardDescription: "The BCeID or BC Services Card you use to sign in",
      profileDescription: "Your personal details linked to your sign-in account",
      usernameLabel: "Sign-in Username",
      username: "community_events_society",
      manageLabel: "Manage sign-in account",
    },
    org: {
      label: "Organization",
      icon: "briefcase",
      cardTitle: "Organization Details",
      cardDescription: "The organization you apply for permits on behalf of",
      sourceBadge: "Linked from BCeID",
      fields: [
        { label: "Organization Name", value: "Community Events Society", isOrgName: true },
        { label: "Organization Type", value: "Non-profit Society" },
        { label: "Society Registration Number", value: "S0071234" },
        { label: "Primary Contact", value: "Alex Rivera" },
      ],
      footnote:
        "Applying as an individual instead? You can leave organization details blank — they are only used to pre-fill permit applications.",
      addresses: [{ label: "Mailing Address", value: "310 Water St, Vancouver, BC V6B 1B6" }],
    },
    notificationTypes: [
      {
        id: "applicationUpdates",
        label: "Permit Application Updates",
        description: "Status changes, document requests, and approval notifications",
        defaultOn: true,
      },
      {
        id: "eventReminders",
        label: "Event Reminders",
        description: "Reminders before your event date and permit conditions to review",
        defaultOn: true,
      },
      NEWS_UPDATES,
    ],
    delegates: {
      label: "Delegates",
      cardTitle: "Delegate Access",
      cardDescription: "Let others in your organization apply for and manage permits",
      addButtonLabel: "Add Delegate",
      addDialogTitle: "Add Delegate",
      addDialogDescription:
        "Invite someone from your organization to manage permits. They'll appear as pending until they accept.",
      note: "Delegates sign in with their own credentials. You remain responsible for permits submitted under this account.",
    },
    showCommenting: false,
  },

  police: {
    description: "Manage your profile, agency details, agency users, and notification preferences",
    home: { label: "Dashboard", href: "/police" },
    identity: {
      cardTitle: "BCeID Account",
      cardDescription: "Your BCeID Business credentials and login information",
      profileDescription: "Your personal details as recorded for your agency",
      usernameLabel: "BCeID Username",
      username: "vpd_licensing",
      manageLabel: "Manage BCeID",
    },
    org: {
      label: "Agency",
      icon: "shield",
      cardTitle: "Agency Details",
      cardDescription: "The police agency you review applications for",
      sourceBadge: "Maintained by LCRB",
      fields: [
        { label: "Agency Name", value: "Vancouver Police Department", isOrgName: true },
        { label: "Agency Type", value: "Municipal Police Department" },
        { label: "Detachment / Division", value: "Licensing & Liquor Unit" },
        { label: "Jurisdiction", value: "City of Vancouver" },
      ],
      footnote:
        "Agency details and the jurisdictions routed to you are maintained by the LCRB. Contact your LCRB liaison to request a change.",
      addresses: [{ label: "Office Address", value: "2120 Cambie St, Vancouver, BC V5Z 4N6" }],
    },
    notificationTypes: [
      {
        id: "newReferrals",
        label: "New Applications for Review",
        description: "A Special Event Permit in your jurisdiction has been referred to you",
        defaultOn: true,
      },
      {
        id: "deadlineReminders",
        label: "Review Deadline Reminders",
        description: "Reminders before a referral's response deadline passes",
        defaultOn: true,
      },
      {
        id: "urgentEvents",
        label: "Short-notice Events",
        description: "Events submitted with less than 14 days' notice",
        defaultOn: true,
      },
      NEWS_UPDATES,
    ],
    delegates: {
      label: "Agency Users",
      cardTitle: "Agency User Access",
      cardDescription: "Manage which officers and staff can review referrals for your agency",
      addButtonLabel: "Add User",
      addDialogTitle: "Add Agency User",
      addDialogDescription:
        "Invite an officer or staff member to review referrals. They'll appear as pending until they accept.",
      note: "Agency users sign in with their own BCeID. All review decisions are recorded against the individual who made them.",
    },
    showCommenting: false,
    showSepAutoApproval: true,
  },

  "local-government": {
    description:
      "Manage your profile, jurisdiction details, delegated users, and how your jurisdiction takes part in commenting",
    home: { label: "Local Government Review", href: "/local-government" },
    identity: {
      cardTitle: "BCeID Account",
      cardDescription: "Your BCeID Business credentials and login information",
      profileDescription: "Your personal details as recorded for your jurisdiction",
      usernameLabel: "BCeID Username",
      username: "kelowna_licensing",
      manageLabel: "Manage BCeID",
    },
    org: {
      label: "Jurisdiction",
      icon: "landmark",
      cardTitle: "Jurisdiction Details",
      cardDescription: "The local government or Indigenous Nation you comment on behalf of",
      sourceBadge: "Maintained by LCRB",
      fields: [
        { label: "Jurisdiction Name", value: "City of Kelowna", isOrgName: true },
        { label: "Jurisdiction Type", value: "Municipality" },
        { label: "Region", value: "Thompson-Okanagan" },
        { label: "Referral Contact", value: "Licensing & Bylaw Services" },
      ],
      footnote:
        "Jurisdiction boundaries and the applications routed to you are maintained by the LCRB. Contact your LCRB liaison to request a change.",
      addresses: [{ label: "Office Address", value: "1435 Water St, Kelowna, BC V1Y 1J4" }],
    },
    notificationTypes: [
      {
        id: "newReferrals",
        label: "New Applications for Comment",
        description: "An application in your jurisdiction has been referred to you for comment",
        defaultOn: true,
      },
      {
        id: "deadlineReminders",
        label: "Comment Deadline Reminders",
        description: "Reminders before an application's comment period closes",
        defaultOn: true,
      },
      {
        id: "decisionOutcomes",
        label: "Decision Outcomes",
        description: "How the LCRB decided on applications you commented on",
        defaultOn: false,
      },
      NEWS_UPDATES,
    ],
    delegates: {
      label: "Delegates",
      cardTitle: "Delegated User Access",
      cardDescription: "Manage which staff can review and comment on applications",
      addButtonLabel: "Add Delegate",
      addDialogTitle: "Add Delegated User",
      addDialogDescription:
        "Invite a staff member to review and comment on referrals. They'll appear as pending until they accept.",
      note: "Delegated users sign in with their own BCeID. Only the account owner can change jurisdiction-wide settings such as the commenting opt-out.",
    },
    showCommenting: true,
  },

  lcrb: {
    description: "Manage your profile and notification preferences",
    home: { label: "Intake Overview", href: "/lcrb" },
    identity: {
      cardTitle: "IDIR Account",
      cardDescription: "Your BC Government IDIR credentials and login information",
      profileDescription: "Your personal details from your IDIR account",
      usernameLabel: "IDIR Username",
      username: "MLEE",
      manageLabel: "Manage IDIR account",
    },
    org: {
      label: "Branch",
      icon: "building",
      cardTitle: "Branch & Role",
      cardDescription: "Your assignment within the Liquor & Cannabis Regulation Branch",
      sourceBadge: "Maintained by IDIR",
      fields: [
        { label: "Branch", value: "Liquor & Cannabis Regulation Branch", isOrgName: true },
        { label: "Team", value: "Licensing & Intake" },
        { label: "Position", value: "Licensing Analyst" },
        { label: "Employee ID", value: "BC-40218" },
      ],
      footnote:
        "Branch, team, and position are managed in IDIR and the Dynamics security model. Contact your supervisor or the service desk to request a change.",
      addresses: [{ label: "Office Address", value: "400 - 645 Tyee Rd, Victoria, BC V9A 6X5" }],
    },
    notificationTypes: [
      {
        id: "caseAssignments",
        label: "Case Assignments",
        description: "A case has been assigned to you or your queue in Dynamics",
        defaultOn: true,
      },
      {
        id: "slaAlerts",
        label: "Service Standard Alerts",
        description: "Cases approaching or exceeding their service standard",
        defaultOn: true,
      },
      {
        id: "partnerResponses",
        label: "Partner Responses",
        description: "Police and local government comments received on your cases",
        defaultOn: true,
      },
      NEWS_UPDATES,
    ],
    // LCRB staff accounts are provisioned centrally, so delegation is not offered.
    delegates: null,
    delegatesUnavailableNote:
      "LCRB staff accounts are provisioned through IDIR and the Dynamics security model, so delegate access is not managed here. Contact the service desk to change someone's access.",
    showCommenting: false,
  },
}

export function getAccountConfig(role: UserRole): AccountConfig {
  return ACCOUNT_CONFIG[role]
}

/** Default notification-type state for a role. */
export function defaultNotificationState(role: UserRole): Record<string, boolean> {
  return Object.fromEntries(
    getAccountConfig(role).notificationTypes.map((t) => [t.id, t.defaultOn]),
  )
}
