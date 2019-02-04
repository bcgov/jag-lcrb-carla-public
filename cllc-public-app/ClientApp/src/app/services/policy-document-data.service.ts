import { Injectable } from '@angular/core';
import { Http, Headers, Response } from '@angular/http';

import { PolicyDocument } from '../models/policy-document.model';
import { PolicyDocumentSummary } from '../models/policy-document-summary.model';
import { HttpClient } from '@angular/common/http';
import { DataService } from './data.service';
import { catchError } from 'rxjs/operators';

@Injectable()
export class PolicyDocumentDataService extends DataService {
  constructor(private http: HttpClient) {
    super();
  }

  getPolicyDocument(slug: any) {

    return this.http.get<PolicyDocument>('api/policydocument/' + slug, {
      headers: this.headers
    }).pipe(catchError(this.handleError));
  }

  getPolicyDocuments(category: string) {

    return this.http.get<PolicyDocumentSummary[]>('api/policydocument?category=' + category, {
      headers: this.headers
    }).pipe(catchError(this.handleError));
  }
}
