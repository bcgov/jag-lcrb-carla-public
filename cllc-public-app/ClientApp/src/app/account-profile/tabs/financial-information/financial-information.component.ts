
import {filter} from 'rxjs/operators';
import { Component, OnInit, Input } from '@angular/core';
import { DynamicsDataService } from '../../../services/dynamics-data.service';
import { ActivatedRoute } from '@angular/router';
import { Store } from '@ngrx/store';
import { AppState } from '../../../app-state/models/app-state';

@Component({
  selector: 'app-financial-information',
  templateUrl: './financial-information.component.html',
  styleUrls: ['./financial-information.component.scss']
})
export class FinancialInformationComponent implements OnInit {
  @Input() accountId: string;
  @Input() businessType: string;
  operatingForMoreThanOneYear: string = '';

  constructor(private route: ActivatedRoute,
    private store: Store<AppState>,
    private dynamicsDataService: DynamicsDataService) { }

  ngOnInit() {
    this.store.select(state => state.currentAccountState).pipe(
    filter(state => !!state))
    .subscribe(state => {
      this.accountId = state.currentAccount.id;
      this.businessType = state.currentAccount.businessType;
    });
  }

}
