import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-shareholders-ucl-llc',
  templateUrl: './shareholders-ucl-llc.component.html',
  styleUrls: ['./shareholders-ucl-llc.component.css']
})
export class ShareholdersUclLlcComponent implements OnInit {
  @Input() accountId: string;
  constructor() { }

  ngOnInit() {
  }

}
