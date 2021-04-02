import { Injectable } from '@angular/core';
import Dexie, { Table } from 'dexie';

@Injectable()
export class IndexDBService {
  db: Dexie;
  applications: Table<any, number>;

  constructor() {
    this.db = new Dexie('SepApplicationDatabase');
    this.db.version(1).stores({ applications: '++id,eventName,agreeToTnC,dateAgreeToTnC,stepCompleted,status' });
    this.applications = this.db.table("applications");
  }

  public async addSepApplication(data: any) {
    await this.db.table("applications").add(data);
  }

  public getSepApplication(id: number) {
    return this.applications.where({ id }).first();
  }
  public getSepApplications() {
    return this.applications.toArray();
  }
}
