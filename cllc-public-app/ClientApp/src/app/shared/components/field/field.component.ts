import { Component, OnInit, Input } from "@angular/core";
import { faChevronLeft } from "@fortawesome/free-solid-svg-icons";

@Component({
  selector: "app-field",
  templateUrl: "./field.component.html",
  styleUrls: ["./field.component.scss"]
})
export class FieldComponent implements OnInit {
  faChevronLeft = faChevronLeft;
  @Input()
  required = false;
  @Input()
  showChevrons = true;
  @Input()
  isFullWidth = false;
  @Input()
  valid = true;
  @Input()
  label: string;
  @Input()
  leadingText: string;
  @Input()
  afterLabelText: string;
  @Input()
  errorMessage: string;

  constructor() {}

  ngOnInit() {
  }

}
