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
  Account = Account;

  constructor() { }

  ngOnInit() {
  }

  asLicenseeChangeLog(val): LicenseeChangeLog { return val; }

  updateChildred(node: LicenseeChangeLog, children: LicenseeChangeLog[], changeType: string) {
    children = children || [];
    if (changeType === 'Leadership') {
      node.children = [...children, 
        ...this.asLicenseeChangeLog(node).individualShareholderChildren,  
        ...this.asLicenseeChangeLog(node).businessShareholderChildren
      ];
    } else if (changeType === 'IndividualShareholder') {
      node.children = [...children, 
        ...this.asLicenseeChangeLog(node).keyPersonnelChildren,  
        ...this.asLicenseeChangeLog(node).businessShareholderChildren
      ];
    } else if (changeType === 'BusinessShareholder') {
      node.children = [...children, 
        ...this.asLicenseeChangeLog(node).individualShareholderChildren,  
        ...this.asLicenseeChangeLog(node).keyPersonnelChildren
      ];
    }
  }
}
