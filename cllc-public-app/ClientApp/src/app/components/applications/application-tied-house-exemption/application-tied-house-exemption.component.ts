import { Component, OnInit, ViewChild, AfterViewInit } from "@angular/core";
import { FormBase, ApplicationHTMLContent } from "@shared/form-base";
import { FormGroup, FormBuilder, Validators } from "@angular/forms";
import { Subscription, Subject, Observable, forkJoin, of, pipe } from 'rxjs';
import { ApplicationType, ApplicationTypeNames, FormControlState } from "@models/application-type.model";
import { Store } from "@ngrx/store";
import { AppState } from "@app/app-state/models/app-state";
import { MatDialog } from "@angular/material/dialog";
import { MatSnackBar } from "@angular/material/snack-bar";
import { Router, ActivatedRoute } from "@angular/router";
import { FeatureFlagService } from "@services/feature-flag.service";
import { EstablishmentWatchWordsService } from "@services/establishment-watch-words.service";
import { takeWhile, filter, catchError, mergeMap, first } from "rxjs/operators";
import { Account, TransferAccount } from "@models/account.model";
import { LicenseDataService } from "@services/license-data.service";
import { License } from "@models/license.model";
import { faSave } from "@fortawesome/free-regular-svg-icons";
import { faTrashAlt } from "@fortawesome/free-solid-svg-icons";
import { RelatedLicence } from "@models/related-licence";
import { ApplicationDataService } from "../../../services/application-data.service";
import { Application } from "../../../models/application.model";
import { ApplicationTypeDataService } from "../../../services/application-type-data.service";
import { RelatedLicencePickerComponent } from "@shared/components/related-licence-picker/related-licence-picker.component";
import { RelatedJobnumberPickerComponent } from "@shared/components/related-jobnumber-picker/related-jobnumber-picker.component";

const ValidationErrorMap = {
  "proposedOwner.accountId": "Please select the proposed transferee",
  consent: "Please consent to the invitation, if you wish to proceed.",
  authorizedToSubmit: "Please affirm that you are authorized to submit the application.",
  signatureAgreement: "Please affirm that all of the information provided for this application is true and complete.",
};

@Component({
  selector: 'app-application-tied-house-exemption',
  templateUrl: './application-tied-house-exemption.component.html',
  styleUrls: ['./application-tied-house-exemption.component.scss']
})
export class ApplicationTiedHouseExemptionComponent extends FormBase implements OnInit {

  faSave = faSave;
  faTrashAlt = faTrashAlt;
  licence: License;
  form: FormGroup;
  licenceId: string;
  busy: Subscription;
  busyPromise: any;
  validationMessages: any[];
  showValidationMessages: boolean;
  htmlContent = {} as ApplicationHTMLContent;
  ApplicationTypeNames = ApplicationTypeNames;
  FormControlState = FormControlState;
  account: Account;
  minDate = new Date();
  dataLoaded: boolean;
  applicationType: ApplicationType;
  applicationId: string;
  isAppId: string;

  @ViewChild(RelatedLicencePickerComponent) autocompletelicencecomponent;
  @ViewChild(RelatedJobnumberPickerComponent) autocompletejobcomponent;


  constructor(private store: Store<AppState>,
    public snackBar: MatSnackBar,
    public router: Router,
    private licenseDataService: LicenseDataService,
    public featureFlagService: FeatureFlagService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    public dialog: MatDialog,
    public applicationDataService: ApplicationDataService,
    public establishmentWatchWordsService: EstablishmentWatchWordsService) {
    super();
    this.route.paramMap.subscribe(pmap => this.licenceId = pmap.get("licenceId"));
    this.route.paramMap.subscribe(pmap => this.applicationId = pmap.get("applicationId"));
    this.route.paramMap.subscribe(pmap => this.isAppId = pmap.get("isAppId"));


  }


