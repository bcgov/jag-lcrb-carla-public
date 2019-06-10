import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AdoxioApplication } from '../models/adoxio-application.model';
import { AdoxioApplicationDataService } from '../services/adoxio-application-data.service';
import { MatPaginator, MatTableDataSource, MatSort} from '@angular/material';

@Component({
  selector: 'app-applications-list',
  templateUrl: './applications-list.component.html',
  styleUrls: ['./applications-list.component.css']
})
export class ApplicationsListComponent implements OnInit {
  // adoxioApplications: AdoxioApplication[];

  displayedColumns = ['name', 'applyingPerson', 'jobNumber', 'licenseType'];
  dataSource = new MatTableDataSource<AdoxioApplication>();

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  applyFilter(filterValue: string) {
    filterValue = filterValue.trim(); // Remove whitespace
    filterValue = filterValue.toLowerCase(); // MatTableDataSource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  constructor(private adoxioApplicationDataService: AdoxioApplicationDataService, private route: ActivatedRoute) {
  }

  ngOnInit() {
    this.adoxioApplicationDataService.getAdoxioApplications()
      .subscribe((data: AdoxioApplication[]) => {
        this.dataSource.data = data;
      });
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

}
