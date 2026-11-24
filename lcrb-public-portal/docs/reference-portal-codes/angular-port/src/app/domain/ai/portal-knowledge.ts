/** Static knowledge base for the prototype assistant; in production this comes from the Azure AI Foundry stack (RAG over secure indexes, Portal Navigator tool). */

export type PortalRole =
  | "applicant"
  | "licensee"
  | "police"
  | "local-government"
  | "lcrb-staff"
  | "public"

/** Known navigation destinations the assistant can route users to. */
export interface NavDestination {
  id: string
  label: string
  href: string
  description: string
  /** Roles this destination is relevant to (empty = everyone). */
  roles?: PortalRole[]
}

export const NAV_DESTINATIONS: NavDestination[] = [
  {
    id: "dashboard",
    label: "Dashboard",
    href: "/dashboard",
    description: "Your home page with licences, tasks, and alerts.",
    roles: ["applicant", "licensee"],
  },
  {
    id: "applications",
    label: "My Applications",
    href: "/applications",
    description: "Track your in-progress, submitted, and completed applications.",
    roles: ["applicant", "licensee"],
  },
  {
    id: "applications-new",
    label: "Start a new application",
    href: "/applications/new",
    description: "Browse every liquor and cannabis application type and start a new one.",
    roles: ["applicant", "licensee"],
  },
  {
    id: "licence-finder",
    label: "Find the right licence",
    href: "/licence-finder",
    description: "Answer a few questions to find out which licence or permit you need.",
    roles: ["applicant", "licensee"],
  },
  {
    id: "special-events",
    label: "Special Event Permits (SEP)",
    href: "/special-events",
    description: "Apply for and track Special Event Permits for serving liquor at an event.",
    roles: ["applicant", "licensee"],
  },
  {
    id: "sep-apply",
    label: "Start a Special Event Permit application",
    href: "/special-events/apply",
    description: "The step-by-step SEP application wizard.",
    roles: ["applicant", "licensee"],
  },
  {
    id: "licences",
    label: "Licences & Authorizations",
    href: "/licences",
    description: "View and manage your existing liquor and cannabis licences.",
    roles: ["applicant", "licensee"],
  },
  {
    id: "security-screening",
    label: "Security Screening",
    href: "/security-screening",
    description: "Complete and track security screening for associates.",
    roles: ["applicant", "licensee"],
  },
  {
    id: "notices",
    label: "Notices",
    href: "/notices",
    description: "Read official notices and messages from the LCRB.",
    roles: ["applicant", "licensee"],
  },
  {
    id: "map",
    label: "Licensing & Events Map",
    href: "/map",
    description: "Map of liquor and cannabis licensed locations with search and filters.",
    roles: ["applicant", "licensee", "public"],
  },
  {
    id: "account",
    label: "Account & Business Profile",
    href: "/account",
    description: "Manage your business profile and delegate access.",
    roles: ["applicant", "licensee"],
  },
  {
    id: "contact",
    label: "Contact LCRB",
    href: "/contact",
    description: "Phone, email, and contact details for the LCRB.",
  },
  {
    id: "police-dashboard",
    label: "Police Dashboard",
    href: "/police",
    description: "Police review dashboard for Special Event Permits and endorsements.",
    roles: ["police"],
  },
  {
    id: "police-queue",
    label: "Police Review Queue",
    href: "/police/queue",
    description: "The queue of SEP applications awaiting police review.",
    roles: ["police"],
  },
  {
    id: "police-events",
    label: "Events Map",
    href: "/police/events",
    description: "Geographic map of events for situational awareness.",
    roles: ["police"],
  },
  {
    id: "lg-dashboard",
    label: "Local Government Dashboard",
    href: "/local-government",
    description: "Local government review dashboard with comment deadlines.",
    roles: ["local-government"],
  },
  {
    id: "lg-queue",
    label: "Local Government Review Queue",
    href: "/local-government/queue",
    description: "Applications awaiting local government comment.",
    roles: ["local-government"],
  },
  {
    id: "lg-map",
    label: "Local Government Map",
    href: "/local-government/map",
    description: "Map of licensed establishments in your jurisdiction.",
    roles: ["local-government"],
  },
  {
    id: "establishment-planner",
    label: "Establishment Planner",
    href: "/establishment-planner",
    description:
      "Plan a new liquor or cannabis establishment: check feasibility, estimate fees and timeline, and build a document checklist.",
    roles: ["applicant", "licensee"],
  },
  {
    id: "lcrb-dashboard",
    label: "LCRB Intake Dashboard",
    href: "/lcrb",
    description: "Staff intake dashboard: priority cases, queues, and volumes synced to MS Dynamics.",
    roles: ["lcrb-staff"],
  },
  {
    id: "lcrb-triage",
    label: "Case Triage",
    href: "/lcrb/triage",
    description: "Triage incoming cases, review flags, and route them for adjudication.",
    roles: ["lcrb-staff"],
  },
  {
    id: "lcrb-analytics",
    label: "Intake Analytics",
    href: "/lcrb/analytics",
    description: "Metrics on case intake volumes, routing, and processing performance.",
    roles: ["lcrb-staff"],
  },
  {
    id: "lcrb-site-analytics",
    label: "Site Analytics",
    href: "/lcrb/site-analytics",
    description: "Portal usage analytics by page and section.",
    roles: ["lcrb-staff"],
  },
]

