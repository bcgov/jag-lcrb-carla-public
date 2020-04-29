import { Injectable } from '@angular/core';
import { FeatureFlagDataService } from './feature-flag-data.service';
import { map } from 'rxjs/operators';
import { of, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class FeatureFlagService {

  private _featureFlags: Array<string> = [] // A list of all features turned ON
  private initialized = false;

  constructor(private featureFlagDataService: FeatureFlagDataService) {

    
  }

  public init() {
    if (!this.initialized) {
      console.log("GETTING FLAGS");

      this.featureFlagDataService.getFeatureFlags()
        .toPromise()
        .then(featureFlags => {
          console.log("GOT FLAGS");
          console.log(featureFlags);
          this._featureFlags = featureFlags;
          this.initialized = true;
        });
    }
  }

  featureOn(featureName: string): Observable<boolean> {
    if (!featureName) {
      return of(false);
    }

    console.log("Looking for feature " + featureName);
    console.log("FEATURE FLAGS");
    console.log(this._featureFlags);

    // Find the feature flag that is turned on
    if (this._featureFlags && !!this._featureFlags.find(feature => {
      return feature === featureName;
    })) {
      console.log("Found feature " + featureName)
      return of(true);
    }
    else {
      console.log("Did not find feature " + featureName);
      return of(false);
    }
    
    }
    
}
