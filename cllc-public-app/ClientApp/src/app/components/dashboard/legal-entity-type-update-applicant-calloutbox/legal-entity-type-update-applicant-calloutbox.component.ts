import { Component, Input, OnInit } from "@angular/core";
import { Account } from "@models/account.model";

@Component({
  selector: "app-legal-entity-type-update-applicant-calloutbox",
  templateUrl: "./legal-entity-type-update-applicant-calloutbox.component.html",
  styleUrls: ["./legal-entity-type-update-applicant-calloutbox.component.scss"]
})
export class LegalEntityTypeUpdateApplicantCalloutboxComponent implements OnInit {
  @Input()
  account: Account;

  constructor() {}

  ngOnInit(): void {
  }

}
