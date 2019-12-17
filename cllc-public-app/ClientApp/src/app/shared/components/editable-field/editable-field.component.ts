import { Component, OnInit, HostListener, Input, ElementRef, Output, EventEmitter, ViewChild } from '@angular/core';

@Component({
  selector: 'app-editable-field',
  templateUrl: './editable-field.component.html',
  styleUrls: ['./editable-field.component.scss']
})
export class EditableFieldComponent {
  @Input() value: string;
  @Input() emptyText: string;
  @Input() isNumeric = false;
  @Output() submit: EventEmitter<any> = new EventEmitter();
  @ViewChild('inputField', {static: false}) inputField: ElementRef;
  el: ElementRef;
  isEditing = false;

  constructor(elementRef: ElementRef) {
    this.el = elementRef;
  }

  @HostListener('click', ['$event']) editClicked(event) {
    event.preventDefault();
    this.isEditing = true;
    setTimeout(() => this.inputField.nativeElement.focus(), 0);
  }

  public rejectIfNotDigitOrBackSpace(event) {
    if (!this.isNumeric) {
      return false;
    }

    const acceptedKeys = [
      'Backspace', 'Tab', 'End', 'Home', 'ArrowLeft', 'ArrowRight', 'Control',
      '1', '2', '3', '4', '5', '6', '7', '8', '9', '0'
    ];
    if (acceptedKeys.indexOf(event.key) === -1) {
        event.preventDefault();
    }
  }

  focusOut(event) {
    this.submit.emit(event.target.value);
    this.isEditing = false;
  }
}
