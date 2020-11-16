import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { AppState } from '@app/app-state/models/app-state';
import { Account } from '@models/account.model';
import { ApplicationLicenseSummary } from '@models/application-license-summary.model';
import { Application } from '@models/application.model';
import { Store } from '@ngrx/store';
import { ApplicationDataService } from '@services/application-data.service';
import { FormBase } from '@shared/form-base';
import { filter } from 'rxjs/operators';

interface PageData {
  licences: {
    cannabis: ApplicationLicenseSummary[],
    liquor: ApplicationLicenseSummary[],
  },
  application: Application

}

@Component({
  selector: 'app-permanent-changes-to-a-licensee',
  templateUrl: './permanent-changes-to-a-licensee.component.html',
  styleUrls: ['./permanent-changes-to-a-licensee.component.scss']
})
export class PermanentChangesToALicenseeComponent extends FormBase implements OnInit {
  value: string; // Place holder propertry
  applicationId: string; // Place holder prop
  account: Account;
  businessType: string;

  get hasLiquor(): boolean {
    return this.licences.liquor.length > 0;
  }

  get hasCannabis(): boolean {
    return this.licences.cannabis.length > 0;
  }

  get licences() {
    return this.form.get('licences').value;
  }
  
  changeList = [];
  form: FormGroup;

  constructor(private applicationDataService: ApplicationDataService,
    private fb: FormBuilder,
    private store: Store<AppState>) {
    super();
    this.store.select(state => state.currentAccountState.currentAccount)
      .pipe(filter(account => !!account))
      .subscribe(account => {
        this.account = account;
        this.changeList = masterChangeList.filter(item => !!item.availableTo.find(bt => bt === account.businessType));
      });
  }

  ngOnInit(): void {
    const defaultLicences = {
      liquor: [],
      cannabis: []
    };

    this.form = this.fb.group({
      application: this.fb.group({}),
      licences: [{ value: defaultLicences, disabled: true }]
    });

    this.applicationDataService.getPermanentChangesToLicenseeData()
      .subscribe(({ application, licences }) => {
        const data: PageData = { licences: {} } as PageData;
        data.licences.liquor = licences.filter(item => item.licenceTypeCategory === 'Liquor');
        data.licences.cannabis = licences.filter(item => item.licenceTypeCategory === 'Cannabis');
        data.application = application;
        this.form.patchValue(data);
      });

  }
}

const masterChangeList = [
  {
    name: 'Internal Transfer of Shares',
    availableTo: ['PrivateCorporation', 'LimitedLiabilityPartnership'],
    CannabisFee: 110,
    LiquorFee: 110,
    RequiresPHS: false,
    RequiresCAS: false,
    helpTextHeader: 'Use this option to report:',
    helpText: ['When shares or partnership units are redistributed between existing sharesholders/unit holders',
      'Removal of shareholders/unit holders',
      'Amalgamations that do not add new shareholders or legal entities to the licensee coroporation',
      'Third party operators should also complete this section when an internal share transfer or an amalgamation occurs'
    ],
    helpTextLink: 'https://www2.gov.bc.ca/gov/content/employment-business/business/liquor-regulation-licensing/liquor-licences-permits/changing-a-liquor-licence',
  },
  {
    name: 'External Transfer of Shares',
    availableTo: ['PrivateCorporation', 'LimitedLiabilityPartnership'],
    CannabisFee: 330,
    LiquorFee: 330,
    RequiresPHS: false,
    RequiresCAS: false,
    helpTextHeader: 'Use this option to report:',
    helpText: ['when new shareholders have been added (companies or individuals) to the licensee corporation or holding companies as a result of a transfer of existing shares or the issuance of new shares',
      'Third party operators should also complete this section when an external transfer occurs'
    ],
    helpTextLink: 'https://www2.gov.bc.ca/gov/content/employment-business/business/liquor-regulation-licensing/liquor-licences-permits/changing-a-liquor-licence',
  },
  {
    name: 'Change of Directors or Officers',
    availableTo: ['PrivateCorporation', 'PublicCorporation', 'Society'],
    CannabisFee: 550,
    LiquorFee: 220,
    RequiresPHS: false,
    RequiresCAS: false,
    helpTextHeader: 'Use this option to report:',
    helpText: ['When there are changes in directors or officers of a public corporation or society within the licensee legal entity'
    ],
    helpTextLink: 'https://www2.gov.bc.ca/gov/content/employment-business/business/liquor-regulation-licensing/liquor-licences-permits/changing-a-liquor-licence',
  },
  {
    name: 'Name Change, Licensee -- Corporation',
    availableTo: ['PrivateCorporation', 'PublicCorporation'],
    CannabisFee: 500,
    LiquorFee: 500,
    RequiresPHS: false,
    RequiresCAS: false,
    helpTextHeader: 'Use this option to report:',
    helpText: ['When a corporation with an interest in a licence has legally changed its name, but existing corporate shareholders, directors and officers, and certificate number on the certificate of incorporation have not changed'
    ],
    helpTextLink: 'https://www2.gov.bc.ca/gov/content/employment-business/business/liquor-regulation-licensing/liquor-licences-permits/changing-a-liquor-licence',
  },
  {
    name: 'Name Change, Licensee -- Partnership',
    availableTo: ['GeneralPartnership', 'LimitedLiabilityPartnership'],
    CannabisFee: 220,
    LiquorFee: 220,
    RequiresPHS: false,
    RequiresCAS: false,
    helpTextHeader: 'Use this option to report:',
    helpText: ['When the legal name of a partnership is changed but no new partners are added and no existing partners'
    ],
    helpTextLink: 'https://www2.gov.bc.ca/gov/content/employment-business/business/liquor-regulation-licensing/liquor-licences-permits/changing-a-liquor-licence',

  },
  {
    name: 'Name Change, Licensee -- Society',
    availableTo: ['Society'],
    CannabisFee: 220,
    LiquorFee: 220,
    RequiresPHS: false,
    RequiresCAS: false,
    helpTextHeader: 'Use this option to report:',
    helpText: ['When the legal name of a society is changed, but the society structure, membership and certification number on the certificate of incorporation does not change'
    ],
    helpTextLink: 'https://www2.gov.bc.ca/gov/content/employment-business/business/liquor-regulation-licensing/liquor-licences-permits/changing-a-liquor-licence',
  },
  {
    name: 'Name Change, Person',
    availableTo: ['PrivateCorporation', 'PublicCorporation', 'GeneralPartnership',
      'LimitedLiabilityPartnership', 'IndigenousNation', 'LocalGovernment', 'Society'],
    CannabisFee: 220,
    LiquorFee: 220,
    RequiresPHS: false,
    RequiresCAS: false,
    helpTextHeader: 'Use this option to report:',
    helpText: ['when a person holding an interest in a licence has legally changed their name'
    ],
    helpTextLink: 'https://www2.gov.bc.ca/gov/content/employment-business/business/liquor-regulation-licensing/liquor-licences-permits/changing-a-liquor-licence',
  },
  {
    name: 'Addition of Receiver or Executor',
    availableTo: ['PrivateCorporation', 'PublicCorporation', 'GeneralPartnership',
      'LimitedLiabilityPartnership', 'Society'],
    CannabisFee: 220,
    LiquorFee: 220,
    RequiresPHS: false,
    RequiresCAS: false,
    helpTextHeader: 'Use this option to report:',
    helpText: ['upon the death, bankruptcy or receivership of a licensee'
    ],
    helpTextLink: 'https://www2.gov.bc.ca/gov/content/employment-business/business/liquor-regulation-licensing/liquor-licences-permits/changing-a-liquor-licence',
  }
];
