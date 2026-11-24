"use client"

/** Site / web analytics (prototype). */

import { createCollection, makeId } from "../mock-storage/kv-store"
import { type Entity } from "@domain/models/entity"
import { getCatalogEntry } from "../applications/mock/applications"

export type DeviceClass = "desktop" | "mobile" | "tablet"
export type ReferrerKind = "direct" | "search" | "social" | "internal" | "email"

export interface PageView extends Entity {
  /** Normalized path (dynamic ids collapsed, e.g. /licences/[id]). */
  path: string
  /** Human-readable page label. */
  title: string
  /** Top-level section key used for grouping (e.g. "applications"). */
  section: string
  /** When the page is an application flow, the application slug. */
  applicationSlug?: string
  /** Anonymous, rotating visitor id. */
  visitorId: string
  /** Per-tab session id. */
  sessionId: string
  /** Audience the visitor was browsing as (portal role), if known. */
  audience: string
  device: DeviceClass
  referrer: ReferrerKind
  /** True if this view created the visitor id (first-ever view). */
  isNewVisitor: boolean
  /** ISO timestamp. */
  ts: string
}

const collection = createCollection<PageView>("site-analytics")

const VISITOR_KEY = "lcrb.analytics.vid"
const SESSION_KEY = "lcrb.analytics.sid"


/** Top-level section metadata for grouping and friendly labels. */
export const SECTION_META: Record<string, { label: string }> = {
  home: { label: "Home & Landing" },
  dashboard: { label: "Dashboard" },
  applications: { label: "Applications" },
  licences: { label: "Licences" },
  "special-events": { label: "Special Event Permits" },
  compliance: { label: "Compliance" },
  "legal-entity": { label: "Business Structure" },
  "tied-house": { label: "Business Structure" },
  map: { label: "Maps & Finder" },
  "licence-finder": { label: "Maps & Finder" },
  account: { label: "Account & Profile" },
  notifications: { label: "Account & Profile" },
  announcements: { label: "Announcements" },
  login: { label: "Sign In" },
  lcrb: { label: "Staff — LCRB" },
  police: { label: "Partner — Police" },
  "local-government": { label: "Partner — Local Government" },
  support: { label: "Help & Support" },
  other: { label: "Other" },
}

/** Long/opaque path segments (ids) collapse to a token so pages group cleanly. */
function collapseSegment(seg: string): string | null {
  if (/^\d+$/.test(seg)) return "[id]"
  // ids like case_ab12cd, LCRB-123456, uuids, or long random tokens
  if (/^[a-z]+_[a-z0-9]{4,}$/i.test(seg)) return "[id]"
  if (/^[0-9a-f]{8}-[0-9a-f]{4}/i.test(seg)) return "[id]"
  if (seg.length > 18 && /[0-9]/.test(seg)) return "[id]"
  return null
}

const SUPPORT_PATHS = new Set([
  "contact",
  "accessibility",
  "privacy",
  "disclaimer",
  "copyright",
  "help",
])

/** Normalize a raw pathname into a stable, groupable path + section + label. */
export function classifyPath(rawPath: string): {
  path: string
  section: string
  title: string
  applicationSlug?: string
} {
  const clean = (rawPath || "/").split("?")[0].split("#")[0]
  const segs = clean.split("/").filter(Boolean)

  if (segs.length === 0) {
    return { path: "/", section: "home", title: "Home / Landing" }
  }

  // Application flows: /applications/new/[slug] and /applications/food-primary
  if (segs[0] === "applications" && segs[1] === "new" && segs[2]) {
    const entry = getCatalogEntry(segs[2])
    return {
      path: `/applications/new/${segs[2]}`,
      section: "applications",
      title: entry ? `Apply: ${entry.title}` : `Apply: ${humanize(segs[2])}`,
      applicationSlug: segs[2],
    }
  }

  const normalizedSegs = segs.map((s) => collapseSegment(s) ?? s)
  const path = `/${normalizedSegs.join("/")}`

  const head = segs[0]
  const section = SUPPORT_PATHS.has(head) ? "support" : SECTION_META[head] ? head : "other"
  const title = labelForPath(normalizedSegs)
  return { path, section, title }
}

