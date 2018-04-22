import { Injectable } from '@angular/core';
import { Http, Headers, Response } from "@angular/http";
import "rxjs/add/operator/toPromise";

import { PolicyDocument } from "../models/policy-document.model";
import { PolicyDocumentSummary } from "../models/policy-document-summary.model";

@Injectable()
export class PolicyDocumentDataService {
   constructor(private http: Http) { }

   getPolicyDocument(slug: any) {
     let headers = new Headers();
     headers.append("Content-Type", "application/json");

     return this.http.get("api/policydocument/" + slug, {
       headers: headers
     })
       .toPromise()
       .then((res: Response) => {
         let data = res.json();
         let policyDocument = new PolicyDocument();
         policyDocument.id = data.id;         
         policyDocument.slug = data.slug;
         policyDocument.title = data.title;
         policyDocument.body = data.body;
         policyDocument.category = data.category;
         policyDocument.menuText = data.menuText;
         policyDocument.displayOrder = data.displayOrder;
         return policyDocument;
       })
       .catch(this.handleError);
   }

   getPolicyDocuments(category: string) {     
       let headers = new Headers();
       headers.append("Content-Type", "application/json");

       return this.http.get("api/policydocument?category=" + category, {
         headers: headers
       })
         .toPromise()
         .then((res: Response) => {
           let data = res.json();
           let allPolicyDocuments = [];

           data.forEach((entry) => {
             let policyDocumentSummary = new PolicyDocumentSummary();
             policyDocumentSummary.slug = entry.slug;
             policyDocumentSummary.menuText = entry.menuText;
             allPolicyDocuments.push(policyDocumentSummary);
           });
           return allPolicyDocuments;
         })
         .catch(this.handleError);
   }
  
     private handleError(error: Response | any) {
     let errMsg: string;
     if (error instanceof Response) {
       const body = error.json() || "";
       const err = body.error || JSON.stringify(body);
       errMsg = `${error.status} - ${error.statusText || ""} ${err}`;
     } else {
       errMsg = error.message ? error.message : error.toString();
     }
     console.error(errMsg);
     return Promise.reject(errMsg);
   }
}
