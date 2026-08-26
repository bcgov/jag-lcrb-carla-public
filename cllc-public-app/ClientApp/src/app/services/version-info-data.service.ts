import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { VersionInfo } from '@models/version-info.model';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { DataService } from './data.service';

@Injectable()
export class VersionInfoDataService extends DataService {
  apiPath = 'api/ApplicationVersionInfo';
  headers = new HttpHeaders({
    'Content-Type': 'application/json'
  });

  constructor(private http: HttpClient) {
    super();
  }

  getVersionInfo(): Observable<VersionInfo> {
    return this.http.get<VersionInfo>(this.apiPath, { headers: this.headers }).pipe(catchError(this.handleError));
  }
}
