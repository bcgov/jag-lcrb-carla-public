import { Injectable } from "@angular/core";
import { AppState } from "@app/app-state/models/app-state";
import { SepApplication } from "@models/sep-application.model";
import { SepLocation } from "@models/sep-location.model";
import { SepSchedule } from "@models/sep-schedule.model";
import { SepServiceArea } from "@models/sep-service-area.model";
import { User } from "@models/user.model";
import { Store } from "@ngrx/store";
import Dexie, { PromiseExtended, Table } from "dexie";

@Injectable()
export class IndexedDBService {
  db: Dexie;
  applications: Table<SepApplication, number>;
  locations: Table<SepLocation, number>;
  serviceAreas: Table<SepServiceArea, number>;
  eventDates: Table<SepSchedule, number>;
  userId: string;

  constructor(private store: Store<AppState>) {
    store.select(state => state.currentUserState.currentUser)
      .subscribe(user => {
        this.userId = `${user.accountid}${user.contactid}`;
      });
    this.db = new Dexie("SepApplicationDatabase");
    this.db.version(3).stores({
      applications: "++localId, agreeToTnC, applicantInfo, dateAgreedToTnC, dateCreated, eligibilityAtPrivateResidence, eligibilityLocalSignificance, eligibilityMajorSignificance, eligibilityMajorSignificanceRational, eventName, eventStatus, id, invoiceTrigger, isBeerGarden,numMaxGuest lastUpdated, permitNumber,isTastingEvent, stepsCompleted, tempJobNumber, totalServings",
    });
    this.db.version(4).stores({
      applications: "++localId, agreeToTnC, applicantInfo, dateAgreedToTnC, dateCreated, eligibilityAtPrivateResidence, eligibilityLocalSignificance, eventName, eventStatus, invoiceTrigger, isBeerGarden, isMajorSignificance, isMajorSignificanceRational, isTastingEvent, lastUpdated, numMaxGuest permitNumber, stepsCompleted, tempJobNumber, totalServings",
    });
    this.db.version(4).stores({
      applications: "++localId, userId, agreeToTnC, applicantInfo, dateAgreedToTnC, dateCreated, eligibilityAtPrivateResidence, eligibilityLocalSignificance, eventName, eventStatus, invoiceTrigger, isBeerGarden, isMajorSignificance, isMajorSignificanceRational, isTastingEvent, lastUpdated, numMaxGuest permitNumber, stepsCompleted, tempJobNumber, totalServings",
    });
    this.applications = this.db.table("applications");

  }

  public async saveSepApplication(data: SepApplication) {
    // Save Application
    let applicationId = data.localId;
    data.userId = this?.userId;
    if (applicationId) { // update if exists
      await this.applications.update(applicationId, data);
    } else { // create and get new id
      applicationId = await this.applications.add(data);
    }
    return applicationId;
  }

  public async getSepApplication(localId: number) {
    const app = await this.applications.where({ localId }).first();
    return app;
  }

  public deleteSepApplication(id: number) {
    return this.applications.delete(id);
  }

  public async getSepApplications() {
    return this.applications.where({ userId: this?.userId }).toArray() as PromiseExtended<SepApplication[]>;
  }
}
