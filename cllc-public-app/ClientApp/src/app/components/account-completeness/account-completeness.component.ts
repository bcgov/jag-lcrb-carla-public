import { Component, OnInit, Input } from '@angular/core';
import { Subscription } from 'rxjs';
import { ApplicationDataService } from '@services/application-data.service';
import { map } from 'rxjs/operators';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { DocumentTypeStatus } from '@models/document-type-status.model';
import { FileDataService } from '@services/file-data.service';

@Component({
  selector: 'app-account-completeness',
  templateUrl: './account-completeness.component.html',
  styleUrls: ['./account-completeness.component.scss']
})
export class AccountCompletenessComponent implements OnInit {
  @Input('entityName') entityName: string;
  @Input('entityId') entityId: string;
  @Input('formId') formId: string;

  public documentTypeStatusResult: DocumentTypeStatus[] = []; 

  constructor(private http: HttpClient, private fileDataService: FileDataService) { }

  ngOnInit() {
    if (this.entityName && this.entityId) {

    
      this.fileDataService.getDocumentStatus(this.entityName, this.entityId, this.formId)
        .subscribe((documentTypeStatusResult) => {
          this.documentTypeStatusResult = documentTypeStatusResult;

        

      });
    }
  }

}
