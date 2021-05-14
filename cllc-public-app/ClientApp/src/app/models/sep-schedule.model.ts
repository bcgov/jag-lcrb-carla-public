export class SepSchedule {
    eventScheduleId: string; // server side primary key
    specialEventId: string; // server side foreign key
    locationId: string; // server side foreign key
    eventStart: Date;
    eventEnd: Date;
    serviceStart: Date;
    serviceEnd: Date;

}