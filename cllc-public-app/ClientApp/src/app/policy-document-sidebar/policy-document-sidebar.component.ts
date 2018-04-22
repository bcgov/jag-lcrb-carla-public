import { Component, Input } from '@angular/core';
import { PolicyDocumentDataService } from "../services/policy-document-data.service";
import { PolicyDocumentSummary } from "../models/policy-document-summary.model";
@Component({
    selector: 'app-policy-document-sidebar',
    templateUrl: './policy-document-sidebar.component.html',
    styleUrls: ['./policy-document-sidebar.component.scss']
})
/** PolicyDocumentSidebar component*/
export class PolicyDocumentSidebarComponent {
  @Input('category') category: string;
  public policyDocumentSummaries:PolicyDocumentSummary[];
    /** PolicyDocumentSidebar ctor */
  constructor(private policyDocumentDataService: PolicyDocumentDataService) {
  }

  ngOnInit(): void {  
    if (this.category != null && this.category.length > 0) {
      this.policyDocumentDataService.getPolicyDocuments(this.category)
        .then((data) => {
          this.policyDocumentSummaries = data;
        });
    }
  }
}
