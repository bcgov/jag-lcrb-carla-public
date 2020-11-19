export enum OffsiteStorageStatus {
  Active = 1,
  Removed = 845280000,
}

export class OffsiteStorage {
  id: string; // guid
  name: string;
  street1: string;
  street2: string;
  city: string;
  province: string;
  postalCode: string;
  status: OffsiteStorageStatus;
}
