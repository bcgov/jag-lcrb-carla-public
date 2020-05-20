import { ChangeDetectorRef, Component, EventEmitter, Input, OnInit, Output, ViewChild, ModuleWithProviders } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { MatDialog, MatSnackBar } from '@angular/material';
import { ActivatedRoute, Router } from '@angular/router';
import { AppState } from '@app/app-state/models/app-state';
import { Account } from '@models/account.model';
import { ApplicationLicenseSummary } from '@models/application-license-summary.model';
import { ApplicationSummary } from '@models/application-summary.model';
import { ApplicationType, ApplicationTypeNames } from '@models/application-type.model';
import { Application } from '@models/application.model';
import { LegalEntity } from '@models/legal-entity.model';
import { LicenseeChangeLog } from '@models/licensee-change-log.model';
import { Store } from '@ngrx/store';
import { ApplicationDataService } from '@services/application-data.service';
import { FeatureFlagService } from '@services/feature-flag.service';
import { LegalEntityDataService } from '@services/legal-entity-data.service';
import { LicenseDataService } from '@services/license-data.service';
import { PaymentDataService } from '@services/payment-data.service';
import { OrgStructureComponent } from '@shared/components/org-structure/org-structure.component';
import { FormBase } from '@shared/form-base';
import { forkJoin, Observable, of, Subject } from 'rxjs';
import { filter, mergeMap, switchMap, takeWhile } from 'rxjs/operators';
import { AppRemoveIfFeatureOnDirective } from '../../../directives/remove-if-feature-on.directive';

@Component({
  selector: 'app-application-licensee-changes',
  templateUrl: './application-licensee-changes.component.html',
  styleUrls: ['./application-licensee-changes.component.scss']
})
export class ApplicationLicenseeChangesComponent extends FormBase implements OnInit {
  account: Account;
  treeRoot: LicenseeChangeLog;
  individualShareholderChanges: LicenseeChangeLog[];
  organizationShareholderChanges: LicenseeChangeLog[];
  leadershipChanges: LicenseeChangeLog[];
  applicationId: string;
  application: Application;
  currentChangeLogs: LicenseeChangeLog[];
  currentLegalEntities: LegalEntity;
  @ViewChild('orgStructure', { static: false }) orgStructure: OrgStructureComponent;
  @Output() saveComplete: EventEmitter<boolean> = new EventEmitter<boolean>();
  @Input() redirectToDashboardOnSave = true;

  editedTree: LicenseeChangeLog;
  LicenseeChangeLog = LicenseeChangeLog;
  busy: any;
  busySave: any;
  numberOfNonTerminatedApplications: number;
  cancelledLicenseeChanges: LicenseeChangeLog[] = [];
  validationErrors: string[];
  thereIsExistingOrgStructure: boolean;
  busyPromise: Promise<any>;
  licenses: ApplicationLicenseSummary[];
  licencesOnFile: boolean;
  securityScreeningEnabled: boolean;
  licenseeApplicationLoaded: boolean;
  loadedValue: LicenseeChangeLog;

  constructor(public dialog: MatDialog,
    public snackBar: MatSnackBar,
    private fb: FormBuilder,
    public cd: ChangeDetectorRef,
    public router: Router,
    private store: Store<AppState>,
    private route: ActivatedRoute,
    private licenseService: LicenseDataService,
    private paymentDataService: PaymentDataService,
    private applicationDataService: ApplicationDataService,
    private legalEntityDataService: LegalEntityDataService,
    private featureFlagService: FeatureFlagService) {
    super();

    featureFlagService.featureOn('SecurityScreening')
      .subscribe(featureOn => this.securityScreeningEnabled = featureOn);

    this.busy = this.licenseService.getAllCurrentLicenses()
      .subscribe(data => {
        this.licenses = data;
        this.licencesOnFile = (this.licenses && this.licenses.length > 0);
      });
  }


  ngOnInit() {
    this.form = this.fb.group({
      id: [''],
      contactPersonFirstName: ['', Validators.required],
      contactPersonLastName: ['', Validators.required],
      contactPersonRole: [''],
      amalgamationDone: [''],
      contactPersonEmail: ['', Validators.required],
      contactPersonPhone: ['', Validators.required],
      authorizedToSubmit: ['', [this.customRequiredCheckboxValidator()]],
      signatureAgreement: ['', [this.customRequiredCheckboxValidator()]],
    });


    this.store.select(state => state.currentAccountState.currentAccount)
      .pipe(takeWhile(() => this.componentActive))
      .pipe(filter(account => !!account))
      .subscribe((account) => {
        this.account = account;
        if (!this.licenseeApplicationLoaded) {
          this.loadLicenseeApplication()
            .pipe(filter(id => !!id))
            .subscribe(id => {
              this.applicationId = id;
              this.loadData();
            });
        }
        this.licenseeApplicationLoaded = true;
      });
  }

