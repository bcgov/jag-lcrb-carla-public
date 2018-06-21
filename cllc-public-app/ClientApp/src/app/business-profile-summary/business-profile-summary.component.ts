import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator, MatTableDataSource, MatSort } from '@angular/material';
import { AdoxioLegalEntityDataService } from '../services/adoxio-legal-entity-data.service';
import { LicenseApplicationSummary } from '../models/license-application-summary.model';
import { Subscription } from 'rxjs';

export class ProfileSummary {
  name: string;
  legalentitytype: string;
  profileComplete: boolean;
}

@Component({
  selector: 'app-business-profile-summary',
  templateUrl: './business-profile-summary.component.html',
  styleUrls: ['./business-profile-summary.component.scss']
})
export class BusinessProfileSummaryComponent implements OnInit {

  public dataLoaded;

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

  constructor(private adoxioLegalEntityDataService: AdoxioLegalEntityDataService) { }

  ngOnInit() {
    this.getBusinessProfileData();
  }

  getBusinessProfileData() {
    this.busy = this.adoxioLegalEntityDataService.getBusinessProfileSummary().subscribe(
      res => {
        let data = res.json();
        //console.log("getBusinessProfileSummary():", data);
        if (data) {
          //Change Business Releationship label when 
          data.forEach((entry) => {
            let profileSummary = new ProfileSummary();
            profileSummary.name = entry.name;
            profileSummary.profileComplete = false;
            if (entry.legalentitytype == 0) {
              profileSummary.legalentitytype = "Applicant"
            } else {
              profileSummary.legalentitytype = entry.legalentitytype;
            }
            this.profileSummaryList.push(profileSummary);
          });
          //console.log("this.profileSummaryList:", this.profileSummaryList);
        }
        this.dataSource.data = this.profileSummaryList;
        this.dataLoaded = true;
        setTimeout(() => {
          this.dataSource.paginator = this.paginator;
          this.dataSource.sort = this.sort;
        }, 0);
      },
      err => {
        console.error("Error", err);
      });

  }

}
