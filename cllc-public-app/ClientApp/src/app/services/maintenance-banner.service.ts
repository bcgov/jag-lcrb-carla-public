import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";

export interface MaintenanceBanner {
    bannerEnabled: boolean;
    bannerText: string;
    bannerStartDate: string;
    bannerEndDate: string;
}

@Injectable({
    providedIn: 'root'
})

export class MaintenanceBannerService {
    apiPath = "api/banner";

    constructor(private http: HttpClient) { }

    getBanner(): Observable<MaintenanceBanner> {
        return this.http.get<MaintenanceBanner>(`${this.apiPath}`);
    }
}