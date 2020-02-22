import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'app-eligibility-form',
  templateUrl: './eligibility-form.component.html',
  styleUrls: ['./eligibility-form.component.scss']
})
export class EligibilityFormComponent implements OnInit {
  eligibilityForm = this.fb.group({
    isConnectedToUnlicencedStore: [null, [Validators.required]],
    isInvolvedIllegalDistribution: [null, [Validators.required]],
    isEligibilityCertified: [null, [Validators.required]],
    eligibilitySignature: [null, [Validators.required]],
    dateSignedOrDismissed: [null, [Validators.required]],
    nameLocationUnlicencedRetailer: [null, []],
    isRetailerStillOperating: [null, []],
    dateOperationsCeased: [null, []],
    illegalDistributionInvolvementDetails: [null, []],
    nameLocationRetailer: [null, []],
    isInvolvementContinuing: [null, []],
    dateInvolvementCeased: [null, []]
  });

  constructor(
    private fb: FormBuilder
  ) { }

  ngOnInit() {
    this.eligibilityForm.controls['dateSignedOrDismissed'].setValue(new Date());
  }


}
