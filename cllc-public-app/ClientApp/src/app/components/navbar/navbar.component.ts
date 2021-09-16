import { Component, EventEmitter, Input, OnInit, Output, ViewEncapsulation } from '@angular/core';
import { Account } from "@models/account.model";
import { User } from '@models/user.model';
import { FeatureFlagService } from '@services/feature-flag.service';
import { combineLatest } from 'rxjs';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
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
  dataLoaded = false;

  get isAnonymous() {
    return !this.currentUser;
  }
  get isAuthenticated() {
    return this.currentUser && !this.isAssociate;
  }
  get isPoliceRepresentative() {
    return this.isAuthenticated && this.account.businessType == "Police";
  }

  constructor(public featureFlagService: FeatureFlagService) { }

  ngOnInit() {
    const flag1 = this.featureFlagService.featureOn("Maps");
    const flag2 = this.featureFlagService.featureOn("Sep");
    combineLatest([flag1, flag2]).subscribe(([maps, sep]) => {
      this.showMapLink = maps;
      this.sepFeatureOn = sep;
      this.dataLoaded = true;
    });
  }
}