  loadData() {
    this.GetNotTerminatedCRSApplicationCount();

    this.busyPromise = forkJoin(this.applicationDataService.getApplicationById(this.applicationId),
      this.legalEntityDataService.getApplicationChangeLogs(this.applicationId),
      this.legalEntityDataService.getCurrentHierachy())
      .pipe(takeWhile(() => this.componentActive))
      .toPromise()
      .then((data: [Application, LicenseeChangeLog[], LegalEntity]) => {
        this.application = data[0];
        const currentChangeLogs = data[1] || [];
        this.currentLegalEntities = data[2];
        this.thereIsExistingOrgStructure = this.currentLegalEntities.children.length > 0;
        this.treeRoot = LicenseeChangeLog.processLegalEntityTree(this.currentLegalEntities);
        this.treeRoot.isRoot = true;
        this.treeRoot.applySavedChangeLogs(currentChangeLogs);

        this.loadedValue = this.cleanSaveData(this.treeRoot);

        this.addDynamicContent();
        this.form.patchValue(this.application);
      });
  }

  /**
   * Gets licensee application id. Create the application and return id if it does not exist
   */
  loadLicenseeApplication() {
    return this.applicationDataService.getOngoingLicenseeChangeApplicationId()
      .pipe(mergeMap(id => {
        if (id) {
          return of(id);
        } else { // create licensee application and return its id
          const newLicenceApplicationData = <Application>{
            applicantType: this.account.businessType,
            applicationType: <ApplicationType>{ name: ApplicationTypeNames.LicenseeChanges },
            account: this.account,
          };
          return this.applicationDataService.createApplication(newLicenceApplicationData)
            .pipe(switchMap(app => of(app.id)));
        }
      }));
  }

  getSaveLabel(): string {
    let label = 'Continue to Application';

    // if No Organizational Information on File  OR changes made
    if (!this.thereIsExistingOrgStructure || (this.treeRoot && LicenseeChangeLog.HasChanges(this.treeRoot))) {
      label = 'Submit Organization Information';
    }
    // if Organization Information on File  AND no changes
    else if (this.thereIsExistingOrgStructure && this.treeRoot && !LicenseeChangeLog.HasChanges(this.treeRoot)) {
      label = 'Confirm Organization Information Is Complete';
    }    
    return label.toUpperCase();
  }

  disableSaveLabel(): boolean {
    let disable = false;
    const errors = this.validateNonIndividauls(this.treeRoot);
    if (errors.length > 0) {
      disable = true;
    }
    return disable;
  }

  /**
   * Gets the number of applications owned by the current user that are not terminated
   */
  private GetNotTerminatedCRSApplicationCount() {
    this.busy =
      this.applicationDataService.getAllCurrentApplications()
        .pipe(takeWhile(() => this.componentActive))
        .subscribe((applications: ApplicationSummary[]) => {
          // filter out approved applications
          const notTerminatedApplications =
            applications.filter(app => {
              const noneTerminatedCRSApplications: boolean = ['Terminated and refunded'].indexOf(app.applicationStatus) === -1
                && app.applicationTypeName === ApplicationTypeNames.CannabisRetailStore;
              return noneTerminatedCRSApplications;
            });
          this.numberOfNonTerminatedApplications = notTerminatedApplications.length;
        });
  }


  validateFormData() {
    let errors = [];
    let fileErrors = [];

    // Only check file validation errors if changes were made
    if (!this.thereIsExistingOrgStructure || (this.treeRoot && LicenseeChangeLog.HasChanges(this.treeRoot))) {
      fileErrors = this.validateFileUploads(this.treeRoot);
    }
    if (this.treeRoot) {
      errors = [
        ...this.validateNonIndividauls(this.treeRoot),
        ...fileErrors
      ]
    }
    return errors;
  }

  validateFileUploads(node: LicenseeChangeLog): string[] {
    let errors = [];
    node = Object.assign(new LicenseeChangeLog(), node);

    errors = errors.concat(node.getFileUploadValidationErrors());
    debugger;
    node.children = node.children || [];
    node.children.forEach(child => {
      errors = errors.concat(this.validateFileUploads(child));
    });

    return errors;
  }


  validateNonIndividauls(node: LicenseeChangeLog): string[] {
    node = Object.assign(new LicenseeChangeLog(), node);
    let validationMessages = [];
    if (!node.isRemoveChangeType()) {
      validationMessages = LicenseeChangeLog.ValidateNonIndividaul(node);
      node.children = node.children || [];
      node.children.forEach((child: LicenseeChangeLog) => {
        validationMessages = validationMessages.concat(this.validateNonIndividauls(child));
      });
    }
    return validationMessages;
  }

