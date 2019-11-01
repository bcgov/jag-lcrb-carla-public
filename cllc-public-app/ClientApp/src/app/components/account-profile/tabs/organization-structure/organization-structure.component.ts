
import {filter} from 'rxjs/operators';
import { Component, OnInit, Input } from '@angular/core';
import { Store } from '@ngrx/store';
import { AppState } from '@app/app-state/models/app-state';

@Component({
  selector: 'app-organization-structure',
  templateUrl: './organization-structure.component.html',
  styleUrls: ['./organization-structure.component.scss']
})
export class OrganizationStructureComponent implements OnInit {
  @Input() accountId: string;
  @Input() businessType: string;

  constructor(private store: Store<AppState>) { }

  ngOnInit() {
    this.store.select(state => state.currentAccountState.currentAccount).pipe(
      filter(account => !!account))
      .subscribe(account => {
        this.accountId = account.id;
        this.businessType = account.businessType;
      });
  }

}
