import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from "@angular/router";
import { PolicyDocument } from "../models/policy-document.model";
import { PolicyDocumentDataService } from "../services/policy-document-data.service";

@Component({
    selector: 'app-policy-document',
    templateUrl: './policy-document.component.html',
    styleUrls: ['./policy-document.component.scss']
})
/** PolicyDocument component*/
export class PolicyDocumentComponent {
  private policyDocument: PolicyDocument;
  private title: string;
  private intro: string;
  private body: string;

  /** PolicyDocument ctor */
  constructor(private policyDocumentDataService: PolicyDocumentDataService, private route: ActivatedRoute) {

  }

  ngOnInit(): void {    
    const slug = this.route.snapshot.params["slug"];
    this.policyDocumentDataService.getPolicyDocument(slug)
      .then((data) => {
        this.policyDocument = data;
        this.title = this.policyDocument.title;
        this.body = this.policyDocument.body;
        this.intro = this.policyDocument.intro;
      });
  }

}
