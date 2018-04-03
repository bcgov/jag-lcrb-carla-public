import { Injectable } from '@angular/core';
import { Http, Headers, Response } from "@angular/http";
import "rxjs/add/operator/toPromise";

import { VoteOption } from "./vote-option.model";
import { VoteQuestion } from "./vote-question.model";
@Injectable()
export class VoteDataService {
   constructor(private http: Http) { }


   getQuestion(slug: any) {
     let headers = new Headers();
     headers.append("Content-Type", "application/json");

     return this.http.get("/api/voteQuestion/" + slug, {
       headers: headers
     })
       .toPromise()
       .then((res: Response) => {
         let data = res.json();
         let voteQuestion = new VoteQuestion();
         voteQuestion.id = data.id;
         voteQuestion.options = data.options;
         voteQuestion.question = data.question;
         voteQuestion.title = data.title;
         return voteQuestion;
       })
       .catch(this.handleError);
   }

   postVote(slug: any, option: any) {
     let headers = new Headers();
     headers.append("Content-Type", "application/json");

     return this.http.post("/api/voteQuestion/" + slug + "/vote?option=" + option, {
       headers: headers
     })
       .toPromise()
       .then((res: Response) => {
         let data = res.json();
         let voteQuestion = new VoteQuestion();
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
       const body = error.json() || "";
       const err = body.error || JSON.stringify(body);
       errMsg = `${error.status} - ${error.statusText || ""} ${err}`;
     } else {
       errMsg = error.message ? error.message : error.toString();
     }
     console.error(errMsg);
     return Promise.reject(errMsg);
   }
}
