import { Component, ViewChild, Input } from '@angular/core';
import { DynamicsFormComponent } from '../dynamics-form/dynamics-form.component';
import { User } from '../models/user.model';
import { UserDataService } from '../services/user-data.service';
import { DynamicsDataService } from '../services/dynamics-data.service';

@Component({
    selector: 'app-business-profile',
    templateUrl: './business-profile.component.html',
    styleUrls: ['./business-profile.component.scss']
})
/** BusinessProfile component*/
export class BusinessProfileComponent {
  @ViewChild(DynamicsFormComponent) dynamicsFormComponent: DynamicsFormComponent;
  @Input('currentUser') currentUser: User;
  // GUID for the account we want to edit the profile for.  If blank then it will be the current user's account.
  @Input('accountId') accountId: string;

  public view_tab: string;
  public contactId: string;
  public componentLoaded: boolean;

  number_tabs = 7;
  /** BusinessProfile ctor */
  constructor(private userDataService: UserDataService, private dynamicsDataService: DynamicsDataService) {
      this.view_tab = "tab-0";
  }

  ngOnInit(): void {
    // TODO - pass currentUser in as router data rather than doing another call to getCurrentUser.
    if (!this.currentUser) {
      this.userDataService.getCurrentUser()
        .then((data) => {
          this.currentUser = data;

          if (!this.accountId) {
            this.accountId = this.currentUser.accountid;
          }
          // fetch the account to get the primary contact.
          this.dynamicsDataService.getRecord("account", this.accountId)
            .then((data) => {
              if (data.primarycontact) {
                this.contactId = data.primarycontact.id;
              }              
            });

          this.componentLoaded = true;
        });
      }
    }
    
  getTab() {
    var temp = this.view_tab.substring(4);
    var result = parseInt(temp);
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
      currentTab = this.number_tabs;
    }
    this.view_tab = "tab-" + currentTab;
  }

  next() {
    var currentTab = this.getTab();
    currentTab++;
    if (currentTab > this.number_tabs) {
      currentTab = 0;
    }
    this.view_tab = "tab-" + currentTab;
  }
}
