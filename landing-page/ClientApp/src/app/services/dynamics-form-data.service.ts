import { Injectable } from '@angular/core';
import { FileSystemItem } from '@models/file-system-item.model';
import { Application } from '@models/application.model';
import { ApplicationSummary } from '@models/application-summary.model';
import { catchError } from 'rxjs/operators';
import { HttpClient, HttpHeaders } from '@angular/common/http';
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
