import { Injectable } from "@angular/core";
import { ApplicationLicenseSummary } from "@models/application-license-summary.model";
import { Application } from "@models/application.model";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { catchError } from "rxjs/operators";
import { DataService } from "./data.service";
import { License } from "@models/license.model";
import { OutstandingPriorBalanceInvoice } from "../models/outstanding-prior-balance-invoce.model";
import { RelatedLicence } from "@models/related-licence";

@Injectable({
  providedIn: "root"
})
export class LicenseDataService extends DataService {

  apiPath = "api/licenses/";

  constructor(private http: HttpClient) {
    super();
  }

  /**
   * Get autocomplete list of licenses based on name and/or licence number.
   *
   * @param {{ name?: string; licenceNumber?: string }} queryParams
   * @return {*}  {Observable<RelatedLicence[]>}
   * @memberof LicenseDataService
   */
  getAutocomplete(queryParams: { name?: string; licenceNumber?: string }): Observable<RelatedLicence[]> {
    const params = new URLSearchParams();
    if (queryParams.name) {
      params.append('name', queryParams.name);
    }
    if (queryParams.licenceNumber) {
      params.append('licenceNumber', queryParams.licenceNumber);
    }
    const queryString = params.toString();
    return this.http
      .get<
        RelatedLicence[]
      >(this.apiPath + `autocomplete${queryString ? '?' + queryString : ''}`, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  getLicenceById(licenseId: string): Observable<License> {
    const url = `${this.apiPath}${licenseId}`;
    return this.http.get<License>(url, { headers: this.headers });
  }

  cancelTransfer(licenceId: string, accountId: string) {
    const url = `${this.apiPath}cancel-transfer`;
    return this.http.post<Application>(url, { licenceId, accountId }, { headers: this.headers });
  }

  initiateTransfer(licenceId: string, accountId: string) {
    const url = `${this.apiPath}initiate-transfer`;
    return this.http.post<Application>(url, { licenceId, accountId }, { headers: this.headers });
  }

  initiateTiedHouseExcemption(licenceId: string, relatedLicenceId: string, manufacturerProductionAmountforPrevYear: string, manufacturerProductionAmountUnit:string) {
    const url = `${this.apiPath}initiate-tied-house-excemption`;
    return this.http.post<Application>(url, { licenceId, relatedLicenceId, manufacturerProductionAmountforPrevYear, manufacturerProductionAmountUnit}, { headers: this.headers });
  }

  requestTermChange(licenceId: string, accountId: string, termId: string, reason: string) {
    const url = `${this.apiPath}request-term-change`;
    return this.http.post<Application>(url, { licenceId, accountId, termId, reason }, { headers: this.headers });
  }

  setThirdPartyOperator(licenceId: string, accountId: string) {
    const url = `${this.apiPath}set-third-party-operator`;
    return this.http.post<Application>(url, { licenceId, accountId }, { headers: this.headers });
  }

  // cancel Third Party Operator Application for given licence
  cancelThirdPartyOperator(licenceId: string, accountId: string) {
    const url = `${this.apiPath}cancel-operator-application`;
    return this.http.post<Application>(url, { licenceId, accountId }, { headers: this.headers });
  }

  // terminate Third Party Operator relation for given licence (after approval)
  terminateThirdPartyOperator(licenceId: string, accountId: string) {
    const url = `${this.apiPath}terminate-operator-relationship`;
    return this.http.post<Application>(url, { licenceId, accountId }, { headers: this.headers });
  }

  getAllCurrentLicenses(): Observable<ApplicationLicenseSummary[]> {
    return this.http.get<ApplicationLicenseSummary[]>(this.apiPath + "current",
      {
        headers: this.headers
      })
      .pipe(catchError(this.handleError));
  }

  getOutstandingBalancePriorInvoices(): Observable<OutstandingPriorBalanceInvoice[]> {
    return this.http.get<OutstandingPriorBalanceInvoice[]>(this.apiPath + "outstanding-prior-balance-invoice",
      {
        headers: this.headers
      })
      .pipe(catchError(this.handleError));
  }

  getAllOperatedLicenses(): Observable<ApplicationLicenseSummary[]> {
    return this.http.get<ApplicationLicenseSummary[]>(this.apiPath + "third-party-operator",
      {
        headers: this.headers
      })
      .pipe(catchError(this.handleError));
  }

  getAllProposedLicenses(): Observable<ApplicationLicenseSummary[]> {
    return this.http.get<ApplicationLicenseSummary[]>(this.apiPath + "proposed-owner",
      {
        headers: this.headers
      })
      .pipe(catchError(this.handleError));
  }

  createApplicationForActionType(licenseId: string, applicationType: string): Observable<Application> {
    const url = `${this.apiPath}${licenseId}/create-action-application?applicationType=${encodeURIComponent(applicationType)}`;
    return this.http.post<Application>(url, null, { headers: this.headers });
  }

  createApplicationForActionTypeTerm(licenseId: string, applicationType: string, termId: string): Observable<Application> {
    const url = `${this.apiPath}${licenseId}/create-action-application-term/${termId}?applicationType=${encodeURIComponent(applicationType)}`;
    return this.http.post<Application>(url, null, { headers: this.headers });
  }

  updateLicenceEstablishment(licenceId: string, licence: ApplicationLicenseSummary):
    Observable<ApplicationLicenseSummary> {
    return this.http.put<ApplicationLicenseSummary>(this.apiPath + licenceId + "/establishment",
      licence,
      { headers: this.headers });
  }

  updateLicenceLDBOrders(licenceId: string, total: number) {
    return this.http.put<License>(this.apiPath + licenceId + "/ldbordertotals", total, { headers: this.headers });
  }

  updateLicenseeRepresentative(licenceId: string, licence: ApplicationLicenseSummary):
    Observable<ApplicationLicenseSummary> {
    return this.http.put<ApplicationLicenseSummary>(this.apiPath + licenceId + "/representative",
      licence,
      { headers: this.headers });
  }

  updateLicenceOffsiteStorage(licenceId: string, licence: ApplicationLicenseSummary):
    Observable<ApplicationLicenseSummary> {
    return this.http.put<ApplicationLicenseSummary>(`${this.apiPath}${licenceId}/offsite-storage`,
      licence,
      { headers: this.headers });
  }
}
