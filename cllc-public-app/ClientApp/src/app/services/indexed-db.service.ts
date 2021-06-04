import { Injectable } from '@angular/core';
import { SepApplication } from '@models/sep-application.model';
import { SepLocation } from '@models/sep-location.model';
import { SepSchedule } from '@models/sep-schedule.model';
import { SepServiceArea } from '@models/sep-service-area.model';
import Dexie, { PromiseExtended, Table } from 'dexie';

@Injectable()
export class IndexedDBService {
  db: Dexie;
  applications: Table<SepApplication, number>;
  locations: Table<SepLocation, number>;
  serviceAreas: Table<SepServiceArea, number>;
  eventDates: Table<SepSchedule, number>;

  constructor() {
    this.db = new Dexie('SepApplicationDatabase');
    this.db.version(3).stores({
      applications: '++localId, id, tempJobNumber,dateCreated,lastUpdated,eventName,applicantInfo,agreeToTnC,dateAgreedToTnC,stepsCompleted,eventStatus,totalServings,invoiceTrigger,eligibilityAtPrivateResidence,isMajorSignificance,isMajorSignificanceRational,eligibilityLocalSignificance,permitNumber,isTastingEvent,isBeerGarden,numMaxGuest',
    });
    this.applications = this.db.table("applications");

  }

  public async saveSepApplication(data: SepApplication) {
    // Save Application
    let applicationId = data.localId;
    if (applicationId) { // update if exists
      await this.applications.update(applicationId, data);
    } else { // create and get new id
      applicationId = await this.applications.add(data);
    }
    return applicationId;
  }

  public async getSepApplication(localId: number) {
    let app = await this.applications.where({ localId }).first();
    return app;
  }

  public deleteSepApplication(id: number) {
    return this.applications.delete(id);
  }

  public async getSepApplications() {
    return this.applications.toArray() as PromiseExtended<SepApplication[]>;
  }
}