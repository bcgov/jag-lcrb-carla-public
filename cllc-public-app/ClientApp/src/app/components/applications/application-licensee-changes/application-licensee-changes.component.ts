import { Component, OnInit, ViewChild } from '@angular/core';
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

  editedTree: LicenseeChangeLog;
  busy: any;

  constructor(public dialog: MatDialog,
    private fb: FormBuilder,
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
    forkJoin(this.applicationDataService.getApplicationById(this.applicationId),
      this.legalEntityDataService.getChangeLogs(this.applicationId),
      this.legalEntityDataService.getCurrentHierachy())
      .pipe(takeWhile(() => this.componentActive))
      .subscribe((data: [Application, LicenseeChangeLog[], LegalEntity]) => {
        this.application = data[0];
        this.currentChangeLogs = data[1] || [];
        this.currentLegalEntities = data[2];
        this.addDynamicContent();
      },
        () => {
          console.log('Error occured');
        }
      );
  }

  populateChangeTables(node: LicenseeChangeLog) {
    if (node.isShareholderNew && node.isIndividual && node.changeType !== 'unchanged') {
      this.individualShareholderChanges.push(node);
    } else if (node.isShareholderNew && node.changeType !== 'unchanged') {
      this.organizationShareholderChanges.push(node);
    } else if (!node.isShareholderNew && node.changeType !== 'unchanged') {
      this.leadershipChanges.push(node);
    }

    if (node.children && node.children.length) {
      node.children.forEach(child => {
        this.populateChangeTables(child);
      });
    }
  }

  isAddChangeType(node: LicenseeChangeLog): boolean {
    const result = node.changeType === LicenseeChangeType.addLeadership
      || node.changeType === LicenseeChangeType.addBusinessShareholder
      || node.changeType === LicenseeChangeType.addIndividualShareholder;
    return result;
  }

  isUpdateChangeType(node: LicenseeChangeLog): boolean {
    const result = node.changeType === LicenseeChangeType.updateLeadership
      || node.changeType === LicenseeChangeType.updateBusinessShareholder
      || node.changeType === LicenseeChangeType.updateIndividualShareholder;
    return result;
  }

  isRemoveChangeType(node: LicenseeChangeLog): boolean {
    const result = node.changeType === LicenseeChangeType.removeLeadership
      || node.changeType === LicenseeChangeType.removeBusinessShareholder
      || node.changeType === LicenseeChangeType.removeIndividualShareholder;
    return result;
  }

  getRenderChangeType(item: LicenseeChangeLog): string {
    let changeType = '';
    if (this.isAddChangeType(item)) {
      changeType = 'Add';
    } else if (this.isUpdateChangeType(item)) {
      changeType = 'Update';
    } else if (this.isRemoveChangeType(item)) {
      changeType = 'Remove';
    }
    return changeType;
  }

  save() {
    const data = this.cleanSaveData(this.editedTree);
    this.legalEntityDataService.saveLicenseeChanges(data, this.applicationId)
      .subscribe(() => {
        this.loadData();
      });
  }

  cancelApplication(){
  }

  cleanSaveData(data: LicenseeChangeLog): LicenseeChangeLog {
    const result = { ...data } as LicenseeChangeLog;
    this.removeParentReference(result);
    return result;
  }

  removeParentReference(node: LicenseeChangeLog) {
    node.parentLinceseeChangeLog = undefined;
    if (node.children && node.children.length) {
      node.children.forEach(child => this.removeParentReference(child))
    }
  }
}

