/** Processing times per licence type from elapsed days of closed applications. TODO: replace `CLOSED_APPLICATION_SAMPLES` with a Dynamics query; keep the returned shapes. */

import { LICENCE_TYPE_LABELS, type LicenceType } from "@domain/models/applications"

/** One closed application, as fed back from Dynamics on approval/close. */
export interface ClosedApplicationFact {
  licenceType: LicenceType
  /** Calendar days from submission to decision. */
  elapsedDays: number
  /** ISO date the decision was communicated back to the portal. */
  decidedOn: string
}

export interface ProcessingTimeDatum {
  licenceType: LicenceType
  label: string
  /** Median calendar days to a decision over the trailing window. */
  medianDays: number
  /** 90th-percentile days, "most applications completed within". */
  p90Days: number
  /** Number of closed applications behind the estimate. */
  sampleSize: number
  /** Direction vs. the previous window: faster, slower, or steady. */
  trend: "faster" | "slower" | "steady"
}

export interface ProcessingTimesResult {
  /** Trailing window the estimates are based on, in days. */
  windowDays: number
  /** ISO timestamp the analytics were computed. */
  updatedAt: string
  /** Portal-wide median across all licence types. */
  overallMedianDays: number
  byLicenceType: ProcessingTimeDatum[]
}

// Deterministic sample fact table, hand-tuned per licence type (cannabis and manufacturer take longer, common authorizations are quick).

function daysAgo(n: number): string {
  return new Date(Date.now() - n * 86_400_000).toISOString().slice(0, 10)
}

/** Generate a spread of elapsed-day samples around a center for one type. */
function samples(
  licenceType: LicenceType,
  center: number,
  spread: number,
  count: number,
): ClosedApplicationFact[] {
  const out: ClosedApplicationFact[] = []
  for (let i = 0; i < count; i++) {
    // Deterministic pseudo-spread: symmetric offsets scaled by index.
    const offset = Math.round((((i * 37) % (spread * 2 + 1)) - spread))
    out.push({
      licenceType,
      elapsedDays: Math.max(1, center + offset),
      decidedOn: daysAgo((i * 83) % 180),
    })
  }
  return out
}

const CLOSED_APPLICATION_SAMPLES: ClosedApplicationFact[] = [
  ...samples("common", 12, 6, 40),
  ...samples("food-primary", 34, 12, 36),
  ...samples("liquor-primary", 62, 20, 30),
  ...samples("cannabis-retail-store", 95, 28, 24),
  ...samples("licensee-retail-store", 58, 18, 20),
  ...samples("manufacturer", 88, 26, 18),
  ...samples("deemed-manufacturer", 45, 14, 12),
  ...samples("agent", 28, 10, 16),
  ...samples("catering", 18, 8, 22),
  ...samples("ubrew-uvin", 40, 12, 10),
]

function percentile(sorted: number[], p: number): number {
  if (sorted.length === 0) return 0
  const idx = Math.min(sorted.length - 1, Math.floor((p / 100) * sorted.length))
  return sorted[idx]
}

const WINDOW_DAYS = 180

/** Computes current processing times per licence type from the closed-application facts. */
export function computeProcessingTimes(
  facts: ClosedApplicationFact[] = CLOSED_APPLICATION_SAMPLES,
): ProcessingTimesResult {
  const byType = new Map<LicenceType, number[]>()
  for (const f of facts) {
    const arr = byType.get(f.licenceType) ?? []
    arr.push(f.elapsedDays)
    byType.set(f.licenceType, arr)
  }

  const byLicenceType: ProcessingTimeDatum[] = []
  for (const [licenceType, daysList] of byType) {
    const sorted = [...daysList].sort((a, b) => a - b)
    const median = percentile(sorted, 50)
    const p90 = percentile(sorted, 90)

    // Trend: compare the most-recent third of decisions to the oldest third.
    const recent = daysList.slice(0, Math.max(1, Math.floor(daysList.length / 3)))
    const older = daysList.slice(-Math.max(1, Math.floor(daysList.length / 3)))
    const avg = (xs: number[]) => xs.reduce((s, x) => s + x, 0) / xs.length
    const delta = avg(recent) - avg(older)
    const trend: ProcessingTimeDatum["trend"] =
      delta <= -3 ? "faster" : delta >= 3 ? "slower" : "steady"

    byLicenceType.push({
      licenceType,
      label: LICENCE_TYPE_LABELS[licenceType],
      medianDays: median,
      p90Days: p90,
      sampleSize: daysList.length,
      trend,
    })
  }

  const allDays = facts.map((f) => f.elapsedDays).sort((a, b) => a - b)

  return {
    windowDays: WINDOW_DAYS,
    updatedAt: new Date().toISOString(),
    overallMedianDays: percentile(allDays, 50),
    byLicenceType,
  }
}

/** Convenience: the processing time for a single licence type, if known. */
export function getProcessingTimeFor(
  licenceType: LicenceType,
  result: ProcessingTimesResult = computeProcessingTimes(),
): ProcessingTimeDatum | undefined {
  return result.byLicenceType.find((d) => d.licenceType === licenceType)
}
