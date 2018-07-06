import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-site-map',
  templateUrl: './site-map.component.html',
  styleUrls: ['./site-map.component.scss']
})
export class SiteMapComponent implements OnInit {

  @Input('accountId') accountId: string;
  @Input('applicationId') applicationId: string;

  constructor() { }

  ngOnInit() {
  }

}
