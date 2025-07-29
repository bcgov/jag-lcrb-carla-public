import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import { AppState } from '@app/app-state/models/app-state';
import { faIdCard, faSave, faTrashAlt } from '@fortawesome/free-regular-svg-icons';
import { faQuestionCircle } from '@fortawesome/free-solid-svg-icons';
import { Account } from '@models/account.model';
import { ApplicationLicenseSummary } from '@models/application-license-summary.model';
import { Application } from '@models/application.model';
import { TiedHouseViewMode } from '@models/tied-house-connection.model';
import { Store } from '@ngrx/store';
import { ApplicationDataService } from '@services/application-data.service';
import { GenericConfirmationDialogComponent } from '@shared/components/dialog/generic-confirmation-dialog/generic-confirmation-dialog.component';
import { FormBase } from '@shared/form-base';
import { Observable, of } from 'rxjs';
import { catchError, filter, mergeMap, takeWhile } from 'rxjs/operators';
import { TiedHouseDeclarationComponent } from '../tied-house-decleration/tied-house-declaration.component';

export const SharepointNameRegex = /^[^~#%&*{}\\:<>?/+|""]*$/;

/**
 * A component that displays a form page for a legal entity review application.
 *
 * This is step 1 of the legal entity review process, where the user submits supporting documents and declares any tied
 * house connections.
 * For step 2, see `LegalEntityReviewPermanentChangeToALicenseeComponent`.
 *
 * @export
 * @class LegalEntityReviewComponent
 * @extends {FormBase}
 * @implements {OnInit}
 */
@Component({
  selector: 'app-legal-entity-review',
  templateUrl: './legal-entity-review.component.html',
  styleUrls: ['./legal-entity-review.component.scss']
})
export class LegalEntityReviewComponent extends FormBase implements OnInit {
  @ViewChild('tiedHouseDeclaration')
  tiedHouseDeclaration: TiedHouseDeclarationComponent;
  isTiedHouseVisible: boolean = false;

  account: Account;
  application: Application;
  liquorLicences: ApplicationLicenseSummary[] = [];
  cannabisLicences: ApplicationLicenseSummary[] = [];

  applicationId: string;

  validationMessages: string[];
  showValidationMessages: boolean;

  isDataLoaded: boolean;
  isSubmitting: boolean;

  form: FormGroup;

  faQuestionCircle = faQuestionCircle;
  faIdCard = faIdCard;
  faSave = faSave;
  faTrashAlt = faTrashAlt;

  get hasLiquor(): boolean {
    return this.liquorLicences.length > 0;
  }

  get hasCannabis(): boolean {
    return this.cannabisLicences.length > 0;
  }

  constructor(
    private applicationDataService: ApplicationDataService,
    private route: ActivatedRoute,
    private router: Router,
    private snackBar: MatSnackBar,
    private fb: FormBuilder,
    private store: Store<AppState>,
    private dialog: MatDialog
  ) {
    super();

    this.store
      .select((state) => state.currentAccountState.currentAccount)
      .pipe(filter((account) => !!account))
      .subscribe((account) => {
        this.account = account;
      });

    this.route.paramMap.subscribe((pmap) => {
      this.applicationId = pmap.get('applicationId');
    });
  }

  ngOnInit(): void {
    this.initForm();
    this.loadData();
  }

  /**
   * Initializes the form with the required controls and validators.
   *
   * @private
   */
  private initForm() {
    this.form = this.fb.group({
      authorizedToSubmit: ['', [this.customRequiredCheckboxValidator()]],
      signatureAgreement: ['', [this.customRequiredCheckboxValidator()]]
    });
  }

  /**
   * Loads the form data.
   *
   * @private
   */
  private loadData() {
    const sub = this.applicationDataService
      .getLegalEntityPermanentChangesToLicenseeData(this.applicationId)
      .subscribe((data) => {
        this.setFormData(data);
      });

    this.subscriptionList.push(sub);
  }

  /**
   * Sets the form data based on the provided application, licences, and invoice information.
   *
   * @private
   * @param {*} { application, licences }
   */
  private setFormData({ application, licences }) {
    this.liquorLicences = licences.filter((item) => item.licenceTypeCategory === 'Liquor' && item.status === 'Active');

    this.cannabisLicences = licences.filter(
      (item) => item.licenceTypeCategory === 'Cannabis' && item.status === 'Active'
    );

    this.application = application;

    this.form.patchValue(application);

    this.isDataLoaded = true;
  }

  /**
   * Submit the application.
   */
  onSubmit() {
    if (!this.isValid()) {
      this.showValidationMessages = true;
      this.markControlsAsTouched(this.form);

      return;
    }

    this.isSubmitting = true;

    this.application.invoiceTrigger = 0;

    this.save()
      .pipe(takeWhile(() => this.componentActive))
      .subscribe(([saveSucceeded]) => {
        if (!saveSucceeded) {
          this.snackBar.open('Error saving Application', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
        }

        this.isSubmitting = false;
      });
  }

  /**
   * Checks if the form is valid and collects validation messages.
   *
   * @return {*}  {boolean}
   */
  private isValid(): boolean {
    this.showValidationMessages = false;
    this.validationMessages = this.listControlsWithErrors(this.form, this.getValidationErrorMap());
    let valid = this.form.disabled || this.form.valid;

    if (!this.areTiedHouseDeclarationsValid()) {
      this.validationMessages.push('Tide House Declaration has not been saved.');
      valid = false;
    }

    return valid;
  }

  /**
   * Saves the application data.
   *
   * @private
   * @return {*}  {Observable<[boolean, Application]>}
   */
  private save(): Observable<[boolean, Application]> {
    return this.applicationDataService
      .submitLegalEntityApplication({
        ...this.application,
        ...this.form.value
      })
      .pipe(takeWhile(() => this.componentActive))
      .pipe(
        catchError(() => {
          this.snackBar.open('Error saving Application', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });

          const res: [boolean, Application] = [false, null];

          return of(res);
        })
      )
      .pipe(
        mergeMap((data) => {
          this.snackBar.open('Application has been saved', 'Success', {
            duration: 3500,
            panelClass: ['green-snackbar']
          });

          const res: [boolean, Application] = [true, data as Application];
          this.router.navigate(['/dashboard']);
          return of(res);
        })
      );
  }

  /**
   * Returns a map of validation error messages for the form controls.
   *
   * @private
   * @return {*}
   */
  private getValidationErrorMap() {
    const errorMap = {
      signatureAgreement:
        'Please affirm that all of the information provided for this application is true and complete',
      authorizedToSubmit: 'Please affirm that you are authorized to submit the application'
    };
    return errorMap;
  }

  /**
   * Cancels the application and returns the user to the dashboard page.
   */
  onCancel() {
    this.dialog.open(GenericConfirmationDialogComponent, {
      disableClose: true,
      autoFocus: true,
      data: {
        title: 'Cancel Legal Entity Review',
        message: `Are you sure you want to cancel? Any unsaved changes will be lost.`,
        confirmButtonText: 'Yes, Cancel',
        cancelButtonText: 'No, Go Back',
        onConfirm: () => {
          this.router.navigate(['/dashboard']);
        }
      }
    });
  }

  /**
   * Toggles the visibility of the tied house connections section.
   *
   * @param {boolean} showValue
   */
  showTiedHouseConnections(showValue: boolean) {
    this.isTiedHouseVisible = showValue;
  }

  /**
   * Checks if the tied house declarations are valid.
   *
   * @return {*}  {boolean} `true` if valid, `false` otherwise.
   */
  areTiedHouseDeclarationsValid(): boolean {
    if (!this.isTiedHouseVisible || !this.tiedHouseDeclaration) {
      // If the tied house section is not visible, or the component is not initialized, we assume it's valid.
      return true;
    }

    if (
      this.tiedHouseDeclaration?.tiedHouseDeclarations.find((item) =>
        [TiedHouseViewMode.new, TiedHouseViewMode.editExistingRecord, TiedHouseViewMode.addNewRelationship].includes(
          item.viewMode
        )
      )
    ) {
      // One or more declarations are in an unsaved state.
      return false;
    }

    return true;
  }
}
