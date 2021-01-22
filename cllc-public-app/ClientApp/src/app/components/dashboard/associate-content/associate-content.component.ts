import { Component, OnInit, Input } from "@angular/core";
import { Account } from "@models/account.model";
import { FeatureFlagService } from "@services/feature-flag.service";

@Component({
  selector: "app-associate-content",
  templateUrl: "./associate-content.component.html",
  styleUrls: ["./associate-content.component.scss"]
})
export class AssociateContentComponent implements OnInit {
  @Input()
  account: Account;
  @Input()
  isIndigenousNation: boolean;
  @Input()
  showBothContents = true;
  @Input()
  hasLicence = false;
  licenseeChangeFeatureOn: boolean = false;

  constructor(private featureFlagService: FeatureFlagService) {
    featureFlagService.featureOn("LicenseeChanges")
      .subscribe(x => this.licenseeChangeFeatureOn = x);
  }

  ngOnInit() {
  }

}
