import { Injectable } from '@angular/core';
import { FeatureFlag } from '@models/feature-flag.model';
import { Observable, of } from 'rxjs';
import { mergeMap, shareReplay } from 'rxjs/operators';
import { FeatureFlagDataService } from './feature-flag-data.service';

@Injectable({
  providedIn: 'root'
})
export class FeatureFlagService {
  private _featureFlags: FeatureFlag[] = []; // A list of all features turned ON
  private initialized = false;
  private initializationObservable: Observable<FeatureFlag[]> | null = null;

  constructor(private featureFlagDataService: FeatureFlagDataService) {}

  init() {
    // Trigger initialization if not already started
    this.ensureInitialized();
  }

  /**
   * Ensures feature flags are loaded.
   *
   * Returns an observable that completes when loaded.
   * Uses shareReplay to ensure the HTTP call is only made once even if multiple
   * subscribers are waiting.
   */
  private ensureInitialized(): Observable<FeatureFlag[]> {
    if (this.initialized) {
      return of(this._featureFlags);
    }

    if (!this.initializationObservable) {
      this.initializationObservable = this.featureFlagDataService.getFeatureFlags().pipe(
        mergeMap((featureFlags) => {
          this._featureFlags = featureFlags;
          this.initialized = true;
          this.initializationObservable = null; // Clear the pending observable

          return of(featureFlags);
        }),
        shareReplay(1) // Share the result with all subscribers
      );
    }

    return this.initializationObservable;
  }

  private getFeature(featureName: string): Observable<FeatureFlag | null> {
    if (!featureName) {
      return of(null);
    }

    return this.ensureInitialized().pipe(
      mergeMap(() => {
        if (!this._featureFlags) {
          return of(null);
        }

        const feature = this._featureFlags.find((feature) => {
          return feature.name === featureName;
        });

        return of(feature);
      })
    );
  }

  /**
   * Return the value of the feature flag.
   * Returns `null` if the feature flag is not found, or has a null or empty value.
   *
   * @param {string} featureName
   * @return {*}  {(Observable<string | null>)}
   */
  featureValue(featureName: string): Observable<string | null> {
    const feature = this.getFeature(featureName);
    return feature.pipe(mergeMap((feature) => of(feature ? feature.value : null)));
  }

  /**
   * Return `true` if the feature is enabled and has a non-null/non-empty value.
   * Return `false` otherwise.
   *
   * @param {string} featureName
   * @return {*}  {Observable<boolean>}
   */
  featureOn(featureName: string): Observable<boolean> {
    const feature = this.getFeature(featureName);
    return feature.pipe(mergeMap((feature) => of(feature ? feature.enabled : false)));
  }
}
