import { Component, OnInit, ViewChild } from '@angular/core';
//import { ActivatedRoute } from '@angular/router';
import { MatPaginator, MatTableDataSource, MatSort } from '@angular/material';
import { AdoxioLegalEntityDataService } from '../services/adoxio-legal-entity-data.service';
import { LicenseApplicationSummary } from '../models/license-application-summary.model';
//import { AdoxioLicense } from '../models/adoxio-license.model';

@Component({
  selector: 'app-business-profile-summary',
  templateUrl: './business-profile-summary.component.html',
  styleUrls: ['./business-profile-summary.component.scss']
})
export class BusinessProfileSummaryComponent implements OnInit {

  public dataLoaded;

  displayedColumns = ['organization', 'businessRelationship', 'profileComplete'];
  dataSource = new MatTableDataSource<any>();

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
    //this.dataSource.data = this.getBusinessProfileData();
    //console.log("this.dataSource.data:", this.dataSource.data);
    //this.dataLoaded = true;
    //setTimeout(() => {
    //  this.dataSource.paginator = this.paginator;
    //  this.dataSource.sort = this.sort;
    //});

  }

  getBusinessProfileData() : any {
    //let summary1 = { "organization": "ACME Inc", "businessRelationship": "Applicant Company", "profileComplete": "No" };
    //let summary2 = { "organization": "LJHR Inc", "businessRelationship": "Shareholder Company", "profileComplete": "No" };
    //let summary: any[] = [];
    //summary.push(summary1);
    //summary.push(summary2);
    //console.log("summary: ", summary);
    //return summary;

    this.adoxioLegalEntityDataService.getBusinessProfileSummary().subscribe(
      res => {
        let data: any = res.json();
        console.log("getBusinessProfileSummary():", data);
        this.dataSource.data = data;
        this.dataLoaded = true;
        setTimeout(() => {
          this.dataSource.paginator = this.paginator;
          this.dataSource.sort = this.sort;
        });
      },
      err => {
        console.error("Error", err);
      });

  }

}
