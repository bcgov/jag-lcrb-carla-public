import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PolicyDocument } from '../models/policy-document.model';
import { PolicyDocumentDataService } from '../services/policy-document-data.service';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-policy-document',
  templateUrl: './policy-document.component.html',
  styleUrls: ['./policy-document.component.scss']
})
/** PolicyDocument component*/
export class PolicyDocumentComponent implements OnInit {
  public policyDocument: PolicyDocument;
  public title: string;
  public category: string;
  public body: string;

  /** PolicyDocument ctor */
  constructor(private policyDocumentDataService: PolicyDocumentDataService,
    private titleService: Title,
    private route: ActivatedRoute) {

  }

  ngOnInit(): void {
    this.route.params.subscribe((data: any) => {
      this.setSlug(data.slug);
    });
  }

  setSlug(slug) {
    this.policyDocumentDataService.getPolicyDocument(slug).then((data) => {
      this.policyDocument = data;
      this.title = this.policyDocument.title;
      this.body = this.policyDocument.body;
      this.category = this.policyDocument.category;
      this.titleService.setTitle(`${this.title} - Liquor and Cannabis Regulation Branch`);
    });
  }

}
