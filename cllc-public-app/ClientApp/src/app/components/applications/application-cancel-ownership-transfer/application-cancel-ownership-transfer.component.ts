import { Component, OnInit } from '@angular/core';
import { FormBase } from '@shared/form-base';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Subscription, Observable, of } from 'rxjs';
import { ApplicationTypeNames, FormControlState } from '@models/application-type.model';
import { Store } from '@ngrx/store';
import { AppState } from '@app/app-state/models/app-state';
import { PaymentDataService } from '@services/payment-data.service';
import { MatSnackBar, MatDialog } from '@angular/material';
import { Router, ActivatedRoute } from '@angular/router';
import { FeatureFlagService } from '@services/feature-flag.service';
import { EstablishmentWatchWordsService } from '@services/establishment-watch-words.service';
import { takeWhile, filter, catchError, mergeMap } from 'rxjs/operators';
import { Account, TransferAccount } from '@models/account.model';
import { LicenseDataService } from '@services/license-data.service';
import { License } from '@models/license.model';
import { ApplicationHTMLContent } from '../application/application.component';

@Component({
  selector: 'app-application-cancel-ownership-transfer',
  templateUrl: './application-cancel-ownership-transfer.component.html',
  styleUrls: ['./application-cancel-ownership-transfer.component.scss']
})
export class ApplicationCancelOwnershipTransferComponent extends FormBase implements OnInit {
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
      establishmentName: [''],
      establishmentAddressStreet: [''],
      establishmentAddressCity: [''],
      establishmentAddressPostalCode: [''],
      establishmentParcelId: [''],      
      transferConsent: ['', [this.customRequiredCheckboxValidator()]]     
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
        return this.licenseDataService.cancelTransfer(this.licence.id, this.account.id)
      .pipe(takeWhile(() => this.componentActive))
      .pipe(catchError(() => {
        this.snackBar.open('Error submitting cancel transfer', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
        return of(false);
      }))
      .pipe(mergeMap(() => {
        if (showProgress === true) {
          this.snackBar.open('Transfer has been cancelled', 'Success', { duration: 2500, panelClass: ['green-snackbar'] });
        }
        return of(true);
      }));
  }

  /**
  * Initiate licence transfer
  * */
  cancelTransfer() {
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
    // mark controls as touched
    for (const c in this.form.controls) {
      if (typeof (this.form.get(c).markAsTouched) === 'function') {
        this.form.get(c).markAsTouched();
      }
    }
    this.showValidationMessages = false;
    let valid = true;
    this.validationMessages = [];


    if (!this.form.valid) {
      valid = false;
      this.validationMessages.push('Some required fields have not been completed');
    }
    return valid;
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
    return [FormControlState.Show.toString(), FormControlState.Reaonly.toString()]
      .indexOf(state) !== -1;
  }

  onAccountSelect(proposedAccount: TransferAccount) {
    this.form.get('proposedOwner').patchValue(proposedAccount);
  }

}
