import { Component, OnInit, Input } from '@angular/core';
import { UserDataService } from '../../../services/user-data.service';

@Component({
  selector: 'app-site-map',
  templateUrl: './site-map.component.html',
  styleUrls: ['./site-map.component.scss']
})
export class SiteMapComponent implements OnInit {
  @Input() accountId: string;

  constructor(private userDataService: UserDataService) { }

  ngOnInit() {
    this.userDataService.getCurrentUser()
      .then((data) => {
        this.accountId = data.accountid;
      });
  }

}
