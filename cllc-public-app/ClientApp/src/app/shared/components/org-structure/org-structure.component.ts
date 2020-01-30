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

  asLicenseeChangeLog(val): LicenseeChangeLog { return val}; 

  updateChildred(children: LicenseeChangeLog[], changeType: string) {
    children = children || [];
    this.node.children = this.node.children || [];
    if (changeType === 'Leadership') {
      this.node.children = [...children, 
        ...this.node.individualShareholderChildren,  
        ...this.node.businessShareholderChildren
      ];
    } else if (changeType === 'IndividualShareholder') {
      this.node.children = [...children, 
        ...this.node.keyPersonnelChildren,  
        ...this.node.businessShareholderChildren
      ];
    } else if (changeType === 'BusinessShareholder') {
      this.node.children = [...children, 
        ...this.node.individualShareholderChildren,  
        ...this.node.keyPersonnelChildren
      ];
    }
  }
}
