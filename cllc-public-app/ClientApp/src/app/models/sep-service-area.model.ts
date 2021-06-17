export class SepServiceArea {
    id: string; // server side primary key
    sepLocationId: string;
    specialEventId: string;
    minorPresent: boolean;
    licencedAreaMaxNumberOfGuests: number;
    maximumNumberOfGuests: number;
    numberOfMinors: number;
    licencedAreaNumberOfMinors: number;
    setting: number;
    statusCode: number;
    stateCode: number;
    eventName: string;
    isOutdoors: boolean;
    licencedAreaDescription: string;

    constructor(area: IAreaFormValue) {
      if (area) {
          this.setting = area.setting;
          this.licencedAreaDescription = area.licencedAreaDescription;
          this.maximumNumberOfGuests = area.maximumNumberOfGuests;
          this.numberOfMinors = area.numberOfMinors;
          this.minorPresent = area.minorPresent;
      }
  }
}

export interface IAreaFormValue {
  setting: number
  licencedAreaDescription: string;
  maximumNumberOfGuests: number;
  numberOfMinors: number;
  minorPresent: boolean;
}
