import { Component, OnInit, ViewChild, ChangeDetectorRef } from '@angular/core';
import { FormBase, CanadaPostalRegex } from '@shared/form-base';
import { NestedTreeControl } from '@angular/cdk/tree';
import { LicenseeChangeLog, LicenseeChangeType } from '@models/legal-entity-change.model';
import { MatTreeNestedDataSource, MatTree, MatDialog } from '@angular/material';
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
  numberOfNonTerminatedApplications: number;

  constructor(public dialog: MatDialog,
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



    this.busy = this.applicationDataService.getApplicationById(this.applicationId)
      .pipe(takeWhile(() => this.componentActive))
      .subscribe((data: Application) => {
        if (data.establishmentParcelId) {
          data.establishmentParcelId = data.establishmentParcelId.replace(/-/g, '');
        }
        if (data.applicantType === 'IndigenousNation') {
          (<any>data).applyAsIndigenousNation = true;
        }
        this.application = data;

        this.addDynamicContent();

        const noNulls = Object.keys(data)
          .filter(e => data[e] !== null)
          .reduce((o, e) => {
            o[e] = data[e];
            return o;
          }, {});

        this.form.patchValue(noNulls);
        if (data.isPaid) {
          this.form.disable();
        }
      },
        () => {
          console.log('Error occured');
        }
      );
    this.loadData();
  }

  loadData() {
    this.GetNotTerminatedCRSApplicationCount();

    forkJoin(this.applicationDataService.getApplicationById(this.applicationId),
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

  save() {
    const data = this.cleanSaveData(this.changeTree);
    forkJoin(
      this.applicationDataService.updateApplication({ ...this.application, ...this.form.value }),
      this.legalEntityDataService.saveLicenseeChanges(data, this.applicationId))
      .subscribe(() => {
        this.loadData();
      });
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

