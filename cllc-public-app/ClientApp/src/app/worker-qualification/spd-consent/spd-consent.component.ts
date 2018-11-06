import { Component, OnInit, ViewChild } from '@angular/core';
import { PaymentDataService } from '../../services/payment-data.service';
import { UserDataService } from '../../services/user-data.service';
import { WorkerDataService } from '../../services/worker-data.service.';
import { User } from '../../models/user.model';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, NgForm } from '@angular/forms';
import { Subscription } from 'rxjs/Subscription';
import { Observable } from 'rxjs/Observable';
import { Subject } from 'rxjs/Subject';
import { FileUploaderComponent } from '../../file-uploader/file-uploader.component';
import { MatSnackBar } from '@angular/material';

@Component({
  selector: 'app-spd-consent',
  templateUrl: './spd-consent.component.html',
  styleUrls: ['./spd-consent.component.scss']
})
export class SpdConsentComponent implements OnInit {
  @ViewChild('mainForm') mainForm: FileUploaderComponent;
  @ViewChild('name') nameInputRef: NgForm;
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
  workerStatus: string;

  constructor(
    private fb: FormBuilder,
    private workerDataService: WorkerDataService,
    private userDataService: UserDataService,
    private paymentDataService: PaymentDataService,
    public snackBar: MatSnackBar,
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
    this.busy = this.userDataService.getCurrentUser()
      .subscribe((data: User) => {
        this.currentUser = data;
      });

    this.busy = this.workerDataService.getWorker(this.workerId).subscribe(res => {
      this.form.patchValue(res);
      this.workerStatus = res.status;
      this.saveFormData = this.form.value;
    });
  }

  isValid(): boolean {
    this.showValidationMessages = false;
    let valid = true;
    if (!this.isFileUploadValid()) {
      valid = false;
    }
    if (!this.isDeclarationValid()) {
      valid = false;
    }
    if (!this.isCriminalBackgroundValid()) {
      valid = false;
    }

    return valid;
  }

  isCriminalBackgroundValid(): boolean {
    const valid = (this.form.value.selfdisclosure === true || this.form.value.selfdisclosure === false);
    return valid;
  }

  isDeclarationValid(): boolean {
    const valid = !!(this.signName && this.consentToCollection && this.infoAccurate);
    return valid;
  }

  isFileUploadValid(): boolean {
    const valid = !!(this.mainForm && this.mainForm.files && this.mainForm.files.length > 0);
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
    if (this.workerStatus !== 'Application Incomplete' ||
      JSON.stringify(this.saveFormData) === JSON.stringify(this.form.value)) {
      return true;
    } else {
      return this.save();
    }
  }

  save(): Subject<boolean> {
    const subResult = new Subject<boolean>();
    const worker = this.form.value;

    this.busy = this.workerDataService.updateWorker(worker, worker.id).subscribe(() => {
      subResult.next(true);
      this.reloadUser();
    }, () => subResult.next(false)
    );
    return subResult;
  }

  goToNextStep() {
    if (this.isValid()) {
      this.submitPayment();
    } else {
      this.showValidationMessages = true;
      this.nameInputRef.control.markAsTouched();
    }
  }

  /**
* Redirect to payment processing page (Express Pay / Bambora service)
* */
  private submitPayment() {
    this.save().subscribe(r => {
      this.busy = this.paymentDataService.getWorkerPaymentSubmissionUrl(this.workerId).subscribe(res => {
        const jsonUrl = res.json();
        window.location.href = jsonUrl['url'];
        return jsonUrl['url'];
      }, err => {
        if (err._body === 'Payment already made') {
          this.snackBar.open('Application payment has already been made.', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
        }
      });
    });
  }

}
