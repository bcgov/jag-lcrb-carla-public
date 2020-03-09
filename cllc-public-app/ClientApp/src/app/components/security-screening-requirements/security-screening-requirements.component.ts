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
  data: SecurityScreeningSummary;
  applicationId: string;
  applicationType: ApplicationType;
  liquorLicenceExist: boolean;
  cannabisLicenceExist: boolean;
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
      if( application.applicationType.category === 'Liquor'){
          this.isLiquorApplication = true;
        }
        if( application.applicationType.category === 'Cannabis'){
          this.isCannabisApplication = true;
        }
      });
    }

    this.licenseDataService.getAllCurrentLicenses()
    .subscribe(licences => {
      this.liquorLicenceExist = licences.filter(lc => lc.applicationTypeCategory === 'Liquor').length > 0;
      this.cannabisLicenceExist = licences.filter(lc => lc.applicationTypeCategory === 'Cannabis').length > 0;
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
