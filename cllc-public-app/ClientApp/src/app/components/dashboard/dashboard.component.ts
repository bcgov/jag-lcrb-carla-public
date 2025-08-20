import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { AppState } from '@app/app-state/models/app-state';
import { faExclamationTriangle, faPencilAlt } from '@fortawesome/free-solid-svg-icons';
import { Account } from '@models/account.model';
import { ApplicationType, ApplicationTypeNames } from '@models/application-type.model';
import { Application } from '@models/application.model';
import { LegalEntity } from '@models/legal-entity.model';
import { LicenseeChangeLog } from '@models/licensee-change-log.model';
import { OutstandingPriorBalanceInvoice } from '@models/outstanding-prior-balance-invoce.model';
import { Store } from '@ngrx/store';
import { ApplicationDataService } from '@services/application-data.service';
import { LegalEntityDataService } from '@services/legal-entity-data.service';
import { LicenseDataService } from '@services/license-data.service';
import { PaymentDataService } from '@services/payment-data.service';
import { FormBase } from '@shared/form-base';
import { Subscription } from 'rxjs';
import { takeWhile } from 'rxjs/operators';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent extends FormBase implements OnInit {
  faPencilAlt = faPencilAlt;
  faExclamationTriangle = faExclamationTriangle;
  account: Account;
  indigenousNationModeActive: boolean;
  currentLegalEntities: LegalEntity;
  tree: LicenseeChangeLog;
  hasLicence: boolean;
  busy: Subscription;
  outstandingBalancePriorInvoiceData: OutstandingPriorBalanceInvoice[] = [];
  isOutstandingBalancePriorInvoiceDue: boolean;
  canCreatePCLApplication = true;

  constructor(
    private store: Store<AppState>,
    private router: Router,
    private snackBar: MatSnackBar,
    private legalEntityDataService: LegalEntityDataService,
    private licenseDataService: LicenseDataService,
    private applicationDataService: ApplicationDataService,
    private paymentService: PaymentDataService
  ) {
    super();
  }

  ngOnInit(): void {
    this.outstandingBalancePriorInvoiceData = [];

    this.store
      .select((state) => state.currentAccountState.currentAccount)
      .pipe(takeWhile(() => this.componentActive))
      .subscribe((account) => {
        this.account = account;

        if (this.account && this.account.id) {
          let sub = this.licenseDataService.getAllCurrentLicenses().subscribe((licences) => {
            this.hasLicence = licences.length > 0;
          });

          this.subscriptionList.push(sub);

          this.store
            .select((state) => state.indigenousNationState.indigenousNationModeActive)
            .pipe(takeWhile(() => this.componentActive))
            .subscribe((active) => {
              this.indigenousNationModeActive = active;
            });

          sub = this.legalEntityDataService
            .getCurrentHierachy()
            .pipe(takeWhile(() => this.componentActive))
            .subscribe(
              (data: LegalEntity) => {
                this.tree = LicenseeChangeLog.CreateFromLegalEntity(data);
                this.tree.isRoot = true;
              },
              () => {
                console.log('Error occured');
              }
            );
          this.subscriptionList.push(sub);

          this.licenseDataService
            .getOutstandingBalancePriorInvoices()
            .pipe(takeWhile(() => this.componentActive))
            .subscribe((data) => {
              data.forEach((item: OutstandingPriorBalanceInvoice) => {
                this.outstandingBalancePriorInvoiceData.push(item);
                if (!this.isOutstandingBalancePriorInvoiceDue && item.overdue) {
                  this.isOutstandingBalancePriorInvoiceDue = true;
                }
              });
            });
        }
      });
  }

  startLicenseeChangeApplication() {
    const newLicenceApplicationData = {
      // licenseType: ApplicationTypeNames.LeaderhsipChange,
      applicantType: this.account.businessType,
      applicationType: { name: ApplicationTypeNames.LicenseeChanges } as ApplicationType,
      account: this.account
    } as Application;

    this.applicationDataService.createApplication(newLicenceApplicationData).subscribe(
      (data) => {
        this.router.navigateByUrl(`/licensee-changes/${data.id}`);
      },
      () => {
        this.snackBar.open('Error starting a New Licensee Application', 'Fail', {
          duration: 3500,
          panelClass: ['red-snackbar']
        });
        console.log('Error starting a New Licensee Application');
      }
    );
  }

  payOutstandingPriorBalanceInvoice(applicationId: string) {
    if (applicationId) {
      this.busy = this.paymentService
        .payOutstandingPriorBalanceInvoicePaymentSubmissionUrl(applicationId)
        .pipe(takeWhile(() => this.componentActive))
        .subscribe(
          (res) => {
            const data = res as any;
            window.location.href = data.url;
          },
          (err) => {
            if (err === 'Payment already made') {
              this.snackBar.open(
                'Outstanding Prior Balance Payment has already been made, please refresh the page',
                'Fail',
                { duration: 3500, panelClass: ['red-snackbar'] }
              );
            } else {
              this.snackBar.open('Outstanding Prior Balance Payment fail, please try later.', 'Fail', {
                duration: 3500,
                panelClass: ['red-snackbar']
              });
            }
          }
        );
    }
  }

  handleCanCreatePCLApplicationEvent(event: boolean) {
    this.canCreatePCLApplication = event;
  }
}
