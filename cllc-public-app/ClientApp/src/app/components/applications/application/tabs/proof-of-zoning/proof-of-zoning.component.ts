import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { Application } from '@models/application.model';
import { ApplicationDataService } from '@services/application-data.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { FormBase } from '@shared/form-base';
import { FileUploaderComponent } from '@shared/components/file-uploader/file-uploader.component';

@Component({
  selector: 'app-proof-of-zoning',
  templateUrl: './proof-of-zoning.component.html',
  styleUrls: ['./proof-of-zoning.component.scss']
})
export class ProofOfZoningComponent extends FormBase implements OnInit {
  @Input() application: Application;
  @Input() formGroup: FormGroup;
  @ViewChild('proofOfZoningDocs', { static: false }) mainForm: FileUploaderComponent;
  validationMessages: string[];
  // form: FormGroup;
  uploadedZoningDocuments = 0;

  constructor(private applicationDataService: ApplicationDataService,
    private fb: FormBuilder) { 
      super();
    }

  ngOnInit() {
    // this.form = this.fb.group({

    // });
    //this.form.patchValue(this.application);
  }


  getValidationErrors(): string[]{
    let res = [];
    if (this.application && this.application.licenseType !== 'Manufacturer' && (this.uploadedZoningDocuments || 0) < 1) {
      res.push('At least one zoning document is required.');
    }
    return res;
  }

}
