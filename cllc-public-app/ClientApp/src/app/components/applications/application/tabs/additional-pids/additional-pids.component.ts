import { Component, Input, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { Application } from '@models/application.model';
import { ApplicationDataService } from '@services/application-data.service';
import { FormBase } from '@shared/form-base';

@Component({
  selector: 'app-additional-pids',
  templateUrl: './additional-pids.component.html',
  styleUrls: ['./additional-pids.component.scss']
})
export class AdditionalPidsComponent extends FormBase implements OnInit {
  @Input()
  application: Application;
  validationMessages: string[];
  @Input()
  form: FormGroup;

  constructor(
    private applicationDataService: ApplicationDataService,
    private fb: FormBuilder
  ) {
    super();
  }

  ngOnInit() {
    //this.form.addControl('hasMultiplePIDs', new FormControl(''));
    this.form.addControl('pidList', new FormControl(''));
    this.form.get('pidList').patchValue(this.application.pidList);
  }
}
