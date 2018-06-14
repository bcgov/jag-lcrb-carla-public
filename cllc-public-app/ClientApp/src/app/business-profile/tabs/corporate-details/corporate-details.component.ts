import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-corporate-details',
  templateUrl: './corporate-details.component.html',
  styleUrls: ['./corporate-details.component.scss']
})
export class CorporateDetailsComponent implements OnInit {
  @Input() accountId: string;

  constructor() { }

  ngOnInit() {

  }

}
