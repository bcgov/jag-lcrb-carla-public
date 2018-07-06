import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-financial-information',
  templateUrl: './financial-information.component.html',
  styleUrls: ['./financial-information.component.scss']
})
export class FinancialInformationComponent implements OnInit {
  @Input() accountId: string;
  @Input() businessType: string;
  operatingForMoreThanOneYear: string = '';

  constructor() { }

  ngOnInit() {
  }

}
