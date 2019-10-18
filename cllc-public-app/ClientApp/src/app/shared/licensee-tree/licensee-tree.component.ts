import { Component, OnInit, ViewChild, Input, Output, EventEmitter } from '@angular/core';
import { NestedTreeControl } from '@angular/cdk/tree';
import { MatTreeNestedDataSource, MatTree } from '@angular/material/tree';
import { Store } from '@ngrx/store';
import { AppState } from '@app/app-state/models/app-state';
import { Account } from '@models/account.model';
import { LegalEntity } from '@models/legal-entity.model';
import { LicenseeChangeLog, LicenseeChangeType } from '@models/legal-entity-change.model';
import { LegalEntityDataService } from '@services/legal-entity-data.service';
import { MatDialog } from '@angular/material';
import { ShareholdersAndPartnersComponent } from './dialog-boxes/shareholders-and-partners/shareholders-and-partners.component';
import { OrganizationLeadershipComponent } from './dialog-boxes/organization-leadership/organization-leadership.component';
import { filter, takeWhile } from 'rxjs/operators';
import { ActivatedRoute } from '@angular/router';
import { ApplicationDataService } from '@services/application-data.service';
import { FormBase } from '@shared/form-base';
import { Application } from '@models/application.model';
import { forkJoin } from 'rxjs';


@Component({
  selector: 'app-licensee-tree',
  templateUrl: './licensee-tree.component.html',
  styleUrls: ['./licensee-tree.component.scss'],
})
export class LicenseeTreeComponent extends FormBase implements OnInit {
  @Input() currentChangeLogs: LicenseeChangeLog[];
  @Input() currentLegalEntityTree: LegalEntity;
  @Input() enableEditing = true;
  @Output() editedTree: EventEmitter<LicenseeChangeLog> = new EventEmitter<LicenseeChangeLog>();
  treeControl = new NestedTreeControl<LicenseeChangeLog>(node => node.children);
  dataSource = new MatTreeNestedDataSource<any>();
  @ViewChild('tree') tree: MatTree<any>;
  componentActive = true;
  changeTree: LicenseeChangeLog;
  individualShareholderChanges: LicenseeChangeLog[];
  organizationShareholderChanges: LicenseeChangeLog[];
  leadershipChanges: LicenseeChangeLog[];
  treeRoot: LicenseeChangeLog;
  applicationId: string;
  application: Application;

  constructor(public dialog: MatDialog,
    private route: ActivatedRoute,
    private applicationDataService: ApplicationDataService,
    private legalEntityDataService: LegalEntityDataService) {
    super();
    this.route.paramMap.subscribe(pmap => this.applicationId = pmap.get('applicationId'));
  }

  hasChild = (_: number, node: LicenseeChangeLog) => !!node.children && node.children.length > 0;

  ngOnInit() {
    this.treeRoot = this.processLegalEntityTree(this.currentLegalEntityTree);
    this.editedTree.emit(this.treeRoot);
    this.treeRoot.isRoot = true;
    this.changeTree = this.treeRoot;
    this.dataSource.data = [this.treeRoot];
    this.applySavedChangeLogs();
    this.refreshTreeAndChangeTables();
    this.treeControl.dataNodes = this.dataSource.data;
    this.treeControl.expandAll();
  }



  applySavedChangeLogs() {
    const changesWithLegalEntityId = this.currentChangeLogs.filter(item => !!item.legalEntityId);
    const changesWithParentLegalEntityId = this.currentChangeLogs.filter(item => !item.legalEntityId && !!item.parentLegalEntityId);
    const changesWithParentChangeLogId =
      this.currentChangeLogs.filter(item => !item.legalEntityId && !item.parentLegalEntityId && !!item.parentLinceseeChangeLogId);

    changesWithLegalEntityId.forEach(change => {
      const node = this.findNodeInTree(this.treeRoot, change.legalEntityId);
      if (node) {
        if (change.firstNameNew) {
          change.businessNameNew = `${change.firstNameNew} ${change.lastNameNew}`;
        }

        change.isIndividual = false;
        if (this.isIndividualFromChangeType(change.changeType)) {
          change.isIndividual = true;
        }
        Object.assign(node, change);
      }
    });

    changesWithParentLegalEntityId.forEach(change => {
      const node = this.findNodeInTree(this.treeRoot, change.parentLegalEntityId);
      if (node) {
        node.children = node.children || [];
        const newNode = Object.assign(new LicenseeChangeLog(), change);
        if (newNode.firstNameNew) {
          newNode.businessNameNew = `${newNode.firstNameNew} ${newNode.lastNameNew}`;
        }

        newNode.isIndividual = false;
        if (this.isIndividualFromChangeType(newNode.changeType)) {
          newNode.isIndividual = true;
        }
        node.children.push(newNode);
      }
    });

    changesWithParentChangeLogId.forEach(change => {
      const node = this.findNodeInTree(this.treeRoot, null, change.parentLinceseeChangeLogId);
      if (node) {
        node.children = node.children || [];
        const newNode = Object.assign(new LicenseeChangeLog(), change);
        if (newNode.firstNameNew) {
          newNode.businessNameNew = `${newNode.firstNameNew} ${newNode.lastNameNew}`;
        }

        newNode.isIndividual = false;
        if (this.isIndividualFromChangeType(newNode.changeType)) {
          newNode.isIndividual = true;
        }
        node.children.push(newNode);
      }
    });
  }

