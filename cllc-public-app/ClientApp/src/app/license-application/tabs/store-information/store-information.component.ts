import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-store-information',
  templateUrl: './store-information.component.html',
  styleUrls: ['./store-information.component.scss']
})
export class StoreInformationComponent implements OnInit {

  @Input('accountId') accountId: string;
  @Input('applicationId') applicationId: string;

  constructor() { }

  ngOnInit() {
  }

}
