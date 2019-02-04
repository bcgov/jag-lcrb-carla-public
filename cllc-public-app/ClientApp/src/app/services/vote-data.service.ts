import { Injectable } from '@angular/core';
import { Http, Headers, Response } from '@angular/http';


import { VoteOption } from '../models/vote-option.model';
import { VoteQuestion } from '../models/vote-question.model';
@Injectable()
export class VoteDataService {
   constructor(private http: Http) { }

   getQuestion(slug: any) {
     const headers = new Headers();
     headers.append('Content-Type', 'application/json');

     return this.http.get('api/voteQuestion/' + slug, {
       headers: headers
     })
       .toPromise()
       .then((res: Response) => {
         const data = res.json();
         const voteQuestion = new VoteQuestion();
         voteQuestion.id = data.id;
         voteQuestion.options = data.options;
         voteQuestion.question = data.question;
         voteQuestion.title = data.title;
         return voteQuestion;
       })
       .catch(this.handleError);
   }

   postVote(slug: any, option: any) {
     const headers = new Headers();
     headers.append('Content-Type', 'application/json');

     return this.http.post('api/voteQuestion/' + slug + '/vote?option=' + option, {
       headers: headers
     })
       .toPromise()
       .then((res: Response) => {
         const data = res.json();
         const voteQuestion = new VoteQuestion();
         voteQuestion.id = data.id;
         voteQuestion.options = data.options;
         voteQuestion.question = data.question;
         voteQuestion.title = data.title;
         return voteQuestion;
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
