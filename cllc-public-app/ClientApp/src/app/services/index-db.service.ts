import { Injectable } from '@angular/core';
import { SepApplication } from '@models/sep-application.model';
import Dexie, { PromiseExtended, Table } from 'dexie';

@Injectable()
export class IndexDBService {
  db: Dexie;
  applications: Table<any, number>;

  constructor() {
    this.db = new Dexie('SepApplicationDatabase');
    this.db.version(1).stores({ applications: '++id,eventName,agreeToTnC,dateAgreeToTnC,stepCompleted,status' });
    this.applications = this.db.table("applications");
  }

  public async addSepApplication(data: SepApplication) {
    let res = await this.applications.add(data);
    return res;
  }

  public getSepApplication(id: number) {
    return this.applications.where({ id }).first() as PromiseExtended<SepApplication>;
  }

  public deleteSepApplication(id: number) {
    return this.applications.delete(id);
  }

  public async getSepApplications() {
    return this.applications.toArray()  as PromiseExtended<SepApplication[]>;
  }
}
