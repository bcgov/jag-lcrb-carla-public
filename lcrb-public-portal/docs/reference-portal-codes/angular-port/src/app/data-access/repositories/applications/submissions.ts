"use client"

/** Submitted-application store (prototype). */

import { computed, type Signal } from "@angular/core"
import { injectExternalStore, read, type Arg } from "@shared/util/external-store"
import type { AgentLog } from "@domain/models/staff"

/** One line on the payment receipt. */
export interface FeeLineItem {
  label: string
  amount: number
}

/** AI assessment of the application - staff-only (synced to MS Dynamics). */
export interface ApplicationAssessment {
  /** Single templated report body (maps into a Dynamics data field). */
  report: string
  /** Overall confidence score out of 100. */
  confidence: number
  /** One-line recommendation for triage, e.g. "Low-touch - likely approvable". */
  recommendation: string
  /** Short reasons supporting the score (shown to staff). */
  highlights: string[]
  /** Items a reviewer should pay attention to. */
  flags: string[]
  /** Model identifier + generation timestamp for auditability. */
  model: string
  generatedAt: string
}

export interface SubmissionRecord {
  /** Stable confirmation id used in the receipt URL. */
  id: string
  /** Human-facing confirmation number, e.g. "PAY-2026-4F9C2A". */
  confirmationNumber: string
  /** Application job/reference number, e.g. "LP-126203". */
  jobNumber: string
  flowId: string
  title: string
  /** Applicant / business this was submitted for. */
  applicant: string
  /** Fee breakdown shown on the receipt. */
  lineItems: FeeLineItem[]
  total: number
  currency: string
  /** Masked payment method, e.g. "Visa ending in 4242". */
  paymentMethod: string
  /** Receipt number from the payment boundary; present whenever a fee was charged. */
  receiptNumber?: string
  /** Where the receipt email was sent. */
  payerEmail?: string
  submittedAt: string
  /** Flattened answers (label/value) used for the receipt summary + assessment. */
  summary: { label: string; value: string }[]
  /** Status of the push to MS Dynamics. */
  syncStatus: "synced" | "pending" | "failed"
  /** AI assessment (null if generation failed - submission still succeeds). */
  assessment: ApplicationAssessment | null
  /** AI pre-screening agent log (checks, confidence, recommended queue). */
  agentLog?: AgentLog | null
}

const STORAGE_KEY = "lcrb.app.submissions"
const listeners = new Set<() => void>()

function readAll(): Record<string, SubmissionRecord> {
  if (typeof window === "undefined") return {}
  try {
    const raw = localStorage.getItem(STORAGE_KEY)
    return raw ? (JSON.parse(raw) as Record<string, SubmissionRecord>) : {}
  } catch {
    return {}
  }
}

function writeAll(map: Record<string, SubmissionRecord>) {
  try {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(map))
  } catch {
    // ignore quota / availability errors
  }
  listeners.forEach((l) => l())
}

export function saveSubmission(record: SubmissionRecord) {
  const map = readAll()
  map[record.id] = record
  writeAll(map)
}

export function getSubmission(id: string): SubmissionRecord | undefined {
  return readAll()[id]
}

/** All submitted applications, newest first. Used by global search + history. */
export function getAllSubmissions(): SubmissionRecord[] {
  return Object.values(readAll()).sort((a, b) => b.submittedAt.localeCompare(a.submittedAt))
}

function subscribe(cb: () => void) {
  listeners.add(cb)
  const onStorage = (e: StorageEvent) => {
    if (e.key === STORAGE_KEY) cb()
  }
  window.addEventListener("storage", onStorage)
  return () => {
    listeners.delete(cb)
    window.removeEventListener("storage", onStorage)
  }
}

/** Live subscription to a single submission record (re-renders on change). */
export function injectSubmission(idArg: Arg<string>): Signal<SubmissionRecord | undefined> {
  // The whole store is the snapshot; the record is picked per id below, so a changing id stays reactive.
  const getSnapshot = () => JSON.stringify(readAll())
  const json = injectExternalStore(subscribe, getSnapshot, () => "null")
  return computed(() => {
    const all = JSON.parse(json()) as Record<string, SubmissionRecord> | null
    return (all?.[read(idArg)] ?? null) as SubmissionRecord | undefined
  })
}

/** Generate a short, readable confirmation number. */
export function makeConfirmationNumber(): string {
  const rand = Math.random().toString(16).slice(2, 8).toUpperCase()
  return `PAY-${new Date().getFullYear()}-${rand}`
}

/** Derive a plausible job number from a flow id + sequence. */
export function makeJobNumber(flowId: string): string {
  const prefix = flowId
    .split("-")
    .map((p) => p[0])
    .join("")
    .slice(0, 4)
    .toUpperCase()
  const seq = 126000 + Math.floor(Math.random() * 999)
  return `${prefix || "APP"}-${seq}`
}
