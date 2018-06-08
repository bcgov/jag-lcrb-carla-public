import { Component, OnInit } from '@angular/core';
import { UserDataService } from '../../../services/user-data.service';
import { User } from '../../../models/user.model';

@Component({
  selector: 'app-corporate-details',
  templateUrl: './corporate-details.component.html',
  styleUrls: ['./corporate-details.component.scss']
})
export class CorporateDetailsComponent implements OnInit {
  user: User;

  constructor(private userDataService: UserDataService) { }

  ngOnInit() {
    this.userDataService.getCurrentUser().then(user =>{
      this.user = user;
    })
  }

}
