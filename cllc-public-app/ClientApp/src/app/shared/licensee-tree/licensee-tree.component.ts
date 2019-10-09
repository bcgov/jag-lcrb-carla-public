import { Component, OnInit, Injectable, ViewChild, OnDestroy } from '@angular/core';
import { NestedTreeControl } from '@angular/cdk/tree';
import { MatTreeNestedDataSource, MatTree } from '@angular/material/tree';
import { Store } from '@ngrx/store';
import { AppState } from '@app/app-state/models/app-state';
import { Account } from '@models/account.model';
import { LegalEntity } from '@models/legal-entity.model';
import { LicenseeChangeLog } from '@models/legal-entity-change.model';
import { LegalEntityDataService } from '@services/legal-entity-data.service';
import { MatDialog } from '@angular/material';
import { ShareholdersAndPartnersComponent } from './dialog-boxes/shareholders-and-partners/shareholders-and-partners.component';
import { OrganizationLeadershipComponent } from './dialog-boxes/organization-leadership/organization-leadership.component';
import { filter } from 'rxjs/operators';
import { LicenseeChangeLog } from './../../models/legal-entity-change.model';



/**
 * Food; data; with nested structure.
 * Each; node; has; a; name; and; an; optiona; list; of; children.
 */
interface FoodNode {
  name: string;
  newName?: string;
  children?: FoodNode[];
  isNew?: boolean;
  deleted?: boolean;
  edited?: boolean;
}

@Component({
  selector: 'app-licensee-tree',
  templateUrl: './licensee-tree.component.html',
  styleUrls: ['./licensee-tree.component.scss'],
})
export class LicenseeTreeComponent implements OnInit {
  treeControl = new NestedTreeControl<FoodNode>(node => node.children);
  dataSource = new MatTreeNestedDataSource<any>();
  @ViewChild('tree') tree: MatTree<any>;
  componentActive = true;
  account: Account;
  changeTree: LicenseeChangeLog;
  individualShareholderChanges: LicenseeChangeLog[];
  organizationShareholderChanges: LicenseeChangeLog[];
  leadershipChanges: LicenseeChangeLog[];
  treeRoot: LicenseeChangeLog;

  constructor(private store: Store<AppState>,
    public dialog: MatDialog,
    private legalEntityDataService: LegalEntityDataService) {
    // this.dataSource.data = TREE_DATA;
  }

  hasChild = (_: number, node: FoodNode) => !!node.children && node.children.length > 0;

  ngOnInit() {
    this.legalEntityDataService.getCurrentHierachy()
      .subscribe(legalEntity => {
        this.treeRoot = this.processLegalEntityTree(legalEntity);
        this.treeRoot.isRoot = true;
        this.changeTree = this.treeRoot;
        this.dataSource.data = [this.treeRoot];
        this.refreshTreeAndChangeTables();
      });
  }

  editAssociate(node) {
    if (node.isShareholderNew) {
      this.openShareholderDialog(node)
        .pipe(filter(data => !!data))
        .subscribe(
          formData => {
            if (node.typeOfChange !== 'add') {
              formData.typeOfChange = 'edit';
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
            if (node.typeOfChange !== 'add') {
              formData.typeOfChange = 'edit';
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
          formData.typeOfChange = 'add';
          formData.isIndividual = true;
          formData.parentLinceseeChangeLog = node;
          node.children = node.children || [];
          node.children.push(formData);
          this.refreshTreeAndChangeTables();
        }
      );
  }

  addShareholder(node) {
    this.openShareholderDialog({})
      .pipe(filter(data => !!data))
      .subscribe(
        formData => {
          formData.typeOfChange = 'add';
          formData.parentLinceseeChangeLog = node;
          node.children = node.children || [];
          node.children.push(formData);
          this.refreshTreeAndChangeTables();
        }
      );
  }

  deleteAssociate(node: LicenseeChangeLog, changeType = 'delete') {
    if (node.typeOfChange === 'add') {
      const index = node.parentLinceseeChangeLog.children.indexOf(node);
      node.parentLinceseeChangeLog.children.splice(index, 1);
    } else {
      node.typeOfChange = changeType;
      const children = node.children || [];
      children.forEach(child => {
        this.deleteAssociate(child, 'parent-deleted');
      });
    }
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
        childNode.parentLinceseeChangeLog = newNode;
        newNode.children.push(childNode);
      });
    }
    return newNode;
  }

  showAddLink(node: LicenseeChangeLog): boolean {
    return true;
  }

  showEditLink(node: LicenseeChangeLog): boolean {
    return true;
  }

  showDeleteLink(node: LicenseeChangeLog): boolean {
    return true;
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
    const sortByTypeOfChange = (a, b) => {
      if (a.titleOld <= b.typeOfChange) {
        return 1;
      }
      return -1;
    };
    this.individualShareholderChanges.sort(sortByTypeOfChange);
    this.organizationShareholderChanges.sort(sortByTypeOfChange);
    this.leadershipChanges.sort(sortByTypeOfChange);
  }

  populateChangeTables(node: LicenseeChangeLog) {
    if (node.isShareholderNew && node.isIndividual && node.typeOfChange !== 'unchanged') {
      this.individualShareholderChanges.push(node);
    } else if (node.isShareholderNew && node.typeOfChange !== 'unchanged') {
      this.organizationShareholderChanges.push(node);
    } else if (!node.isShareholderNew && node.typeOfChange !== 'unchanged') {
      this.leadershipChanges.push(node);
    }

    if (node.children && node.children.length) {
      node.children.forEach(child => {
        this.populateChangeTables(child);
      });
    }
  }

  OnDestroy() {
    this.componentActive = false;
  }
}
