"use client"

/** Scheduled Local Government / Indigenous Nation review-report delivery (prototype). */

import { createCollection, makeId } from "../mock-storage/kv-store"
import { type Entity } from "@domain/models/entity"
import { notify } from "../notifications/notification-service"
import { recordAudit } from "../activity/audit-service"
import { needsReview } from "@domain/lg-review/lg-review-calendar"
import { type LgApplication } from "@domain/models/lg-review"
import type { UserRole } from "@domain/roles"
import { type ReportCadence, type LgReportFilters } from "@domain/models/lg-review"
import { cadenceLabel, describeFilters } from "@domain/lg-review/report-schedule"

export interface LgReportSchedule extends Entity {
  owner: string
  email: string
  ownerName: string
  role: UserRole
  cadence: ReportCadence
  filters: LgReportFilters
  active: boolean
  createdAt: string
  lastRunAt?: string
  lastRunCount?: number
  lastRunFilename?: string
  nextRunAt: string
}

const schedules = createCollection<LgReportSchedule>("lg-report-schedules")


function isoDate(d: Date): string {
  return d.toISOString().slice(0, 10)
}

function addDays(d: Date, days: number): Date {
  const next = new Date(d)
  next.setDate(next.getDate() + days)
  return next
}

/** The deadline window a cadence covers: everything due on or before the horizon (daily → 1 day out, weekly → 7 days out). */
export function computeDeadlineHorizon(cadence: ReportCadence, reference: Date = new Date()): string {
  return isoDate(addDays(reference, cadence === "daily" ? 1 : 7))
}

export function computeNextRun(cadence: ReportCadence, reference: Date = new Date()): string {
  const next = new Date(reference)
  next.setHours(7, 0, 0, 0)
  if (cadence === "daily") {
    if (next <= reference) next.setDate(next.getDate() + 1)
  } else {
    do {
      next.setDate(next.getDate() + 1)
    } while (next.getDay() !== 1 || next <= reference)
  }
  return next.toISOString()
}

/** Filters applications down to those a schedule run should include: still awaiting comment */
export function filterApplicationsForSchedule(
  applications: LgApplication[],
  schedule: Pick<LgReportSchedule, "cadence" | "filters">,
  reference: Date = new Date(),
): { horizon: string; applications: LgApplication[] } {
  const horizon = computeDeadlineHorizon(schedule.cadence, reference)
  const q = schedule.filters.query.trim().toLowerCase()
  const matched = applications
    .filter((a) => {
      if (!needsReview(a)) return false
      if (a.commentDueDate > horizon) return false
      if (schedule.filters.status !== "all" && a.status !== schedule.filters.status) return false
      if (q) {
        return [a.jobNumber, a.establishmentName, a.applicantName, a.businessName, a.address, a.city, a.licenceType, a.applicationType]
          .join(" ")
          .toLowerCase()
          .includes(q)
      }
      return true
    })
    .sort((a, b) => a.commentDueDate.localeCompare(b.commentDueDate))
  return { horizon, applications: matched }
}


export interface CreateScheduleInput {
  owner: string
  email: string
  ownerName: string
  role: UserRole
  cadence: ReportCadence
  filters: LgReportFilters
}

export function listSchedules(owner: string): LgReportSchedule[] {
  return schedules
    .list()
    .filter((s) => s.owner === owner)
    .sort((a, b) => (a.createdAt < b.createdAt ? 1 : -1))
}

export function createSchedule(input: CreateScheduleInput): LgReportSchedule {
  const now = new Date()
  const record: LgReportSchedule = {
    id: makeId("lgsched"),
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
    action: "lg-report-schedule.created",
    summary: `${cadenceLabel(record.cadence)} review-deadline report scheduled to ${record.email}`,
    actor: input.ownerName,
    actorRole: "local-government",
    meta: { cadence: record.cadence, filters: describeFilters(record.filters) },
  })
  return record
}

export function setScheduleActive(id: string, active: boolean): void {
  const existing = schedules.get(id)
  if (!existing) return
  const patch: Partial<LgReportSchedule> = { active }
  if (active) patch.nextRunAt = computeNextRun(existing.cadence)
  schedules.update(id, patch)
  recordAudit({
    category: "notification",
    action: active ? "lg-report-schedule.resumed" : "lg-report-schedule.paused",
    summary: `${cadenceLabel(existing.cadence)} review-deadline report ${active ? "resumed" : "paused"} for ${existing.email}`,
    actor: existing.ownerName,
    actorRole: "local-government",
  })
}

export function deleteSchedule(id: string): void {
  const existing = schedules.get(id)
  schedules.remove(id)
  if (existing) {
    recordAudit({
      category: "notification",
      action: "lg-report-schedule.deleted",
      summary: `${cadenceLabel(existing.cadence)} review-deadline report unsubscribed for ${existing.email}`,
      actor: existing.ownerName,
      actorRole: "local-government",
    })
  }
}


export interface RunDetails {
  horizon: string
  count: number
  filename: string
}

export function recordScheduleRun(
  schedule: LgReportSchedule,
  details: RunDetails,
  opts: { automatic?: boolean } = {},
): void {
  const trigger = opts.automatic ? "Scheduled" : "On-demand"
  notify({
    recipient: schedule.role,
    category: "reminder",
    title: `${cadenceLabel(schedule.cadence)} review-deadline report`,
    body: `${trigger} delivery: ${details.count} application${details.count === 1 ? "" : "s"} due by ${details.horizon}. PDF attached: ${details.filename}.`,
    href: "/local-government/queue",
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

export function getDueSchedules(owner: string, reference: Date = new Date()): LgReportSchedule[] {
  return listSchedules(owner).filter((s) => s.active && new Date(s.nextRunAt) <= reference)
}

export const lgReportScheduleCollection = schedules
