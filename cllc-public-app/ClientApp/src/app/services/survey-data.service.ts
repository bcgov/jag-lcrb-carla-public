import { Injectable } from '@angular/core';
import { Http, Headers, Response } from '@angular/http';
import { HttpClient } from '@angular/common/http';
import { DataService } from './data.service';
import { catchError } from 'rxjs/operators';

@Injectable()
export class SurveyDataService extends DataService {
  constructor(private http: HttpClient) {
    super();
  }

  getSurveyData(clientId: string) {
    return this.http.get('api/survey/getResultByClient/' + clientId, {
      headers: this.headers
    }).pipe(catchError(this.handleError));
  }
}
