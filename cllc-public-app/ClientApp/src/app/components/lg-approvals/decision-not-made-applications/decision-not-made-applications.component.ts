import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { merge, of } from 'rxjs';
import { startWith, switchMap, map, catchError } from 'rxjs/operators';
import { faPencilAlt } from "@fortawesome/free-solid-svg-icons";
import { ApplicationDataService } from '@services/application-data.service';
import { Application } from '@models/application.model';
import { Console } from 'console';
@Component({
  selector: 'app-decision-not-made-applications',
  templateUrl: './decision-not-made-applications.component.html',
  styleUrls: ['./decision-not-made-applications.component.scss']
})
export class DecisionNotMadeApplicationsComponent implements OnInit, AfterViewInit  {
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
    
    // If the user changes the sort order, reset back to the first page.
    //this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);

    merge(this.paginator.page)
      .pipe(
        startWith({}),
        switchMap(() => {
          this.isLoadingResults = true;
          this.dataLoaded = false;
          console.log("page index " + this.paginator.pageIndex);
          console.log("page size " + this.paginator.pageSize);
          return this.applicationDataService.getLGApprovalApplicationsDecisionNotMade(this.paginator.pageIndex, this.paginator.pageSize);
        }),
        map(result => {
          // Flip flag to show that loading has finished.
          this.isLoadingResults = false;
          this.dataLoaded = true;
          this.isRateLimitReached = false;        
          this.resultsLength = result.count;
          return result.value;
        }),
        catchError(() => {
          this.isLoadingResults = false;
          this.dataLoaded = true;
          this.isRateLimitReached = true;
          return of([] as Application[]);
        })
      ).subscribe((data) => this.data = data.map((el, i) => {
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
