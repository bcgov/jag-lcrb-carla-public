import { Injectable } from "@angular/core";
import { Account } from "@models/account.model";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { ProfileValidation } from "@models/profile-validation.model";
import { Observable, forkJoin } from "rxjs";
import { DataService } from "./data.service";
import { catchError, map } from "rxjs/operators";
import { TiedHouseConnection } from "@models/tied-house-connection.model";
import { Store } from "@ngrx/store";
import { AppState } from "@app/app-state/models/app-state";
import { SetCurrentAccountAction } from "@app/app-state/actions/current-account.action";
import { TiedHouseConnectionsDataService } from "@services/tied-house-connections-data.service";
import { LegalEntityDataService } from "@services/legal-entity-data.service";
import { FileSystemItem } from "@models/file-system-item.model";
import { Contact } from "../models/contact.model";

@Injectable()
export class AccountDataService extends DataService {

  apiPath = "api/accounts/";

  constructor(private http: HttpClient,
    private tiedHouseService: TiedHouseConnectionsDataService,
    private legalEntityDataService: LegalEntityDataService,
    private store: Store<AppState>) {
    super();
  }

  getAccount(accountId: string): Observable<Account> {
    return this.http.get<Account>(this.apiPath + accountId, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  getCurrentAccountContacts(): Observable<Contact[]> {
    return this.http.get<Contact[]>(this.apiPath + "current/contacts", { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  getAutocomplete(search: string): Observable<any[]> {
    return this.http.get<any[]>(this.apiPath + `autocomplete?name=${search}`, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  getCurrentAccount() {
    return this.http.get<Account>(this.apiPath + "current", { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  loadCurrentAccountToStore(id: string) {
    return forkJoin(this.getAccount(id),
        this.tiedHouseService.GetAllTiedHouseConnectionsForUser(id),
        this.legalEntityDataService.getBusinessProfileSummary())
      .pipe(map(data => {
        const account: Account = data[0];
        account.tiedHouse = data[1];
        account.legalEntity = data[2].length ? data[2][0] : null;
        this.store.dispatch(new SetCurrentAccountAction({ ...account } as Account));
        return account;
      }));
  }

  getBusinessProfile(accountId: string) {
    return this.http.get<ProfileValidation[]>(`${this.apiPath}business-profile/${accountId}`, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  getBCeID() {
    return this.http.get(this.apiPath + "bceid", { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  updateAccount(accountModel: Account) {
    return this.http.put(this.apiPath + accountModel.id, accountModel, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  createTiedHouseConnection(tiedHouse: TiedHouseConnection, accountId: string) {
    return this.http.post(this.apiPath + accountId + "/tiedhouseconnection", tiedHouse, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  deleteAccount(accountModel: Account) {
    return this.http.post(this.apiPath + accountModel.id + "/delete", accountModel, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  deleteCurrentAccount() {
    return this.http.post(this.apiPath + "delete/current", {}, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  /**
   * Get a file list of documents attached to the account by ID and document type
   * @param accountId The account ID to query for documents
   * @param documentType The document type (e.g. "Notice" for inspection notices under the account)
   */
  getFilesAttachedToAccount(accountId: string, documentType: string): Observable<FileSystemItem[]> {
    const headers = new HttpHeaders({});
    const url = `api/file/${accountId}/attachments/account/${documentType}`;
    return this.http.get<FileSystemItem[]>(url, { headers: headers })
      .pipe(map(files => this.processFiles(accountId, documentType, files)));
  }

  private processFiles(accountId: string, documentType: string, files: FileSystemItem[]): FileSystemItem[] {
    for (const file of files) {
      file.name = `${documentType}__${file.name}`;
      file.downloadUrl = `api/file/${accountId}/download-file/account/${file.name}`;
      file.downloadUrl += `?serverRelativeUrl=${encodeURIComponent(file.serverrelativeurl)}&documentType=${documentType
        }`;
    }
    return files;
  }
}
