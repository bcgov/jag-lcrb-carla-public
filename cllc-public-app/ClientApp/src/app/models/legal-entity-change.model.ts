import { Account } from './account.model';
import { Application } from './application.model';
import { LegalEntity } from './legal-entity.model';
import { Contact } from './contact.model';

export class LicenseeChangeLog {
  id: string; // guid
  changeType: string;
  isDirectorNew: boolean;
  isDirectorOld: boolean;
  isManagerNew: boolean;
  isManagerOld: boolean;
  isOfficerNew: boolean;
  isOfficerOld: boolean;
  isShareholderNew: boolean;
  isShareholderOld: boolean;
  isTrusteeNew: boolean;
  isTrusteeOld: boolean;
  businessAccountType: string;
  numberofSharesNew: number;
  numberofSharesOld: number;
  totalSharesNew: number;
  totalSharesOld: number;
  emailNew: string;
  emailOld: string;
  firstNameNew: string;
  firstNameOld: string;
  lastNameNew: string;
  lastNameOld: string;
  LicenseeChangelogid: string;
  businessNameNew: string;
  businessNameOld: string;
  nameOld: string;
  dateofBirthNew: Date;
  dateofBirthOld: Date;
  titleNew: string;
  titleOld: string;

  applicationId: string;
  applicationType: string;
  legalEntityId: string;
  parentLegalEntityId: string;
  parentLinceseeChangeLogId: string;
  children: LicenseeChangeLog[];
  parentLinceseeChangeLog: LicenseeChangeLog;

  isRoot: boolean; // This is only used on the client side
  isIndividual: boolean; // This is only used on the client side
  percentageShares: number; // This in only used on the client side

  /**
   * Create from LegalEntity
   */
  constructor(legalEntity: LegalEntity = null) {
    if (legalEntity) {
      this.legalEntityId = legalEntity.id;
      this.isIndividual = legalEntity.isindividual;
      this.parentLegalEntityId = legalEntity.parentLegalEntityId;
      this.changeType = 'unchanged';
      this.isDirectorNew = legalEntity.isDirector;
      this.isDirectorOld = legalEntity.isDirector;
      this.isManagerNew = legalEntity.isSeniorManagement;
      this.isManagerOld = legalEntity.isSeniorManagement;
      this.isOfficerNew = legalEntity.isOfficer;
      this.isOfficerOld = legalEntity.isOfficer;
      this.isShareholderNew = legalEntity.isShareholder;
      this.isShareholderOld = legalEntity.isShareholder;
      this.isTrusteeNew = legalEntity.isTrustee;
      this.isTrusteeOld = legalEntity.isTrustee;
      if (legalEntity.account) {
        this.businessAccountType = legalEntity.account.businessType;
      }
      this.numberofSharesNew = legalEntity.commonvotingshares;
      this.numberofSharesOld = legalEntity.commonvotingshares;
      this.emailNew = legalEntity.email;
      this.emailOld = legalEntity.email;
      this.firstNameNew = legalEntity.firstname;
      this.firstNameOld = legalEntity.firstname;
      this.lastNameNew = legalEntity.lastname;
      this.lastNameOld = legalEntity.lastname;
      this.businessNameNew = legalEntity.name;
      this.nameOld = legalEntity.name;
      this.dateofBirthNew = legalEntity.dateofbirth;
      this.dateofBirthOld = legalEntity.dateofbirth;
      this.titleNew = legalEntity.jobTitle;
      this.titleOld = legalEntity.jobTitle;


    }
  }

  getNewLeadershipPosition(): string {
    let position = '';
    if (this.isDirectorNew) {
      position += 'Director, ';
    }
    if (this.isManagerNew) {
      position += 'Manager, ';
    }
    if (this.isOfficerNew) {
      position += 'Officer, ';
    }
    if (this.isTrusteeNew) {
      position += 'Trustee, ';
    }
    if (this.titleNew) {
      position += `${this.titleNew}, `;
    }
    position = position.substring(0, position.length - 2);
    return position;
  }

  getOldLeadershipPosition(): string {
    let position = '';
    if (this.isDirectorOld) {
      position += 'Director, ';
    }
    if (this.isManagerOld) {
      position += 'Manager, ';
    }
    if (this.isOfficerOld) {
      position += 'Officer, ';
    }
    if (this.isTrusteeOld) {
      position += 'Trustee, ';
    }
    if (this.titleOld) {
      position += `${this.titleOld}, `;
    }
    position = position.substring(0, position.length - 2);
    return position;
  }

