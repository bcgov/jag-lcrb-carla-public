import { Injectable } from "@angular/core";
import { CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from "@angular/router";
import { Store } from "@ngrx/store";
import { AppState } from "../app-state/models/app-state";
import { map } from "rxjs/operators";

@Injectable()
export class PoliceAuthGuard implements CanActivate {

  constructor(private router: Router, private store: Store<AppState>) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    console.log("PoliceAuthGuard#canActivate called");
    return this.store.select((s) => s.currentUserState.currentUser)
      .pipe(map(user => {
        const allowAccess = (user && user.userType === "Business" && user.isPoliceRepresentative);
        if (!allowAccess) {
          this.router.navigate(["/"]);
        }
        return allowAccess;
      }));
  }
}

