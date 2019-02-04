import { Injectable } from '@angular/core';
import { Http, Headers, Response } from '@angular/http';

import { DynamicsForm } from '../models/dynamics-form.model';
import { DynamicsFormTab } from '../models/dynamics-form-tab.model';
import { DynamicsFormSection } from '../models/dynamics-form-section.model';
import { DynamicsFormField } from '../models/dynamics-form-field.model';
import { DynamicsFormFieldOption } from '../models/dynamics-form-field-option.model';

@Injectable()
export class DynamicsDataService {
   constructor(private http: Http) { }

   getForm(id: string) {
     const headers = new Headers();
     headers.append('Content-Type', 'application/json');

     return this.http.get('api/systemform/' + id, {
       headers: headers
     })
       .toPromise()
       .then((res: Response) => {
         const data = res.json();
         const dynamicsForm = new DynamicsForm();
         dynamicsForm.id = data.id;
         dynamicsForm.name = data.name;
         dynamicsForm.displayname = data.displayname;
         dynamicsForm.entity = data.entity;

         // process the form tabs.
         dynamicsForm.tabs = [];
         data.tabs.forEach((tab) => {
           const newTab = new DynamicsFormTab();
           newTab.id = tab.id;
           newTab.name = tab.name;
           newTab.visible = tab.visible;
           newTab.showlabel = tab.showlabel;
           newTab.sections = [];
           tab.sections.forEach((section) => {
             const newSection = new DynamicsFormSection();
             newSection.id = section.id;
             newSection.name = section.name;
             newSection.visible = section.visible;
             newSection.showlabel = section.showlabel;
             newSection.fields = [];
             section.fields.forEach((field) => {
               const newField = new DynamicsFormField();
               newField.name = field.name;
               newField.datafieldname = field.datafieldname;
               newField.showlabel = field.showlabel;
               newField.visible = field.visible;
               newField.classid = field.classid;
               newField.controltype = field.controltype;
               newField.required = field.required;

               newField.options = [];
               if (field.options) {
                 field.options.forEach((option) => {
                   const newFieldOption = new DynamicsFormFieldOption();
                   newFieldOption.description = option.description;
                   newFieldOption.label = option.label;
                   newFieldOption.value = option.value;
                   newField.options.push(newFieldOption);
                 });
               }
               newSection.fields.push(newField);
             });
             newTab.sections.push(newSection);
           });


           dynamicsForm.tabs.push(newTab);
         });


         return dynamicsForm;
       })
       .catch(this.handleError);
  }

  // load a record from Dynamics.
  getRecord(entity: string, recordId: string) {
    const headers = new Headers();
    headers.append('Content-Type', 'application/json');

    return this.http.get('api/' + entity + '/' + recordId, {
      headers: headers
    })
      .toPromise()
      .then((res: Response) => {
        return res.json();
      })
      .catch(this.handleError);
  }

   createRecord(entity: string, data: any) {
     const headers = new Headers();
     headers.append('Content-Type', 'application/json');

     return this.http.post('api/' + entity, data, {
       headers: headers
     })
       .toPromise()
       .then((res: Response) => {
         return res.json();
       })
       .catch(this.handleError);
  }


  updateRecord(entity: string, id: string, data: any) {
    const headers = new Headers();
    headers.append('Content-Type', 'application/json');

    return this.http.put('api/' + entity + '/' + id, data, {
      headers: headers
    })
      .toPromise()
      .then((res: Response) => {
        return res.json();
      })
      .catch(this.handleError);
  }


     private handleError(error: Response | any) {
     let errMsg: string;
     if (error instanceof Response) {
       const body = error.json() || '';
       const err = body.error || JSON.stringify(body);
       errMsg = `${error.status} - ${error.statusText || ''} ${err}`;
     } else {
       errMsg = error.message ? error.message : error.toString();
     }
     console.error(errMsg);
     return Promise.reject(errMsg);
   }
}
