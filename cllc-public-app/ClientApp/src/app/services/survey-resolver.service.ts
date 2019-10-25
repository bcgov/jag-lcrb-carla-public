
import { map } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import {
  Router, Resolve, RouterStateSnapshot,
  ActivatedRouteSnapshot
} from '@angular/router';
import { Http, Response } from '@angular/http';
import { HttpClient, HttpParams } from '@angular/common/http';


@Injectable()
export class SurveyResolver implements Resolve<any> {
  constructor(private router: Router, private http: HttpClient) { }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<any> {

    if (route.data.survey) {
      return of(route.data.survey);
    }

    if (route.data.survey_path) {
      const params = new HttpParams();
      return this.http.get(route.data.survey_path, {
        params: new HttpParams().set('t', (new Date().getTime().toString()))
      });
    }

  }

  handeLoadError() {
    this.router.navigate(['not-found']);
  }
}
