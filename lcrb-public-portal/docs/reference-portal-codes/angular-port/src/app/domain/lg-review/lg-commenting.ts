import { type LgCommentingPreference } from "@domain/models/lg-review"

/** Comment opt-out: later referrals skip the comment hold; owner only. TODO: persist per jurisdiction and enforce owner-only server-side. */

/** Days the LCRB holds a submitted application awaiting LG/IN comment before it can move along in Dynamics; replaces the legacy 90-day clock. */
export const COMMENT_HOLD_DAYS = 30

export const DEFAULT_LG_COMMENTING: LgCommentingPreference = {
  optedOut: false,
  updatedAt: null,
  updatedBy: null,
  effectiveFrom: null,
}

/** Whether a referral skips the comment hold because of the opt-out; false for referrals submitted before it took effect. */
export function isReferralExempt(
  pref: LgCommentingPreference,
  submittedDate: string,
): boolean {
  if (!pref.optedOut || !pref.effectiveFrom) return false
  return submittedDate >= pref.effectiveFrom
}

/** Human-readable summary of the current setting, used in banners and audits. */
export function commentingStatusLabel(pref: LgCommentingPreference): string {
  return pref.optedOut ? "Commenting turned off" : "Commenting turned on"
}
