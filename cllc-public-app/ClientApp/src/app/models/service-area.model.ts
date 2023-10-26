
export enum AreaCategory {
  Service = 845280000,
  OutsideArea = 845280001,
  Capacity = 845280002
}

export class ServiceArea {
  id: string;
  areaCategory: number;  // picklist
  areaNumber: number;
  areaLocation: string;
  isIndoor: boolean;
  isOutdoor: boolean;
  isPatio: boolean;
  capacity: string;
}
