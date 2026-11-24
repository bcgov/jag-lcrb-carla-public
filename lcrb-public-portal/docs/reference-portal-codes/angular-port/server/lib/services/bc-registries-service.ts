/** BC Registries lookup (prototype). Without `BC_REGISTRIES_API_KEY` it returns `simulated: true`. TODO: set the key, drop the `simulate*` fallback. */

export type RegistryMatchStatus =
  /** Company found and every declared director matches a registry record. */
  | "verified"
  /** Company found but one or more declared directors are not on the registry. */
  | "partial"
  /** Company/number could not be found in the registry. */
  | "not-found"
  /** The registry could not be reached (network/credentials) — advisory only. */
  | "unavailable"

export interface RegistryDirector {
  name: string
  /** Whether this declared director matched an active registry record. */
  matched: boolean
  /** The registry role, when matched (e.g. "Director", "Officer"). */
  role?: string
}

export interface RegistryLookupInput {
  /** Company legal name as read from the uploaded document, if any. */
  companyName?: string | null
  /** Incorporation / registration number, if any (e.g. "BC1234567"). */
  incorporationNumber?: string | null
  /** Director names extracted from the uploaded document. */
  declaredDirectors: string[]
}

export interface RegistryLookupResult {
  status: RegistryMatchStatus
  /** True when the result came from the simulated fallback, not the live API. */
  simulated: boolean
  /** Registry's canonical company name, when found. */
  registryName?: string
  incorporationNumber?: string
  /** Registration status on the registry (e.g. "Active", "Historical"). */
  registrationState?: string
  /** Per-director match outcomes for the declared directors. */
  directors: RegistryDirector[]
  /** Directors present on the registry that were NOT declared in the document. */
  unlistedOnDocument?: string[]
  /** One plain-language sentence summarising the check for logs + UI. */
  summary: string
  checkedAt: string
}

const REGISTRY_BASE_URL =
  process.env.BC_REGISTRIES_API_URL ?? "https://api.bcregistry.gov.bc.ca/registry/v1"

/** Normalise a person name for tolerant comparison (case/space/punctuation). */
function normName(n: string): string {
  return n
    .toLowerCase()
    .replace(/\b(mr|mrs|ms|dr|the hon|hon)\b\.?/g, "")
    .replace(/[^a-z\s]/g, "")
    .replace(/\s+/g, " ")
    .trim()
}

/** Small seeded registry so the simulated path returns stable results. */
const SEED_REGISTRY: Record<
  string,
  { name: string; incorporationNumber: string; state: string; directors: { name: string; role: string }[] }
> = {
  "bc1234567": {
    name: "Cascade Hospitality Group Ltd.",
    incorporationNumber: "BC1234567",
    state: "Active",
    directors: [
      { name: "Sam Rivera", role: "Director" },
      { name: "Priya Anand", role: "Director" },
      { name: "Jordan Lee", role: "Officer" },
    ],
  },
  "cascade hospitality group ltd": {
    name: "Cascade Hospitality Group Ltd.",
    incorporationNumber: "BC1234567",
    state: "Active",
    directors: [
      { name: "Sam Rivera", role: "Director" },
      { name: "Priya Anand", role: "Director" },
      { name: "Jordan Lee", role: "Officer" },
    ],
  },
  "bc0987654": {
    name: "Harbourview Cannabis Co.",
    incorporationNumber: "BC0987654",
    state: "Active",
    directors: [
      { name: "Dana Whitecloud", role: "Director" },
      { name: "Marcus Tran", role: "Director" },
    ],
  },
}

