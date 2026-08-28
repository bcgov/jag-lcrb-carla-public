import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot } from '@angular/router';
import { Store } from '@ngrx/store';
import { map } from 'rxjs/operators';
import { AppState } from '../app-state/models/app-state';

@Injectable()
export class PoliceAuthGuard implements CanActivate {
  constructor(
    private router: Router,
    private store: Store<AppState>
  ) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    console.log('PoliceAuthGuard#canActivate called');
    return this.store
      .select((s) => s.currentUserState.currentUser)
      .pipe(
        map((user) => {
          const allowAccess = user && user.isPoliceRepresentative;
          if (!allowAccess) {
            this.router.navigate(['/']);
          }
          return allowAccess;
        })
      );
  }
}
