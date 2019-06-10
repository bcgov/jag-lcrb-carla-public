
import {filter, takeWhile} from 'rxjs/operators';
import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { UserDataService } from '../../../services/user-data.service';
import { User } from '../../../models/user.model';
import { ActivatedRoute } from '@angular/router';
import { DynamicsDataService } from '../../../services/dynamics-data.service';
import { Store } from '@ngrx/store';
import { AppState } from '../../../app-state/models/app-state';
import { Subscription } from 'rxjs';
import { FormBase } from '@shared/form-base';

@Component({
  selector: 'app-before-you-start',
  templateUrl: './before-you-start.component.html',
  styleUrls: ['./before-you-start.component.scss']
})
export class BeforeYouStartComponent extends FormBase implements OnInit, OnDestroy {
  @Input() accountId: string;
  @Input() businessType: string;

  constructor(private store: Store<AppState>, private route: ActivatedRoute) {
    super();
   }

  ngOnInit() {
    this.store.select(state => state.currentAccountState)
    .pipe(takeWhile(() => this.componentActive))
    .pipe(filter(account => !!account))
      .subscribe(account => {
        this.businessType = account.currentAccount.businessType;
      });
  }
}
