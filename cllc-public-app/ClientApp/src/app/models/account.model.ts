import { Contact } from "./contact.model";
import { TiedHouseConnection } from "@models/tied-house-connection.model";
import { LegalEntity } from "@models/legal-entity.model";

/**
 * This is a list of Business Type classified as other.
 * They influence the apperance/behavior of the account profile page, the application page and the application dashboard
 */
const BUSINESS_TYPE_OTHER = ["Coop", "MilitaryMess", "Other", "University", "LocalGovernment"];

export class Account {
  id: string;
  localGovernmentId: string;
  name: string;
  description: string;
  bcIncorporationNumber: string;
  dateOfIncorporationInBC: Date;
  businessNumber: string;
  pstNumber: string;
  contactEmail: string;
  contactPhone: string;
  termsOfUseAcceptedDate: Date;
  termsOfUseAccepted: boolean;
  businessDBAName: string;

  mailingAddressName: string;
  mailingAddressStreet: string;
  mailingAddressStreet2: string;
  mailingAddressCity: string;
  mailingAddressProvince: string;
  mailingAddressCountry: string;
  mailingAddressPostalCode: string;

  physicalAddressName: string;
  physicalAddressStreet: string;
  physicalAddressStreet2: string;
  physicalAddressCity: string;
  physicalAddressProvince: string;
  physicalAddressCountry: string;
  physicalAddressPostalCode: string;

  primarycontact: Contact;
  businessType: string;
  tiedHouse: TiedHouseConnection[];
  legalEntity: LegalEntity;

  websiteUrl: string;
  accountUrls: string | null;

  // SEP Police Review Limits
  isLateHoursApproval: boolean;
  maxGuestsForPublicEvents: number;
  maxGuestsForPrivateEvents: number;
  maxGuestsForFamilyEvents: number;

  isPartnership(): boolean {
    const isPartnership = [
      "GeneralPartnership",
      "LimitedPartnership",
      "LimitedLiabilityPartnership",
      "Partnership"
    ].indexOf(this.businessType) !==
      -1;
    return isPartnership;
  }

  isPrivateCorporation(): boolean {
    const isPrivateCorp = [
      "PrivateCorporation",
      "UnlimitedLiabilityCorporation",
      "LimitedLiabilityCorporation"
    ].indexOf(this.businessType) !==
      -1;
    return isPrivateCorp;
  }

  businessTypeIsSociety(): boolean {
    const isSociety = this.businessType === 'Society';
    return isSociety;
  }

  // Returns true if the businessType of the account is classified as other
  // Any businessType can be added to the BUSINESS_TYPE_OTHER array to classify it as "other"
  isOtherBusinessType(): boolean {
    const isOtherType = BUSINESS_TYPE_OTHER.indexOf(this.businessType) !== -1;
    return isOtherType;
  }

  isPublicCorporation(): boolean {
    const isPublicCorp = ["PublicCorporation"].indexOf(this.businessType) !== -1;
    return isPublicCorp;
  }

  getBusinessTypeName(): string {
    return Account.getBusinessTypeFromName(this.businessType);
  }

  static getBusinessTypeFromName(businessType: string): string {
    if (!businessType) {
      return "";
    }
    let name = "";
    switch (businessType) {
      case "GeneralPartnership":
      case 'LimitedPartnership"':
      case "LimitedLiabilityPartnership":
        name = "Partnership";
        break;
      case "SoleProprietorship":
        name = "Sole Proprietorship";
        break;
      case "IndigenousNation":
        name = "Indigenous Nation";
        break;
      case "PublicCorporation":
        name = "Public Corporation";
        break;
      case "PrivateCorporation":
      case "UnlimitedLiabilityCorporation":
      case "LimitedLiabilityCorporation":
        name = "Private Corporation";
        break;
      case "MilitaryMess":
        name = "Military Mess";
        break;
      case "LocalGovernment":
        name = "Local Government";
        break;
      default:
        name = businessType;
        break;
    }
    return name;
  }
}

export const BUSINESS_TYPE_LIST = [
  { value: "", name: "Choose the organization type" },
  // { value: "Church", name: "Church"},
  { value: "Coop", name: "Co-Op" },
  { value: "GeneralPartnership", name: "General Partnership" },
  { value: "IndigenousNation", name: "Indigenous Nation" },
  { value: "LimitedLiabilityCorporation", name: "Limited Liability Corporation" },
  { value: "LimitedLiabilityPartnership", name: "Limited Liability Partnership" },
  { value: "LimitedPartnership", name: "Limited Partnership" },
  { value: "LocalGovernment", name: "Local Government" },
  { value: "Marketer", name: "Marketer" },
  { value: "MilitaryMess", name: "Military Mess" },
  { value: "Partnership", name: "Partnership" },
  { value: "Police", name: "Police" },
  { value: "PrivateCorporation", name: "Private Corporation" },
  { value: "PublicCorporation", name: "Public Corporation" },
  { value: "Society", name: "Society" },
  { value: "SoleProprietorship", name: "Sole Proprietor" },
  { value: "University", name: "University" },
  { value: "UnlimitedLiabilityCorporation", name: "Unlimited Liability Corporation" },
];


export class TransferAccount {
  accountId: string;
  accountName: string;
  businessType: string;
  contactName: string;
}
