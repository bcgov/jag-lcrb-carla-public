import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { catchError } from "rxjs/operators";
import { Observable } from "rxjs";
import { DataService } from "./data.service";
import { AnnualVolume } from "@models/annual-volume.model";

@Injectable()
export class AnnualVolumeService extends DataService {

  constructor(private http: HttpClient) {
    super();
  }

  updateAnnualVolumeForApplication(data: AnnualVolume, applicationId: string): Observable<AnnualVolume> {
    const apiPath = `api/annualvolume/application/${applicationId}`;
    return this.http.post<AnnualVolume>(apiPath, data, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

}
