import { Injectable } from "@angular/core";

import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { DataService } from "./data.service";
import { catchError } from "rxjs/operators";

@Injectable({
  providedIn: "root"
})
export class PoliceJurisdictionDataService extends DataService {

  apiPath = "api/policejurisdictions/";

  constructor(private http: HttpClient) {
    super();
  }


  getAutocomplete(search: string): Observable<any[]> {
    return this.http.get<any[]>(this.apiPath + `autocomplete?name=${search}`, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }
}
