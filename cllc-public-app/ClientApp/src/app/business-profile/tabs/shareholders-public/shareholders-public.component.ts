import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-shareholders-public',
  templateUrl: './shareholders-public.component.html',
  styleUrls: ['./shareholders-public.component.scss']
})
export class ShareholdersPublicComponent implements OnInit {
  @Input() accountId: string;
  constructor() { }

  ngOnInit() {
  }

}
