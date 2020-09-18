import { Component, OnInit, Input, Output, EventEmitter, QueryList, ViewChildren, ChangeDetectorRef } from '@angular/core';
import { LicenseeChangeLog } from '@models/licensee-change-log.model';
import { FormBuilder, Validators, FormArray, FormGroup, FormControl } from '@angular/forms';
import { FormBase } from '@shared/form-base';
import { Account } from '@models/account.model';
import { MatSnackBar } from '@angular/material';
import { of, Observable, forkJoin } from 'rxjs';
import { mergeMap } from 'rxjs/operators';
import { OrgStructureComponent } from '../org-structure/org-structure.component';

@Component({
  selector: 'app-associate-list',
  templateUrl: './associate-list.component.html',
  styleUrls: ['./associate-list.component.scss']
})
export class AssociateListComponent extends FormBase implements OnInit {
  @Input() rootNode: LicenseeChangeLog;
  @Input() account: Account;
  @Input() licencesOnFile: boolean;
  @Input() isReadOnly: boolean;
  @Input() changeTypeSuffix: string;
  @Input() addLabel: string = 'Add Associate';
  @Output() childAdded = new EventEmitter<LicenseeChangeLog>();
  items: LicenseeChangeLog[] = [];
  @Input('personalHistoryItems') set personalHistoryItems(value: LicenseeChangeLog[]) {
    this.items = value || [];
  };
  @Output() changesMade = new EventEmitter<boolean>();
  @ViewChildren('orgStructure') orgStructureList: QueryList<OrgStructureComponent>;

  LicenseeChangeLog = LicenseeChangeLog;
  busy: any;
  YearsAgo19: Date;

  public get associates(): FormArray {
    return this.form.get('associates') as FormArray;
  }

  constructor(private fb: FormBuilder,
    private cd: ChangeDetectorRef,
    public snackBar: MatSnackBar) {
    super();
    this.YearsAgo19 = new Date();
    this.YearsAgo19.setFullYear(this.YearsAgo19.getFullYear() - 19);
  }

  ngOnInit() {
    this.form = this.fb.group({
      associates: this.fb.array([])
    });

    this.items.forEach(item => {
      this.addFormArray(item);
    });
  }

  asLicenseeChangeLog(val): LicenseeChangeLog { return Object.assign(new LicenseeChangeLog(), val); }

  addFormArray(item: LicenseeChangeLog = null) {
    item = item || <LicenseeChangeLog>{};
    const group = this.fb.group({
      id: [''],
      changeType: [''],
      isDirectorNew: [''],
      isOwnerNew: [''],
      isOwnerOld: [''],
      isDirectorOld: [''],
      isManagerNew: [''],
      isManagerOld: [''],
      isOfficerNew: [''],
      isOfficerOld: [''],
      isShareholderNew: [''],
      isShareholderOld: [''],
      isTrusteeNew: [''],
      isTrusteeOld: [''],
      businessType: [''],
      numberofSharesNew: [''],
      numberofSharesOld: [''],
      numberOfNonVotingSharesNew: [''],
      numberOfNonVotingSharesOld: [''],
      totalSharesNew: [''],
      totalSharesOld: [''],
      emailNew: [''],
      emailOld: [''],
      firstNameNew: [''],
      firstNameOld: [''],
      lastNameNew: [''],
      lastNameOld: [''],
      LicenseeChangelogid: [''],
      businessNameNew: [''],
      businessNameOld: [''],
      nameOld: [''],
      dateofBirthNew: [''],
      dateofBirthOld: [''],
      titleNew: [''],
      titleOld: [''],
      applicationId: [''],
      applicationType: [''],
      legalEntityId: [''],
      parentLegalEntityId: [''],
      parentLicenseeChangeLogId: [''],
      parentBusinessAccountId: [''],
      businessAccountId: [''],
      children: [''],
      parentLinceseeChangeLog: [''],
      interestPercentageNew: [''],
      interestPercentageOld: [''],
      phsLink: [''],
      isContactComplete: [''],
      isRoot: [''],
      isIndividual: [''],
      edit: [''],
      collapse: [''],
      isLeadershipIndividual: [''],
      isShareholderIndividual: [''],
      saved: [false]
    });

    if (this.changeTypeSuffix === 'Leadership') {
      group.get('firstNameNew').setValidators([Validators.required]);
      group.get('lastNameNew').setValidators([Validators.required]);
      group.get('emailNew').setValidators([Validators.required, Validators.email]);
      group.get('dateofBirthNew').setValidators([Validators.required]);
      // these validators are not required for SoleProps because they're always owners
      if (this.rootNode.businessType !== 'SoleProprietorship' && this.rootNode.businessType !== 'Trust') {
        group.get('isDirectorNew').setValidators([this.requiredCheckboxGroupValidator(['isDirectorNew', 'isOfficerNew', 'isManagerNew'])]);
        group.get('isOfficerNew').setValidators([this.requiredCheckboxGroupValidator(['isDirectorNew', 'isOfficerNew', 'isManagerNew'])]);
        group.get('isManagerNew').setValidators([this.requiredCheckboxGroupValidator(['isDirectorNew', 'isOfficerNew', 'isManagerNew'])]);
      }
    }


    if (this.changeTypeSuffix === 'Trust') {
      group.get('firstNameNew').setValidators([Validators.required]);
      group.get('lastNameNew').setValidators([Validators.required]);
      group.get('emailNew').setValidators([Validators.required, Validators.email]);
      group.get('dateofBirthNew').setValidators([Validators.required]);
    }

    if (this.changeTypeSuffix === 'IndividualShareholder') {
      group.get('firstNameNew').setValidators([Validators.required]);
      group.get('lastNameNew').setValidators([Validators.required]);
      group.get('emailNew').setValidators([Validators.required, Validators.email]);
      group.get('dateofBirthNew').setValidators([Validators.required]);
      if (this.rootNode.businessType === 'Partnership') {
        group.get('interestPercentageNew').setValidators([Validators.required]);
      } else {
        group.get('numberofSharesNew').setValidators([this.requireOneOfGroupValidator(['numberofSharesNew' , 'numberOfNonVotingSharesNew'])]);
        group.get('numberOfNonVotingSharesNew').setValidators([this.requireOneOfGroupValidator(['numberofSharesNew' , 'numberOfNonVotingSharesNew'])]);
      }
    }

    if (this.changeTypeSuffix === 'BusinessShareholder') {
      group.get('businessNameNew').setValidators([Validators.required]);
      group.get('businessType').setValidators([Validators.required]);
      if (this.rootNode.businessType === 'Partnership') {
        group.get('interestPercentageNew').setValidators([Validators.required]);
      } else {
        group.get('numberofSharesNew').setValidators([this.requireOneOfGroupValidator(['numberofSharesNew' , 'numberOfNonVotingSharesNew'])]);
        group.get('numberOfNonVotingSharesNew').setValidators([this.requireOneOfGroupValidator(['numberofSharesNew' , 'numberOfNonVotingSharesNew'])]);
      }
      group.get('emailNew').setValidators([Validators.required, Validators.email]);
    }

    group.patchValue(item);
    this.associates.push(group);
  }

