export enum OffsiteStorageStatus {
  Added = 1,
  Removed = 845280000,
}

export class OffsiteStorage {
  id: string; // guid
  name: string;
  street1: string;
  street2: string;
  city: string;
  province: string;
  status: OffsiteStorageStatus;
}
