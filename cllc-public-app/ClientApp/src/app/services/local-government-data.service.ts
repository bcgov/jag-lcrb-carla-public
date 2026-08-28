import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { DataService } from './data.service';

@Injectable({
  providedIn: 'root'
})
export class LocalGovernmentDataService extends DataService {
  apiPath = 'api/localgovernments/';

  constructor(private http: HttpClient) {
    super();
  }

  getAutocomplete(search: string): Observable<any[]> {
    return this.http
      .get<any[]>(this.apiPath + `autocomplete?name=${search}`, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }
}
