import { Component, OnInit } from "@angular/core";
import { faEnvelope, faFax, faPhone, faQuestion } from "@fortawesome/free-solid-svg-icons";

@Component({
  selector: "app-worker-qualification",
  templateUrl: "./worker-qualification.component.html",
  styleUrls: ["./worker-qualification.component.scss"]
})
export class WorkerQualificationComponent implements OnInit {
  faQuestion = faQuestion;
  faEnvelope = faEnvelope;
  faFax = faFax;
  faPhone = faPhone;

  constructor() {}

  ngOnInit() {
  }

}
