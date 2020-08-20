import { Component, OnInit, Input } from '@angular/core';
import { Application } from '@models/application.model';
import { FormGroup, FormControl } from '@angular/forms';

@Component({
  selector: 'app-production-stages',
  templateUrl: './production-stages.component.html',
  styleUrls: ['./production-stages.component.scss']
})
export class ProductionStagesComponent implements OnInit {
  @Input() application: Application;
  @Input() form: FormGroup;
  readOnly = false;
  options = [
    {name:'Fermenting', checked:true, readonly:true},
    {name:'Blending', checked:false, readonly: false},
    {name:'Crushing', checked:false, readonly: false} ,
    {name:'Filtering',  checked:false, readonly: false},
    {name:'Aging, for at least 3 months',  checked:false, readonly: false},
    {name:'Secondary fermentation or carbonation', checked:false, readonly: false},
    {name:'Packaging', checked:false, readonly: false}
  ]



  ngOnInit() {
    this.form.addControl('description2', new FormControl(''));
    this.setOptions(this.application.description2);
    this.application.description2 = this.getSelectedOptions();
  }

  // update the combined string from the checkbox options.
  updateOptions(option) {
    
    option.checked = !option.checked;

    
    this.application.description2 = this.getSelectedOptions();;
    this.form.get('description2').patchValue(this.application.description2);
  }



  // Set the checkbox status from the given string
  setOptions(data) {
    if (!data) {
      data = "";
    }
    var dataOptions = data.split("\n");

    this.options.forEach(function (value) {
      this.combinedString = value;
      let found = false;
      if (dataOptions) {
        dataOptions.forEach(function (dataValue) {
          if (dataValue && dataValue === value.name) {
            found = true;
          }
        }, this);
      }
      // Fermenting is always selected, and is read only.
      if (value.name === 'Fermenting') {
        found = true;
      }
      value.checked = found;
    }, this);

  }

  getSelectedOptions() { 
    let result = "";
    
    this.options.forEach(function (value) {
      if (value.checked) {
        if (result !== "") {
          result += "\n";
        }
        result += value.name;
      }
    }, this);
    return result;
}

}
