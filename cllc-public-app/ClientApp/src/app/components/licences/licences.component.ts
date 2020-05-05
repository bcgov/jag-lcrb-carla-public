import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { forkJoin, Subscription } from 'rxjs';
import { MatSnackBar } from '@angular/material';
import { ApplicationDataService } from '@app/services/application-data.service';
import { LicenseDataService } from '@app/services/license-data.service';
import { Router } from '@angular/router';
import { Application } from '@models/application.model';
import { ApplicationSummary } from '@models/application-summary.model';
import { ApplicationTypeNames } from '@models/application-type.model';
import { Account } from '@models/account.model';
import { FeatureFlagService } from '@services/feature-flag.service';
import { FormBase } from '@shared/form-base';
import { takeWhile } from 'rxjs/operators';
import { ApplicationLicenseSummary } from '@models/application-license-summary.model';
import { Store } from '@ngrx/store';
import { AppState } from '@app/app-state/models/app-state';
import * as moment from 'moment';
import { PaymentDataService } from '@services/payment-data.service';
import { EstablishmentDataService } from '@services/establishment-data.service';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Establishment } from '@models/establishment.model';
import { LicenceEventsService } from '@services/licence-events.service';
import { EventStatus } from '@models/licence-event.model';
import { License } from '@models/license.model';


export const UPLOAD_FILES_MODE = 'UploadFilesMode';
export const CRS_RENEWAL_LICENCE_TYPE_NAME = 'crs';
export const LIQUOR_RENEWAL_LICENCE_TYPE_NAME = 'liquor';


const ACTIVE = 'Active';
const PAYMENT_REQUIRED = 'Payment Required';
const RENEWAL_DUE = 'Renewal Due';

@Component({
  selector: 'app-licences',
  templateUrl: './licences.component.html',
  styleUrls: ['./licences.component.scss']
})
export class LicencesComponent extends FormBase implements OnInit {
  applications: ApplicationSummary[] = [];
  licenceForms = {};
  mainForm: FormGroup;
  eventStatus = EventStatus;

  readonly ACTIVE = ACTIVE;
  readonly PAYMENT_REQUIRED = PAYMENT_REQUIRED;
  readonly RENEWAL_DUE = RENEWAL_DUE;

  busy: Subscription;
  @Input() applicationInProgress: boolean;
  @Input() account: Account;
  @Output() marketerApplicationExists: EventEmitter<boolean> = new EventEmitter<boolean>();
  dataLoaded = false;
  ApplicationTypeNames = ApplicationTypeNames;
  licenceTransferFeatureOn = false;
  licenceMappings = {};
  supportedLicenceTypes = [
    "Catering", "Wine Store", "Cannabis Retail Store", "Marketing",
    "Operated - Wine Store", "Operated - Catering"
  ];

  constructor(
    private applicationDataService: ApplicationDataService,
    private licenceDataService: LicenseDataService,
    private router: Router,
    private store: Store<AppState>,
    private snackBar: MatSnackBar,
    private paymentService: PaymentDataService,
    private establishmentService: EstablishmentDataService,
    public featureFlagService: FeatureFlagService,
    private licenceEventsService: LicenceEventsService,
    public fb: FormBuilder) {
    super();
    featureFlagService.featureOn('LicenceTransfer')
      .pipe(takeWhile(() => this.componentActive))
      .subscribe((featureOn: boolean) => {
        this.licenceTransferFeatureOn = featureOn;
      });
    this.mainForm = new FormGroup({});
  }

  ngOnInit() {
    this.displayApplications();
  }

  showPlanStoreOpening(item: ApplicationLicenseSummary): boolean {
    let show = (
      item &&
      !item.storeInspected
      && (
        item.applicationTypeName === ApplicationTypeNames.CannabisRetailStore
        || item.applicationTypeName === ApplicationTypeNames.CRSLocationChange
      )
    );
    return show;
  }