  /**
   * Sends data to dynamics
   */
  save() {
    this.orgStructure.saveAll()
      .subscribe(result => {
        this.validationErrors = this.validateFormData();
        if (!result) {
          this.validationErrors = ['There are incomplete fields on the page', ...this.validationErrors];
        }
        if (this.validationErrors.length === 0) {
          // set value to cause invoice generationP
          this.busyPromise = this.prepareSaveRequest({ invoicetrigger: 1 })
            .pipe(mergeMap(results => {
              console.log(results);
              const saveOverrideValue = { invoicetrigger: 1 };
              return this.applicationDataService.updateApplication({ ...this.application, ...this.form.value, ...saveOverrideValue })
                .pipe(takeWhile(() => this.componentActive))
                .toPromise()
                .then(app => {
                  // payment is required
                  if (app && app.adoxioInvoiceId) {
                    this.submitPayment();
                  } else if (app) { // go to the application page
                    this.loadedValue = this.cleanSaveData(this.treeRoot); // update the loaded value after a succesfull save
                    this.saveComplete.emit(true);
                    if (this.redirectToDashboardOnSave) {
                      this.router.navigateByUrl('/dashboard');
                    }
                  }
                  return of(app);
                });
            }))
            .toPromise()
            .then(() => {
              this.snackBar.open('Application has been saved', 'Success', { duration: 2500, panelClass: ['green-snackbar'] });

            })
            .catch(() => {
              this.snackBar.open('Error saving application', 'Error', { duration: 2500, panelClass: ['red-snackbar'] });
            });
        }
      });
  }

  /**
 * Redirect to payment processing page (Express Pay / Bambora service)
 * */
  private submitPayment() {
    this.busy = this.paymentDataService.getPaymentSubmissionUrl(this.applicationId)
      .pipe(takeWhile(() => this.componentActive))
      .subscribe(jsonUrl => {
        window.location.href = jsonUrl['url'];
        return jsonUrl['url'];
      }, err => {
        if (err._body === 'Payment already made') {
          this.snackBar.open('Application payment has already been made.', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
        }
      });
  }

  private prepareSaveRequest(saveOverrideValue: Partial<Application>) {
    this.validationErrors = [];

    saveOverrideValue = saveOverrideValue || {};
    const data = this.cleanSaveData(this.treeRoot);
    
    return forkJoin(this.legalEntityDataService.updateLegalEntity({ ...this.currentLegalEntities, numberOfMembers: this.treeRoot.numberOfMembers, annualMembershipFee: this.treeRoot.annualMembershipFee }, this.currentLegalEntities.id),
      this.legalEntityDataService.saveLicenseeChanges(data, this.applicationId));
  }

  saveForLater(navigateAfterSaving: boolean = true): Observable<boolean> {
    let subject  = new Subject<boolean>();
    this.orgStructure.saveAll()
      .subscribe(result => {
        this.validationErrors = [];
        if (!result) {
          this.validationErrors = ['There are incomplete fields on the page'];
          subject.next(false);
        }
        if (this.validationErrors.length === 0) {
          this.busyPromise = this.prepareSaveRequest({})
            .toPromise()
            .then(() => {
              this.snackBar.open('Application has been saved', 'Success', { duration: 2500, panelClass: ['green-snackbar'] });
              if(navigateAfterSaving){
                this.router.navigateByUrl('/dashboard');
              }
              subject.next(true);
            });
        }
      });
      return subject;
  }

  addCancelledChange(change: LicenseeChangeLog) {
    this.cancelledLicenseeChanges.push(change);
  }

  cancelApplication() {
  }

  canDeactivate(): Observable<boolean> {
    const data = this.cleanSaveData(this.treeRoot);
    if(JSON.stringify(data) === JSON.stringify(this.loadedValue)){ //no change made. Skip save
      return of(true);
    }
    return this.saveForLater(false);
  }

  /**
   * Returns true if there is an ongoing or approved (but not terminated)
   * CRS application
   */
  aNonTerminatedCrsApplicationExistOnAccount(): boolean {
    return this.numberOfNonTerminatedApplications > 0;
  }

  cleanSaveData(data: LicenseeChangeLog): LicenseeChangeLog {
    const result = { ...data } as LicenseeChangeLog;
    this.removeParentReferences(result);
    return result;
  }

  removeParentReferences(node: LicenseeChangeLog) {
    //Form the parent account relationship
    if (node.parentLinceseeChangeLog && node.parentLinceseeChangeLog.businessAccountId) {
      node.parentBusinessAccountId = node.parentLinceseeChangeLog.businessAccountId;
    }
    // remove parent reference
    node.parentLinceseeChangeLog = undefined;
    node.refObject = undefined;

    if (node.children && node.children.length) {
      node.children.forEach(child => {
        this.removeParentReferences(child)
      });
    }
  }
}
