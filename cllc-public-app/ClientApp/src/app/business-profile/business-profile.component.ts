import { Component, ViewChild } from '@angular/core';
import { DynamicsFormComponent } from '../dynamics-form/dynamics-form.component';

@Component({
    selector: 'app-business-profile',
    templateUrl: './business-profile.component.html',
    styleUrls: ['./business-profile.component.scss']
})
/** BusinessProfile component*/
export class BusinessProfileComponent {
  @ViewChild(DynamicsFormComponent)
  private dynamicsFormComponent: DynamicsFormComponent;

  public view_tab: string;
  number_tabs = 7;
    /** BusinessProfile ctor */
    constructor() {
      this.view_tab = "tab-1";
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
    if (currentTab < 1) {
      currentTab = this.number_tabs;
    }
    this.view_tab = "tab-" + currentTab;
  }

  next() {
    var currentTab = this.getTab();
    currentTab++;
    if (currentTab > this.number_tabs) {
      currentTab = 1;
    }
    this.view_tab = "tab-" + currentTab;
  }


}
