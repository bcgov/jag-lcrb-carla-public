import { Component, OnInit } from "@angular/core";

import { FeatureFlagService } from "@services/feature-flag.service";
import { ActivatedRoute } from "@angular/router";
import { CRS_RENEWAL_LICENCE_TYPE_NAME, LIQUOR_RENEWAL_LICENCE_TYPE_NAME } from "../licences.component";

@Component({
  selector: "app-licence-renewal-steps",
  templateUrl: "./licence-renewal-steps.component.html",
  styleUrls: ["./licence-renewal-steps.component.scss"]
})
export class LicenceRenewalStepsComponent implements OnInit {
  busy: any;
  licenseeChangesEnabled: boolean;
  licenceType: string;
  CRS_RENEWAL_LICENCE_TYPE_NAME = CRS_RENEWAL_LICENCE_TYPE_NAME;
  LIQUOR_RENEWAL_LICENCE_TYPE_NAME = LIQUOR_RENEWAL_LICENCE_TYPE_NAME;


  constructor(public featureFlagService: FeatureFlagService,
    private route: ActivatedRoute) {

    featureFlagService.featureOn("LicenseeChanges")
      .subscribe(featureOn => this.licenseeChangesEnabled = featureOn);

    this.route.paramMap.subscribe(pmap => this.licenceType = pmap.get("licenceType"));
  }

  ngOnInit() {
  }

  selectionChange(event) {
  }
}
