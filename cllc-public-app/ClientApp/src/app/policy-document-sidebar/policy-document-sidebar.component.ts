import { Component } from '@angular/core';
import { PolicyDocumentDataService } from "../services/policy-document-data.service";
import { PolicyDocumentSummary } from "../models/policy-document-summary.model";
@Component({
    selector: 'app-policy-document-sidebar',
    templateUrl: './policy-document-sidebar.component.html',
    styleUrls: ['./policy-document-sidebar.component.scss']
})
/** PolicyDocumentSidebar component*/
export class PolicyDocumentSidebarComponent {
  public policyDocumentSummaries:PolicyDocumentSummary[];
    /** PolicyDocumentSidebar ctor */
  constructor(private policyDocumentDataService: PolicyDocumentDataService) {
  }

  ngOnInit(): void {   
    this.policyDocumentDataService.getPolicyDocuments()
        .then((data) => {
          this.policyDocumentSummaries = data;          
        });    
    }
}
