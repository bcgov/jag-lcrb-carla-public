import { Injectable } from '@angular/core';
import { FeatureFlagDataService } from './feature-flag-data.service';
import { map } from 'rxjs/operators';
import { of, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class FeatureFlagService {

  private _featureFlags: Observable<string[]> = of([]); // A list of all features turned ON

  constructor(private featureFlagDataService: FeatureFlagDataService) {
    this._featureFlags = this.featureFlagDataService.getFeatureFlags();
   }

  featureOn(featureName: string): Observable<boolean> {
    if (!featureName) {
      return of(true);
    }
    // Find the feature flag that is turned on
    // if feature not found, default to turned off
    return this._featureFlags
      .pipe(map(features => {
        return !!features.find(feature => feature === featureName);
      }));
  }
}
