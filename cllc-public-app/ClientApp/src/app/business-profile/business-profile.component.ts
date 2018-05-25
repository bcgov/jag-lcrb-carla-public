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
    /** BusinessProfile ctor */
    constructor() {
      this.view_tab = "application-information-tab";
  }

  saveApplicantInformation() {    
    
    this.dynamicsFormComponent.onSubmit();
  }

  changeTab(tab) {
    this.view_tab = tab;
  }

  back() {

  }
  next() {

  }


}
