import { Component, OnInit, ViewChild, Input, EventEmitter } from '@angular/core';
import { NestedTreeControl } from '@angular/cdk/tree';
import { MatTreeNestedDataSource, MatTree } from '@angular/material/tree';
import { LegalEntity } from '@models/legal-entity.model';
import { LicenseeChangeLog, LicenseeChangeType } from '@models/licensee-change-log.model';
import { MatDialog } from '@angular/material';
import { ShareholdersAndPartnersComponent } from './dialog-boxes/shareholders-and-partners/shareholders-and-partners.component';
import { OrganizationLeadershipComponent } from './dialog-boxes/organization-leadership/organization-leadership.component';
import { filter } from 'rxjs/operators';
import { FormBase } from '@shared/form-base';
import { Account } from '@models/account.model';


@Component({
  selector: 'app-licensee-tree',
  templateUrl: './licensee-tree.component.html',
  styleUrls: ['./licensee-tree.component.scss'],
})
export class LicenseeTreeComponent extends FormBase implements OnInit {
  @Input() treeRoot: LicenseeChangeLog;
  @Input() enableEditing = true;
  treeControl = new NestedTreeControl<LicenseeChangeLog>(node => node.children);
  dataSource = new MatTreeNestedDataSource<any>();
  @ViewChild('tree', { static: false }) tree: MatTree<any>;
  componentActive = true;
  changeTree: LicenseeChangeLog;
  individualShareholderChanges: LicenseeChangeLog[];
  organizationShareholderChanges: LicenseeChangeLog[];
  leadershipChanges: LicenseeChangeLog[];
  Account = Account;
  cancelledChanges: EventEmitter<LicenseeChangeLog> = new EventEmitter<LicenseeChangeLog>();

  constructor(public dialog: MatDialog) {
    super();
  }

  hasChild = (_: number, node: LicenseeChangeLog) => !!node.children && node.children.length > 0;

  ngOnInit() {
    this.dataSource.data = [this.treeRoot];
    this.refreshTreeAndChangeTables();
    this.treeControl.dataNodes = this.dataSource.data;
    this.treeControl.expandAll();
  }

  /**
   * Opens a dialog to edit a leader or shareholder
   * @param node 'A LicenseeChangeLog to edit'
   */
  editAssociate(node: LicenseeChangeLog) {
    let rootBusinessType = 'shareholder';
    if (node.isRoot && Account.getBusinessTypeFromName(node.businessType) === 'Partnership') {
      rootBusinessType = 'partnership';
    } else if (!node.isRoot && node.parentLinceseeChangeLog && Account.getBusinessTypeFromName(node.parentLinceseeChangeLog.businessType) === 'Partnership') {
      rootBusinessType = 'partnership';
    }
    if (node.isShareholderNew || node.isRoot) {
      this.openShareholderDialog(node, '', 'edit', rootBusinessType)
        .pipe(filter(data => !!data))
        .subscribe((formData: LicenseeChangeLog) => {
          if (node.changeType !== LicenseeChangeType.addBusinessShareholder
            && node.changeType !== LicenseeChangeType.addIndividualShareholder) {
            formData.changeType = formData.isIndividual ? LicenseeChangeType.updateIndividualShareholder
              : LicenseeChangeType.updateBusinessShareholder;
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
        this.treeControl.expandAll();
      }
      );
  }

  /**
   * Add a shareholder to the parent node
   * @param parentNode 'A LicenseeChangeLog to add the shareholder to'
   */
  addShareholder(parentNode: LicenseeChangeLog) {
    debugger;
    let rootBusinessType = 'shareholder';
    if (parentNode.isRoot && Account.getBusinessTypeFromName(parentNode.businessType) === 'Partnership') {
      rootBusinessType = 'partnership';
    } else if (!parentNode.isRoot && parentNode.parentLinceseeChangeLog && Account.getBusinessTypeFromName(parentNode.businessType) === 'Partnership') {
      rootBusinessType = 'partnership';
    }
    this.openShareholderDialog({ parentLinceseeChangeLog: parentNode } as LicenseeChangeLog, parentNode.businessNameNew, 'add', rootBusinessType)
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
        this.treeControl.expandAll();
      }
      );
  }

  /**
   * Marks a change log for deletion
   * @param node 'A LicenseeChangeLog to mark for delete'
   */
  deleteAssociate(node: LicenseeChangeLog) {
    if (node.legalEntityId) {
      if (node.isShareholderNew && node.isIndividual) {
        node.changeType = LicenseeChangeType.removeIndividualShareholder;
      } else if (node.isShareholderNew) {
        node.changeType = LicenseeChangeType.removeBusinessShareholder;
      } else if (node.isShareholderNew) {
        node.changeType = LicenseeChangeType.removeLeadership;
      }
    } else {
      this.cancelChange(node);
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
  openShareholderDialog(shareholder: LicenseeChangeLog, parentName: string, action: string, rootBusinessType: string) {
    // set dialogConfig settings
    const dialogConfig = {
      disableClose: true,
      autoFocus: true,
      maxWidth: '400px',
      data: {
        businessType: 'PrivateCorporation',
        shareholder: shareholder,
        parentName,
        action,
        rootBusinessType
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

  cancelChange(node: LicenseeChangeLog) {
    this.cancelledChanges.emit(node);
    if (!node.isRoot) {
      if (!node.legalEntityId) {
        const index = node.parentLinceseeChangeLog.children.indexOf(node);
        node.parentLinceseeChangeLog.children.splice(index, 1)
      } else {
        node.changeType = 'unchanged';
        node.businessNameNew = node.nameOld;
        node.isDirectorNew = node.isDirectorOld;
        node.isManagerNew = node.isManagerOld;
        node.isOfficerNew = node.isOfficerOld;
        node.isShareholderNew = node.isShareholderOld;
        node.isTrusteeNew = node.isTrusteeOld;
        node.numberofSharesNew = node.numberofSharesOld;
        node.totalSharesNew = node.totalSharesOld;
        node.emailNew = node.emailOld;
        node.firstNameNew = node.firstNameOld;
        node.lastNameNew = node.lastNameOld;
        node.businessNameNew = node.businessNameOld;
        node.dateofBirthNew = node.dateofBirthOld;
        node.titleNew = node.titleOld;
      }
    }
    this.refreshTreeAndChangeTables();
  }
}

