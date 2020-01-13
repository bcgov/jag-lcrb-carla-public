import { Component, OnInit, Input } from '@angular/core';
import { LicenseeChangeLog } from '@models/licensee-change-log.model';

@Component({
  selector: 'app-org-structure',
  templateUrl: './org-structure.component.html',
  styleUrls: ['./org-structure.component.scss']
})
export class OrgStructureComponent implements OnInit {
  @Input() node: LicenseeChangeLog;

  constructor() { }

  ngOnInit() {
  }

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
