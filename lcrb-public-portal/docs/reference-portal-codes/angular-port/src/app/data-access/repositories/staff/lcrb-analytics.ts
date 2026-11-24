"use client"

/** LCRB staff intake analytics. */

import { listSyncRecords, seedSampleCasesIfEmpty, type CaseSyncRecord } from "../activity/dynamics-service"
import { QUEUE_META, confidenceBand } from "@domain/staff/agent-log"
import { type DynamicsQueue, type CheckOutcome, type AgentCheckKind } from "@domain/models/staff"

const ALL_QUEUES: DynamicsQueue[] = [
  "beyond-licence-cap",
  "section-119-review",
  "enhanced-review",
  "standard-processing",
]

export interface QueueDatum {
  queue: DynamicsQueue
  label: string
  count: number
  tone: (typeof QUEUE_META)[DynamicsQueue]["tone"]
}

export interface ConfidenceDatum {
  band: "high" | "medium" | "low"
  label: string
  count: number
}

export interface CheckOutcomeDatum {
  kind: AgentCheckKind
  label: string
  clear: number
  attention: number
  flag: number
}

export interface IntakeAnalytics {
  total: number
  syncedCount: number
  failedCount: number
  pendingCount: number
  /** Percent of cases synced successfully to Dynamics. */
  syncSuccessRate: number
  /** Cases the agent flagged for mandatory human review. */
  requiresReviewCount: number
  /** Percent of cases auto-cleared to standard processing. */
  autoRouteRate: number
  /** Mean overall agent confidence across cases with a log. */
  avgConfidence: number
  byQueue: QueueDatum[]
  byConfidence: ConfidenceDatum[]
  byCheckOutcome: CheckOutcomeDatum[]
}

const CHECK_LABELS: Record<AgentCheckKind, string> = {
  "licence-cap": "Licence cap",
  "section-119": "Section 119",
  consistency: "Consistency",
  completeness: "Completeness",
  "tied-house": "Tied-house",
  "fit-and-proper": "Fit & proper",
}

const CONFIDENCE_LABELS: Record<"high" | "medium" | "low", string> = {
  high: "High (80+)",
  medium: "Medium (55–79)",
  low: "Low (<55)",
}

/** Ensure the queues are demonstrable, then read every synced case. */
export function loadCases(): CaseSyncRecord[] {
  seedSampleCasesIfEmpty()
  return listSyncRecords()
}

export function computeIntakeAnalytics(cases: CaseSyncRecord[]): IntakeAnalytics {
  const total = cases.length
  const syncedCount = cases.filter((c) => c.state === "synced").length
  const failedCount = cases.filter((c) => c.state === "failed").length
  const pendingCount = cases.filter((c) => c.state === "pending").length

  const withLog = cases.filter((c) => c.agentLog)
  const requiresReviewCount = withLog.filter((c) => c.agentLog?.requiresHumanReview).length
  const standardCount = cases.filter(
    (c) => (c.queue ?? "standard-processing") === "standard-processing",
  ).length
  const avgConfidence =
    withLog.length === 0
      ? 0
      : Math.round(
          withLog.reduce((sum, c) => sum + (c.agentLog?.overallConfidence ?? 0), 0) / withLog.length,
        )

  // By queue
  const queueCountMap: Record<DynamicsQueue, number> = {
    "beyond-licence-cap": 0,
    "section-119-review": 0,
    "enhanced-review": 0,
    "standard-processing": 0,
  }
  for (const c of cases) queueCountMap[c.queue ?? "standard-processing"]++
  const byQueue: QueueDatum[] = ALL_QUEUES.map((queue) => ({
    queue,
    label: QUEUE_META[queue].label,
    count: queueCountMap[queue],
    tone: QUEUE_META[queue].tone,
  }))

  // By confidence band
  const bandMap: Record<"high" | "medium" | "low", number> = { high: 0, medium: 0, low: 0 }
  for (const c of withLog) {
    bandMap[confidenceBand(c.agentLog!.overallConfidence)]++
  }
  const byConfidence: ConfidenceDatum[] = (["high", "medium", "low"] as const).map((band) => ({
    band,
    label: CONFIDENCE_LABELS[band],
    count: bandMap[band],
  }))

  // By check outcome (stacked)
  const checkMap = new Map<AgentCheckKind, { clear: number; attention: number; flag: number }>()
  for (const c of withLog) {
    for (const check of c.agentLog!.checks) {
      const entry = checkMap.get(check.kind) ?? { clear: 0, attention: 0, flag: 0 }
      entry[check.outcome as CheckOutcome]++
      checkMap.set(check.kind, entry)
    }
  }
  const byCheckOutcome: CheckOutcomeDatum[] = Array.from(checkMap.entries()).map(([kind, v]) => ({
    kind,
    label: CHECK_LABELS[kind],
    ...v,
  }))

  return {
    total,
    syncedCount,
    failedCount,
    pendingCount,
    syncSuccessRate: total === 0 ? 0 : Math.round((syncedCount / total) * 100),
    requiresReviewCount,
    autoRouteRate: total === 0 ? 0 : Math.round((standardCount / total) * 100),
    avgConfidence,
    byQueue,
    byConfidence,
    byCheckOutcome,
  }
}
