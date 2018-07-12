import { Component, OnInit, Input } from '@angular/core';
import { AdoxioApplicationDataService } from '../../../services/adoxio-application-data.service';
import { FormBuilder, FormGroup, FormControl, Validators, NgForm } from '@angular/forms';
import { MatSnackBar } from '@angular/material';
import { Subscription } from 'rxjs/Subscription';
import { ActivatedRoute } from '@angular/router';
import { auditTime } from 'rxjs/operators';
import { UserDataService } from '../../../services/user-data.service';
import { Observable } from '../../../../../node_modules/rxjs/Observable';
import { Subject } from '../../../../../node_modules/rxjs/Subject';

@Component({
  selector: 'app-store-information',
  templateUrl: './store-information.component.html',
  styleUrls: ['./store-information.component.scss']
})
export class StoreInformationComponent implements OnInit {

  @Input() accountId: string;
  @Input() applicationId: string;
  storeInformationForm: FormGroup;
  busy: Subscription;

  constructor(private applicationDataService: AdoxioApplicationDataService,
    private fb: FormBuilder,
    private userDataService: UserDataService,
    public snackBar: MatSnackBar, private route: ActivatedRoute) {
    this.applicationId = this.route.parent.snapshot.params.applicationId;
  }

  ngOnInit() {
    this.userDataService.getCurrentUser()
      .then((data) => {
        this.accountId = data.accountid;
      });

    this.createForm();
    // get application data, display form
    this.busy = this.applicationDataService.getApplicationById(this.applicationId).subscribe(
      res => {
        const data = res.json();
        this.storeInformationForm.patchValue(data);
      },
      err => {
        console.log('Error occured');
      }
    );
  }

  createForm() {
    this.storeInformationForm = this.fb.group({
      id: [''],
      establishmentName: [''], // Validators.required
    });
    this.storeInformationForm.valueChanges
      .pipe(auditTime(10000)).subscribe(data => {
        this.save();
      });
  }

  canDeactivate(): Observable<boolean> | boolean {
    return this.save();
  }

  save(): Subject<boolean> {
    // console.log('storeInformationForm valid, value: ', this.storeInformationForm.valid, this.storeInformationForm.value);
    const saveResult = new Subject<boolean>();
    this.applicationDataService.updateApplication(this.storeInformationForm.value).subscribe(
      res => {
        saveResult.next(true);
        // console.log("Application updated:", res.json());
        // this.snackBar.open('Store Information has been saved', 'Success', { duration: 2500, extraClasses: ['red-snackbar'] });
      },
      err => {
        saveResult.next(false);
        this.snackBar.open('Error saving Store Information', 'Fail', { duration: 3500, extraClasses: ['red-snackbar'] });
        console.log('Error occured');
      });
    // if (!this.storeInformationForm.valid) {
    //   Object.keys(this.storeInformationForm.controls).forEach(field => {
    //     const control = this.storeInformationForm.get(field);
    //     control.markAsTouched({ onlySelf: true });
    //   });
    // }
    return saveResult;
  }

  isFieldError(field: string) {
    const isError = !this.storeInformationForm.get(field).valid && this.storeInformationForm.get(field).touched;
    return isError;
  }

}
