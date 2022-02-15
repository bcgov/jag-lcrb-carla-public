import { Component, OnInit, Input, Output, EventEmitter } from "@angular/core";
import { forkJoin, Subscription } from "rxjs";
import { ApplicationDataService } from "@app/services/application-data.service";
import { LicenseDataService } from "@app/services/license-data.service";
import { Router } from "@angular/router";
import { Application } from "@models/application.model";
import { ApplicationSummary } from "@models/application-summary.model";
import { ApplicationTypeNames } from "@models/application-type.model";
import { Account } from "@models/account.model";
import { FeatureFlagService } from "@services/feature-flag.service";
import { FormBase } from "@shared/form-base";
import { takeWhile } from "rxjs/operators";
import { ApplicationLicenseSummary, LicenceActionApplication } from "@models/application-license-summary.model";
import { FormBuilder, FormGroup } from "@angular/forms";
import { LicenceEventsService } from "@services/licence-events.service";


export const UPLOAD_FILES_MODE = "UploadFilesMode";
export const INCOMPLETE = "Incomplete";
export const CRS_RENEWAL_LICENCE_TYPE_NAME = "crs";
export const LIQUOR_RENEWAL_LICENCE_TYPE_NAME = "liquor";


const ACTIVE = "Active";
const RENEWAL_DUE = "Renewal Due";

@Component({
  selector: "app-licences",
  templateUrl: "./licences.component.html",
  styleUrls: ["./licences.component.scss"]
})
export class LicencesComponent extends FormBase implements OnInit {
  applications: ApplicationSummary[] = [];
  mainForm: FormGroup;

  readonly ACTIVE = ACTIVE;
  readonly RENEWAL_DUE = RENEWAL_DUE;

  busy: Subscription;
  @Input()
  applicationInProgress: boolean;
  @Input()
  account: Account;
  @Output()
  marketerApplicationExists = new EventEmitter<boolean>();
  dataLoaded = false;
  ApplicationTypeNames = ApplicationTypeNames;
  licenceMappings = {};
  liquorThree: boolean;
  RLRS: boolean;

  // note, in order for a licence type to show on the dashboard, they must be configured here:
  supportedLicenceTypes = [
    "Catering", "Operated - Catering", "Deemed - Catering", "Transfer in Progress - Catering",
    "Wine Store", "Operated - Wine Store", "Deemed - Wine Store", "Transfer in Progress - Wine Store",
    "Cannabis Retail Store",
    "Marketing",
    "Manufacturer", "Operated - Manufacturer", "Deemed - Manufacturer", "Transfer in Progress - Manufacturer",
    "Licensee Retail Store", "Transfer in Progress - Licensee Retail Store", "Operated - Licensee Retail Store",
    "Deemed - Licensee Retail Store",
    "UBrew and UVin", "Transfer in Progress - UBrew and UVin", "Operated - UBrew and UVin", "Deemed - UBrew and UVin",
    "S119 CRS Authorization", "Transfer in Progress - S119 CRS Authorization"
  ];

  constructor(
    private applicationDataService: ApplicationDataService,
    private licenceDataService: LicenseDataService,
    private router: Router,
    public featureFlagService: FeatureFlagService,
    private licenceEventsService: LicenceEventsService,
    public fb: FormBuilder) {
    super();
    this.mainForm = new FormGroup({});

    featureFlagService.featureOn("LiquorThree")
      .subscribe(x => this.liquorThree = x);
    featureFlagService.featureOn("RLRS")
      .subscribe(x => this.RLRS = x);
  }

  ngOnInit() {
    if (this.liquorThree) {
      // control the licence rows as part of the feature flag.
      var liquorThree = ["Food Primary", "Transfer in Progress - Food Primary", "Operated - Food Primary", "Deemed - Food Primary",
        "Liquor Primary", "Transfer in Progress - Liquor Primary", "Operated - Liquor Primary", "Deemed - Liquor Primary",
        "Liquor Primary Club", "Transfer in Progress - Liquor Primary Club", "Operated - Liquor Primary Club", "Deemed - Liquor Primary Club",
        "Agent", "Transfer in Progress - Agent", "Operated - Agent", "Deemed - Agent"];
      this.supportedLicenceTypes = this.supportedLicenceTypes.concat(liquorThree);
    }

    if (this.RLRS) {
      // control the availability of the RLRS using the feature flag.
      var RLRS = ["Rural Licensee Retail Store"];
      this.supportedLicenceTypes = this.supportedLicenceTypes.concat(RLRS);
    }

    this.displayApplications();
  }

