import { Component, OnInit, Input, ViewContainerRef, ViewChild } from '@angular/core';
import { MatPaginator, MatTableDataSource, MatSnackBar } from '@angular/material';
import { Subscription } from 'rxjs';
import { AdoxioLegalEntity } from '../../../models/adoxio-legalentities.model';
import { AdoxioLegalEntityDataService } from '../../../services/adoxio-legal-entity-data.service';
import { DynamicsDataService } from '../../../services/dynamics-data.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-security-assessments',
  templateUrl: './security-assessments.component.html',
  styleUrls: ['./security-assessments.component.scss']
})
export class SecurityAssessmentsComponent implements OnInit {

  @Input() accountId: string;
  @Input() parentLegalEntityId: string;
  @Input() businessType: string;

  adoxioLegalEntityList: AdoxioLegalEntity[] = [];
  dataSource = new MatTableDataSource<AdoxioLegalEntity>();
  displayedColumns = ['sendConsentRequest', 'firstname', 'lastname', 'email', 'position', 'emailsent'];
  busy: Promise<any>;
  busyObsv: Subscription;

  @ViewChild(MatPaginator) paginator: MatPaginator;

  constructor(private legalEntityDataservice: AdoxioLegalEntityDataService,
    private route: ActivatedRoute,
    private dynamicsDataService: DynamicsDataService,
    public snackBar: MatSnackBar,
    vcr: ViewContainerRef) {
  }

  ngOnInit() {
    this.route.parent.params.subscribe(p => {
      this.parentLegalEntityId = p.legalEntityId;
      this.accountId = p.accountId;
      this.dynamicsDataService.getRecord('account', this.accountId)
        .then((data) => {
          this.businessType = data.businessType;
        });
      this.getDirectorsAndOfficersAndShareholders();
    });
    this.dataSource.paginator = this.paginator;
  }

  getDirectorsAndOfficersAndShareholders() {
    const legalEntitiesList = [];
    this.busyObsv = this.legalEntityDataservice.getLegalEntitiesbyPosition(this.parentLegalEntityId, 'director-officer-shareholder')
      .subscribe((data) => {
        data.forEach((entry) => {
          entry.sendConsentRequest = !entry.securityAssessmentEmailSentOn;
          if (entry.isindividual) {
            legalEntitiesList.push(entry);
          }
        });
        this.dataSource.data = legalEntitiesList;
      });
  }

  getRoles(legalEntity: AdoxioLegalEntity): string {
    const roles = [];
    if (legalEntity.isDirector === true) {
      roles.push('Director');
    }
    if (legalEntity.isOfficer === true) {
      roles.push('Officer');
    }
    if (legalEntity.isOwner === true) {
      roles.push('Owner');
    }
    if (legalEntity.isPartner === true) {
      roles.push('Partner');
    }
    if (legalEntity.isSeniorManagement === true) {
      roles.push('Senior Manager');
    }
    if (legalEntity.isShareholder === true) {
      roles.push('Shareholder');
    }
    return roles.join(', ');
  }

  anySelected(): boolean {
    const selected = this.dataSource.data.filter(i => i.sendConsentRequest === true);
    return selected.length > 0;
  }

  sendConsentRequestEmail() {
    const consentRequestList: string[] = [];

    this.dataSource.data.forEach((row) => {
      if (row.sendConsentRequest) {
        consentRequestList.push(row.id);
      }
    });

    if (consentRequestList) {
      this.busyObsv = this.legalEntityDataservice.sendConsentRequestEmail(consentRequestList)
        .subscribe(
          res => {
            this.snackBar.open('Consent Request(s) Sent', 'Success',
            { duration: 2500, panelClass: ['red-snackbar'] });
          },
          err => {
            this.snackBar.open('Consent Request(s) Sent', 'Failed',
            { duration: 4500, panelClass: ['red-snackbar'] });
          }
        );
    }

  }

  private handleError(error: Response | any) {
    let errMsg: string;
    if (error instanceof Response) {
      const body = error.json() || '';
      const err = body || JSON.stringify(body);
      errMsg = `${error.status} - ${error.statusText || ''} ${err}`;
    } else {
      errMsg = error.message ? error.message : error.toString();
    }
    console.error(errMsg);
  }

}
