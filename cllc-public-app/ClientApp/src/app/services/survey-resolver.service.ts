import { Injectable }  from '@angular/core';
import { Observable, of } from 'rxjs';
import { Router, Resolve, RouterStateSnapshot,
         ActivatedRouteSnapshot } from '@angular/router';
import { Http, Response } from '@angular/http';
import 'rxjs/add/operator/map';

@Injectable()
export class SurveyResolver implements Resolve<any> {
  constructor(private router: Router, private http : Http) {}

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<any> {

    if(route.data.survey) {
      return of(route.data.survey);
    }

    if(route.data.survey_path) {
      return this.http.get(route.data.survey_path, {params: {t: new Date().getTime()}})
            .map((x) => x.json())
            ; //.catch(this.handleLoadError);
    }

  }

  handeLoadError() {
    this.router.navigate(['not-found']);
  }
}
