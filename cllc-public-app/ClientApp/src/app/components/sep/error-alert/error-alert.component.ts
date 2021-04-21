import { Component, Input, OnInit } from '@angular/core';
import { faExclamationCircle } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-error-alert',
  templateUrl: './error-alert.component.html',
  styleUrls: ['./error-alert.component.scss']
})
export class ErrorAlertComponent implements OnInit {
  @Input() type: 'error' | 'alert' = 'alert';
  faExclamationCircle = faExclamationCircle;

  constructor() { }

  ngOnInit(): void {
  }

}
