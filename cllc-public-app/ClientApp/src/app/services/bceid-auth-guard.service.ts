import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from '@angular/router';
import { Store } from '@ngrx/store';
import { map } from 'rxjs/operators';
import { AppState } from '../app-state/models/app-state';
import { UserDataService } from './user-data.service';

@Injectable()
export class BCeidAuthGuard implements CanActivate {
  constructor(
    private userService: UserDataService,
    private router: Router,
    private store: Store<AppState>
  ) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    console.log('BCeidAuthGuard#canActivate called');
    return this.store
      .select((s) => s.currentUserState.currentUser)
      .pipe(
        map((user) => {
          const allowAccess = user && user.userType === 'Business';
          if (!allowAccess) {
            this.router.navigate(['/']);
          }
          return allowAccess;
        })
      );
  }
}
