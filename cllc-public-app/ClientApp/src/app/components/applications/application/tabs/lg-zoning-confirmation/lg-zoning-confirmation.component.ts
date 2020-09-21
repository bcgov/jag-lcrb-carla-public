import { Component, OnInit, Input } from '@angular/core';
import { FormBase } from '@shared/form-base';
import { Application } from '@models/application.model';
import { FormBuilder, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material';
import { ApplicationDataService } from '@services/application-data.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-lg-zoning-confirmation',
  templateUrl: './lg-zoning-confirmation.component.html',
  styleUrls: ['./lg-zoning-confirmation.component.scss']
})
export class LgZoningConfirmationComponent extends FormBase implements OnInit {
  @Input() application: Application;
  @Input() isOpenedByLGForApproval: boolean;
  @Input() disableForm = false;
  busy: any;
  validationMessages: string[];


  constructor(private fb: FormBuilder,
    private snackBar: MatSnackBar,
    private router: Router,
    private applicationDataService: ApplicationDataService) {
    super();
  }

  ngOnInit() {
    this.form = this.fb.group({
      lgInName: [{ value: '', disabled: true }],
      lGNameOfOfficial: ['', [Validators.required]],
      lGTitlePosition: ['', [Validators.required]],
      lGContactPhone: ['', [Validators.required]],
      lGContactEmail: ['', [Validators.required, Validators.email]],
      lgZoning: ['', [Validators.required]],
      lGDecisionComments: [''],
    });
    this.form.patchValue(this.application);

    if (this.disableForm) {
      this.form.disable();
    }
  }

  isValid() {
    this.markControlsAsTouched(this.form);
    this.validationMessages = this.listControlsWithErrors(this.form);
    return this.form.valid;
  }

  submit() {
    if (!this.isValid()) {
      return;
    }

    let data = <Application>{
      ...this.application,
      ...this.form.value,
      // lGApprovalDecision: 'Approved',
      lGDecisionSubmissionDate: new Date()
    };

    this.busy = this.applicationDataService.updateApplication(data)
      .subscribe(res => {
        this.snackBar.open('Zoning decision submitted', 'Success', { duration: 2500, panelClass: ['green-snackbar'] });
        this.router.navigateByUrl('/lg-approvals');
      }, error => {
        this.snackBar.open('Error submitting zoning decision', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
      });

  }

  saveForLater() {
    let data = <Application>{
      ...this.application,
      ...this.form.value
    };

    this.busy = this.applicationDataService.updateApplication(data)
      .subscribe(res => {
        this.snackBar.open('Zoning decision saved', 'Success', { duration: 2500, panelClass: ['green-snackbar'] });
        this.router.navigateByUrl('/lg-approvals');
      }, error => {
        this.snackBar.open('Error saving zoning decision', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
      });

  }

}
