import { filter, map, catchError, takeWhile } from "rxjs/operators";
import { Component, OnInit, ViewChild, Input, TemplateRef, EventEmitter, Output } from "@angular/core";
import { User } from "@models/user.model";
import { ContactDataService } from "@services/contact-data.service";
import { Contact } from "@models/contact.model";
import { Store } from "@ngrx/store";
import { Subscription, Observable, forkJoin, of } from "rxjs";
import { FormBuilder, FormGroup, Validators, FormArray } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { COUNTRIES } from "./country-list";


// tslint:disable-next-line:no-duplicate-imports
import { AccountDataService } from "@services/account-data.service";
import { Account, BUSINESS_TYPE_LIST } from "@models/account.model";
import { FormBase } from "@shared/form-base";
import { ConnectionToProducersComponent } from "./tabs/connection-to-producers/connection-to-producers.component";
import { TiedHouseConnection } from "@models/tied-house-connection.model";
import { TiedHouseConnectionsDataService } from "@services/tied-house-connections-data.service";
import { AppState } from "@app/app-state/models/app-state";
import { faAddressCard, faChevronRight, faEnvelope, faExclamationTriangle, faPhone, faTrash, faPlus } from
  "@fortawesome/free-solid-svg-icons";
import { UserDataService } from "@services/user-data.service";
import { endOfToday } from "date-fns";
import { ApplicationDataService } from "@services/application-data.service";
import { ApplicationTypeNames } from "../../models/application-type.model";
import { MatDialog } from "@angular/material/dialog";
import { Clipboard } from '@angular/cdk/clipboard';
import { MatSnackBar } from '@angular/material/snack-bar';
import { environment } from "environments/environment";

// See the Moment.js docs for the meaning of these formats:
// https://momentjs.com/docs/#/displaying/format/
export const MY_FORMATS = {
  parse: {
    dateInput: "LL",
  },
  display: {
    dateInput: "YYYY-MM-DD",
    monthYearLabel: "MMM YYYY",
    dateA11yLabel: "YYYY-MM-DD",
    monthYearA11yLabel: "MMMM YYYY",
  },
};

const ValidationFieldNameMap = {
  'businessProfile.id': "Account ID",
  'businessProfile._mailingSameAsPhysicalAddress': "Mailing Address Same as Physical Address",
  'businessProfile.bcIncorporationNumber': "B.C. Incorporation Number",
  'businessProfile.dateOfIncorporationInBC': "Date of Incorporation In B.C.",
  'businessProfile.businessNumber': "Business Number",
  'businessProfile.businessType': "Business Type",
  'businessProfile.contactPhone': "Corporation Address Business Phone",
  'businessProfile.accountUrls': "Account URL(s)",
  // 'businessProfile.contactEmail': 'Corporation Address Business Email',

  'businessProfile.physicalAddressStreet': "Physical Address Street",
  'businessProfile.physicalAddressStreet2': "Physical Address Street2",
  'businessProfile.physicalAddressCity': "Physical Address City",
  'businessProfile.physicalAddressPostalCode': "Physical Address Postal Code",
  'businessProfile.physicalAddressProvince': "Physical Address Province",
  'businessProfile.physicalAddressCountry': "Physical Address Country",
  'businessProfile.mailingAddressStreet': "Mailing Address Street",
  'businessProfile.mailingAddressStreet2': "Mailing Address Street2",
  'businessProfile.mailingAddressCity': "Mailing Address City",
  'businessProfile.mailingAddressPostalCode': "Mailing Address Postal Code",
  'businessProfile.mailingAddressProvince': "Mailing Address Province",
  'businessProfile.mailingAddressCountry': "Mailing Address Country",

  'contact.id': "Corporation Contact ID",
  'contact.firstname': "Corporation Contact First Name",
  'contact.lastname': "Corporation Contact LastName",
  'contact.jobTitle': "Corporation Contact Job Title",
  'contact.telephone1': "Corporation Contact Telephone",
  'contact.emailaddress1': "Corporation Contact Email",
};

@Component({
  selector: "app-account-profile",
  templateUrl: "./account-profile.component.html",
  styleUrls: ["./account-profile.component.scss"]
})
export class AccountProfileComponent extends FormBase implements OnInit {
  faExclamationTriangle = faExclamationTriangle;
  faTrash = faTrash;
  faChevronRight = faChevronRight;
  faAddressCard = faAddressCard;
  faEnvelope = faEnvelope;
  faPhone = faPhone;
  faPlus = faPlus;
  @Input()
  useInStepperMode = false;
  @Output()
  saveComplete = new EventEmitter<boolean>();
  currentUser: User;
  dataLoaded = false;
  busy: Subscription;
  busy2: Promise<any>;
  busy3: Promise<any>;
  form: FormGroup;
  countryList = COUNTRIES;
  maxDate = endOfToday();

