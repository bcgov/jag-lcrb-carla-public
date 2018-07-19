import { Component, ViewChild, Input, OnInit } from '@angular/core';
import { DynamicsFormComponent } from '../dynamics-form/dynamics-form.component';
import { User } from '../models/user.model';
import { UserDataService } from '../services/user-data.service';
import { DynamicsDataService } from '../services/dynamics-data.service';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { switchMap } from 'rxjs/operators';
import { Store } from '@ngrx/store';
import { AppState } from '../app-state/models/app-state';
import * as CurrenAccountActions from '../app-state/actions/current-account.action';

@Component({
  selector: 'app-business-profile',
  templateUrl: './business-profile.component.html',
  styleUrls: ['./business-profile.component.scss']
})
/** BusinessProfile component*/
export class BusinessProfileComponent implements OnInit {
  @ViewChild(DynamicsFormComponent) dynamicsFormComponent: DynamicsFormComponent;
  @Input() currentUser: User;
  // GUID for the account we want to edit the profile for.  If blank then it will be the current user's account.
  legalEntityId: string;

  public view_tab: string;
  public contactId: string;
  public componentLoaded: boolean;

  tabs: any = {
    privateCorportation: ['before-you-start', 'corporate-details', 'organization-structure', 'directors-and-officers', 'key-personnel',
      'shareholders', 'connections-to-producers', 'finance-integrity', 'security-assessment'],
    society: ['before-you-start', 'corporate-details', 'organization-structure', 'directors-and-officers', 'key-personnel',
      'connections-to-producers', 'finance-integrity', 'security-assessment'],
    partnership: ['before-you-start', 'corporate-details', 'organization-structure', 'key-personnel', 'shareholders',
      'connections-to-producers', 'finance-integrity', 'security-assessment'],
    soleProprietor: ['before-you-start', 'corporate-details', 'directors-and-officers', 'key-personnel',
      'finance-integrity', 'security-assessment']
  };

  tabStructure: any[] = this.tabs.privateCorportation;

  number_tabs = 7;
  _businessType: string;
  accountId: string;
  businessName: string;
  get businessType(): string {
    return this._businessType;
  }
  set businessType(value: string) {
    this._businessType = value;
    this.onBusinessTypeChange(value);
    console.log(`Business Type: ${value}`);
  }
  /** BusinessProfile ctor */
  constructor(private userDataService: UserDataService,
    private store: Store<AppState>,
    private router: Router,
    private route: ActivatedRoute,
    private dynamicsDataService: DynamicsDataService) {
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

    this.route.params.subscribe(p => {
      this.legalEntityId = p.legalEntityId;
      this.accountId = p.accountId;
      this.dynamicsDataService.getRecord('account', this.accountId)
        .then((data) => {
          this.store.dispatch(new CurrenAccountActions.SetCurrentAccountAction(data));
          if (data.primarycontact) {
            this.contactId = data.primarycontact.id;
          }
          this.businessType = data.businessType;
          this.componentLoaded = true;
          this.businessName = data.name;
        });
    });
  }

  getTab() {
    const result = this.tabStructure.indexOf(this.view_tab);
    return result;
  }

  saveApplicantInformation() {
    this.dynamicsFormComponent.onSubmit();
  }

  isOnLastTab(): boolean {
    const isLast = (this.tabStructure.indexOf(this.view_tab)  === (this.tabStructure.length - 1));
    return isLast;

  }

  changeTab(tab) {
    this.view_tab = tab;
    this.router.navigate([`/business-profile/${this.accountId}/${this.legalEntityId}/${tab}`]);
  }

  back() {
    let currentTab = this.getTab();
    currentTab--;
    if (currentTab < 0) {
      currentTab = this.tabStructure.length - 1;
    }
    this.view_tab = this.tabStructure[currentTab];
    this.router.navigate([`/business-profile/${this.accountId}/${this.legalEntityId}/${this.view_tab}`]);
  }

  next() {
    let currentTab = this.getTab();
    currentTab++;
    if (currentTab >= this.tabStructure.length) {
      currentTab = 0;
    }
    this.view_tab = this.tabStructure[currentTab];
    this.router.navigate([`/business-profile/${this.accountId}/${this.legalEntityId}/${this.view_tab}`]);
  }

  onBusinessTypeChange(value: string) {
    switch (value) {
      case 'PrivateCorporation':
      case 'PublicCorporation':
      case 'LimitedLiabilityCorporation':
      case 'UnlimitedLiabilityCorporation':
        this.tabStructure = this.tabs.privateCorportation;
        break;
      case 'SoleProprietor':
        this.tabStructure = this.tabs.soleProprietor;
        break;
      case 'LimitedPartnership':
      case 'LimitedLiabilityPartnership':
      case 'GeneralPartnership':
        this.tabStructure = this.tabs.partnership;
        break;
        
      case 'Society':
        this.tabStructure = this.tabs.society;
        break;
      default:
        break;
    }
  }
}
