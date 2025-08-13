import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormGroup } from '@angular/forms';

/**
 * The tied house connections section of a legal entity review application.
 *
 * @export
 * @class LegalEntityReviewComponent
 */
@Component({
  selector: 'app-legal-entity-review-tied-house',
  templateUrl: './legal-entity-review-tied-house.component.html',
  styleUrls: ['./legal-entity-review-tied-house.component.scss']
})
export class LegalEntityReviewTiedHouseComponent {
  @Input() disabled = false;
  @Output() showTiedHouseConnections: EventEmitter<boolean> = new EventEmitter<boolean>();

  form: FormGroup;

  hasTiedHouseChangesToDeclare = false;
}
