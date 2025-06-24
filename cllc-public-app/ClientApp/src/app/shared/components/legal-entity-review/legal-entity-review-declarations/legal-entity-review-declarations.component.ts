import { Component, Input, OnInit } from '@angular/core';
import { ControlContainer, FormGroup, FormGroupDirective } from '@angular/forms';

/**
 * The declarations section of a legal entity review application.
 *
 * @export
 * @class LegalEntityReviewDeclarationsComponent
 * @implements {OnInit}
 */
@Component({
  selector: 'app-legal-entity-review-declarations',
  templateUrl: './legal-entity-review-declarations.component.html',
  styleUrls: ['./legal-entity-review-declarations.component.scss'],
  viewProviders: [{ provide: ControlContainer, useExisting: FormGroupDirective }]
})
export class LegalEntityReviewDeclarationsComponent implements OnInit {
  @Input() hasLiquor: boolean = false;
  @Input() hasCannabis: boolean = false;
  /**
   * Set to `true` to force the display of validation messages.
   *
   * @type {boolean} Default is `false`.
   * @memberof LegalEntityReviewDeclarationsComponent
   */
  @Input() showValidationMessages: boolean = false;

  form: FormGroup;

  constructor(public controlContainer: ControlContainer) {}

  ngOnInit() {
    this.form = this.controlContainer.control as FormGroup;
  }
}
