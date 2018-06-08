import { Component, OnInit } from '@angular/core';
import { DynamicsDataService } from '../services/dynamics-data.service';
import { User } from '../models/user.model';
import { UserDataService } from '../services/user-data.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  user: User;
  accountId: string;
  contactId: string;
  account: any;

  constructor(private userDataService: UserDataService, private dynamicsDataService: DynamicsDataService) {
}

ngOnInit(): void {
  // TODO - pass currentUser in as router data rather than doing another call to getCurrentUser.
  if (!this.user) {
    this.userDataService.getCurrentUser()
      .then((data) => {
        this.user = data;

        if (!this.accountId) {
          this.accountId = this.user.accountid;
        }
        // fetch the account to get the primary contact.
        this.dynamicsDataService.getRecord("account", this.accountId)
          .then((data) => {
            this.account = data;
            if (data.primarycontact) {
              this.contactId = data.primarycontact.id;
            }              
          });

      });
    }
  }

}
