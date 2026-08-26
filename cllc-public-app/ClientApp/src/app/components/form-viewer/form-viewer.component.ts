import { Component } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { DynamicsForm } from '@models/dynamics-form.model';
import { DynamicsDataService } from '@services/dynamics-data.service';

@Component({
  selector: 'app-form-viewer',
  templateUrl: './form-viewer.component.html',
  styleUrls: ['./form-viewer.component.scss']
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
  constructor(
    private dynamicsDataService: DynamicsDataService,
    private route: ActivatedRoute
  ) {
    this.route.paramMap.subscribe((params) => (this.id = params.get('id')));
  }
}
