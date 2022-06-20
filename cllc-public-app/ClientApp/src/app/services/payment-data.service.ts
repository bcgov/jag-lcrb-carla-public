import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { DataService } from "./data.service";
import { catchError } from "rxjs/operators";

type PaymentType =
'default' |
'worker' |
'licenceFee' |
'primaryInvoice' |
'secondaryInvoice' |
'specialEventInvoice'
;
/**
 * 
 */
@Injectable()
export class PaymentDataService extends DataService {
  apiPath = "api/payment/";
  submitPath = "submit/";
  verifyPath = "verify/";

  readonly paymentTypes = {
    default: {
      getPaymentURI: (id) => `api/payment/submit/${id}`,
      verifyPaymentURI: (id) => `api/payment/verify/${id}`
    },
    worker: {
      getPaymentURI: (id) => `api/payment/submit/worker/${id}`,
      verifyPaymentURI: (id) => `api/payment/verify/worker/${id}`
    },
    licenceFee: {
      getPaymentURI: (id) => `api/payment/submit/licence-fee/${id}`,
      verifyPaymentURI: (id) => `api/payment/verify/licence-fee/${id}`
    },
    primaryInvoice: {
      getPaymentURI: (id) => `api/payment/payment-uri/primary/${id}`,
      verifyPaymentURI: (id) => `api/payment/verify-by-invoice-type/primary/${id}`
    },
    secondaryInvoice: {
      getPaymentURI: (id) => `api/payment/payment-uri/secondary/${id}`,
      verifyPaymentURI: (id) => `api/payment/verify-by-invoice-type/secondary/${id}`
    },
    specialEventInvoice: {
      getPaymentURI: (id) => `api/payment/submit/sep-application/${id}`,
      verifyPaymentURI: (id) => `api/payment/verify/sep-application/${id}`
    }
  };

  constructor(private http: HttpClient) {
    super();
  }

  getPaymentURI(paymentType: PaymentType, id: string) {
    const payType = this.paymentTypes[paymentType];
    if (!payType) {
      return;
    }
    return this.http.get(payType.getPaymentURI(id), { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  verifyPaymentURI(paymentType: PaymentType, id: string) {
    const payType = this.paymentTypes[paymentType];
    if (!payType) {
      return;
    }
    return this.http.get(payType.verifyPaymentURI(id), { headers: this.headers })
      .pipe(catchError(this.handleErrorWith503));
  }

  getPaymentSubmissionUrl(id: string) {
    return this.http.get(this.apiPath + this.submitPath + id, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  getInvoiceFeePaymentSubmissionUrl(id: string) {
    const invoiceFeePath = "submit/licence-fee/";
    return this.http.get(this.apiPath + invoiceFeePath + id, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }
  payOutstandingPriorBalanceInvoicePaymentSubmissionUrl(id: string) {
    const additionalFeePath = "submit/outstanding-prior-balance-invoice/";
    return this.http.get(this.apiPath + additionalFeePath + id, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }
  getWorkerPaymentSubmissionUrl(workerId: string) {
    return this.http.get(`${this.apiPath}${this.submitPath}worker/${workerId}`, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  verifyPaymentSubmission(id: string) {
    return this.http.get(this.apiPath + this.verifyPath + id, { headers: this.headers })
      .pipe(catchError(this.handleErrorWith503));
  }

  verifyLicenceFeePaymentSubmission(id: string) {
    return this.http.get(this.apiPath + this.verifyPath + "licence-fee/" + id, { headers: this.headers })
      .pipe(catchError(this.handleErrorWith503));
  }

  verifyWorkerPaymentSubmission(id: string) {
    return this.http.get(`${this.apiPath}${this.verifyPath}worker/${id}`, { headers: this.headers })
      .pipe(catchError(this.handleErrorWith503));
  }

}
