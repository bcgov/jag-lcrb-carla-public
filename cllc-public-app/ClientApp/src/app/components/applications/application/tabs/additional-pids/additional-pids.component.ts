import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { Application } from '@models/application.model';
import { ApplicationDataService } from '@services/application-data.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { FormBase } from '@shared/form-base';

@Component({
  selector: 'app-additional-pids',
  templateUrl: './additional-pids.component.html',
  styleUrls: ['./additional-pids.component.scss']
})
export class AdditionalPidsComponent extends FormBase implements OnInit {
  @Input() application: Application;
  validationMessages: string[];
  form: FormGroup;

  constructor(private applicationDataService: ApplicationDataService,
    private fb: FormBuilder) {
      super();
     }

  ngOnInit() {
    this.form = this.fb.group({
      hasMultiplePIDs: ['', []],
      pidList: ['', []]
    });
    this.form.patchValue(this.application);
  }

}
