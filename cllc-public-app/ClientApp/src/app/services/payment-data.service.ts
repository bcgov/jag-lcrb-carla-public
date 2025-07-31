import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { DataService } from "./data.service";
import { catchError } from "rxjs/operators";
import { Observable } from "rxjs";

type PaymentType =
'default' |
'worker' |
'licenceFee' |
'primaryInvoice' |
'secondaryInvoice' |
'specialEventInvoice'
;

type PaymentTypes = Record<PaymentType, {
  getPaymentURI: (id: string) => string;
  verifyPaymentURI: (id: string) => string;
}>;

/**
 * Service for handling payment-related data operations.
 *
 * @export
 * @class PaymentDataService
 * @extends {DataService}
 */
@Injectable()
export class PaymentDataService extends DataService {
  apiPath = "api/payment/";
  submitPath = "submit/";
  verifyPath = "verify/";

  readonly paymentTypes: PaymentTypes = {
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


  /**
   * Gets the payment URI for a permanent change application.
   *
   * @param {PaymentType} paymentType
   * @param {string} id
   * @return {*}  {Observable<Object>}
   */
  getPermanentChangePaymentURI(paymentType: PaymentType, id: string): Observable<Object> {
    return this.getPaymentURI(paymentType, id, {
      redirectContext: 'permanent-change'
    });
  }

  /**
   * Gets the payment URI for a legal entity application.
   *
   * @param {PaymentType} paymentType
   * @param {string} id
   * @return {*}  {Observable<Object>}
   */
  getLegalEntityPaymentURI(paymentType: PaymentType, id: string): Observable<Object> {
    return this.getPaymentURI(paymentType, id, {
      redirectContext: 'legal-entity'
    });
  }

  /**
   * Gets the payment URI for a specific payment type and ID.
   *
   * @param {PaymentType} paymentType
   * @param {string} id
   * @param {Record<string, string>} [queryParams] Optional query parameters to include in the request.
   * @return {*}  {Observable<Object>}
   */
  getPaymentURI(paymentType: PaymentType, id: string, queryParams?: Record<string, string>): Observable<Object> {
    const payType = this.paymentTypes[paymentType];

    return this.http.get(payType.getPaymentURI(id), { headers: this.headers, params: queryParams })
      .pipe(catchError(this.handleError));
  }

  /**
   * Verifies the payment for a specific payment type and ID.
   *
   * @param {PaymentType} paymentType
   * @param {string} id
   * @return {*}
   */
  verifyPaymentURI(paymentType: PaymentType, id: string) {
    const payType = this.paymentTypes[paymentType];

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
