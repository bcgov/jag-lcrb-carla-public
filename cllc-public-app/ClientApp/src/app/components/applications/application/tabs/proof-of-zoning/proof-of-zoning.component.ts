import { Component, OnInit, Input, ViewChild } from "@angular/core";
import { Application } from "@models/application.model";
import { ApplicationDataService } from "@services/application-data.service";
import { FormBuilder, FormGroup } from "@angular/forms";
import { FormBase } from "@shared/form-base";
import { FileUploaderComponent } from "@shared/components/file-uploader/file-uploader.component";

@Component({
  selector: "app-proof-of-zoning",
  templateUrl: "./proof-of-zoning.component.html",
  styleUrls: ["./proof-of-zoning.component.scss"]
})
export class ProofOfZoningComponent extends FormBase implements OnInit {
  @Input()
  application: Application;
  @Input()
  formGroup: FormGroup;
  @ViewChild("proofOfZoningDocs")
  mainForm: FileUploaderComponent;
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
  isPermittedInZoningChanged() {
    this.application.isPermittedInZoning = !this.application.isPermittedInZoning;
  }
  showQuestion() {

    const licenceTypes = [
      "Manufacturer", "Food Primary", "Liquor Primary", "Liquor Primary Club"
    ];
    const applicationTypes = [
      "On-Site Store Endorsement",
      this.ApplicationTypeNames.FPRelo
    ];

    return (
      licenceTypes.indexOf(this.application.licenseType) > -1 ||
        applicationTypes.indexOf(this.application.applicationType.name) > -1
    );

  }


  getValidationErrors(): string[] {
    const res = [];
    if (!this.showQuestion() && (this.uploadedZoningDocuments || 0) < 1) {
      res.push("At least one zoning document is required.");
    }
    return res;
  }
  relocateOnSiteStoreChanged() {
    this.application.relocateOnSiteStore = !this.application.relocateOnSiteStore;
  }
  relocatePicnicAreaEndorsementChanged() {
    this.application.relocatePicnicAreaEndorsement = !this.application.relocatePicnicAreaEndorsement;

  }
}
