import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Form, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SepApplication } from '@models/sep-application.model';

@Component({
  selector: 'app-total-servings',
  templateUrl: './total-servings.component.html',
  styleUrls: ['./total-servings.component.scss']
})
export class TotalServingsComponent implements OnInit {
  _application: SepApplication
  @Input()
  set application(value: SepApplication) {
    this._application = value;
    this.totalServings = value.totalServings
  };
  get application() {
    return this._application;
  }
  @Output() saved: EventEmitter<{ totalServings: number }> = new EventEmitter<{ totalServings: number }>();
  minServings = 1;
  maxServings = 114;
  form: FormGroup;
  @Input() totalServings: number;

  constructor(private fb: FormBuilder) { }

  ngOnInit(): void {

  }

  formatLabel(value: number) {
    return value;
  }

  next() {
    this.saved.next({ totalServings: this.totalServings });
  }

}