  isPositionValid(item: FormGroup): boolean {
    item.get('isDirectorNew').updateValueAndValidity()
    item.get('isOfficerNew').updateValueAndValidity()
    item.get('isManagerNew').updateValueAndValidity()

    let valid = (!item.get('isDirectorNew').touched || item.get('isDirectorNew').valid)
      && (!item.get('isOfficerNew').touched || item.get('isOfficerNew').valid)
      && (!item.get('isManagerNew').touched || item.get('isManagerNew').valid);
    return valid;
  }

  getNewLeadershipPosition(item) {
    item = item || <LicenseeChangeLog>{}
    let changeLog = new LicenseeChangeLog()
    let change = Object.assign(changeLog, item) as LicenseeChangeLog;
    return change.getNewLeadershipPosition();
  }

  addAssociate() {
    const associate = new LicenseeChangeLog();
    associate.changeType = `add${this.changeTypeSuffix}`;
    associate.parentLicenseeChangeLog = this.rootNode;
    associate.edit = true;
    associate.collapse = true;
    this.childAdded.emit(associate);
    this.addFormArray(associate);
  }

  saveLog(index: number): Observable<boolean> {
    for(var item in (<FormGroup>this.associates.at(index)).controls){
      this.associates.at(index).get(item).updateValueAndValidity();
    }

    const valid = this.associates.at(index).valid;
    this
    const patchValue = <LicenseeChangeLog>{};
    let value = this.associates.at(index).value;
    value = Object.assign(new LicenseeChangeLog, value);
    let saved = false;

    if (value.isRemoveChangeType()) {
      saved = true;
    } else {
      if (valid) {
        value = Object.assign(new LicenseeChangeLog(), value || {}) as LicenseeChangeLog;
        if (!value.isAddChangeType() && value.someFieldsHaveChanged()) {
          patchValue.changeType = `update${this.changeTypeSuffix}`;;
        }
        patchValue.edit = false;
        patchValue.saved = true;

        if (this.changeTypeSuffix === 'Leadership') {
          patchValue.isIndividual = true;
          patchValue.isLeadershipIndividual = true;
          // check to see if this is a sole prop.
          if (this.rootNode.businessType === 'SoleProprietorship') {
            patchValue.isOwnerNew = true;
          }
          // check to see if this is a trust.
          if (this.rootNode.businessType === 'Trust') {
            patchValue.isTrusteeNew = true;
          }

        } else if (this.changeTypeSuffix === 'IndividualShareholder') {
          patchValue.isIndividual = true;
          patchValue.isShareholderIndividual = true;
          patchValue.isShareholderNew = true;
        } else if (this.changeTypeSuffix === 'BusinessShareholder') {
          patchValue.isIndividual = false;
          patchValue.isShareholderNew = true;
        }


        this.associates.at(index).patchValue(patchValue);
        this.changesMade.next(true);
        saved = true;
      } else {
        // put associate into edit mode to show validation errors
        this.associates.at(index).get('edit').setValue(true);
        // mark all contols as touched to show validation rules
        const controls = (<FormGroup>(this.associates.at(index))).controls;
        for (let control in controls) {
          if (control) {
            (controls[control] as FormControl).markAsTouched();
          }
        }
      }
    }
    return of(saved);
  }

