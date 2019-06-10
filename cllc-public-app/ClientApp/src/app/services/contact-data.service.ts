import { Injectable } from '@angular/core';

import { HttpHeaders, HttpClient } from '@angular/common/http';

import { Contact } from '../models/contact.model';
import { DataService } from './data.service';
import { catchError } from 'rxjs/operators';

@Injectable()
export class ContactDataService extends DataService {

  apiPath = 'api/contact/';

  constructor(private http: HttpClient) {
    super();
   }

  public getContact(contactId: string) {
    return this.http.get<Contact>(this.apiPath + contactId, { headers: this.headers })
    .pipe(catchError(this.handleError));
  }

  public createContact(contact: Contact) {
    return this.http.post<Contact>(this.apiPath, contact, { headers: this.headers })
    .pipe(catchError(this.handleError));
  }

  public createWorkerContact(contact: Contact) {
    return this.http.post<Contact>(this.apiPath + 'worker', contact, { headers: this.headers })
    .pipe(catchError(this.handleError));
  }
  public updateContact(contact: Contact) {
    return this.http.put<Contact>(this.apiPath + contact.id, contact, { headers: this.headers })
    .pipe(catchError(this.handleError));
  }

}
