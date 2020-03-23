import { LegalEntity } from '@models/legal-entity.model';
import { Account } from '@models/account.model';
import { Application } from '@models/application.model';
import { User } from '@models/user.model';

export interface AppState {
    legalEntitiesState: LegalEntitiesState;
    indigenousNationState: IndigenousNationState;
    currentAccountState: CurrentAccountState;
    currentApplicaitonState: CurrentApplicationState;
    currentLegalEntityState: CurrentLegalEntityState;
    currentUserState: CurrentUserState;
    onGoingLicenseeChangesApplicationIdState: OnGoingLicenseeChangesApplicationIdState;
}

export interface LegalEntitiesState {
    legalEntities: LegalEntity[];
}

export interface IndigenousNationState {
    indigenousNationModeActive: boolean;
}

export interface CurrentAccountState {
    currentAccount: Account;
}

export interface CurrentUserState {
    currentUser: User;
}

export interface CurrentApplicationState {
    currentApplication: Application;
}

export interface CurrentLegalEntityState {
    currentLegalEntity: LegalEntity;
}

export interface OnGoingLicenseeChangesApplicationIdState {
    onGoingLicenseeChangesApplicationId: string;
}
