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
  /**
   * Set to `true` if the application has liquor-related declarations.
   *
   * @type {boolean}
   */
  @Input() hasLiquor: boolean = false;
  /**
   * Set to `true` if the application has cannabis-related declarations.
   *
   * @type {boolean}
   */
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
