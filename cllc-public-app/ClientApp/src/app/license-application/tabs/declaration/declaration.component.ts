import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-declaration',
  templateUrl: './declaration.component.html',
  styleUrls: ['./declaration.component.scss']
})
export class DeclarationComponent implements OnInit {

  @Input('accountId') accountId: string;
  @Input('applicationId') applicationId: string;

  constructor() { }

  ngOnInit() {
  }

}
