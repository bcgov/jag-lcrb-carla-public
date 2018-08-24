import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { UserDataService } from './user-data.service';
import { Store } from '@ngrx/store';
import { AppState } from '../app-state/models/app-state';
import { User } from '../models/user.model';
import { Subject } from 'rxjs';

@Injectable()
export class ServiceCardAuthGuard implements CanActivate {

    constructor(private userService: UserDataService, 
        private router: Router,
        private store: Store<AppState>) {
         }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        var result = new Subject<boolean>();
        this.userService.getCurrentUser()
        .subscribe(user=> {
            console.log('ServiceCardAuthGuard#canActivate called');
            var allowAccess = (user && user.userType == "VerifiedIndividual");
                if (!allowAccess) {
                    this.router.navigate(['/']);
                }
            result.next(allowAccess);
        });
        return result;
    }
}