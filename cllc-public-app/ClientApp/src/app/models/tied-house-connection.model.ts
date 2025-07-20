import { RelatedLicence } from './related-licence';

export class TiedHouseConnection {
  id: string;
  accountid: string;
  corpConnectionFederalProducer: string;
  corpConnectionFederalProducerDetails: string;
  familyMemberFederalProducer: string;
  familyMemberFederalProducerDetails: string;
  federalProducerConnectionToCorp: string;
  federalProducerConnectionToCorpDetails: string;
  isConnectionBoolean: Boolean;
  name: string;
  ownershipType: string;
  partnersConnectionFederalProducer: string;
  partnersConnectionFederalProducerDetails: string;
  percentageofOwnershipNumber: number;
  share20PlusConnectionProducer: string;
  share20PlusConnectionProducerDetails: string;
  share20PlusFamilyConnectionProducer: string;
  share20PlusFamilyConnectionProducerDetail: string;
  shareType: string;
  societyConnectionFederalProducer: string;
  societyConnectionFederalProducerDetails: string;
  tiedHouse: string;
  tiedHouseName: string;
  liquorFinancialInterest: number;
  liquorFinancialInterestDetails: string;

  crsConnectionToMarketer: string;
  crsConnectionToMarketerDetails: string;
  marketerConnectionToCrs: string;
  marketerConnectionToCrsDetails: string;
  iNConnectionToFederalProducer: string;
  iNConnectionToFederalProducerDetails: string;

  applicationId: string;
  isLegalEntity: boolean = false;
  dateOfBirth: string;
  firstName: string;
  middleName: string;
  lastName: string;
  relationshipToLicence: string;
  associatedLiquorLicense: RelatedLicence[];
  viewMode: number;
  legalEntityName: string;
  otherDescription: string;
  businessType: string;
  statusCode: number = TiedHouseStatusCode.new;
  supersededById: string;

  /**
   * Indicates that an existing tied house declartion record is marked for removal.
   */
  markedForRemoval: boolean;
}

export enum TiedHouseViewMode {
  /**
   * The record is new and is editable.
   */
  new = 1,
  /**
   * The record was previously persisted to dynamics, and is being displayed in a non-editable format.
   */
  existing = 3,
  /**
   * The record is not editable.
   */
  disabled = 4,
  /**
   * The record is new and is partially editable.
   * The record is related to an existing record, and so not all fields are editable.
   */
  addNewRelationship = 5,
  /**
   * The record was previously persisted to dynamics, and is being edited.
   */
  editExistingRecord = 6,
  /**
   * The record is hidden (not displayed in the UI).
   */
  hidden = 7
}

export enum TiedHouseStatusCode {
  /**
   * The record is new (not yet persisted).
   */
  new = 1,
  /**
   * The record is existing (has previously been persisted).
   */
  existing = 845280001
}

export const TiedHouseTypes = [
  { name: 'Individual', value: false },
  { name: 'Legal Entity', value: true }
];

// TODO: Populate real values
export const RelationshipTypes = [
  { name: 'TBD 1', value: 845280000 },
  { name: 'TBD 2', value: 845280001 },
  { name: 'Other', value: 845280002 }
];

export const BusinessTypes = [
  { value: 845280000, name: 'Private Corporation' },
  { value: 845280001, name: 'General Partnership' },
  { value: 845280002, name: 'Sole Proprietorship' },
  { value: 845280003, name: 'Public Corporation' },
  { value: 845280004, name: 'Society' },
  { value: 845280005, name: 'Partnership' },
  { value: 845280006, name: 'Limited Liability Corporation' },
  { value: 845280007, name: 'Unlimited Liability Corporation' },
  { value: 845280008, name: 'Limited Partnership' },
  { value: 845280009, name: 'Limited Liability Partnership' },
  { value: 845280010, name: 'Indigenous Nation' },
  { value: 845280011, name: 'Co-op' },
  { value: 845280012, name: 'Trust' },
  { value: 845280013, name: 'Estate' },
  { value: 845280014, name: 'Local Government' },
  { value: 845280015, name: 'Marketer' },
  { value: 845280016, name: 'University' },
  { value: 845280017, name: 'Military Mess' },
  { value: 845280018, name: 'Other' },
  { value: 845280019, name: 'Police' }
];
