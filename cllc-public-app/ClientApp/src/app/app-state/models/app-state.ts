import { LegalEntity } from '../../models/legal-entity.model';
import { DynamicsAccount } from '../../models/dynamics-account.model';
import { Application } from '../../models/application.model';
import { User } from '../../models/user.model';

export interface AppState {
    legalEntitiesState: LegalEntitiesState;
    applicationsState: ApplicationsState;
    currentAccountState: CurrentAccountState;
    currentApplicaitonState: CurrentApplicationState;
    currentLegalEntityState: CurrentLegalEntityState;
    currentUserState: CurrentUserState;
}

export interface LegalEntitiesState {
    legalEntities: LegalEntity[];
}

export interface ApplicationsState {
    applications: Application[];
}

export interface CurrentAccountState {
    currentAccount: DynamicsAccount;
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