  ngOnInit() {
    this.form = this.fb.group({
      licenseNumber: [""],
      establishmentName: [""],
      establishmentAddressStreet: [""],
      establishmentAddressCity: [""],
      establishmentAddressPostalCode: [""],
      establishmentParcelId: [""],
      assignedLicence: this.fb.group({
        id: ["", [Validators.required]],
        establishmentName: [{ value: "", disabled: true }],
        name: [{ value: "", disabled: true }],
        city: [{ value: "", disabled: true }],
        country: [{ value: "", disabled: true }],
        streetaddress: [{ value: "", disabled: true }],
        postalCode: [{ value: "", disabled: true }],
        licensee: [{ value: "", disabled: true }],
        // 2024-04-19 LCSD-6368 waynezen
        jobNumber: [{ value: "", disabled: true }],
        licenceNumber: [{ value: "", disabled: true }],
      }),
      licenseeContact: this.fb.group({
        name: [{ value: "", disabled: true }],
        email: [{ value: "", disabled: true }],
        phone: [{ value: "", disabled: true }]
      }),
      consent: ["", [this.customRequiredCheckboxValidator()]],
      authorizedToSubmit: ["", [this.customRequiredCheckboxValidator()]],
      signatureAgreement: ["", [this.customRequiredCheckboxValidator()]],
      manufacturerProductionAmountforPrevYear: [''],
      manufacturerProductionAmountUnit: ['']
    });

    if (!this.licenceId) {
      this.applicationDataService.getApplicationTypeByName('Tied House Exemption Application')
        .pipe(takeWhile(() => this.componentActive))
        .subscribe((appType: ApplicationType) => {
          this.applicationType = appType;
        });

      if (this.isAppId == 'true') {
        this.store.select(state => state.currentAccountState.currentAccount)
          .pipe(takeWhile(() => this.componentActive))
          .subscribe((account) => {
            this.account = account;
            this.form.patchValue(account);
            this.busy = this.applicationDataService.getApplicationById(this.applicationId)
              .pipe(takeWhile(() => this.componentActive))
              .subscribe((data: Application) => {
                this.form.patchValue(data);
                this.form.get("licenseeContact.name").setValue(data.applicant.name);
                this.form.get("licenseeContact.phone").setValue(data.applicant.contactPhone);
                this.form.get("licenseeContact.email").setValue(data.applicant.contactEmail);
                this.form.get("authorizedToSubmit").setValue(false);
                this.form.get("signatureAgreement").setValue(false);
              });
          });
      }
    }
    // Get licence data
    this.busy = this.licenseDataService.getLicenceById(this.licenceId)
      .pipe(takeWhile(() => this.componentActive))
      .subscribe((licence: License) => {
        this.licence = licence;
        this.form.patchValue(this.licence);

        if (this.licenceHasRepresentativeContact()
        ) { //If the licence has a representative, set it to be the licensee contact
          const contact = {
            name: this.licence.representativeFullName,
            email: this.licence.representativeEmail,
            phone: this.licence.representativePhoneNumber
          };
          this.form.get("licenseeContact").patchValue(contact);
          this.dataLoaded = true;
        } else if (this.account) { // If the account is loaded, use it for the licensee contact
          const contact = {
            name: (this?.account?.primarycontact?.firstname || "") + " " + (this?.account?.primarycontact?.lastname || ""),
            email: this.account.contactEmail,
            phone: this.account.contactPhone
          };
          this.form.get("licenseeContact").patchValue(contact);
          this.dataLoaded = true;
        } else { // Otherwise load the account and use it for the licensee representative
          this.store.select(state => state.currentAccountState.currentAccount)
            .pipe(filter(account => !!account))
            .pipe(first())
            .subscribe((account) => {
              this.account = account;
              const contact = {
                name: (this.account?.primarycontact?.firstname || "") + " " + (this.account?.primarycontact?.lastname || ""),
                email: this.account.contactEmail,
                phone: this.account.contactPhone
              };
              this.form.get("licenseeContact").patchValue(contact);
              this.dataLoaded = true;
            });
        }

      },
        () => {
          console.log("Error occured");

          this.dataLoaded = true;
        }
      );

  }


  // 2024-04-10 LCSD-6368 waynezen
  public autoCompFldEventHandler($event: any) {

    switch ($event.toString()) {
      case "autocompleteInput":
        // cursor entered autocomplete search by Licence fld - clear autocomplete search by JobNumber fld
        this.autocompletejobcomponent.autoCompFldClear();


        break;
      case "autocompleteJobNumber":
        // cursor entered autocomplete search by JobNumber fld - clear autocomplete search by Licence fld
        this.autocompletelicencecomponent.autoCompFldClear();
        break;
    }

  }

  private licenceHasRepresentativeContact(): boolean {
    let hasContact = false;
    if (this.licence && this.licence.representativeFullName) {
      hasContact = true;
    }
    return hasContact;
  }