  /**
   * saves all open list items
   * returns an Observable<boolean>. False means there is validation errors
   */
  saveAll(): Observable<boolean> {
    let saveResults = [];
    this.associates.getRawValue().forEach((value, index) => {
      saveResults.push(this.saveLog(index));
    });

    // save all org structure children
    if (this.orgStructureList.length > 0) {
      this.orgStructureList.forEach(org => {
        saveResults.push(org.saveAll());
      });
    }
    if (saveResults.length > 0) {
      return forkJoin(...saveResults)
        .pipe(mergeMap(results => {
          return of(results.indexOf(false) === -1);
        }));
    }
    else {
      return of(true);
    }
  }

  getData(): LicenseeChangeLog[] {
    let res = [];
    const controls = this.associates.controls;
    for (let control in controls) {
      let value = controls[control].value;
      if (control) {
        // check for children
        let childOrgs = this.orgStructureList.filter(item => item.parentAssociate === controls[control]);
        if (childOrgs.length) {
          let childOrg = childOrgs[0];
          value = childOrg.getData();
        }
        res.push(value);
      }
    }

    return res;
  }

  deleteChange(node: LicenseeChangeLog, index: number) {
    node.businessNameNew = node.nameOld;
    node.isDirectorNew = node.isDirectorOld;
    node.isManagerNew = node.isManagerOld;
    node.isOfficerNew = node.isOfficerOld;
    node.isOwnerNew = node.isOwnerOld;
    node.isShareholderNew = node.isShareholderOld;
    node.isTrusteeNew = node.isTrusteeOld;
    node.numberofSharesNew = node.numberofSharesOld;
    node.numberOfNonVotingSharesNew = node.numberOfNonVotingSharesOld;
    node.totalSharesNew = node.totalSharesOld;
    node.emailNew = node.emailOld;
    node.firstNameNew = node.firstNameOld;
    node.lastNameNew = node.lastNameOld;
    node.businessNameNew = node.businessNameOld;
    node.dateofBirthNew = node.dateofBirthOld;
    node.titleNew = node.titleOld;

    if (!node.id && !node.legalEntityId) { // added but never saved to dynamics. Just delete from client side
      this.associates.removeAt(index);
    } else if (!node.isRoot && (node.id || node.legalEntityId)) { // already saved to dynamics. Update changeType. This should be deleted by the API if there is no legalEntityId
      if (this.changeTypeSuffix === 'Leadership' || this.changeTypeSuffix === 'Trustee') {
        node.changeType = 'removeLeadership';
      } else if (this.changeTypeSuffix === 'IndividualShareholder') {
        node.changeType = 'removeIndividualShareholder';
      } else if (this.changeTypeSuffix === 'BusinessShareholder') {
        node.changeType = 'removeBusinessShareholder';
      }
    }
    this.changesMade.next(true);
  }

  isNameChangePerformed(changeLog: LicenseeChangeLog): boolean {
    let res = false;
    if (changeLog) {
      changeLog = Object.assign(new LicenseeChangeLog(), changeLog);
      res = changeLog.isNameChangePerformed();
    }
    return res;
  }

  showNameChangeSection(associate): boolean {
    const show = associate && !this.asLicenseeChangeLog(associate.value).isRemoveChangeType()
      && this.licencesOnFile
      && this.isNameChangePerformed(associate.value)
      && !associate.get('edit').value;
    return show;
  }

  updateNumberOfFiles(numberOfFiles: number, docType: string, node: LicenseeChangeLog) {
    node.fileUploads[docType] = numberOfFiles;
  }

  getBusinessTypeName(typeValue: string) {
    let typeName = '';

    switch (typeValue) {
      case 'PrivateCorporation':
        typeName = 'Private Corporation';
        break;
      case 'PublicCorporation':
        typeName = 'Public Corporation';
        break;
      case 'SoleProprietorship':
        typeName = 'Sole Proprietorship';
        break;
      default:
        typeName = typeValue;
        break;
    }
    return typeName;
  }

  showPosition(): boolean {
    return this.rootNode && (
      this.rootNode.businessType === 'Society'
      || this.rootNode.businessType === 'PrivateCorporation'
      || this.rootNode.businessType === 'PublicCorporation'
      || this.rootNode.businessType === 'Church'
      || this.rootNode.businessType === 'University');
  }

  // check to see if there is a link in any child records; when set to true the Level 1 Personal History Summary column will show.
  showPHSLevel1(): boolean {
    let show = false;
    // get associates with a phsLink
    const links = this.associates.value.filter(item => !!item.phsLink && !item.isRemoveChangeType());
    if (links.length > 0) {
      show = true;
    }
    return show;
  }

}
