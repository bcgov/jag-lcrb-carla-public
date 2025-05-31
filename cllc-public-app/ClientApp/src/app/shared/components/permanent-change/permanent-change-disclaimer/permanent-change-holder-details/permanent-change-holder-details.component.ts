import { Component, Input, OnInit } from '@angular/core';
import { ApplicationLicenseSummary } from '@models/application-license-summary.model';

/**
 * The disclaimer section of a permanent change application.
 *
 * @export
 * @class PermanentChangeHolderDetailsComponent
 * @implements {OnInit}
 */
@Component({
  selector: 'app-permanent-change-holder-details',
  templateUrl: './permanent-change-holder-details.component.html',
  styleUrls: ['./permanent-change-holder-details.component.scss']
})
export class PermanentChangeHolderDetailsComponent implements OnInit {
  @Input() liquorLicences: ApplicationLicenseSummary[] = [];
  @Input() cannabisLicences: ApplicationLicenseSummary[] = [];

  constructor() {}

  ngOnInit() {}
}