/** Plain-language guidance for the SEP application, keyed by field name. */
export const SEP_FIELD_HELP: Record<string, string> = {
  eventName: "The public-facing name of your event, e.g. 'Summer Community BBQ'. This appears on your permit.",
  isPrivateResidence:
    "Events at a private home are NOT eligible for a Special Event Permit. Choose 'No' if your event is at a venue, park, hall, or other location.",
  isPublicProperty:
    "Public property means parks, streets, plazas, or community-owned land. If 'Yes', you'll need written approval from the local government or First Nation.",
  isSignificantEvent:
    "Choose 'Yes' only for events of provincial, national, or international significance (e.g. a major festival). Most events are 'No'.",
  eventType:
    "Private events are for invited guests only. Public events are open to anyone. Note: spirits can only be served at private events.",
  eventStartDate:
    "The first day of your event. SEP applications must be submitted at least 14 days before this date.",
  hasCertificate:
    "Serving It Right (SIR) or Special Event Server (SES) certification is required for the person responsible for liquor service.",
  certificateNumber: "Enter the certificate number printed on your SIR or SES certificate.",
  eventOccasion: "Briefly describe the purpose of your event, e.g. 'fundraising gala' or 'wedding reception'. Max 255 characters.",
  hasAdmissionCharge: "Choose 'Yes' if guests pay an admission price to attend.",
  eventCity: "The city or town where the event takes place. This routes your application to the correct police detachment.",
  locationName: "The name of the venue, e.g. 'Nanaimo Community Centre'.",
  venueType: "The kind of venue, e.g. community centre, banquet hall, park, or convention centre.",
  addressLine1: "The street address of the event venue.",
  postalCode: "The venue's postal code in the format V0V 0V0.",
  maxGuests:
    "The maximum number of people at the location at any one time. Events with 500 or more guests are routed to police for review.",
  eventDate: "The calendar date of the event.",
  eventStartTime: "The time liquor service begins.",
  eventEndTime: "The time liquor service ends.",
  liquorTypes:
    "Select every type of liquor you'll serve. Spirits may only be served at private events; public events are limited to beer, cider, coolers, and wine.",
  liquorSaleType: "Choose 'Selling' if guests pay for drinks, or 'Free of charge' if liquor is provided at no cost.",
  estimatedDrinks: "Your best estimate of the total number of standard drinks served across the whole event.",
  hasSecurity: "Choose 'Yes' if you'll have security staff. Larger events typically require security.",
  securityStaffCount: "How many security staff will be on site.",
}

