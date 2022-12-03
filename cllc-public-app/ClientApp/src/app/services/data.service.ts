import { HttpHeaders, HttpErrorResponse } from "@angular/common/http";
import { throwError } from "rxjs";

export class DataService {

  apiPath = "api/";
  headers = new HttpHeaders({
    'Content-Type': "application/json"
  });

  handleErrorWith503(error: HttpErrorResponse) {
    if (error.error instanceof ErrorEvent) {
      // A client-side or network error occurred. Handle it accordingly.
      console.error("An error occurred:", error.error.message);
    } else {
      // The backend returned an unsuccessful response code.
      // The response body may contain clues as to what went wrong,
      console.error(
        `Backend returned code ${error.status}, ` +
        `body was: ${error.error}`);
    }
    if (error.status === 503) {
      return throwError(
        "503");
    }
    // return an observable with a user-facing error message
    if (error.error == "Payment already made") {
      return throwError(
        "Payment already made");
   }
    return throwError(
      "Something bad happened; please try again later.");
  }

  handleError(error: HttpErrorResponse) {
    if (error.error instanceof ErrorEvent) {
      // A client-side or network error occurred. Handle it accordingly.
      console.error("An error occurred:", error.error.message);
    } else {
      // The backend returned an unsuccessful response code.
      // The response body may contain clues as to what went wrong,
      console.error(
        `Backend returned code ${error.status}, ` +
        `body was: ${error.error}`);
    }
    if (error.error == "Payment already made") {
      return throwError(
        "Payment already made");
    }
    // return an observable with a user-facing error message
    return throwError(
      "Something bad happened; please try again later.");
  }
}
