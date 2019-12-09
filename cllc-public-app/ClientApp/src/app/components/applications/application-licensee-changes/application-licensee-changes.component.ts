import { Component, OnInit, ViewChild, ChangeDetectorRef } from '@angular/core';
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
  changeTree: LicenseeChangeLog;
  individualShareholderChanges: LicenseeChangeLog[];
  organizationShareholderChanges: LicenseeChangeLog[];
  leadershipChanges: LicenseeChangeLog[];
  applicationId: string;
  application: Application;
  currentChangeLogs: LicenseeChangeLog[];
  currentLegalEntities: LegalEntity;
  @ViewChild(LicenseeTreeComponent, { static: false }) tree: LicenseeTreeComponent;

  editedTree: LicenseeChangeLog;
  LicenseeChangeLog = LicenseeChangeLog;
  busy: any;
  busySave: any;
  numberOfNonTerminatedApplications: number;
  cancelledLicenseeChanges: LicenseeChangeLog[] = [];

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
    this.route.paramMap.subscribe(pmap => this.applicationId = pmap.get('applicationId'));
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
      });

    this.loadData();
  }

  loadData() {
    this.GetNotTerminatedCRSApplicationCount();

    this.busy = forkJoin(this.applicationDataService.getApplicationById(this.applicationId),
      this.legalEntityDataService.getChangeLogs(this.applicationId),
      this.legalEntityDataService.getCurrentHierachy())
      .pipe(takeWhile(() => this.componentActive))
      .subscribe((data: [Application, LicenseeChangeLog[], LegalEntity]) => {
        this.application = data[0];
        const currentChangeLogs = data[1] || [];
        const currentLegalEntities = data[2];
        const tree = LicenseeChangeLog.processLegalEntityTree(currentLegalEntities);
        tree.isRoot = true;
        tree.applySavedChangeLogs(currentChangeLogs);
        this.changeTree = tree;

        this.addDynamicContent();
        this.form.patchValue(this.application);
      },
        () => {
          console.log('Error occured');
        }
      );
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

  /**
   * Sends data to dynamics
   */
  save() {
    const data = this.cleanSaveData(this.changeTree);
    this.busySave = forkJoin(
      this.applicationDataService.updateApplication({ ...this.application, ...this.form.value }),
      this.legalEntityDataService.saveLicenseeChanges(data, this.applicationId),
      this.legalEntityDataService.cancelLicenseeChanges(this.cancelledLicenseeChanges))
      .subscribe(() => {
        this.snackBar.open('Application has been saved', 'Success', { duration: 2500, panelClass: ['green-snackbar'] });
        this.loadData();
      });
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
    node.parentLinceseeChangeLog = undefined;
    if (node.children && node.children.length) {
      node.children.forEach(child => this.removeParentReferences(child))
    }
  }
}

