import { Component, OnInit, ViewChild, ChangeDetectorRef, Input, Output, EventEmitter } from '@angular/core';
import { FormBase, CanadaPostalRegex } from '@shared/form-base';
import { NestedTreeControl } from '@angular/cdk/tree';
import { LicenseeChangeLog, LicenseeChangeType } from '@models/licensee-change-log.model';
import { MatTreeNestedDataSource, MatTree, MatDialog, MatSnackBar } from '@angular/material';
import { Application } from '@models/application.model';
import { ActivatedRoute, Router } from '@angular/router';
import { ApplicationDataService } from '@services/application-data.service';
import { LegalEntityDataService } from '@services/legal-entity-data.service';
import { forkJoin } from 'rxjs';
import { takeWhile, filter } from 'rxjs/operators';
import { LegalEntity } from '@models/legal-entity.model';
import { FormBuilder, Validators } from '@angular/forms';
import { Store } from '@ngrx/store';
import { AppState } from '@app/app-state/models/app-state';
import { Account } from '@models/account.model';
import { LicenseeTreeComponent } from '@shared/components/licensee-tree/licensee-tree.component';
import { ApplicationSummary } from '@models/application-summary.model';
import { ApplicationTypeNames } from '@models/application-type.model';

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
  @Input() applicationId: string;
  application: Application;
  currentChangeLogs: LicenseeChangeLog[];
  currentLegalEntities: LegalEntity;
  @ViewChild(LicenseeTreeComponent, { static: false }) tree: LicenseeTreeComponent;
  @Output() saveComplete: EventEmitter<boolean> = new EventEmitter<boolean>();

  editedTree: LicenseeChangeLog;
  LicenseeChangeLog = LicenseeChangeLog;
  busy: any;
  busySave: any;
  numberOfNonTerminatedApplications: number;
  cancelledLicenseeChanges: LicenseeChangeLog[] = [];
  validationErrors: string[];
  thereIsExistingOrgStructure: boolean;
  busyPromise: Promise<any>;

  constructor(public dialog: MatDialog,
    public snackBar: MatSnackBar,
    private fb: FormBuilder,
    public cd: ChangeDetectorRef,
    public router: Router,
    private store: Store<AppState>,
    private route: ActivatedRoute,
    private applicationDataService: ApplicationDataService,
    private legalEntityDataService: LegalEntityDataService) {
    super();
    // this.route.paramMap.subscribe(pmap => this.applicationId = pmap.get('licenseeChangeAppId'));

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

    this.store.select(state => state.onGoingLicenseeChangesApplicationIdState.onGoingLicenseeChangesApplicationId)
    .pipe(takeWhile(() => this.componentActive))
    .pipe(filter(id => !!id))
    .subscribe(id => {
      this.applicationId = id;
      this.loadData();
    });
    
    this.store.select(state => state.currentAccountState.currentAccount)
    .pipe(takeWhile(() => this.componentActive))
    .pipe(filter(account => !!account))
      .subscribe((account) => {
        this.account = account;
      });

    // this.loadData();
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
        const currentLegalEntities = data[2];
        this.thereIsExistingOrgStructure = currentLegalEntities.children.length > 0;
        this.treeRoot = LicenseeChangeLog.processLegalEntityTree(currentLegalEntities);
        this.treeRoot.isRoot = true;
        this.treeRoot.applySavedChangeLogs(currentChangeLogs);

        this.addDynamicContent();
        this.form.patchValue(this.application);
      }  );
  }

  getSaveLabel(): string {
    let label = 'Save';
    if (!this.thereIsExistingOrgStructure) {
      label = 'Submit Org Structure';
    }

    if (LicenseeChangeLog.HasChanges(this.treeRoot)) {
      label = 'Save Changes to Org Structure';
    }

    if (!this.thereIsExistingOrgStructure && !LicenseeChangeLog.HasChanges(this.treeRoot)) {
      label = 'Confirm No Changes to Current Org Structure';
    }

    return label;
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
              let noneTerminatedCRSApplications: boolean = ['Terminated and refunded'].indexOf(app.applicationStatus) === -1
                && app.applicationTypeName === ApplicationTypeNames.CannabisRetailStore;
              return noneTerminatedCRSApplications;
            });
          this.numberOfNonTerminatedApplications = notTerminatedApplications.length;
        });
  }

  validateNonIndividauls() {
    this.validationErrors = this.validateRecursive(this.treeRoot);
  }

  validateRecursive(node: LicenseeChangeLog): string[] {
    node = Object.assign(new LicenseeChangeLog, node);
    let validationMessages = [];
    if (!node.isRemoveChangeType()) {
      validationMessages = LicenseeChangeLog.ValidateNonIndividaul(node);
      node.children = node.children || [];
      node.children.forEach((child: LicenseeChangeLog) => {
        validationMessages = validationMessages.concat(this.validateRecursive(child));
      });
    }
    return validationMessages;
  }

  /**
   * Sends data to dynamics
   */
  save() {
    this.validateNonIndividauls();
    if (this.validationErrors.length === 0) {
      const data = this.cleanSaveData(this.treeRoot);
      this.busySave = forkJoin(
        this.applicationDataService.updateApplication({ ...this.application, ...this.form.value, isApplicationComplete: 'Yes' }),
        this.legalEntityDataService.saveLicenseeChanges(data, this.applicationId),
        this.legalEntityDataService.cancelLicenseeChanges(this.cancelledLicenseeChanges))
        .subscribe(() => {
          this.snackBar.open('Application has been saved', 'Success', { duration: 2500, panelClass: ['green-snackbar'] });
          this.saveComplete.emit(true);
          this.loadData();
        });
    }
  }

  addCancelledChange(change: LicenseeChangeLog) {
    this.cancelledLicenseeChanges.push(change);
  }

  cancelApplication() {
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
      })
    }
  }
}

