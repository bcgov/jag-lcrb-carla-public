import { Component, OnInit } from "@angular/core";
import { SecurityScreeningSummary } from "@models/security-screening-summary.model";
import { MatSnackBar } from "@angular/material/snack-bar";
import { LegalEntityDataService } from "@services/legal-entity-data.service";
import { ActivatedRoute } from "@angular/router";
import { ApplicationDataService } from "@services/application-data.service";
import { LicenseDataService } from "@services/license-data.service";
import { ApplicationType } from "@models/application-type.model";
import { PaymentDataService } from "@services/payment-data.service";
import { Subscription, forkJoin } from "rxjs";
import { faCheck, faCopy, faExclamation } from "@fortawesome/free-solid-svg-icons";
import { FeatureFlagService } from "@services/feature-flag.service";
import { LEConnectionsDataService } from "@services/le-connections-data.service";

@Component({
  selector: "app-security-screening-requirements",
  templateUrl: "./security-screening-requirements.component.html",
  styleUrls: ["./security-screening-requirements.component.scss"]
})
export class SecurityScreeningRequirementsComponent implements OnInit {
  faCopy = faCopy;
  faExclamation = faExclamation;
  faCheck = faCheck;
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
  dataLoaded: boolean;
  LEConnectionsFeatureOn: boolean;

  constructor(private snackBar: MatSnackBar,
    private route: ActivatedRoute,
    private applicationDataService: ApplicationDataService,
    private licenseDataService: LicenseDataService,
    private paymentDataService: PaymentDataService,
    private featureFlagService: FeatureFlagService,
    private leConnectionsDataService: LEConnectionsDataService,
    private legalEntityDataService: LegalEntityDataService) {
    this.route.paramMap.subscribe(pmap => this.applicationId = pmap.get("applicationId"));
  }

  ngOnInit() {

    if (this.applicationId) {
      this.applicationDataService.getApplicationById(this.applicationId)
        .subscribe((application) => {
          this.applicationType = application.applicationType;
          if (application.applicationType.category === "Liquor") {
            this.isLiquorApplication = true;
          }
          if (application.applicationType.category === "Cannabis") {
            this.isCannabisApplication = true;
          }

          if (application.applicant !== null &&
            ["LocalGovernment", "IndigenousNation"].indexOf(application.applicant.businessType) >= 0) {
            this.skipScreeningRequirements = true;
          }
        });
    }

    this.featureFlagService.featureOn("LEConnections")
      .subscribe(LEConnectionsFeatureOn => {
        // call the appropriate service depending on the feature flag
        let securitySummary = this.legalEntityDataService.getCurrentSecurityScreeningItems();
        if (LEConnectionsFeatureOn) {
          securitySummary = this.leConnectionsDataService.getCurrentSecurityScreeningItems();
        }
        forkJoin([
          securitySummary,
          this.licenseDataService.getAllCurrentLicenses()
        ]).subscribe(([summary, licences]) => {
          this.data = summary;
          this.liquorLicenceExist = licences.filter(lc => lc.licenceTypeCategory === "Liquor").length > 0;
          this.cannabisLicenceExist = licences.filter(lc => lc.licenceTypeCategory === "Cannabis").length > 0;
          this.dataLoaded = true;
        });
      });

  }

  // Copy value to clipboard
  copyMessage(value: string) {
    const selBox = document.createElement("textarea");
    selBox.style.position = "fixed";
    selBox.style.left = "0";
    selBox.style.top = "0";
    selBox.style.opacity = "0";
    selBox.value = value;
    document.body.appendChild(selBox);
    selBox.focus();
    selBox.select();
    document.execCommand("copy");
    document.body.removeChild(selBox);
    this.snackBar.open("The link is copied to the clipboard and can be shared",
      "",
      { duration: 2500, panelClass: ["green-snackbar"] });
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
    const valid = true;
    if (this.showLiquorContent() &&
      this.applicationId &&
      this.data &&
      this.data.liquor &&
      this.data.liquor.outstandingItems &&
      this.data.liquor.outstandingItems.length &&
      this.data.liquor.outstandingItems.length > 0) {
      this.errorMessages.push("Please complete all outstanding items");
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
            window.location.href = jsonUrl["url"];
            return jsonUrl["url"];
          },
          err => {
            if (err === "Payment already made") {
              this.snackBar.open("Application payment has already been made, please refresh the page.",
                "Fail",
                { duration: 3500, panelClass: ["red-snackbar"] });
            }
          });
    }
  }

}
