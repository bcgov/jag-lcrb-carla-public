import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { Application } from '@models/application.model';
import { ApplicationDataService } from '@services/application-data.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { FormBase } from '@shared/form-base';

@Component({
  selector: 'app-proof-of-zoning',
  templateUrl: './proof-of-zoning.component.html',
  styleUrls: ['./proof-of-zoning.component.scss']
})
export class ProofOfZoningComponent extends FormBase implements OnInit {
  @Input() application: Application;
  //@ViewChild('mainForm', { static: false }) mainForm: FileUploaderComponent;
  validationMessages: string[];
  form: FormGroup;

  constructor(private applicationDataService: ApplicationDataService,
    private fb: FormBuilder) { 
      super();
    }

  ngOnInit() {
    this.form = this.fb.group({
    });
    //this.form.patchValue(this.application);
  }

}
