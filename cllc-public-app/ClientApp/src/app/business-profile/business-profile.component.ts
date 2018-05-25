import { Component } from '@angular/core';

@Component({
    selector: 'app-business-profile',
    templateUrl: './business-profile.component.html',
    styleUrls: ['./business-profile.component.scss']
})
/** BusinessProfile component*/
export class BusinessProfileComponent {
  public view_tab: string;
    /** BusinessProfile ctor */
    constructor() {
      this.view_tab = "application-information-tab";
  }

  changeTab(tab) {
    this.view_tab = tab;
  }
}
