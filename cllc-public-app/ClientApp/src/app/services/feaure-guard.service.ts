import { Injectable } from "@angular/core";
import { CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from "@angular/router";
import { Store } from "@ngrx/store";
import { AppState } from "../app-state/models/app-state";
import { map } from "rxjs/operators";
import { FeatureFlagService } from "./feature-flag.service";

@Injectable({ providedIn: "root" })
export class FeatureGuard implements CanActivate {

  constructor(public featureFlagService: FeatureFlagService,
    private router: Router,
    private store: Store<AppState>) {
  }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    console.log("FeatureGuard#canActivate called");
    return this.featureFlagService.featureOn(route.data.feature)
      .pipe(map(featureOn => {
        if (!featureOn) {
          this.router.navigate(["/dashboard"]);
        }
        return featureOn;
      }));
  }
}
