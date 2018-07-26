import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { Subscription } from 'rxjs';
import { MatPaginator, MatTableDataSource, MatSort } from '@angular/material';
import { AdoxioApplicationDataService } from '../services/adoxio-application-data.service';
import { LicenseApplicationSummary } from '../models/license-application-summary.model';

@Component({
  selector: 'app-lite-application-dashboard',
  templateUrl: './lite-application-dashboard.component.html',
  styleUrls: ['./lite-application-dashboard.component.scss']
})
export class LiteApplicationDashboardComponent implements OnInit {

  busy: Subscription;
  @Input() applicationSubmitted: boolean;

  //displayedColumns = ['name', 'establishmentName', 'establishmentAddress', 'status', 'licenseType', 'licenseNumber'];
  displayedColumns = ['name', 'establishmentName', 'status'];
  dataSource = new MatTableDataSource<LicenseApplicationSummary>();
  //@ViewChild(MatPaginator) paginator: MatPaginator;
  //@ViewChild(MatSort) sort: MatSort;

  constructor(private adoxioApplicationDataService: AdoxioApplicationDataService) { }

  ngOnInit() {
    let licenseApplicationSummary: LicenseApplicationSummary[] = [];
    this.busy = this.adoxioApplicationDataService.getAllCurrentApplications().subscribe(
      res => {
        let adoxioApplications = res.json();
        adoxioApplications.forEach((entry) => {
          let licAppSum = new LicenseApplicationSummary();
          licAppSum.id = entry.id;
          licAppSum.name = entry.name;
          licAppSum.establishmentName = entry.establishmentName;
          licAppSum.establishmentAddress = entry.establishmentAddress;
          licAppSum.licenseType = entry.licenseType;
          licAppSum.status = entry.applicationStatus;
          licAppSum.applicationSubmitDate = entry.applicationSubmitDate;
          licenseApplicationSummary.push(licAppSum);
        });
        this.dataSource.data = licenseApplicationSummary;
      });
  }

}
