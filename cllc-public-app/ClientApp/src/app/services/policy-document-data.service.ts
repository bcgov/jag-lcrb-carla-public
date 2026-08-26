import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { PolicyDocumentSummary } from '@models/policy-document-summary.model';
import { PolicyDocument } from '@models/policy-document.model';
import { catchError } from 'rxjs/operators';
import { DataService } from './data.service';

@Injectable()
export class PolicyDocumentDataService extends DataService {
  constructor(private http: HttpClient) {
    super();
  }

  getPolicyDocument(slug: string) {
    return this.http
      .get<PolicyDocument>(`api/policydocument/${slug}`, {
        headers: this.headers
      })
      .pipe(catchError(this.handleError));
  }

  getPolicyDocuments(category: string) {
    return this.http
      .get<PolicyDocumentSummary[]>(`api/policydocument?category=${category}`, {
        headers: this.headers
      })
      .pipe(catchError(this.handleError));
  }
}
