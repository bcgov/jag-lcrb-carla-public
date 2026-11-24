"use client"

/** Scheduled SEP report delivery (prototype). */

import { createCollection, makeId } from "../mock-storage/kv-store"
import { type Entity } from "@domain/models/entity"
import { notify } from "../notifications/notification-service"
import { recordAudit } from "../activity/audit-service"
import { type SepRecord } from "@domain/models/special-events"
import type { UserRole } from "@domain/roles"
import { type ReportCadence, type ReportScheduleFilters } from "@domain/models/police-review"
import { cadenceLabel, describeFilters } from "@domain/police-review/report-schedule"

export interface ReportSchedule extends Entity {
  /** Stable owner key - the account email in the prototype, user id in prod. */
  owner: string
  /** Destination email (the logged-in user's account address). */
  email: string
  /** Display name for audit / notification copy. */
  ownerName: string
  /** Role of the subscriber (used to route the in-portal notice). */
  role: UserRole
  cadence: ReportCadence
  filters: ReportScheduleFilters
  active: boolean
  createdAt: string
  /** ISO timestamp of the last simulated send, if any. */
  lastRunAt?: string
  /** Permit count included in the last send. */
  lastRunCount?: number
  /** Filename of the last generated PDF. */
  lastRunFilename?: string
  /** ISO timestamp when the next automatic send is due. */
  nextRunAt: string
}

const schedules = createCollection<ReportSchedule>("sep-report-schedules")


function isoDate(d: Date): string {
  return d.toISOString().slice(0, 10)
}

function addDays(d: Date, days: number): Date {
  const next = new Date(d)
  next.setDate(next.getDate() + days)
  return next
}

/** The reporting window a cadence covers, relative to a reference day (default today). */
export function computeReportRange(
  cadence: ReportCadence,
  reference: Date = new Date(),
): { fromDate: string; toDate: string } {
  const start = addDays(reference, 1)
  const end = cadence === "daily" ? start : addDays(start, 6)
  return { fromDate: isoDate(start), toDate: isoDate(end) }
}

/** When the next automatic run is due. */
export function computeNextRun(cadence: ReportCadence, reference: Date = new Date()): string {
  const next = new Date(reference)
  next.setHours(7, 0, 0, 0)
  if (cadence === "daily") {
    if (next <= reference) next.setDate(next.getDate() + 1)
  } else {
    // Advance to the next Monday (day 1).
    do {
      next.setDate(next.getDate() + 1)
    } while (next.getDay() !== 1 || next <= reference)
  }
  return next.toISOString()
}

/** Filters SEP records to those a schedule run should include, sorted by event start. */
export function filterRecordsForSchedule(
  records: SepRecord[],
  schedule: Pick<ReportSchedule, "cadence" | "filters">,
  reference: Date = new Date(),
): { fromDate: string; toDate: string; records: SepRecord[] } {
  const { fromDate, toDate } = computeReportRange(schedule.cadence, reference)
  const q = schedule.filters.query.trim().toLowerCase()
  const matched = records
    .filter((r) => {
      // Overlap test on ISO date strings (lexicographic works for yyyy-mm-dd).
      if (!(r.eventStart <= toDate && r.eventEnd >= fromDate)) return false
      if (schedule.filters.status !== "all" && r.status !== schedule.filters.status) return false
      if (schedule.filters.city !== "all" && r.city !== schedule.filters.city) return false
      if (q) {
        return [r.eventName, r.applicantName, r.venueName, r.city, r.jurisdiction]
          .join(" ")
          .toLowerCase()
          .includes(q)
      }
      return true
    })
    .sort((a, b) => a.eventStart.localeCompare(b.eventStart) || a.startTime.localeCompare(b.startTime))
  return { fromDate, toDate, records: matched }
}


export interface CreateScheduleInput {
  owner: string
  email: string
  ownerName: string
  role: UserRole
  cadence: ReportCadence
  filters: ReportScheduleFilters
}

export function listSchedules(owner: string): ReportSchedule[] {
  return schedules
    .list()
    .filter((s) => s.owner === owner)
    .sort((a, b) => (a.createdAt < b.createdAt ? 1 : -1))
}

export function createSchedule(input: CreateScheduleInput): ReportSchedule {
  const now = new Date()
  const record: ReportSchedule = {
    id: makeId("sched"),
    owner: input.owner,
    email: input.email,
    ownerName: input.ownerName,
    role: input.role,
    cadence: input.cadence,
    filters: input.filters,
    active: true,
    createdAt: now.toISOString(),
    nextRunAt: computeNextRun(input.cadence, now),
  }
  schedules.upsert(record)
  recordAudit({
    category: "notification",
    action: "report-schedule.created",
    summary: `${cadenceLabel(record.cadence)} SEP report scheduled to ${record.email}`,
    actor: input.ownerName,
    meta: { cadence: record.cadence, filters: describeFilters(record.filters) },
  })
  return record
}

export function setScheduleActive(id: string, active: boolean): void {
  const existing = schedules.get(id)
  if (!existing) return
  const patch: Partial<ReportSchedule> = { active }
  // Re-arm the next run when re-enabling so it doesn't fire immediately.
  if (active) patch.nextRunAt = computeNextRun(existing.cadence)
  schedules.update(id, patch)
  recordAudit({
    category: "notification",
    action: active ? "report-schedule.resumed" : "report-schedule.paused",
    summary: `${cadenceLabel(existing.cadence)} SEP report ${active ? "resumed" : "paused"} for ${existing.email}`,
    actor: existing.ownerName,
  })
}

export function deleteSchedule(id: string): void {
  const existing = schedules.get(id)
  schedules.remove(id)
  if (existing) {
    recordAudit({
      category: "notification",
      action: "report-schedule.deleted",
      summary: `${cadenceLabel(existing.cadence)} SEP report unsubscribed for ${existing.email}`,
      actor: existing.ownerName,
    })
  }
}


export interface RunDetails {
  fromDate: string
  toDate: string
  count: number
  filename: string
}

/** Records a simulated delivery of a schedule's report: an email entry in the delivery log plus an in-portal notice */
export function recordScheduleRun(
  schedule: ReportSchedule,
  details: RunDetails,
  opts: { automatic?: boolean } = {},
): void {
  const rangeLabel =
    details.fromDate === details.toDate ? details.fromDate : `${details.fromDate} to ${details.toDate}`
  const trigger = opts.automatic ? "Scheduled" : "On-demand"
  notify({
    recipient: schedule.role,
    category: "reminder",
    title: `${cadenceLabel(schedule.cadence)} Special Event Permit report`,
    body: `${trigger} delivery: ${details.count} permit${details.count === 1 ? "" : "s"} for ${rangeLabel}. PDF attached: ${details.filename}.`,
    href: "/police/events",
    channels: ["in-portal", "email"],
    email: schedule.email,
  })
  schedules.update(schedule.id, {
    lastRunAt: new Date().toISOString(),
    lastRunCount: details.count,
    lastRunFilename: details.filename,
    nextRunAt: computeNextRun(schedule.cadence),
  })
}

/** Returns the active schedules for an owner whose `nextRunAt` is in the past - i.e. automatic deliveries that are now due. */
export function getDueSchedules(owner: string, reference: Date = new Date()): ReportSchedule[] {
  return listSchedules(owner).filter((s) => s.active && new Date(s.nextRunAt) <= reference)
}

export const reportScheduleCollection = schedules
