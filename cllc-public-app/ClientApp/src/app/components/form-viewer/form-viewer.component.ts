import { Component } from "@angular/core";
import { DynamicsDataService } from "@services/dynamics-data.service";
import { ActivatedRoute } from "@angular/router";

import { FormGroup } from "@angular/forms";

import { DynamicsForm } from "@models/dynamics-form.model";

@Component({
  selector: "app-form-viewer",
  templateUrl: "./form-viewer.component.html",
  styleUrls: ["./form-viewer.component.scss"]
})
/** form-viewer component*/
// reference - https://angular.io/guide/dynamic-form
export class FormViewerComponent {
  id: string;
  payload: string;
  responseText: string;
  dynamicsForm: DynamicsForm;
  form: FormGroup;

  /** form-viewer ctor */
  constructor(private dynamicsDataService: DynamicsDataService, private route: ActivatedRoute) {
    this.route.paramMap.subscribe(params => this.id = params.get("id"));
  }


}
