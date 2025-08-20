import { Component, Input, OnInit } from '@angular/core';
import { Account } from '@models/account.model';

@Component({
  selector: 'app-legal-entity-type-update-calloutbox',
  templateUrl: './legal-entity-type-update-calloutbox.component.html',
  styleUrls: ['./legal-entity-type-update-calloutbox.component.scss']
})
export class LegalEntityTypeUpdateCalloutboxComponent implements OnInit {
  @Input()
  account: Account;
  @Input()
  canCreatePCLApplication: boolean;

  constructor() {}

  ngOnInit(): void {}
}
