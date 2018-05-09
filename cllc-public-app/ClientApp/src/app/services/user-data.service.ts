import { Injectable } from '@angular/core';
import { Http, Headers, Response } from "@angular/http";
import "rxjs/add/operator/toPromise";

import { User } from "../models/user.model";

@Injectable()
export class UserDataService {
   constructor(private http: Http) { }

   getCurrentUser() {
     let headers = new Headers();
     headers.append("Content-Type", "application/json");

     return this.http.get("api/user/current", {
       headers: headers
     })
       .toPromise()
       .then((res: Response) => {
         let data = res.json();
         let user = new User();
         user.id = data.id;
         user.email = data.email;
         user.firstname = data.firstname;
         user.lastname = data.lastname;
         user.name = data.name;
         user.businessname = data.businessname;
         user.isNewUser = data.isNewUser;
         user.isContactCreated = data.isContactCreated;
         user.isAccountCreated = data.isAccountCreated;
         user.contactid = data.contactid;
         user.accountid = data.accountid;
         return user;
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