function humanize(slug: string): string {
  return slug
    .split(/[-_]/)
    .map((w) => w.charAt(0).toUpperCase() + w.slice(1))
    .join(" ")
}

function labelForPath(segs: string[]): string {
  const map: Record<string, string> = {
    dashboard: "Dashboard",
    applications: "My Applications",
    licences: "Licences",
    "special-events": "Special Event Permits",
    compliance: "Compliance & Enforcement",
    "legal-entity": "Legal Entity Builder",
    "tied-house": "Tied House",
    map: "Map",
    "licence-finder": "Licence Finder",
    account: "Account Profile",
    notifications: "Notifications",
    announcements: "Announcements",
    login: "Sign In",
    calendar: "Renewals & Calendar",
    notices: "Notices",
    "security-screening": "Security Screening",
    activity: "Activity Log",
    upload: "Document Upload",
    lcrb: "LCRB Intake",
    police: "Police Portal",
    "local-government": "Local Government Portal",
    contact: "Contact Us",
  }
  const base = map[segs[0]] ?? humanize(segs[0])
  if (segs.length === 1) return base
  const tail = segs[segs.length - 1]
  if (tail === "[id]") return `${base} — Detail`
  return `${base} — ${humanize(tail)}`
}


function getVisitorId(): { id: string; isNew: boolean } {
  if (typeof window === "undefined") return { id: "server", isNew: false }
  try {
    let id = localStorage.getItem(VISITOR_KEY)
    if (id) return { id, isNew: false }
    id = makeId("v")
    localStorage.setItem(VISITOR_KEY, id)
    return { id, isNew: true }
  } catch {
    return { id: "anon", isNew: false }
  }
}

function getSessionId(): string {
  if (typeof window === "undefined") return "server"
  try {
    let id = sessionStorage.getItem(SESSION_KEY)
    if (!id) {
      id = makeId("s")
      sessionStorage.setItem(SESSION_KEY, id)
    }
    return id
  } catch {
    return "anon-session"
  }
}

function detectDevice(): DeviceClass {
  if (typeof navigator === "undefined") return "desktop"
  const ua = navigator.userAgent
  if (/iPad|Tablet|PlayBook|Silk/i.test(ua) || (/Android/i.test(ua) && !/Mobile/i.test(ua)))
    return "tablet"
  if (/Mobi|iPhone|Android.*Mobile|Windows Phone/i.test(ua)) return "mobile"
  return "desktop"
}

function detectReferrer(): ReferrerKind {
  if (typeof document === "undefined") return "direct"
  const ref = document.referrer
  if (!ref) return "direct"
  try {
    const url = new URL(ref)
    if (url.host === window.location.host) return "internal"
    if (/google|bing|duckduckgo|yahoo/i.test(url.host)) return "search"
    if (/facebook|twitter|x\.com|linkedin|instagram|t\.co/i.test(url.host)) return "social"
    return "direct"
  } catch {
    return "direct"
  }
}

/** Record a single page view. Safe to call on every client-side navigation. */
export function recordPageView(rawPath: string, audience = "guest"): void {
  if (typeof window === "undefined") return
  seedSampleTrafficIfEmpty()

  const { id: visitorId, isNew } = getVisitorId()
  const sessionId = getSessionId()
  const { path, section, title, applicationSlug } = classifyPath(rawPath)

  // Guard against duplicate consecutive records (React strict mode / re-renders)
  const recent = collection.list()
  const last = recent[recent.length - 1]
  if (last && last.sessionId === sessionId && last.path === path) {
    const gap = Date.now() - new Date(last.ts).getTime()
    if (gap < 1500) return
  }

  collection.upsert({
    id: makeId("pv"),
    path,
    title,
    section,
    applicationSlug,
    visitorId,
    sessionId,
    audience,
    device: detectDevice(),
    referrer: detectReferrer(),
    isNewVisitor: isNew,
    ts: new Date().toISOString(),
  })
}


