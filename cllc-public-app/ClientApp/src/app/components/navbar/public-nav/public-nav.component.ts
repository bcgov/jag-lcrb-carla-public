import { Component, Input, OnInit, ViewEncapsulation } from '@angular/core';

@Component({
  selector: 'app-public-nav',
  templateUrl: './public-nav.component.html',
  encapsulation: ViewEncapsulation.None
})
export class PublicNavComponent implements OnInit {
  @Input() showMapLink = false;

  constructor() { }

  ngOnInit() {
  }
}
