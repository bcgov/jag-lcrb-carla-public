import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBase } from '@shared/form-base';
import { NestedTreeControl } from '@angular/cdk/tree';
import { LicenseeChangeLog, LicenseeChangeType } from '@models/legal-entity-change.model';
import { MatTreeNestedDataSource, MatTree, MatDialog } from '@angular/material';
import { Application } from '@models/application.model';
import { ActivatedRoute } from '@angular/router';
import { ApplicationDataService } from '@services/application-data.service';
import { LegalEntityDataService } from '@services/legal-entity-data.service';
import { forkJoin } from 'rxjs';
import { takeWhile, filter } from 'rxjs/operators';
import { LegalEntity } from '@models/legal-entity.model';

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

  constructor(public dialog: MatDialog,
    private route: ActivatedRoute,
    private applicationDataService: ApplicationDataService,
    private legalEntityDataService: LegalEntityDataService) {
    super();
    this.route.paramMap.subscribe(pmap => this.applicationId = pmap.get('applicationId'));
  }


  ngOnInit() {
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
    this.legalEntityDataService.saveLicenseeChanges(this.editedTree, this.applicationId)
      .subscribe(() => {
        this.loadData();
      });
  }
}
