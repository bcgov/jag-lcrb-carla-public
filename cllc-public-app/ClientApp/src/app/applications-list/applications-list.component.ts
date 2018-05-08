import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from "@angular/router";
import { AdoxioApplication } from "../models/adoxio-application.model";
import { AdoxioApplicationDataService } from "../services/adoxio-application-data.service";
import { MatPaginator, MatTableDataSource } from '@angular/material';

@Component({
  selector: 'app-applications-list',
  templateUrl: './applications-list.component.html',
  styleUrls: ['./applications-list.component.css']
})
export class ApplicationsListComponent {
  adoxioApplications: AdoxioApplication[];
  displayedColumns = ['name', 'applyingPerson', 'jobNumber', 'licenseType'];
  dataSource = new MatTableDataSource<AdoxioApplication>(this.adoxioApplications);
  @ViewChild(MatPaginator) paginator: MatPaginator;

  constructor(private adoxioApplicationDataService: AdoxioApplicationDataService, private route: ActivatedRoute) { }

  ngOnInit(): void {
    this.adoxioApplicationDataService.getAdoxioApplications()
      .then((data) => {
        this.adoxioApplications = data;
      });
    this.dataSource.paginator = this.paginator;
  }

}
