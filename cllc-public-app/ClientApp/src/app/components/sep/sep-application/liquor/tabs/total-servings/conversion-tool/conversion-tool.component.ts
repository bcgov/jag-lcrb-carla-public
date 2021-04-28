import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-conversion-tool',
  templateUrl: './conversion-tool.component.html',
  styleUrls: ['./conversion-tool.component.scss']
})
export class ConversionToolComponent implements OnInit {
  @Input() title: string;

  constructor() { }

  ngOnInit(): void {
  }

}
