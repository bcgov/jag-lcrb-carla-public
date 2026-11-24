/** LG review due-date helper. */

/** Days remaining until comment is due (negative = overdue). */
export function daysUntilDue(iso: string): number {
  const due = new Date(iso + "T00:00:00").getTime()
  const now = Date.now()
  return Math.ceil((due - now) / (1000 * 60 * 60 * 24))
}

export function formatLgDate(iso: string): string {
  const d = new Date(iso + "T00:00:00")
  return d.toLocaleDateString("en-CA", { year: "numeric", month: "short", day: "numeric" })
}
