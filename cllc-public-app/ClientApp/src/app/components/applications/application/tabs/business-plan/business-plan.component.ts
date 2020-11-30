import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { Application } from '@models/application.model';
import { ApplicationDataService } from '@services/application-data.service';
import { FormBuilder, FormGroup, Validators, FormControl } from '@angular/forms';
import { FormBase } from '@shared/form-base';
import { ProductionStagesComponent } from './production-stages/production-stages.component';

//import { FileUploaderComponent } from '@shared/components/file-uploader/file-uploader.component';
//import { UPLOAD_FILES_MODE } from '@components/licences/licences.component';

@Component({
  selector: 'app-business-plan',
  templateUrl: './business-plan.component.html',
  styleUrls: ['./business-plan.component.scss']
})
export class BusinessPlanComponent extends FormBase implements OnInit {
  @Input() application: Application;
  //@ViewChild('mainForm', { static: false }) mainForm: FileUploaderComponent;
  validationMessages: string[];
  

  @Input() form: FormGroup;
  @ViewChild(ProductionStagesComponent)
  private productionStages: ProductionStagesComponent;

  constructor(private applicationDataService: ApplicationDataService,
    private fb: FormBuilder) {
    super();
  }

  ngOnInit() {
    this.form.addControl('licenceSubCategory', new FormControl(''));
    this.form.addControl('mfgBrewPubOnSite', new FormControl(''));
    this.form.addControl('mfgPipedInProduct', new FormControl(''));
    this.form.addControl('neutralGrain', new FormControl(''));
    this.form.addControl('mfgAcresOfGrapes', new FormControl(''));
    this.form.addControl('mfgAcresOfFruit', new FormControl(''));
    this.form.addControl('mfgAcresOfHoney', new FormControl(''));
    this.form.addControl('mfgUsesNeutralGrainSpirits', new FormControl(''));
    
   
    
    this.form.get('licenceSubCategory').patchValue(this.application.licenceSubCategory);
    this.form.get('mfgBrewPubOnSite').patchValue(this.application.mfgBrewPubOnSite);
    this.form.get('mfgPipedInProduct').patchValue(this.application.mfgPipedInProduct);
    this.form.get('mfgAcresOfGrapes').patchValue(this.application.mfgAcresOfGrapes);
    this.form.get('mfgAcresOfFruit').patchValue(this.application.mfgAcresOfHoney);
    this.form.get('mfgAcresOfHoney').patchValue(this.application.mfgAcresOfHoney);
    this.form.get('mfgUsesNeutralGrainSpirits').patchValue(this.application.mfgUsesNeutralGrainSpirits);
    

    // to do patch in a value to description2 with subscription to productionStages.selectedobjects
    // each option should be separated with a \n so that it shows as separate lines on the field in dynamics

    

    
  }

    /* Helper functions for the Manufactuer Licence Business Plan
    There are a lot of conditional requirements depending on what is selected.
    Most are self explanatory
  */

 hasType(): boolean {
  // to do, set validation requirements
   return this.form.get('licenceSubCategory').value;
}

isBrewery(): boolean {
  // to do, set validation requirements
  return this.form.get('licenceSubCategory').value === "Brewery";
}
isWinery(): boolean {
  // to do, set validation requirements
  return this.form.get('licenceSubCategory').value === "Winery";
}

isCopacker(): boolean {
  return this.form.get('licenceSubCategory').value === "Co-packer";
}
isDistillery(): boolean {
  return this.form.get('licenceSubCategory').value === "Distillery";
}


isBrewPub(): boolean {
  // to do, set validation requirements
  return this.form.get('licenceSubCategory').value === "Brewery" && this.form.get('mfgBrewPubOnSite').value === "Yes";
}

}


