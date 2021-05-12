import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Account } from '@models/account.model';
import { SepApplication } from '@models/sep-application.model';
import { trigger, state, style, animate, transition } from '@angular/animations';
import { IndexDBService } from '@services/index-db.service';
import { faQuestionCircle } from '@fortawesome/free-solid-svg-icons';
import { Router } from '@angular/router';
import { FormBase } from '@shared/form-base';
import { distinct, distinctUntilChanged } from 'rxjs/operators';
@Component({
  selector: 'app-eligibility',
  templateUrl: './eligibility.component.html',
  styleUrls: ['./eligibility.component.scss']
})
export class EligibilityComponent extends FormBase implements OnInit {
  @Input() account: Account;
  _appID: number;
  sepApplication: SepApplication;
  @Output()
  saveComplete = new EventEmitter<boolean>();
  faQuestionCircle = faQuestionCircle;
  form: FormGroup;
  showValidationMessages: boolean;
  validationMessages: string[];
  get minDate() {
    return new Date();
  }

  @Input()
  set applicationId(value: number) {
    this._appID = value;
    //get the last saved application
    this.db.getSepApplication(value)
      .then(app => {
        this.sepApplication = app;
        if (this.form) {
          this.form.patchValue(this.sepApplication);
        }
      });
  };

  constructor(private fb: FormBuilder,
    private router: Router,
    private db: IndexDBService) {
    super();
  }

  ngOnInit(): void {
    this.form = this.fb.group({
      eligibilityAtPrivateResidence: ['', [Validators.required]],
      eligibilityOnPublicProperty: ['', [Validators.required]],
      eligibilityMajorSignificance: [false, [Validators.required]],
      eligibilityMajorSignificanceRationale: [''],
      eligibilityLocalSignificance: [false, [Validators.required]],
      eventStartDate: ['', [Validators.required]],
      eligibilityPrivateOrPublic: ['', [Validators.required]],
      eligibilityResponsibleBevServiceNumber: ['', [Validators.required]],
      eligibilityResponsibleBevServiceNumberDoesNotHave: [false],
      eventDescription: ['', [Validators.required, Validators.maxLength(255)]],
      eligibilityChargingAdmission: ['', [Validators.required]],
      eligibilityLocalIsLicensed: [null, [Validators.required]],
      hostingOrganizationName: [''],
      hostingOrganizationAddress: [''],
      hostingOrganizationCategory: [''],
    });

    this.form.get('eligibilityPrivateOrPublic').valueChanges
      .pipe(distinctUntilChanged())
      .subscribe(selectedValue => {
        // if not Private – Family and invited friends only
        if (selectedValue && selectedValue !== '1') {
          this.form.get('hostingOrganizationName').setValidators([Validators.required]);
          this.form.get('hostingOrganizationAddress').setValidators([Validators.required]);
          this.form.get('hostingOrganizationCategory').setValidators([Validators.required]);
        } else {
          this.form.get('hostingOrganizationName').clearValidators();
          this.form.get('hostingOrganizationName').reset();
          this.form.get('hostingOrganizationAddress').clearValidators();
          this.form.get('hostingOrganizationAddress').reset();
          this.form.get('hostingOrganizationCategory').clearValidators();
          this.form.get('hostingOrganizationCategory').reset();
        }
      });

    this.form.get('eligibilityMajorSignificance').valueChanges
      .pipe(distinctUntilChanged())
      .subscribe((hasMajorSignificance: boolean) => {
        if (hasMajorSignificance) {
          this.form.get('eligibilityMajorSignificanceRationale').setValidators([Validators.required]);
        } else {
          this.form.get('eligibilityMajorSignificanceRationale').clearValidators();
          this.form.get('eligibilityMajorSignificanceRationale').reset();
        }
      });


    // this.form.get('eligibilityResponsibleBevServiceNumber').valueChanges
    //   .pipe(distinctUntilChanged())
    //   .subscribe(value => {
    //     if (value) {
    //       // if the value is entered disable the checkbox and clear its validation
    //       this.form.get('eligibilityResponsibleBevServiceNumberDoesNotHave').clearValidators();
    //       this.form.get('eligibilityResponsibleBevServiceNumberDoesNotHave').reset();
    //       this.form.get('eligibilityResponsibleBevServiceNumberDoesNotHave').disable();
    //     } else {
    //       // enable the checkbox and its validation
    //       this.form.get('eligibilityResponsibleBevServiceNumberDoesNotHave').setValidators([this.customRequiredCheckboxValidator()]);
    //       this.form.get('eligibilityResponsibleBevServiceNumberDoesNotHave').enable();
    //     }
    //   });

    // this.form.get('eligibilityResponsibleBevServiceNumberDoesNotHave').valueChanges
    //   .pipe(distinctUntilChanged())
    //   .subscribe(value => {
    //     if (value === false) {
    //       this.form.get('eligibilityResponsibleBevServiceNumber').setValidators([Validators.required, Validators.minLength(6), Validators.maxLength(6)]);
    //       this.form.get('eligibilityResponsibleBevServiceNumber').enable();
    //     } else {
    //       this.form.get('eligibilityResponsibleBevServiceNumber').clearValidators();
    //       this.form.get('eligibilityResponsibleBevServiceNumber').reset();
    //       this.form.get('eligibilityResponsibleBevServiceNumber').disable();
    //     }
    //   });

    if (this.sepApplication) {
      this.form.patchValue(this.sepApplication);
    }

  }

  isValid() {
    this.markControlsAsTouched(this.form);
    this.validationMessages = this.listControlsWithErrors(this.form, {});
    return this.form.valid && !this.form.get('eligibilityAtPrivateResidence').value === true;
  }

  save() {
    const data = {
      ...this.sepApplication,
      lastUpdated: new Date(),
      status: 'unsubmitted',
      stepsCompleted: (steps => {
        const step = 'eligibility';
        if (steps.indexOf(step) === -1) {
          steps.push(step);
        }
        return steps;
      })(this?.sepApplication?.stepsCompleted || []),
      ...this.form.value
    } as SepApplication;

    if (data.id) {
      this.db.applications.update(data.id, data);
    } else {
      console.error("The id should already exist at this point.")
    }
  }

  next() {
    this.showValidationMessages = false;
    if (this.isValid()) {
      this.save();
      this.saveComplete.emit(true);
    } else {
      this.showValidationMessages = true;
    }
  }

  saveForLater() {
    this.save();
    this.router.navigateByUrl('/sep/my-applications')
  }

}
