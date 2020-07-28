import { Component, OnInit, Input } from '@angular/core';
import { Application } from '@models/application.model';

@Component({
  selector: 'app-production-stages',
  templateUrl: './production-stages.component.html',
  styleUrls: ['./production-stages.component.scss']
})
export class ProductionStagesComponent implements OnInit {
  @Input() application: Application;
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
  constructor() { }

  ngOnInit() {
  }


  get selectedOptions() { 
  return this.options
            .filter(opt => opt.checked)
            .map(opt => opt.name)
}

}
