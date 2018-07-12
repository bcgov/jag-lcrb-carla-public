import { Component, OnInit, Input } from '@angular/core';
import { UserDataService } from '../../../services/user-data.service';

@Component({
  selector: 'app-floor-plan',
  templateUrl: './floor-plan.component.html',
  styleUrls: ['./floor-plan.component.scss']
})
export class FloorPlanComponent implements OnInit {
  @Input() accountId: string;

  constructor(private userDataService: UserDataService) { }

  ngOnInit() {
    this.userDataService.getCurrentUser()
      .then((data) => {
        this.accountId = data.accountid;
      });
  }

}
