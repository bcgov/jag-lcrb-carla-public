import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { LicenseeChangeLog } from '@models/licensee-change-log.model';
import { Account } from '@models/account.model';

@Component({
  selector: 'app-org-structure',
  templateUrl: './org-structure.component.html',
  styleUrls: ['./org-structure.component.scss']
})
export class OrgStructureComponent implements OnInit {
  @Input() node: LicenseeChangeLog;
  @Input() account: Account;
  @Output() deletedChanges: EventEmitter<LicenseeChangeLog> = new EventEmitter<LicenseeChangeLog>();

  constructor() { }

  ngOnInit() {
  }

  asLicenseeChangeLog(val): LicenseeChangeLog { return val; }

  addChild(node: LicenseeChangeLog, child: LicenseeChangeLog, changeType: string) {
    node.children = node.children || [];
    if (changeType === 'AddLeadership') {
      child.isShareholderNew = false;
      child.isIndividual = true
    } else if (changeType === 'AddIndividualShareholder') {
      child.isShareholderNew = true;
      child.isIndividual = true
    } else if (changeType === 'AddBusinessShareholder') {
      child.isShareholderNew = true;
      child.isIndividual = false
    }
    node.children.push(child);
  }

}
