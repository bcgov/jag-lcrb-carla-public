import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { EligibilityFormDataService } from '@services/eligibility-data.service';
import { pacificToday } from '@shared/pacific-time';

@Component({
  selector: 'app-eligibility-form',
  templateUrl: './eligibility-form.component.html',
  styleUrls: ['./eligibility-form.component.scss']
})
export class EligibilityFormComponent implements OnInit {
  busy: any;
  maxDate: Date;
  eligibilityForm = this.fb.group({
    // question 1
    isConnectedToUnlicencedStore: [null, [Validators.required]],
    nameLocationUnlicencedRetailer: [null, []],
    isRetailerStillOperating: [null, []],
    dateOperationsCeased: [null, []],

    // question 2
    isInvolvedIllegalDistribution: [null, [Validators.required]],
    nameLocationRetailer: [null, []],
    illegalDistributionInvolvementDetails: [null, []],
    isInvolvementContinuing: [null, []],
    dateInvolvementCeased: [null, []],

    // "e-sig"
    isEligibilityCertified: [null, [Validators.required]],
    eligibilitySignature: [null, [Validators.required]],
    dateSignedOrDismissed: [null, [Validators.required]]
  });

  constructor(
    private fb: FormBuilder,
    private service: EligibilityFormDataService,
    private dialogRef: MatDialogRef<EligibilityFormComponent>
  ) {
    this.maxDate = pacificToday();
  }

  ngOnInit() {
    this.eligibilityForm.controls['dateSignedOrDismissed'].setValue(pacificToday());
  }

  submitForm() {
    this.busy = this.service.submit({ ...this.eligibilityForm.value }).subscribe(() => {
      this.dialogRef.close();
    });
  }

  closeForm() {
    this.busy = this.service
      .submit({
        dateSignedOrDismissed: this.eligibilityForm.get('dateSignedOrDismissed').value,
        isEligibilityCertified: false
      })
      .subscribe(() => {
        this.dialogRef.close();
      });
  }
}
