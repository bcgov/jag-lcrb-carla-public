import { Establishment } from './establishment.model';

export const AreaCategory = [
    {
      label: '???',
      value: 845280000,
    }
];

export class ServiceArea {
    // picklist
    // areaCategory: number;
    areaNumber: number;
    areaLocation: string;
    isIndoor: boolean;
    isOutdoor: boolean;
    isPatio: boolean;
    capacity: string;
}
