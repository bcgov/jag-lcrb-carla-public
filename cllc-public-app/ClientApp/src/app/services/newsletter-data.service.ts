import { Injectable } from '@angular/core';
import { Http, Headers, Response } from '@angular/http';


import { Newsletter } from '../models/newsletter.model';

@Injectable()
export class NewsletterDataService {
   constructor(private http: Http) { }

   getNewsletter(slug: any) {
     const headers = new Headers();
     headers.append('Content-Type', 'application/json');

     return this.http.get('api/newsletter/' + slug, {
       headers: headers
     })
       .toPromise()
       .then((res: Response) => {
         const data = res.json();
         const newsletter = new Newsletter();
         newsletter.id = data.id;
         newsletter.description = data.description;
         newsletter.slug = data.slug;
         newsletter.title = data.title;
         return newsletter;
       })
       .catch(this.handleError);
   }

   signup(slug: any, email: any) {
     const headers = new Headers();
     headers.append('Content-Type', 'application/json');

     return this.http.post('api/newsletter/' + slug + '/subscribe?email=' + email, {
       headers: headers
     })
       .toPromise()
       .then((res: Response) => {
         // do nothing
       })
       .catch(this.handleError);
   }

  verifyCode(slug: any, code: any) {
    const headers = new Headers();
    headers.append('Content-Type', 'application/json');

    return this.http.get('api/newsletter/' + slug + '/verifycode?code=' + code, {
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
