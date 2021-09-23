import { Component, EventEmitter, Input, OnInit, Output, ViewEncapsulation } from '@angular/core';
import { faBell } from '@fortawesome/free-solid-svg-icons';
import { Account } from "@models/account.model";
import { User } from '@models/user.model';

@Component({
  selector: 'app-user-nav',
  templateUrl: './user-nav.component.html',
  styleUrls: ['./user-nav.component.scss']
})
export class UserNavComponent implements OnInit {
  // icons
  faBell = faBell;
  isNavbarCollapsed = false;
  @Input() currentUser: User;
  @Input() account: Account;
  @Input() showNoticesBadge = false;
  @Input() showMessageCenterBadge = false;
  @Output() messageCenterClick = new EventEmitter<void>();

  constructor() { }
  ngOnInit() { }
}
