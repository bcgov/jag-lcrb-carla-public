import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from '@angular/router';
import { Store } from '@ngrx/store';
import { map } from 'rxjs/operators';
import { AppState } from '../app-state/models/app-state';
import { FeatureFlagService } from './feature-flag.service';

@Injectable({ providedIn: 'root' })
export class FeatureGuard implements CanActivate {
  constructor(
    public featureFlagService: FeatureFlagService,
    private router: Router,
    private store: Store<AppState>
  ) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    console.log('FeatureGuard#canActivate called');
    return this.featureFlagService.featureOn(route.data.feature).pipe(
      map((featureOn) => {
        if (!featureOn) {
          this.router.navigate(['/dashboard']);
        }
        return featureOn;
      })
    );
  }
}
