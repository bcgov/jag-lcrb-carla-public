import { Component, Input, OnInit } from "@angular/core";
import { PaymentResult } from "@models/payment-result.model";

@Component({
  selector: "app-invoice-details",
  templateUrl: "./invoice-details.component.html",
  styleUrls: ["./invoice-details.component.scss"]
})
export class InvoiceDetailsComponent implements OnInit {
  @Input()
  data: PaymentResult;

  constructor() {}

  ngOnInit(): void {
  }

}
