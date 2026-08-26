import { AfterViewInit, ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
import { faExclamationCircle, faIdCard, faQuestion } from '@fortawesome/free-solid-svg-icons';
import { PolicyDocument } from '@models/policy-document.model';
import { FeatureFlagService } from '@services/feature-flag.service';
import { PolicyDocumentDataService } from '@services/policy-document-data.service';
import { PolicyDocumentComponent } from '../../policy-document/policy-document.component';

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
  @ViewChild('policyDocs', { static: true })
  policyDocs: PolicyDocumentComponent;
  window = window;
  disableLogin: boolean;
  faExclamationCircle = faExclamationCircle;
  source = '/sep/dashboard';

  constructor(
    public dialog: MatDialog,
    private sanitizer: DomSanitizer,
    private featureFlagService: FeatureFlagService,
    private policyDocumentDataService: PolicyDocumentDataService,
    private route: ActivatedRoute,
    private cd: ChangeDetectorRef
  ) {
    featureFlagService.featureOn('DisableLogin').subscribe((featureOn) => {
      this.disableLogin = featureOn;
    });
  }

  ngOnInit() {
    this.getPolicyDocumentHTML();
    this.route.queryParams.subscribe((params) => {
      this.source = params['source'] || '/sep/dashboard';
    });
  }
  getPolicyDocumentHTML() {
    const policySlug = 'sep-welcome';
    this.policyDocumentDataService.getPolicyDocument(policySlug).subscribe(
      (data: PolicyDocument) => {
        this.title = data.title;
        this.body = this.sanitizer.bypassSecurityTrustHtml(data.body);
        this.category = data.category;
        this.cd.detectChanges();
      },
      (error) => {
        console.error('failed to get body policy documents', error);
      }
    );

    const calloutSlug = 'sep-welcome-callout';
    this.policyDocumentDataService.getPolicyDocument(calloutSlug).subscribe(
      (data: PolicyDocument) => {
        this.calloutTitle = data.title;
        this.callout = this.sanitizer.bypassSecurityTrustHtml(data.body);
        this.cd.detectChanges();
      },
      (error) => {
        console.error('failed to get callout policy document', error);
      }
    );
  }

  ngAfterViewInit(): void {
    this.cd.detectChanges();
  }

  sepBCeIDLogin() {
    const returnPath = encodeURIComponent(this.source);
    window.location.href = `login?source=${returnPath}`;
  }

  sepBCServiceLogin() {
    const returnPath = encodeURIComponent(this.source);
    window.location.href = `bcservice?source=${returnPath}`;
  }
}
