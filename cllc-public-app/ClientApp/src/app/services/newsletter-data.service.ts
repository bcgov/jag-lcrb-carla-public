import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Newsletter } from '@models/newsletter.model';
import { catchError } from 'rxjs/operators';
import { DataService } from './data.service';

@Injectable()
export class NewsletterDataService extends DataService {
  constructor(private http: HttpClient) {
    super();
  }

  getNewsletter(slug: string) {
    return this.http
      .get<Newsletter>(`api/newsletter/${slug}`, {
        headers: this.headers
      })
      .pipe(catchError(this.handleError));
  }

  signup(slug: string, email: string) {
    return this.http
      .post<Newsletter>(`api/newsletter/${slug}/subscribe?email=${email}`, {
        headers: this.headers
      })
      .pipe(catchError(this.handleError));
  }

  verifyCode(slug: string, code: string) {
    return this.http
      .get(`api/newsletter/${slug}/verifycode?code=${code}`, {
        headers: this.headers
      })
      .pipe(catchError(this.handleError));
  }
}
