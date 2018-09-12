import { Component, OnInit, ViewChild } from '@angular/core';
import { PaymentDataService } from '../../services/payment-data.service';
import { UserDataService } from '../../services/user-data.service';
import { WorkerDataService } from '../../services/worker-data.service.';
import { User } from '../../models/user.model';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Subscription } from 'rxjs/Subscription';
import { Observable } from 'rxjs/Observable';
import { Subject } from 'rxjs/Subject';
import { FileUploaderComponent } from '../../file-uploader/file-uploader.component';

@Component({
  selector: 'app-spd-consent',
  templateUrl: './spd-consent.component.html',
  styleUrls: ['./spd-consent.component.scss']
})
export class SpdConsentComponent implements OnInit {
  @ViewChild('mainForm') mainForm: FileUploaderComponent;
  currentUser: any;
  workerId: string;
  form: FormGroup;
  busy: Subscription;
  infoAccurate = false;
  consentToCollection = false;
  signName: string;
  saveFormData: any;
  showValidationMessages: boolean;
  validationMessages: string[];

  constructor(
    private fb: FormBuilder,
    private paymentDataService: PaymentDataService,
    private workerDataService: WorkerDataService,
    private userDataService: UserDataService,
    private router: Router,
    private route: ActivatedRoute) {
    this.route.params.subscribe(params => {
      this.workerId = params.id;
    });
  }

  ngOnInit() {
    this.reloadUser();

    this.form = this.fb.group({
      id: [],
      selfdisclosure: [''],
    });
  }


  reloadUser() {
    this.userDataService.getCurrentUser()
      .subscribe((data: User) => {
        this.currentUser = data;
      });

    this.busy = this.workerDataService.getWorker(this.workerId).subscribe(res => {
      this.form.patchValue(res);
      this.saveFormData = this.form.value;
    });
  }

  isValid(): boolean {
    this.showValidationMessages = false;
    let valid = true;
    this.validationMessages = [];
    if (!this.mainForm || !this.mainForm.files || this.mainForm.files.length < 1) {
      valid = false;
      this.validationMessages.push('Signature form is required.');
    }
    if (!this.signName || !this.consentToCollection || !this.infoAccurate) {
      valid = false;
      this.validationMessages.push('Please complete the "Declaration and Consent" section');
    }
    if (!(this.form.value.selfdisclosure === true || this.form.value.selfdisclosure === false)) {
      valid = false;
      this.validationMessages.push('Please complete the "Criminal Background" section');

    }

    return valid;
  }

  formValid() {
    return this.infoAccurate
      && (this.mainForm && this.mainForm.files && this.mainForm.files.length > 0)
      && this.signName
      && this.consentToCollection
      && (this.form.value.selfdisclosure === true || this.form.value.selfdisclosure === false);
  }

  canDeactivate(): Observable<boolean> | boolean {
    if (JSON.stringify(this.saveFormData) === JSON.stringify(this.form.value)) {
      return true;
    } else {
      return this.save();
    }
  }

  save(): Subject<boolean> {
    const subResult = new Subject<boolean>();
    const worker = this.form.value;

    this.busy = this.workerDataService.updateWorker(worker, worker.id).subscribe(res => {
      subResult.next(true);
      this.reloadUser();
    }, err => subResult.next(false)
    );
    return subResult;
  }

  goToNextStep() {
    if (this.isValid()) {
      this.router.navigate([`/worker-registration/pre-payment/${this.workerId}`]);
    } else {
      this.showValidationMessages = true;
    }
  }

}
