import { Component, OnInit, ViewChild, AfterViewInit, ChangeDetectionStrategy, ChangeDetectorRef } from "@angular/core";
import { filter } from "rxjs/operators";
import { PolicyDocumentComponent } from "../../policy-document/policy-document.component";
import { MatDialogRef, MatDialog } from "@angular/material/dialog";
import { ActivatedRoute } from "@angular/router";
import { faChevronRight, faIdCard, faQuestion } from "@fortawesome/free-solid-svg-icons";
import { PolicyDocumentDataService } from "@services/policy-document-data.service";
import { DomSanitizer, SafeHtml } from "@angular/platform-browser";
import { PolicyDocument } from "@models/policy-document.model";

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

  faIdCard = faIdCard;
  faQuestion = faQuestion;
  @ViewChild("policyDocs", { static: true })
  policyDocs: PolicyDocumentComponent;

  constructor(public dialog: MatDialog,
    private sanitizer: DomSanitizer,
    private policyDocumentDataService: PolicyDocumentDataService,
    private cd: ChangeDetectorRef) {
  }

  ngOnInit() {
    this.getPolicyDocumentHTML();
  }
  getPolicyDocumentHTML() {
    const policySlug = "worker-qualification-home";
    this.policyDocumentDataService.getPolicyDocument(policySlug)
      .subscribe((data: PolicyDocument) => {
        this.title = data.title;
        this.body = this.sanitizer.bypassSecurityTrustHtml(data.body);
        this.category = data.category;
        this.cd.detectChanges();
      },
        error => {
          console.error('failed to get policy documents', error);
        });
  }

  ngAfterViewInit(): void {
    this.cd.detectChanges();
  }
}