/** General FAQ snippets the assistant can draw on. */
export const PORTAL_FAQ: { q: string; a: string }[] = [
  {
    q: "What is a Special Event Permit?",
    a: "A Special Event Permit (SEP) authorizes serving liquor at a one-time event such as a wedding, fundraiser, or festival. It replaces the older Special Occasion Licence.",
  },
  {
    q: "How long does SEP approval take?",
    a: "Events under 500 guests that meet all criteria can be approved automatically. Larger events are routed to police and local government for review, which can take several days, so apply at least 14 days in advance.",
  },
  {
    q: "Can I save my application and finish later?",
    a: "Yes. Use 'Save & Continue Later' at any step. Your draft appears in the Drafts tab on the Special Events page and you can resume it anytime.",
  },
  {
    q: "Who needs Serving It Right?",
    a: "The person responsible for managing liquor service at your event must hold a valid Serving It Right (SIR) or Special Event Server (SES) certificate.",
  },
  {
    q: "What's the difference between a private and public event?",
    a: "Private events are restricted to invited guests; public events are open to the general public. Spirits can only be served at private events.",
  },
]

/** Step-by-step "how do I..." workflows naming the exact pages and clicks; steps reference NAV_DESTINATIONS. */
export interface PortalWorkflow {
  id: string
  /** The task phrased the way a user would ask it. */
  task: string
  /** Roles this workflow applies to (empty/undefined = everyone). */
  roles?: PortalRole[]
  /** Ordered, plain-language steps referencing real portal pages/buttons. */
  steps: string[]
  /** Primary destination to offer as an action button, when applicable. */
  href?: string
}