  /**
   * Save form data
   * @param showProgress
   */
  save(showProgress: boolean = false, appData: Application = <Application>{}): Observable<boolean> {
    this.application = appData;
    const assignedLicenceId = this.form.get("assignedLicence.id").value;
    const assignedLicenceNumber = this.form.get("assignedLicence.licenceNumber").value;
    const assignedJobNumber = this.form.get("assignedLicence.jobNumber").value;


    console.log("Tied House Exemption===> Save: licenceId: " + this.licenceId + ", assignedLicenceId: " + assignedLicenceId + ", assignedLicenceNumber: " + assignedLicenceNumber + ", assignedJobNumber: " + assignedJobNumber);


    if (!this.licenceId) {
      this.application.parentApplicationId = this.applicationId;
      const applicationType = this.applicationType;
      const willHaveTiedHouseExemption = true;
      this.application.manufacturerProductionAmountForPrevYear = this.form.get("manufacturerProductionAmountforPrevYear").value;
      this.application.manufacturerProductionAmountUnit = this.form.get("manufacturerProductionAmountUnit").value;

      this.busy = forkJoin(

        this.applicationDataService.createApplication({
          ...this.application, assignedLicenceId, willHaveTiedHouseExemption, applicationType
        })
      ).pipe(takeWhile(() => this.componentActive))
        .pipe(catchError(() => {
          this.snackBar.open('Error saving Application', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
          return of(false);
        }))
        .pipe(mergeMap(() => {
          //this.savedFormData = saveData;
          //this.updateApplicationInStore();
          this.snackBar.open('Application has been saved', 'Success', { duration: 2500, panelClass: ['green-snackbar'] });
          return of(true);
        })).subscribe(res => {
          // this.saveComplete.emit(true);
          this.snackBar.open('Application Submitted to Local Government For Approval', 'Success', { duration: 2500, panelClass: ['green-snackbar'] });
          this.router.navigateByUrl('/dashboard');
        });

      // this.busy =
      return of(true);
    }
    else {

      if (assignedLicenceId) {
        //2024-04-25 LCSD-6368 waynezen; Has related Licence #; user has searched by Licence # or JobNumber

        return this.licenseDataService.initiateTiedHouseExcemption(assignedLicenceId,
            this.licenceId,
            this.form.get("manufacturerProductionAmountforPrevYear").value,
            this.form.get("manufacturerProductionAmountUnit").value)

          .pipe(takeWhile(() => this.componentActive))
          .pipe(catchError(() => {
            this.snackBar.open("Error submitting Tied House Exemption", "Fail", { duration: 3500, panelClass: ["red-snackbar"] });
            return of(false);
          }))
          .pipe(mergeMap(() => {
            if (showProgress === true) {
              this.snackBar.open("Tied House Exemption Invitation has been sent",
                "Success",
                { duration: 2500, panelClass: ["green-snackbar"] });
            }
            return of(true);
          }));
      }
      return of(true);
    }
  }

  normalizeFormData() {
    let description2 = '';

    // flatten the service areas if need be
    const serviceAreas = '';
    const outsideAreas = ('areas' in this.form.get('outsideAreas').value) ? this.form.get('outsideAreas').value['areas'] : this.form.get('outsideAreas').value;
    const capacityArea = [this.form.get('capacityArea').value];


    return {
      ...this.form.value,
      description2,
      serviceAreas,
      outsideAreas,
      capacityArea,
      indigenousNationId: this.form.value.indigenousNation && this.form.value.indigenousNation.id,
      policeJurisdictionId: this.form.value.policeJurisdiction && this.form.value.policeJurisdiction.id,
    }
  }
  /**
  * Create the application
  * */
  createApplication() {
    if (!this.isValid()) {
      this.showValidationMessages = true;
    } else {
      this.busy = this.save(true)
        .pipe(takeWhile(() => this.componentActive))
        .subscribe((result: boolean) => {
          if (result) {
            this.router.navigate(["/dashboard"]);
          }
        });
    }
  }

  isValid(): boolean {
    this.markControlsAsTouched(this.form);
    this.showValidationMessages = false;
    this.validationMessages = this.listControlsWithErrors(this.form, ValidationErrorMap);

    return this.form.valid;
  }

  businessTypeIsPartnership(): boolean {
    return this.account &&
      [
        "GeneralPartnership",
        "LimitedPartnership",
        "LimitedLiabilityPartnership",
        "Partnership"
      ].indexOf(this.account.businessType) !==
      -1;
  }

  businessTypeIsPrivateCorporation(): boolean {
    return this.account &&
      [
        "PrivateCorporation",
        "UnlimitedLiabilityCorporation",
        "LimitedLiabilityCorporation"
      ].indexOf(this.account.businessType) !==
      -1;
  }

  showFormControl(state: string): boolean {
    return [FormControlState.Show.toString(), FormControlState.ReadOnly.toString()]
      .indexOf(state) !==
      -1;
  }

  onLicenceSelect(assignedLicenceIn: RelatedLicence) {

    if (assignedLicenceIn.valid) {
      this.form.get("assignedLicence.licenceNumber").setValue("");
      this.form.get("assignedLicence.jobNumber").setValue("");

      this.form.get("assignedLicence").patchValue(assignedLicenceIn);

      // 2024-04-22 LCSD-6368 waynezen; change DOM dynamically, depending if user searched using Licence Num or Job Num
      if (assignedLicenceIn.jobNumber === null) {
        document.getElementById("lblapplicantname").innerHTML = "Licence Name:";
        document.getElementById("lblassiglicensee").innerHTML = "Assigned Licensee:";
        document.getElementById("fldassiglicensee").innerHTML = assignedLicenceIn.licensee;
      }
      else {
        document.getElementById("lblapplicantname").innerHTML = "Applicant Name:";
        document.getElementById("lblassiglicensee").innerHTML = "Job Number:";
        document.getElementById("fldassiglicensee").innerHTML = assignedLicenceIn.jobNumber;
      }
    }

  }

}
