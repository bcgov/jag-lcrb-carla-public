import { Component, OnInit } from '@angular/core';
import { SecurityScreeningCategorySummary } from '@models/security-screening-category-summary.model';
import { SecurityScreeningSummary } from '@models/security-screening-summary.model';
import { MatSnackBar } from '@angular/material';
import { LegalEntityDataService } from '@services/legal-entity-data.service';
import { ActivatedRoute } from '@angular/router';
import { ApplicationDataService } from '@services/application-data.service';
import { LicenseDataService } from '@services/license-data.service';
import { ApplicationType } from '@models/application-type.model';
import { PaymentDataService } from '@services/payment-data.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-security-screening-requirements',
  templateUrl: './security-screening-requirements.component.html',
  styleUrls: ['./security-screening-requirements.component.scss']
})
export class SecurityScreeningRequirementsComponent implements OnInit {
  data: SecurityScreeningSummary;
  busy: Subscription;
  applicationId: string;
  applicationType: ApplicationType;
  liquorLicenceExist: boolean;
  cannabisLicenceExist: boolean;
  isLiquorApplication: boolean;
  isCannabisApplication: boolean;
  errorMessages: string[] = [];
  skipScreeningRequirements: boolean = false;

  constructor(private snackBar: MatSnackBar,
    private route: ActivatedRoute,
    private applicationDataService: ApplicationDataService,
    private licenseDataService: LicenseDataService,
    private paymentDataService: PaymentDataService,
    private legalEntityDataService: LegalEntityDataService) {
    this.busy = this.legalEntityDataService.getCurrentSecurityScreeningItems()
      .subscribe(summary => {
        this.data = summary;
      });

    this.route.paramMap.subscribe(pmap => this.applicationId = pmap.get('applicationId'));
  }

  ngOnInit() {

    if (this.applicationId) {
      this.applicationDataService.getApplicationById(this.applicationId)
        .subscribe((application) => {
          this.applicationType = application.applicationType;
          if (application.applicationType.category === 'Liquor') {
            this.isLiquorApplication = true;
          }
          if (application.applicationType.category === 'Cannabis') {
            this.isCannabisApplication = true;
          }
          if (application.account !== null && (application.account.isLocalGovernment() || application.account.isIndigenousNation())) {
            this.skipScreeningRequirements = true;
          }
        });
    }

    this.busy = this.licenseDataService.getAllCurrentLicenses()
      .subscribe(licences => {
        this.liquorLicenceExist = licences.filter(lc => lc.licenceTypeCategory === 'Liquor').length > 0;
        this.cannabisLicenceExist = licences.filter(lc => lc.licenceTypeCategory === 'Cannabis').length > 0;
      });
  }

  // Copy value to clipboard
  copyMessage(value: string) {
    const selBox = document.createElement('textarea');
    selBox.style.position = 'fixed';
    selBox.style.left = '0';
    selBox.style.top = '0';
    selBox.style.opacity = '0';
    selBox.value = value;
    document.body.appendChild(selBox);
    selBox.focus();
    selBox.select();
    document.execCommand('copy');
    document.body.removeChild(selBox);
    this.snackBar.open('The link is copied to the clipboard and can be shared', '', { duration: 2500, panelClass: ['green-snackbar'] });
  }

  showLiquorContent(): boolean {
    let show = false;
    // always show the liquor content if accessing from the dashboard 
    // or if it is for a liquor application
    if ((!this.applicationId) || this.isLiquorApplication) {
      show = true;
    }
    return show;
  }

  showCannabisContent(): boolean {
    let show = false;
    // always show the Cannabis content if accessing from the dashboard 
    // or if it is for a cannabis application
    if ((!this.applicationId) || this.isCannabisApplication) {
      show = true;
    }
    return show;
  }

  isValid(): boolean {
    this.errorMessages = [];
    let valid = true;
    if (this.showLiquorContent() && this.applicationId && this.data && this.data.liquor && this.data.liquor.outstandingItems && this.data.liquor.outstandingItems.length && this.data.liquor.outstandingItems.length > 0) {
      this.errorMessages.push('Please complete all outstanding items');
    }
    return valid;
  }

  /**
 * Redirect to payment processing page (Express Pay / Bambora service)
 * */
  private submitApplicationPayment() {
    if (this.isValid()) {
      this.paymentDataService.getPaymentSubmissionUrl(this.applicationId)
        .subscribe(jsonUrl => {
          window.location.href = jsonUrl['url'];
          return jsonUrl['url'];
        }, err => {
          if (err._body === 'Payment already made') {
            this.snackBar.open('Application payment has already been made.', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
          }
        });
    }
  }

}
