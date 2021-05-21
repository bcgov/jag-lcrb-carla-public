import { Component, OnInit, ViewChild, AfterViewInit, ChangeDetectionStrategy, ChangeDetectorRef } from "@angular/core";
import { filter } from "rxjs/operators";
import { PolicyDocumentComponent } from "../../policy-document/policy-document.component";
import { MatDialogRef, MatDialog } from "@angular/material/dialog";
import { ActivatedRoute } from "@angular/router";
import { faChevronRight, faIdCard, faQuestion } from "@fortawesome/free-solid-svg-icons";
import { PolicyDocumentDataService } from "@services/policy-document-data.service";
import { DomSanitizer, SafeHtml } from "@angular/platform-browser";
import { PolicyDocument } from "@models/policy-document.model";
import { FeatureFlagDataService } from "@services/feature-flag-data.service";
import { FeatureFlagService } from "@services/feature-flag.service";
import { faExclamationCircle } from "@fortawesome/free-solid-svg-icons"

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class LoginComponent implements OnInit, AfterViewInit {
  title: string;
  category: string;
  body: SafeHtml;
  calloutTitle: string;
  callout: SafeHtml;

  faIdCard = faIdCard;
  faQuestion = faQuestion;
  @ViewChild("policyDocs", { static: true })
  policyDocs: PolicyDocumentComponent;
  window = window;
  disableLogin: boolean;
  faExclamationCircle = faExclamationCircle;

  constructor(public dialog: MatDialog,
    private sanitizer: DomSanitizer,
    private featureFlagService: FeatureFlagService,
    private policyDocumentDataService: PolicyDocumentDataService,
    private cd: ChangeDetectorRef) {
      featureFlagService.featureOn("DisableLogin")
      .subscribe(featureOn => {
        this.disableLogin = featureOn;
      });
  }

  ngOnInit() {
    this.getPolicyDocumentHTML();
  }
  getPolicyDocumentHTML() {
    const policySlug = "sep-welcome";
    this.policyDocumentDataService.getPolicyDocument(policySlug)
      .subscribe((data: PolicyDocument) => {
        this.title = data.title;
        this.body = this.sanitizer.bypassSecurityTrustHtml(data.body);
        this.category = data.category;
        this.cd.detectChanges();
      },
        error => {
          console.error('failed to get body policy documents', error);
        });

    const calloutSlug = "sep-welcome-callout";
    this.policyDocumentDataService.getPolicyDocument(calloutSlug)
      .subscribe((data: PolicyDocument) => {
        this.calloutTitle = data.title;
        this.callout = this.sanitizer.bypassSecurityTrustHtml(data.body);
        this.cd.detectChanges();
      },
        error => {
          console.error('failed to get callout policy document', error);
        });
  }

  ngAfterViewInit(): void {
    this.cd.detectChanges();
  }
}



