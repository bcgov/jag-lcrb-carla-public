export class Endorsement {
  endorsementId: string;
  endorsementName: string;
  applicationTypeId: string;
  applicationTypeName: string;
  hoursOfServiceList: HoursOfService[];
  areaCapacity: number;
}
export class HoursOfService {
  dayOfWeek: number | undefined;
  startTimeHour: number | undefined;
  startTimeMinute: number | undefined;
  endTimeHour: number | undefined;
  endTimeMinute: number | undefined;
}


