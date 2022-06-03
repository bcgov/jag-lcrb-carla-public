import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { merge, of } from 'rxjs';
import { startWith, switchMap, map, catchError } from 'rxjs/operators';
import { faPencilAlt } from "@fortawesome/free-solid-svg-icons";
import { ApplicationDataService } from '@services/application-data.service';
import { Application } from '@models/application.model';
@Component({
  selector: 'app-for-zoning-applications',
  templateUrl: './for-zoning-applications.component.html',
  styleUrls: ['./for-zoning-applications.component.scss']
})
export class ForZoningApplicationsComponent implements OnInit, AfterViewInit  {
  displayedColumns: string[] = ['number', 'application', 'applyingBusiness', 'establishmentAddress', 'action'];

  faPencilAlt = faPencilAlt;
  resultsLength = 0;
  isLoadingResults = true;
  isRateLimitReached = false;
  dataLoaded = false; // this is set to true when all page data is loaded

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  data: TableElement[]; 

  constructor(private applicationDataService: ApplicationDataService) {}

  ngOnInit(): void {   
  }
  ngAfterViewInit() {

    merge(this.paginator.page)
      .pipe(
        startWith({}),
        switchMap(() => {
          this.isLoadingResults = true;
          return this.applicationDataService.getLGApprovalApplicationsForZoning(this.paginator.pageIndex, this.paginator.pageSize);
        }),
        map(result => {
          // Flip flag to show that loading has finished.
          this.isLoadingResults = false;
          this.dataLoaded = true;
          this.isRateLimitReached = false;
          //this.resultsLength = data.total_count;
          this.resultsLength = result.count;

          return result.value;          
        }),
        catchError(() => {
          this.isLoadingResults = false;
          this.dataLoaded = true;
          this.isRateLimitReached = true;
          return of([] as Application[]);
        })
    ).subscribe(data => this.data = data.map((el, i) => {
        return {
          ...el,
          index: 1 + i + this.paginator.pageIndex*this.paginator.pageSize
        };
      }));
  }
}

interface TableElement extends Application {
  index: number;
}
