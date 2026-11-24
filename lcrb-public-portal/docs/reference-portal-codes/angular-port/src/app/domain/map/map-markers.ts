import type { SepRecord } from "@domain/models/special-events"
import { formatEventDate, SEP_STATUS_LABELS } from "@domain/special-events/seps"
import type { LgApplication } from "@domain/models/lg-review"
import { type MapMarkerData } from "@domain/models/map"

/** Status -> pin colour for Special Event Permits. */
export const SEP_STATUS_COLOR: Record<string, string> = {
  submitted: "#1d4ed8",
  "under-review": "#b45309",
  "changes-requested": "#c2410c",
  approved: "#15803d",
  denied: "#b91c1c",
  "auto-approved": "#64748b",
  cancelled: "#71717a",
}

/** Risk level -> pin colour for establishments under review. */
export const RISK_COLOR: Record<string, string> = {
  Low: "#15803d",
  Medium: "#b45309",
  High: "#b91c1c",
}

/** Brand colour used when an establishment layer isn't risk-coded. */
export const BRAND_COLOR = "#013366"

/** Escape user/content strings before injecting into popup markup. */
function esc(value: string): string {
  return String(value)
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/"/g, "&quot;")
    .replace(/'/g, "&#39;")
}

function badge(color: string, label: string): string {
  return `<span style="display:inline-block;padding:1px 6px;border-radius:9999px;background:${color};color:#fff;font-size:11px">${esc(
    label,
  )}</span>`
}

function popupLink(href: string, label: string): string {
  return `<a href="${esc(
    href,
  )}" style="display:inline-block;margin-top:6px;font-size:12px;color:#013366;font-weight:600;text-decoration:underline">${esc(
    label,
  )} →</a>`
}

/** Convert a SEP record into a generic marker. */
export function sepMarker(
  r: SepRecord,
  opts: { layerId: string; reviewHref?: (r: SepRecord) => string } = { layerId: "sep" },
): MapMarkerData {
  const color = SEP_STATUS_COLOR[r.status] ?? BRAND_COLOR
  const link = opts.reviewHref ? popupLink(opts.reviewHref(r), "Open review") : ""
  const popupHtml = `<div style="min-width:190px">
      <div style="font-weight:600;color:#013366">${esc(r.eventName)}</div>
      <div style="font-size:12px;color:#475569;margin:2px 0">${esc(r.venueName)}, ${esc(r.city)}</div>
      <div style="font-size:12px;color:#475569">${esc(formatEventDate(r))}</div>
      <div style="font-size:12px;margin:4px 0">
        ${badge(color, SEP_STATUS_LABELS[r.status])}
        <span style="color:#475569"> · ${r.expectedAttendance.toLocaleString()} expected</span>
      </div>
      ${link}
    </div>`
  return { id: r.id, layerId: opts.layerId, lat: r.lat, lng: r.lng, color, popupHtml }
}

/** Convert an LG licence application (establishment) into a generic marker. */
export function establishmentMarker(
  a: LgApplication,
  opts: {
    layerId: string
    colorBy?: "risk" | "brand"
    reviewHref?: (a: LgApplication) => string
  } = { layerId: "establishment" },
): MapMarkerData {
  const color = opts.colorBy === "risk" ? RISK_COLOR[a.riskLevel] ?? BRAND_COLOR : BRAND_COLOR
  const link = opts.reviewHref ? popupLink(opts.reviewHref(a), "Open application") : ""
  const riskLine =
    opts.colorBy === "risk"
      ? `<div style="font-size:12px;margin:4px 0">${badge(color, `${a.riskLevel} risk`)}<span style="color:#475569"> · ${
          a.contraventions
        } contraventions</span></div>`
      : ""
  const popupHtml = `<div style="min-width:190px">
      <div style="font-weight:600;color:#013366">${esc(a.establishmentName)}</div>
      <div style="font-size:12px;color:#475569;margin:2px 0">${esc(a.address)}, ${esc(a.city)}</div>
      <div style="font-size:12px;color:#475569">${esc(a.licenceType)} · ${esc(a.applicationType)}</div>
      ${riskLine}
      ${link}
    </div>`
  return { id: a.jobNumber, layerId: opts.layerId, lat: a.lat, lng: a.lng, color, popupHtml }
}

/** Legend describing SEP status colours (only the statuses worth showing). */
export const SEP_LEGEND: { label: string; color: string }[] = [
  { label: "Submitted", color: SEP_STATUS_COLOR.submitted },
  { label: "Under review", color: SEP_STATUS_COLOR["under-review"] },
  { label: "Changes requested", color: SEP_STATUS_COLOR["changes-requested"] },
  { label: "Approved", color: SEP_STATUS_COLOR.approved },
  { label: "Auto-approved", color: SEP_STATUS_COLOR["auto-approved"] },
  { label: "Denied", color: SEP_STATUS_COLOR.denied },
]

/** Legend describing establishment risk colours. */
export const RISK_LEGEND: { label: string; color: string }[] = [
  { label: "Low risk", color: RISK_COLOR.Low },
  { label: "Medium risk", color: RISK_COLOR.Medium },
  { label: "High risk", color: RISK_COLOR.High },
]
