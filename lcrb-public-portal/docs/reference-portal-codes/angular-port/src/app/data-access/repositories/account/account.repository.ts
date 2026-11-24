import { InjectionToken } from '@angular/core';
import type { BusinessProfile, AccountConfig } from '@domain/models/account';
import type { UserRole } from '@domain/roles';
import { MockAccountRepository } from './account.repository.mock';

/** Account repository (backend area: 6.13 Account, session and delegates). */
export interface AccountRepository {
  businessProfile(organization?: string): BusinessProfile;
  accountConfig(role: UserRole): AccountConfig;
  defaultNotificationState(role: UserRole): Record<string, boolean>;
}

export const ACCOUNT_REPOSITORY = new InjectionToken<AccountRepository>('AccountRepository', {
  providedIn: 'root',
  factory: () => new MockAccountRepository(),
});
