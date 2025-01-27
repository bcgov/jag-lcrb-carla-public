import { ChangeDetectorRef, Component, OnInit } from "@angular/core";
import { MatDialogRef } from "@angular/material/dialog";
import { FeedbackService } from "@services/feedback.service";
import { ca } from "date-fns/locale";

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
    private feedbackService: FeedbackService
  ) {}

  submitFeedback() {
    if (this.feedbackText.trim().length < 5) {
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

  filterText(event: KeyboardEvent) {
    // const inputElement = event.target as HTMLInputElement;
    // const filteredText = inputElement.value.replace(/[<>{};`~]/g, "");
    // if (this.feedbackText !== filteredText) {
    //   this.feedbackText = filteredText;
    //   inputElement.value = filteredText;
    //   this.cd.detectChanges();
    // }
  }
}
