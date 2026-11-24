import { computed, inject, type Signal } from "@angular/core"
import { injectSession } from "./session-provider"
import { type BusinessProfile } from "@domain/models/account"
import { ACCOUNT_REPOSITORY } from "@data-access/repositories/account/account.repository"

/** The authenticated licensee's business profile; combines the session with the account repository, so it lives in core. */
export function injectBusinessProfile(): Signal<BusinessProfile> {
  const { session } = injectSession()
  const account = inject(ACCOUNT_REPOSITORY)
  return computed(() => account.businessProfile(session()?.organization))
}