export const PORTAL_WORKFLOWS: PortalWorkflow[] = [
  {
    id: "wf-sep-apply",
    task: "How do I apply for a Special Event Permit (SEP)?",
    roles: ["applicant", "licensee"],
    href: "/special-events/apply",
    steps: [
      "Open Special Event Permits from the menu, then choose 'Apply for a Permit' (or use the Start a Special Event Permit button).",
      "Work through the wizard: event details, location, dates/times, liquor service, and the Serving It Right certificate holder.",
      "Use 'Save & Continue Later' anytime — your draft appears in the Drafts tab on the Special Events page.",
      "Review the summary and submit. Apply at least 14 days before your event; events with 500+ guests are routed to police and local government.",
    ],
  },
  {
    id: "wf-find-licence",
    task: "How do I find out which licence or permit I need?",
    roles: ["applicant", "licensee"],
    href: "/licence-finder",
    steps: [
      "Open the Licence Finder (Find the right licence).",
      "Answer the short interview about what you want to do (serve/sell liquor or cannabis, on-site vs off-site, one-time event vs ongoing).",
      "Review the recommended licence type, then tap the suggested action to start that application.",
    ],
  },
  {
    id: "wf-plan-establishment",
    task: "How do I plan or open a new licensed establishment?",
    roles: ["applicant", "licensee"],
    href: "/establishment-planner",
    steps: [
      "From your Dashboard, choose the 'Plan a new establishment' card (or open the Establishment Planner).",
      "Start a new plan and complete the interview: establishment type, location, and add-ons. Your business name and existing licences are pre-filled from your account.",
      "Review the feasibility summary — licence(s) needed, licence-cap status, Section 119 (cannabis), estimated fees, and timeline.",
      "Work the document checklist to raise your readiness score, then use 'Start application' to hand off into the right application, pre-filled.",
    ],
  },
  {
    id: "wf-check-status",
    task: "How do I check the status of my application?",
    roles: ["applicant", "licensee"],
    href: "/applications",
    steps: [
      "Open My Applications to see in-progress, submitted, and completed applications with their current status.",
      "Select an application to view its detailed timeline and any outstanding tasks or requests.",
    ],
  },
  {
    id: "wf-resume-draft",
    task: "How do I save my application and finish it later?",
    roles: ["applicant", "licensee"],
    steps: [
      "Use 'Save & Continue Later' at any step of an application wizard.",
      "To resume, open the relevant section (e.g. Special Events → Drafts tab, or My Applications) and reopen the draft.",
    ],
  },
  {
    id: "wf-upload-docs",
    task: "How do I upload or provide requested documents?",
    roles: ["applicant", "licensee"],
    href: "/applications",
    steps: [
      "Open My Applications and select the submitted application that requested documents.",
      "Go to its Documents section and upload the requested files; the assistant can also run a quick document check for you.",
    ],
  },
  {
    id: "wf-security-screening",
    task: "How do I complete security screening for associates?",
    roles: ["applicant", "licensee"],
    href: "/security-screening",
    steps: [
      "Open Security Screening.",
      "Add each associate/owner/director who requires screening and complete their consent and details.",
      "Track each person's screening status from the same page.",
    ],
  },
  {
    id: "wf-manage-licences",
    task: "How do I view or manage my existing licences?",
    roles: ["applicant", "licensee"],
    href: "/licences",
    steps: [
      "Open Licences & Authorizations to see every licence you hold and its status.",
      "Select a licence to view details or start a change/renewal action available for it.",
    ],
  },
  {
    id: "wf-account-delegate",
    task: "How do I update my business profile or give someone else access?",
    roles: ["applicant", "licensee"],
    href: "/account",
    steps: [
      "Open Account & Business Profile.",
      "Update your business details, or use the delegate access section to invite and manage additional users.",
    ],
  },
  {
    id: "wf-map",
    task: "How do I see licensed establishments or events near a location?",
    roles: ["applicant", "licensee", "public"],
    href: "/map",
    steps: [
      "Open the Licensing & Events Map.",
      "Search an address or area and use the filters to show the licence/event types you care about.",
    ],
  },
  {
    id: "wf-police-review",
    task: "How do I review a Special Event Permit application?",
    roles: ["police"],
    href: "/police/queue",
    steps: [
      "Open the Police Review Queue to see SEP applications awaiting review.",
      "Open an application, then use 'Assign to me' to take it.",
      "Review the event details and either 'Approve permit' or 'Deny permit'. You can attach conditions before approving.",
    ],
  },
  {
    id: "wf-police-conditions",
    task: "How do I add conditions to a permit?",
    roles: ["police"],
    steps: [
      "Open the application from the review queue.",
      "In the Permit conditions section, use 'Add a standard condition' (or write a custom one).",
      "Approve the permit to apply the conditions.",
    ],
  },
  {
    id: "wf-police-events",
    task: "How do I see upcoming events in my jurisdiction?",
    roles: ["police"],
    href: "/police/events",
    steps: ["Open the Events Map for a geographic view of upcoming events for situational awareness."],
  },
  {
    id: "wf-lg-comment",
    task: "How do I comment on or provide a recommendation for an application?",
    roles: ["local-government"],
    href: "/local-government/queue",
    steps: [
      "Open your Review Queue and select the application referred to you.",
      "Review the details, then in 'Submit your comment' provide your recommendation (e.g. No objection, Recommend, or oppose with reasons).",
      "If you need more information, use 'Request document' before submitting.",
    ],
  },
  {
    id: "wf-lg-deadlines",
    task: "How do I see what's due for comment this week?",
    roles: ["local-government"],
    href: "/local-government",
    steps: [
      "Open your Dashboard to see the comment-deadline list and applications awaiting review, sorted by due date.",
    ],
  },
  {
    id: "wf-lcrb-triage",
    task: "How do I triage an incoming case?",
    roles: ["lcrb-staff"],
    href: "/lcrb/triage",
    steps: [
      "Open Case Triage from the intake dashboard.",
      "Review the case flags and details, then route it for adjudication or mark it for human review.",
      "Standard, low-risk cases are auto-routed; focus on the cases flagged as needing review.",
    ],
  },
  {
    id: "wf-lcrb-priority",
    task: "How do I find cases that need human review?",
    roles: ["lcrb-staff"],
    href: "/lcrb",
    steps: [
      "Open the LCRB Intake Dashboard.",
      "Use the 'Need human review' metric and the Priority cases list to jump straight to those cases.",
    ],
  },
  {
    id: "wf-lcrb-analytics",
    task: "How do I view intake or portal analytics?",
    roles: ["lcrb-staff"],
    href: "/lcrb/analytics",
    steps: [
      "Open Intake Analytics for case volume, routing, and processing metrics.",
      "Open Site Analytics for portal usage broken down by page and section.",
    ],
  },
]

/** Workflows relevant to a given role (role-specific + everyone). */
export function workflowsForRole(role: string): PortalWorkflow[] {
  return PORTAL_WORKFLOWS.filter(
    (w) => !w.roles || w.roles.length === 0 || w.roles.includes(role as PortalRole),
  )
}
