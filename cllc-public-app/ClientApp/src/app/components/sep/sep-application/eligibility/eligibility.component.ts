import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Account } from '@models/account.model';
import { SepApplication } from '@models/sep-application.model';
import { trigger, state, style, animate, transition } from '@angular/animations';
import { IndexedDBService } from '@services/indexed-db.service';
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
  saveComplete = new EventEmitter<SepApplication>();
  faQuestionCircle = faQuestionCircle;
  form: FormGroup;
  showValidationMessages: boolean;
  validationMessages: string[];
  get minDate() {
    return new Date();
  }

  @Input()
  set localId(value: number) {
    this._appID = value;
    if (!value) {
      return;
    }
    //get the last saved application
    this.db.getSepApplication(value)
      .then(app => {
        this.sepApplication = app;
        if (this.form && app) {
          this.form.patchValue(this.sepApplication);
        }
      });
  };

  constructor(private fb: FormBuilder,
    private router: Router,
    private db: IndexedDBService) {
    super();
  }

  ngOnInit(): void {
    this.form = this.fb.group({
      isPrivateResidence: ['', [Validators.required]],
      isOnPublicProperty: ['', [Validators.required]],
      isMajorSignificance: [false, [Validators.required]],
      majorSignificanceRationale: [''],
      isLocalSignificance: ['', [Validators.required]],
      eventStartDate: ['', [Validators.required]],
      privateOrPublic: ['', [Validators.required]],
      responsibleBevServiceNumber: [''],
      responsibleBevServiceNumberDoesNotHave: [false],
      specialEventDescription: ['', [Validators.required, Validators.maxLength(255)]],
      admissionFee: ['', [Validators.required]],
      tastingEvent: ['', [Validators.required]],
      isLocationLicensed: [null, [Validators.required]],
      hostOrganizationName: [''],
      hostOrganizationAddress: [''],
      hostOrganizationCategory: [''],
    });

    this.form.get('privateOrPublic').valueChanges
      .pipe(distinctUntilChanged())
      .subscribe(selectedValue => {
        // if not Private – Family and invited friends only
        if (selectedValue && selectedValue !== 'Family') {
          this.form.get('hostOrganizationName').setValidators([Validators.required]);
          this.form.get('hostOrganizationAddress').setValidators([Validators.required]);
          this.form.get('hostOrganizationCategory').setValidators([Validators.required]);
        } else {
          this.form.get('hostOrganizationName').clearValidators();
          this.form.get('hostOrganizationName').reset();
          this.form.get('hostOrganizationAddress').clearValidators();
          this.form.get('hostOrganizationAddress').reset();
          this.form.get('hostOrganizationCategory').clearValidators();
          this.form.get('hostOrganizationCategory').reset();
        }
      });

    this.form.get('isMajorSignificance').valueChanges
      .pipe(distinctUntilChanged())
      .subscribe((hasMajorSignificance: boolean) => {
        if (hasMajorSignificance) {
          this.form.get('majorSignificanceRationale').setValidators([Validators.required]);
        } else {
          this.form.get('majorSignificanceRationale').clearValidators();
          this.form.get('majorSignificanceRationale').reset();
        }
      });

    if (this.sepApplication) {
      this.form.patchValue(this.sepApplication);
    }

  }

  isValid() {
    this.markControlsAsTouched(this.form);
    this.validationMessages = this.listControlsWithErrors(this.form, {
      isPrivateResidence: 'Please indicate whether this event is being hosted at a private residence',
      isOnPublicProperty: 'Please indicate whether this event is being held on public property',
      isMajorSignificance: 'Is this an event of provincial, national, or international significance? ',
      isLocalSignificance: 'Is this an event designated by municipal council as an event of municipal significance? ',
      eventStartDate: 'Please enter the Event start date',
      privateOrPublic: 'Is your event private or public?',
      responsibleBevServiceNumber: ' Please enter the Responsible Beverage Service Number',
      specialEventDescription: 'What’s the occasion of your event?',
      admissionFee: 'Are you charging an event admission price?',
      tastingEvent: 'Is this a tasting event?',
      isLocationLicensed: 'Is there currently a liquor licence at your event location?',
      hostOrganizationName: 'What is the name of the organization hosting the event? ',
      hostOrganizationAddress: 'What is the address of the organization?',
      hostOrganizationCategory: 'Select a category that best describes the organization hosting the event',
    });
    const bevMsg = this.bevNumberValidationMessage();
    if (bevMsg !== null) {
      this.validationMessages.push(bevMsg);
    }

    const valid = this.form.valid
      && !this.form.get('isPrivateResidence').value === true
      && bevMsg === null;
    return valid;
  }

  bevNumberValidationMessage(): string {
    const bevNumber: string = this.form.get('responsibleBevServiceNumber').value;
    const bevNumberDoesNotHave: boolean = this.form.get('responsibleBevServiceNumberDoesNotHave').value;
    let message: string = null;
    if (bevNumberDoesNotHave === true && bevNumber) {
      message = 'Invalid choice. Data values in conflict';
    } else if (bevNumberDoesNotHave !== true && !bevNumber) {
      message = 'Please enter the Responsible Beverage Service Number';
    } else if (bevNumber && bevNumber.length !== 6 && bevNumber.length !== 22) {
      message = 'The Responsible Beverage Service Number can only be 6 or 22 characters long';
    }
    return message;
  }

  getFormData(): SepApplication {
    const data = {
      ...this.sepApplication,
      lastUpdated: new Date(),
      eventStatus: 'Draft',
      lastStepCompleted: 'eligibility',
      ...this.form.value
    } as SepApplication;
    return data;
  }

  next() {
    this.showValidationMessages = false;
    if (this.isValid()) {
      this.saveComplete.emit(this.getFormData());
    } else {
      this.showValidationMessages = true;
    }
  }
}
