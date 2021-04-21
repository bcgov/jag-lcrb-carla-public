import { Component, Input, OnInit } from '@angular/core';
import { User } from '@models/user.model';
import { faUserCircle } from "@fortawesome/free-regular-svg-icons";
import { faChevronDown, faChevronUp } from "@fortawesome/free-solid-svg-icons";

@Component({
  selector: 'app-user-menu',
  templateUrl: './user-menu.component.html',
  styleUrls: ['./user-menu.component.scss']
})
export class UserMenuComponent implements OnInit {
  // icons
  faUserCircle = faUserCircle;
  faChevronDown = faChevronDown;
  faChevronUp = faChevronUp;

  @Input() currentUser: User;

  constructor() { }

  ngOnInit() {
  }

}
