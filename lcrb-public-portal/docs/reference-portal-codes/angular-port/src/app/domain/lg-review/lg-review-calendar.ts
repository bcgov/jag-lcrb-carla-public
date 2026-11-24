"use client"

/** Comment-deadline calendar events and .ics export. TODO: feed live referral data for the jurisdiction. */

import { LG_STATUS_LABELS, type LgApplication, type LgReviewStatus } from "@domain/models/lg-review"
import { daysUntilDue } from "./due-dates"

/** Statuses that still require a comment back to the LCRB (i.e. open work). */
const OPEN_STATUSES: LgReviewStatus[] = ["pending-review", "documents-requested", "community-review"]

/** True when the application still needs a comment submitted. */
export function needsReview(app: LgApplication): boolean {
  return OPEN_STATUSES.includes(app.status)
}

/** Only the applications with an outstanding comment deadline, earliest first. */
export function openApplications(apps: LgApplication[]): LgApplication[] {
  return apps.filter(needsReview).sort((a, b) => a.commentDueDate.localeCompare(b.commentDueDate))
}


function icsDate(iso: string): string {
  return iso.replace(/-/g, "")
}

function shiftDay(iso: string, days: number): string {
  const d = new Date(iso + "T00:00:00")
  d.setDate(d.getDate() + days)
  return d.toISOString().slice(0, 10)
}

function icsStamp(): string {
  return new Date().toISOString().replace(/[-:]/g, "").split(".")[0] + "Z"
}

function icsEscape(text: string): string {
  return text.replace(/\\/g, "\\\\").replace(/;/g, "\\;").replace(/,/g, "\\,").replace(/\n/g, "\\n")
}

/** Serializes open applications' comment deadlines to an RFC 5545 VCALENDAR (all-day VEVENTs on `commentDueDate`, with a 7-day reminder). */
export function toICS(apps: LgApplication[]): string {
  const stamp = icsStamp()
  const lines: string[] = [
    "BEGIN:VCALENDAR",
    "VERSION:2.0",
    "PRODID:-//BC LCRB Portal//LG-IN Review Deadlines//EN",
    "CALSCALE:GREGORIAN",
    "METHOD:PUBLISH",
    "X-WR-CALNAME:LCRB Review Deadlines",
  ]
  for (const app of openApplications(apps)) {
    const start = icsDate(app.commentDueDate)
    const end = icsDate(shiftDay(app.commentDueDate, 1))
    const summary = `Comment due — ${app.establishmentName} (Job #${app.jobNumber})`
    const description = [
      `${app.applicationType} · ${app.licenceType}`,
      `Applicant: ${app.applicantName} (${app.businessName})`,
      `Status: ${LG_STATUS_LABELS[app.status]}`,
      `${app.address}, ${app.city}`,
    ].join("\n")
    lines.push(
      "BEGIN:VEVENT",
      `UID:lg-review-${app.jobNumber}@lcrb-portal`,
      `DTSTAMP:${stamp}`,
      `DTSTART;VALUE=DATE:${start}`,
      `DTEND;VALUE=DATE:${end}`,
      `SUMMARY:${icsEscape(summary)}`,
      `DESCRIPTION:${icsEscape(description)}`,
      "BEGIN:VALARM",
      "TRIGGER:-P7D",
      "ACTION:DISPLAY",
      `DESCRIPTION:${icsEscape(`Reminder: ${summary}`)}`,
      "END:VALARM",
      "END:VEVENT",
    )
  }
  lines.push("END:VCALENDAR")
  return lines.join("\r\n")
}

/** Count of open applications, and how many are overdue / due within 14 days. */
export function summarizeDeadlines(apps: LgApplication[]): { open: number; overdue: number; dueSoon: number } {
  const open = openApplications(apps)
  let overdue = 0
  let dueSoon = 0
  for (const app of open) {
    const days = daysUntilDue(app.commentDueDate)
    if (days < 0) overdue++
    else if (days <= 14) dueSoon++
  }
  return { open: open.length, overdue, dueSoon }
}
