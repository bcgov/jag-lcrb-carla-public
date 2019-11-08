
import {filter, takeWhile} from 'rxjs/operators';
import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Store } from '@ngrx/store';
import { FormBase } from '@shared/form-base';
import { AppState } from '@app/app-state/models/app-state';

@Component({
  selector: 'app-before-you-start',
  templateUrl: './before-you-start.component.html',
  styleUrls: ['./before-you-start.component.scss']
})
export class BeforeYouStartComponent extends FormBase implements OnInit, OnDestroy {
  @Input() accountId: string;
  @Input() businessType: string;

  constructor(private store: Store<AppState>) {
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
