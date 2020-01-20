
import { filter, takeWhile } from 'rxjs/operators';
import { Component, OnInit, Input } from '@angular/core';
import { Store } from '@ngrx/store';
import { AppState } from '@app/app-state/models/app-state';
import { Account } from '@models/account.model';
import { LicenseeChangeType, LicenseeChangeLog } from '@models/licensee-change-log.model';
import { FormBase } from '@shared/form-base';
import { LegalEntityDataService } from '@services/legal-entity-data.service';
import { forkJoin } from 'rxjs';
import { Application } from '@models/application.model';
import { LegalEntity } from '@models/legal-entity.model';
import { MatSnackBar } from '@angular/material';

@Component({
  selector: 'app-organization-structure',
  templateUrl: './organization-structure.component.html',
  styleUrls: ['./organization-structure.component.scss']
})
export class OrganizationStructureComponent extends FormBase implements OnInit {
  account: Account;
  applicationId = '1';
  busy: any;
  busySave: any;
  treeRoot: LicenseeChangeLog;
  cancelledLicenseeChanges: LicenseeChangeLog[];

  constructor(private store: Store<AppState>,
    private snackBar: MatSnackBar,
    private legalEntityDataService: LegalEntityDataService) {
    super();
  }

  ngOnInit() {
    this.store.select(state => state.currentAccountState.currentAccount).pipe(
      filter(account => !!account))
      .subscribe(account => {
        this.account = account;
        this.loadData();
      });
  }

  loadData() {
    this.busy = forkJoin(
      this.legalEntityDataService.getAccountChangeLogs(this.account.id),
      this.legalEntityDataService.getCurrentHierachy())
      .pipe(takeWhile(() => this.componentActive))
      .subscribe((data: [LicenseeChangeLog[], LegalEntity]) => {
        const currentChangeLogs = data[0] || [];
        const currentLegalEntities = data[1];
        this.treeRoot = LicenseeChangeLog.processLegalEntityTree(currentLegalEntities);
        this.treeRoot.isRoot = true;
        this.treeRoot.applySavedChangeLogs(currentChangeLogs);
      },
        () => {
          console.log('Error occured');
        }
      );
  }

  /**
   * Sends data to dynamics
   */
  save() {
    const data = this.cleanSaveData(this.treeRoot);
    this.busySave = forkJoin(
      this.legalEntityDataService.saveAccountLicenseeChanges(data, this.account.id),
      // this.legalEntityDataService.cancelLicenseeChanges(this.cancelledLicenseeChanges)
      )
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
