import { daysUntil } from "./enforcement"
import { type CalendarEventType, type EventUrgency, type CalendarEvent } from "@domain/models/compliance"

export const EVENT_META: Record<
  CalendarEventType,
  { label: string; dot: string; chip: string }
> = {
  renewal: {
    label: "Renewal",
    dot: "bg-[#013366]",
    chip: "bg-blue-100 text-blue-800 border-blue-200",
  },
  fee: {
    label: "Annual fee",
    dot: "bg-[#2E8540]",
    chip: "bg-green-100 text-green-800 border-green-200",
  },
  "enforcement-deadline": {
    label: "Enforcement response",
    dot: "bg-[#D8292F]",
    chip: "bg-red-100 text-red-800 border-red-200",
  },
  hearing: {
    label: "Hearing",
    dot: "bg-[#F5A623]",
    chip: "bg-orange-100 text-orange-800 border-orange-200",
  },
  inspection: {
    label: "Inspection",
    dot: "bg-[#38598A]",
    chip: "bg-blue-100 text-blue-800 border-blue-200",
  },
}

export function urgencyFor(iso: string): EventUrgency {
  const d = daysUntil(iso)
  if (d < 0) return "overdue"
  if (d <= 14) return "urgent"
  if (d <= 45) return "soon"
  return "upcoming"
}

export const URGENCY_META: Record<
  EventUrgency,
  { label: string; text: string; row: string }
> = {
  overdue: { label: "Overdue", text: "text-red-700", row: "border-l-[#D8292F]" },
  urgent: { label: "Due soon", text: "text-orange-700", row: "border-l-[#F5A623]" },
  soon: { label: "Approaching", text: "text-yellow-700", row: "border-l-yellow-400" },
  upcoming: { label: "Upcoming", text: "text-muted-foreground", row: "border-l-border" },
}

/** Group events by "Month YYYY" preserving chronological order. */
export function groupByMonth(events: CalendarEvent[]): { key: string; label: string; events: CalendarEvent[] }[] {
  const groups: { key: string; label: string; events: CalendarEvent[] }[] = []
  for (const ev of events) {
    const d = new Date(ev.date)
    const key = `${d.getFullYear()}-${d.getMonth()}`
    const label = d.toLocaleDateString("en-CA", { month: "long", year: "numeric" })
    let group = groups.find((g) => g.key === key)
    if (!group) {
      group = { key, label, events: [] }
      groups.push(group)
    }
    group.events.push(ev)
  }
  return groups
}

export function shiftDays(iso: string, days: number): string {
  const d = new Date(iso)
  d.setDate(d.getDate() + days)
  return d.toISOString()
}


function icsDate(iso: string): string {
  const d = new Date(iso)
  const y = d.getFullYear()
  const m = String(d.getMonth() + 1).padStart(2, "0")
  const day = String(d.getDate()).padStart(2, "0")
  return `${y}${m}${day}`
}

function icsStamp(): string {
  return new Date().toISOString().replace(/[-:]/g, "").split(".")[0] + "Z"
}

/** Escape reserved characters per RFC 5545. */
function icsEscape(text: string): string {
  return text.replace(/\\/g, "\\\\").replace(/;/g, "\\;").replace(/,/g, "\\,").replace(/\n/g, "\\n")
}

/** Serialize events to an RFC 5545 VCALENDAR string (all-day VEVENTs). */
export function toICS(events: CalendarEvent[]): string {
  const stamp = icsStamp()
  const lines: string[] = [
    "BEGIN:VCALENDAR",
    "VERSION:2.0",
    "PRODID:-//BC LCRB Portal//Compliance Calendar//EN",
    "CALSCALE:GREGORIAN",
    "METHOD:PUBLISH",
    "X-WR-CALNAME:LCRB Compliance & Renewals",
  ]
  for (const ev of events) {
    const start = icsDate(ev.date)
    const end = icsDate(shiftDays(ev.date, 1))
    lines.push(
      "BEGIN:VEVENT",
      `UID:${ev.id}@lcrb-portal`,
      `DTSTAMP:${stamp}`,
      `DTSTART;VALUE=DATE:${start}`,
      `DTEND;VALUE=DATE:${end}`,
      `SUMMARY:${icsEscape(`[${EVENT_META[ev.type].label}] ${ev.title}`)}`,
      `DESCRIPTION:${icsEscape(ev.description)}`,
      "BEGIN:VALARM",
      "TRIGGER:-P7D",
      "ACTION:DISPLAY",
      `DESCRIPTION:${icsEscape(`Reminder: ${ev.title}`)}`,
      "END:VALARM",
      "END:VEVENT",
    )
  }
  lines.push("END:VCALENDAR")
  // RFC 5545 requires CRLF line endings.
  return lines.join("\r\n")
}
