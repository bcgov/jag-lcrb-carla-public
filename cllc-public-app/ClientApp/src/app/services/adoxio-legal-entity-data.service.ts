import { Injectable } from '@angular/core';
import { Http, Headers, Response } from "@angular/http";

@Injectable()
export class AdoxioLegalEntityDataService {
   constructor(private http: Http) { }

  post(data: any) {
    let headers = new Headers();
    headers.append("Content-Type", "application/json");
    console.log('===== AdoxioLegalEntityDataService =====');
    console.log(data.controls);

    return this.http.post("api/adoxiolegalentity/", data.controls, {
      headers: headers
    })
      .subscribe(
        res => {
          console.log(res);
        },
        err => {
          console.log("Error occured");
          this.handleError(err);
        }
      );
  }

   private handleError(error: Response | any) {
     let errMsg: string;
     if (error instanceof Response) {
       const body = error.json() || "";
       const err = body.error || JSON.stringify(body);
       errMsg = `${error.status} - ${error.statusText || ""} ${err}`;
     } else {
       errMsg = error.message ? error.message : error.toString();
     }
     console.error(errMsg);
     return Promise.reject(errMsg);
   }
}
