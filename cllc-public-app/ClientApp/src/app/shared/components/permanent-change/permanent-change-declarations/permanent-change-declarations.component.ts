import { Component, Input, OnInit } from '@angular/core';
import { ControlContainer, FormGroup, FormGroupDirective } from '@angular/forms';

/**
 * The declarations section of a permanent change application.
 *
 * @export
 * @class PermanentChangeDeclarationsComponent
 * @implements {OnInit}
 */
@Component({
  selector: 'app-permanent-change-declarations',
  templateUrl: './permanent-change-declarations.component.html',
  styleUrls: ['./permanent-change-declarations.component.scss'],
  viewProviders: [{ provide: ControlContainer, useExisting: FormGroupDirective }]
})
export class PermanentChangeDeclarationsComponent implements OnInit {
  @Input() hasLiquor: boolean = false;
  @Input() hasCannabis: boolean = false;
  /**
   * Set to `true` to force the display of validation messages.
   *
   * @type {boolean} Default is `false`.
   * @memberof PermanentChangeDeclarationsComponent
   */
  @Input() showValidationMessages: boolean = false;

  form: FormGroup;

  constructor(public controlContainer: ControlContainer) {}

  ngOnInit() {
    this.form = this.controlContainer.control as FormGroup;
  }
}
