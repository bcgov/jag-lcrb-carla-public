import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { Account } from '@models/account.model';

/**
 * Loads the tied house data and renders the tied house connections component.
 *
 * TODO tiedhouse: load tied house data.
 *
 * @export
 * @class ConnectionToOtherLiquorLicencesComponent
 * @implements {OnInit}
 * @implements {OnDestroy}
 */
@Component({
  selector: 'app-connection-to-other-liquor-licences',
  templateUrl: './connection-to-other-liquor-licences.component.html',
  styleUrls: ['./connection-to-other-liquor-licences.component.scss']
})
export class ConnectionToOtherLiquorLicencesComponent implements OnInit, OnDestroy {
  @Input() account: Account;

  constructor() {}

  ngOnInit() {}

  ngOnDestroy() {}
}
