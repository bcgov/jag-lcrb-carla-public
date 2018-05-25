import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MatPaginator, MatTableDataSource, MatSort } from '@angular/material';
import { AdoxioApplicationDataService } from '../services/adoxio-application-data.service';
import { AdoxioLicenseDataService } from '../services/adoxio-license-data.service';
import { LicenseApplicationSummary } from '../models/license-application-summary.model';
import { AdoxioApplication } from '../models/adoxio-application.model';
import { AdoxioLicense } from '../models/adoxio-license.model';


@Component({
  selector: 'app-license-application-summary',
  templateUrl: './license-application-summary.component.html',
  styleUrls: ['./license-application-summary.component.css']
})
export class LicenseApplicationSummaryComponent implements OnInit {

  public dataLoaded;

  displayedColumns = ['establishmentName', 'establishmentAddress', 'status', 'licenseType', 'licenseNumber'];
  dataSource = new MatTableDataSource<LicenseApplicationSummary>();

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // MatTableDataSource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  constructor(private adoxioApplicationDataService: AdoxioApplicationDataService,
    private adoxioLicenseDataService: AdoxioLicenseDataService,
    private route: ActivatedRoute) {
  }

  ngOnInit() {
    let licenseApplicationSummary: LicenseApplicationSummary[] = [];

    Promise.all([
      this.adoxioApplicationDataService.getAdoxioApplications(),
      this.adoxioLicenseDataService.getAdoxioLicenses()
    ]).then(value => {
      let adoxioApplications = value[0];
      let adoxioLicenses = value[1];

      adoxioApplications.forEach((entry) => {
        let licAppSum = new LicenseApplicationSummary();
        licAppSum.establishmentName = entry.establishmentName;
        licAppSum.establishmentAddress = entry.establishmentAddress;
        licAppSum.licenseType = entry.licenseType;
        licAppSum.status = entry.applicationStatus;
        licenseApplicationSummary.push(licAppSum);
      });

      adoxioLicenses.forEach((entry) => {
        let licAppSum = new LicenseApplicationSummary();
        licAppSum.establishmentName = entry.establishmentName;
        licAppSum.establishmentAddress = entry.establishmentAddress;
        licAppSum.licenseType = entry.licenseType;
        licAppSum.status = entry.licenseStatus;
        licAppSum.licenseNumber = entry.licenseNumber;
        licenseApplicationSummary.push(licAppSum);
      });

      this.dataSource.data = licenseApplicationSummary;
      this.dataLoaded = true;
      //console.log(adoxioApplications);
      //console.log(adoxioLicenses);
      //console.log(this.dataSource.data);

      setTimeout(() => {
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
      });
    });
  }

}
