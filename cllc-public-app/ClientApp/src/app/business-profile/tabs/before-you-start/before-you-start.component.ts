import { Component, OnInit, Input } from '@angular/core';
import { UserDataService } from '../../../services/user-data.service';
import { User } from '../../../models/user.model';
import { ActivatedRoute } from '@angular/router';
import { DynamicsDataService } from '../../../services/dynamics-data.service';

@Component({
  selector: 'app-before-you-start',
  templateUrl: './before-you-start.component.html',
  styleUrls: ['./before-you-start.component.scss']
})
export class BeforeYouStartComponent implements OnInit {
  @Input() accountId: string;
  @Input() businessType: string;

  constructor(private dynamicsDataService: DynamicsDataService, private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.parent.params.subscribe(p => {
      this.accountId = p.accountId;

      this.dynamicsDataService.getRecord('account', this.accountId)
        .then((data) => {
          this.businessType = data.businessType;
        });
    });
  }

}
