import { Component, OnInit, Input, Inject, ChangeDetectorRef } from "@angular/core";
import { ApplicationDataService } from "@services/application-data.service";
import { Application } from "@models/application.model";
import { FormBuilder, Validators } from "@angular/forms";
import { FormBase, ApplicationHTMLContent } from "@shared/form-base";
import { MatDialogRef, MAT_DIALOG_DATA, MatDialog } from "@angular/material/dialog";
import { MatSnackBar } from "@angular/material/snack-bar";
import { Router } from "@angular/router";
import { takeWhile } from "rxjs/operators";
import { Observable } from "rxjs";

@Component({
  selector: "app-lg-in-confirmation-of-receipt",
  templateUrl: "./lg-in-confirmation-of-receipt.component.html",
  styleUrls: ["./lg-in-confirmation-of-receipt.component.scss"]
})
export class LgInConfirmationOfReceiptComponent extends FormBase implements OnInit {
  @Input()
  application: Application;
  @Input()
  isOpenedByLGForApproval: boolean;
  @Input()
  htmlContent: ApplicationHTMLContent;
  @Input()
  disableForm = false;
  validationMessages: string[];
  busy: any;
  approvingApplication: boolean;
  rejectingApp: boolean;
  optingOutOfComment: boolean;
  uploadedResolutionDocuments: number = 0;
  uploadedStampedFloorPlanDocuments: number = 0;
  showValidationMessages: boolean;
  providingResolution: boolean;
  lgHasApproved = false;

  constructor(private applicationDataService: ApplicationDataService,
    private snackBar: MatSnackBar,
    public dialog: MatDialog,
    private router: Router,
    private cd: ChangeDetectorRef,
    private fb: FormBuilder) {
    super();
  }

  ngOnInit() {
    this.form = this.fb.group({
      lgInName: [{ value: "", disabled: true }],
      lGNameOfOfficial: ["", [Validators.required]],
      lGTitlePosition: ["", [Validators.required]],
      lGContactPhone: ["", [Validators.required]],
      lGContactEmail: ["", [Validators.required, Validators.email]],
      lgNoObjection: ["", []] // not required
    });
    this.lgHasApproved = this?.application?.lGApprovalDecision  === 'Approved';

    this.form.patchValue(this.application);
    if (this.disableForm || this.lgHasApproved) {
      this.form.disable();
    }
  }

  isValid(resolutionRequired: boolean = false) {
    this.showValidationMessages = false;
    this.markControlsAsTouched(this.form);
    this.validationMessages = this.listControlsWithErrors(this.form);
    let valid = this.form.disabled || this.form.valid;

    if (resolutionRequired && ((this.uploadedResolutionDocuments || 0) < 1)) {
      valid = false;
      this.validationMessages.push("At least one site plan document is required.");
    }

    if (!valid) {
      this.showValidationMessages = true;
    }

    return valid;
  }

  OptOutOfComment() {
    if (!this.isValid()) {
      return;
    }

    this.showComfirmation("OptOut", true).subscribe(result => {
      if (result === "OK") {
        this.optingOutOfComment = true;
        this.cd.detectChanges();

        const data = {
          ...this.application,
          ...this.form.value,
          lGApprovalDecision: "OptOut",
          lGDecisionSubmissionDate: new Date()
        } as Application;

        this.busy = this.applicationDataService.updateApplication(data)
          .subscribe(res => {
              this.snackBar.open("Application has been saved",
                "Success",
                { duration: 2500, panelClass: ["green-snackbar"] });
              this.router.navigateByUrl("/lg-approvals");
              this.optingOutOfComment = false;
              this.cd.detectChanges();
            },
            error => {
              this.snackBar.open("Error saving Application", "Fail", { duration: 3500, panelClass: ["red-snackbar"] });
              this.optingOutOfComment = false;
              this.cd.detectChanges();
            });
      }
    });
  }