  /*
  * Performs a Depth First Traversal and transforms the LegalEntity tree to change objects
  */
  public static processLegalEntityTree(node: LegalEntity): LicenseeChangeLog {
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

  applySavedChangeLogs(currentChangeLogs: LicenseeChangeLog[]) {
    const changesWithLegalEntityId = currentChangeLogs.filter(item => !!item.legalEntityId);
    const changesWithParentLegalEntityId = currentChangeLogs.filter(item => !item.legalEntityId && !!item.parentLegalEntityId);
    const changesWithParentChangeLogId =
      currentChangeLogs.filter(item => !item.legalEntityId && !item.parentLegalEntityId && !!item.parentLinceseeChangeLogId);

    changesWithLegalEntityId.forEach(change => {

      change = Object.assign(new LicenseeChangeLog(), change);
      const node = LicenseeChangeLog.findNodeInTree(this, (node) => node.legalEntityId === change.legalEntityId);
      if (node) {
        if (change.firstNameNew) {
          change.businessNameNew = `${change.firstNameNew} ${change.lastNameNew}`;
        }

        change.isIndividual = false;
        if (change.isIndividualFromChangeType()) {
          change.isIndividual = true;
        }
        Object.assign(node, change);
      }
    });

    changesWithParentLegalEntityId.forEach(change => {
      const node = LicenseeChangeLog.findNodeInTree(this, (node) => node.legalEntityId === change.parentLegalEntityId);
      if (node) {
        node.children = node.children || [];
        const newNode = Object.assign(new LicenseeChangeLog(), change);
        if (newNode.firstNameNew) {
          newNode.businessNameNew = `${newNode.firstNameNew} ${newNode.lastNameNew}`;
        }

        newNode.isIndividual = false;
        if (newNode.isIndividualFromChangeType()) {
          newNode.isIndividual = true;
        }
        node.children.push(newNode);
      }
    });

    changesWithParentChangeLogId.forEach(change => {
      const node = LicenseeChangeLog.findNodeInTree(this, (node) => node.id === change.parentLinceseeChangeLogId);
      if (node) {
        node.children = node.children || [];
        const newNode = Object.assign(new LicenseeChangeLog(), change);
        if (newNode.firstNameNew) {
          newNode.businessNameNew = `${newNode.firstNameNew} ${newNode.lastNameNew}`;
        }

        newNode.isIndividual = false;
        if (newNode.isIndividualFromChangeType()) {
          newNode.isIndividual = true;
        }
        node.children.push(newNode);
      }
    });
  }
  /**
    * Finds a node in the tree where the compare predicate returns true
    * @param node 'Node in tree to search from'
    * @param compareFn 'a predicate to search for a node by
    */
  static findNodeInTree(node: LicenseeChangeLog, compareFn: (node: LicenseeChangeLog) => boolean): LicenseeChangeLog {
    let result = null;

    if (compareFn(node)) {
      result = node;
    } else {
      const children = node.children || [];
      for (const child of children) {
        const res = LicenseeChangeLog.findNodeInTree(child, compareFn);
        if (res) {
          result = res;
          break;
        }
      }
    }
    return result;
  }

  /**
   * Use the chagetype to check if the licensee is an individual
   * @param changeType
   */
  isIndividualFromChangeType(): boolean {
    const result = this.changeType.toLowerCase().indexOf('individual') !== -1
      || this.changeType.toLowerCase().indexOf('leadership') !== -1;
    return result;
  }

  /**
   * Returns true if the change type is an add. Otherwise it returs false
   * @param node 'A LicenseeChangeLog'
   */
  isAddChangeType(): boolean {
    const result = this.changeType === LicenseeChangeType.addLeadership
      || this.changeType === LicenseeChangeType.addBusinessShareholder
      || this.changeType === LicenseeChangeType.addIndividualShareholder;
    return result;
  }

  /**
   * Returns true if the change type is an update. Otherwise it returs false
   * @param node 'A LicenseeChangeLog'
   */
  isUpdateChangeType(): boolean {
    const result =this.changeType === LicenseeChangeType.updateLeadership
      ||this.changeType === LicenseeChangeType.updateBusinessShareholder
      ||this.changeType === LicenseeChangeType.updateIndividualShareholder;
    return result;
  }

  /**
   * Returns true if the change type is a delete. Otherwise it returs false
   * @param node 'A LicenseeChangeLog'
   */
  isRemoveChangeType(): boolean {
    const result =this.changeType === LicenseeChangeType.removeLeadership
      ||this.changeType === LicenseeChangeType.removeBusinessShareholder
      ||this.changeType === LicenseeChangeType.removeIndividualShareholder;
    return result;
  }


  cancelChange(node: LicenseeChangeLog) {
    // delete change log record
    // ??What to do with children when their parent add is cancelled
    // 
  }

  /**
   * Returns true if a leader (e.g. a director or  manager) was added in
   * the application
   */
  public static leadershipWasAdded(treeRoot: LicenseeChangeLog): boolean {
    const isLeaderAddChange = (node: LicenseeChangeLog) => node.changeType === LicenseeChangeType.addLeadership;
    const result = !!this.findNodeInTree(treeRoot, isLeaderAddChange);
    return result;
  }

  /**
   * Returns true if a business shareholder  was added in
   * the application
   */
  public static businessShareholderWasAdded(treeRoot: LicenseeChangeLog): boolean {
    const businessShareholderWasAdded = (node: LicenseeChangeLog) => node.changeType === LicenseeChangeType.addBusinessShareholder;
    const result = !!this.findNodeInTree(treeRoot, businessShareholderWasAdded);
    return result;
  }

  /**
   * Returns true if an individual shareholder  was added in
   * the application
   */
  public static individualShareholderWasAdded(treeRoot: LicenseeChangeLog): boolean {
    const individualShareholderWasAdded = (node: LicenseeChangeLog) => node.changeType === LicenseeChangeType.addIndividualShareholder;
    const result = !!this.findNodeInTree(treeRoot, individualShareholderWasAdded);
    return result;
  }

  /**
  * Returns true iff a shareholder  was added in
  * the application
  */
  public static shareholderWasAdded(treeRoot: LicenseeChangeLog): boolean {
    return this.businessShareholderWasAdded(treeRoot) || this.individualShareholderWasAdded(treeRoot);
  }

  /**
  * Returns true iff there is a delete-type in
  * the application
  */
  public static shareholderWasRemoved(treeRoot: LicenseeChangeLog): boolean {
    const shareholderWasRemoved = (node: LicenseeChangeLog) => node.changeType === LicenseeChangeType.removeBusinessShareholder
      || node.changeType === LicenseeChangeType.removeIndividualShareholder;
    const result = !!this.findNodeInTree(treeRoot, shareholderWasRemoved);
    return result;
  }

  /**
  * Returns true iff a share amount was changed in any of the edit type change-logs in
  * the application
  */
  public static someShareAmountWasChanged(treeRoot: LicenseeChangeLog): boolean {
    const shareAmoutWasChanged = (node: LicenseeChangeLog) => node.isUpdateChangeType() && node.numberofSharesOld !== node.numberofSharesNew;
    const result = !!this.findNodeInTree(treeRoot, shareAmoutWasChanged);
    return result;
  }

  /**
   * Returns true if a name of was changed in any of the edit type change-logs
   * the application
   */
  public static aNameChangeWasPerformed(treeRoot: LicenseeChangeLog): boolean {
    const nameWasChanged = (node: LicenseeChangeLog) => {
      const isNameChange = node.isUpdateChangeType() &&
        (
          // check for individual name change
          (node.isIndividualFromChangeType() &&
            (node.firstNameNew !== node.firstNameOld || node.lastNameNew !== node.lastNameOld))

          // check for busines type name change
          || (!node.isIndividualFromChangeType() && node.businessNameNew !== node.businessNameOld)
        );
      return isNameChange;
    };
    const result = !!this.findNodeInTree(treeRoot, nameWasChanged);
    return result;
  }

}


export enum LicenseeChangeType {
  addLeadership = 'addLeadership',
  updateLeadership = 'updateLeadership',
  removeLeadership = 'removeLeadership',
  addBusinessShareholder = 'addBusinessShareholder',
  updateBusinessShareholder = 'updateBusinessShareholder',
  removeBusinessShareholder = 'removeBusinessShareholder',
  addIndividualShareholder = 'addIndividualShareholder',
  updateIndividualShareholder = 'updateIndividualShareholder',
  removeIndividualShareholder = 'removeIndividualShareholder'
}