/** Look up a business and confirm its declared directors. Never throws: on failure it returns an `unavailable` advisory so submission is not blocked. */
export async function lookupBusiness(input: RegistryLookupInput): Promise<RegistryLookupResult> {
  const apiKey = process.env.BC_REGISTRIES_API_KEY
  if (!apiKey) {
    return simulateLookup(input)
  }

  try {
    const identifier = (input.incorporationNumber || input.companyName || "").trim()
    const res = await fetch(
      `${REGISTRY_BASE_URL}/businesses/${encodeURIComponent(identifier)}/directors`,
      {
        headers: {
          "x-apikey": apiKey,
          Accept: "application/json",
        },
        // Keep the prototype snappy; a real integration may raise this.
        signal: AbortSignal.timeout(8000),
      },
    )
    if (res.status === 404) {
      return {
        status: "not-found",
        simulated: false,
        directors: input.declaredDirectors.map((name) => ({ name, matched: false })),
        summary: `No BC Registries record found for "${identifier}".`,
        checkedAt: new Date().toISOString(),
      }
    }
    if (!res.ok) throw new Error(`BC Registries responded ${res.status}`)
    const data = (await res.json()) as {
      business?: { legalName?: string; identifier?: string; state?: string }
      directors?: { officer?: { firstName?: string; lastName?: string }; roles?: { roleType?: string }[] }[]
    }
    const registryDirectors = (data.directors ?? []).map((d) => ({
      name: `${d.officer?.firstName ?? ""} ${d.officer?.lastName ?? ""}`.trim(),
      role: d.roles?.[0]?.roleType ?? "Director",
    }))
    return reconcile(input, {
      name: data.business?.legalName ?? input.companyName ?? "",
      incorporationNumber: data.business?.identifier ?? input.incorporationNumber ?? "",
      state: data.business?.state ?? "Active",
      directors: registryDirectors,
    }, false)
  } catch (err) {
    return {
      status: "unavailable",
      simulated: false,
      directors: input.declaredDirectors.map((name) => ({ name, matched: false })),
      summary:
        "BC Registries could not be reached to verify directors. Staff should confirm manually. This does not block submission.",
      checkedAt: new Date().toISOString(),
    }
  }
}

/** Deterministic, believable result used when no API key is configured. */
function simulateLookup(input: RegistryLookupInput): RegistryLookupResult {
  const key =
    (input.incorporationNumber && normName(input.incorporationNumber).replace(/\s/g, "")) ||
    (input.companyName && normName(input.companyName)) ||
    ""
  const record =
    SEED_REGISTRY[key] ||
    SEED_REGISTRY[(input.incorporationNumber ?? "").toLowerCase().replace(/\s/g, "")]

  if (!record) {
    // Unknown company: with directors, simulate "partial" (matched by name, directors unconfirmed), otherwise "not-found"; the demo stays advisory and never blocks.
    if (input.companyName || input.incorporationNumber) {
      return reconcile(
        input,
        {
          name: input.companyName ?? "Declared company",
          incorporationNumber: input.incorporationNumber ?? "Unknown",
          state: "Active",
          // Simulate the registry knowing the first declared director only.
          directors: input.declaredDirectors.slice(0, 1).map((name) => ({ name, role: "Director" })),
        },
        true,
      )
    }
    return {
      status: "not-found",
      simulated: true,
      directors: input.declaredDirectors.map((name) => ({ name, matched: false })),
      summary:
        "Simulated BC Registries lookup: no matching company could be identified from the document. Staff should confirm.",
      checkedAt: new Date().toISOString(),
    }
  }

  return reconcile(input, record, true)
}

/** Compare declared directors against a registry record and build the result. */
function reconcile(
  input: RegistryLookupInput,
  record: { name: string; incorporationNumber: string; state: string; directors: { name: string; role: string }[] },
  simulated: boolean,
): RegistryLookupResult {
  const registryNorm = record.directors.map((d) => ({ ...d, norm: normName(d.name) }))
  const directors: RegistryDirector[] = input.declaredDirectors.map((name) => {
    const hit = registryNorm.find((r) => r.norm === normName(name))
    return { name, matched: Boolean(hit), role: hit?.role }
  })
  const declaredNorm = new Set(input.declaredDirectors.map(normName))
  const unlistedOnDocument = record.directors.filter((r) => !declaredNorm.has(normName(r.name))).map((r) => r.name)

  const allMatched = directors.length > 0 && directors.every((d) => d.matched)
  const anyMatched = directors.some((d) => d.matched)
  const status: RegistryMatchStatus = allMatched ? "verified" : anyMatched ? "partial" : "partial"

  const sim = simulated ? "Simulated BC Registries check: " : "BC Registries check: "
  const summary = allMatched
    ? `${sim}all ${directors.length} declared director(s) match the active registry record for ${record.name} (${record.incorporationNumber}).`
    : `${sim}${directors.filter((d) => d.matched).length}/${directors.length} declared director(s) matched the registry record for ${record.name}. Staff should confirm the remainder.`

  return {
    status,
    simulated,
    registryName: record.name,
    incorporationNumber: record.incorporationNumber,
    registrationState: record.state,
    directors,
    unlistedOnDocument: unlistedOnDocument.length ? unlistedOnDocument : undefined,
    summary,
    checkedAt: new Date().toISOString(),
  }
}
