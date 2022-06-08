import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { merge, of } from 'rxjs';
import { startWith, switchMap, map, catchError } from 'rxjs/operators';
import { faPencilAlt } from "@fortawesome/free-solid-svg-icons";
import { ApplicationDataService } from '@services/application-data.service';
import { Application } from '@models/application.model';
@Component({
  selector: 'app-resolved-applications',
  templateUrl: './resolved-applications.component.html',
  styleUrls: ['./resolved-applications.component.scss']
})
export class ResolvedApplicationsComponent implements OnInit, AfterViewInit  {
  displayedColumns: string[] = ['number', 'application', 'applyingBusiness', 'establishmentAddress', 'action'];

  faPencilAlt = faPencilAlt;
  resultsLength = 0;
  isLoadingResults = true;
  isRateLimitReached = false;
  dataLoaded = false;

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  data: TableElement[];

  constructor(private applicationDataService: ApplicationDataService) {}

  ngOnInit(): void {
  }
  ngAfterViewInit() {
    this.dataLoaded = false;
    // If the user changes the sort order, reset back to the first page.
    this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);

    merge(this.sort.sortChange, this.paginator.page)
      .pipe(
        startWith({}),
        switchMap(() => {
          this.isLoadingResults = true;
          return this.applicationDataService.getResolvedLGApplications(this.paginator.pageIndex, this.paginator.pageSize);

          // return this.exampleDatabase!.getRepoIssues(
          //   this.sort.active, this.sort.direction, this.paginator.pageIndex);
        }),
        map(result => {
          // Flip flag to show that loading has finished.
          this.isLoadingResults = false;
          this.isRateLimitReached = false;
          this.dataLoaded = true;
          //this.resultsLength = data.total_count;
          this.resultsLength = result.count;

          return result.value;
        }),
        catchError(() => {
          this.isLoadingResults = false;
          this.dataLoaded = true;
          // Catch if the GitHub API has reached its rate limit. Return empty data.
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
