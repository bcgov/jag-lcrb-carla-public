import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { DynamicsForm } from '../models/dynamics-form.model';
import { DataService } from './data.service';

@Injectable()
export class DynamicsFormDataService extends DataService {
  apiPath = 'api/forms/';

  constructor(private http: HttpClient) {
    super();
  }

  /**
   * Get Dynamics Form
   * */
  getDynamicsForm(formId: string): Observable<DynamicsForm> {
    return this.http
      .get<DynamicsForm>(this.apiPath + formId, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }
}
