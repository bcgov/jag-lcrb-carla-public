import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

/**
 * Interface representing a SEP payment record.
 */
export interface SepPayment {
  permitNumber: string;
  contactName: string;
  contactEmail: string;
  contactPhone: string;
  eventName: string;
  drinkServings: string;
  drinkPrice: string;
  drinkRevenue: string;
  drinkCost: string;
  drinkCorrectedCost: string;
  drinkCostDiff: string;
  drinkCostPSTDiff: string;
  sepId: string;
}

@Injectable({
  providedIn: 'root'
})
export class SepPaymentService {
  private readonly apiUrl = 'api/SepPayment';

  constructor(private http: HttpClient) { }

  /**
   * Fetches a SEP payment by its SEPId.
   * @param sepId The unique identifier of the SEP payment.
   * @returns Observable of SepPayment
   */
  getPaymentById(sepId: string, txnId: string): Observable<SepPayment> {
    const url = `${this.apiUrl}?sepId=${encodeURIComponent(sepId)}&txnId=${encodeURIComponent(txnId)}`;
    return this.http.get<SepPayment>(url).pipe(
      catchError(this.handleError)
    );
  }

  /**
   * Fetches all SEP payments.
   * @returns Observable of an array of SepPayment
   */
  getAllPayments(): Observable<SepPayment[]> {
    return this.http.get<SepPayment[]>(this.apiUrl).pipe(
      catchError(this.handleError)
    );
  }

  /**
   * Generic error handling for HTTP operations.
   */
  private handleError(error: HttpErrorResponse) {
    let message = 'An unknown error occurred.';
    if (error.error instanceof ErrorEvent) {
      // Client-side or network error
      message = `Network error: ${error.error.message}`;
    } else {
      // Backend returned an unsuccessful response code
      message = `Server error (status ${error.status}): ${error.message}`;
    }
    return throwError(() => new Error(message));
  }
}
