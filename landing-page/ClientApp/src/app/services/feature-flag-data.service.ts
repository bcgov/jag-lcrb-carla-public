import { Injectable } from '@angular/core';
import { DataService } from './data.service';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class FeatureFlagDataService extends DataService {
  apiPath = 'api/features';
  constructor(private http: HttpClient) {
    super();
  }

  public getFeatureFlags(): Observable<string[]> {
    return this.http.get<string[]>(this.apiPath , { headers: this.headers })
    .pipe(catchError(this.handleError));
  }
}
