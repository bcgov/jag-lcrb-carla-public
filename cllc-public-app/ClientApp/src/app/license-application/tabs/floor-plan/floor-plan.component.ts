import { Component, OnInit, Input } from '@angular/core';
import { UserDataService } from '../../../services/user-data.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-floor-plan',
  templateUrl: './floor-plan.component.html',
  styleUrls: ['./floor-plan.component.scss']
})
export class FloorPlanComponent implements OnInit {
  applicationId: string;

  constructor(private userDataService: UserDataService, private route: ActivatedRoute) {
    this.applicationId = route.parent.snapshot.params.applicationId;
  }

  ngOnInit() {
  }

}
