import { Component, OnInit, Input } from '@angular/core';
import { Subscription } from 'rxjs';
import { ApplicationDataService } from '@services/application-data.service';
import { map, takeWhile } from 'rxjs/operators';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { DocumentTypeStatus } from '@models/document-type-status.model';
import { FileDataService } from '@services/file-data.service';
import { FormBase } from '../../shared/form-base';

@Component({
  selector: 'app-account-completeness',
  templateUrl: './account-completeness.component.html',
  styleUrls: ['./account-completeness.component.scss']
})
export class AccountCompletenessComponent extends FormBase implements OnInit {
  @Input('entityName') entityName: string;
  @Input('entityId') entityId: string;
  @Input('formId') formId: string;
  @Input('account') account: Account;

  public documentTypeStatusResult: DocumentTypeStatus[] = []; 

  constructor(private http: HttpClient, private fileDataService: FileDataService) { super(); }

  ngOnInit() {
    if (this.entityName && this.entityId && this.formId) {

      this.fileDataService.getDocumentStatus(this.entityName, this.entityId, this.formId)
        .pipe(takeWhile(() => this.componentActive))
        .subscribe((documentTypeStatusResult) => {
          this.documentTypeStatusResult = documentTypeStatusResult;

        });
    }
    else {
      console.log("Invalid parameters for AccountCompletenessComponent")
      console.log(this.entityName)
      console.log(this.entityId)
      console.log(this.formId)

    }
  }

}
