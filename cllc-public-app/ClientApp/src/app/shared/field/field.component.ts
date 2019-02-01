import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-field',
  templateUrl: './field.component.html',
  styleUrls: ['./field.component.scss']
})
export class FieldComponent implements OnInit {
  @Input() required = false;
  @Input() showChevrons = true;
  @Input() valid = true;
  @Input() label: string;
  @Input() leadingText: string;
  @Input() afterLabelText: string;
  @Input() errorMessage: string;

  constructor() { }

  ngOnInit() {
  }

}
