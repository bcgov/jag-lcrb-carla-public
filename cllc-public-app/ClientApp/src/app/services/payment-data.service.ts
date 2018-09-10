import { Injectable } from '@angular/core';
import { Http, Headers, Response } from '@angular/http';
import 'rxjs/add/operator/toPromise';

import { AdoxioApplication } from '../models/adoxio-application.model';

@Injectable()
   export class PaymentDataService {

   apiPath = 'api/payment/';
   submitPath = 'submit/';
   verifyPath = 'verify/';

   constructor(private http: Http) { }

   getPaymentSubmissionUrl(id: string) {
     const headers = new Headers();
     headers.append('Content-Type', 'application/json');

     // call API
     // console.log("===== PaymentService.submit: ", id);
     return this.http.get(this.apiPath + this.submitPath + id, { headers: headers });
   }
   getWorkerPaymentSubmissionUrl(workerId: string) {
     const headers = new Headers();
     headers.append('Content-Type', 'application/json');

     // call API
     // console.log("===== PaymentService.submit: ", id);
     return this.http.get(`${this.apiPath}${this.submitPath}worker/${workerId}`, { headers: headers });
   }

   verifyPaymentSubmission(id: string) {
     const headers = new Headers();
     headers.append('Content-Type', 'application/json');

     // call API
     // console.log("===== PaymentService.verify: ", id);
     return this.http.get(this.apiPath + this.verifyPath + id, { headers: headers });
   }

   verifyWorkerPaymentSubmission(id: string) {
     const headers = new Headers();
     headers.append('Content-Type', 'application/json');

     // call API
     // console.log("===== PaymentService.verify: ", id);
     return this.http.get(`${this.apiPath}${this.verifyPath}worker/${id}`, { headers: headers });
   }

   private handleError(error: Response | any) {
     let errMsg: string;
     if (error instanceof Response) {
       const body = error.json() || '';
       const err = body.error || JSON.stringify(body);
       errMsg = `${error.status} - ${error.statusText || ''} ${err}`;
     } else {
       errMsg = error.message ? error.message : error.toString();
     }
     console.error(errMsg);
     return Promise.reject(errMsg);
   }
}
