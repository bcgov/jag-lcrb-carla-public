import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { PoliceTableElement } from '@components/police-representative/police-table-element';
import { SepApplicationSummary } from '@models/sep-application-summary.model';
import { SpecialEventsDataService } from '@services/special-events-data.service';

@Component({
  selector: 'app-submitted-applications',
  templateUrl: './submitted-applications.component.html',
  styleUrls: ['./submitted-applications.component.scss']
})
export class SubmittedApplicationsComponent implements OnInit {
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  dataSource = new MatTableDataSource<PoliceTableElement>();
  // angular material table columns to display
  columnsToDisplay = [
    'dateSubmitted', 'eventName', 'eventStartDate',
    'eventStatusLabel', 'maximumNumberOfGuests', 'actions'
  ];

  constructor(private sepDataService: SpecialEventsDataService) { }

  ngOnInit(): void {
    this.sepDataService.getSubmittedApplications()
      .subscribe(data => this.dataSource.data = data);
  }

  payNow(){
    
  }

}
