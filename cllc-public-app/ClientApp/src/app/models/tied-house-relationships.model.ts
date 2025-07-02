import { RelatedLicence } from "./related-licence";

export class TiedHouseDeclaration
{
    id: string;
    applicationId: string;
    tiedHouseType: number = TiedHouseTypeEnum.Individual;
    dateOfBirth: Date;
    firstName: string;
    middleName: string;
    lastName: string;
    relationshipToLicence: string;
    associatedLiquorLicense: RelatedLicence[];
    removeExistingLicense: boolean = false;
    viewMode: TiedHouseViewMode;
};

export enum TiedHouseViewMode
{
    new =1,
    readonly = 2,
    table = 3,
    disabled = 4,
    addNewRelationship =5,
    editExistingRecord = 6
}

export enum TiedHouseTypeEnum
{
    Individual = 1,
    LegalEntity = 2,
}

export const TiedHouseTypes = [
  { name: "Individual", id: TiedHouseTypeEnum.Individual },
  { name: "Legal Entity", id: TiedHouseTypeEnum.LegalEntity }
];