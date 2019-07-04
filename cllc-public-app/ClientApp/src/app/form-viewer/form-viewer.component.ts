import { Component, Input, OnInit } from '@angular/core';
import { DynamicsDataService } from '../services/dynamics-data.service';
import { ActivatedRoute, Router } from '@angular/router';

import { Injectable } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import { DynamicsForm } from '../models/dynamics-form.model';

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
    this.route.paramMap.subscribe(params => this.id = params.get('id'));
  }


}
