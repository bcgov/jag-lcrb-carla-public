import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Account } from '@models/account.model';
import { SepApplication } from '@models/sep-application.model';
import { trigger, state, style, animate, transition } from '@angular/animations';
import { IndexDBService } from '@services/index-db.service';
import { faQuestionCircle } from '@fortawesome/free-solid-svg-icons';
import { Router } from '@angular/router';
@Component({
  selector: 'app-eligibility',
  templateUrl: './eligibility.component.html',
  styleUrls: ['./eligibility.component.scss'],
  // animations: [
  //   trigger(
  //     'inOutAnimation',
  //     [
  //       transition(':enter', [
  //         style({ opacity: 0 }),
  //         animate('3000ms', style({ opacity: 1 })),
  //       ]),
  //       transition(':leave', [
  //         animate('1000ms', style({ opacity: 0 }))
  //       ])
  //     ]
  //   )
  // ]
})
export class EligibilityComponent implements OnInit {
  @Input() account: Account;
  _appID: number;
  application: SepApplication;
  @Output()
  saveComplete = new EventEmitter<boolean>();
  faQuestionCircle = faQuestionCircle;
  form: FormGroup;
  get minDate() {
    return new Date();
  }
  @Input()
  set applicationId(value: number) {
    this._appID = value;
    //get the last saved application
    this.db.getSepApplication(value)
      .then(app => {
        this.application = app;
        if (this.form) {
          this.form.patchValue(this.application);
        }
      });
  };


  constructor(private fb: FormBuilder,
    private router: Router,
    private db: IndexDBService) {
  }

  ngOnInit(): void {
    this.form = this.fb.group({
      eligibilityAtPrivateResidence: ['', [Validators.required]],
      eligibilityOnPublicProperty: ['', [Validators.required]],
      eligibilityMajorSignificance: [false, [Validators.required]],
      eligibilityMajorSignificanceRationale: ['', [Validators.required]],
      eligibilityLocalSignificance: [false, [Validators.required]],
      eventStartDate: ['', [Validators.required]],
      eligibilityPrivateOrPublic: ['', [Validators.required]],
      eligibilityResponsibleBevServiceNumber: ['', [Validators.required]],
      eligibilityResponsibleBevServiceNumberDoesNotHave: ['', [Validators.required]],
      eventDescription: ['', [Validators.required]],
      eligibilityChargingAdmission: ['', [Validators.required]],
      eligibilityLocalIsLicensed: ['', [Validators.required]],
      hostingOrganizationName: ['', [Validators.required]],
      hostingOrganizationAddress: ['', [Validators.required]],
      hostingOrganizationCategory: ['', [Validators.required]],
    });

    if (this.application) {
      this.form.patchValue(this.application);
    }
  }

  isValid() {
    this.form.markAsTouched();
    return this.form.valid;
  }

  save() {
    const data = {
      ...this.application,
      lastUpdated: new Date(),
      status: 'unsubmitted',
      stepsCompleted: (steps => {
        const step = 'eligibility';
        if (steps.indexOf(step) === -1) {
          steps.push(step);
        }
        return steps;
      })(this?.application?.stepsCompleted || []),
      ...this.form.value
    } as SepApplication;

    if (data.id) {
      this.db.applications.update(data.id, data);
    } else {
      console.error("The id should already exist at this point.")
    }
  }

  next() {
    if (this.isValid()) {
      this.save();
      this.saveComplete.emit(true);
    }
  }

  saveForLater() {
    this.save();
    this.router.navigateByUrl('/sep/my-applications')
  }

}
