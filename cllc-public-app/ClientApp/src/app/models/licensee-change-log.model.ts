import { Account } from './account.model';
import { Application } from './application.model';
import { LegalEntity } from './legal-entity.model';
import { Contact } from './contact.model';
import { LicenseeTreeComponent } from '@shared/components/licensee-tree/licensee-tree.component';

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
  businessType: string;
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
  parentBusinessAccountId: string;
  businessAccountId: string;
  children: LicenseeChangeLog[];
  parentLinceseeChangeLog: LicenseeChangeLog;
  interestPercentageNew: number;
  interestPercentageOld: number;
  phsLink: string;

  isRoot: boolean; // This is only used on the client side
  isIndividual: boolean; // This is only used on the client side
  edit: boolean; // This is only used on the client side
  collapse: boolean; // This is only used on the client side


  public get percentageShares(): number {
    let percent = 0;
    if (this.parentLinceseeChangeLog && this.parentLinceseeChangeLog.totalSharesNew && this.numberofSharesNew) {
      percent = this.numberofSharesNew / this.parentLinceseeChangeLog.totalSharesNew * 100;
      percent = Math.round(percent * 100) / 100; // round to two decimal places
    }
    return percent;
  }


  public get totalChildShares(): number {
    let totalShares = 0;
    if (this.children && this.children.length) {
      totalShares = this.children.map(v => v.numberofSharesNew || 0).reduce((previous, current) => previous + current);
    }
    return totalShares;
  }

  public get keyPersonnelChildren(): LicenseeChangeLog[] {
    const leaders = (this.children || []).filter(item => item.isIndividual && !item.isShareholderNew);
    return leaders;
  }

  public get individualShareholderChildren(): LicenseeChangeLog[] {
    const leaders = (this.children || []).filter(item => item.isIndividual && item.isShareholderNew);
    return leaders;
  }

  public get businessShareholderChildren(): LicenseeChangeLog[] {
    const leaders = (this.children || []).filter(item => !item.isIndividual && item.isShareholderNew);
    return leaders;
  }

  public static GetKeyPersonnelDecendents(changeLog: LicenseeChangeLog): LicenseeChangeLog[] {
    let children = changeLog.children || [];
    let leaders = children.filter(item => item.isIndividual && !item.isShareholderNew && item.changeType !== 'unchanged');
    children.forEach(child => {
      leaders = leaders.concat(LicenseeChangeLog.GetKeyPersonnelDecendents(child));
    });
    return leaders;
  }

  public static HasChanges(changeLog: LicenseeChangeLog): Boolean {
    return this.GetKeyPersonnelDecendents(changeLog).length > 0 || this.GetBusinessShareholderDecendents(changeLog).length > 0 || this.GetIndividualShareholderDecendents(changeLog).length > 0;
  }

  public static GetIndividualShareholderDecendents(changeLog: LicenseeChangeLog): LicenseeChangeLog[] {
    let children = changeLog.children || [];
    let shareholders = children.filter(item => item.isIndividual && item.isShareholderNew && item.changeType !== 'unchanged');
    children.forEach(child => {
      shareholders = shareholders.concat(LicenseeChangeLog.GetIndividualShareholderDecendents(child));
    });
    return shareholders;
  }

  public static GetBusinessShareholderDecendents(changeLog: LicenseeChangeLog): LicenseeChangeLog[] {
    let children = changeLog.children || [];
    let shareholders = children.filter(item => !item.isIndividual && item.isShareholderNew && item.changeType !== 'unchanged');
    children.forEach(child => {
      shareholders = shareholders.concat(LicenseeChangeLog.GetBusinessShareholderDecendents(child));
    });
    return shareholders;
  }

  // construct file name prefix from name and names of parents
  public get fileUploadPrefix(): string {
    let prefix = this.nameToFilePrefix();
    // let parent = this.parentLinceseeChangeLog;
    // while (parent) {
    //   prefix = `${parent.nameToFilePrefix()} ${prefix}`;
    //   parent = parent.parentLinceseeChangeLog;
    // }

    return prefix;
  }

  // construct file name prefix from name
  private nameToFilePrefix(): string {
    const MAX_SIZE = 12;
    let prefix = '';
    if (this.isIndividual) {
      prefix = `${this.firstNameNew} ${this.lastNameNew}`;
    } else {
      if ((this.businessNameNew || '').length <= MAX_SIZE) {
        prefix = this.businessNameNew || '';
      } else {
        const length = (this.businessNameNew || '').length;
        // First 8 Characters + Last 4 characters, unless name is less than 12 characters, then show whole name
        prefix = this.businessNameNew.substring(0, 8) + this.businessNameNew.substring(length - 4);
      }
    }
    return prefix;
  }

  /**
   * Create from LegalEntity
   */
  constructor(legalEntity: LegalEntity = null) {
    if (legalEntity) {
      this.legalEntityId = legalEntity.id;
      this.businessAccountId = legalEntity.shareholderAccountId;
      this.businessType = legalEntity.legalentitytype;
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
      if (legalEntity.isApplicant){
        this.businessAccountId = legalEntity.accountId;
      } else {
        this.parentBusinessAccountId = legalEntity.accountId;
        this.businessAccountId = legalEntity.shareholderAccountId;
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
      this.phsLink = legalEntity.phsLink;
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
      newNode.children.sort((a, b) => {
        return a.totalSharesNew - b.totalSharesNew;
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

        change.isIndividual = change.isIndividualFromChangeType();
        change.children = node.children; //do not overide
        change.isRoot = node.isRoot; //do not overide
        change.parentLinceseeChangeLog = node.parentLinceseeChangeLog; // do not overide
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
        newNode.parentLinceseeChangeLog = node;
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
        newNode.parentLinceseeChangeLog = node;
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
    * Finds a nodes in the tree where the compare predicate returns true
    * @param node 'Node in tree to search from'
    * @param compareFn 'a predicate to search for a node by
    */
  static findNodesInTree(node: LicenseeChangeLog, compareFn: (node: LicenseeChangeLog) => boolean): LicenseeChangeLog[] {
    let result = [];

    if (node) {
      if (compareFn(node)) {
        result.push(node);
      }
      const children = node.children || [];
      for (const child of children) {
        const res = LicenseeChangeLog.findNodesInTree(child, compareFn);
        result = result.concat(res);
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
    const result = this.changeType === LicenseeChangeType.updateLeadership
      || this.changeType === LicenseeChangeType.updateBusinessShareholder
      || this.changeType === LicenseeChangeType.updateIndividualShareholder;
    return result;
  }

  /**
   * Returns true if the change type is a delete. Otherwise it returs false
   * @param node 'A LicenseeChangeLog'
   */
  isRemoveChangeType(): boolean {
    const result = this.changeType === LicenseeChangeType.removeLeadership
      || this.changeType === LicenseeChangeType.removeBusinessShareholder
      || this.changeType === LicenseeChangeType.removeIndividualShareholder;
    return result;
  }

  getChageTypeVerb() {
    let change = '';
    if (this.isAddChangeType()) {
      change = 'Added';
    } else if (this.isRemoveChangeType()) {
      change = 'Removed';
    } else if (this.isUpdateChangeType()) {
      change = 'Updated';
    }
    return change;
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
          // || (!node.isIndividualFromChangeType() && node.businessNameNew !== node.businessNameOld)
        );
      return isNameChange;
    };
    const result = !!this.findNodeInTree(treeRoot, nameWasChanged);
    return result;
  }

  public static getIndivialNameChanges(treeRoot: LicenseeChangeLog): LicenseeChangeLog[] {
    const nameWasChanged = (node: LicenseeChangeLog) => {
      const isNameChange = node.isUpdateChangeType() &&
        (
          // check for individual name change
          node.isIndividualFromChangeType() &&
          (node.firstNameNew !== node.firstNameOld || node.lastNameNew !== node.lastNameOld)
        );
      return isNameChange;
    };
    const result = LicenseeChangeLog.findNodesInTree(treeRoot, nameWasChanged);
    return result;
  }

  public static getNewShareholderOrganizations(treeRoot: LicenseeChangeLog): LicenseeChangeLog[] {
    const isOrganizationShareholder = (node: LicenseeChangeLog) => node.changeType === LicenseeChangeType.addBusinessShareholder;
    const result = LicenseeChangeLog.findNodesInTree(treeRoot, isOrganizationShareholder);
    return result;
  }

  public static getListNeedingSupportingDocument(treeRoot: LicenseeChangeLog): DocumentGroup {
    const result = new DocumentGroup();

    //notice of articles predicate
    const needsNoticeOfArticels = (node: LicenseeChangeLog): boolean =>
      (
        (
          (treeRoot.businessType === 'PublicCorporation' || treeRoot.businessType === 'PrivateCorporation') &&
          (
            node.changeType === LicenseeChangeType.addLeadership ||
            node.changeType === LicenseeChangeType.removeLeadership ||
            node.changeType === LicenseeChangeType.updateLeadership
          )
        )
      );
    result.noticeOfArticles = LicenseeChangeLog.findNodesInTree(treeRoot, needsNoticeOfArticels);

    //central securities register predicate
    const needsCentralSecuritiesRegister = (node: LicenseeChangeLog): boolean =>
      (treeRoot.businessType === 'PrivateCorporation' &&
        (
          node.changeType === LicenseeChangeType.addBusinessShareholder ||
          node.changeType === LicenseeChangeType.removeBusinessShareholder ||
          node.changeType === LicenseeChangeType.updateIndividualShareholder ||
          node.changeType === LicenseeChangeType.addIndividualShareholder ||
          node.changeType === LicenseeChangeType.removeIndividualShareholder ||
          node.changeType === LicenseeChangeType.updateIndividualShareholder
        )
      );
    result.centralSecuritiesResgister = LicenseeChangeLog.findNodesInTree(treeRoot, needsCentralSecuritiesRegister);

    //shareholder list predicate
    const needsShareholderRecord = (node: LicenseeChangeLog): boolean =>
      (treeRoot.businessType === 'PublicCorporation' &&
        (
          node.changeType === LicenseeChangeType.addBusinessShareholder ||
          node.changeType === LicenseeChangeType.removeBusinessShareholder ||
          node.changeType === LicenseeChangeType.updateIndividualShareholder ||
          node.changeType === LicenseeChangeType.addIndividualShareholder ||
          node.changeType === LicenseeChangeType.removeIndividualShareholder ||
          node.changeType === LicenseeChangeType.updateIndividualShareholder
        )
      );
    result.shareholderList = LicenseeChangeLog.findNodesInTree(treeRoot, needsShareholderRecord);

    //partnership agreement
    const needsParnershipAgreement = (node: LicenseeChangeLog): boolean =>
      (treeRoot.businessType === 'Partnership' &&
        (
          node.changeType === LicenseeChangeType.addBusinessShareholder ||
          node.changeType === LicenseeChangeType.removeBusinessShareholder ||
          node.changeType === LicenseeChangeType.updateIndividualShareholder ||
          node.changeType === LicenseeChangeType.addIndividualShareholder ||
          node.changeType === LicenseeChangeType.removeIndividualShareholder ||
          node.changeType === LicenseeChangeType.updateIndividualShareholder
        )
      );
    result.partnershipAgreement = LicenseeChangeLog.findNodesInTree(treeRoot, needsParnershipAgreement);

    return result;
  }

  public static getNewShareholderDocumentList(treeRoot: LicenseeChangeLog): DocumentGroup {
    const result = new DocumentGroup();

    //notice of articles predicate
    const needsNoticeOfArticels = (node: LicenseeChangeLog): boolean =>
      (
        (node.businessType === 'PublicCorporation' || node.businessType === 'PrivateCorporation') &&
        node.changeType === LicenseeChangeType.addBusinessShareholder
      );
    result.noticeOfArticles = LicenseeChangeLog.findNodesInTree(treeRoot, needsNoticeOfArticels);

    //central securities register predicate
    const needsCentralSecuritiesRegister = (node: LicenseeChangeLog): boolean =>
      (
        node.businessType === 'PrivateCorporation' &&
        node.changeType === LicenseeChangeType.addBusinessShareholder
      );
    result.centralSecuritiesResgister = LicenseeChangeLog.findNodesInTree(treeRoot, needsCentralSecuritiesRegister);

    //shareholder list predicate
    const needsShareholderRecord = (node: LicenseeChangeLog): boolean =>
      (
        node.businessType === 'PublicCorporation' &&
        node.changeType === LicenseeChangeType.addBusinessShareholder
      );
    result.shareholderList = LicenseeChangeLog.findNodesInTree(treeRoot, needsShareholderRecord);

    //partnership agreement
    const needsParnershipAgreement = (node: LicenseeChangeLog): boolean =>
      (
        node.businessType === 'Partnership' &&
        node.changeType === LicenseeChangeType.addBusinessShareholder
      );
    result.partnershipAgreement = LicenseeChangeLog.findNodesInTree(treeRoot, needsParnershipAgreement);

    //trust agreement
    const needsTrustAgreement = (node: LicenseeChangeLog): boolean =>
      (
        node.businessType === 'Trust' &&
        node.changeType === LicenseeChangeType.addBusinessShareholder
      );
    result.trustAgreement = LicenseeChangeLog.findNodesInTree(treeRoot, needsTrustAgreement);

    //society documents
    const needsSocietyDocuments = (node: LicenseeChangeLog): boolean =>
      (
        node.businessType === 'Society' &&
        node.changeType === LicenseeChangeType.addBusinessShareholder
      );
    result.societyDocuments = LicenseeChangeLog.findNodesInTree(treeRoot, needsSocietyDocuments);

    return result;
  }

  public static getNewIndividualAssociateList(treeRoot: LicenseeChangeLog): LicenseeChangeLog[] {
    let result = [];

    //new individual associate predicate
    const newIndividualAssociate = (node: LicenseeChangeLog): boolean =>
      (
        node.changeType === LicenseeChangeType.addIndividualShareholder
      );
    result = LicenseeChangeLog.findNodesInTree(treeRoot, newIndividualAssociate);
    return result;
  }
}

class DocumentGroup {
  noticeOfArticles: LicenseeChangeLog[] = [];
  centralSecuritiesResgister: LicenseeChangeLog[] = [];
  shareholderList: LicenseeChangeLog[] = [];
  partnershipAgreement: LicenseeChangeLog[] = [];
  trustAgreement: LicenseeChangeLog[] = [];
  societyDocuments: LicenseeChangeLog[] = [];

  get changeLogsPresent(): boolean {
    const allDocsCount = this.noticeOfArticles.length + this.centralSecuritiesResgister.length +
      this.shareholderList.length + this.partnershipAgreement.length;
    return allDocsCount > 0;
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
