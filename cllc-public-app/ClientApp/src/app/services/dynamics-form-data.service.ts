import { Injectable } from '@angular/core';
import { catchError } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DataService } from './data.service';
import { DynamicsForm } from '../models/dynamics-form.model';

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
        return this.http.get<DynamicsForm>(this.apiPath + formId, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }


}
