import { Component, OnInit, Input } from '@angular/core';
import { Account } from '@models/account.model';

@Component({
  selector: 'app-associate-content',
  templateUrl: './associate-content.component.html',
  styleUrls: ['./associate-content.component.scss']
})
export class AssociateContentComponent implements OnInit {
  @Input() account: Account;
  @Input() isIndigenousNation: boolean;
  @Input() showBothContents = true;

  constructor() { }

  ngOnInit() {
  }

}
