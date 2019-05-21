import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { UserDataService } from './user-data.service';
import { Store } from '@ngrx/store';
import { AppState } from '../app-state/models/app-state';
import { User } from '../models/user.model';
import { Subject } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable()
export class ServiceCardAuthGuard implements CanActivate {

    constructor(private userService: UserDataService,
        private router: Router,
        private store: Store<AppState>) {
    }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {

        return this.store.select(s => s.currentUserState.currentUser)
            .pipe(map(user => {
                console.log('ServiceCardAuthGuard#canActivate called');
                const allowAccess = (user && user.userType === 'VerifiedIndividual');
                console.log(allowAccess);

                if (!allowAccess) {
                    this.router.navigate(['/']);
                }
                return allowAccess;
            }));
    }
}
