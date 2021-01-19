import { Injectable } from '@angular/core';
import { catchError } from 'rxjs/operators';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DataService } from './data.service';
import { DocumentTypeStatus } from '../models/document-type-status.model';

@Injectable()
export class FileDataService extends DataService {

  apiPath = 'api/file/';  

  constructor(private http: HttpClient) {
    super();
   }

  /**
   * Get Document Status
   * */
    getDocumentStatus(entityName: string, entityId: string, formId: string): Observable<DocumentTypeStatus[]> {
      return this.http.get<DocumentTypeStatus[]>(this.apiPath + entityName + '/' + entityId + '/documentStatus/' + formId, { headers: this.headers })
      .pipe(catchError(this.handleError));
  }

  uploadPublicCovidDocument(applicationId: any, documentType: string, file: File) {
    const formData = new FormData();
    formData.append('file', file, file.name);

    formData.append('documentType', documentType);

    const headers: HttpHeaders = new HttpHeaders();

    const path = `${this.apiPath}${applicationId}/public-covid-application`;
    return this.http.post<any>(path, formData, { headers: headers })
      .pipe(catchError(this.handleError));
  }


}
