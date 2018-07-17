import { Component, OnInit, Input } from '@angular/core';
import { DynamicsDataService } from '../../../services/dynamics-data.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-financial-information',
  templateUrl: './financial-information.component.html',
  styleUrls: ['./financial-information.component.scss']
})
export class FinancialInformationComponent implements OnInit {
  @Input() accountId: string;
  @Input() businessType: string;
  operatingForMoreThanOneYear: string = '';

  constructor(private route: ActivatedRoute,
    private dynamicsDataService: DynamicsDataService) { }

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
