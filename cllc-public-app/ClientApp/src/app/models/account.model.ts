import { Contact } from './contact.model';
import { TiedHouseConnection } from '@models/tied-house-connection.model';
import { LegalEntity } from '@models/legal-entity.model';

export class Account {
  id: string;
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
  tiedHouse: TiedHouseConnection;
  legalEntity: LegalEntity;

  isPartnership(): boolean {
    const isPartnership = [
      'GeneralPartnership',
      'LimitedPartnership',
      'LimitedLiabilityPartnership',
      'Partnership'].indexOf(this.businessType) !== -1;
    return isPartnership;
  }

  isPrivateCorporation(): boolean {
    const isPrivateCorp = [
      'PrivateCorporation',
      'UnlimitedLiabilityCorporation',
      'LimitedLiabilityCorporation'].indexOf(this.businessType) !== -1;
    return isPrivateCorp;
  }

  isPublicCorporation(): boolean {
    const isPublicCorp = ['PublicCorporation'].indexOf(this.businessType) !== -1;
    return isPublicCorp;
  }
}

