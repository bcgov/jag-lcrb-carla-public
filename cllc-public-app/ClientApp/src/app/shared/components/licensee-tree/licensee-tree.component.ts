import { Component, OnInit, ViewChild, Input, Output, EventEmitter } from '@angular/core';
import { NestedTreeControl } from '@angular/cdk/tree';
import { MatTreeNestedDataSource, MatTree } from '@angular/material/tree';
import { LegalEntity } from '@models/legal-entity.model';
import { LicenseeChangeLog, LicenseeChangeType } from '@models/legal-entity-change.model';
import { LegalEntityDataService } from '@services/legal-entity-data.service';
import { MatDialog } from '@angular/material';
import { ShareholdersAndPartnersComponent } from './dialog-boxes/shareholders-and-partners/shareholders-and-partners.component';
import { OrganizationLeadershipComponent } from './dialog-boxes/organization-leadership/organization-leadership.component';
import { filter } from 'rxjs/operators';
import { ActivatedRoute } from '@angular/router';
import { ApplicationDataService } from '@services/application-data.service';
import { FormBase } from '@shared/form-base';
import { Application } from '@models/application.model';


@Component({
  selector: 'app-licensee-tree',
  templateUrl: './licensee-tree.component.html',
  styleUrls: ['./licensee-tree.component.scss'],
})
export class LicenseeTreeComponent extends FormBase implements OnInit {
  @Input() treeRoot: LicenseeChangeLog;
  @Input() currentLegalEntityTree: LegalEntity;
  @Input() enableEditing = true;
  @Output() editedTree: EventEmitter<LicenseeChangeLog> = new EventEmitter<LicenseeChangeLog>();
  treeControl = new NestedTreeControl<LicenseeChangeLog>(node => node.children);
  dataSource = new MatTreeNestedDataSource<any>();
  @ViewChild('tree', { static: false }) tree: MatTree<any>;
  componentActive = true;
  changeTree: LicenseeChangeLog;
  individualShareholderChanges: LicenseeChangeLog[];
  organizationShareholderChanges: LicenseeChangeLog[];
  leadershipChanges: LicenseeChangeLog[];
  applicationId: string;
  application: Application;

  constructor(public dialog: MatDialog,
    private route: ActivatedRoute) {
    super();
    this.route.paramMap.subscribe(pmap => this.applicationId = pmap.get('applicationId'));
  }

  hasChild = (_: number, node: LicenseeChangeLog) => !!node.children && node.children.length > 0;

  ngOnInit() {
    // this.treeRoot = this.processLegalEntityTree(this.currentLegalEntityTree);
    // this.editedTree.emit(this.treeRoot);
    // this.treeRoot.isRoot = true;
    // this.changeTree = this.treeRoot;
    this.dataSource.data = [this.treeRoot];
    this.refreshTreeAndChangeTables();
    this.treeControl.dataNodes = this.dataSource.data;
    this.treeControl.expandAll();
  }

  /**
    * Finds a node in the tree where the compare predicate returns true
    * @param node 'Node in tree to search from'
    * @param compareFn 'a predicate to search for a node by
    */
  findNodeInTree(node: LicenseeChangeLog, compareFn: (node: LicenseeChangeLog) => boolean): LicenseeChangeLog {
    let result = null;

    if (compareFn(node)) {
      result = node;
    } else {
      const children = node.children || [];
      for (const child of children) {
        const res = this.findNodeInTree(child, compareFn);
        if (res) {
          result = res;
          break;
        }
      }
    }
    return result;
  }

