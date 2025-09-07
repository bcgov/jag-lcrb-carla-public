import { RelatedLicence } from './related-licence';

export class TiedHouseConnection {
  id: string;
  accountId: string;

  corpConnectionFederalProducer: number | null;
  corpConnectionFederalProducerDetails: string;

  familyMemberFederalProducer: string;
  familyMemberFederalProducerDetails: string;

  federalProducerConnectionToCorp: number | null;
  federalProducerConnectionToCorpDetails: string;

  isConnectionBoolean: Boolean;

  name: string;
  ownershipType: string;

  partnersConnectionFederalProducer: number | null;
  partnersConnectionFederalProducerDetails: string;

  percentageofOwnershipNumber: number;

  share20PlusConnectionProducer: number | null;
  share20PlusConnectionProducerDetails: string;
  share20PlusFamilyConnectionProducer: number | null;
  share20PlusFamilyConnectionProducerDetail: string;
  shareType: string;

  societyConnectionFederalProducer: number | null;
  societyConnectionFederalProducerDetails: string;

  liquorFinancialInterest: number | null;
  liquorFinancialInterestDetails: string;

  crsConnectionToMarketer: number | null;
  crsConnectionToMarketerDetails: string;

  marketerConnectionToCrs: number | null;
  marketerConnectionToCrsDetails: string;

  iNConnectionToFederalProducer: number | null;
  iNConnectionToFederalProducerDetails: string;

  /**
   * TODO: tiedhouse - Deprecated?
   */
  tiedHouse: string;
  /**
   * TODO: tiedhouse - Deprecated?
   */
  tiedHouseName: string;

  applicationId: string;
  /**
   * The type of the liquor tied house connections (i.e. `Individual` or `Legal Entity`)
   */
  liqTiedHouseType: number | null;
  /**
   * The date of birth of the individual (if type is `Individual`).
   */
  dateOfBirth: string;
  /**
   * The first name of the individual (if type is `Individual`).
   */
  firstName: string;
  /**
   * The middle name of the individual (if type is `Individual`).
   */
  middleName: string;
  /**
   * The last name of the individual (if type is `Individual`).
   */
  lastName: string;
  /**
   * The relationship of the individual or legal entity to the liquor license.
   */
  relationshipToLicence: string;
  /**
   * The associated liquor licenses.
   */
  associatedLiquorLicense: RelatedLicence[];
  /**
   * Additional description for the tied house connection.
   */
  otherDescription: string;
  /**
   * The name of the legal entity (if type is `Legal Entity`).
   */
  legalEntityName: string;
  /**
   * The legal entity business type.
   */
  businessType: string;
  /**
   * The status code of the tied house connection (i.e. `New` or `Existing`)
   */
  statusCode: number = TiedHouseStatusCode.new;
  /**
   * The ID of the record that supersedes this one (if applicable).
   * Tied house connections are never hard-deleted, but instead are superseded by the updated version.
   */
  supersededById: string;
  /**
   * The category type of the tied house connection (i.e. `Liquor` or `Cannabis`)
   */
  categorytype: number;
  /**
   * The view mode of the tied house connection.
   * Used by the UI to determine how to display the record.
   */
  viewMode: number;
  /**
   * Indicates that an existing tied house connection record is marked for removal.
   */
  markedForRemoval?: boolean;
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

export const LIQTiedHouseTypeCodes = {
  Individual: 845280000,
  LegalEntity: 845280001
};

export const LIQTiedHouseTypes = [
  { name: 'Individual', value: LIQTiedHouseTypeCodes.Individual },
  { name: 'Legal Entity', value: LIQTiedHouseTypeCodes.LegalEntity }
];

export const RelationshipTypes = [
  { name: 'Shareholder', value: 845280000 },
  { name: 'Director', value: 845280001 },
  { name: 'Officer', value: 845280002 },
  { name: 'Partner', value: 845280003 },
  { name: 'Key Personnel', value: 845280004 },
  { name: 'Trustee', value: 845280005 },
  { name: 'Beneficiary', value: 845280006 },
  { name: 'Representative', value: 845280007 },
  { name: 'Immediate Family Member', value: 845280008 },
  { name: 'Other', value: 845280009 }
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

export const TiedHouseCategoryTypes = [
  { name: 'Liquor', value: 845280000 },
  { name: 'Cannabis', value: 845280001 }
];
