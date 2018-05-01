import { Component, Input, OnInit } from '@angular/core';
import { DynamicsDataService } from "../services/dynamics-data.service"
import { ActivatedRoute, Router } from '@angular/router';

import { Injectable } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { DynamicsForm } from "../models/dynamics-form.model";

@Component({
    selector: 'app-form-viewer',
    templateUrl: './form-viewer.component.html',
    styleUrls: ['./form-viewer.component.scss']
})
/** form-viewer component*/
// reference - https://angular.io/guide/dynamic-form

export class FormViewerComponent {
  public id: string;
  public payload: string;
  public responseText: string;
  public dynamicsForm: DynamicsForm;
  public form: FormGroup;

    /** form-viewer ctor */
  constructor(private dynamicsDataService: DynamicsDataService, private route: ActivatedRoute) {
      this.id = this.route.snapshot.params["id"];
  }

    toFormGroup(dynamicsForm: DynamicsForm) {
      let group: any = {};

      dynamicsForm.tabs.forEach(tab => {
        tab.sections.forEach(section => {
          section.fields.forEach(field => {
            group[field.datafieldname] = new FormControl('');
          });
        });
      });

        //group[question.key] = question.required ? new FormControl(question.value || '', Validators.required)

      return new FormGroup(group);
    }

    ngOnInit(): void {

      if (this.id != null) {
        // get data.
        this.dynamicsDataService.getForm(this.id)
          .then((dynamicsForm) => {
            this.dynamicsForm = dynamicsForm;
            // update the form group.
            this.form = this.toFormGroup(this.dynamicsForm);
          });        
      }
    }

    onSubmit() {
      this.payload = JSON.stringify(this.form.value);
      this.dynamicsDataService.createRecord(this.dynamicsForm.entity, this.payload)
        .then((data) => {
          this.responseText = JSON.stringify(data);
        });        

    }
}
