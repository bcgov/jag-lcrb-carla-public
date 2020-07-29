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
    {name:'Fermenting', checked:false, readonly:true},
    {name:'Blending', checked:false, readonly: false},
    {name:'Crushing', checked:false, readonly: false} ,
    {name:'Filtering',  checked:false, readonly: false},
    {name:'Aging, for at least 3 months',  checked:false, readonly: false},
    {name:'Secondary fermentation or carbonation', checked:false, readonly: false},
    {name:'Packaging', checked:false, readonly: false}
  ]


  constructor() {
    
  }

  ngOnInit() {
    this.form.addControl('description2', new FormControl(''));
    this.setOptions(this.application.description2);

  }

  // update the combined string from the checkbox options.
  updateOptions(option) {
    let result = "";
    option.checked = !option.checked;
    this.options.forEach(function (value) {
      if (value.checked) {
        if (result !== "") {
          result += "\n";
        }
        result += value.name;
      }
    }, this);
    
    this.application.description2 = result;
    this.form.get('description2').patchValue(result);
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

      value.checked = found;
    }, this);

  }

  get selectedOptions() { 
  return this.options
            .filter(opt => opt.checked)
            .map(opt => opt.name)
}

}
