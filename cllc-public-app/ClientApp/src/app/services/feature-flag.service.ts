import { Injectable } from '@angular/core';
import { FeatureFlagDataService } from './feature-flag-data.service';
import { mergeMap } from 'rxjs/operators';
import { of, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class FeatureFlagService {

  private _featureFlags: Array<string> = [] // A list of all features turned ON
  public initialized = false;

  constructor(private featureFlagDataService: FeatureFlagDataService) {
  }

  public init() {
    // need to get features here to make sure initialized will be set to true at some point
    this.featureFlagDataService.getFeatureFlags()
      .subscribe(featureFlags => {
        this._featureFlags = featureFlags;
        this.initialized = true;
        return featureFlags;
      });
  }

  getFeature(featureName: string): Observable<boolean> {
    if (!featureName) {
      return of(false);
    }
    // Find the feature flag that is turned on
    if (this._featureFlags && !!this._featureFlags.find(feature => {
      return feature === featureName;
    })) {
      return of(true);
    }
    else {
      return of(false);
    }
  }

  featureOn(featureName: string): Observable<boolean> {
    if (this.initialized) {
      return this.getFeature(featureName);
    }
    else {
      return this.featureFlagDataService.getFeatureFlags()
        .pipe(mergeMap(featureFlags => {
          this._featureFlags = featureFlags;
          this.initialized = true;
          return this.getFeature(featureName);
        }));
    }


  }

}