  accountId: string;
  saveFormData: any;
  _showAdditionalAddress: boolean;
  _showAdditionalContact: boolean;
  legalEntityId: string;
  @ViewChild(ConnectionToProducersComponent)
  connectionsToProducers: ConnectionToProducersComponent;
  applicationId: string;
  applicationMode: string;
  account: Account;
  tiedHouseFormData: Observable<TiedHouseConnection>;
  validationMessages: string[];
  renewalType: string;

  @ViewChild('badgeTemplateDialog') badgeTemplateDialog: TemplateRef<any>;
  // 2024-09-13: Temporary disabling this until further development work can be completed to support this feature.
  // generatedOrvCode: string = `<a href="#" onclick="window.open('https://orgbook-app-b7aa30-dev.apps.silver.devops.gov.bc.ca/verify/BC123456', '_blank', 'width=800,height=600'); return false;">Verify Retailer</a>`

  get contacts(): FormArray {
    return this.form.get("otherContacts") as FormArray;
  }

  get accountUrls(): FormArray {
    return this.form.get("businessProfile.accountUrls") as FormArray;
  }

  /**
   * Regex validator that asserts a valid url string.
   *
   * @example
   * 'www.gov.bc.ca' // valid
   * 'bad string' // invalid
   */
  urlValidator = Validators.pattern(/^(https?:\/\/)?([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,}(\/[^\s]*)?$/);

  /**
   * Removes an account URL field from the form.
   */
  removeAccountUrl(index: number): void {
    const accountUrls = this.form.get("businessProfile.accountUrls") as FormArray;
    accountUrls.removeAt(index);
  }

  /**
   * Adds a new account URL field to the form.
   */
  addAccountUrl(): void {
    const accountUrls = this.form.get("businessProfile.accountUrls") as FormArray;
    accountUrls.push(this.fb.control("", [this.urlValidator]));
  }

  /**
   * Splits a comma separated string into an array of strings.
   * Trims each string, and removes empty strings.
   *
   * @param {(string | null)} csvString
   * @return {*}  {string[]} An array of strings. If the input string is null or empty, returns an array with an empty
   * string.
   * @memberof AccountProfileComponent
   */
  splitAccountURLString(csvString: string | null): string[] {
    const accountUrls = csvString?.split(",").map(item => item.trim()).filter(Boolean) ?? []

    if(accountUrls.length === 0) {
      // No initial account URLs, return an array with an empty string
      // to ensure the form control is initialized with at least one empty field
      return [""];
    }

    return accountUrls;
  }

  /**
   * Combines an array of strings into a comma separated string.
   * Trims each string, and removes empty strings.
   *
   * @param {(string[] | null)} strings
   * @return {*}  {string} A comma separated string. If the input array is null or empty, returns an empty string.
   * @memberof AccountProfileComponent
   */
  combineAccountURLStrings(strings: string[] | null): string {
    if (!strings?.length) {
      return "";
    }

    return strings.map(item => item.trim()).filter(Boolean).join(",");
  }

  businessTypes = BUSINESS_TYPE_LIST;

  constructor(private store: Store<AppState>,
    private accountDataService: AccountDataService,
    private contactDataService: ContactDataService,
    private userDataService: UserDataService,
    private applicationDataService: ApplicationDataService,
    private fb: FormBuilder,
    private router: Router,
    private route: ActivatedRoute,
    private tiedHouseService: TiedHouseConnectionsDataService,
    private dialog: MatDialog,
    private clipboard: Clipboard,
    private snackBar: MatSnackBar
  ) {
    super();
    this.route.paramMap.subscribe(params => {
      this.applicationId = params.get("applicationId");
      this.applicationDataService.getApplicationById(this.applicationId)
      .subscribe(res => {
        this.application = res;
      });
    });
    this.route.paramMap.subscribe(params => this.renewalType = params.get("renewalType"));
    this.route.paramMap.subscribe(params => this.applicationMode = params.get("mode"));
  }

  ngOnInit() {
    this.form = this.fb.group({
      businessProfile: this.fb.group({
        id: [""],
        _mailingSameAsPhysicalAddress: [],
        // name: [''],
        // businessDBAName: [''],
        bcIncorporationNumber: [""],
        dateOfIncorporationInBC: [""],
        // CRA business numbers are 9 digit long and start with a non-zero digit
        businessNumber: ["", [Validators.required, Validators.pattern("^[1-9][0-9]{8}$")]],
        businessType: ["", Validators.required],
        contactPhone: ["", [Validators.required, /*Validators.minLength(10), Validators.maxLength(10)*/]],
        contactEmail: ["", [Validators.required, Validators.email]],

        physicalAddressStreet: ["", Validators.required],
        physicalAddressStreet2: [""],
        physicalAddressCity: ["", Validators.required],
        physicalAddressPostalCode: ["", [Validators.required, this.customZipCodeValidator("physicalAddressCountry")]],
        physicalAddressProvince: ["British Columbia", Validators.required],
        physicalAddressCountry: ["Canada", Validators.required],
        mailingAddressStreet: ["", Validators.required],
        mailingAddressStreet2: [""],
        mailingAddressCity: ["", Validators.required],
        mailingAddressPostalCode: ["", [Validators.required, this.customZipCodeValidator("mailingAddressCountry")]],
        mailingAddressProvince: ["British Columbia", Validators.required],
        mailingAddressCountry: ["Canada", Validators.required],
        websiteUrl: [""],
        accountUrls: this.fb.array([]),
      }),
      contact: this.fb.group({
        id: [],
        firstname: [{ value: "", disabled: true }, Validators.required],
        lastname: [{ value: "", disabled: true }, Validators.required],
        jobTitle: [""],
        telephone1: ["", [Validators.required]],
        emailaddress1: ["", [Validators.required, Validators.email]],
      }),
    });
    // Watch for changes to the current user and account
    this.subscribeForData();

    this.form.get("businessProfile._mailingSameAsPhysicalAddress").valueChanges.pipe(
      filter(value => value === true))
      .subscribe(() => {
        this.copyPhysicalToMailingAddress();
      });

    this.form.get("businessProfile.physicalAddressStreet").valueChanges.pipe(
      filter(() => this.form.get("businessProfile._mailingSameAsPhysicalAddress").value))
      .subscribe(() => {
        this.copyPhysicalToMailingAddress();
      });
    this.form.get("businessProfile.physicalAddressStreet2").valueChanges.pipe(
      filter(() => this.form.get("businessProfile._mailingSameAsPhysicalAddress").value))
      .subscribe(() => {
        this.copyPhysicalToMailingAddress();
      });
    this.form.get("businessProfile.physicalAddressCity").valueChanges.pipe(
      filter(() => this.form.get("businessProfile._mailingSameAsPhysicalAddress").value))
      .subscribe(() => {
        this.copyPhysicalToMailingAddress();
      });
    this.form.get("businessProfile.physicalAddressPostalCode").valueChanges.pipe(
      filter(() => this.form.get("businessProfile._mailingSameAsPhysicalAddress").value))
      .subscribe(() => {
        this.copyPhysicalToMailingAddress();
      });

    this.form.get("businessProfile.physicalAddressProvince").valueChanges.pipe(
      filter(() => this.form.get("businessProfile._mailingSameAsPhysicalAddress").value))
      .subscribe(() => {
        this.copyPhysicalToMailingAddress();
      });

    this.form.get("businessProfile.physicalAddressCountry").valueChanges.pipe(
      filter(() => this.form.get("businessProfile._mailingSameAsPhysicalAddress").value))
      .subscribe(() => {
        this.copyPhysicalToMailingAddress();
      });

  }

  copyPhysicalToMailingAddress() {
    this.form.get("businessProfile.mailingAddressStreet")
      .patchValue(this.form.get("businessProfile.physicalAddressStreet").value);
    this.form.get("businessProfile.mailingAddressStreet2")
      .patchValue(this.form.get("businessProfile.physicalAddressStreet2").value);
    this.form.get("businessProfile.mailingAddressCity")
      .patchValue(this.form.get("businessProfile.physicalAddressCity").value);
    this.form.get("businessProfile.mailingAddressPostalCode")
      .patchValue(this.form.get("businessProfile.physicalAddressPostalCode").value);
    this.form.get("businessProfile.mailingAddressProvince")
      .patchValue(this.form.get("businessProfile.physicalAddressProvince").value);
    this.form.get("businessProfile.mailingAddressCountry")
      .patchValue(this.form.get("businessProfile.physicalAddressCountry").value);
  }

  getBusinessTypeName() {
    if (!(this.saveFormData && this.saveFormData.businessProfile)
      || this.account?.isOtherBusinessType()) {
      return "";
    }
    let name = "";
    switch (this.saveFormData.businessProfile.businessType) {
      case "GeneralPartnership":
      case 'LimitedPartnership"':
      case "LimitedLiabilityPartnership":
        name = "Partnership";
        break;
      case "SoleProprietorship":
        name = "Sole Proprietorship";
        break;
      case "IndigenousNation":
        name = "Indigenous Nation";
        break;
      case "LocalGovernment":
        name = "Local Government";
        break;
      case "Police":
        name = "Police";
        break;
      case "PublicCorporation":
      case "PrivateCorporation":
      case "UnlimitedLiabilityCorporation":
      case "LimitedLiabilityCorporation":
        name = "Corporation";
        break;
      default:
        name = this.saveFormData.businessProfile.businessType;
        break;
    }
    return name;
  }

  legalNameLabel() {
    const businessType = this.getBusinessTypeName();
    let label = `${businessType} ${this.account?.isOtherBusinessType() ? '' : '-'} Legal Name`;
    if (businessType === "IndigenousNation") {
      label = "Full name of Indigenous Nation";
    }
    return label;
  }

  subscribeForData() {
    this.store.select(state => state.currentAccountState.currentAccount)
      .pipe(takeWhile(() => this.componentActive))
      .pipe(filter(s => !!s))
      .subscribe(account => this.loadAccount(account));

    this.store.select(state => state.currentUserState.currentUser)
      .pipe(takeWhile(() => this.componentActive))
      .pipe(filter(user => !!user))
      .subscribe(user => this.loadUser(user));
  }

  private loadAccount(account: Account) {
    this.account = account;

    // Make a copy of the account object stored in Ngrx (which is read-only)
    // Remove `accountUrls` as the API returns a string, but the form is expecting an array.
    // See https://stackoverflow.com/questions/57591012/ngrx-cannot-assign-to-read-only-property-property-of-object-object
    const { accountUrls, ...businessProfile }: Partial<Account> = { ...account };

    businessProfile.physicalAddressProvince = businessProfile.physicalAddressProvince || "British Columbia";
    businessProfile.physicalAddressCountry = "Canada";
    businessProfile.mailingAddressProvince = businessProfile.mailingAddressProvince || "British Columbia";
    businessProfile.mailingAddressCountry = "Canada";

    this.form.patchValue({ businessProfile: businessProfile });

    this.saveFormData = this.form.value;

    // normalize postal codes
    this.form.get("businessProfile.mailingAddressPostalCode").setValue(
      (this.form.get("businessProfile.mailingAddressPostalCode").value || "").replace(/\s+/g, "")
    );
    this.form.get("businessProfile.physicalAddressPostalCode").setValue(
      (this.form.get("businessProfile.physicalAddressPostalCode").value || "").replace(/\s+/g, "")
    );

    //LCSD-7412
    //Simplified vaidators for bcIncorporationNumber
   // Incorporation Number	|incorporationNumber:	Registry number for Corporations |	Up to 10 alphanumeric characters.
   
    if (this.account.isPrivateCorporation() || this.account.businessType === "Society") {
     this.form.get("businessProfile.bcIncorporationNumber")
     .setValidators([Validators.pattern("^[A-Za-z0-9]{1,15}$")]);
  } else {
    this.form.get("businessProfile.bcIncorporationNumber").clearValidators();

    // Transform the accountUrls comma-separated string into an array
    const accountUrlsArray = this.splitAccountURLString(accountUrls);
    const accountUrlsArrayControl = this.form.get("businessProfile.accountUrls") as FormArray;
    // Clear the existing account form controls, if any, so duplicate controls are not created if this function is
    // called multiple times
    accountUrlsArrayControl.clear();
    for (const accountUrl of accountUrlsArray) {
        // Add a form control for each account URL
        accountUrlsArrayControl.push(this.fb.control(accountUrl, [this.urlValidator]));
    }
  }
  }

  private loadUser(user: User) {
    this.currentUser = user;
    if (this.currentUser && this.currentUser.contactid) {
      this.contactDataService.getContact(this.currentUser.contactid)
        .subscribe(contact => {
          this.form.patchValue({ contact: contact });
          this.saveFormData = this.form.value;
        });
    }
  }

  canDeactivate(): Observable<boolean> {
    if (!this.connectionsToProducers.formHasChanged() &&
      JSON.stringify(this.saveFormData) === JSON.stringify(this.form.value)) {
      return of(true);
    } else {
      return this.save();
    }
  }

  openBadgeTemplateDialog() {
    this.dialog.open(this.badgeTemplateDialog, {
      disableClose: true,
      autoFocus: true,
      width: "auto",
      height: "auto",
      maxWidth: "500px",
      maxHeight: "80vh",
      panelClass: 'custom-dialog-container'
    });
  }

  // 2024-09-13: Temporary disabling this until further development work can be completed to support this feature.
  // onCopy(): void {
  //   this.clipboard.copy(this.generatedOrvCode);
  //   this.snackBar.open('HTML copied to clipboard', null, {
  //     duration: 2000,
  //   });
  //   this.dialog.closeAll();
  // }

  save(): Observable<boolean> {
    const _tiedHouse = this.tiedHouseFormData || {};
    this.form.get("businessProfile").patchValue({ physicalAddressCountry: "Canada" });
    const value = {
      ...this.form.get("businessProfile").value,
      // Transform the accountUrls array into a comma-separated string as expected by the API
      accountUrls: this.combineAccountURLStrings(this.form.get("businessProfile.accountUrls").value),
    } as Account;
    const saves = [
      this.accountDataService.updateAccount(value),
      this.contactDataService.updateContact(this.form.get("contact").value)
    ];

    if (this.connectionsToProducers) {
      saves.push(
        this.prepareTiedHouseSaveRequest({ ...this.account.tiedHouse, ..._tiedHouse })
      );
    }

    return forkJoin(saves)
      .pipe(catchError(() => of(false)),
        map(() => {
          this.accountDataService.loadCurrentAccountToStore(this.account.id).subscribe(() => { });
          // reload the user to fetch updated contact information
          this.userDataService.loadUserToStore().then(() => { });
          return true;
        }));
  }

  gotoReview() {
    this.validationMessages = [];

    var route = "/dashboard";

    if (this.getBusinessTypeName() == 'Police') {
      this.form.get('businessProfile.businessNumber').setValidators([]);
      this.form.get('businessProfile.businessNumber').updateValueAndValidity();
      route = "/sep/dashboard";
    }

    if (this.form.valid && (!this.connectionsToProducers || this.connectionsToProducers.form.valid)) {
      this.busy = this.save().subscribe(() => {
        if (this.useInStepperMode) {
          this.saveComplete.emit(true);
        } else if (this.applicationId) {
          if (this.application?.applicationType?.name === ApplicationTypeNames.TiedHouseExemptionApplication) {
            const route: any[] = [`/tied-house-exemption/${this.applicationId}`];
            this.router.navigate(route);
          }

          if (this.renewalType) {
            const route: any[] = [`/renew-licence/${this.renewalType}/${this.applicationId}`];
            if (this.applicationMode) {
              route.push({ mode: this.applicationMode });
            }
            this.router.navigate(route);
          } else if (this.applicationMode === "catering") {// divert catering
            const route: any[] = [`/application/catering/${this.applicationId}`];
            if (this.applicationMode) {
              route.push({ mode: this.applicationMode });
            }
            this.router.navigate(route);
          } else {
            const route: any[] = [`/application/${this.applicationId}`];
            if (this.applicationMode) {
              route.push({ mode: this.applicationMode });
            }
            this.router.navigate(route);
          }
        } else {

          this.router.navigate([route]);
        }
      });
    } else {
      this.markAsTouched();
      this.listControlsWithErrors(this.form, ValidationFieldNameMap).forEach(m => this.validationMessages.push(m));
    }
  }

  prepareTiedHouseSaveRequest(_tiedHouseData) {
    const data = { ...this.account.tiedHouse, ..._tiedHouseData };

    if (data.id) {
      return this.tiedHouseService.updateTiedHouse(data, data.id);
    } else {
      return this.accountDataService.createTiedHouseConnection(data, this.accountId);
    }
  }

  // marking the form as touched makes the validation messages show
  markAsTouched() {
    this.form.markAsTouched();

    const businessProfileControls = ((this.form.get("businessProfile")) as FormGroup).controls;
    for (const c in businessProfileControls) {
      if (typeof (businessProfileControls[c].markAsTouched) === "function") {
        businessProfileControls[c].markAsTouched();
      }
    }

    const contactControls = ((this.form.get("contact")) as FormGroup).controls;
    for (const c in contactControls) {
      if (typeof (contactControls[c].markAsTouched) === "function") {
        contactControls[c].markAsTouched();
      }
    }
  }
}
