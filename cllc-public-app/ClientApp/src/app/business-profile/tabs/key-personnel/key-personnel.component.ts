import { Component, OnInit, Input } from '@angular/core';
import { ActivatedRoute } from '../../../../../node_modules/@angular/router';
import { DynamicsDataService } from '../../../services/dynamics-data.service';

@Component({
  selector: 'app-key-personnel',
  templateUrl: './key-personnel.component.html',
  styleUrls: ['./key-personnel.component.scss']
})
export class KeyPersonnelComponent implements OnInit {
  @Input() accountId: string;
  @Input() businessType: string;

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
