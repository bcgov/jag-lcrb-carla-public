/** Global search index. */

import type { UserRole } from "@domain/roles"
import { MOCK_LICENCES, MOCK_OPERATED_LICENCES } from "../licences/mock/licences"
import { getAllSubmissions } from "../applications/submissions"
import { SEP_RECORDS } from "../special-events/mock/seps"
import { SEP_STATUS_LABELS } from "@domain/special-events/seps"
import { LG_APPLICATIONS } from "../lg-review/mock/lg-applications"
import { LG_STATUS_LABELS } from "@domain/models/lg-review"
import { EXTENDED_CATALOG } from "../applications/mock/application-catalog-extended"
import { type SearchResult, type SearchOptions } from "@domain/models/search"

/** Build the full result set available to a given role. */
function buildIndex(role: UserRole | undefined): SearchResult[] {
  const results: SearchResult[] = []

  // Licences + submitted applications + the catalog are licensee/applicant facing.
  if (role === "licensee" || role === "applicant" || !role) {
    for (const l of [...MOCK_LICENCES, ...MOCK_OPERATED_LICENCES]) {
      results.push({
        id: `licence-${l.id}-${l.category}`,
        kind: "licence",
        title: `${l.name} (${l.id})`,
        subtitle: `${l.type} · ${l.status} · ${l.address}`,
        badge: l.status,
        href: `/licences/${l.id}`,
        haystack: `${l.name} ${l.id} ${l.type} ${l.status} ${l.address} ${l.flags.join(" ")}`.toLowerCase(),
      })
    }
    for (const s of getAllSubmissions()) {
      results.push({
        id: `submission-${s.id}`,
        kind: "application",
        title: `${s.title} (${s.jobNumber})`,
        subtitle: `${s.applicant} · submitted ${new Date(s.submittedAt).toLocaleDateString()}`,
        badge: s.syncStatus,
        href: `/applications/confirmation/${s.id}`,
        haystack: `${s.title} ${s.jobNumber} ${s.confirmationNumber} ${s.applicant}`.toLowerCase(),
      })
    }
    for (const c of EXTENDED_CATALOG) {
      results.push({
        id: `catalog-${c.slug}`,
        kind: "catalog",
        title: c.title,
        subtitle: `${c.licenceType} · ${c.kind}${c.fee ? ` · ${c.fee}` : ""}`,
        href: `/applications/new/${c.slug}`,
        haystack: `${c.title} ${c.description} ${c.licenceType} ${c.kind}`.toLowerCase(),
      })
    }
  }

  // SEPs are visible to applicants (their own) and police (review).
  if (role === "licensee" || role === "applicant" || role === "police" || !role) {
    for (const sep of SEP_RECORDS) {
      const href = role === "police" ? `/police/review/${sep.id}` : "/special-events"
      results.push({
        id: `sep-${sep.id}`,
        kind: "sep",
        title: `${sep.eventName} (${sep.id})`,
        subtitle: `${sep.venueName}, ${sep.city} · ${SEP_STATUS_LABELS[sep.status]}`,
        badge: SEP_STATUS_LABELS[sep.status],
        href,
        haystack:
          `${sep.eventName} ${sep.id} ${sep.applicantName} ${sep.applicantOrg ?? ""} ${sep.venueName} ${sep.city} ${sep.jurisdiction}`.toLowerCase(),
      })
    }
  }

  // Local-government referrals.
  if (role === "local-government" || !role) {
    for (const app of LG_APPLICATIONS) {
      results.push({
        id: `lg-${app.jobNumber}`,
        kind: "lg-referral",
        title: `${app.establishmentName} (${app.jobNumber})`,
        subtitle: `${app.applicationType} · ${LG_STATUS_LABELS[app.status]}`,
        badge: LG_STATUS_LABELS[app.status],
        href: `/local-government/review/${app.jobNumber}`,
        haystack:
          `${app.establishmentName} ${app.jobNumber} ${app.applicationType} ${app.applicantName ?? ""} ${app.address ?? ""}`.toLowerCase(),
      })
    }
  }

  return results
}

/** Subsequence fuzzy score: rewards contiguous + early matches. Returns -1 on miss. */
function fuzzyScore(query: string, text: string): number {
  if (!query) return 0
  // Fast path: direct substring is the strongest signal.
  const idx = text.indexOf(query)
  if (idx !== -1) return 1000 - idx

  let score = 0
  let ti = 0
  let streak = 0
  for (let qi = 0; qi < query.length; qi++) {
    const ch = query[qi]
    let found = -1
    for (let j = ti; j < text.length; j++) {
      if (text[j] === ch) {
        found = j
        break
      }
    }
    if (found === -1) return -1
    streak = found === ti ? streak + 1 : 0
    score += 10 + streak * 2 - (found - ti)
    ti = found + 1
  }
  return score
}

/** Run a query against the role-aware index, ranked best-first. */
export function search(query: string, opts: SearchOptions = {}): SearchResult[] {
  const { role, limit = 20 } = opts
  const index = buildIndex(role)
  const q = query.trim().toLowerCase()
  if (!q) {
    // Empty query: show a useful default (catalog + most recent items).
    return index.slice(0, limit)
  }
  const terms = q.split(/\s+/)
  const scored = index
    .map((r) => {
      // Every term must match somewhere; total score is the sum.
      let total = 0
      for (const t of terms) {
        const s = fuzzyScore(t, r.haystack)
        if (s < 0) return { r, score: -1 }
        total += s
      }
      return { r, score: total }
    })
    .filter((x) => x.score >= 0)
    .sort((a, b) => b.score - a.score)
    .slice(0, limit)
  return scored.map((x) => x.r)
}
