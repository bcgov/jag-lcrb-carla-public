import { Component, OnInit } from '@angular/core';
import { FormBase, ApplicationHTMLContent } from '@shared/form-base';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Subscription, Observable, of } from 'rxjs';
import { ApplicationTypeNames, FormControlState } from '@models/application-type.model';
import { Store } from '@ngrx/store';
import { AppState } from '@app/app-state/models/app-state';
import { MatSnackBar, MatDialog } from '@angular/material';
import { Router, ActivatedRoute } from '@angular/router';
import { FeatureFlagService } from '@services/feature-flag.service';
import { EstablishmentWatchWordsService } from '@services/establishment-watch-words.service';
import { takeWhile, filter, catchError, mergeMap } from 'rxjs/operators';
import { Account, TransferAccount } from '@models/account.model';
import { LicenseDataService } from '@services/license-data.service';
import { License } from '@models/license.model';

const ValidationErrorMap = {
  "proposedTPO.accountId": 'Please select the business name to be a third party operator of  your licence',
  authorizedToSubmit: 'Please affirm that you are authorized to submit the application.',
  signatureAgreement: 'Please affirm that all of the information provided for this application is true and complete.',
};

@Component({
  selector: 'app-application-third-party-operator',
  templateUrl: './application-third-party-operator.component.html',
  styleUrls: ['./application-third-party-operator.component.scss']
})
export class ApplicationThirdPartyOperatorComponent extends FormBase implements OnInit {
  licence: License;
  form: FormGroup;
  licenceId: string;
  busy: Subscription;
  validationMessages: any[];
  showValidationMessages: boolean;
  htmlContent: ApplicationHTMLContent = <ApplicationHTMLContent>{};
  ApplicationTypeNames = ApplicationTypeNames;
  FormControlState = FormControlState;
  account: Account;
  minDate = new Date();


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
    this.route.paramMap.subscribe(pmap => this.licenceId = pmap.get('licenceId'));
  }

  ngOnInit() {
    this.form = this.fb.group({
      licenseNumber: [''],
      establishmentName: [''],
      establishmentAddressStreet: [''],
      establishmentAddressCity: [''],
      establishmentAddressPostalCode: [''],
      establishmentParcelId: [''],
      proposedTPO: this.fb.group({
        accountId: ['', [Validators.required]],
        accountName: [{value: '', disabled: true}],
        contactName: [{value: '', disabled: true}],
        businessType: [{value: '', disabled: true}],
      }),
      authorizedToSubmit: ['', [this.customRequiredCheckboxValidator()]],
      signatureAgreement: ['', [this.customRequiredCheckboxValidator()]],
    });

    this.store.select(state => state.currentAccountState.currentAccount)
      .pipe(takeWhile(() => this.componentActive))
      .pipe(filter(account => !!account))
      .subscribe((account) => {
        this.account = account;
      });


    this.busy = this.licenseDataService.getLicenceById(this.licenceId)
      .pipe(takeWhile(() => this.componentActive))
      .subscribe((data: License) => {


        this.licence = data;
        this.form.patchValue(data);
      },
        () => {
          console.log('Error occured');
        }
      );
  }





  /**
   * Save form data
   * @param showProgress
   */
  save(showProgress: boolean = false): Observable<boolean> {
    return this.licenseDataService.setThirdPartyOperator(this.licence.id, this.form.get('proposedTPO.accountId').value)
      .pipe(takeWhile(() => this.componentActive))
      .pipe(catchError(() => {
        this.snackBar.open('Error submitting transfer', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
        return of(false);
      }))
      .pipe(mergeMap(() => {
        if (showProgress === true) {
          this.snackBar.open('Third Party Operator Application has been initiated', 'Success', { duration: 2500, panelClass: ['green-snackbar'] });
        }
        return of(true);
      }));
  }

  /**
  * Initiate licence transfer
  * */
  initiateTransfer() {
    if (!this.isValid()) {
      this.showValidationMessages = true;
    } else {
      this.busy = this.save(true)
        .pipe(takeWhile(() => this.componentActive))
        .subscribe((result: boolean) => {
          if (result) {
            this.router.navigate(['/dashboard']);
          }
        });
    }
  }

  isValid(): boolean {
    this.markControlsAsTouched(this.form);
    this.showValidationMessages = false;
    let valid = true;
    this.validationMessages = this.listControlsWithErrors(this.form, ValidationErrorMap);
    return this.form.valid;
  }

  businessTypeIsPartnership(): boolean {
    return this.account &&
      ['GeneralPartnership',
        'LimitedPartnership',
        'LimitedLiabilityPartnership',
        'Partnership'].indexOf(this.account.businessType) !== -1;
  }

  businessTypeIsPrivateCorporation(): boolean {
    return this.account &&
      ['PrivateCorporation',
        'UnlimitedLiabilityCorporation',
        'LimitedLiabilityCorporation'].indexOf(this.account.businessType) !== -1;
  }

  showFormControl(state: string): boolean {
    return [FormControlState.Show.toString(), FormControlState.ReadOnly.toString()]
      .indexOf(state) !== -1;
  }

  onAccountSelect(proposedAccount: TransferAccount) {
    this.form.get('proposedTPO').patchValue(proposedAccount);
  }

}
