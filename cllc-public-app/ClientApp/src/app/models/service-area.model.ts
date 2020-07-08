import { Establishment } from './establishment.model';

export const AreaCategory = [
    {
      label: '???',
      value: 845280000,
    }
];

export class ServiceArea {
    licenceId: string;
    applicationId: string;
    establishment: Establishment;
    // picklist
    areaCategory: number;
    areaNumber: number;
    areaLocation: string;
    isIndoor: boolean;
    isOutdoor: boolean;
    isPatio: boolean;
    capacity: number;
    dateAdded: Date;
    dateUpdated: Date;
}