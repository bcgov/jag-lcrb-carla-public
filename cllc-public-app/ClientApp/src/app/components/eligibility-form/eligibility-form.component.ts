import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'app-eligibility-form',
  templateUrl: './eligibility-form.component.html',
  styleUrls: ['./eligibility-form.component.scss']
})
export class EligibilityFormComponent implements OnInit {
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
    private fb: FormBuilder
  ) { }

  ngOnInit() {
    this.eligibilityForm.controls['dateSignedOrDismissed'].setValue(new Date());
  }

}
