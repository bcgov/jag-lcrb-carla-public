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

  applicationId: string;
  storeInformationForm: FormGroup;
  busy: Subscription;
  savedFormData: any = {};

  constructor(private applicationDataService: AdoxioApplicationDataService,
    private fb: FormBuilder,
    private userDataService: UserDataService,
    public snackBar: MatSnackBar, private route: ActivatedRoute) {
    this.applicationId = this.route.parent.snapshot.params.applicationId;
  }

  ngOnInit() {
    this.createForm();
    // get application data and display in form
    this.busy = this.applicationDataService.getApplicationById(this.applicationId).subscribe(
      res => {
        const data = res.json();
        this.storeInformationForm.patchValue(data);
        this.savedFormData = this.storeInformationForm.value;
      },
      err => {
        console.log('Error occured');
      }
    );
  }

  /**
   * Create the entry form
   * */
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
    if (JSON.stringify(this.savedFormData) === JSON.stringify(this.storeInformationForm.value)) {
      return true;
    } else {
      return this.save(true);
    }
  }

  /**
   * Save form data
   * @param showProgress
   */
  save(showProgress: boolean = false): Subject<boolean> {
    const saveResult = new Subject<boolean>();
    const saveData = this.storeInformationForm.value;
    const subscription = this.applicationDataService.updateApplication(this.storeInformationForm.value).subscribe(
      res => {
        saveResult.next(true);
        this.savedFormData = saveData;
        if (showProgress === true) {
          this.snackBar.open('Store Information has been saved', 'Success', { duration: 2500, extraClasses: ['red-snackbar'] });
        }
      },
      err => {
        saveResult.next(false);
        this.snackBar.open('Error saving Store Information', 'Fail', { duration: 3500, extraClasses: ['red-snackbar'] });
        console.log('Error occured');
      });

    if (showProgress === true) {
      this.busy = subscription;
    }

    return saveResult;
  }

  isFieldError(field: string) {
    const isError = !this.storeInformationForm.get(field).valid && this.storeInformationForm.get(field).touched;
    return isError;
  }

}
