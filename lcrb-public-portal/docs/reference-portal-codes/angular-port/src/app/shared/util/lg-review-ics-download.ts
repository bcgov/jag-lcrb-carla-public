/** Browser download of the LG review deadlines as an .ics file, kept apart so the domain calendar file touches no browser APIs. */
import type { LgApplication } from "@domain/models/lg-review"
import { toICS } from "@domain/lg-review/lg-review-calendar"

/** Trigger a browser download of the open applications as an .ics file. */
export function downloadLgReviewICS(apps: LgApplication[], filename = "lcrb-review-deadlines.ics"): void {
  const blob = new Blob([toICS(apps)], { type: "text/calendar;charset=utf-8" })
  const url = URL.createObjectURL(blob)
  const a = document.createElement("a")
  a.href = url
  a.download = filename
  document.body.appendChild(a)
  a.click()
  document.body.removeChild(a)
  URL.revokeObjectURL(url)
}
