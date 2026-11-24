import { generateText, Output } from "ai"
import { z } from "zod"
import { logAiError, AI_MAX_RETRIES, friendlyJsonError } from "@domain/ai/ai-errors"
import { AI_MODEL, AI_PROVIDER_OPTIONS } from "@server-lib/ai/model-config"
import { lookupBusiness, type RegistryLookupResult } from "@server-lib/services/bc-registries-service"
import {
  verifyCertificate,
  type CertificateProgram,
  type CertificateVerifyResult,
} from "@server-lib/services/certificate-registry-service"
import { checkFitAndProper, type FitAndProperResult } from "@server-lib/services/personnel-screening-service"
import { retrieveKnowledge } from "@server-lib/ai/knowledge-base"

export const maxDuration = 60

/** Advisory completeness check and type-aware extraction on an uploaded PDF or image. Feeds the staff AI assessment via `advisories`; never blocks submission. */

type DocType =
  | "floor-plan"
  | "director-info"
  | "incorporation"
  | "lease"
  | "lg-approval"
  | "certificate"
  | "generic"

/** Contextual values from the application form used for cross-checks. */
interface DocContext {
  /** The establishment / premises address declared on the application. */
  applicationAddress?: string
  /** The establishment (business) name declared on the application. */
  establishmentName?: string
  /** Declared floor area, for a floor-plan scale sanity check. */
  declaredArea?: number
  declaredAreaUnit?: "sqft" | "sqm"
  /** Currently approved occupant capacity, for a structural-change comparison. */
  currentApprovedCapacity?: number
}


const occupancySchema = z.object({
  stampFound: z.boolean().describe("True if an occupancy/occupant load stamp or notice is visible on the plan."),
  spaces: z
    .array(z.object({ label: z.string(), load: z.number() }))
    .describe(
      "Each distinct space/floor/room with its own occupant load number read from the stamp (numbers are often handwritten). Empty if a single total is given or none found.",
    ),
  totalLoad: z
    .number()
    .nullable()
    .describe("The total occupant load. If the stamp gives a single total use it; if it lists per-space loads, SUM them. null if not legible."),
  stampDate: z
    .string()
    .nullable()
    .describe("The date on the occupancy stamp as an ISO date YYYY-MM-DD, or null if none is legible."),
  floorAreaValue: z
    .number()
    .nullable()
    .describe("The total floor area of the licensed/affected space if stated or derivable from labelled dimensions, else null."),
  floorAreaUnit: z
    .enum(["sqft", "sqm"])
    .nullable()
    .describe("Unit for floorAreaValue: 'sqft' or 'sqm'. null if no area is available."),
  scaleNoted: z
    .boolean()
    .describe("True if the plan states a scale (e.g. 1:100) or shows labelled dimensions; false if neither is present."),
})

const corporateSchema = z.object({
  found: z.boolean().describe("True if the document contains director or corporate-registry information."),
  companyName: z.string().nullable().describe("The company legal name, or null."),
  incorporationNumber: z.string().nullable().describe("The BC incorporation/registration number (e.g. BC1234567), or null."),
  directors: z.array(z.string()).describe("Full names of directors/officers listed. Empty if none."),
  companyStatus: z
    .string()
    .nullable()
    .describe("Registration status if stated on the document (e.g. 'Active', 'In Good Standing'), else null."),
})

const leaseSchema = z.object({
  found: z.boolean().describe("True if this is a lease / tenancy / property-tenure document."),
  premisesAddress: z.string().nullable().describe("The leased premises address, or null."),
  landlord: z.string().nullable().describe("The landlord / lessor name, or null."),
  tenant: z.string().nullable().describe("The tenant / lessee name, or null."),
  termStart: z.string().nullable().describe("Lease term start as ISO YYYY-MM-DD, or null."),
  termEnd: z.string().nullable().describe("Lease term end/expiry as ISO YYYY-MM-DD, or null."),
  signed: z.boolean().describe("True if the lease appears signed/executed by the parties."),
})

