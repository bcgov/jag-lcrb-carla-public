export class Endorsement {
  endorsementId: string;
  endorsementName: string;
  applicationTypeId: string;
  applicationTypeName: string;
  hoursOfServiceList: HoursOfService[];
  /**
   * The sum total capacity of all service areas under this endorsement.
   */
  areaCapacity: number;
}
export class HoursOfService {
  dayOfWeek: number | undefined;
  startTimeHour: number | undefined;
  startTimeMinute: number | undefined;
  endTimeHour: number | undefined;
  endTimeMinute: number | undefined;
}


