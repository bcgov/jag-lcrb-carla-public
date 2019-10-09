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

  constructor(private store: Store<AppState>,
    public dialog: MatDialog,
    private legalEntityDataService: LegalEntityDataService) {
    // this.dataSource.data = TREE_DATA;
  }

  hasChild = (_: number, node: FoodNode) => !!node.children && node.children.length > 0;

  ngOnInit() {
    this.legalEntityDataService.getCurrentHierachy()
      .subscribe(legalEntity => {
        const tree = this.processLegalEntityTree(legalEntity);
        tree.isRoot = true;
        this.changeTree = tree;
        this.dataSource.data = [tree];
        this.refreshTree();
      });
  }

  editAssociate(node) {
    if (node.isShareholderNew) {
      this.openShareholderDialog(node)
        .pipe(filter(data => !!data))
        .subscribe(
          formData => {
            if (node.changeType !== 'add') {
              formData.changeType = 'edit';
            }
            node = Object.assign(node, formData);
            this.refreshTree();
          }
        );
    } else {
      this.openLeadershipDialog(node)
        .pipe(filter(data => !!data))
        .subscribe(
          formData => {
            if (node.changeType !== 'add') {
              formData.changeType = 'edit';
            }
            node = Object.assign(node, formData);
            this.refreshTree();
          }
        );
    }
  }

  addLeadership(node) {
    this.openLeadershipDialog({})
      .pipe(filter(data => !!data))
      .subscribe(
        formData => {
          formData.changeType = 'add';
          formData.isIndividual = true;
          node.children = node.children || [];
          node.children.push(formData);
          this.refreshTree();
        }
      );
  }

  addShareholder(node) {
    this.openShareholderDialog({})
      .pipe(filter(data => !!data))
      .subscribe(
        formData => {
          formData.changeType = 'add';
          node.children = node.children || [];
          node.children.push(formData);
          this.refreshTree();
        }
      );
  }

  deleteAssociate(node) {
    node.changeType = 'delete';
    const children = node.children || [];
    children.forEach(element => {
      this.deleteAssociate(element);
    });
    this.refreshTree();
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


  refreshTree() {
    const data = [...this.dataSource.data];
    this.dataSource.data = [];
    this.dataSource.data = data;
  }

  OnDestroy() {
    this.componentActive = false;
  }
}
