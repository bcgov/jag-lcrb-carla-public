import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { Application } from '@models/application.model';
import { ApplicationDataService } from '@services/application-data.service';
import { FormBuilder, FormGroup, Validators, FormControl } from '@angular/forms';
import { FormBase } from '@shared/form-base';

@Component({
  selector: 'app-additional-pids',
  templateUrl: './additional-pids.component.html',
  styleUrls: ['./additional-pids.component.scss']
})
export class AdditionalPidsComponent extends FormBase implements OnInit {
  @Input() application: Application;
  validationMessages: string[];
  @Input() form: FormGroup;

  constructor(private applicationDataService: ApplicationDataService,
    private fb: FormBuilder) {
      super();
     }

  ngOnInit() {
    //this.form.addControl('hasMultiplePIDs', new FormControl(''));
    this.form.addControl('pidList', new FormControl(''));
    this.form.get('pidList').patchValue(this.application.pidList);
}

}
