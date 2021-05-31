import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, FormGroupDirective, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AppState } from '@app/app-state/models/app-state';
import { PolicyDocumentComponent } from '@components/policy-document/policy-document.component';
import { Account } from '@models/account.model';
import { Contact } from '@models/contact.model';
import { SepApplication } from '@models/sep-application.model';
import { Store } from '@ngrx/store';
import { ContactDataService } from '@services/contact-data.service';
import { IndexedDBService } from '@services/indexed-db.service';
import { SpecialEventsDataService } from '@services/special-events-data.service';
import { FormBase } from '@shared/form-base';
import { Observable } from 'rxjs';
import { from } from 'rxjs/internal/observable/from';
import { of } from 'rxjs/internal/observable/of';
import { map } from 'rxjs/operators';

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
  contact: Contact;
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
  saveComplete = new EventEmitter<SepApplication>();
  form: FormGroup;


  constructor(private fb: FormBuilder,
    private router: Router,
    private store: Store<AppState>,
    private contactDataService: ContactDataService,
    private sepDataService: SpecialEventsDataService,
    private db: IndexedDBService) {
    store.select(state => state.currentUserState.currentUser)
      .subscribe(user => {
        contactDataService.getContact(user.contactid)
          .subscribe(contact => {
            this.contact = contact;
            //debugger;
          });
      });
  }

  ngOnInit(): void {
    this.form = this.fb.group({
      eventName: ['', [Validators.required]],
      applicantInfo: [''],
      isAgreeTsAndCs: ['', [this.customRequiredCheckboxValidator()]],
      dateAgreedToTsAndCs: ['']
    });

    if (this.application) {
      this.form.patchValue(this.application);
    }

    this.form.get('isAgreeTsAndCs').valueChanges
      .subscribe((agree: boolean) => {
        if (agree) {
          this.form.get('dateAgreedToTsAndCs').setValue(new Date());
        }
      });

    this.policyDocs.setSlug(this.policySlug);
  }

  customRequiredCheckboxValidator(): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } | null => {
      if (control.value === true) {
        return null;
      } else {
        return { 'shouldBeTrue': 'But value is false' };
      }
    };
  }

  isValid() {
    this.form.markAsTouched();
    return this.form.valid;
  }

  save(): Observable<number> {
    const data = {
      ...this.application,
      lastUpdated: new Date(),
      status: 'unsubmitted',
      stepsCompleted: (steps => {
        const step = 'applicant';
        if (steps.indexOf(step) === -1) {
          steps.push(step);
        }
        return steps;
      })(this?.application?.stepsCompleted || []),
      ...this.form.value
    } as SepApplication;

    if (data.localId) {
      this.db.applications.update(data.localId, data);
      return of(data.localId);
    } else {
      data.dateCreated = new Date();
      return from(this.db.saveSepApplication(data));
    }
  }

  saveToAPI() {
    this.db.getSepApplication(this?.application?.localId)
      .then((appData) => {
        if (appData.id) { // do an update ( the record exists in dynamics)
          this.sepDataService.updateSepApplication({ ...appData, invoiceTrigger: 1 }, appData.id)
            .subscribe(result => {
              if (result.localId) {
                this.db.applications.update(result.localId, result);
              }
            });
        }
      });
  }

  next() {
    const data = {
      ...this.application,
      lastUpdated: new Date(),
      status: 'unsubmitted',
      stepsCompleted: (steps => {
        const step = 'applicant';
        if (steps.indexOf(step) === -1) {
          steps.push(step);
        }
        return steps;
      })(this?.application?.stepsCompleted || []),
      ...this.form.value
    } as SepApplication;
    if (this.isValid()) {
      this.saveComplete.emit(data);
    }
  }


}
