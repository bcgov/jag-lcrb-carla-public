import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { EligibilityForm } from '@models/eligibility-form.model';
import { catchError } from 'rxjs/operators';
import { DataService } from './data.service';

@Injectable()
export class EligibilityFormDataService extends DataService {
  apiPath = 'api/eligibility/';

  constructor(private http: HttpClient) {
    super();
  }

  submit(eligibilityForm: EligibilityForm) {
    return this.http
      .post(this.apiPath + 'submit', eligibilityForm, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }
}
