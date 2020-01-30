import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { LicenseeChangeLog } from '@models/licensee-change-log.model';
import { FormBuilder, Validators, FormArray, FormGroup, FormControl } from '@angular/forms';
import { FormBase } from '@shared/form-base';
import { Account } from '@models/account.model';
import { MatSnackBar } from '@angular/material';

@Component({
  selector: 'app-associate-list',
  templateUrl: './associate-list.component.html',
  styleUrls: ['./associate-list.component.scss']
})
export class AssociateListComponent extends FormBase implements OnInit {
  @Input() rootNode: LicenseeChangeLog;
  @Input() account: Account;
  @Input() changeTypeSuffix: string;
  @Input() addLabel: string = 'Add Associate';
  businessType: string = 'Society';
  @Output() childAdded = new EventEmitter<LicenseeChangeLog>();
  items: LicenseeChangeLog[] = [];
  @Input('personalHistoryItems') set personalHistoryItems(value: LicenseeChangeLog[]) {
    this.items = value || [];
  };
  @Output() personalHistoryItemsChange = new EventEmitter<LicenseeChangeLog[]>();

  LicenseeChangeLog = LicenseeChangeLog;
  busy: any;
  YearsAgo19: Date;

  public get associates(): FormArray {
    return this.form.get('associates') as FormArray;
  }

  constructor(private fb: FormBuilder,
    public snackBar: MatSnackBar, ) {
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
      parentLinceseeChangeLogId: [''],
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
      saved: [false]
    });

    if (this.changeTypeSuffix === 'Leadership') {
      group.get('firstNameNew').setValidators([Validators.required]);
      group.get('lastNameNew').setValidators([Validators.required]);
      group.get('emailNew').setValidators([Validators.required, Validators.email]);
      group.get('dateofBirthNew').setValidators([Validators.required]);
      group.get('isDirectorNew').setValidators([this.requiredCheckboxGroupValidator(['isDirectorNew', 'isOfficerNew', 'isManagerNew'])]);
      group.get('isOfficerNew').setValidators([this.requiredCheckboxGroupValidator(['isDirectorNew', 'isOfficerNew', 'isManagerNew'])]);
      group.get('isManagerNew').setValidators([this.requiredCheckboxGroupValidator(['isDirectorNew', 'isOfficerNew', 'isManagerNew'])]);
    }

    if (this.changeTypeSuffix === 'IndividualShareholder') {
      group.get('firstNameNew').setValidators([Validators.required]);
      group.get('lastNameNew').setValidators([Validators.required]);
      group.get('emailNew').setValidators([Validators.required, Validators.email]);
      group.get('dateofBirthNew').setValidators([Validators.required]);
      group.get('interestPercentageNew').setValidators([Validators.required]);
    }

    if (this.changeTypeSuffix === 'BusinessShareholder') {
      group.get('businessNameNew').setValidators([Validators.required]);
      group.get('businessType').setValidators([Validators.required]);
      group.get('interestPercentageNew').setValidators([Validators.required]);
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
    associate.parentLinceseeChangeLog = this.rootNode;
    associate.edit = true;
    associate.collapse = true;
    this.childAdded.emit(associate);
    this.addFormArray(associate);
  }

  emitValue() {
    let value = [];
    const controls = this.associates.controls;
    for (let control in controls) {
      if (control) {
        value.push(controls[control].value);
      }
    }
    this.personalHistoryItemsChange.emit(value);
  }

  saveLog(item: LicenseeChangeLog, index: number) {
    const valid = this.associates.at(index).valid;
    if (valid) {
      item = Object.assign(new LicenseeChangeLog(), item || {}) as LicenseeChangeLog;
      if (!item.isAddChangeType()) {
        item.changeType = `update${this.changeTypeSuffix}`;
      }
      this.associates.at(index).get('edit').setValue(false);
      this.associates.at(index).get('saved').setValue(true);

      if (this.changeTypeSuffix === 'Leadership') {
        this.associates.at(index).get('isIndividual').setValue(true);
      } else if (this.changeTypeSuffix === 'IndividualShareholder') {
        this.associates.at(index).get('isIndividual').setValue(true);
        this.associates.at(index).get('isShareholderNew').setValue(true);
      } else if (this.changeTypeSuffix === 'BusinessShareholder') {
        this.associates.at(index).get('isIndividual').setValue(false);
        this.associates.at(index).get('isShareholderNew').setValue(true);
      }

      this.emitValue();
    } else {
      // mark all contols as touched to show validation rules
      const controls = (<FormGroup>(this.associates.at(index))).controls;
      for (let control in controls) {
        if (control) {
          (controls[control] as FormControl).markAsTouched();
        }
      }
    }
  }

  // Copy value to clipboard
  copyMessage(value: string) {
    const selBox = document.createElement('textarea');
    selBox.style.position = 'fixed';
    selBox.style.left = '0';
    selBox.style.top = '0';
    selBox.style.opacity = '0';
    selBox.value = value;
    document.body.appendChild(selBox);
    selBox.focus();
    selBox.select();
    document.execCommand('copy');
    document.body.removeChild(selBox);
    this.snackBar.open('The link is copied to the clipboard', '', { duration: 2500, panelClass: ['green-snackbar'] });
  }

  deleteChange(node: LicenseeChangeLog, index: number) {
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

    if (!node.id && !node.legalEntityId) { // added but never saved to dynamics. Just delete from client side
      this.associates.removeAt(index);
    } else if (!node.isRoot && (node.id || node.legalEntityId)) { // already saved to dynamics. Update changeType. This should be deleted by the API if there is no legalEntityId
      if (this.changeTypeSuffix === 'Leadership') {
        node.changeType = 'removeLeadership';
      } else if (this.changeTypeSuffix === 'IndividualShareholder') {
        node.changeType = 'removeIndividualShareholder';
      } else if (this.changeTypeSuffix === 'BusinessShareholder') {
        node.changeType = 'removeBusinessShareholder';
      }
    }
    this.emitValue();
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
      case 'SoleProprietor':
        typeName = 'Sole Proprietor';
        break;
      default:
        typeName = typeValue;
        break;
    }
    return typeName;
  }

  showPosition(): boolean {
    return this.businessType === 'Society'
      || this.businessType === 'PublicCorporation';
  }

  showPHSLevel1(item: LicenseeChangeLog): boolean {
    let show = this.rootNode.children.length > 0;

    return show;
  }

}
