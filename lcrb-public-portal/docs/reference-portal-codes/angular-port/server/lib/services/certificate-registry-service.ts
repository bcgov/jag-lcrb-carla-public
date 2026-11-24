/** Serving It Right certificate check (prototype). Without `RESPONSIBLE_SERVICE_API_KEY` it returns `simulated: true`. TODO: set the key and URL, drop `simulate*`. */

export type CertificateProgram = "serving-it-right" | "selling-it-right" | "unknown"

export type CertificateMatchStatus =
  /** Certificate found on the registry and currently valid (not expired). */
  | "valid"
  /** Certificate found but past its expiry date. */
  | "expired"
  /** No matching certificate number/holder found on the registry. */
  | "not-found"
  /** The registry could not be reached — advisory only. */
  | "unavailable"

export interface CertificateVerifyInput {
  program: CertificateProgram
  /** Certificate number as read from the document, if any. */
  certificateNumber?: string | null
  /** Certificate holder full name, if any. */
  holderName?: string | null
  /** Expiry date read from the document (ISO YYYY-MM-DD), if any. */
  expiryDate?: string | null
}

export interface CertificateVerifyResult {
  status: CertificateMatchStatus
  simulated: boolean
  program: CertificateProgram
  certificateNumber?: string
  holderName?: string
  /** Registry's authoritative expiry date, when found. */
  expiryDate?: string
  /** Whole months until (positive) or since (negative) expiry. */
  monthsToExpiry?: number | null
  summary: string
  checkedAt: string
}

const REGISTRY_BASE_URL =
  process.env.RESPONSIBLE_SERVICE_API_URL ?? "https://api.responsibleservice.bc.ca/v1"

function normName(n: string): string {
  return n
    .toLowerCase()
    .replace(/[^a-z\s]/g, "")
    .replace(/\s+/g, " ")
    .trim()
}

/** Fractional months between an ISO date and now (positive = in the future). */
function monthsUntil(iso: string): number | null {
  const then = new Date(iso)
  if (Number.isNaN(then.getTime())) return null
  const now = new Date()
  const months =
    (then.getFullYear() - now.getFullYear()) * 12 +
    (then.getMonth() - now.getMonth()) +
    (then.getDate() - now.getDate()) / 30
  return Math.round(months * 10) / 10
}

const PROGRAM_LABEL: Record<CertificateProgram, string> = {
  "serving-it-right": "Serving It Right",
  "selling-it-right": "Selling It Right",
  unknown: "responsible service",
}

/** Small seeded registry keyed by normalised certificate number, so the simulated path returns stable results. */
const SEED_CERTIFICATES: Record<
  string,
  { holder: string; program: CertificateProgram; expiry: string }
> = {
  sir1029384: { holder: "Sam Rivera", program: "serving-it-right", expiry: "2028-04-30" },
  sir5566778: { holder: "Priya Anand", program: "serving-it-right", expiry: "2024-01-15" }, // expired
  sir2233445: { holder: "Jordan Lee", program: "selling-it-right", expiry: "2027-11-01" },
}

/** Confirm a responsible-service certificate. Never throws: on failure it returns an `unavailable` advisory. */
export async function verifyCertificate(
  input: CertificateVerifyInput,
): Promise<CertificateVerifyResult> {
  const apiKey = process.env.RESPONSIBLE_SERVICE_API_KEY
  if (!apiKey) return simulateVerify(input)

  try {
    const number = (input.certificateNumber ?? "").trim()
    const res = await fetch(`${REGISTRY_BASE_URL}/certificates/${encodeURIComponent(number)}`, {
      headers: { "x-apikey": apiKey, Accept: "application/json" },
      signal: AbortSignal.timeout(8000),
    })
    if (res.status === 404) {
      return {
        status: "not-found",
        simulated: false,
        program: input.program,
        certificateNumber: input.certificateNumber ?? undefined,
        summary: `No ${PROGRAM_LABEL[input.program]} certificate found for number "${number}".`,
        checkedAt: new Date().toISOString(),
      }
    }
    if (!res.ok) throw new Error(`Responsible-service registry responded ${res.status}`)
    const data = (await res.json()) as {
      holderName?: string
      program?: CertificateProgram
      expiryDate?: string
    }
    return build(input, data.holderName ?? input.holderName ?? "", data.expiryDate ?? null, false)
  } catch {
    return {
      status: "unavailable",
      simulated: false,
      program: input.program,
      certificateNumber: input.certificateNumber ?? undefined,
      summary:
        "The responsible-service registry could not be reached to verify this certificate. Staff should confirm manually. This does not block submission.",
      checkedAt: new Date().toISOString(),
    }
  }
}

function simulateVerify(input: CertificateVerifyInput): CertificateVerifyResult {
  const key = (input.certificateNumber ?? "").toLowerCase().replace(/[^a-z0-9]/g, "")
  const record = key ? SEED_CERTIFICATES[key] : undefined

  if (record) {
    return build(
      { ...input, program: record.program },
      record.holder,
      record.expiry,
      true,
    )
  }

  // Unknown number: use the document's own expiry date for a believable valid/expired result, otherwise not-found.
  if (input.expiryDate) {
    return build(input, input.holderName ?? "Certificate holder", input.expiryDate, true)
  }
  return {
    status: "not-found",
    simulated: true,
    program: input.program,
    certificateNumber: input.certificateNumber ?? undefined,
    summary: `Simulated registry check: no matching ${PROGRAM_LABEL[input.program]} certificate could be found. Staff should confirm.`,
    checkedAt: new Date().toISOString(),
  }
}

function build(
  input: CertificateVerifyInput,
  holderName: string,
  expiryDate: string | null,
  simulated: boolean,
): CertificateVerifyResult {
  const months = expiryDate ? monthsUntil(expiryDate) : null
  const expired = months != null ? months < 0 : false
  const sim = simulated ? "Simulated registry check: " : "Registry check: "
  const label = PROGRAM_LABEL[input.program]
  let summary: string
  if (expired) {
    summary = `${sim}${label} certificate${holderName ? ` for ${holderName}` : ""} EXPIRED on ${expiryDate}. A current certificate should be provided.`
  } else if (months != null) {
    summary = `${sim}${label} certificate${holderName ? ` for ${holderName}` : ""} is valid (expires ${expiryDate}).`
  } else {
    summary = `${sim}${label} certificate located${holderName ? ` for ${holderName}` : ""}, but no expiry date could be read.`
  }
  return {
    status: expired ? "expired" : "valid",
    simulated,
    program: input.program,
    certificateNumber: input.certificateNumber ?? undefined,
    holderName: holderName || undefined,
    expiryDate: expiryDate ?? undefined,
    monthsToExpiry: months,
    summary,
    checkedAt: new Date().toISOString(),
  }
}

export { normName as _normNameForTests }
