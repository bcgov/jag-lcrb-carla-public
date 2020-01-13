
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

@Component({
  selector: 'app-organization-structure',
  templateUrl: './organization-structure.component.html',
  styleUrls: ['./organization-structure.component.scss']
})
export class OrganizationStructureComponent extends FormBase implements OnInit {
  account: Account;
  applicationId = '1';
  busy: any;
  treeRoot: LicenseeChangeLog;

  constructor(private store: Store<AppState>,
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
        this.treeRoot .isRoot = true;
        this.treeRoot .applySavedChangeLogs(currentChangeLogs);
      },
        () => {
          console.log('Error occured');
        }
      );
  }



}
