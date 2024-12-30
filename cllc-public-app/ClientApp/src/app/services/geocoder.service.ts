import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { GeocoderModel } from '@models/geocoder.model';

// Define the interface for the simplified response
export interface Address {
  fullAddress: String,
  streetName: string;
  streetQualifier: string;
  localityName: string;
  provinceCode: string;
  siteID: string;
}

@Injectable({
  providedIn: 'root'
})
export class AddressService {
  apiPath = "api/geocoder";

  constructor(private http: HttpClient) { }

  getAddressData(query: string): Observable<Address[]> {
    return this.http.get<GeocoderModel>(`${this.apiPath}/get-civic-address?queryParam=${query}`).pipe(
      map(response =>
        response.features.map(feature => ({
          fullAddress: feature.properties.fullAddress,
          streetName: feature.properties.streetName,
          streetQualifier: feature.properties.streetQualifier,
          localityName: feature.properties.localityName,
          provinceCode: feature.properties.provinceCode,
          siteID: feature.properties.siteID
        }))
      )
    );
  }

  getPid(siteId?: String): Observable<string> {
    return this.http.get<string>(`${this.apiPath}/get-pid?siteId=${siteId}`);
  }

}