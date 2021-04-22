import { Component, Input, OnInit, ViewEncapsulation } from '@angular/core';
import { User } from '@models/user.model';
import { faUserCircle, faBuilding } from "@fortawesome/free-regular-svg-icons";
import { faChevronDown, faChevronUp } from "@fortawesome/free-solid-svg-icons";

@Component({
  selector: 'app-user-menu',
  templateUrl: './user-menu.component.html',
  styleUrls: ['./user-menu.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class UserMenuComponent implements OnInit {
  // icons
  faUserCircle = faUserCircle;
  faBuilding = faBuilding;
  faChevronDown = faChevronDown;
  faChevronUp = faChevronUp;

  @Input() currentUser: User;

  get userIcon() {
    if (this.currentUser?.userType == 'Business') {
      return this.faBuilding;
    }
    return this.faUserCircle;
  }

  constructor() { }

  ngOnInit() {
  }

}
