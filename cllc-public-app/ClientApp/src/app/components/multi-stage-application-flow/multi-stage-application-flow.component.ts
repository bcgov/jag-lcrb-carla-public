import { Component, OnInit, Input } from '@angular/core';
import { FeatureFlagService } from '@services/feature-flag.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-multi-stage-application-flow',
  templateUrl: './multi-stage-application-flow.component.html',
  styleUrls: ['./multi-stage-application-flow.component.scss']
})
export class MultiStageApplicationFlowComponent implements OnInit {
  securityScreeningEnabled: boolean;
  useDynamicFormMode: boolean = false;
  applicationId: string;

  constructor(public featureFlagService: FeatureFlagService, private route: ActivatedRoute, ) {

    featureFlagService.featureOn('SecurityScreening')
      .subscribe(featureOn => this.securityScreeningEnabled = featureOn);

    this.route.paramMap.subscribe(params => {
      this.useDynamicFormMode = params.get('useDynamicFormMode') === 'true';
      this.applicationId = params.get('applicationId');
    });

   }

  ngOnInit() {
  }

  selectionChange(event) {
  }

}
