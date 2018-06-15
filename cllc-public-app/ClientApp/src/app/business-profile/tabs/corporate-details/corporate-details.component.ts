import { Component, OnInit } from '@angular/core';
import { UserDataService } from '../../../services/user-data.service';
import { AccountDataService } from '../../../services/account-data.service';
import { User } from '../../../models/user.model';
import { DynamicsAccount } from '../../../models/dynamics-account.model';
import { FormBuilder, FormGroup, FormControl, Validators, NgForm } from '@angular/forms';

@Component({
  selector: 'app-corporate-details',
  templateUrl: './corporate-details.component.html',
  styleUrls: ['./corporate-details.component.scss']
})
export class CorporateDetailsComponent implements OnInit {
  user: User;
  corporateDetailsForm: FormGroup;

  constructor(private userDataService: UserDataService, private accountDataService: AccountDataService,
              private fb: FormBuilder) {
  }

  ngOnInit() {
    this.createForm();

    this.userDataService.getCurrentUser().then(user => {
      this.user = user;
      this.accountDataService.getAccount(user.accountid).subscribe(
        res => {
          this.corporateDetailsForm.patchValue(res.json());
        },
        err => {
          console.log("Error occured");
        }
      );
    });
  }

  createForm() {
    this.corporateDetailsForm = this.fb.group({
      bcIncorporationNumber: ['', Validators.required],
      dateOfIncorporationInBC: [''],
      businessNumber: ['', Validators.required],
      pstNumber: ['', Validators.required],
      contactEmail: ['', Validators.required],
      contactPhone: ['', Validators.required],
      //isCorporationOutsideBC: ['', Validators.required],
      mailingAddressName: ['', Validators.required],
      mailingAddressStreet: ['', Validators.required],
      mailingAddressCity: ['', Validators.required],
      mailingAddressCountry: ['', Validators.required],
      mailingAddressProvince: ['', Validators.required],
      mailingAddresPostalCode: ['', Validators.required]
    }, { validator: this.dateLessThanToday('dateOfIncorporationInBC') });
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
    console.log('is corporateDetailsForm valid: ', this.corporateDetailsForm.valid, this.corporateDetailsForm.value);
    if (this.corporateDetailsForm.valid) {
      console.log("corporateDetailsForm value: ", this.corporateDetailsForm.value);
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
        console.log("accountVM: ", res.json());
        return res.json();
      },
      err => {
        //console.log("Error occured");
      }
    );
  }

}
