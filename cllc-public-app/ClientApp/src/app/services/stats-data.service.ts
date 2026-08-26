import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Stat } from '@models/stat.model';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { DataService } from './data.service';

@Injectable({
  providedIn: 'root'
})
export class StatsDataService extends DataService {
  apiPath = 'api/stats/';

  constructor(private http: HttpClient) {
    super();
  }

  getStats(savedQueryName: string): Observable<Stat[]> {
    return this.http
      .get<Stat[]>(this.apiPath + encodeURIComponent(savedQueryName), { headers: this.headers })
      .pipe(catchError(this.handleError));
  }
}
