/** Startable-application catalog for the AI assistant. */

import { APPLICATION_CATALOG } from "../applications/mock/applications"
import { LICENCE_TYPE_LABELS } from "@domain/models/applications"
import type { LicenceType } from "@domain/models/applications"

export interface StartableApplication {
  slug: string
  title: string
  description: string
  licenceType: LicenceType
  licenceLabel: string
  kind: "application" | "authorization"
  href: string
}

/** Every catalog entry as a launchable action the assistant can offer. */
export function getStartableApplications(): StartableApplication[] {
  return APPLICATION_CATALOG.map((e) => ({
    slug: e.slug,
    title: e.title,
    description: e.description,
    licenceType: e.licenceType,
    licenceLabel: LICENCE_TYPE_LABELS[e.licenceType],
    kind: e.kind,
    href: `/applications/new/${e.slug}`,
  }))
}

/** Hand-picked primary launch points (one main application per licence type, plus the Special Event Permit), surfaced as CTAs first. */
export const PRIMARY_LAUNCH_POINTS: { slug: string; href: string; intent: string }[] = [
  {
    slug: "liquor-primary-licence",
    href: "/applications/new/liquor-primary-licence",
    intent: "open a bar, pub, nightclub, lounge, or other place whose main purpose is serving liquor",
  },
  {
    slug: "food-primary",
    href: "/applications/food-primary",
    intent: "open a restaurant or café that serves liquor with food",
  },
  {
    slug: "cannabis-retail-store-application",
    href: "/applications/new/cannabis-retail-store-application",
    intent: "open a cannabis / marijuana retail store",
  },
  {
    slug: "licensee-retail-store-application",
    href: "/applications/new/licensee-retail-store-application",
    intent: "open a private liquor store (licensee retail store)",
  },
  {
    slug: "manufacturer-application",
    href: "/applications/new/manufacturer-application",
    intent: "make liquor — a brewery, winery, distillery or cidery",
  },
  {
    slug: "special-event-permit",
    href: "/special-events/apply",
    intent: "serve liquor at a one-time event, wedding, festival or fundraiser (Special Event Permit)",
  },
]

/** Which roles are allowed to START applications, and what they can start. - licensee */
export function canStartApplications(role: string): boolean {
  return role === "licensee" || role === "applicant"
}

/** Compact catalog listing for the system prompt, grouped by licence type so the model can map an intent to a real slug. */
export function buildApplicationCatalogPrompt(options?: {
  includeSpecialEvent?: boolean
  role?: string
}): string {
  const role = options?.role ?? "licensee"

  // Reviewer roles cannot file anything.
  if (!canStartApplications(role)) {
    return `### Starting applications
This user is a reviewer (${role}) and does NOT file applications. Do NOT offer to start any application or use suggestActions to launch application flows. Instead help them navigate their review queues and answer policy questions.`
  }

  // SEP applicants only ever start a Special Event Permit.
  if (role === "applicant") {
    return `### Common intents → application to start
- If they want to serve liquor at a one-time event, wedding, festival or fundraiser → href "/special-events/apply" (Special Event Permit)

This user is a Special Event Permit applicant. The ONLY application they can start is a Special Event Permit (/special-events/apply). Do NOT offer business licence applications; if they ask about running a bar/store/manufacturer, explain that requires a business (BCeID) licensee account.`
  }

  const includeSpecialEvent = options?.includeSpecialEvent ?? true

  const groups = new Map<string, StartableApplication[]>()
  for (const a of getStartableApplications()) {
    const list = groups.get(a.licenceLabel) ?? []
    list.push(a)
    groups.set(a.licenceLabel, list)
  }

  const launchPoints = includeSpecialEvent
    ? PRIMARY_LAUNCH_POINTS
    : PRIMARY_LAUNCH_POINTS.filter((p) => p.slug !== "special-event-permit")

  const primary = launchPoints
    .map((p) => `- If they want to ${p.intent} → href "${p.href}"`)
    .join("\n")

  const full = Array.from(groups.entries())
    .map(([label, apps]) => {
      const items = apps.map((a) => `  - "${a.title}" → /applications/new/${a.slug}`).join("\n")
      return `${label}:\n${items}`
    })
    .join("\n")

  return `### Common intents → application to start
${primary}

### Full application catalog (start any of these with /applications/new/{slug}, or the SEP href above)
${full}`
}