const lgApprovalSchema = z.object({
  found: z.boolean().describe("True if this is a local-government or First Nation approval / resolution / referral response."),
  governmentBody: z.string().nullable().describe("The local or Indigenous government body issuing the document, or null."),
  establishmentName: z.string().nullable().describe("The establishment named in the approval, or null."),
  establishmentAddress: z.string().nullable().describe("The establishment address named in the approval, or null."),
  decision: z
    .enum(["support", "oppose", "no-comment", "resolution-passed", "unclear"])
    .describe("The government's recorded decision/position on the application."),
  decisionDate: z.string().nullable().describe("The date of the decision/resolution as ISO YYYY-MM-DD, or null."),
})

const certificateSchema = z.object({
  found: z.boolean().describe("True if this is a Serving It Right or Selling It Right (responsible service) certificate."),
  program: z
    .enum(["serving-it-right", "selling-it-right", "unknown"])
    .describe("Which responsible-service program the certificate is for."),
  certificateNumber: z.string().nullable().describe("The certificate / registration number, or null."),
  holderName: z.string().nullable().describe("The certificate holder's full name, or null."),
  issueDate: z.string().nullable().describe("Issue date as ISO YYYY-MM-DD, or null."),
  expiryDate: z.string().nullable().describe("Expiry date as ISO YYYY-MM-DD, or null."),
})

const assessmentSchema = z.object({
  status: z
    .enum(["complete", "incomplete", "unclear"])
    .describe(
      "complete = the document clearly satisfies every requirement; incomplete = one or more required elements are missing or wrong; unclear = the file could not be read or is the wrong kind of document",
    ),
  summary: z.string().describe("One plain-language sentence (max ~25 words) describing the result for the applicant."),
  documentType: z
    .string()
    .describe("What this document actually appears to be, e.g. 'scaled floor plan', 'lease agreement', 'blank page'."),
  present: z.array(z.string()).describe("Requirements the document appears to satisfy. Empty array if none."),
  missing: z
    .array(z.string())
    .describe("Requirements that are missing, unclear, or not met. Empty array if the document is complete."),
  occupancy: occupancySchema.nullable().describe("Provide ONLY for floor plans; null otherwise."),
  corporate: corporateSchema.nullable().describe("Provide ONLY for director/registry/incorporation documents; null otherwise."),
  lease: leaseSchema.nullable().describe("Provide ONLY for lease/tenure documents; null otherwise."),
  lgApproval: lgApprovalSchema.nullable().describe("Provide ONLY for local/Indigenous government approvals; null otherwise."),
  certificate: certificateSchema.nullable().describe("Provide ONLY for responsible-service certificates; null otherwise."),
})

interface CheckRequest {
  fileData: string
  filename: string
  mediaType: string
  label: string
  criteria: string[]
  applicationTitle?: string
  docType?: DocType
  context?: DocContext
}


function monthsSince(iso: string): number | null {
  const then = new Date(iso)
  if (Number.isNaN(then.getTime())) return null
  const now = new Date()
  return (
    (now.getFullYear() - then.getFullYear()) * 12 +
    (now.getMonth() - then.getMonth()) +
    (now.getDate() - then.getDate()) / 30
  )
}

function addMonths(iso: string, months: number): string | null {
  const d = new Date(iso)
  if (Number.isNaN(d.getTime())) return null
  d.setMonth(d.getMonth() + months)
  return d.toISOString().slice(0, 10)
}

function isPast(iso: string): boolean | null {
  const d = new Date(iso)
  if (Number.isNaN(d.getTime())) return null
  return d.getTime() < Date.now()
}


