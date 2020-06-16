import { Component, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { takeWhile } from 'rxjs/operators';
import { AppState } from '@app/app-state/models/app-state';
import { Store } from '@ngrx/store';
import { User } from '@models/user.model';
import { FormBase } from '@shared/form-base';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-licence-representative-form',
  templateUrl: './licence-representative-form.component.html',
  styleUrls: ['./licence-representative-form.component.scss'],
})
export class LicenceRepresentativeFormComponent extends FormBase implements OnInit {
  busy: Subscription;
  form: FormGroup;

  constructor(
    private fb: FormBuilder,
    private store: Store<AppState>,
    private router: Router,
    private route: ActivatedRoute
    ) {
      super();
      this.route.paramMap.subscribe(params => {
        console.log(params);
      });
      
    }

  ngOnInit() {
    this.form = this.fb.group({
      representativeFullName: ['', [Validators.required]],
      representativePhoneNumber: ['', [Validators.required]],
      representativeEmail: ['', [Validators.required]],
      representativeCanSubmitPermanentChangeApplications: ['', []],
      representativeCanSignTemporaryChangeApplications: ['', []],
      representativeCanObtainLicenceInformation: ['', []],
      representativeCanSignGroceryStoreProofOfSale: ['', []],
      representativeCanAttendEducationSessions: ['', []],
      representativeCanAttendComplianceMeetings: ['', []],
      representativeCanRepresentAtHearings: ['', []],
      signatureAgreement: [false, [Validators.required]]
    });
  }

  save(submit = false) {
    console.log('save');
  }
}
