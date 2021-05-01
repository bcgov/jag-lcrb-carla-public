import { Component, EventEmitter, Input, OnInit, Output, ViewEncapsulation } from '@angular/core';
import { faBell } from '@fortawesome/free-solid-svg-icons';
import { Account } from "@models/account.model";
import { User } from '@models/user.model';

@Component({
  selector: 'app-user-nav',
  templateUrl: './user-nav.component.html',
  encapsulation: ViewEncapsulation.None
})
export class UserNavComponent implements OnInit {
  // icons
  faBell = faBell;

  @Input() currentUser: User;
  @Input() account: Account;
  @Input() showNoticesBadge = false;
  @Input() showMessageCenterBadge = false;
  @Output() messageCenterClick = new EventEmitter<void>();

  constructor() { }
  ngOnInit() { }
}