  findNodeInTree(node: LicenseeChangeLog, legalEntityId: string = null, changeLogId: string = null): LicenseeChangeLog {
    let result = null;

    if (legalEntityId && node.legalEntityId === legalEntityId) {
      result = node;
    } else if (changeLogId && node.id === changeLogId) {
      result = node;
    } else {
      const children = node.children || [];
      for (const child of children) {
        const res = this.findNodeInTree(child, legalEntityId, changeLogId);
        if (res) {
          result = res;
          break;
        }
      }
    }
    return result;
  }

  isIndividualFromChangeType(changeType: string) {
    const result = changeType.toLowerCase().indexOf('individual') !== -1
      || changeType.toLowerCase().indexOf('leadership') !== -1;
    return result;
  }

  editAssociate(node: LicenseeChangeLog) {
    if (node.isShareholderNew) {
      this.openShareholderDialog(node)
        .pipe(filter(data => !!data))
        .subscribe((formData: LicenseeChangeLog) => {
          if (node.changeType !== LicenseeChangeType.addBusinessShareholder
            && node.changeType !== LicenseeChangeType.addIndividualShareholder) {
            formData.changeType = formData.isIndividual ? LicenseeChangeType.updateIndividualShareholder
              : LicenseeChangeType.updateIndividualShareholder;
          }
          node = Object.assign(node, formData);
          this.refreshTreeAndChangeTables();
        }
        );
    } else {
      this.openLeadershipDialog(node)
        .pipe(filter(data => !!data))
        .subscribe(
          formData => {
            if (node.changeType !== LicenseeChangeType.addLeadership) {
              formData.changeType = LicenseeChangeType.updateLeadership;
            }
            node = Object.assign(node, formData);
            this.refreshTreeAndChangeTables();
          }
        );
    }
  }

  addLeadership(node) {
    this.openLeadershipDialog({})
      .pipe(filter(data => !!data))
      .subscribe((formData: LicenseeChangeLog) => {
        formData.changeType = LicenseeChangeType.addLeadership;
        node.children = node.children || [];
        node.children.push(formData);
        this.refreshTreeAndChangeTables();
      }
      );
  }

  addShareholder(node) {
    this.openShareholderDialog({})
      .pipe(filter(data => !!data))
      .subscribe((formData: LicenseeChangeLog) => {
        if (formData.isIndividual) {
          formData.changeType = LicenseeChangeType.addIndividualShareholder;
        } else {
          formData.changeType = LicenseeChangeType.addBusinessShareholder;
        }
        node.children = node.children || [];
        node.children.push(formData);
        this.refreshTreeAndChangeTables();
      }
      );
  }

  deleteAssociate(node: LicenseeChangeLog, changeType = 'deleted') {
    // if (node.changeType === 'add') {
    //   const index = node.parentLinceseeChangeLog.children.indexOf(node);
    //   node.parentLinceseeChangeLog.children.splice(index, 1);
    // } else {
    if (node.isShareholderNew && node.isIndividual) {
      changeType = LicenseeChangeType.removeIndividualShareholder;
    } else if (node.isShareholderNew) {
      changeType = LicenseeChangeType.removeBusinessShareholder;
    } else if (node.isShareholderNew) {
      changeType = LicenseeChangeType.removeLeadership;
    }
    node.changeType = changeType;
    // const children = node.children || [];
    // children.forEach(child => {
    //   this.deleteAssociate(child, 'parent-deleted');
    // });
    // }
    this.refreshTreeAndChangeTables();
  }

  /*
  * Perform Depth First Traversal and transform tree to change objects
  */
  processLegalEntityTree(node: LegalEntity): LicenseeChangeLog {
    const newNode = new LicenseeChangeLog(node);
    if (node.children && node.children.length) {
      newNode.children = [];
      node.children.forEach(child => {
        const childNode = this.processLegalEntityTree(child);
        // childNode.parentLinceseeChangeLog = newNode;
        newNode.children.push(childNode);
      });
    }
    return newNode;
  }

  openShareholderDialog(shareholder) {
    // set dialogConfig settings
    const dialogConfig = {
      disableClose: true,
      autoFocus: true,
      maxWidth: '400px',
      data: {
        businessType: 'PrivateCorporation',
        shareholder: shareholder
      }
    };

    // open dialog, get reference and process returned data from dialog
    const dialogRef = this.dialog.open(ShareholdersAndPartnersComponent, dialogConfig);
    return dialogRef.afterClosed();
  }

  openLeadershipDialog(leader) {
    // set dialogConfig settings
    const dialogConfig = {
      disableClose: true,
      autoFocus: true,
      width: '500px',
      data: {
        person: leader,
        businessType: 'PrivateCorporation'
      }
    };

    // open dialog, get reference and process returned data from dialog
    const dialogRef = this.dialog.open(OrganizationLeadershipComponent, dialogConfig);
    return dialogRef.afterClosed();

  }


  refreshTreeAndChangeTables() {
    const data = [...this.dataSource.data];
    this.dataSource.data = [];
    this.dataSource.data = data;
    this.refreshChangeTables();
  }

  refreshChangeTables() {
    this.individualShareholderChanges = [];
    this.organizationShareholderChanges = [];
    this.leadershipChanges = [];
    this.populateChangeTables(this.treeRoot);

    const sortByChangeType = (a: LicenseeChangeLog, b: LicenseeChangeLog) => {
      if (a.changeType <= b.changeType) {
        return 1;
      }
      return -1;
    };

    this.individualShareholderChanges.sort(sortByChangeType);
    this.organizationShareholderChanges.sort(sortByChangeType);
    this.leadershipChanges.sort(sortByChangeType);

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

  OnDestroy() {
    this.componentActive = false;
  }
}
