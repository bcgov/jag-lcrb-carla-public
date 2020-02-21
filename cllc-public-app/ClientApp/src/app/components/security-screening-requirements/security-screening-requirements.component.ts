import { Component, OnInit } from '@angular/core';
import { SecurityScreeningCategorySummary } from '@models/security-screening-category-summary.model';
import { SecurityScreeningSummary } from '@models/security-screening-summary.model';
import { MatSnackBar } from '@angular/material';
import { LegalEntityDataService } from '@services/legal-entity-data.service';
import { ActivatedRoute } from '@angular/router';
import { ApplicationDataService } from '@services/application-data.service';
import { LicenseDataService } from '@services/license-data.service';
import { ApplicationType } from '@models/application-type.model';

@Component({
  selector: 'app-security-screening-requirements',
  templateUrl: './security-screening-requirements.component.html',
  styleUrls: ['./security-screening-requirements.component.scss']
})
export class SecurityScreeningRequirementsComponent implements OnInit {

  data: SecurityScreeningSummary = <SecurityScreeningSummary>{
    cannabis: <SecurityScreeningCategorySummary>{
      outstandingItems: [
        {
          firstName: 'Moffat',
          middleName: 'Tshepi',
          lastName: 'Sehudi',
          dateSubmitted: new Date(),
          screeningLink: 'link'
        },
        {
          firstName: 'Moffat',
          middleName: 'Tshepi',
          lastName: 'Sehudi',
          dateSubmitted: new Date(),
          screeningLink: 'link'
        },
        {
          firstName: 'Moffat',
          middleName: 'Tshepi',
          lastName: 'Sehudi',
          dateSubmitted: new Date(),
          screeningLink: 'link'
        }
      ],
      completedItems: [
        {
          firstName: 'Moffat2',
          middleName: 'Tshepi2',
          lastName: 'Sehudi2',
          dateSubmitted: new Date(),
          screeningLink: 'link'
        }
      ]
    },
    liquor: <SecurityScreeningCategorySummary>{
      outstandingItems: [
        {
          firstName: 'b',
          middleName: 'Tshepi',
          lastName: 'Sehudi',
          dateSubmitted: new Date(),
          screeningLink: 'link'
        },
        {
          firstName: 'b',
          middleName: 'Tshepi',
          lastName: 'Sehudi',
          dateSubmitted: new Date(),
          screeningLink: 'link'
        }
      ],
      completedItems: [
        {
          firstName: 'Cory',
          middleName: 'Will',
          lastName: 'Saunder',
          dateSubmitted: new Date(),
          screeningLink: 'link'
        },
        {
          firstName: 'Cory',
          middleName: 'Will',
          lastName: 'R',
          dateSubmitted: new Date(),
          screeningLink: 'link'
        },
        {
          firstName: 'Cory',
          middleName: 'Will',
          lastName: 'Saunder',
          dateSubmitted: new Date(),
          screeningLink: 'link'
        },
        {
          firstName: 'Cory',
          middleName: 'Will',
          lastName: 'Saunder',
          dateSubmitted: new Date(),
          screeningLink: 'link'
        }
      ]
    }
  }
  applicationId: string;
  applicationType: ApplicationType;
  liquorLicencesExist: boolean;
  cannabisLicencesExist: boolean;
  isLiquorApplication: boolean;
  isCannabisApplication: boolean;

  constructor(private snackBar: MatSnackBar,
    private route: ActivatedRoute,
    private applicationDataService: ApplicationDataService,
    private licenseDataService: LicenseDataService,
    private legalEntityDataService: LegalEntityDataService) {
      this.legalEntityDataService.getCurrentSecurityScreeningItems()
      .subscribe(summary => {
        this.data = summary;
      });

      this.route.paramMap.subscribe(pmap => this.applicationId = pmap.get('applicationId'));
   }

  ngOnInit() {

    if(this.applicationId){
      this.applicationDataService.getApplicationById(this.applicationId)
      .subscribe((application) => {
        this.applicationType = application.applicationType;
        if( application.applicationType.name === 'Liquor Primary'){
          this.isLiquorApplication = true;
        }
        if( application.applicationType.name === 'Cannabis Retail Store'){
          this.isCannabisApplication = true;
        }
      });
    }

    this.licenseDataService.getAllCurrentLicenses()
    .subscribe(licences => {
      this.liquorLicencesExist = licences.filter(lc => lc.licenceTypeName === 'Liquor Primary').length > 0;
      this.cannabisLicencesExist = licences.filter(lc => lc.licenceTypeName === 'Cannabis Retail Store').length > 0;
    });
  }
  
  // Copy value to clipboard
  copyMessage(value: string) {
    const selBox = document.createElement('textarea');
    selBox.style.position = 'fixed';
    selBox.style.left = '0';
    selBox.style.top = '0';
    selBox.style.opacity = '0';
    selBox.value = value;
    document.body.appendChild(selBox);
    selBox.focus();
    selBox.select();
    document.execCommand('copy');
    document.body.removeChild(selBox);
    this.snackBar.open('The link is copied to the clipboard', '', { duration: 2500, panelClass: ['green-snackbar'] });
  }

}
