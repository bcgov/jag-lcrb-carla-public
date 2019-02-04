import { Injectable } from '@angular/core';

import { Newsletter } from '../models/newsletter.model';
import { DataService } from './data.service';
import { HttpClient } from '@angular/common/http';
import { catchError } from 'rxjs/operators';

@Injectable()
export class NewsletterDataService extends DataService {
  constructor(private http: HttpClient) {
    super();
  }

  getNewsletter(slug: any) {
    return this.http.get<Newsletter>('api/newsletter/' + slug, {
      headers: this.headers
    }).pipe(catchError(this.handleError));
  }

  signup(slug: any, email: any) {
    return this.http.post<Newsletter>('api/newsletter/' + slug + '/subscribe?email=' + email, {
      headers: this.headers
    }).pipe(catchError(this.handleError));
  }

  verifyCode(slug: any, code: any) {
    return this.http.get('api/newsletter/' + slug + '/verifycode?code=' + code, {
      headers: this.headers
    }).pipe(catchError(this.handleError));
  }
}
