import { Injectable } from '@angular/core';
import { Http, Headers, Response } from '@angular/http';


import { Application } from '../models/application.model';
import { HttpClient } from '@angular/common/http';
import { DataService } from './data.service';
import { catchError } from 'rxjs/operators';

@Injectable()
export class PaymentDataService extends DataService {

  apiPath = 'api/payment/';
  submitPath = 'submit/';
  verifyPath = 'verify/';

  constructor(private http: HttpClient) {
    super();
  }

  getPaymentSubmissionUrl(id: string) {
    return this.http.get(this.apiPath + this.submitPath + id, { headers: this.headers })
    .pipe(catchError(this.handleError));
  }

  getInvoiceFeePaymentSubmissionUrl(id: string) {
    const invoiceFeePath = 'submit/licence-fee/';
    return this.http.get(this.apiPath + invoiceFeePath + id, { headers: this.headers })
    .pipe(catchError(this.handleError));
  }

  getWorkerPaymentSubmissionUrl(workerId: string) {
    return this.http.get(`${this.apiPath}${this.submitPath}worker/${workerId}`, { headers: this.headers })
    .pipe(catchError(this.handleError));
  }

  verifyPaymentSubmission(id: string) {
    return this.http.get(this.apiPath + this.verifyPath + id, { headers: this.headers })
    .pipe(catchError(this.handleError));
  }

  verifyLicenceFeePaymentSubmission(id: string) {
    return this.http.get(this.apiPath + this.verifyPath + 'licence-fee/' + id, { headers: this.headers })
    .pipe(catchError(this.handleError));
  }

  verifyWorkerPaymentSubmission(id: string) {
    return this.http.get(`${this.apiPath}${this.verifyPath}worker/${id}`, { headers: this.headers })
    .pipe(catchError(this.handleError));
  }

}
