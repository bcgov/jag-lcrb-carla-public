import { Injectable } from '@angular/core';
import { DataService } from './data.service';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Establishment } from '@models/establishment.model';


@Injectable({
    providedIn: 'root'
})
export class EstablishmentDataService extends DataService {

    apiPath = 'api/establishments';
    constructor(private http: HttpClient) {
        super();
    }

    public getEstablishmentsMap(): Observable<string> {
      return this.http.get<string>(this.apiPath + '/map', { headers: this.headers })
      .pipe(catchError(this.handleError));
    }

    public getEstablishmentsMapSearch(search: string): Observable<string> {
      return this.http.get<string>(this.apiPath + '/map?search=' + encodeURIComponent(search), { headers: this.headers })
      .pipe(catchError(this.handleError));
    }

    public upEstablishment(establishment: Establishment): Observable<Establishment> {
      return this.http.put<Establishment>(this.apiPath + '/' + establishment.id, establishment, { headers: this.headers })
        .pipe(catchError(this.handleError));
    }

}