  RejectApplication() {
    if (!this.isValid(true)) {
      return;
    }

    this.showComfirmation("Reject", true).subscribe(result => {
      if (result === "OK") {
        this.rejectingApp = true;
        this.cd.detectChanges();

        const data = {
          ...this.application,
          ...this.form.value,
          lGApprovalDecision: "Rejected",
          lGDecisionSubmissionDate: new Date()
        } as Application;

        this.busy = this.applicationDataService.updateApplication(data)
          .subscribe(res => {
              this.snackBar.open("Application has been saved",
                "Success",
                { duration: 2500, panelClass: ["green-snackbar"] });
              this.router.navigateByUrl("/lg-approvals");
              this.rejectingApp = false;
              this.cd.detectChanges();
            },
            error => {
              this.snackBar.open("Error saving Application", "Fail", { duration: 3500, panelClass: ["red-snackbar"] });
              this.rejectingApp = false;
              this.cd.detectChanges();
            });
      }
    });


  }


  ApproveApplication() {
    if (!this.isValid()) {
      return;
    }
    const filesUploaded = (this.uploadedResolutionDocuments > 0);
    this.showComfirmation("Approve", filesUploaded).subscribe(result => {
      if (result === "OK") {
        this.approvingApplication = true;
        this.cd.detectChanges();

        const data = {
          ...this.application,
          ...this.form.value,
          lGApprovalDecision: this.uploadedResolutionDocuments > 0 ? "Approved" : "Pending",
          lGDecisionSubmissionDate: new Date()
        } as Application;

        this.busy = this.applicationDataService.updateApplication(data)
          .subscribe(res => {
              this.snackBar.open("Application has been saved",
                "Success",
                { duration: 2500, panelClass: ["green-snackbar"] });
              this.router.navigateByUrl("/lg-approvals");
              this.approvingApplication = false;
              this.cd.detectChanges();
            },
            error => {
              this.snackBar.open("Error saving Application", "Fail", { duration: 3500, panelClass: ["red-snackbar"] });
              this.approvingApplication = false;
              this.cd.detectChanges();
            });
      }
    });
  }


  showComfirmation(category: string, requiredFilesUploaded: boolean): Observable<string> {
    const dialogConfig = {
      disableClose: true,
      autoFocus: true,
      width: "400px",
      data: {
        category,
        requiredFilesUploaded,
        application: this.application
      }
    };

    // open dialog, get reference and process returned data from dialog
    const dialogRef = this.dialog.open(LGDecisionDialogComponent, dialogConfig);
    return dialogRef.afterClosed()
      .pipe(takeWhile(() => this.componentActive));

  }

  ProvideResolution() {
    // Update the status if a resolution file was uploaded and the status is pending
    if (this.isOpenedByLGForApproval &&
      this.uploadedResolutionDocuments > 0 &&
      this.application.lGApprovalDecision == "Pending") {
      const data = {
        ...this.application,
        ...this.form.value,
        lGApprovalDecision: "Approved",
      } as Application;
      this.providingResolution = true;
      this.applicationDataService.updateApplication(data)
        .subscribe(res => {
            this.snackBar.open("Application has been saved",
              "Success",
              { duration: 2500, panelClass: ["green-snackbar"] });
            this.router.navigateByUrl("/lg-approvals");
            this.providingResolution = false;
            this.cd.detectChanges();
          },
          error => {
            this.snackBar.open("Error saving Application", "Fail", { duration: 3500, panelClass: ["red-snackbar"] });
            this.providingResolution = false;
            this.cd.detectChanges();
          });
    }
  }
}

@Component({
  selector: "app-lg-decision-dialog",
  templateUrl: "lg-decision-dialog.component.html",
})
export class LGDecisionDialogComponent {

  category: string;
  application: Application;
  requiredFilesUploaded: boolean;

  constructor(
    public dialogRef: MatDialogRef<LGDecisionDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    this.category = data.category;
    this.requiredFilesUploaded = data.requiredFilesUploaded;
    this.application = data.application;
  }

  accept() {
    this.dialogRef.close("OK");
  }

  cancel() {
    this.dialogRef.close("CANCEL");
  }

}
