import { Injectable } from '@angular/core';

import { HttpHeaders, HttpClient } from '@angular/common/http';

import { DynamicsContact } from '../models/dynamics-contact.model';
import { DataService } from './data.service';
import { catchError } from 'rxjs/operators';

@Injectable()
export class ContactDataService extends DataService {

  apiPath = 'api/contact/';

  constructor(private http: HttpClient) {
    super();
   }

  public getContact(contactId: string) {
    return this.http.get<DynamicsContact>(this.apiPath + contactId, { headers: this.headers })
    .pipe(catchError(this.handleError));
  }

  public createContact(contact: DynamicsContact) {
    return this.http.post<DynamicsContact>(this.apiPath, contact, { headers: this.headers })
    .pipe(catchError(this.handleError));
  }

  public createWorkerContact(contact: DynamicsContact) {
    return this.http.post<DynamicsContact>(this.apiPath + 'worker', contact, { headers: this.headers })
    .pipe(catchError(this.handleError));
  }
  public updateContact(contact: DynamicsContact) {
    return this.http.put<DynamicsContact>(this.apiPath + contact.id, contact, { headers: this.headers })
    .pipe(catchError(this.handleError));
  }

}
