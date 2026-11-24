import { Injectable } from '@angular/core';
import { getAccountConfig, defaultNotificationState } from './mock/account-settings';
import { getBusinessProfile } from './mock/business-profile';
import type { BusinessProfile, AccountConfig } from '@domain/models/account';
import type { UserRole } from '@domain/roles';
import type { AccountRepository } from './account.repository';

/** Mock AccountRepository over the prototype's seed data and mock services. */
@Injectable()
export class MockAccountRepository implements AccountRepository {
  businessProfile(organization?: string): BusinessProfile {
    return getBusinessProfile(organization);
  }

  accountConfig(role: UserRole): AccountConfig {
    return getAccountConfig(role);
  }

  defaultNotificationState(role: UserRole): Record<string, boolean> {
    return defaultNotificationState(role);
  }
}
