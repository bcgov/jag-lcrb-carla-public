import { Component, OnInit, Input } from '@angular/core';
import { UserDataService } from '../../../services/user-data.service';
import { User } from '../../../models/user.model';
import { ActivatedRoute } from '@angular/router';
import { DynamicsDataService } from '../../../services/dynamics-data.service';
import { Store } from '../../../../../node_modules/@ngrx/store';
import { AppState } from '../../../app-state/models/app-state';

@Component({
  selector: 'app-before-you-start',
  templateUrl: './before-you-start.component.html',
  styleUrls: ['./before-you-start.component.scss']
})
export class BeforeYouStartComponent implements OnInit {
  @Input() accountId: string;
  @Input() businessType: string;

  constructor(private store: Store<AppState>, private route: ActivatedRoute) { }

  ngOnInit() {
    this.store.select(state => state.currentAccountState)
      .filter(account => !!account)
      .subscribe(account => {
        this.businessType = account.currentAccount.businessType;
      });
  }

}
