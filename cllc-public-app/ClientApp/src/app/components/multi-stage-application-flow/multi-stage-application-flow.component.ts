import { Component, OnInit } from '@angular/core';
import { FeatureFlagService } from '@services/feature-flag.service';

@Component({
  selector: 'app-multi-stage-application-flow',
  templateUrl: './multi-stage-application-flow.component.html',
  styleUrls: ['./multi-stage-application-flow.component.scss']
})
export class MultiStageApplicationFlowComponent implements OnInit {
  securityScreeningEnabled: boolean;

  constructor(public featureFlagService: FeatureFlagService) {
    featureFlagService.featureOn('SecurityScreening')
    .subscribe(featureOn => this.securityScreeningEnabled = featureOn);
   }

  ngOnInit() {
  }

  selectionChange(event) {
  }

}
