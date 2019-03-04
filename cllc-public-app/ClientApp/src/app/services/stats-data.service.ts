import { Injectable } from '@angular/core';
import { DynamicsAccount } from '../models/dynamics-account.model';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DataService } from './data.service';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class StatsDataService extends DataService {

  apiPath = 'api/stats';

  constructor(private http: HttpClient) {
    super();
  }

  public getStats(): Observable<DynamicsAccount> {
    return this.http.get<DynamicsAccount>(this.apiPath, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

}
