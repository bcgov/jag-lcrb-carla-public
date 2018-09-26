import { Injectable } from '@angular/core';
import { Http, Headers, Response } from '@angular/http';
import 'rxjs/add/operator/toPromise';

import { AdoxioApplication } from '../models/adoxio-application.model';

@Injectable()
export class PaymentDataService {

  apiPath = 'api/payment/';
  submitPath = 'submit/';
  verifyPath = 'verify/';
  headers = new Headers({'Content-Type': 'application/json'});

  constructor(private http: Http) { }

  getPaymentSubmissionUrl(id: string) {
    return this.http.get(this.apiPath + this.submitPath + id, { headers: this.headers });
  }

  getInvoiceFeePaymentSubmissionUrl(id: string) {
    const invoiceFeePath = 'submit/licence-fee/';
    return this.http.get(this.apiPath + invoiceFeePath + id, { headers: this.headers });
  }

  getWorkerPaymentSubmissionUrl(workerId: string) {
    return this.http.get(`${this.apiPath}${this.submitPath}worker/${workerId}`, { headers: this.headers });
  }

  verifyPaymentSubmission(id: string) {
    return this.http.get(this.apiPath + this.verifyPath + id, { headers: this.headers });
  }

  verifyLicenceFeePaymentSubmission(id: string) {
    return this.http.get(this.apiPath + this.verifyPath + 'licence-fee/' + id, { headers: this.headers });
  }

  verifyWorkerPaymentSubmission(id: string) {
    return this.http.get(`${this.apiPath}${this.verifyPath}worker/${id}`, { headers: this.headers });
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
