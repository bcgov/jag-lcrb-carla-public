import { Component, OnInit } from '@angular/core';

import { FeatureFlagService } from '@services/feature-flag.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-licence-renewal-steps',
  templateUrl: './licence-renewal-steps.component.html',
  styleUrls: ['./licence-renewal-steps.component.scss']
})
export class LicenceRenewalStepsComponent implements OnInit {
  busy: any;
  licenseeChangesEnabled: boolean;
  licenceType: string;


  constructor(public featureFlagService: FeatureFlagService,
    private route: ActivatedRoute) {

    featureFlagService.featureOn('LicenseeChanges')
      .subscribe(featureOn => this.licenseeChangesEnabled = featureOn);

      this.route.paramMap.subscribe(pmap => this.licenceType = pmap.get('licenceType'));

  }

  ngOnInit() {
  }

  selectionChange(event) {
  }
}
