import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError } from 'rxjs/operators';
import { DataService } from './data.service';

@Injectable()
export class SurveyDataService extends DataService {
  constructor(private http: HttpClient) {
    super();
  }

  getSurveyData(clientId: string) {
    return this.http
      .get(`api/survey/getResultByClient/${clientId}`, {
        headers: this.headers
      })
      .pipe(catchError(this.handleError));
  }
}
