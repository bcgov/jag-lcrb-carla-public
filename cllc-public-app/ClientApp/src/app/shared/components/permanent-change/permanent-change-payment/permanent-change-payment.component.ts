import { Component, Input, OnInit } from '@angular/core';
import { ControlContainer, FormGroup, FormGroupDirective } from '@angular/forms';
import { ApplicationLicenseSummary } from '@models/application-license-summary.model';
import { Application } from '@models/application.model';

/**
 * The payment section of a permanent change application.
 *
 * @export
 * @class PermanentChangePaymentComponent
 * @implements {OnInit}
 */
@Component({
  selector: 'app-permanent-change-payment',
  templateUrl: './permanent-change-payment.component.html',
  styleUrls: ['./permanent-change-payment.component.scss'],
  viewProviders: [{ provide: ControlContainer, useExisting: FormGroupDirective }]
})
export class PermanentChangePaymentComponent implements OnInit {
  @Input() application: Application;
  @Input() liquorLicences: ApplicationLicenseSummary[] = [];
  @Input() cannabisLicences: ApplicationLicenseSummary[] = [];
  @Input() selectedChangeList: any[];

  @Input() canCreateNewApplication: boolean;
  @Input() onNewApplication: (invoiceType: 'primary' | 'secondary') => void;
  @Input() createApplicationInProgress: boolean;
  @Input() primaryInvoice: any;
  @Input() secondaryInvoice: any;
  @Input() primaryPaymentInProgress: boolean;
  @Input() secondaryPaymentInProgress: boolean;

  @Input() onSubmit: (invoiceType: 'primary' | 'secondary') => void;

  form: FormGroup;

  constructor(public controlContainer: ControlContainer) {}

  ngOnInit() {
    this.form = this.controlContainer.control as FormGroup;
  }

  /**
   * Return `true` if there are any cannabis licences with fees that require payment.
   *
   * @return {*}  {boolean}
   */
  get isCannabisPaymentRequired(): boolean {
    return this.cannabisLicences?.length > 0 && this.selectedChangeList.some((change) => change.CannabisFee > 0);
  }

  /**
   * Return `true` if the cannabis invoice has been paid.
   *
   * @readonly
   * @type {boolean}
   */
  get isCannabisInvoicePaid(): boolean {
    return this.application?.primaryInvoicePaid || false;
  }

  /**
   * Return `true` if there are any liquor licences with fees that require payment.
   *
   * @return {*}  {boolean}
   */
  get isLiquorPaymentRequired(): boolean {
    return this.liquorLicences?.length > 0 && this.selectedChangeList.some((change) => change.LiquorFee > 0);
  }

  /**
   * Return `true` if the liquor invoice has been paid.
   *
   * @readonly
   * @type {boolean}
   */
  get isLiquorInvoicePaid(): boolean {
    return this.application?.secondaryInvoicePaid || false;
  }
}
