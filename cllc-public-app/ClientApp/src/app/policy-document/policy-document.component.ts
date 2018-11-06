import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PolicyDocument } from '../models/policy-document.model';
import { PolicyDocumentDataService } from '../services/policy-document-data.service';
import { Title, DomSanitizer, SafeHtml } from '@angular/platform-browser';

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
  public body: SafeHtml;
  @Input() fullWidth: false;
  @Output() slugChange = new EventEmitter<string>();
  dataLoaded: boolean;
  busy: Promise<any>;

  /** PolicyDocument ctor */
  constructor(private policyDocumentDataService: PolicyDocumentDataService,
    private titleService: Title,
    private route: ActivatedRoute,
    private sanitizer: DomSanitizer
  ) {

}

  ngOnInit(): void {
    this.route.params
      .filter(data => !!data && !!data.slug)
      .subscribe((data: any) => {
        this.setSlug(data.slug);
      });
  }

  public setSlug(slug) {
    this.slugChange.emit(slug);
    this.busy = this.policyDocumentDataService.getPolicyDocument(slug).then((data) => {
      this.dataLoaded = true;
      this.policyDocument = data;
      this.title = this.policyDocument.title;
      this.body = this.sanitizer.bypassSecurityTrustHtml(this.policyDocument.body);
      this.category = this.policyDocument.category;
      this.titleService.setTitle(`${this.title} - Liquor and Cannabis Regulation Branch`);
    }).catch(error => this.dataLoaded = true);
  }

}
