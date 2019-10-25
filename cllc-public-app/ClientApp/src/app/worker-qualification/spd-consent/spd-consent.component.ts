import { Component, OnInit, ViewChild } from '@angular/core';
import { PaymentDataService } from '../../services/payment-data.service';
import { UserDataService } from '../../services/user-data.service';
import { WorkerDataService } from '../../services/worker-data.service.';
import { User } from '../../models/user.model';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, NgForm } from '@angular/forms';
import { Subscription, Observable, Subject, forkJoin } from 'rxjs';
import { FileUploaderComponent } from '@shared/file-uploader/file-uploader.component';
import { MatSnackBar } from '@angular/material';
import { ContactDataService } from '@services/contact-data.service';

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
  uploadedDocuments = 0;
  submitting: boolean;

  constructor(
    private fb: FormBuilder,
    private workerDataService: WorkerDataService,
    private userDataService: UserDataService,
    private paymentDataService: PaymentDataService,
    private contactDataService: ContactDataService,
    public snackBar: MatSnackBar,
    private route: ActivatedRoute) {
    this.route.paramMap.subscribe(params => {
      this.workerId = params.get('id');
    });
  }

  ngOnInit() {
    this.form = this.fb.group({
      id: [],
      contact: this.fb.group({
        id: [],
        selfDisclosure: [''],
      }),
    });
    this.reloadUser();

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
    const valid = (this.form.value.contact.selfDisclosure === 1 || this.form.value.contact.selfDisclosure === 0);
    return valid;
  }

  isDeclarationValid(): boolean {
    const valid = !!(this.signName && this.consentToCollection && this.infoAccurate);
    return valid;
  }

  isFileUploadValid(): boolean {
    return (this.uploadedDocuments === 1);
  }


  formValid() {
    return this.infoAccurate
      && (this.uploadedDocuments === 1)
      && this.signName
      && this.consentToCollection
      && this.isCriminalBackgroundValid();
  }

  canDeactivate(): Observable<boolean> | boolean {
    if (this.workerStatus !== 'Application Incomplete' ||
      JSON.stringify(this.saveFormData) === JSON.stringify(this.form.value)) {
      return true;
    } else {
      return this.save(true);
    }
  }

  save(trackResult: boolean = false): Subject<boolean> {
    const subResult = new Subject<boolean>();
    const worker = this.form.value;
    worker.selfdisclosure = worker.contact.selfDisclosure;

    const busy = forkJoin(
      this.contactDataService.updateContact(this.form.value.contact),
      this.workerDataService.updateWorker(worker, worker.id)
    ).subscribe(() => {
      subResult.next(true);
      this.reloadUser();
    }, () => subResult.next(false)
    );
    if (trackResult) {
      this.busy = busy;
    }
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
    this.submitting = true;
    this.busy = this.save().subscribe(r => {
      this.busy = this.paymentDataService.getWorkerPaymentSubmissionUrl(this.workerId).subscribe(res => {
        const jsonUrl = res;
        window.location.href = jsonUrl['url'];
        this.submitting = false;
        return jsonUrl['url'];
      }, err => {
        this.submitting = false;
        if (err._body === 'Payment already made') {
          this.snackBar.open('Application payment has already been made.', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
        }
      });
    });
  }

}
