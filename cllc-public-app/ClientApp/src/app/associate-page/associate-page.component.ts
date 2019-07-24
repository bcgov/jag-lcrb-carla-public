import { Component, OnInit } from '@angular/core';
import { takeWhile, filter } from 'rxjs/operators';
import { FormBase } from '@shared/form-base';
import { UserDataService } from '@services/user-data.service';
import { Store } from '@ngrx/store';
import { AppState } from '@app/app-state/models/app-state';
import { AccountDataService } from '@services/account-data.service';
import { ContactDataService } from '@services/contact-data.service';
import { DynamicsDataService } from '@services/dynamics-data.service';
import { FormBuilder, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { TiedHouseConnectionsDataService } from '@services/tied-house-connections-data.service';
import { Account } from '@models/account.model';

@Component({
  selector: 'app-associate-page',
  templateUrl: './associate-page.component.html',
  styleUrls: ['./associate-page.component.scss']
})
export class AssociatePageComponent extends FormBase implements OnInit {

  account: Account;
  legalEntityId: string;
  lockAssociates = true;


  constructor(private store: Store<AppState>, private fb: FormBuilder) {
    super();
  }

  ngOnInit() {
    this.form = this.fb.group({
      noOrgStructureChange: [''],
      noShareholderChange: [''],
      noKeyPersonnelChange: [''],
      noLeadershipChange: ['']
    });
    this.subscribeForData();
  }


  subscribeForData() {
    this.store.select(state => state.currentAccountState.currentAccount)
      .pipe(takeWhile(() => this.componentActive))
      .pipe(filter(s => !!s))
      .subscribe(account => {
        this.account = account;
        this.legalEntityId = this.account.legalEntity.id;
      });
  }

}
