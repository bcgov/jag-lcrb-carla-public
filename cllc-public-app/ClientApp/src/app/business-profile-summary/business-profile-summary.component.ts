import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator, MatTableDataSource, MatSort } from '@angular/material';
import { LegalEntityDataService } from '../services/legal-entity-data.service';
import { Subscription } from 'rxjs';
import { Router } from '@angular/router';
import { AccountDataService } from '../services/account-data.service';

export class ProfileSummary {
  legalEntityId: string;
  accountId: string;
  shareholderAccountId: string;
  name: string;
  legalentitytype: string;
  profileComplete: string;
  businessRelationship: string;
  isComplete: boolean;
}

@Component({
  selector: 'app-business-profile-summary',
  templateUrl: './business-profile-summary.component.html',
  styleUrls: ['./business-profile-summary.component.scss']
})
export class BusinessProfileSummaryComponent implements OnInit {

  displayedColumns = ['organization', 'businessRelationship', 'profileComplete'];
  dataSource = new MatTableDataSource<ProfileSummary>();
  profileSummaryList: ProfileSummary[] = [];
  busy: Subscription;

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // MatTableDataSource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  constructor(private adoxioLegalEntityDataService: LegalEntityDataService,
    private accountDataService: AccountDataService,
    private router: Router) { }

  ngOnInit() {
    this.getBusinessProfileData();
  }

  /**
   *
   * */
  getBusinessProfileData() {
    this.busy = this.adoxioLegalEntityDataService.getBusinessProfileSummary().subscribe(
      data => {
        // console.log("getBusinessProfileSummary():", data);
        if (data) {
          // Change Business Releationship label when
          data.forEach((entry) => {
            const profileSummary = new ProfileSummary();
            profileSummary.legalEntityId = entry.id;
            profileSummary.accountId = entry.accountId;
            profileSummary.shareholderAccountId = entry.shareholderAccountId;
            profileSummary.name = entry.name;
            profileSummary.profileComplete = '...';
            if (entry.shareholderAccountId) {
              profileSummary.businessRelationship = 'Shareholder';
            } else {
              profileSummary.businessRelationship = 'Applicant';
              this.getIsCompleteStatus(entry.accountId);
            }
            this.profileSummaryList.push(profileSummary);
          });
          // console.log("this.profileSummaryList:", this.profileSummaryList);
        }
        // sort the array
        this.profileSummaryList = this.sortbyProperty(this.profileSummaryList, 'legalentitytype');
        // console.log("profileSummaryList sorted:", this.profileSummaryList);
        // set table data source
        this.dataSource.data = this.profileSummaryList;
        // set
        setTimeout(() => {
          this.dataSource.paginator = this.paginator;
          this.dataSource.sort = this.sort;
        }, 0);
      },
      err => {
        console.error('Error', err);
      });

  }

  getIsCompleteStatus(accountId: string) {
    this.accountDataService.getBusinessProfile(accountId)
      .subscribe(response => {
        const data = response;
        data.forEach(element => {
          const d = this.profileSummaryList.filter(e => e.legalEntityId === element.legalEntityId)[0];
          if (d) {
            d.profileComplete = element.isComplete === true ? 'Yes' : 'No';
          }
        });
      });
  }

  /**
   * Sort Array by property name
   * @param array
   * @param property
   */
  sortbyProperty(array: any[], property: string) {
    const res = array.sort((leftSide, rightSide): number => {
      if (leftSide[property] < rightSide[property]) {
        return -1;
      }
      if (leftSide[property] > rightSide[property]) {
        return 1;
      }
      return 0;
    });
    return res;
  }
}
