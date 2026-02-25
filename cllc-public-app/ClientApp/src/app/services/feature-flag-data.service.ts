import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { FeatureFlag } from '@models/feature-flag.model';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { DataService } from './data.service';

@Injectable({
  providedIn: 'root'
})
export class FeatureFlagDataService extends DataService {
  apiPath = 'api/features';

  constructor(private http: HttpClient) {
    super();
  }

  getFeatureFlags(): Observable<FeatureFlag[]> {
    return this.http.get<FeatureFlag[]>(this.apiPath, { headers: this.headers }).pipe(catchError(this.handleError));
  }
}
