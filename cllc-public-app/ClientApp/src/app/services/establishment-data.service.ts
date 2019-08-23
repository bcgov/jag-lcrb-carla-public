import { Injectable } from '@angular/core';
import { DataService } from './data.service';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { KeyValue } from '../../../node_modules/@angular/common';


@Injectable({
    providedIn: 'root'
})
export class EstablishmentDataService extends DataService {

    apiPath = 'api/establishments';
    constructor(private http: HttpClient) {
        super();
    }

    public getEstablishmentsMap(): Observable<string> {
      return this.http.get<string>(this.apiPath + "/map", { headers: this.headers })
      .pipe(catchError(this.handleError));
    }
    

}
