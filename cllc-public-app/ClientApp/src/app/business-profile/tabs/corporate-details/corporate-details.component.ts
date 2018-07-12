import { Component, OnInit, Input } from '@angular/core';
import { UserDataService } from '../../../services/user-data.service';
import { AccountDataService } from '../../../services/account-data.service';
import { User } from '../../../models/user.model';
import { DynamicsAccount } from '../../../models/dynamics-account.model';
import { FormBuilder, FormGroup, FormControl, Validators, NgForm } from '@angular/forms';
import { MatSnackBar } from '@angular/material';
import { Subscription, Subject, Observable } from 'rxjs';
import { DatePipe } from '@angular/common';
import { auditTime } from 'rxjs/operators';
import { DynamicsDataService } from '../../../services/dynamics-data.service';
import { ActivatedRoute } from '../../../../../node_modules/@angular/router';

@Component({
  selector: 'app-corporate-details',
  templateUrl: './corporate-details.component.html',
  styleUrls: ['./corporate-details.component.scss']
})
export class CorporateDetailsComponent implements OnInit {
  @Input() accountId: string;
  @Input() businessType: string;
  corporateDetailsForm: FormGroup;
  accountModel: DynamicsAccount;
  busy: Subscription;

  constructor(private userDataService: UserDataService,
    private accountDataService: AccountDataService,
    private dynamicsDataService: DynamicsDataService,
    private route: ActivatedRoute,
    private fb: FormBuilder, public snackBar: MatSnackBar) {
  }

  ngOnInit() {
    this.createForm();

    this.route.parent.params.subscribe(p => {
      this.accountId = p.accountId;
      this.dynamicsDataService.getRecord('account', this.accountId)
        .then((data) => {
          this.businessType = data.businessType;
        });
      this.getFormData();
    });

  }

  getFormData() {
    this.busy = this.accountDataService.getAccount(this.accountId).subscribe(
      res => {
        // let data = this.toFormModel(res.json());
        let data = res.json();
        // format date based on user locale
        let dp = new DatePipe(this.getLang());
        const dateFormat = 'y-MM-dd'; // YYYY-MM-DD
        let dtr = dp.transform(new Date(data.dateOfIncorporationInBC), dateFormat);
        data.dateOfIncorporationInBC = dtr;
        this.corporateDetailsForm.patchValue(data);
      },
      err => {
        console.log('Error occured');
      }
    );
  }

  getLang() {
    if (navigator.languages !== undefined) {
      return navigator.languages[0];
    } else {
      return navigator.language;
    }
  }

  createForm() {
    this.corporateDetailsForm = this.fb.group({
      bcIncorporationNumber: [''], // Validators.required
      dateOfIncorporationInBC: [''],
      businessNumber: [''],
      contactEmail: [''],
      contactPhone: [''],
      mailingAddressName: [''],
      mailingAddressStreet: [''],
      mailingAddressCity: [''],
      mailingAddressCountry: [''],
      mailingAddressProvince: [''],
      mailingAddresPostalCode: ['']
    });

    this.corporateDetailsForm.valueChanges
      .pipe(auditTime(10000)).subscribe(formData => {
        this.save();
      });

  }

  dateLessThanToday(field1) {
    return form => {
      const d1 = form.controls[field1].value;
      if (!d1) {
        return {};
      }
      const d1Date = new Date(d1.year, d1.month, d1.day);
      if (d1Date < new Date()) {
        return { dateLessThanToday: true };
      }
      return {};
    };
  }

  canDeactivate(): Observable<boolean> | boolean {
    return this.save();
  }

  save(): Subject<boolean> {
    // console.log('is corporateDetailsForm valid: ', this.corporateDetailsForm.valid, this.corporateDetailsForm.value);
    // if (!this.corporateDetailsForm.valid) {
    //   Object.keys(this.corporateDetailsForm.controls).forEach(field => {
    //     const control = this.corporateDetailsForm.get(field);
    //     control.markAsTouched({ onlySelf: true });
    //   });
    // }
    const saveResult = new Subject<boolean>();
    this.accountModel = this.toAccountModel(this.corporateDetailsForm.value);
    this.accountDataService.updateAccount(this.accountModel).subscribe(
      res => {
        // this.snackBar.open('Corporate Details have been saved', 'Success', { duration: 2500, extraClasses: ['red-snackbar'] });
        saveResult.next(true);
      },
      err => {
        this.snackBar.open('Error saving Corporate Details', 'Fail', { duration: 3500, extraClasses: ['red-snackbar'] });
        saveResult.next(false);
        console.log('Error occured');
      });
    return saveResult;
  }

  isFieldError(field: string) {
    const isError = !this.corporateDetailsForm.get(field).valid && this.corporateDetailsForm.get(field).touched;
    return isError;
  }

  getAccount(accountId: string) {
    this.accountDataService.getAccount(accountId).subscribe(
      res => {
        // console.log("accountVM: ", res.json());
        return res.json();
      },
      err => {
        console.log('Error occured');
      }
    );
  }

  toAccountModel(formData) {
    formData.id = this.accountId;
    // let date = formData.dateOfIncorporationInBC;
    // formData.dateOfIncorporationInBC = new Date(date.year, date.month-1, date.day);
    return formData;
  }

  toFormModel(dynamicsData) {
    // let date: Date = new Date(dynamicsData.dateOfIncorporationInBC);
    // dynamicsData.dateOfIncorporationInBC = {
    //  year: date.getFullYear(),
    //  month: date.getMonth()+1,
    //  day: date.getDate()
    // }
    return dynamicsData;
  }

}
