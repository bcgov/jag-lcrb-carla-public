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
import { FormBuilder, FormGroup, FormArray } from '@angular/forms';
import { Establishment } from '@models/establishment.model';
import { LicenceEventsService } from '@services/licence-events.service';
import { EventStatus } from '@models/licence-event.model';
import { License } from '@models/license.model';


export const UPLOAD_FILES_MODE = 'UploadFilesMode';
export const CRS_RENEWAL_LICENCE_TYPE_NAME = 'crs';
export const LIQUOR_RENEWAL_LICENCE_TYPE_NAME = 'liquor';


const ACTIVE = 'Active';
// const PAYMENT_REQUIRED = 'Payment Required';
const RENEWAL_DUE = 'Renewal Due';

@Component({
  selector: 'app-licence-row',
  templateUrl: './licence-row.component.html',
  styleUrls: ['./licence-row.component.scss']
})
export class LicenceRowComponent extends FormBase implements OnInit {
    licenceTransferFeatureOn = false;
    mainForm: FormGroup;
    busy: Subscription;
    licenceForms = {};
    eventStatus = EventStatus;

    @Input() available: boolean;
    @Input() licenceType: string;
    @Input() licences: ApplicationLicenseSummary[];
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
        this.licences.forEach((licence) => {
            this.licenceForms[licence.licenseId] = this.fb.group({
                phone: [licence.establishmentPhoneNumber],
                email: [licence.establishmentEmail]
              });
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
            this.updateLicenceEstablishment(licenceResp);
        });
    }

    updateLicenceEstablishment(licence: ApplicationLicenseSummary) {
        const licenceIndex = this.licences.findIndex(l => l.licenseId === licence.licenseId);
        if (licenceIndex >= 0) {
          this.licences[licenceIndex] = licence;
        }
    }

    updateEstablishment(establishment: Establishment) {
        this.busy = this.establishmentService.upEstablishment(establishment).subscribe();
    }

    isExpired(licence: ApplicationLicenseSummary) {
        return licence.status === 'Expired';
    }

    actionsVisible(licence: ApplicationLicenseSummary) {
        let retVal = true;
        if (licence.transferRequested && this.licenceTransferFeatureOn) {
            if (licence.licenceTypeCategory === 'Cannabis' && licence.isDeemed) {
                retVal = false;
            } else if (licence.licenceTypeCategory === 'Liquor' && !licence.isDeemed) {
                retVal = false;
            }
        }
        return retVal;
    }

    payLicenceFee(licence: ApplicationLicenseSummary) {
        // locate the application associated with the issuance of this licence
        const application = licence.actionApplications.find(app => app.applicationTypeName === licence.licenceTypeName);
        if (application) {
          this.busy = this.paymentService.getInvoiceFeePaymentSubmissionUrl(application.applicationId)
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

    planStoreOpening(licence: ApplicationLicenseSummary) {
        const application = licence.actionApplications.find(app => app.applicationTypeName === licence.applicationTypeName);
        if (application) {
          this.router.navigate([`/store-opening/${application.applicationId}`]);
        } else {
          this.snackBar.open('Unable to find Application', 'Fail',
            { duration: 3500, panelClass: ['red-snackbar'] });
        }
    }

    toggleStoreOpen(index: number, establishmentId: string, isOpen: boolean) {
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
            this.licences[index].establishmentIsOpen = establishmentResp.isOpen;
          });
    }

    isAboutToExpire(expiryDate: string) {
        const now = moment(new Date()).startOf('day');
        const expiry = moment(expiryDate).startOf('day');
        const diff = expiry.diff(now, 'days') + 1;

        return diff <= 60 || expiry < now;
    }

    isRecentlyExpired(licence: ApplicationLicenseSummary) {
        const now = moment(new Date()).startOf('day');
        const expiry = moment(licence.expiryDate).startOf('day');
        const diff = now.diff(expiry, 'days') + 1;
        return licence.status === 'Expired' && diff <= 30;
    }

    isActive(licence: ApplicationLicenseSummary) {
        let active = licence.status === 'Active';
        if (licence.dormant || licence.suspended) {
            active = false;
        }
        return active;
    }

    isActiveOrRecentlyExpired(licence: ApplicationLicenseSummary) {
        return this.isRecentlyExpired(licence) || this.isActive(licence);
    }

    doAction(licence: ApplicationLicenseSummary, actionName: string) {
        const actionApplication = licence.actionApplications.find(
            app => app.applicationTypeName === actionName && app.applicationStatus !== 'Active');
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

    startRenewal(licence: ApplicationLicenseSummary) {
        let renewalType = CRS_RENEWAL_LICENCE_TYPE_NAME;
        let renewalApplication = licence.actionApplications.find(app =>
          app.applicationTypeName === ApplicationTypeNames.CRSRenewal && app.applicationStatus !== 'Active');

        if (licence.licenceTypeCategory === 'Liquor') {
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

    hasEndorsement(endorsement: string, licence: License) {
        return licence.endorsements.indexOf(endorsement) >= 0;
    }

    getHandbookLink(licenceType: string) {
        switch (licenceType) {
          case 'Cannabis Retail Store':
            return 'https://www2.gov.bc.ca/assets/gov/employment-business-and-economic-development/business-management/liquor-regulation-licensing/guides-and-manuals/cannabis-retail-store-licence-handbook.pdf';
          case 'Marketing':
            return 'https://www2.gov.bc.ca/assets/gov/employment-business-and-economic-development/business-management/liquor-regulation-licensing/guides-and-manuals/marketing-handbook.pdf';
          case 'Operated - Catering':
          case 'Catering':
          case 'Transfer in Progress - Catering':
            return 'https://www2.gov.bc.ca/assets/gov/employment-business-and-economic-development/business-management/liquor-regulation-licensing/guides-and-manuals/catering-handbook.pdf';
          case 'Wine Store':
          case 'Transfer in Progress - Wine Store':
          case 'Operated - Wine Store':
              return 'https://www2.gov.bc.ca/assets/gov/employment-business-and-economic-development/business-management/liquor-regulation-licensing/guides-and-manuals/winestore-handbook.pdf';
          default:
            return '404';
        }
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

    getLicenceStatusText(status: string) {
        switch (status) {
            case 'PreInspection':
                return 'Pre-Inspection';
            case 'PendingLicenceFee':
                return 'Pending Licence Fee';
            default:
                return status;
        }
    }
}
