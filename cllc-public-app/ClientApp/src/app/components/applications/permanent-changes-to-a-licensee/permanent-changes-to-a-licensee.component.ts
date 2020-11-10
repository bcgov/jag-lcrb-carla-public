import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { AppState } from '@app/app-state/models/app-state';
import { Account } from '@models/account.model';
import { Application } from '@models/application.model';
import { Store } from '@ngrx/store';
import { ApplicationDataService } from '@services/application-data.service';
import { FormBase } from '@shared/form-base';
import { filter } from 'rxjs/operators';

const masterChangeList = [
  {
    name: 'Internal Transfer of Shared',
    availbleTo: ['PrivateCorporation', 'LimitedLiabilityPartnership']
  },
  {
    name: 'External Transfer of Shared',
    availbleTo: ['PrivateCorporation', 'LimitedLiabilityPartnership']
  },
  {
    name: 'Change of Directors or Officers',
    availbleTo: ['PrivateCorporation', 'PublicCorporation', 'Society']
  },
  {
    name: 'Name Change, licensee -- corporation',
    availbleTo: ['PrivateCorporation', 'PublicCorporation']
  },
  {
    name: 'Name Change, licensee -- partnership',
    availbleTo: ['GeneralPartnership', 'LimitedLiabilityPartnership', 'Society']
  },
  {
    name: 'Name Change, licensee -- society',
    availbleTo: ['Society']
  },
  {
    name: 'Name Change, person',
    availbleTo: ['PrivateCorporation', 'PublicCorporation', 'GeneralPartnership',
      'LimitedLiabilityPartnership', 'IndigenousNation', 'LocalGovernment'
    ]
  },
  {
    name: 'Addition of receiver or executor',
    availbleTo: ['PrivateCorporation', 'PublicCorporation', 'GeneralPartnership',
      'LimitedLiabilityPartnership', 'Society']
  }
];

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-permanent-changes-to-a-licensee',
  templateUrl: './permanent-changes-to-a-licensee.component.html',
  styleUrls: ['./permanent-changes-to-a-licensee.component.scss']
})
export class PermanentChangesToALicenseeComponent extends FormBase implements OnInit {
  data: any = {};
  value: string; // Place holder propertry
  applicationId: string; // Place holder prop

  changeList = [];

  account: Account;

  constructor(private applicationDataService: ApplicationDataService,
    private fb: FormBuilder,
    private store: Store<AppState>) {
    super();
    this.store.select(state => state.currentAccountState.currentAccount)
    .pipe(filter(account  => !!account))
      .subscribe(account => {
        this.account = account;
        this.changeList = masterChangeList.filter(item => !!item.availbleTo.find(bt => bt === account.businessType));
      });
  }

  ngOnInit(): void {
    this.applicationDataService.getPermanentChangesToLicenseeData()
      .subscribe(data => this.data = data);

  }

}
