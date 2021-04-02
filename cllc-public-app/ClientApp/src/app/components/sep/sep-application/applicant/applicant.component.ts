import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Account } from '@models/account.model';
import { SepApplication } from '@models/sep-application.model';
import { IndexDBService } from '@services/index-db.service';

@Component({
  selector: 'app-applicant',
  templateUrl: './applicant.component.html',
  styleUrls: ['./applicant.component.scss']
})
export class ApplicantComponent implements OnInit {
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
      dateAgreedToTnC: [''],
      stepCompleted: [''],
      status: [''],
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
  }

  save(event) {
    const data = {
      dateCreated: new Date(),
      lastUpdated: new Date(),
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
    };
    
    this.db.addSepApplication(data);
    this.saveComplete.emit(true);
  }
}
