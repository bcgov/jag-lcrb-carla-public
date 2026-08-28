import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { VoteQuestion } from '@models/vote-question.model';
import { catchError } from 'rxjs/operators';
import { DataService } from './data.service';

@Injectable()
export class VoteDataService extends DataService {
  constructor(private http: HttpClient) {
    super();
  }

  getQuestion(slug: any) {
    return this.http
      .get<VoteQuestion>(`api/voteQuestion/${slug}`, {
        headers: this.headers
      })
      .pipe(catchError(this.handleError));
  }

  postVote(slug: any, option: any) {
    return this.http
      .post<VoteQuestion>(`api/voteQuestion/${slug}/vote?option=${option}`, {
        headers: this.headers
      })
      .pipe(catchError(this.handleError));
  }
}
