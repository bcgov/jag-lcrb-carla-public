import { Component, OnInit, Input } from "@angular/core";
import { Application } from "@models/application.model";

@Component({
  selector: "app-lg-info-panel",
  templateUrl: "./lg-info-panel.component.html",
  styleUrls: ["./lg-info-panel.component.scss"]
})
export class LgInfoPanelComponent implements OnInit {
  @Input()
  application: Application;
  category: String;

  constructor() {}

  ngOnInit() {
    this.category = this.application.applicationType.isEndorsement ? "endorsement" : "application";
  }

}
