import { Component, Input, OnInit } from '@angular/core';

/**
 * The personal history summary forms section of a permanent change application.
 *
 * @export
 * @class PermanentChangePersonalHistorySummaryFormsComponent
 * @implements {OnInit}
 */
@Component({
  selector: 'app-permanent-change-personal-history-summary-forms',
  templateUrl: './permanent-change-personal-history-summary-forms.component.html',
  styleUrls: ['./permanent-change-personal-history-summary-forms.component.scss']
})
export class PermanentChangePersonalHistorySummaryFormsComponent implements OnInit {
  /**
   * Set to `true` to indicate that the applicant has at least 1 liquor licence, which enables this section.
   *
   * @type {boolean} Default is `false`.
   * @memberof PermanentChangePersonalHistorySummaryFormsComponent
   */
  @Input() hasLiquorLicences: boolean = false;

  constructor() {}

  ngOnInit() {}
}
