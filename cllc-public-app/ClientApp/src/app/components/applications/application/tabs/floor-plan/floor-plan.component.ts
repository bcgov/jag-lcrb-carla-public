import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { Application } from '@models/application.model';
import { ApplicationDataService } from '@services/application-data.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { FormBase, ApplicationHTMLContent } from '@shared/form-base';
import { ApplicationTypeNames, FormControlState } from '@models/application-type.model';

@Component({
  selector: 'app-floor-plan',
  templateUrl: './floor-plan.component.html',
  styleUrls: ['./floor-plan.component.scss']
})
export class FloorPlanComponent extends FormBase implements OnInit {
  @Input() application: Application;
  //@ViewChild('mainForm', { static: false }) mainForm: FileUploaderComponent;
  validationMessages: string[];
  form: FormGroup;
  FormControlState = FormControlState;
  @Input() htmlContent: ApplicationHTMLContent;

  constructor(private applicationDataService: ApplicationDataService,
    private fb: FormBuilder) {
      super();
     }

  ngOnInit() {
    this.form = this.fb.group({
      //IsReadyProductNotVisibleOutside: ['', []],
    });
    //this.form.patchValue(this.application);
  }

  isRAS(): boolean {
    return this.application.licenseType === 'Rural Agency Store';
  }

}
