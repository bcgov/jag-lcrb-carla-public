import { Component, ViewChild, Input } from '@angular/core';
import { DynamicsFormComponent } from '../dynamics-form/dynamics-form.component';
import { User } from '../models/user.model';
import { UserDataService } from '../services/user-data.service';
import { DynamicsDataService } from '../services/dynamics-data.service';
import { ActivatedRoute } from '@angular/router';
import { switchMap } from 'rxjs/operators';
import { State } from '../app-state/reducers/app-state';
import { Store } from '@ngrx/store';

@Component({
  selector: 'app-business-profile',
  templateUrl: './business-profile.component.html',
  styleUrls: ['./business-profile.component.scss']
})
/** BusinessProfile component*/
export class BusinessProfileComponent {
  businessProfileId: string;
  @ViewChild(DynamicsFormComponent) dynamicsFormComponent: DynamicsFormComponent;
  @Input('currentUser') currentUser: User;
  // GUID for the account we want to edit the profile for.  If blank then it will be the current user's account.
  @Input('accountId') accountId: string;
  legalEntityId: string;

  public view_tab: string;
  public contactId: string;
  public componentLoaded: boolean;

  tabs: any = {
    privateCorportation: ['before-you-start', 'corporate-details', 'organization-structure', 'directors-and-officers', 'key-personnel', 'shareholders', 'connections-to-producers', 'finance-integrity', 'security-assessment'],
    society: ['before-you-start', 'corporate-details', 'organization-structure', 'directors-and-officers', 'key-personnel', 'connections-to-producers', 'finance-integrity', 'security-assessment'],
    partnership: ['before-you-start', 'corporate-details', 'organization-structure', 'key-personnel', 'shareholders', 'connections-to-producers', 'finance-integrity', 'security-assessment'],
    soleProprietor: ['before-you-start', 'corporate-details', 'key-personnel', 'finance-integrity', 'security-assessment']
  };

  tabStructure: any[] = this.tabs.privateCorportation;

  number_tabs = 7;
  _businessType: string;
  get businessType(): string {
    return this._businessType;
  }
  set businessType(value: string) {
    this._businessType = value;
    this.onBusinessTypeChange(value);
  }
  /** BusinessProfile ctor */
  constructor(private userDataService: UserDataService, private route: ActivatedRoute, private store: Store<State>,
    private dynamicsDataService: DynamicsDataService) {
    this.view_tab = "before-you-start";
  }

  ngOnInit(): void {
    this.store.select(state => state.appState.currentAccountId)
    .filter(id => !!id)
    .subscribe(accountId => {
      this.accountId = accountId;
      this.dynamicsDataService.getRecord("account", this.accountId)
            .then((data) => {
              if (data.primarycontact) {
                this.contactId = data.primarycontact.id;
              }
              this.businessType = data.businessType;
              this.componentLoaded = true;
            });
    });
  }

  getTab() {
    let result = this.tabStructure.indexOf(this.view_tab);
    return result;
  }

  saveApplicantInformation() {
    this.dynamicsFormComponent.onSubmit();
  }

  changeTab(tab) {
    this.view_tab = tab;
  }

  back() {
    var currentTab = this.getTab();
    currentTab--;
    if (currentTab < 0) {
      currentTab = this.tabStructure.length - 1;
    }
    this.view_tab = this.tabStructure[currentTab];
  }

  next() {
    var currentTab = this.getTab();
    currentTab++;
    if (currentTab >= this.tabStructure.length) {
      currentTab = 0;
    }
    this.view_tab = this.tabStructure[currentTab];
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
