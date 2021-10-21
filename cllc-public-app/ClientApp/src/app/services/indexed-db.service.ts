import { Injectable } from "@angular/core";
import { AppState } from "@app/app-state/models/app-state";
import { SepApplication } from "@models/sep-application.model";
import { SepLocation } from "@models/sep-location.model";
import { SepSchedule } from "@models/sep-schedule.model";
import { SepServiceArea } from "@models/sep-service-area.model";
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
    this.db.version(1).stores({
      applications: "++localId, id, userId",
    });
    this.applications = this.db.table("applications");

  }

  public async saveSepApplication(data: SepApplication) {
    // Save Application
    let applicationId = data.localId;
    data.userId = this?.userId;
    /*
    if (!applicationId && data.id)
    {
        const apps = await this.applications.where("id").equals(data.id).toArray();
        if (apps && apps.length > 0)
        {
          applicationId = apps[0].localId;
          data.localId = applicationId;
        }
    }
    */
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
