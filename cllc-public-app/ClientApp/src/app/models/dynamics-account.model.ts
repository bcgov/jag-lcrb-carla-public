import { DynamicsContact } from './dynamics-contact.model';
import { TiedHouseConnection } from '@models/tied-house-connection.model';

export interface DynamicsAccount {
  id: string;
  name: string;
  description: string;
  bcIncorporationNumber: string;
  dateOfIncorporationInBC: Date;
  businessNumber: string;
  pstNumber: string;
  contactEmail: string;
  contactPhone: string;

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

  primarycontact: DynamicsContact;

  businessType: string;

  tiedHouse: TiedHouseConnection;
}
