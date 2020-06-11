import { Component, OnInit, Input } from '@angular/core';
import { ApplicationDataService } from '@services/application-data.service';
import { Application } from '@models/application.model';
import { FormBuilder, Validators } from '@angular/forms';
import { FormBase } from '@shared/form-base';

@Component({
  selector: 'app-lg-in-confirmation-of-receipt',
  templateUrl: './lg-in-confirmation-of-receipt.component.html',
  styleUrls: ['./lg-in-confirmation-of-receipt.component.scss']
})
export class LgInConfirmationOfReceiptComponent extends FormBase implements OnInit {
  @Input() application: Application;
  validationMessages: string[];
  busy: any;

  constructor(private applicationDataService: ApplicationDataService,
    private fb: FormBuilder) {
    super();
  }

  ngOnInit() {
    this.form = this.fb.group({
      lgInName: [{ value: '', disabled: true }],
      lGNameOfOfficial: ['', [Validators.required]],
      lGTitlePosition: ['', [Validators.required]],
      lGContactPhone: ['', [Validators.required]],
      lGContactEmail: ['', [Validators.required, Validators.email]]
    });
    this.form.patchValue(this.application);
  }

  isValid(){
    this.markConstrolsAsTouched(this.form);
    this.validationMessages = this.listControlsWithErrors(this.form);
    return this.form.valid;
  }

  OptOutOfComment() {
    if(!this.isValid()){
      return;
    }

    let data = <Application>{
      ...this.application,
      ...this.form.value,
      lGApprovalDecision: 'OptOut',
      lGDecisionSubmissionDate: new Date()
    };

    this.busy = this.applicationDataService.updateApplication(data)
      .subscribe(res => {
        debugger;
      });
  }


  RejectApplication() {
    if(!this.isValid()){
      return;
    }

    let data = <Application>{
      ...this.application,
      ...this.form.value,
      lGApprovalDecision: 'Rejected',
      lGDecisionSubmissionDate: new Date()
    };

    this.busy = this.applicationDataService.updateApplication(data)
      .subscribe(res => {
        debugger;
      });
  }


  ApproveApplication() {
    if(!this.isValid()){
      return;
    }

    let data = <Application>{
      ...this.application,
      ...this.form.value,
      lGApprovalDecision: 'Approved',
      lGDecisionSubmissionDate: new Date()
    };

    this.busy = this.applicationDataService.updateApplication(data)
      .subscribe(res => {
        debugger;
      });

  }


}
