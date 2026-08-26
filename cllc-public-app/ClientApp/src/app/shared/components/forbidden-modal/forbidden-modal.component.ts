import { Component, OnInit } from '@angular/core';
import { faExclamationTriangle } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-forbidden-modal',
  templateUrl: './forbidden-modal.component.html',
  styleUrls: ['./forbidden-modal.component.scss']
})
export class ForbiddenModalComponent implements OnInit {
  faExclamationTriangle = faExclamationTriangle;

  constructor() {}

  ngOnInit(): void {}
}
