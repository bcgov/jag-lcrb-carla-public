import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { faQuestionCircle } from '@fortawesome/free-solid-svg-icons';
import { SepApplication } from '@models/sep-application.model';
import { IndexDBService } from '@services/index-db.service';
import { FormBase } from '@shared/form-base';
import { distinctUntilChanged } from 'rxjs/operators';
import { Account } from '@models/account.model';

@Component({
  selector: 'app-event',
  templateUrl: './event.component.html',
  styleUrls: ['./event.component.scss']
})
export class EventComponent extends FormBase implements OnInit {
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
      eligibilityMajorSignificanceRationale: ['', [Validators.required]],
      eligibilityLocalSignificance: [false, [Validators.required]],
      eventStartDate: ['', [Validators.required]],
      eligibilityPrivateOrPublic: ['', [Validators.required]],
      eligibilityResponsibleBevServiceNumber: ['', [Validators.required]],
      eligibilityResponsibleBevServiceNumberDoesNotHave: [false],
      eventDescription: ['', [Validators.required]],
      eligibilityChargingAdmission: ['', [Validators.required]],
      eligibilityLocalIsLicensed: [null, [Validators.required]],
      hostingOrganizationName: [''],
      hostingOrganizationAddress: [''],
      hostingOrganizationCategory: [''],
    });


    this.form.get('eligibilityPrivateOrPublic').valueChanges
    .pipe(distinctUntilChanged())
      .subscribe(selectedValue => {
        if (selectedValue && selectedValue !== '1') {
          // if not "Private – Family and invited friends only" then make all hostingOrganization fields required
          this.form.get('hostingOrganizationName').setValidators([Validators.required]);
          this.form.get('hostingOrganizationAddress').setValidators([Validators.required]);
          this.form.get('hostingOrganizationCategory').setValidators([Validators.required]);
        } else { // otherwise clear validators
          this.form.get('hostingOrganizationName').clearAsyncValidators();
          this.form.get('hostingOrganizationAddress').clearAsyncValidators();
          this.form.get('hostingOrganizationCategory').clearAsyncValidators();
        }
      });

    this.form.get('eligibilityResponsibleBevServiceNumber').valueChanges
    .pipe(distinctUntilChanged())
    .subscribe(value => {
        if (value) {
          // if the value is entered disable the checkbox and clear its validation
          this.form.get('eligibilityResponsibleBevServiceNumberDoesNotHave').clearAsyncValidators();
          this.form.get('eligibilityResponsibleBevServiceNumberDoesNotHave').disable();
        } else {
          // enable the checkbox and its validation
          this.form.get('eligibilityResponsibleBevServiceNumberDoesNotHave').setValidators([this.customRequiredCheckboxValidator()]);
          this.form.get('eligibilityResponsibleBevServiceNumberDoesNotHave').enable();
        }
      });

    this.form.get('eligibilityResponsibleBevServiceNumberDoesNotHave').valueChanges
    .pipe(distinctUntilChanged())
    .subscribe(value => {
        if (value === false) {
          this.form.get('eligibilityResponsibleBevServiceNumber').setValidators([Validators.required, Validators.min(6), Validators.max(6)]);
          this.form.get('eligibilityResponsibleBevServiceNumber').enable();
        } else {
          this.form.get('eligibilityResponsibleBevServiceNumber').clearValidators();
          this.form.get('eligibilityResponsibleBevServiceNumber').disable();
        }
      });

    if (this.sepApplication) {
      this.form.patchValue(this.sepApplication);
    }

  }

  isValid() {
    this.markControlsAsTouched(this.form);
    this.validationMessages = this.listControlsWithErrors(this.form, {});
    return this.form.valid;
  }

  save() {
    const data = {
      ...this.sepApplication,
      lastUpdated: new Date(),
      status: 'unsubmitted',
      stepsCompleted: (steps => {
        const step = 'event';
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
