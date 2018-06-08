import { Component, OnInit, Input, ViewContainerRef } from '@angular/core';
import { AdoxioLegalEntity } from '../models/adoxio-legalentities.model';
import { AdoxioLegalEntityDataService } from "../services/adoxio-legal-entity-data.service";
import { MatPaginator, MatTableDataSource, MatSort, MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material';
import { ToastsManager } from 'ng2-toastr/ng2-toastr';

@Component({
  selector: 'app-security-assessments',
  templateUrl: './security-assessments.component.html',
  styleUrls: ['./security-assessments.component.scss']
})
export class SecurityAssessmentsComponent implements OnInit {

  @Input() accountId: string;

  adoxioLegalEntityList: AdoxioLegalEntity[] = [];
  dataSource = new MatTableDataSource<AdoxioLegalEntity>();
  public dataLoaded;
  displayedColumns = ['sendConsentRequest', 'firstname', 'lastname', 'email', 'position', 'emailsent'];

  constructor(private legalEntityDataservice: AdoxioLegalEntityDataService, public toastr: ToastsManager,
          vcr: ViewContainerRef) {
    this.toastr.setRootViewContainerRef(vcr);
  }

  ngOnInit() {
    this.getDirectorsAndOfficersAndShareholders();
  }

  getDirectorsAndOfficersAndShareholders() {
    this.legalEntityDataservice.getLegalEntitiesbyPosition("director-officer-shareholder")
      .then((data) => {
        //console.log("getLegalEntitiesbyPosition(\"director-officer-shareholder\"): ", data);
        data.forEach((entry) => {
          entry.sendConsentRequest = false;
        });
        this.dataSource.data = data;
        this.dataLoaded = true;
      });
  }

  sendConsentRequestEmail() {

    let consentRequestList: string[] = [];

    this.dataSource.data.forEach((row) => {
      console.log("row values: ", row.id + " - " + row.sendConsentRequest + " - " + row.firstname);
      if (row.sendConsentRequest) {
        consentRequestList.push(row.id);
      }
    });

    if (consentRequestList) {
      this.legalEntityDataservice.sendConsentRequestEmail(consentRequestList)
        .subscribe(
          res => { this.toastr.success('Consent Request(s) Sent ', 'Success!'); },
          err => {
            //console.log("Error occured");
            this.handleError(err);
          }
      );
    }

  }

  private handleError(error: Response | any) {
    let errMsg: string;
    if (error instanceof Response) {
      const body = error.json() || "";
      const err = body || JSON.stringify(body);
      errMsg = `${error.status} - ${error.statusText || ""} ${err}`;
    } else {
      errMsg = error.message ? error.message : error.toString();
    }
    console.error(errMsg);
  }

}
