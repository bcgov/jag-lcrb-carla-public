import { Component, OnInit, ViewChild, Input } from '@angular/core';
import { User } from '../models/user.model';
import { UserDataService } from '../services/user-data.service';
import { AdoxioApplicationDataService } from '../services/adoxio-application-data.service';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { switchMap } from 'rxjs/operators';
import { Subscription } from 'rxjs';
import * as currentApplicatioActions from '../app-state/actions/current-application.action';
import { Store } from '@ngrx/store';
import { AppState } from '../app-state/models/app-state';
import { AdoxioApplication } from '../models/adoxio-application.model';

@Component({
  selector: 'app-license-application',
  templateUrl: './license-application.component.html',
  styleUrls: ['./license-application.component.scss']
})
export class LicenseApplicationComponent implements OnInit {

  applicationId: string;
  applicationName: string;
  @Input() currentUser: User;
  @Input() accountId: string;
  busy: Subscription;

  public view_tab = 'contact-details';
  public contactId: string;
  public componentLoaded: boolean;

  tabs: any = {
    application: ['contact-details', 'property-details', 'store-information', 'floor-plan', 'site-map', 'declaration', 'submit-pay'],
  };
  tabStructure: any[] = this.tabs.application;

  constructor(private applicationDataService: AdoxioApplicationDataService,
    private store: Store<AppState>,
    private userDataService: UserDataService,
    private router: Router,
    private route: ActivatedRoute) {
    this.applicationId = route.snapshot.params.applicationId;
    const urlParts = this.router.url.split('/');
    this.view_tab = urlParts[urlParts.length - 1];
  }

  ngOnInit(): void {
    this.router.events
      .subscribe((event) => {
        if (event instanceof NavigationEnd) {
          const urlParts = this.router.url.split('/');
          this.view_tab = urlParts[urlParts.length - 1];
        }
      });

    // TODO - pass currentUser in as router data rather than doing another call to getCurrentUser.
    if (!this.currentUser) {
      this.userDataService.getCurrentUser()
        .subscribe((data: User) => {
          this.currentUser = data;
          if (!this.accountId) {
            this.accountId = this.currentUser.accountid;
          }
          this.componentLoaded = true;
        });
    }

    // get application name
    if (!this.applicationName) {
      this.busy = this.applicationDataService.getApplicationById(this.applicationId).subscribe(
        (data: AdoxioApplication) => {
          this.applicationName = data.name;
          this.store.dispatch(new currentApplicatioActions.SetCurrentApplicationAction(data));
        },
        err => {
          console.log('Error occured');
        }
      );
    }

  }

  getTab() {
    const result = this.tabStructure.indexOf(this.view_tab);
    return result;
  }

  changeTab(tab) {
    this.view_tab = tab;
    this.router.navigate([`/license-application/${this.applicationId}/${tab}`]);
  }

  back() {
    let currentTab = this.getTab();
    currentTab--;
    if (currentTab < 0) {
      currentTab = this.tabStructure.length - 1;
    }
    this.view_tab = this.tabStructure[currentTab];
    this.router.navigate([`/license-application/${this.applicationId}/${this.view_tab}`]);
  }

  next() {
    let currentTab = this.getTab();
    currentTab++;
    if (currentTab >= this.tabStructure.length) {
      currentTab = 0;
    }
    this.view_tab = this.tabStructure[currentTab];
    this.router.navigate([`/license-application/${this.applicationId}/${this.view_tab}`]);
  }

}
