
export enum AreaCategory {
  Service = 845280000,
  OutsideArea = 845280001,
  Capacity = 845280002
}

export class ServiceArea {
    // picklist
    areaCategory: number;
    areaNumber: number;
    areaLocation: string;
    isIndoor: boolean;
    isOutdoor: boolean;
    isPatio: boolean;
    capacity: string;
}
