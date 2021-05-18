import { Injectable } from '@angular/core';
import { SepApplication } from '@models/sep-application.model';
import { SepLocation } from '@models/sep-location.model';
import { SepSchedule } from '@models/sep-schedule.model';
import { SepServiceArea } from '@models/sep-service-are.model';
import Dexie, { Collection, PromiseExtended, Table } from 'dexie';

@Injectable()
export class IndexDBService {
  db: Dexie;
  applications: Table<SepApplication, number>;
  locations: Table<SepLocation, number>;
  serviceAreas: Table<SepServiceArea, number>;
  eventDates: Table<SepSchedule, number>;

  constructor() {
    this.db = new Dexie('SepApplicationDatabase', { addons: [] });
    this.db.version(2).stores({
      applications: '++id,specialEventId, tempJobNumber,dateCreated,lastUpdated,eventName,applicantInfo,agreeToTnC,dateAgreedToTnC,stepsCompleted,eventStatus,totalServings,invoiceTrigger,eligibilityAtPrivateResidence,eligibilityMajorSignificance,eligibilityMajorSignificanceRational,eligibilityLocalSignificance,permitNumber,isTastingEvent,isBeerGarden,numMaxGuest',
    });
    this.applications = this.db.table("applications");

  }

  public async saveSepApplication(data: SepApplication) {
    // Save Application
    let applicationId = data.id;
    if (applicationId) { // update if exists
      await this.applications.update(applicationId, data);
    } else { // create and get new id
      applicationId = await this.applications.add(data);
    }

    // // Save locations
    // data?.eventLocations?.forEach(async (location: SepLocation) => {
    //   let locId = location.id;
    //   if (locId) { // update if exists
    //     await this.locations.update(locId, { ...location, sepApplicationIdFk: applicationId });
    //   } else { // create location
    //     locId = await this.locations.add({ ...location, sepApplicationIdFk: applicationId });
    //   }

    //   // save service areas
    //   location.serviceAreas.forEach(async (area: SepServiceArea) => {
    //     let areaId = area.id;
    //     if (areaId) { // update if exists
    //       await this.serviceAreas.update(areaId, { ...area, locationIdFk: locId });
    //     } else { // create area
    //       areaId = await this.serviceAreas.add({ ...area, locationIdFk: locId });
    //     }

    //     // save event dates
    //     area?.eventDates?.forEach(async (eventDate: SepSchedule) => {
    //       if (eventDate.id) { // update if exists
    //         await this.eventDates.update(eventDate.id, { ...eventDate, serviceAreaIdFk: areaId });
    //       } else { // create otherwise
    //         await this.eventDates.add({ ...eventDate, serviceAreaIdFk: areaId });
    //       }
    //     });
    //   });
    // });
    return applicationId;
  }

  public async getSepApplication(id: number) {
    let app = await this.applications.where({ id }).first();
    return app;
  }

  public deleteSepApplication(id: number) {
    return this.applications.delete(id);
  }

  public async getSepApplications() {
    return this.applications.toArray() as PromiseExtended<SepApplication[]>;
  }
}
