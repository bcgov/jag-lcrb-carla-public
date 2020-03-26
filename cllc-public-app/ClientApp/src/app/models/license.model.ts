export class License {
  id: string;
  licenseType: string;
  licenseStatus: string;
  licenseNumber: string;
  // subcategory is generally blank, except for wine stores
  // using the same mispelled naming convention for consistency
  licenseSubCategory: string;
  establishmentName: string;
  establishmentAddress: string;

  establishmentAddressStreet: string;
  establishmentAddressCity: string;
  establishmentAddressPostalCode: string;
  establishmentParcelId: string;

  expiryDate: Date;
}
