import { ChangeDetectorRef, Component, OnInit } from "@angular/core";
import { MatDialogRef } from "@angular/material/dialog";
import { FeedbackService } from "@services/feedback.service";
import { MatSnackBar } from "@angular/material/snack-bar";

@Component({
  selector: "app-feedback",
  templateUrl: "./feedback.component.html",
  styleUrls: ["./feedback.component.scss"],
})
export class FeedbackComponent {
  feedbackText: string = "";

  isSubmitted: boolean = false;
  isError: boolean = false;
  showValidationError: boolean = false;

  isLoading: boolean = false;

  constructor(
    private dialogRef: MatDialogRef<FeedbackComponent>,
    private cd: ChangeDetectorRef,
    private feedbackService: FeedbackService,
    public snackBar: MatSnackBar
  ) {}

  submitFeedback() {
    if (this.feedbackText.trim().length < 5) {
      this.snackBar.open(`Please enter at least 5 characters.`,
        "Required",
        { duration: 3500, panelClass: ["red-snackbar"] });
      return;
    }

    this.isLoading = true;
    this.cd.detectChanges();

    this.feedbackService.saveFeedback(this.feedbackText).subscribe(
      (response) => {
        this.isLoading = false;
        this.isSubmitted = true;
        this.isError = !response.message;
        this.cd.detectChanges();
      },
      (error) => {
        console.error("Error fetching data:", error);
        this.isLoading = false;
        this.isError = true;
        this.cd.detectChanges();
      }
    );
  }

  closeFeedbackDialog() {
    this.dialogRef.close();
  }

}
