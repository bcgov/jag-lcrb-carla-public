import { Component, OnInit } from '@angular/core';
import { FeatureFlagService } from '@services/feature-flag.service';

@Component({
  selector: 'app-worker-landing-page',
  templateUrl: './worker-landing-page.component.html',
  styleUrls: ['./worker-landing-page.component.scss']
})
export class WorkerLandingPageComponent implements OnInit {

  // This Observable will track the FEATURE_DISABLE_WORKER_QUALIFICATION feature flag as sent by the API
  disableQualification$ = this.featureFlagService.featureOn('DisableWorkerQualification');

  policySlug = 'worker-qualification-no-longer-required';

  constructor(private featureFlagService: FeatureFlagService) {
  }

  ngOnInit() {
  }
}
