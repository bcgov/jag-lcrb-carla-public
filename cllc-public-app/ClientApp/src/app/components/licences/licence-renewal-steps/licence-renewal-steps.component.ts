import { Component, OnInit } from '@angular/core';

import { FeatureFlagService } from '@services/feature-flag.service';

@Component({
  selector: 'app-licence-renewal-steps',
  templateUrl: './licence-renewal-steps.component.html',
  styleUrls: ['./licence-renewal-steps.component.scss']
})
export class LicenceRenewalStepsComponent implements OnInit {

  licenseeChangesEnabled: boolean;
  

  constructor(public featureFlagService: FeatureFlagService ) {

    featureFlagService.featureOn('LicenseeChanges')
      .subscribe(featureOn => this.licenseeChangesEnabled = featureOn);    

  }

  ngOnInit() {
  }

  selectionChange(event) {
  }
}