function normAddress(a: string): string {
  return a
    .toLowerCase()
    .replace(/[.,#]/g, " ")
    .replace(/\b(street|st|avenue|ave|road|rd|drive|dr|boulevard|blvd|highway|hwy|suite|unit|floor|fl)\b/g, "")
    .replace(/\b(british columbia|bc|canada)\b/g, "")
    .replace(/\s+/g, " ")
    .trim()
}

/** Rough token-overlap match so minor formatting differences still match. */
function addressesMatch(a: string, b: string): boolean {
  const ta = new Set(normAddress(a).split(" ").filter(Boolean))
  const tb = new Set(normAddress(b).split(" ").filter(Boolean))
  if (ta.size === 0 || tb.size === 0) return false
  let shared = 0
  for (const t of ta) if (tb.has(t)) shared++
  const ratio = shared / Math.min(ta.size, tb.size)
  return ratio >= 0.6
}


interface OccupancyResult {
  stampFound: boolean
  spaces: { label: string; load: number }[]
  totalLoad: number | null
  stampDate: string | null
  validUntil: string | null
  monthsOld: number | null
  stampExpired: boolean | null
  floorArea: { value: number; unit: "sqft" | "sqm" } | null
  scaleNoted: boolean
}

interface CorporateResult {
  found: boolean
  companyName: string | null
  incorporationNumber: string | null
  directors: string[]
  companyStatus: string | null
  registry?: RegistryLookupResult
  fitAndProper?: FitAndProperResult
}

interface LeaseResult {
  premisesAddress: string | null
  landlord: string | null
  tenant: string | null
  termStart: string | null
  termEnd: string | null
  signed: boolean
  termExpired: boolean | null
  addressMatchesApplication: boolean | null
}

interface LgApprovalResult {
  governmentBody: string | null
  establishmentName: string | null
  establishmentAddress: string | null
  decision: "support" | "oppose" | "no-comment" | "resolution-passed" | "unclear"
  decisionDate: string | null
  addressMatchesApplication: boolean | null
  policyGuidance?: { note: string; source?: string }
}

interface CertificateResult extends CertificateVerifyResult {}


function extractionInstructions(docType: DocType): string {
  switch (docType) {
    case "floor-plan":
      return `

DOCUMENT INTELLIGENCE — FLOOR PLAN:
- Locate the OCCUPANCY LOAD stamp/notice. The occupant-load number(s) are frequently HANDWRITTEN.
- If the stamp lists several spaces/floors each with their own load, read EACH into "occupancy.spaces" (label + number) and SUM them into "occupancy.totalLoad". If it gives a single overall number, put it in "occupancy.totalLoad" and leave "spaces" empty.
- Read the stamp DATE into "occupancy.stampDate" (ISO YYYY-MM-DD; use the 1st if only month/year is legible). If there is no stamp, set "occupancy.stampFound" false and "occupancy.totalLoad" null.
- Read the floor AREA into "occupancy.floorAreaValue" + "occupancy.floorAreaUnit" if stated or derivable from labelled dimensions, else null.
- Set "occupancy.scaleNoted" true only if a scale (e.g. 1:100) or labelled dimensions are present.
- Set corporate, lease, lgApproval, certificate to null.`
    case "director-info":
      return `

DOCUMENT INTELLIGENCE — DIRECTORS / CORPORATE REGISTRY:
- Extract "corporate.companyName", "corporate.incorporationNumber" (e.g. BC1234567), every director/officer full name into "corporate.directors", and "corporate.companyStatus" if stated.
- If none present, set "corporate.found" false.
- Set occupancy, lease, lgApproval, certificate to null.`
    case "incorporation":
      return `

DOCUMENT INTELLIGENCE — INCORPORATION / BUSINESS SUMMARY:
- Extract "corporate.companyName", "corporate.incorporationNumber", "corporate.companyStatus" (registration status), and any directors into "corporate.directors".
- If none present, set "corporate.found" false.
- Set occupancy, lease, lgApproval, certificate to null.`
    case "lease":
      return `

DOCUMENT INTELLIGENCE — LEASE / TENURE:
- Extract the leased "lease.premisesAddress", "lease.landlord", "lease.tenant", "lease.termStart" and "lease.termEnd" (ISO YYYY-MM-DD), and set "lease.signed" true if the document appears executed by the parties.
- If this is not a lease/tenure document set "lease.found" false.
- Set occupancy, corporate, lgApproval, certificate to null.`
    case "lg-approval":
      return `

DOCUMENT INTELLIGENCE — LOCAL / INDIGENOUS GOVERNMENT APPROVAL:
- Extract the issuing "lgApproval.governmentBody", the "lgApproval.establishmentName" and "lgApproval.establishmentAddress" it refers to, the "lgApproval.decision" (support / oppose / no-comment / resolution-passed / unclear), and "lgApproval.decisionDate" (ISO YYYY-MM-DD).
- If this is not a government approval/resolution set "lgApproval.found" false.
- Set occupancy, corporate, lease, certificate to null.`
    case "certificate":
      return `

DOCUMENT INTELLIGENCE — RESPONSIBLE SERVICE CERTIFICATE:
- Determine "certificate.program" (serving-it-right for liquor, selling-it-right for cannabis, else unknown).
- Extract "certificate.certificateNumber", "certificate.holderName", "certificate.issueDate" and "certificate.expiryDate" (ISO YYYY-MM-DD).
- If this is not a responsible-service certificate set "certificate.found" false.
- Set occupancy, corporate, lease, lgApproval to null.`
    default:
      return `

DOCUMENT INTELLIGENCE:
- This document type does not require structured extraction. Set occupancy, corporate, lease, lgApproval and certificate ALL to null.`
  }
}

export async function POST(req: Request) {
  let body: CheckRequest
  try {
    body = (await req.json()) as CheckRequest
  } catch {
    return Response.json({ error: "Invalid request body." }, { status: 400 })
  }

  const { fileData, filename, mediaType, label, criteria, applicationTitle } = body
  const docType: DocType = body.docType ?? "generic"
  const context: DocContext = body.context ?? {}
  if (!fileData || !label) {
    return Response.json({ error: "Missing file or document label." }, { status: 400 })
  }

  const criteriaList =
    criteria && criteria.length
      ? criteria.map((c, i) => `${i + 1}. ${c}`).join("\n")
      : `The document is a valid "${label}" and is legible.`

  const systemPrompt = `You are a meticulous but fair document-intake reviewer for the BC Liquor & Cannabis Regulation Branch (LCRB). An applicant has uploaded a supporting document for ${
    applicationTitle ? `their "${applicationTitle}" application` : "their licensing application"
  }.

Your job is to check whether the uploaded file satisfies the requirements for the document type "${label}", and to extract structured data where instructed.

Requirements this document must meet:
${criteriaList}

Rules:
- Judge ONLY against the requirements above. Do not invent extra requirements.
- If the file is unreadable, blank, or clearly a different type of document, return status "unclear" and explain.
- If every requirement is reasonably satisfied, return status "complete" with an empty "missing" array.
- If one or more requirements are not met, return status "incomplete" and list each gap in "missing" in plain, specific language the applicant can act on.
- Be reasonable: applicants are members of the public, not lawyers. Do not fail a document over trivial formatting.
- For extraction, only report what you can actually read. Never guess a number, date, or name. Use null / false / empty arrays when something is not legible.${extractionInstructions(docType)}`

  try {
    const { experimental_output } = await generateText({
      model: AI_MODEL,
      providerOptions: AI_PROVIDER_OPTIONS,
      maxRetries: AI_MAX_RETRIES,
      experimental_output: Output.object({ schema: assessmentSchema }),
      messages: [
        { role: "system", content: systemPrompt },
        {
          role: "user",
          content: [
            { type: "text", text: `Review the attached "${label}" and assess it against the requirements.` },
            {
              type: "file",
              data: fileData,
              mediaType: mediaType || "application/pdf",
              filename: filename || "document.pdf",
            },
          ],
        },
      ],
    })

    const out = experimental_output as z.infer<typeof assessmentSchema>
    const advisories: string[] = []

    let occupancy: OccupancyResult | undefined
    if (docType === "floor-plan" && out.occupancy) {
      const o = out.occupancy
      const summed = o.spaces.reduce((a, s) => a + (Number.isFinite(s.load) ? s.load : 0), 0)
      const totalLoad = o.totalLoad ?? (o.spaces.length ? summed : null)
      const months = o.stampDate ? monthsSince(o.stampDate) : null
      const stampExpired = months != null ? months > 12 : null
      occupancy = {
        stampFound: o.stampFound,
        spaces: o.spaces,
        totalLoad,
        stampDate: o.stampDate,
        validUntil: o.stampDate ? addMonths(o.stampDate, 12) : null,
        monthsOld: months != null ? Math.round(months * 10) / 10 : null,
        stampExpired,
        floorArea: o.floorAreaValue != null && o.floorAreaUnit ? { value: o.floorAreaValue, unit: o.floorAreaUnit } : null,
        scaleNoted: o.scaleNoted,
      }

      if (totalLoad != null) advisories.push(`Occupancy load read from the stamp: ${totalLoad} persons.`)
      if (!o.stampFound) advisories.push("No occupancy load stamp was detected on the floor plan.")
      if (stampExpired === true)
        advisories.push(`Occupancy stamp dated ${o.stampDate} is older than 12 months (out of date) — a current stamp may be requested.`)
      else if (stampExpired === false) advisories.push(`Occupancy stamp is current (valid until ${occupancy.validUntil}).`)
      if (!o.scaleNoted) advisories.push("The floor plan does not state a scale or labelled dimensions — staff may not be able to verify measurements.")

      // Scale/area sanity check against the declared area.
      if (context.declaredArea && occupancy.floorArea) {
        const decl = context.declaredArea
        const plan = occupancy.floorArea.value
        // Only compare when units line up (best-effort — same unit assumed).
        const diff = Math.abs(plan - decl) / Math.max(decl, 1)
        if (diff > 0.25)
          advisories.push(
            `The floor area on the plan (${plan} ${occupancy.floorArea.unit}) differs from the declared area (${decl}) by more than 25% — please confirm the plan matches the application.`,
          )
        else advisories.push(`Floor area on the plan is consistent with the declared area (~${plan} ${occupancy.floorArea.unit}).`)
      }

      // Structural change: compare new load to the currently approved capacity.
      if (context.currentApprovedCapacity != null && totalLoad != null) {
        const cur = context.currentApprovedCapacity
        if (totalLoad > cur)
          advisories.push(
            `The plan's occupant load (${totalLoad}) is HIGHER than the currently approved capacity (${cur}) — a capacity increase will need review.`,
          )
        else if (totalLoad < cur)
          advisories.push(`The plan's occupant load (${totalLoad}) is lower than the currently approved capacity (${cur}).`)
        else advisories.push(`The plan's occupant load matches the currently approved capacity (${cur}).`)
      }
    }

    let corporate: CorporateResult | undefined
    if ((docType === "director-info" || docType === "incorporation") && out.corporate?.found) {
      const c = out.corporate
      let registry: RegistryLookupResult | undefined
      if (c.directors.length || c.companyName || c.incorporationNumber) {
        registry = await lookupBusiness({
          companyName: c.companyName,
          incorporationNumber: c.incorporationNumber,
          declaredDirectors: c.directors,
        })
      }
      const fitAndProper = c.directors.length ? checkFitAndProper(c.directors) : undefined
      corporate = {
        found: true,
        companyName: c.companyName,
        incorporationNumber: c.incorporationNumber,
        directors: c.directors,
        companyStatus: c.companyStatus,
        registry,
        fitAndProper,
      }
      if (registry) advisories.push(registry.summary)
      if (registry?.registrationState && docType === "incorporation")
        advisories.push(`BC Registries lists this company as "${registry.registrationState}".`)
      if (fitAndProper) advisories.push(fitAndProper.summary)
    }

    let lease: LeaseResult | undefined
    if (docType === "lease" && out.lease?.found) {
      const l = out.lease
      const termExpired = l.termEnd ? isPast(l.termEnd) : null
      const addressMatch = l.premisesAddress && context.applicationAddress ? addressesMatch(l.premisesAddress, context.applicationAddress) : null
      lease = {
        premisesAddress: l.premisesAddress,
        landlord: l.landlord,
        tenant: l.tenant,
        termStart: l.termStart,
        termEnd: l.termEnd,
        signed: l.signed,
        termExpired,
        addressMatchesApplication: addressMatch,
      }
      if (l.premisesAddress) advisories.push(`Lease premises address read as: ${l.premisesAddress}.`)
      if (addressMatch === true) advisories.push("Lease premises address matches the application address.")
      else if (addressMatch === false)
        advisories.push("The lease premises address does NOT appear to match the application address — please confirm.")
      if (termExpired === true) advisories.push(`The lease term ended ${l.termEnd} (expired) — staff may ask for current tenure.`)
      else if (termExpired === false && l.termEnd) advisories.push(`Lease term runs to ${l.termEnd}.`)
      if (!l.signed) advisories.push("The lease does not appear to be signed by both parties.")
    }

    let lgApproval: LgApprovalResult | undefined
    if (docType === "lg-approval" && out.lgApproval?.found) {
      const g = out.lgApproval
      const addressMatch =
        g.establishmentAddress && context.applicationAddress ? addressesMatch(g.establishmentAddress, context.applicationAddress) : null

      // RAG: attach LG-referral policy guidance from the knowledge base.
      let policyGuidance: { note: string; source?: string } | undefined
      const chunks = retrieveKnowledge("local government first nation approval recommendation referral resolution", {
        limit: 1,
      })
      if (chunks.length) {
        policyGuidance = {
          note: "Local/Indigenous government input is a recommendation the general manager considers; it does not by itself decide the application.",
          source: chunks[0].source?.label,
        }
      }

      lgApproval = {
        governmentBody: g.governmentBody,
        establishmentName: g.establishmentName,
        establishmentAddress: g.establishmentAddress,
        decision: g.decision,
        decisionDate: g.decisionDate,
        addressMatchesApplication: addressMatch,
        policyGuidance,
      }
      const decisionLabel: Record<typeof g.decision, string> = {
        support: "supports the application",
        oppose: "opposes the application",
        "no-comment": "provided no comment",
        "resolution-passed": "passed a resolution",
        unclear: "recorded an unclear position",
      }
      advisories.push(`${g.governmentBody ?? "The local/Indigenous government"} ${decisionLabel[g.decision]}${g.decisionDate ? ` (dated ${g.decisionDate})` : ""}.`)
      if (g.decision === "oppose")
        advisories.push("The government opposes this application — flag for staff review; the general manager weighs this input.")
      if (addressMatch === false)
        advisories.push("The establishment address in the approval does NOT appear to match the application address — please confirm.")
    }

    let certificate: CertificateResult | undefined
    if (docType === "certificate" && out.certificate?.found) {
      const cert = out.certificate
      certificate = await verifyCertificate({
        program: cert.program as CertificateProgram,
        certificateNumber: cert.certificateNumber,
        holderName: cert.holderName,
        expiryDate: cert.expiryDate,
      })
      advisories.push(certificate.summary)
    }

    return Response.json({
      status: out.status,
      summary: out.summary,
      documentType: out.documentType,
      present: out.present,
      missing: out.missing,
      occupancy,
      corporate,
      lease,
      lgApproval,
      certificate,
      advisories,
    })
  } catch (error) {
    console.error("[v0] document-check error:", error instanceof Error ? error.message : error)
    logAiError("document-check", error)
    const { body, status } = friendlyJsonError(error, "Could not analyze the document. You can still continue.")
    return Response.json(body, { status })
  }
}
