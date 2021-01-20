import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { HttpClient } from "@angular/common/http";
import { DataService } from "./data.service";
import { LicenceEvent } from "@models/licence-event.model";

@Injectable()
export class LicenceEventsService extends DataService {
  apiPath = "api/licenceevents/";

  constructor(private http: HttpClient) {
    super();
  }

  createLicenceEvent(licenceEvent: LicenceEvent): Observable<LicenceEvent> {
    return this.http.post<LicenceEvent>(this.apiPath, licenceEvent, { headers: this.headers });
  }

  getLicenceEvent(eventId: string): Observable<LicenceEvent> {
    return this.http.get<LicenceEvent>(this.apiPath + eventId, { headers: this.headers });
  }

  updateLicenceEvent(eventId: string, licenceEvent: LicenceEvent): Observable<LicenceEvent> {
    return this.http.put<LicenceEvent>(this.apiPath + eventId, licenceEvent, { headers: this.headers });
  }

  getLicenceEventsList(licenceId: string, num: number): Observable<LicenceEvent[]> {
    return this.http.get<LicenceEvent[]>(`${this.apiPath}list/${licenceId}/${num}`, { headers: this.headers });
  }

}
