import { Component, EventEmitter, Input, OnInit, Output, ViewEncapsulation } from '@angular/core';
import { Account } from "@models/account.model";
import { User } from '@models/user.model';
import { FeatureFlagService } from '@services/feature-flag.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class NavbarComponent implements OnInit {
  @Input() currentUser: User;
  @Input() account: Account;
  @Input() showNoticesBadge = false;
  @Input() showMessageCenterBadge = false;
  @Output() messageCenterClick = new EventEmitter<void>();

  showMapLink = false;
  isAssociate = false;
  sepFeatureOn = false;

  get isAnonymous() {
    return !this.currentUser;
  }
  get isAuthenticated() {
    return this.currentUser && !this.isAssociate;
  }
  get isPoliceRepresentative() {
    return this.sepFeatureOn && this.isAuthenticated && this.account?.businessType === 'Police';
  }

  constructor(public featureFlagService: FeatureFlagService) { }

  ngOnInit() {
    this.featureFlagService.featureOn("Maps").subscribe(value => this.showMapLink = value);
    this.featureFlagService.featureOn("Sep").subscribe(value => this.sepFeatureOn = value);
  }
}