  /**
   *
   * */
  private displayApplications() {
    this.busy =
      forkJoin(this.applicationDataService.getAllCurrentApplications(),
        this.licenceDataService.getAllCurrentLicenses(),
        this.licenceDataService.getAllOperatedLicenses()
      ).pipe(takeWhile(() => this.componentActive))
        .subscribe(([applications, licenses, operatedLicences]) => {
          this.applications = applications;
          operatedLicences.forEach(licence => {
            licence.isOperated = true;
            licence.licenceTypeName = 'Operated - ' + licence.licenceTypeName 
          });
          let combinedLicences = [...licenses, ...operatedLicences];
          combinedLicences.forEach((licence: ApplicationLicenseSummary) => {
            this.addOrUpdateLicence(licence);
          });
        });
  }

  uploadMoreFiles(application: Application) {
    this.router.navigate([`/application/${application.id}`, { mode: UPLOAD_FILES_MODE }]);
  }

  doAction(licence: ApplicationLicenseSummary, actionName: string) {
    const actionApplication = licence.actionApplications.find(app => app.applicationTypeName === actionName && app.applicationStatus !== 'Active');
    if (actionApplication && !actionApplication.isPaid) {
      this.router.navigateByUrl('/account-profile/' + actionApplication.applicationId);
    } else if (actionApplication && actionApplication.isPaid) {
      this.snackBar.open(`${actionName} has already been submitted and is under review`, 'Warning',
        { duration: 3500, panelClass: ['red-snackbar'] });
    } else {
      this.busy = this.licenceDataService.createApplicationForActionType(licence.licenseId, actionName)
        .pipe(takeWhile(() => this.componentActive))
        .subscribe(data => {
          this.router.navigateByUrl('/account-profile/' + data.id);
        },
          () => {
            this.snackBar.open(`Error running licence action for ${actionName}`, 'Fail',
              { duration: 3500, panelClass: ['red-snackbar'] });
          }
        );
    }
  }

  planStoreOpening(licence: ApplicationLicenseSummary) {
    const crsApplication = licence.actionApplications.find(app => app.applicationTypeName === ApplicationTypeNames.CannabisRetailStore);
    if (crsApplication) {
      this.router.navigate([`/store-opening/${crsApplication.applicationId}`]);
    } else {
      this.snackBar.open('Unable to find CRS Application', 'Fail',
        { duration: 3500, panelClass: ['red-snackbar'] });
    }
  }

