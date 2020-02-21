import { Component, OnInit } from '@angular/core';
import { SecurityScreeningCategorySummary } from '@models/security-screening-category-summary.model';
import { SecurityScreeningSummary } from '@models/security-screening-summary.model';
import { MatSnackBar } from '@angular/material';
import { LegalEntityDataService } from '@services/legal-entity-data.service';

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

  constructor(private snackBar: MatSnackBar,
    private legalEntityDataService: LegalEntityDataService) {
      this.legalEntityDataService.getCurrentSecurityScreeningItems()
      .subscribe(summary => {
        this.data = summary;
      })
   }

  ngOnInit() {
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
