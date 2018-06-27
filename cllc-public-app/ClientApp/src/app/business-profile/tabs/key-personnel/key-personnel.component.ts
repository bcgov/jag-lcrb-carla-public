import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-key-personnel',
  templateUrl: './key-personnel.component.html',
  styleUrls: ['./key-personnel.component.scss']
})
export class KeyPersonnelComponent implements OnInit {
  @Input() accountId: string;
  constructor() { }

  ngOnInit() {
  }

}
