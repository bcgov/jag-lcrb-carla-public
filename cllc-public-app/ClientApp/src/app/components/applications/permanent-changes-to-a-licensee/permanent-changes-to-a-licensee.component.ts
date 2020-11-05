import { Component, OnInit } from '@angular/core';
import { Account } from '@models/account.model';

@Component({
  selector: 'app-permanent-changes-to-a-licensee',
  templateUrl: './permanent-changes-to-a-licensee.component.html',
  styleUrls: ['./permanent-changes-to-a-licensee.component.scss']
})
export class PermanentChangesToALicenseeComponent implements OnInit {

  changeList = [
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

  constructor() { }

  ngOnInit(): void {
  }

}
