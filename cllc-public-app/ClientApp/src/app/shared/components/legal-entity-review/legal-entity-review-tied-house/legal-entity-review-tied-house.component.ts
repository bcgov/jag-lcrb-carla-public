import { Component, EventEmitter, Input, Output } from '@angular/core';
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
  /**
   * Emits an event when the user indicates whether they have tied house changes to declare.
   *
   * @type {EventEmitter<boolean>}
   */
  @Output() hasTiedHouseChangesToDeclareEvent: EventEmitter<boolean> = new EventEmitter<boolean>();

  form: FormGroup;

  /**
   * Whether or not the user has tied house changes to declare.
   * Default to `null` so the user has to select either `Yes` or `No`.
   */
  hasTiedHouseChangesToDeclare = null;
}
