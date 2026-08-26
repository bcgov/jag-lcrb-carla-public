import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve, Router, RouterStateSnapshot } from '@angular/router';
import { Observable, of } from 'rxjs';

@Injectable()
export class SurveyResolver implements Resolve<any> {
  constructor(
    private router: Router,
    private http: HttpClient
  ) {}

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<any> {
    if (route.data.survey) {
      return of(route.data.survey);
    }

    if (route.data.survey_path) {
      const params = new HttpParams();
      return this.http.get(route.data.survey_path, {
        params: new HttpParams().set('t', new Date().getTime().toString())
      });
    }
  }

  handeLoadError() {
    this.router.navigate(['not-found']);
  }
}
