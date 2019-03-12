import { Injectable } from '@angular/core';
import { DynamicsAccount } from '../models/dynamics-account.model';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DataService } from './data.service';
import { catchError } from 'rxjs/operators';
import { Stat } from '../models/stat.model';

@Injectable({
  providedIn: 'root'
})
export class StatsDataService extends DataService {

  apiPath = 'api/stats/';

  constructor(private http: HttpClient) {
    super();
  }

  public getStats(savedQueryName: string): Observable<Stat[]> {
    return this.http.get<Stat[]>(this.apiPath + encodeURIComponent(savedQueryName), { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

}
