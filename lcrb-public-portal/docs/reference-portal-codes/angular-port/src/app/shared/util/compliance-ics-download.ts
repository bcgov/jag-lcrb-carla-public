import { type CalendarEvent } from "@domain/models/compliance"
import { toICS } from "@domain/compliance/compliance-calendar"

/** Trigger a browser download of the given events as an .ics file. */
export function downloadICS(events: CalendarEvent[], filename = "lcrb-compliance-calendar.ics"): void {
  const blob = new Blob([toICS(events)], { type: "text/calendar;charset=utf-8" })
  const url = URL.createObjectURL(blob)
  const a = document.createElement("a")
  a.href = url
  a.download = filename
  document.body.appendChild(a)
  a.click()
  document.body.removeChild(a)
  URL.revokeObjectURL(url)
}
