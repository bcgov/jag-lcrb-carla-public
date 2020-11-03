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
import { takeWhile, filter, catchError, mergeMap, first } from 'rxjs/operators';
import { Account, TransferAccount } from '@models/account.model';
import { LicenseDataService } from '@services/license-data.service';
import { License } from '@models/license.model';

const ValidationErrorMap = {
  "proposedOwner.accountId": 'Please select the proposed transferee',
  transferConsent: 'Please consent to the transfer',
  authorizedToSubmit: 'Please affirm that you are authorized to submit the application.',
  signatureAgreement: 'Please affirm that all of the information provided for this application is true and complete.',
};

@Component({
  selector: 'app-application-ownership-transfer',
  templateUrl: './application-ownership-transfer.component.html',
  styleUrls: ['./application-ownership-transfer.component.scss']
})
export class ApplicationOwnershipTransferComponent extends FormBase implements OnInit {
  licence: License;
  form: FormGroup;
  licenceId: string;
  busy: Subscription;
  busyPromise: any;
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
      proposedOwner: this.fb.group({
        accountId: ['', [Validators.required]],
        accountName: [{ value: '', disabled: true }],
        contactName: [{ value: '', disabled: true }],
        businessType: [{ value: '', disabled: true }],
      }),
      licenseeContact: this.fb.group({
        name: [{ value: '', disabled: true }],
        email: [{ value: '', disabled: true }],
        phone: [{ value: '', disabled: true }]
      }),
      transferConsent: ['', [this.customRequiredCheckboxValidator()]],
      authorizedToSubmit: ['', [this.customRequiredCheckboxValidator()]],
      signatureAgreement: ['', [this.customRequiredCheckboxValidator()]],
    });

    // Get licence data
    this.busy = this.licenseDataService.getLicenceById(this.licenceId)
      .pipe(takeWhile(() => this.componentActive))
      .subscribe((licence: License) => {
        this.licence = licence;
        if (this.licenceHasRepresentativeContact()) { //If the licence has a representative, set it to be the licensee contact
          const contact = {
            name: this.licence.representativeFullName,
            email: this.licence.representativeEmail,
            phone: this.licence.representativePhoneNumber
          }
          this.form.get('licenseeContact').patchValue(contact);
        } else if (this.account) { // If the account is loaded, use it for the licensee contact
          const contact = {
            name: this.account.primarycontact.firstname + " " + this.account.primarycontact.lastname,
            email: this.account.contactEmail,
            phone: this.account.contactPhone
          }
          this.form.get('licenseeContact').patchValue(contact);
        } else { // Otherwise load the account and use it for the licensee representative
          this.store.select(state => state.currentAccountState.currentAccount)
          .pipe(filter(account => !!account))
          .pipe(first())
            .subscribe((account) => {
              this.account = account;
              const contact = {
                name: this.account.primarycontact.firstname + " " + this.account.primarycontact.lastname,
                email: this.account.contactEmail,
                phone: this.account.contactPhone
              }
              this.form.get('licenseeContact').patchValue(contact);
            });
        }

        this.form.patchValue(this.licence);
      },
        () => {
          console.log('Error occured');
        }
      );
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
  save(showProgress: boolean = false): Observable<boolean> {
    return this.licenseDataService.initiateTransfer(this.licence.id, this.form.get('proposedOwner.accountId').value)
      .pipe(takeWhile(() => this.componentActive))
      .pipe(catchError(() => {
        this.snackBar.open('Error submitting transfer', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
        return of(false);
      }))
      .pipe(mergeMap(() => {
        if (showProgress === true) {
          this.snackBar.open('Transfer has been initiated', 'Success', { duration: 2500, panelClass: ['green-snackbar'] });
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
    this.form.get('proposedOwner').patchValue(proposedAccount);
  }

}
