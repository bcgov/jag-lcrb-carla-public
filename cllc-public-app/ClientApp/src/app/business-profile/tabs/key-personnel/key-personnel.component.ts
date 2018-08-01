import { Component, OnInit, Input } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DynamicsDataService } from '../../../services/dynamics-data.service';
import { Store } from '@ngrx/store';
import { AppState } from '../../../app-state/models/app-state';

@Component({
  selector: 'app-key-personnel',
  templateUrl: './key-personnel.component.html',
  styleUrls: ['./key-personnel.component.scss']
})
export class KeyPersonnelComponent implements OnInit {
  @Input() accountId: string;
  @Input() businessType: string;

  constructor(private route: ActivatedRoute,
    private store: Store<AppState>,
    private dynamicsDataService: DynamicsDataService) { }

  ngOnInit() {
    this.store.select(state => state.currentAccountState)
    .filter(state => !!state)
    .subscribe(state => {
      this.accountId = state.currentAccount.id;
      this.businessType = state.currentAccount.businessType;
    });
  }

}
