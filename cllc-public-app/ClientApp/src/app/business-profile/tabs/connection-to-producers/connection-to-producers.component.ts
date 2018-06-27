import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-connection-to-producers',
  templateUrl: './connection-to-producers.component.html',
  styleUrls: ['./connection-to-producers.component.css']
})
export class ConnectionToProducersComponent implements OnInit {
  @Input() accountId: string;
  @Input() businessType: string;
  constructor() { }

  ngOnInit() {
  }

}
