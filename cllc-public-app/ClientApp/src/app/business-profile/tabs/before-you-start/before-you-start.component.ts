
import {filter} from 'rxjs/operators';
import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { UserDataService } from '../../../services/user-data.service';
import { User } from '../../../models/user.model';
import { ActivatedRoute } from '@angular/router';
import { DynamicsDataService } from '../../../services/dynamics-data.service';
import { Store } from '@ngrx/store';
import { AppState } from '../../../app-state/models/app-state';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-before-you-start',
  templateUrl: './before-you-start.component.html',
  styleUrls: ['./before-you-start.component.scss']
})
export class BeforeYouStartComponent implements OnInit, OnDestroy {
  @Input() accountId: string;
  @Input() businessType: string;
  subscriptions: Subscription[] = [];

  constructor(private store: Store<AppState>, private route: ActivatedRoute) { }

  ngOnInit() {
    const sub = this.store.select(state => state.currentAccountState).pipe(
      filter(account => !!account))
      .subscribe(account => {
        this.businessType = account.currentAccount.businessType;
      });
      this.subscriptions.push(sub);

  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }
}