  /**
   *
   * */
  private displayApplications() {
    const sub = forkJoin([
      this.applicationDataService.getAllCurrentApplications(),
      this.licenceDataService.getAllCurrentLicenses(),
      this.licenceDataService.getAllOperatedLicenses(),
      this.licenceDataService.getAllProposedLicenses()
    ]
    ).pipe(takeWhile(() => this.componentActive))
      .subscribe(([applications, licenses, operatedLicences, proposedLicences = []]) => {
        this.applications = applications;
        operatedLicences.forEach(licence => {
          licence.isOperated = true;
          licence.licenceTypeName = `Operated - ${licence.licenceTypeName}`;
        });
        proposedLicences.forEach(licence => {
          licence.licenceTypeName = `Deemed - ${licence.licenceTypeName}`;
        });
        const combinedLicences = [
          // do not show transfers if the corresponding application is 'deemed'
          ...licenses.filter(lic => !(lic.licenceTypeName.includes("Transfer in Progress -") &&
            lic.checklistConclusivelyDeem)),
          ...operatedLicences,
          ...proposedLicences.filter(lic => lic.checklistConclusivelyDeem)
        ];

        combinedLicences.forEach((licence: ApplicationLicenseSummary) => {
          licence.headerRowSpan = 1;
          licence.hasPaidForRenewalApplication = this.hasPaidForRenewalApplication(licence);
          this.addOrUpdateLicence(licence);
        });

        this.dataLoaded = true;
      });
    this.subscriptionList.push(sub);
  }

  hasPaidForRenewalApplication(licence: ApplicationLicenseSummary): boolean {
    // check for any renewal application that is paid for and for the given licence
    const apps = this.applications.filter(app => {
      const isRenewalType = (
        app.applicationTypeName === ApplicationTypeNames.LiquorRenewal ||
        app.applicationTypeName === ApplicationTypeNames.CRSRenewal
      );
      return (isRenewalType && app.licenceId === licence.licenseId && app.isPaid);
    });

    return apps && apps.length > 0;
  }

  uploadMoreFiles(application: Application) {
    this.router.navigate([`/application/${application.id}`, { mode: UPLOAD_FILES_MODE }]);
  }

  addOrUpdateLicence(licence: ApplicationLicenseSummary) {
    licence.actionApplications = [];
    const relatedApplications = this.applications.filter(l => l.licenceId === licence.licenseId);
    relatedApplications.forEach(app => {
      const action = {
        applicationId: app.id,
        applicationTypeName: app.applicationTypeName,
        applicationStatus: app.applicationStatus,
        isPaid: app.isPaid,
        isStructuralChange: app?.isStructuralChange
      };
      licence.actionApplications.push(action);
    });
    if (this.licenceTypeHasEvents(licence.licenceTypeName)) {
      licence.eventsBusy = forkJoin([
        this.licenceEventsService.getLicenceEventsList(licence.licenseId, 20)
      ])
        .subscribe(data => {
          licence.events = data[0];
          if (licence.events.length > 0) {
            licence.headerRowSpan += 1;
          }
        });
    }

    if (typeof this.licenceMappings[licence.licenceTypeName] === "undefined") {
      this.licenceMappings[licence.licenceTypeName] = [];
    }
    const licenceIndex =
      this.licenceMappings[licence.licenceTypeName].findIndex(l => l.licenseId === licence.licenseId);
    if (licenceIndex >= 0) {
      this.licenceMappings[licence.licenceTypeName][licenceIndex] = licence;
    } else {
      this.licenceMappings[licence.licenceTypeName].push(licence);
    }
  }

  licenceTypeHasEvents(licenceType: string) {
    return licenceType.indexOf("Catering") >= 0 ||
      licenceType.indexOf("Wine Store") >= 0 ||
      licenceType.indexOf("Manufacturer") >= 0 ||
      licenceType.indexOf("Liquor Primary") >= 0 ||
      licenceType.indexOf("Agent") >= 0 ||
      licenceType.indexOf("Food Primary") >= 0;
  }

  getNumberOfLicences() {
    return Object.keys(this.licenceMappings).length;
  }

  LicenceTypeSupported(licenceType: string) {
    const supported = this.supportedLicenceTypes.indexOf(licenceType) >= 0;
    return supported;
  }
}
