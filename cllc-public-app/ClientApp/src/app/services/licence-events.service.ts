import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { DataService } from './data.service';
import { LicenceEvent } from '@models/licence-event.model';

@Injectable()
export class LicenceEventsService extends DataService {
  apiPath = 'api/licenceevents/';

  constructor(private http: HttpClient) {
    super();
  }

  createLicenceEvent(licenceEvent: LicenceEvent): Observable<LicenceEvent> {
    return this.http.post<LicenceEvent>(this.apiPath, licenceEvent, { headers: this.headers });
  }

}
