import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator, MatTableDataSource, MatSort } from '@angular/material';
import { AdoxioLegalEntityDataService } from '../services/adoxio-legal-entity-data.service';
import { LicenseApplicationSummary } from '../models/license-application-summary.model';
import { Subscription } from 'rxjs';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { State, AppState } from '../app-state/reducers/app-state';
import * as appStateActions from '../app-state/actions/app-state';

export class ProfileSummary {
  legalEntityId: string;
  accountId: string;
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

  constructor(private adoxioLegalEntityDataService: AdoxioLegalEntityDataService, private router: Router,
  private store: Store<State>) { }

  ngOnInit() {
    this.getBusinessProfileData();
  }

  /**
   *
   * */
  getBusinessProfileData() {
    this.busy = this.adoxioLegalEntityDataService.getBusinessProfileSummary().subscribe(
      res => {
        let data = res.json();
        //console.log("getBusinessProfileSummary():", data);
        if (data) {
          //Change Business Releationship label when 
          data.forEach((entry) => {
            let profileSummary = new ProfileSummary();
            profileSummary.legalEntityId = entry.id;
            profileSummary.accountId  = entry.accountId;
            //profileSummary.accountId = entry.accountId;
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
        // sort the array
        this.profileSummaryList = this.sortbyProperty(this.profileSummaryList, "legalentitytype");
        //console.log("profileSummaryList sorted:", this.profileSummaryList);
        // set table data source
        this.dataSource.data = this.profileSummaryList;
        // set 
        setTimeout(() => {
          this.dataSource.paginator = this.paginator;
          this.dataSource.sort = this.sort;
        }, 0);
      },
      err => {
        console.error("Error", err);
      });

  }

  /**
   * Sort Array by property name
   * @param array
   * @param property
   */
  sortbyProperty(array: any[], property: string) {
    let res = array.sort((leftSide, rightSide): number => {
      if (leftSide[property] < rightSide[property]) return -1;
      if (leftSide[property] > rightSide[property]) return 1;
      return 0;
    });
    return res;
  }

  loadBusinessProfile(element){
    debugger;
    this.store.dispatch(new appStateActions.SetCurrentAccountIdAction(element.accountId));
    this.store.dispatch(new appStateActions.SetCurrentLegalEntityIdAction(element.legalEntityId));
    this.router.navigate(['/business-profile']);
  }

}
