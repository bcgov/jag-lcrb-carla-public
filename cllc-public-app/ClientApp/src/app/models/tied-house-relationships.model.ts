import { RelatedLicence } from "./related-licence";

export class TiedHouseDeclaration
{
    id: string;
    applicationId: string;
    tiedHouseType: string;
    dateOfBirth: Date;
    firstName: string;
    middleName: string;
    lastName: string;
    relationshipToLicence: string;
    associatedLiquorLicense: RelatedLicence[];
    viewMode: TiedHouseViewMode;
};

export enum TiedHouseViewMode
{
    editable =1,
    readonly = 2,
    table = 3,
    disabled = 4,
    addNewRelationship =5,
    editExistingRecord = 6
}