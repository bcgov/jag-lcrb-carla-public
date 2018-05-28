import { Injectable } from '@angular/core';
import { Http, Headers, Response } from "@angular/http";

import { Newsletter } from "../models/newsletter.model";

@Injectable()
export class AdoxioLegalEntityDataService {
   constructor(private http: Http) { }

   signup(slug: any, email: any) {
     let headers = new Headers();
     headers.append("Content-Type", "application/json");

     return this.http.post("api/adoxiolegalentity/" + slug + "/subscribe?email=" + email, {
       headers: headers
     })
       .toPromise()
       .then((res: Response) => {
         // do nothing
       })
       .catch(this.handleError);
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
