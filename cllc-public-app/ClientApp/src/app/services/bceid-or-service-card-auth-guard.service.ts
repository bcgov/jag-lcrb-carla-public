import { Injectable } from "@angular/core";
import { CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from "@angular/router";
import { UserDataService } from "./user-data.service";
import { Store } from "@ngrx/store";
import { AppState } from "../app-state/models/app-state";
import { map } from "rxjs/operators";

@Injectable()
export class BCeidOrServiceCardAuthGuard implements CanActivate {
  window = window;
  constructor(private userService: UserDataService,
    private router: Router,
    private store: Store<AppState>) {
  }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    console.log("AuthGuard#canActivate called");
    return this.store.select((s) => s.currentUserState.currentUser)
      .pipe(map(user => {
        // 2021-05-05 - added support for Individual, for Basic BCeID logins.
        const allowAccess = (user && (user.userType === "Business" || user.userType === "VerifiedIndividual" || user.userType === "Individual"));
        if (!allowAccess) {
          debugger;
          if (route[0] === "sep" && route[1] === "claim") {
            this.window.location.href = "sep/login";
          }
          this.router.navigate(["/"]);
        }
        return allowAccess;
      }));
  }
}
