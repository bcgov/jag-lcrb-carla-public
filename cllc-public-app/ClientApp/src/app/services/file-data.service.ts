import { Injectable } from '@angular/core';
import { FileSystemItem } from '@models/file-system-item.model';
import { Application } from '@models/application.model';
import { ApplicationSummary } from '@models/application-summary.model';
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


}
