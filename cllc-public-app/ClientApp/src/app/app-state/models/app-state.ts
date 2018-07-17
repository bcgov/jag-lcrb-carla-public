import { AdoxioLegalEntity } from '../../models/adoxio-legalentities.model';
import { DynamicsAccount } from '../../models/dynamics-account.model';
import { AdoxioApplication } from '../../models/adoxio-application.model';

export interface AppState {
    legalEntitiesState: LegalEntitiesState;
    applicationsState: ApplicationsState;
    currentAccountState: CurrentAccountState;
    currentLegalEntityState: CurrentLegalEntityState;
}

export interface LegalEntitiesState {
    legalEntities: AdoxioLegalEntity[];
}

export interface ApplicationsState {
    applications: AdoxioApplication[];
}

export interface CurrentAccountState {
    currentAccount: DynamicsAccount;
}

export interface CurrentLegalEntityState {
    currentLegalEntity: AdoxioLegalEntity;
}
