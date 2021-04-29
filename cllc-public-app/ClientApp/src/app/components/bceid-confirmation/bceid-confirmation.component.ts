import { Component, Input, Output, EventEmitter } from "@angular/core";
import { DynamicsDataService } from "@services/dynamics-data.service";
import { Account } from "@models/account.model";
import { Contact } from "@models/contact.model";
import { User } from "@models/user.model";
import { UserDataService } from "@services/user-data.service";
import { AccountDataService } from "@services/account-data.service";
import { Store } from "@ngrx/store";
import { AppState } from "@app/app-state/models/app-state";
import { takeWhile } from "rxjs/operators";
import { FormBase } from "@shared/form-base";
import { Subscription } from "rxjs";
import { FeatureFlagService } from "@services/feature-flag.service";

@Component({
  selector: "app-bceid-confirmation",
  templateUrl: "./bceid-confirmation.component.html",
  styleUrls: ["./bceid-confirmation.component.scss"]
})
/** bceid-confirmation component*/
export class BceidConfirmationComponent extends FormBase {
  @Input()
  currentUser: User;
  @Output()
  reloadUser = new EventEmitter();
  bceidConfirmAccount = true;
  bceidConfirmBusinessType = false;
  bceidConfirmContact = false;
  showBceidCorrection: boolean;
  showBceidUserContinue = true;
  businessType = "";
  finalBusinessType = "";
  busy: Promise<any>;
  busySubscription: Subscription;
  termsAccepted = false;
  account: Account;
  lgApprovals: boolean;
  businessTypes = [
    { value: "Coop", name: "Co-Op" },
    { value: "IndigenousNation", name: "Indigenous nation " },
    { value: "MilitaryMess", name: "Military Mess" },
    { value: "Partnership", name: "Partnership" },
    { value: "PrivateCorporation", name: "Private Corporation" },
    { value: "PublicCorporation", name: "Public Corporation" },
    { value: "Society", name: "Society" },
    { value: "SoleProprietorship", name: "Sole Proprietorship" },
    { value: "University", name: "University" },
    // { value: "Church", name:"Church"},
    // that does not fill a role similar to a Local Government in the licensing process
  ];;

  constructor(private dynamicsDataService: DynamicsDataService,
    private userDataService: UserDataService,
    private store: Store<AppState>,
    public featureFlagService: FeatureFlagService,
    private accountDataService: AccountDataService) {
    super();
    // if this passes, this means the user's account exists but it's contact information has not been created.
    // user will skip the BCeid confirmation.
    this.store.select(state => state.currentAccountState.currentAccount)
      .pipe(takeWhile(() => this.componentActive))
      .subscribe((data) => {
        this.account = data;
        if (this.account) {
          this.termsAccepted = this.account.termsOfUseAccepted;
        }
      },
        error => { });

    featureFlagService.featureOn("LGApprovals")
      .subscribe(x => {
        this.lgApprovals = x;

        // add the Local Government option if the feature is enabled
        if (this.lgApprovals) {
          this.businessTypes.push({ value: "LocalGovernment", name: "Local Government" });
          this.businessTypes = this.businessTypes.sort((a, b) => a.name.localeCompare(b.name));
        }
      });

    featureFlagService.featureOn("Sep")
      .subscribe(isSepEnabled => {
        if (isSepEnabled) {
          this.businessTypes.push({ value: "Police", name: "Police" });
          this.businessTypes = this.businessTypes.sort((a, b) => a.name.localeCompare(b.name));
        }
      });
  }

  confirmBceidAccountYes() {
    this.bceidConfirmAccount = false;
    this.bceidConfirmBusinessType = true;
  }

  confirmBceidAccountNo() {
    // confirm BCeID
    this.showBceidCorrection = true;
  }

  confirmBceidUser() {
    // confirm BCeID
    this.bceidConfirmContact = true;
  }

  confirmCorpType() {
    this.bceidConfirmBusinessType = false;
    this.bceidConfirmContact = true;
  }

  confirmContactYes() {
    const account = {
      name: this.currentUser.businessname,
      id: this.currentUser.accountid,
      termsOfUseAccepted: true,
      termsOfUseAcceptedDate: new Date()
    } as Account;
    this.createContact(account);
  }

  createContact(account) {
    const contact = new Contact();
    contact.fullname = this.currentUser.name;
    contact.id = this.currentUser.contactid;
    account.primarycontact = contact;

    // Submit selected company type and sub-type to the account service
    account.businessType = this.businessType;

    const payload = JSON.stringify(account);
    this.busy = this.dynamicsDataService.createRecord("accounts", payload)
      .toPromise()
      .then((data) => {
        this.userDataService.loadUserToStore().then(res => { });
        this.reloadUser.emit();
      });
  }

  confirmContactNo() {
    // confirm Contact
    this.showBceidUserContinue = false;
  }

  onTermsAccepted(accepted: boolean) {
    this.termsAccepted = accepted;
    if (this.account) {
      const data = { ...this.account, termsOfUseAccepted: true, termsOfUseAcceptedDate: new Date() };
      this.dynamicsDataService.updateRecord("accounts", this.account.id, data)
        .subscribe(res => {
          this.accountDataService.loadCurrentAccountToStore(this.account.id)
            .subscribe(() => { });
        });
    }
  }

}