  payLicenceFee(licence: ApplicationLicenseSummary) {
    //console.log("here");

    // locate the application associated with the issuance of this licence
    const crsApplication = licence.actionApplications.find(app => app.applicationTypeName === ApplicationTypeNames.CannabisRetailStore || app.applicationTypeName === ApplicationTypeNames.Catering);
    if (crsApplication) {
      this.busy = this.paymentService.getInvoiceFeePaymentSubmissionUrl(crsApplication.applicationId)
        .pipe(takeWhile(() => this.componentActive))
        .subscribe(res => {
          const data = <any>res;
          window.location.href = data.url;
        }, err => {
          if (err._body === 'Payment already made') {
            this.snackBar.open('Licence Fee payment has already been made.', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
          }
        });
    }
  }

  startRenewal(licence: ApplicationLicenseSummary) {
    const liquorLicenceTypes = ['Liquor Primary', 'Catering', 'Wine Store'];
    let renewalType = CRS_RENEWAL_LICENCE_TYPE_NAME;
    let renewalApplication = licence.actionApplications.find(app =>
      app.applicationTypeName === ApplicationTypeNames.CRSRenewal && app.applicationStatus !== 'Active');

    if (liquorLicenceTypes.indexOf(licence.licenceTypeName) !== -1) {
      renewalType = LIQUOR_RENEWAL_LICENCE_TYPE_NAME;
      renewalApplication = licence.actionApplications.find(app =>
        app.applicationTypeName === ApplicationTypeNames.LiquorRenewal && app.applicationStatus !== 'Active');
    }

    if (renewalApplication && !renewalApplication.isPaid) {
      this.router.navigateByUrl(`/renew-licence/${renewalType}/${renewalApplication.applicationId}`);
    } else if (renewalApplication && renewalApplication.isPaid) {
      this.snackBar.open('Renewal application already submitted', 'Fail',
        { duration: 3500, panelClass: ['red-snackbar'] });
    } else {
      let renewalApplicationTypeName = ApplicationTypeNames.CRSRenewal;
      if (renewalType === LIQUOR_RENEWAL_LICENCE_TYPE_NAME) {
        renewalApplicationTypeName = ApplicationTypeNames.LiquorRenewal;
      }
      this.busy = this.licenceDataService.createApplicationForActionType(licence.licenseId, renewalApplicationTypeName)
        .pipe(takeWhile(() => this.componentActive))
        .subscribe(data => {
          this.router.navigateByUrl(`/renew-licence/${renewalType}/${data.id}`);
        },
          () => {
            this.snackBar.open(`Error running licence action for ${renewalType}`, 'Fail',
              { duration: 3500, panelClass: ['red-snackbar'] });
            console.log('Error starting a Change Licence Location Application');
          }
        );
    }
  }

  isAboutToExpire(expiryDate: string) {
    const now = moment(new Date()).startOf('day');
    const expiry = moment(expiryDate).startOf('day');
    const diff = expiry.diff(now, 'days') + 1;
    return diff <= 60 || expiry < now;
  }

  licenceHasExpired(expiryDate: string) {
    const now = moment(new Date()).startOf('day');
    const expiry = moment(expiryDate).startOf('day');
    return expiry < now;
  }

  addOrUpdateLicence(licence: ApplicationLicenseSummary) {
    licence.actionApplications = [];
    const relatedApplications = this.applications.filter(l => l.licenceId === licence.licenseId);
    relatedApplications.forEach(app => {
      let action = {
        applicationId: app.id,
        applicationTypeName: app.applicationTypeName,
        applicationStatus: app.applicationStatus,
        isPaid: app.isPaid
      };
      licence.actionApplications.push(action);
    });
    if (licence.licenceTypeName === 'Catering' || licence.licenceTypeName === 'Wine Store') {
      forkJoin([
        this.licenceEventsService.getLicenceEventsList(licence.licenseId, 10)
      ])
        .subscribe(data => {
          licence.events = data[0];
        });
    }

    if (typeof this.licenceMappings[licence.licenceTypeName] === 'undefined') {
      this.licenceMappings[licence.licenceTypeName] = [];
    }
    const licenceIndex = this.licenceMappings[licence.licenceTypeName].findIndex(l => l.licenseId === licence.licenseId);
    if (licenceIndex >= 0) {
      this.licenceMappings[licence.licenceTypeName][licenceIndex] = licence;
    } else {
      this.licenceMappings[licence.licenceTypeName].push(licence);
    }
    this.licenceForms[licence.licenseId] = this.fb.group({
      phone: [licence.establishmentPhoneNumber],
      email: [licence.establishmentEmail]
    });
  }

  updateEmail(licenceId: string, establishmentId: string, event: any) {
    if (event.target.value === null) {
      return false;
    }

    const establishment = {
      id: establishmentId,
      email: event.target.value,
      phone: null,
      isOpen: null
    };

    const licence = Object.assign(new ApplicationLicenseSummary(), {
      licenseId: licenceId,
      establishmentEmail: event.target.value
    });

    this.updateEstablishment(establishment);
    this.updateLicence(licence);
  }

  updatePhone(licenceId: string, establishmentId: string, event: any) {
    if (event.target.value === null || typeof this.licenceForms[licenceId] === 'undefined') {
      return false;
    }

    const phone = this.licenceForms[licenceId].controls['phone'].value;

    const establishment = {
      id: establishmentId,
      email: null,
      phone: phone,
      isOpen: null
    };

    const licence = Object.assign(new ApplicationLicenseSummary(), {
      licenseId: licenceId,
      establishmentPhoneNumber: phone
    });

    this.updateEstablishment(establishment);
    this.updateLicence(licence);
  }

  updateLicence(licence: ApplicationLicenseSummary) {
    this.busy = forkJoin([
      this.licenceDataService.updateLicenceEstablishment(licence.licenseId, licence)
    ])
      .subscribe(([licenceResp]) => {
        this.addOrUpdateLicence(licenceResp);
      });
  }

  updateEstablishment(establishment: Establishment) {
    this.busy = this.establishmentService.upEstablishment(establishment).subscribe();
  }

  toggleStoreOpen(licenceType: string, index: number, establishmentId: string, isOpen: boolean) {
    const establishment = {
      id: establishmentId,
      isOpen: isOpen,
      phone: null,
      email: null
    };

    this.busy = forkJoin([
      this.establishmentService.upEstablishment(establishment)
    ])
      .subscribe(([establishmentResp]) => {
        this.licenceMappings[licenceType][index].establishmentIsOpen = establishmentResp.isOpen;
      });
  }

  getHandbookLink(licenceType: string) {
    switch (licenceType) {
      case 'Cannabis Retail Store':
        return 'https://www2.gov.bc.ca/assets/gov/employment-business-and-economic-development/business-management/liquor-regulation-licensing/guides-and-manuals/cannabis-retail-store-licence-handbook.pdf';
      case 'Marketing':
        return 'https://www2.gov.bc.ca/assets/gov/employment-business-and-economic-development/business-management/liquor-regulation-licensing/guides-and-manuals/marketing-handbook.pdf';
      case 'Catering':
        return 'https://www2.gov.bc.ca/assets/gov/employment-business-and-economic-development/business-management/liquor-regulation-licensing/guides-and-manuals/catering-handbook.pdf';
      case 'Wine Store':
        return 'https://www2.gov.bc.ca/assets/gov/employment-business-and-economic-development/business-management/liquor-regulation-licensing/guides-and-manuals/winestore-handbook.pdf';
      // added handling for operated wine stores
      // TODO: refactor Operated approach so that we don't have to add a case for each operator style
      case 'Operated - Wine Store':
          return 'https://www2.gov.bc.ca/assets/gov/employment-business-and-economic-development/business-management/liquor-regulation-licensing/guides-and-manuals/winestore-handbook.pdf';
      default:
        return '404';
    }
  }

  getNumberOfLicences() {
    return Object.keys(this.licenceMappings).length;
  }

  LicenceTypeSupported(licenceType: string) {
    const supported = this.supportedLicenceTypes.indexOf(licenceType) >= 0;
    return supported;
  }

  getOptionFromValue(options: any, value: number) {
    const idx = options.findIndex(opt => opt.value === value);
    if (idx >= 0) {
      return options[idx];
    }
    return {
      value: null,
      label: ''
    };
  }

  getSubCategory(subcategory: string) {
    let label = "";

    switch (subcategory) {
      case "GroceryStore":
        label = "Grocery Store";
        break;
      case "IndependentWineStore":
        label = "Independent Wine Store";
        break;
      case "OffSiteWineStore":
        label = "Off-Site Wine Store";
        break;
      case "OnSiteWineStore":
        label = "On-Site Wine Store";
        break;
      case "SacramentalWineStore":
        label = "Sacramental Wine Store";
        break;
      case "SpecialWineStore":
        label = "Special Wine Store";
        break;
      case "TouristWineStore":
        label = "Tourist Wine Store";
        break;
      case "WineOnShelf":
        label = "Wine on Shelf";
        break;
      case "BCVQA":
        label = "BC VQA Store";
        break;

      default:
        label = subcategory;
    }
    return label;
  }

  hasEndorsement(endorsement: string, licence: License) {
    return licence.endorsements.indexOf(endorsement) >= 0;
  }
}
