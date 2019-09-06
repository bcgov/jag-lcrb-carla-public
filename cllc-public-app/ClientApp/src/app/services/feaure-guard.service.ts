import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { UserDataService } from './user-data.service';
import { Store } from '@ngrx/store';
import { AppState } from '../app-state/models/app-state';
import { User } from '../models/user.model';
import { Subject } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { FeatureFlagService } from './feature-flag.service';
import { all } from 'q';

@Injectable({ providedIn: 'root' })
export class FeatureGuard implements CanActivate {

    constructor(public featureFlagService: FeatureFlagService,
        private router: Router,
        private store: Store<AppState>) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        console.log('BCeidAuthGuard#canActivate called');
        return this.featureFlagService.featureOn(route.data.feature)
                .pipe(map(featureOn => {
                    if (!featureOn) {
                        this.router.navigate(['/dashboard']);
                    }
                    return featureOn;
                }));
    }
}
