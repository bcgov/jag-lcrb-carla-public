/** Client-side PDF of police SEP permits active on a day or across a date range; jsPDF is imported dynamically so it only loads on demand. */

import { SEP_STATUS_LABELS } from "@domain/special-events/seps"
import { type SepRecord } from "@domain/models/special-events"

export interface SepDailyReportOptions {
  /** ISO date (yyyy-mm-dd) the report covers. */
  reportDate: string
  /** SEPs already filtered to that day + any active filters. */
  records: SepRecord[]
  /** Human-readable description of the active filters, shown in the header. */
  filterSummary: string
}

export interface SepReportOptions {
  /** Inclusive ISO start date (yyyy-mm-dd). */
  fromDate: string
  /** Inclusive ISO end date (yyyy-mm-dd). Equal to `fromDate` for a single day. */
  toDate: string
  /** SEPs already filtered to the range + any active filters. */
  records: SepRecord[]
  /** Human-readable description of the active filters, shown in the header. */
  filterSummary: string
  /** "save" (default) downloads; "blob" resolves with the PDF Blob (simulates an email attachment). */
  output?: "save" | "blob"
  /** Override the download / attachment filename (without extension). */
  fileBaseName?: string
}

export interface SepReportResult {
  /** The generated filename (with .pdf extension). */
  filename: string
  /** Present when `output: "blob"` was requested. */
  blob?: Blob
  /** Number of permits included. */
  count: number
}

function formatLongDate(iso: string): string {
  return new Date(iso + "T00:00:00").toLocaleDateString("en-CA", {
    weekday: "long",
    year: "numeric",
    month: "long",
    day: "numeric",
  })
}

function formatShortDate(iso: string): string {
  return new Date(iso + "T00:00:00").toLocaleDateString("en-CA", {
    year: "numeric",
    month: "long",
    day: "numeric",
  })
}

/** Renderer shared by the single-day and range reports; a range report titles itself as one and shows each event's date. */
async function renderReport(opts: SepReportOptions): Promise<SepReportResult> {
  const { fromDate, toDate, records, filterSummary, output = "save", fileBaseName } = opts
  const isRange = fromDate !== toDate

  const { default: jsPDF } = await import("jspdf")
  const { default: autoTable } = await import("jspdf-autotable")

  const doc = new jsPDF({ orientation: "landscape", unit: "pt", format: "letter" })
  const pageWidth = doc.internal.pageSize.getWidth()
  const marginX = 40
  const navy = [1, 51, 102] as const

  // Header band
  doc.setFillColor(navy[0], navy[1], navy[2])
  doc.rect(0, 0, pageWidth, 74, "F")
  doc.setTextColor(255, 255, 255)
  doc.setFont("helvetica", "bold")
  doc.setFontSize(16)
  doc.text(
    isRange ? "Special Event Permits — Range Report" : "Special Event Permits — Daily Report",
    marginX,
    32,
  )
  doc.setFont("helvetica", "normal")
  doc.setFontSize(11)
  doc.text(
    isRange ? `${formatShortDate(fromDate)}  to  ${formatShortDate(toDate)}` : formatLongDate(fromDate),
    marginX,
    52,
  )

  // Meta line
  doc.setTextColor(90, 90, 90)
  doc.setFontSize(9)
  const generated = new Date().toLocaleString("en-CA", {
    dateStyle: "medium",
    timeStyle: "short",
  })
  doc.text(`Filters: ${filterSummary}`, marginX, 92)
  doc.text(
    `${records.length} permit${records.length === 1 ? "" : "s"}  ·  Generated ${generated}`,
    marginX,
    106,
  )

  if (records.length === 0) {
    doc.setTextColor(120, 120, 120)
    doc.setFontSize(12)
    doc.text(
      isRange
        ? "No Special Event Permits match the selected range and filters."
        : "No Special Event Permits match the selected day and filters.",
      marginX,
      150,
    )
  } else {
    // A "Date" column is only meaningful for multi-day ranges.
    const head = isRange
      ? [["Date", "Event", "Time", "Venue", "City", "Attendance", "Service", "Status"]]
      : [["Event", "Time", "Venue", "City", "Attendance", "Service", "Status"]]
    const body = records.map((r) => {
      const base = [
        r.eventName,
        `${r.startTime}–${r.endTime}`,
        r.venueName,
        r.city,
        r.expectedAttendance.toLocaleString(),
        r.serviceType,
        SEP_STATUS_LABELS[r.status],
      ]
      if (isRange) {
        const dateLabel =
          r.eventStart === r.eventEnd
            ? new Date(r.eventStart + "T00:00:00").toLocaleDateString("en-CA", { month: "short", day: "numeric" })
            : `${new Date(r.eventStart + "T00:00:00").toLocaleDateString("en-CA", { month: "short", day: "numeric" })}–${new Date(r.eventEnd + "T00:00:00").toLocaleDateString("en-CA", { month: "short", day: "numeric" })}`
        return [dateLabel, ...base]
      }
      return base
    })
    autoTable(doc, {
      startY: 122,
      margin: { left: marginX, right: marginX },
      head,
      body,
      styles: { fontSize: 9, cellPadding: 5, valign: "top", overflow: "linebreak" },
      headStyles: { fillColor: [1, 51, 102], textColor: 255, fontStyle: "bold" },
      alternateRowStyles: { fillColor: [244, 247, 250] },
      columnStyles: isRange
        ? {
            0: { cellWidth: 78 },
            1: { cellWidth: 130 },
            2: { cellWidth: 66 },
            3: { cellWidth: 130 },
            4: { cellWidth: 70 },
            5: { cellWidth: 62 },
            6: { cellWidth: 96 },
            7: { cellWidth: 80 },
          }
        : {
            0: { cellWidth: 150 },
            1: { cellWidth: 70 },
            2: { cellWidth: 150 },
            3: { cellWidth: 80 },
            4: { cellWidth: 70 },
            5: { cellWidth: 110 },
            6: { cellWidth: 90 },
          },
    })
  }

  // Footer note on every page
  const pageCount = doc.getNumberOfPages()
  for (let i = 1; i <= pageCount; i++) {
    doc.setPage(i)
    const h = doc.internal.pageSize.getHeight()
    doc.setTextColor(140, 140, 140)
    doc.setFontSize(8)
    doc.text(
      "Generated from the LCRB portal for police operational awareness. Not an official record.",
      marginX,
      h - 20,
    )
    doc.text(`Page ${i} of ${pageCount}`, pageWidth - marginX, h - 20, { align: "right" })
  }

  const filename =
    (fileBaseName ?? (isRange ? `SEP-report-${fromDate}_to_${toDate}` : `SEP-daily-report-${fromDate}`)) + ".pdf"

  if (output === "blob") {
    return { filename, blob: doc.output("blob"), count: records.length }
  }
  doc.save(filename)
  return { filename, count: records.length }
}

/** Range-aware report. Can download (`save`) or return a Blob (`blob`). */
export async function generateSepReportPdf(opts: SepReportOptions): Promise<SepReportResult> {
  return renderReport(opts)
}

/** Single-day download; keeps the original Upcoming Events behaviour. */
export async function generateSepDailyReportPdf({
  reportDate,
  records,
  filterSummary,
}: SepDailyReportOptions): Promise<void> {
  await renderReport({ fromDate: reportDate, toDate: reportDate, records, filterSummary, output: "save" })
}
