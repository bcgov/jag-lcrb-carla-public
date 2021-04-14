import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { PolicyDocumentComponent } from '@components/policy-document/policy-document.component';
import { Account } from '@models/account.model';
import { SepApplication } from '@models/sep-application.model';
import { IndexDBService } from '@services/index-db.service';

@Component({
  selector: 'app-applicant',
  templateUrl: './applicant.component.html',
  styleUrls: ['./applicant.component.scss']
})
export class ApplicantComponent implements OnInit {
  policySlug = "sep-terms-and-conditions";
  @ViewChild("policyDocs", { static: true })
  policyDocs: PolicyDocumentComponent;
  @Input() account: Account;
  _app: SepApplication = {} as SepApplication;
  @Input()
  set application(value) {
    this._app = value;
    if (this.form) {
      this.form.patchValue(value);
    }
  };
  get application() {
    return this._app;
  }
  @Output()
  saveComplete = new EventEmitter<boolean>();
  form: FormGroup;


  constructor(private fb: FormBuilder,
    private db: IndexDBService) {
  }

  ngOnInit(): void {
    this.form = this.fb.group({
      eventName: [''],
      applicantInfo: [''],
      agreeToTnC: [''],
      dateAgreedToTnC: ['']
    });

    if (this.application) {
      this.form.patchValue(this.application);
    }

    this.form.get('agreeToTnC').valueChanges
      .subscribe((agree: boolean) => {
        if (agree) {
          this.form.get('dateAgreedToTnC').setValue(new Date());
        }
      });

    this.policyDocs.setSlug(this.policySlug);
  }

  save(event) {
    const data = {
      ...this.application,
      lastUpdated: new Date(),
      status: 'unsubmitted',
      stepCompleted: (_=>{
        const step = 'eligibility';
        let steps = this?.application?.stepCompleted || [];
        if(steps.indexOf(step) === -1){
          steps.push(step);
        }
        return steps;
      })(),
      contact: {
        firstname: this?.account?.primarycontact?.firstname,
        lastname: this?.account?.primarycontact?.lastname,
        address1_line1: this?.account?.primarycontact?.address1_line1,
        address1_city: this?.account?.primarycontact?.address1_city,
        address1_stateorprovince: this?.account?.primarycontact?.address1_stateorprovince,
        address1_postalcode: this?.account?.primarycontact?.address1_postalcode,

        telephone1: this?.account?.primarycontact?.telephone1,
        emailaddress1: this?.account?.primarycontact?.emailaddress1,
      },
      ...this.form.value
    } as SepApplication;

    if (data.id) {
      this.db.applications.update(data.id, data);
    } else {
      data.dateCreated = new Date();
      this.db.addSepApplication(data);
    }
    this.saveComplete.emit(true);
  }
}
