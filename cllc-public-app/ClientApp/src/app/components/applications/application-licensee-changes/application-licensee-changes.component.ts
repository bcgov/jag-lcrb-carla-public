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

  LicenseeChangeLog = LicenseeChangeLog;
  editedTree: LicenseeChangeLog;
  busy: any;
  busySave: any;
  numberOfNonTerminatedApplications: number;
  cancelledLicenseeChanges: LicenseeChangeLog[] = [];
  validationErrors: string[];
  thereIsExistingOrgStructure: boolean;
  busyPromise: Promise<any>;
  licenses: ApplicationLicenseSummary[];
  numberOfCannabisLicences: number;
  numberOfLiquorLicences: number;
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

    this.loadData('on-going');

    this.store.select(state => state.currentAccountState.currentAccount)
      .pipe(takeWhile(() => this.componentActive))
      .pipe(filter(account => !!account))
      .subscribe((account) => {
        this.account = account;

      });
  }

  loadData(type: 'on-going' | 'create') {
    this.busy = this.applicationDataService.getOngoingLicenseeData(type)
      .subscribe(data => {

        this.application = data.application;
        this.applicationId = this.application.id;

        this.form.patchValue(this.application);

        const currentChangeLogs: LicenseeChangeLog[] = LicenseeChangeLog.FixLicenseeChangeLogArray(data.changeLogs || []);

        this.licenses = data.licenses || [];
        // to do: cannabis licences & liquor licences
        this.licencesOnFile = this.licenses.length > 0;

        this.numberOfCannabisLicences = this.licenses.filter(lic => lic.licenceTypeCategory == "Cannabis" && lic.status == 'Active').length;
        this.numberOfLiquorLicences = this.licenses.filter(lic => lic.licenceTypeCategory == "Liquor" && lic.status == 'Active').length;


        this.currentLegalEntities = data.currentHierarchy;
        this.numberOfNonTerminatedApplications = data.nonTerminatedApplications;
        this.thereIsExistingOrgStructure = this.currentLegalEntities && this.currentLegalEntities.children && this.currentLegalEntities.children.length > 0;

        this.treeRoot = new LicenseeChangeLog(data.treeRoot);
        // apply some formatting to the treeRoot.

        this.treeRoot.fileUploads = {}; // This is only used on the client side

        this.treeRoot.isRoot = true;
        this.treeRoot.fixChildren();

        this.treeRoot.applySavedChangeLogs(currentChangeLogs);

        this.loadedValue = this.cleanSaveData(this.treeRoot);


        this.addDynamicContent();

        this.licenseeApplicationLoaded = true;
      });
  }



  getSaveLabel(): string {
    let label = 'Continue to Application';

    // if No Organizational Information on File  OR changes made
    if (!this.thereIsExistingOrgStructure || LicenseeChangeLog.HasChanges(this.orgStructure.getData())) {
      label = 'Submit Organization Information';
    }
    // if Organization Information on File  AND no changes
    else if (this.thereIsExistingOrgStructure && !LicenseeChangeLog.HasChanges(this.orgStructure.getData())) {
      label = 'Confirm Organization Information Is Complete';
    }
    return label.toUpperCase();
  }

  disableSaveLabel(): boolean {
    let disable = false;
    const errors = this.validateNonIndividauls(this.orgStructure.getData());
    if (errors.length > 0) {
      disable = true;
    }
    return disable;
  }


  validateFormData() {
    let errors = [];
    let fileErrors = [];

    let data = this.orgStructure.getData();

    // Only check file validation errors if changes were made
    if (!this.thereIsExistingOrgStructure || (data && LicenseeChangeLog.HasChanges(data))) {
      fileErrors = this.validateFileUploads(data);
    }
    if (data) {
      errors = [
        ...this.validateNonIndividauls(data),
        ...fileErrors
      ]
    }
    return errors;
  }

  validateFileUploads(node: LicenseeChangeLog): string[] {
    let errors = [];
    node = Object.assign(new LicenseeChangeLog(), node);

    errors = errors.concat(node.getFileUploadValidationErrors());

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
  cancel() {

    // delete the application.
    this.busy = this.applicationDataService.cancelApplication(this.applicationId)
      .pipe(takeWhile(() => this.componentActive))
      .toPromise()
      .then(app => {
        this.snackBar.open('Licensee Changes has been cancelled', 'Success', { duration: 2500, panelClass: ['green-snackbar'] });
        this.router.navigateByUrl('/dashboard');
        return of(app);
      })
      .catch(() => {
        this.snackBar.open('Error cancelling Licensee Changes', 'Error', { duration: 2500, panelClass: ['red-snackbar'] });
      });

  }

  saveReadOnly(){
    this.saveComplete.emit(true);
    if (this.redirectToDashboardOnSave) {
      this.router.navigateByUrl('/dashboard');
    }
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

        let noChanges = (this.thereIsExistingOrgStructure && !LicenseeChangeLog.HasChanges(this.orgStructure.getData()));

        if (this.validationErrors.length === 0 || noChanges) {
          // set value to cause invoice generationP
          this.busyPromise = this.prepareSaveRequest()

            .pipe(mergeMap(results => {
              const saveOverrideValue = { invoiceTrigger: 1 };

              return this.applicationDataService.updateApplication({ ...this.application, ...this.form.value, ...saveOverrideValue })
                .pipe(takeWhile(() => this.componentActive))
                .toPromise()
                .then(app => {
                  // payment is required
                  if (app && app.adoxioInvoiceId) {
                    this.submitPayment();
                  } else if (app) { // go to the application page
                    // mark application as complete
                    let saveData = { ...this.application, ...this.form.value };
                    if (LicenseeChangeLog.HasChanges(this.orgStructure.getData())) {
                      saveData.isApplicationComplete = 'Yes';
                    }

                    this.applicationDataService.updateApplication(saveData)
                      .subscribe(res => {
                        this.loadedValue = this.cleanSaveData(this.orgStructure.getData());  // Update loadedValue to prevent double saving
                        this.saveComplete.emit(true);
                        if (this.redirectToDashboardOnSave) {
                          this.router.navigateByUrl('/dashboard');
                        }
                      });
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

  private prepareSaveRequest() {
    this.validationErrors = [];

    const data = this.cleanSaveData(this.orgStructure.getData());
    return this.legalEntityDataService.updateLegalEntity({ ...this.currentLegalEntities, numberOfMembers: data.numberOfMembers, annualMembershipFee: data.annualMembershipFee }, this.currentLegalEntities.id)
      .pipe(mergeMap(result => {
        // do something with result
        return this.legalEntityDataService.saveLicenseeChanges(data, this.applicationId);
      }));
  }

  saveForLater(navigateAfterSaving: boolean = true): Observable<boolean> {
    let subject = new Subject<boolean>();
    this.orgStructure.saveAll()
      .subscribe(result => {
        this.validationErrors = [];
        if (!result) {
          this.validationErrors = ['There are incomplete fields on the page'];
          subject.next(false);
        }
        if (this.validationErrors.length === 0) {
          this.busyPromise = this.prepareSaveRequest()
            .toPromise()
            .then(() => {
              this.snackBar.open('Application has been saved', 'Success', { duration: 2500, panelClass: ['green-snackbar'] });
              this.loadedValue = this.cleanSaveData(this.orgStructure.getData());  // Update loadedValue to prevent double saving
              if (navigateAfterSaving) {
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
    const data = this.cleanSaveData(this.orgStructure.getData());
    if (JSON.stringify(data) === JSON.stringify(this.loadedValue)) { //no change made. Skip save
      return of(true);
    }
    return this.saveForLater(false);
  }
  
  calculateSubTotal(can, liq): string {

      const cannabis = can * this.numberOfCannabisLicences;
      const liquor = liq * this.numberOfLiquorLicences;
      const total = cannabis + liquor;
      return total.toString();
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
    if (node.parentLicenseeChangeLog && node.parentLicenseeChangeLog.businessAccountId) {
      node.parentBusinessAccountId = node.parentLicenseeChangeLog.businessAccountId;
    }
    // remove parent reference
    node.parentLicenseeChangeLog = undefined;

    if (node.children && node.children.length) {
      node.children.forEach(child => {
        this.removeParentReferences(child)
      });
    }
  }
}