export interface DailyDatum {
  date: string // yyyy-mm-dd
  label: string // e.g. "Jun 3"
  visitors: number
  pageviews: number
}
export interface TopPageDatum {
  path: string
  title: string
  views: number
  visitors: number
}
export interface TopAppDatum {
  slug: string
  title: string
  views: number
}
export interface SectionDatum {
  section: string
  label: string
  views: number
}
export interface DeviceDatum {
  device: DeviceClass
  label: string
  sessions: number
}
export interface AudienceDatum {
  audience: string
  label: string
  sessions: number
}
export interface ReferrerDatum {
  referrer: ReferrerKind
  label: string
  sessions: number
}

export interface SiteAnalytics {
  /** Total page views (traffic). */
  totalViews: number
  /** Distinct visitor ids. */
  uniqueVisitors: number
  /** Distinct sessions. */
  sessions: number
  /** Mean unique visitors per active day over the window. */
  avgVisitorsPerDay: number
  /** Unique visitors seen today. */
  visitorsToday: number
  /** Page views today. */
  viewsToday: number
  /** Mean page views per session. */
  pagesPerSession: number
  /** New vs returning split (by visitor id). */
  newVisitors: number
  returningVisitors: number
  /** Percentage change in visitors vs the previous equal-length window. */
  visitorsTrendPct: number
  daily: DailyDatum[]
  topPages: TopPageDatum[]
  topApplications: TopAppDatum[]
  bySection: SectionDatum[]
  byDevice: DeviceDatum[]
  byAudience: AudienceDatum[]
  byReferrer: ReferrerDatum[]
}

const DEVICE_LABELS: Record<DeviceClass, string> = {
  desktop: "Desktop",
  mobile: "Mobile",
  tablet: "Tablet",
}
const REFERRER_LABELS: Record<ReferrerKind, string> = {
  direct: "Direct",
  search: "Search",
  social: "Social",
  internal: "Internal",
  email: "Email",
}
const AUDIENCE_LABELS: Record<string, string> = {
  guest: "Public / Guest",
  licensee: "Licensee",
  applicant: "SEP Applicant",
  police: "Police",
  "local-government": "Local Government",
  lcrb: "LCRB Staff",
}

