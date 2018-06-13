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

  constructor(private userDataService: UserDataService, private accountDataService: AccountDataService, private frmbuilder: FormBuilder) {
    this.corporateDetailsForm = frmbuilder.group({
      bcIncorporationNumber: ['', Validators.required],
      businessNumber: ['', Validators.required],
      pstNumber: ['', Validators.required],
      mailName: ['', Validators.required],
      mailAddress: ['', Validators.required],
      mailCity: ['', Validators.required],
      mailCountry: ['', Validators.required],
      mailProvince: ['', Validators.required],
      mailPostalcode: ['', Validators.required]
    });//, { validator: this.dateLessThanToday('dateIssued') });
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

  ngOnInit() {
    this.userDataService.getCurrentUser().then(user =>{
      this.user = user;
    })
  }

}
