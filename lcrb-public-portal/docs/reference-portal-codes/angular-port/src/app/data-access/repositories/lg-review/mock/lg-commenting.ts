import { type LgCommentingPreference } from "@domain/models/lg-review"
import { isReferralExempt } from "@domain/lg-review/lg-commenting"

/** Referrals that were already pending when the opt-out took effect. */
export function grandfatheredReferrals<T extends { submittedDate: string; status: string }>(
  pref: LgCommentingPreference,
  referrals: T[],
): T[] {
  if (!pref.optedOut) return []
  const stillOpen = (s: string) => s === "pending" || s === "in-review" || s === "documents-requested"
  return referrals.filter((r) => stillOpen(r.status) && !isReferralExempt(pref, r.submittedDate))
}