function dayKey(d: Date): string {
  return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, "0")}-${String(d.getDate()).padStart(2, "0")}`
}
function dayLabel(d: Date): string {
  return d.toLocaleDateString("en-CA", { month: "short", day: "numeric" })
}

/** Load the page-view stream (seeding demo data on first use). */
export function loadPageViews(): PageView[] {
  seedSampleTrafficIfEmpty()
  return collection.list()
}

/** Compute the site analytics over the trailing `windowDays` (default 30). */
export function computeSiteAnalytics(views: PageView[], windowDays = 30): SiteAnalytics {
  const now = new Date()
  const start = new Date(now)
  start.setDate(start.getDate() - (windowDays - 1))
  start.setHours(0, 0, 0, 0)
  const prevStart = new Date(start)
  prevStart.setDate(prevStart.getDate() - windowDays)

  const windowViews = views.filter((v) => new Date(v.ts) >= start)
  const prevWindowViews = views.filter((v) => {
    const t = new Date(v.ts)
    return t >= prevStart && t < start
  })

  const totalViews = windowViews.length
  const uniqueVisitors = new Set(windowViews.map((v) => v.visitorId)).size
  const sessions = new Set(windowViews.map((v) => v.sessionId)).size
  const prevUnique = new Set(prevWindowViews.map((v) => v.visitorId)).size

  const todayKey = dayKey(now)

  // Daily buckets (every day in the window, including zero days)
  const dailyMap = new Map<string, { visitors: Set<string>; views: number; label: string }>()
  for (let i = 0; i < windowDays; i++) {
    const d = new Date(start)
    d.setDate(start.getDate() + i)
    dailyMap.set(dayKey(d), { visitors: new Set(), views: 0, label: dayLabel(d) })
  }
  for (const v of windowViews) {
    const k = dayKey(new Date(v.ts))
    const bucket = dailyMap.get(k)
    if (bucket) {
      bucket.views++
      bucket.visitors.add(v.visitorId)
    }
  }
  const daily: DailyDatum[] = Array.from(dailyMap.entries()).map(([date, b]) => ({
    date,
    label: b.label,
    visitors: b.visitors.size,
    pageviews: b.views,
  }))
  const activeDays = daily.filter((d) => d.visitors > 0).length || 1
  const avgVisitorsPerDay = Math.round(
    daily.reduce((s, d) => s + d.visitors, 0) / activeDays,
  )

  const visitorsToday = new Set(
    windowViews.filter((v) => dayKey(new Date(v.ts)) === todayKey).map((v) => v.visitorId),
  ).size
  const viewsToday = windowViews.filter((v) => dayKey(new Date(v.ts)) === todayKey).length

  // Top pages
  const pageMap = new Map<string, { title: string; views: number; visitors: Set<string> }>()
  for (const v of windowViews) {
    const entry = pageMap.get(v.path) ?? { title: v.title, views: 0, visitors: new Set() }
    entry.views++
    entry.visitors.add(v.visitorId)
    entry.title = v.title
    pageMap.set(v.path, entry)
  }
  const topPages: TopPageDatum[] = Array.from(pageMap.entries())
    .map(([path, e]) => ({ path, title: e.title, views: e.views, visitors: e.visitors.size }))
    .sort((a, b) => b.views - a.views)
    .slice(0, 8)

  // Top applications (by slug)
  const appMap = new Map<string, { title: string; views: number }>()
  for (const v of windowViews) {
    if (!v.applicationSlug) continue
    const entry = appMap.get(v.applicationSlug) ?? {
      title: v.title.replace(/^Apply:\s*/, ""),
      views: 0,
    }
    entry.views++
    appMap.set(v.applicationSlug, entry)
  }
  const topApplications: TopAppDatum[] = Array.from(appMap.entries())
    .map(([slug, e]) => ({ slug, title: e.title, views: e.views }))
    .sort((a, b) => b.views - a.views)
    .slice(0, 7)

  // By section - grouped by DISPLAY LABEL, not raw section key.
  const sectionMap = new Map<string, { section: string; views: number }>()
  for (const v of windowViews) {
    const label = SECTION_META[v.section]?.label ?? humanize(v.section)
    const existing = sectionMap.get(label)
    if (existing) existing.views += 1
    else sectionMap.set(label, { section: v.section, views: 1 })
  }
  const bySection: SectionDatum[] = Array.from(sectionMap.entries())
    .map(([label, { section, views }]) => ({ section, label, views }))
    .sort((a, b) => b.views - a.views)

  // By device (unique sessions per device)
  const deviceSessions = new Map<DeviceClass, Set<string>>()
  for (const v of windowViews) {
    const set = deviceSessions.get(v.device) ?? new Set()
    set.add(v.sessionId)
    deviceSessions.set(v.device, set)
  }
  const byDevice: DeviceDatum[] = (["desktop", "mobile", "tablet"] as DeviceClass[])
    .map((device) => ({
      device,
      label: DEVICE_LABELS[device],
      sessions: deviceSessions.get(device)?.size ?? 0,
    }))
    .filter((d) => d.sessions > 0)

  // By audience (unique sessions per role)
  const audienceSessions = new Map<string, Set<string>>()
  for (const v of windowViews) {
    const set = audienceSessions.get(v.audience) ?? new Set()
    set.add(v.sessionId)
    audienceSessions.set(v.audience, set)
  }
  const byAudience: AudienceDatum[] = Array.from(audienceSessions.entries())
    .map(([audience, set]) => ({
      audience,
      label: AUDIENCE_LABELS[audience] ?? humanize(audience),
      sessions: set.size,
    }))
    .sort((a, b) => b.sessions - a.sessions)

  // By referrer (unique sessions per referrer)
  const refSessions = new Map<ReferrerKind, Set<string>>()
  for (const v of windowViews) {
    const set = refSessions.get(v.referrer) ?? new Set()
    set.add(v.sessionId)
    refSessions.set(v.referrer, set)
  }
  const byReferrer: ReferrerDatum[] = Array.from(refSessions.entries())
    .map(([referrer, set]) => ({
      referrer,
      label: REFERRER_LABELS[referrer],
      sessions: set.size,
    }))
    .sort((a, b) => b.sessions - a.sessions)

  // New vs returning (by visitor id within window)
  const newVisitors = new Set(
    windowViews.filter((v) => v.isNewVisitor).map((v) => v.visitorId),
  ).size
  const returningVisitors = Math.max(uniqueVisitors - newVisitors, 0)

  const visitorsTrendPct =
    prevUnique === 0 ? (uniqueVisitors > 0 ? 100 : 0) : Math.round(((uniqueVisitors - prevUnique) / prevUnique) * 100)

  return {
    totalViews,
    uniqueVisitors,
    sessions,
    avgVisitorsPerDay,
    visitorsToday,
    viewsToday,
    pagesPerSession: sessions === 0 ? 0 : Math.round((totalViews / sessions) * 10) / 10,
    newVisitors,
    returningVisitors,
    visitorsTrendPct,
    daily,
    topPages,
    topApplications,
    bySection,
    byDevice,
    byAudience,
    byReferrer,
  }
}


/** Deterministic-ish PRNG so seeded demo data is stable within a browser. */
function mulberry32(seed: number) {
  return function () {
    seed |= 0
    seed = (seed + 0x6d2b79f5) | 0
    let t = Math.imul(seed ^ (seed >>> 15), 1 | seed)
    t = (t + Math.imul(t ^ (t >>> 7), 61 | t)) ^ t
    return ((t ^ (t >>> 14)) >>> 0) / 4294967296
  }
}

// Weighted destinations approximating real portal usage.
const SEED_PAGES: { path: string; weight: number }[] = [
  { path: "/", weight: 14 },
  { path: "/dashboard", weight: 12 },
  { path: "/applications", weight: 8 },
  { path: "/licences", weight: 9 },
  { path: "/special-events", weight: 7 },
  { path: "/special-events/apply", weight: 5 },
  { path: "/compliance", weight: 4 },
  { path: "/legal-entity", weight: 4 },
  { path: "/tied-house", weight: 2 },
  { path: "/map", weight: 5 },
  { path: "/licence-finder", weight: 4 },
  { path: "/announcements", weight: 3 },
  { path: "/notices", weight: 3 },
  { path: "/account", weight: 3 },
  { path: "/login", weight: 6 },
  { path: "/calendar", weight: 2 },
  { path: "/security-screening", weight: 2 },
  { path: "/contact", weight: 2 },
]

const SEED_APP_SLUGS: { slug: string; weight: number }[] = [
  { slug: "licensee-representative", weight: 6 },
  { slug: "fp-new-patio", weight: 5 },
  { slug: "fp-temp-expanded-service-area", weight: 4 },
  { slug: "fp-hours-after-midnight", weight: 3 },
  { slug: "structural-change", weight: 3 },
  { slug: "transfer-of-licence", weight: 4 },
  { slug: "change-licensee-name", weight: 2 },
]

const SEED_AUDIENCES: { role: string; weight: number }[] = [
  { role: "guest", weight: 8 },
  { role: "licensee", weight: 10 },
  { role: "applicant", weight: 6 },
  { role: "local-government", weight: 3 },
  { role: "police", weight: 2 },
  { role: "lcrb", weight: 3 },
]

const SEED_DEVICES: { device: DeviceClass; weight: number }[] = [
  { device: "desktop", weight: 6 },
  { device: "mobile", weight: 3 },
  { device: "tablet", weight: 1 },
]

const SEED_REFERRERS: { referrer: ReferrerKind; weight: number }[] = [
  { referrer: "direct", weight: 5 },
  { referrer: "search", weight: 4 },
  { referrer: "internal", weight: 3 },
  { referrer: "social", weight: 1 },
  { referrer: "email", weight: 1 },
]

function weightedPick<T extends { weight: number }>(items: T[], rnd: () => number): T {
  const total = items.reduce((s, i) => s + i.weight, 0)
  let r = rnd() * total
  for (const item of items) {
    r -= item.weight
    if (r <= 0) return item
  }
  return items[items.length - 1]
}

/** Populate ~30 days of realistic traffic the first time the dashboard is opened, so charts are meaningful in the prototype. */
export function seedSampleTrafficIfEmpty(): void {
  if (typeof window === "undefined") return
  if (collection.list().length > 0) return

  const rnd = mulberry32(20260706)
  const days = 30
  const now = new Date()
  const records: PageView[] = []

  // A stable pool of anonymous visitors; a subset returns across days.
  const visitorPool = Array.from({ length: 140 }, (_, i) => `seed_v_${i}`)

  for (let d = days - 1; d >= 0; d--) {
    const date = new Date(now)
    date.setDate(now.getDate() - d)
    const dow = date.getDay()
    // Weekday traffic higher than weekends.
    const weekdayFactor = dow === 0 || dow === 6 ? 0.45 : 1
    // Gentle upward trend toward the present.
    const trend = 0.7 + ((days - d) / days) * 0.6
    const dailyVisitors = Math.round((22 + rnd() * 20) * weekdayFactor * trend)

    for (let i = 0; i < dailyVisitors; i++) {
      // ~40% returning visitors reuse a pool id; others are fresh.
      const returning = rnd() < 0.4
      const poolIdx = Math.floor(rnd() * visitorPool.length)
      const visitorId = returning ? visitorPool[poolIdx] : `seed_v_new_${d}_${i}`
      const isNewVisitor = !returning
      const sessionId = `seed_s_${d}_${i}`
      const audience = weightedPick(SEED_AUDIENCES, rnd).role
      const device = weightedPick(SEED_DEVICES, rnd).device
      const referrer = weightedPick(SEED_REFERRERS, rnd).referrer

      // Each session views 1–6 pages.
      const pages = 1 + Math.floor(rnd() * 6)
      for (let p = 0; p < pages; p++) {
        // ~22% of views land on an application flow.
        let rawPath: string
        if (rnd() < 0.22) {
          rawPath = `/applications/new/${weightedPick(SEED_APP_SLUGS, rnd).slug}`
        } else {
          rawPath = weightedPick(SEED_PAGES, rnd).path
        }
        const cls = classifyPath(rawPath)
        const ts = new Date(date)
        ts.setHours(7 + Math.floor(rnd() * 13), Math.floor(rnd() * 60), Math.floor(rnd() * 60))
        records.push({
          id: makeId("pv"),
          path: cls.path,
          title: cls.title,
          section: cls.section,
          applicationSlug: cls.applicationSlug,
          visitorId,
          sessionId,
          audience,
          device,
          referrer,
          isNewVisitor: isNewVisitor && p === 0,
          ts: ts.toISOString(),
        })
      }
    }
  }

  collection.replaceAll(records)
}

export const siteAnalyticsCollection = collection
