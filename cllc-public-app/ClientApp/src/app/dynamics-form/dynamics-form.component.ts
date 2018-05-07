import { Component, Input, OnInit } from '@angular/core';
import { DynamicsDataService } from "../services/dynamics-data.service"
import { ActivatedRoute, Router } from '@angular/router';

import { Injectable } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { DynamicsForm } from "../models/dynamics-form.model";

import { User } from "../models/user.model";

@Component({
    selector: 'app-dynamics-form',
    templateUrl: './dynamics-form.component.html',
    styleUrls: ['./dynamics-form.component.scss']
})
/** dynamics-form component*/
export class DynamicsFormComponent {
  @Input('formid') formid: string;
  @Input('hideHeader') hideHeader: boolean;
  @Input('hideSubmit') hideSubmit: boolean;
  @Input('enableUserOverride') enableUserOverride: boolean;
  @Input('currentUser') currentUser: User;

  public payload: string;
  public responseText: string;
  public dynamicsForm: DynamicsForm;
  public form: FormGroup;

    /** dynamics-form ctor */
  constructor(private dynamicsDataService: DynamicsDataService) {

  }

    toFormGroup(dynamicsForm: DynamicsForm) {
      let group: any = {};

      dynamicsForm.tabs.forEach(tab => {
        tab.sections.forEach(section => {
          section.fields.forEach(field => {
            
            group[field.datafieldname] = new FormControl('');
            if (field.controltype == "CheckBoxControl") {
              group[field.datafieldname].patchValue(false);
            }
            if (this.enableUserOverride) {
              if (field.datafieldname == "firstname") {
                group[field.datafieldname].patchValue(this.currentUser.firstname);
              }
              else if (field.datafieldname == "lastname") {
                group[field.datafieldname].patchValue(this.currentUser.lastname);
              }
            }

          });
        });
      });

      //group[question.key] = question.required ? new FormControl(question.value || '', Validators.required)

      return new FormGroup(group);
    }

    ngOnInit(): void {

      if (this.formid != null) {
        // get data.
        this.dynamicsDataService.getForm(this.formid)
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
