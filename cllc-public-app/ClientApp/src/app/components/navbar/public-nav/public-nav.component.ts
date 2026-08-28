import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-public-nav',
  templateUrl: './public-nav.component.html',
  styleUrls: ['./public-nav.component.scss']
})
export class PublicNavComponent implements OnInit {
  @Input() showMapLink = false;

  constructor() {}

  ngOnInit() {}
}
