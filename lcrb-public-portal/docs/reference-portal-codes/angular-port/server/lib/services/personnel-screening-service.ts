/** Fit-and-proper cross-check (prototype): in-memory seed data, advisory only, never throws. TODO: replace `SCREENING_RECORDS`/`ADVERSE_FINDINGS` with Dynamics. */

export type ScreeningRecordStatus = "cleared" | "in-progress" | "not-started" | "flagged"

export interface PersonScreeningResult {
  name: string
  /** Whether we found any screening record for this person. */
  found: boolean
  status: ScreeningRecordStatus
  /** Short note for staff (e.g. "PHS synced 2026-03-15", "no record on file"). */
  note: string
}

export interface FitAndProperResult {
  simulated: boolean
  people: PersonScreeningResult[]
  /** Count of people with no screening on file yet. */
  outstanding: number
  /** Count of people whose prior screening raised a flag for review. */
  flagged: number
  /** One plain-language sentence for logs + UI. */
  summary: string
  checkedAt: string
}

function normName(n: string): string {
  return n
    .toLowerCase()
    .replace(/[^a-z\s]/g, "")
    .replace(/\s+/g, " ")
    .trim()
}

/** Mirror of the portal's security-screening seed (screening-store.ts), keyed by normalised name; in production this is a Dynamics query. */
const SCREENING_RECORDS: Record<string, { status: ScreeningRecordStatus; note: string }> = {
  "john smith": { status: "cleared", note: "PHS completed and synced to Dynamics (LCRB-482910)." },
  "sam rivera": { status: "cleared", note: "PHS cleared; fit-and-proper on file." },
  "priya anand": { status: "in-progress", note: "PHS submitted, LCRB review in progress." },
  "maria chan": { status: "not-started", note: "Listed as shareholder (25%+); screening not started." },
  "jane doe": { status: "in-progress", note: "Screening invitation sent; PHS not yet returned." },
  "raj patel": { status: "not-started", note: "Key personnel; screening not started." },
}

/** Names with an adverse prior finding that should be flagged for staff review (demo data only). */
const ADVERSE_FINDINGS: Record<string, string> = {
  "jordan lee": "Prior enforcement association on record — recommend manual fit-and-proper review.",
}

/** Cross-check declared people against screening and fit-and-proper records; synchronous and deterministic. */
export function checkFitAndProper(names: string[]): FitAndProperResult {
  const unique = Array.from(new Set(names.map((n) => n.trim()).filter(Boolean)))
  const people: PersonScreeningResult[] = unique.map((name) => {
    const key = normName(name)
    const adverse = ADVERSE_FINDINGS[key]
    if (adverse) {
      return { name, found: true, status: "flagged", note: adverse }
    }
    const rec = SCREENING_RECORDS[key]
    if (rec) return { name, found: true, status: rec.status, note: rec.note }
    return {
      name,
      found: false,
      status: "not-started",
      note: "No security-screening record on file yet — this person will need to complete a Personal History Summary.",
    }
  })

  const outstanding = people.filter((p) => p.status === "not-started").length
  const flagged = people.filter((p) => p.status === "flagged").length

  let summary: string
  if (people.length === 0) {
    summary = "No individuals were extracted to screen."
  } else if (flagged > 0) {
    summary = `Simulated fit-and-proper check: ${flagged} of ${people.length} associate(s) have a prior finding that needs manual review; ${outstanding} still need screening.`
  } else if (outstanding > 0) {
    summary = `Simulated fit-and-proper check: ${outstanding} of ${people.length} associate(s) have no screening on file yet. Staff should ensure a Personal History Summary is completed.`
  } else {
    summary = `Simulated fit-and-proper check: all ${people.length} associate(s) have screening records on file.`
  }

  return {
    simulated: true,
    people,
    outstanding,
    flagged,
    summary,
    checkedAt: new Date().toISOString(),
  }
}
