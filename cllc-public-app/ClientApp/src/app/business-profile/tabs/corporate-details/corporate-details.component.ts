import { Component, OnInit, Input } from '@angular/core';
import { UserDataService } from '../../../services/user-data.service';
import { AccountDataService } from '../../../services/account-data.service';
import { User } from '../../../models/user.model';
import { DynamicsAccount } from '../../../models/dynamics-account.model';
import { FormBuilder, FormGroup, FormControl, Validators, NgForm } from '@angular/forms';
import { MatSnackBar } from '@angular/material';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-corporate-details',
  templateUrl: './corporate-details.component.html',
  styleUrls: ['./corporate-details.component.scss']
})
export class CorporateDetailsComponent implements OnInit {
  @Input() accountId: string;
  corporateDetailsForm: FormGroup;
  accountModel: DynamicsAccount;
  busy: Subscription;

  constructor(private userDataService: UserDataService, private accountDataService: AccountDataService,
    private fb: FormBuilder, public snackBar: MatSnackBar) {
  }

  ngOnInit() {
    this.createForm();
    // get account data and then display form
    this.busy = this.accountDataService.getAccount(this.accountId).subscribe(
      res => {
        let data = this.toFormModel(res.json());
        data.dateOfIncorporationInBC = new Date(data.dateOfIncorporationInBC);
        this.corporateDetailsForm.patchValue(data);
      },
      err => {
        console.log("Error occured");
      }
    );

  }

  createForm() {
    this.corporateDetailsForm = this.fb.group({
      bcIncorporationNumber: [''],//Validators.required
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
    }//, { validator: this.dateLessThanToday('dateOfIncorporationInBC') }
    );
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
    }
  }

  save() {
    //console.log('is corporateDetailsForm valid: ', this.corporateDetailsForm.valid, this.corporateDetailsForm.value);
    if (this.corporateDetailsForm.valid) {

      //console.log("corporateDetailsForm value: ", this.corporateDetailsForm.value);
      this.accountModel = this.toAccountModel(this.corporateDetailsForm.value);
      //console.log("this.accountModel", this.accountModel);
      this.busy =  this.accountDataService.updateAccount(this.accountModel).subscribe(
        res => {
          //console.log("Account updated:", res.json());
          this.snackBar.open('Corporate Details have been saved', "Success", { duration: 2500, extraClasses: ['red-snackbar'] });
      },
        err => {
          this.snackBar.open('Error saving Corporate Details', "Fail", { duration: 3500, extraClasses: ['red-snackbar'] });
          console.log("Error occured");
        });
    } else {
      Object.keys(this.corporateDetailsForm.controls).forEach(field => {
      const control = this.corporateDetailsForm.get(field);
      control.markAsTouched({ onlySelf: true });
      });
    }
  }

  isFieldError(field: string) {
    const isError = !this.corporateDetailsForm.get(field).valid && this.corporateDetailsForm.get(field).touched;
    return isError;
  }

  getAccount(accountId: string) {
    this.accountDataService.getAccount(accountId).subscribe(
      res => {
        //console.log("accountVM: ", res.json());
        return res.json();
      },
      err => {
        console.log("Error occured");
      }
    );
  }

  toAccountModel(formData) {
    formData.id = this.accountId;
    //let date = formData.dateOfIncorporationInBC;
    //formData.dateOfIncorporationInBC = new Date(date.year, date.month-1, date.day);

    return formData;
  }

  toFormModel(dynamicsData) {
    //let date: Date = new Date(dynamicsData.dateOfIncorporationInBC);
    //dynamicsData.dateOfIncorporationInBC = {
    //  year: date.getFullYear(),
    //  month: date.getMonth()+1,
    //  day: date.getDate()
    //}
    return dynamicsData;
  }

}
