import { Injectable } from "@angular/core";
import { catchError } from "rxjs/operators";

import { User } from "@models/user.model";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { DataService } from "./data.service";
import { Store } from "@ngrx/store";
import { AppState } from "@app/app-state/models/app-state";
import { SetCurrentUserAction } from "@app/app-state/actions/current-user.action";
import { of } from "rxjs";

@Injectable({
  providedIn: "root"
})
export class UserDataService extends DataService {
  private currentUser: User;

  constructor(private http: HttpClient,
    private store: Store<AppState>) {
    super();
  }

  getCurrentUser() {
    return of(this.currentUser);
  }

  getCurrentUserRest() {
    const headers = new HttpHeaders();
    headers.append("Content-Type", "application/json");

    return this.http.get<User>("api/user/current",
      {
        headers: headers
      }).pipe(catchError(this.handleError));
  }

  loadUserToStore() {
    return this.getCurrentUserRest()
      // handle the error before converting to a promise
      .pipe(catchError(e => of(null as User)))
      .toPromise().then(user => {
        this.currentUser = user;
        this.store.dispatch(new SetCurrentUserAction(user));
      }).catch(e => {
      });
  }
}
