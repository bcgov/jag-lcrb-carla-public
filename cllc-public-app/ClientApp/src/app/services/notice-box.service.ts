import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";

export interface NoticeBox {
    noticeBoxEnabled: boolean;
    noticeBoxTitle: string;
    noticeBoxText: string;
}

@Injectable({
    providedIn: 'root'
})

export class NoticeBoxService {
    apiPath = "api/noticebox";

    constructor(private http: HttpClient) { }

    getNoticeBox(): Observable<NoticeBox> {
        return this.http.get<NoticeBox>(`${this.apiPath}`);
    }
}
