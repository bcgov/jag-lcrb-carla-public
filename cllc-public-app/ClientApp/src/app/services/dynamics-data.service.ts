import { Injectable } from "@angular/core";


import { DynamicsForm } from "@models/dynamics-form.model";
import { HttpClient } from "@angular/common/http";
import { DataService } from "./data.service";
import { Observable } from "rxjs";
import { catchError } from "rxjs/operators";

@Injectable()
export class DynamicsDataService extends DataService {
  constructor(private http: HttpClient) {
    super();
  }

  getForm(id: string): Observable<DynamicsForm> {
    return this.http.get<DynamicsForm>(`api/systemform/${id}`,
      {
        headers: this.headers
      }).pipe(catchError(this.handleError));
  }

  // load a record from Dynamics.
  getRecord(entity: string, recordId: string): Observable<any> {

    return this.http.get(`api/${entity}/${recordId}`,
      {
        headers: this.headers
      }).pipe(catchError(this.handleError));
  }

  createRecord(entity: string, data: any) {
    return this.http.post(`api/${entity}`,
      data,
      {
        headers: this.headers
      }).pipe(catchError(this.handleError));
  }


  updateRecord(entity: string, id: string, data: any) {
    return this.http.put(`api/${entity}/${id}`,
      data,
      {
        headers: this.headers
      }).pipe(catchError(this.handleError));
  }
}