  /**
   * Opens a dialog to edit a leader or shareholder
   * @param node 'A LicenseeChangeLog to edit'
   */
  editAssociate(node: LicenseeChangeLog) {
    if (node.isShareholderNew || node.isRoot) {
      this.openShareholderDialog(node, '')
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
      this.openLeadershipDialog(node, '')
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

  /**
   * Add a leader to the parent node
   * @param parentNode 'A LicenseeChangeLog to add the leader to'
   */
  addLeadership(parentNode: LicenseeChangeLog) {
    this.openLeadershipDialog({} as LicenseeChangeLog, parentNode.businessNameNew)
      .pipe(filter(data => !!data))
      .subscribe((formData: LicenseeChangeLog) => {
        formData.changeType = LicenseeChangeType.addLeadership;
        parentNode.children = parentNode.children || [];
        parentNode.children.push(formData);
        this.refreshTreeAndChangeTables();
      }
      );
  }

  /**
   * Add a shareholder to the parent node
   * @param parentNode 'A LicenseeChangeLog to add the shareholder to'
   */
  addShareholder(parentNode: LicenseeChangeLog) {
    this.openShareholderDialog({ parentLinceseeChangeLog: parentNode } as LicenseeChangeLog, parentNode.businessNameNew)
      .pipe(filter(data => !!data))
      .subscribe((formData: LicenseeChangeLog) => {
        if (formData.isIndividual) {
          formData.changeType = LicenseeChangeType.addIndividualShareholder;
        } else {
          formData.changeType = LicenseeChangeType.addBusinessShareholder;
        }
        parentNode.children = parentNode.children || [];
        parentNode.children.push(formData);
        this.refreshTreeAndChangeTables();
      }
      );
  }

  /**
   * Marks a change log for deletion
   * @param node 'A LicenseeChangeLog to mark for delete'
   */
  deleteAssociate(node: LicenseeChangeLog) {
    if (node.isShareholderNew && node.isIndividual) {
      node.changeType = LicenseeChangeType.removeIndividualShareholder;
    } else if (node.isShareholderNew) {
      node.changeType = LicenseeChangeType.removeBusinessShareholder;
    } else if (node.isShareholderNew) {
      node.changeType = LicenseeChangeType.removeLeadership;
    }

    this.refreshTreeAndChangeTables();
  }

  /*
  * Performs a Depth First Traversal and transforms the LegalEntity tree to change objects
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

  /**
   * Opens dialog for adding and editting shareholders
   * @param leader 'A LicenseeChangeLog'
   */
  openShareholderDialog(shareholder: LicenseeChangeLog, parentName: string) {
    // set dialogConfig settings
    const dialogConfig = {
      disableClose: true,
      autoFocus: true,
      maxWidth: '400px',
      data: {
        businessType: 'PrivateCorporation',
        shareholder: shareholder,
        parentName
      }
    };

    // open dialog, get reference and process returned data from dialog
    const dialogRef = this.dialog.open(ShareholdersAndPartnersComponent, dialogConfig);
    return dialogRef.afterClosed();
  }

  /**
   * Opens dialog for adding and editting leaders
   * @param leader 'A LicenseeChangeLog'
   */
  openLeadershipDialog(leader: LicenseeChangeLog, parentName: string) {
    // set dialogConfig settings
    const dialogConfig = {
      disableClose: true,
      autoFocus: true,
      width: '500px',
      data: {
        person: leader,
        businessType: 'PrivateCorporation',
        parentName
      }
    };

    // open dialog, get reference and process returned data from dialog
    const dialogRef = this.dialog.open(OrganizationLeadershipComponent, dialogConfig);
    return dialogRef.afterClosed();

  }

  /**
   * Repopulates the licensee tree and the change tables
   */
  refreshTreeAndChangeTables() {
    // change reference of the dataSource.data to cause the tree to re-render
    const data = [...this.dataSource.data];
    this.dataSource.data = [];
    this.dataSource.data = data;

    this.refreshChangeTables();
  }

  /**
   * Repopulates the change tables
   */
  refreshChangeTables() {
    this.individualShareholderChanges = [];
    this.organizationShareholderChanges = [];
    this.leadershipChanges = [];
    this.populateChangeTables(this.treeRoot);

    const sortByChangeType = (a: LicenseeChangeLog, b: LicenseeChangeLog) => {
      if (this.getRenderChangeType(a) >= this.getRenderChangeType(b)) {
        return 1;
      }
      return -1;
    };

    // sort change tables by change type
    this.individualShareholderChanges.sort(sortByChangeType);
    this.organizationShareholderChanges.sort(sortByChangeType);
    this.leadershipChanges.sort(sortByChangeType);

  }

  /**
   * Gets a ChangeType to be rendered
   * @param item 'A LicenseeChangeLog'
   */
  getRenderChangeType(item: LicenseeChangeLog): string {
    let changeType = '';
    if (item.isAddChangeType()) {
      changeType = 'Add';
    } else if (item.isUpdateChangeType()) {
      changeType = 'Update';
    } else if (item.isRemoveChangeType()) {
      changeType = 'Remove';
    }
    return changeType;
  }

  /**
   * Read the licensee tree and the changes to the change tables
   * @param node 'A LicenseeChangeLog to process'
   */
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
}
