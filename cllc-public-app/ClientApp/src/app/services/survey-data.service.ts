import { Injectable } from '@angular/core';
import { Http, Headers, Response } from '@angular/http';

@Injectable()
export class SurveyDataService {
   constructor(private http: Http) { }

   getSurveyData(clientId: string) {
     const headers = new Headers();
     headers.append('Content-Type', 'application/json');

     return this.http.get('api/survey/getResultByClient/' + clientId, {
       headers: headers
     })
       .toPromise()
       .then((res: Response) => {
         const data = res.json();
         return data;
       })
       .catch(this.handleError);
   }

   private handleError(error: Response | any) {
     let errMsg: string;
     if (error instanceof Response) {
       const body = error.json() || '';
       const err = body.error || JSON.stringify(body);
       errMsg = `${error.status} - ${error.statusText || ''} ${err}`;
     } else {
       errMsg = error.message ? error.message : error.toString();
     }
     console.error(errMsg);
     return Promise.reject(errMsg);
   }
}
