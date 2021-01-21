import { Component, OnInit } from "@angular/core";
import { FormBase, ApplicationHTMLContent } from "@shared/form-base";
import { FormGroup, FormBuilder, Validators } from "@angular/forms";
import { Subscription, Observable, of } from "rxjs";
import { ApplicationTypeNames, FormControlState } from "@models/application-type.model";
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

const ValidationErrorMap = {
  description2: "Please enter the reason for your request.",
  authorizedToSubmit: "Please affirm that you are authorized to submit the application.",
  signatureAgreement: "Please affirm that all of the information provided for this application is true and complete.",
};

@Component({
  selector: 'app-application-request-term-change',
  templateUrl: './application-request-term-change.component.html',
  styleUrls: ['./application-request-term-change.component.scss']
})
export class ApplicationRequestTermChangeComponent extends FormBase implements OnInit {
  faSave = faSave;
  faTrashAlt = faTrashAlt;
  licenceId: string;
  termId: string;
  licence: License;
  busy: Subscription;
  busyPromise: any;
  validationMessages: any[];
  showValidationMessages: boolean;
  htmlContent = {} as ApplicationHTMLContent;
  FormControlState = FormControlState;
  dataLoaded: boolean;

  constructor(private store: Store<AppState>,
    public snackBar: MatSnackBar,
    public router: Router,
    private licenseDataService: LicenseDataService,
    public featureFlagService: FeatureFlagService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    public dialog: MatDialog,
    public establishmentWatchWordsService: EstablishmentWatchWordsService) {
    super();
    this.route.paramMap.subscribe(pmap => {
      this.licenceId = pmap.get("licenceId");
      this.termId = pmap.get("termId");
    });

  }

  ngOnInit(): void {
    this.form = this.fb.group({
      licenseNumber: [""],
      establishmentName: [""],
      description2: ["", Validators.required],
      authorizedToSubmit: ["", [this.customRequiredCheckboxValidator()]],
      signatureAgreement: ["", [this.customRequiredCheckboxValidator()]],
    });


    // Get licence data
    this.busy = this.licenseDataService.getLicenceById(this.licenceId)
      .pipe(takeWhile(() => this.componentActive))
      .subscribe((licence: License) => {
        this.licence = licence;
        this.form.patchValue(this.licence);
        this.dataLoaded = true;
      },
        () => {
          console.log("Error occured");

          this.dataLoaded = true;
        }
      );
  
  }

  /**
   * Save form data
   * @param showProgress
   */
  save(showProgress: boolean = false): Observable<boolean> {
    return this.licenseDataService.initiateTransfer(this.licence.id, this.form.get("proposedOwner.accountId").value)
      .pipe(takeWhile(() => this.componentActive))
      .pipe(catchError(() => {
        this.snackBar.open("Error submitting request", "Fail", { duration: 3500, panelClass: ["red-snackbar"] });
        return of(false);
      }))
      .pipe(mergeMap(() => {
        if (showProgress === true) {
          this.snackBar.open("Request for change has been initiated",
            "Success",
            { duration: 2500, panelClass: ["green-snackbar"] });
        }
        return of(true);
      }));
  }

  /**
  * Initiate the request
  * */
  initiateRequest() {
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

}
