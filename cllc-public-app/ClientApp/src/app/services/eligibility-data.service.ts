import { Injectable } from "@angular/core";

import { HttpClient } from "@angular/common/http";
import { DataService } from "./data.service";
import { catchError } from "rxjs/operators";

import { EligibilityForm } from "@models/eligibility-form.model";

@Injectable()
export class EligibilityFormDataService extends DataService {

  apiPath = "api/eligibility/";

  constructor(private http: HttpClient) {
    super();
  }


  submit(eligibilityForm: EligibilityForm) {
    return this.http.post(this.apiPath + "submit", eligibilityForm, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

}
