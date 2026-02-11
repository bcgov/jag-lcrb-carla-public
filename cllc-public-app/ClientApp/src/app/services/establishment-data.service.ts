import { Injectable } from "@angular/core";
import { DataService } from "./data.service";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { catchError } from "rxjs/operators";
import { Establishment } from "@models/establishment.model";


@Injectable({
  providedIn: "root"
})
export class EstablishmentDataService extends DataService {

  apiPath = "api/establishments";

  constructor(private http: HttpClient) {
    super();
  }

  getEstablishmentsMap(): Observable<any[]> {
    return this.http.get<any[]>(this.apiPath + "/map", { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  getEstablishmentsMapSearch(search: string): Observable<any[]> {
    return this.http.get<any[]>(this.apiPath + "/map?search=" + encodeURIComponent(search), { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  upEstablishment(establishment: Establishment): Observable<Establishment> {
    return this.http.put<Establishment>(this.apiPath + "/" + establishment.id, establishment, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  getLrs(): Observable<string> {
    return this.http.get<string>(this.apiPath + "/lrs", { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  getLrsSearch(search: string): Observable<string> {
    return this.http.get<string>(this.apiPath + "/lrs?search=" + encodeURIComponent(search), { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  getProposedLrs(): Observable<string> {
    return this.http.get<string>(this.apiPath + "/proposed-lrs", { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  getProposedLrsSearch(search: string): Observable<string> {
    return this.http.get<string>(this.apiPath + "/proposed-lrs?search=" + encodeURIComponent(search),
        { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

}
