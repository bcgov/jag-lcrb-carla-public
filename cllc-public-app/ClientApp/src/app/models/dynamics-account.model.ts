import { DynamicsContact } from './dynamics-contact.model';

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
  mailingAddressCity: string;
  mailingAddressProvince: string;
  mailingAddressCountry: string;
  mailingAddressPostalCode: string;

  primarycontact: DynamicsContact;

  businessType: string;
}
