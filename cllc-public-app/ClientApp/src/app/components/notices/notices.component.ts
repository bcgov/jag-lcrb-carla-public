import { Component, OnInit } from "@angular/core";
import { Subscription } from "rxjs";
import { filter, takeWhile, map } from "rxjs/operators";
import { Store } from "@ngrx/store";
import { FormBase } from "@shared/form-base";
import { AccountDataService } from "@services/account-data.service";
import { Account } from "@models/account.model";
import { AppState } from "@app/app-state/models/app-state";
import { FileSystemItem } from "@models/file-system-item.model";

@Component({
  selector: "app-notices",
  templateUrl: "./notices.component.html",
  styleUrls: ["./notices.component.scss"],
})
export class NoticesComponent extends FormBase implements OnInit {
  isEditMode = true;
  isReadOnly = false;
  showValidationMessages = false;

  account: Account;
  notices: FileSystemItem[]; // TODO: improve typing here

  busy: Subscription;
  dataLoaded = false; // this is set to true when all page data is loaded


  constructor(
    private store: Store<AppState>,
    private accountDataService: AccountDataService,
  ) {
    super();
  }

  ngOnInit() {
    this.retrieveAccount();
  }

  retrieveAccount() {
    this.store
      .select(state => state.currentAccountState.currentAccount)
      .pipe(takeWhile(() => this.componentActive))
      .pipe(filter(s => !!s))
      .subscribe(account => this.fetchData(account));
  }

  fetchData(account: Account) {
    this.account = account;
    this.busy = this.retrieveNotices(account)
      .subscribe(notices => {
        this.notices = notices;
        this.dataLoaded = true;
      });

  }

  retrieveNotices(account: Account) {
    return this.accountDataService.getFilesAttachedToAccount(account.id, "Notice")
      .pipe(map(files => this.orderByDate(files)));
  }

  orderByDate(files: FileSystemItem[]): FileSystemItem[] {
    return files.sort((a, b) => {
      const dateA = new Date(a.timelastmodified);
      const dateB = new Date(b.timelastmodified);
      if (dateA > dateB) return -1;
      if (dateA < dateB) return 1;
      return 0;
    });
  }
}
