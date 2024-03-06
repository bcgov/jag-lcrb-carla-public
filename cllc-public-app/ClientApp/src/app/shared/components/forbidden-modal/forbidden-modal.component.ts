import { Component, OnInit } from '@angular/core';
import { faAddressCard, faChevronRight, faEnvelope, faExclamationTriangle, faPhone, faTrash } from
  "@fortawesome/free-solid-svg-icons";

@Component({
  selector: 'app-forbidden-modal',
  templateUrl: './forbidden-modal.component.html',
  styleUrls: ['./forbidden-modal.component.scss']
})

// LCSD - 6243: 2024-02-28 waynezen: prevent deep - linking
export class ForbiddenModalComponent implements OnInit {
  faExclamationTriangle = faExclamationTriangle;

  constructor() {

  }

  ngOnInit(): void {


  }

}
