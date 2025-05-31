import { Component, Input, OnInit } from '@angular/core';

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
  styleUrls: ['./permanent-change-declarations.component.scss']
})
export class PermanentChangeDeclarationsComponent implements OnInit {
  /**
   * Set to `true` to force the display of validation messages.
   *
   * @type {boolean} Default is `false`.
   * @memberof PermanentChangeDeclarationsComponent
   */
  @Input() showValidationMessages: boolean = false;

  constructor() {}

  ngOnInit() {}
}
