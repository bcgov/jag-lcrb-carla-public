import { HttpClient } from '@angular/common/http';
import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { merge, Observable, of } from 'rxjs';
import { startWith, switchMap, map, catchError } from 'rxjs/operators';
import { faPencilAlt } from "@fortawesome/free-solid-svg-icons";
import { ApplicationDataService } from '@services/application-data.service';
@Component({
  selector: 'app-resolved-applications',
  templateUrl: './resolved-applications.component.html',
  styleUrls: ['./resolved-applications.component.scss']
})
export class ResolvedApplicationsComponent implements OnInit, AfterViewInit  {
  displayedColumns: string[] = ['number', 'application', 'applyingBusiness', 'establishmentAddress', 'action'];
  exampleDatabase: ExampleHttpDatabase | null;
  data: GithubIssue[] = [];
  faPencilAlt = faPencilAlt;
  resultsLength = 0;
  isLoadingResults = true;
  isRateLimitReached = false;

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  constructor(private _httpClient: HttpClient, private applicationDataService: ApplicationDataService) {}

  ngOnInit(): void {
  }
  ngAfterViewInit() {
    this.exampleDatabase = new ExampleHttpDatabase(this._httpClient);

    // If the user changes the sort order, reset back to the first page.
    this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);

    merge(this.sort.sortChange, this.paginator.page)
      .pipe(
        startWith({}),
        switchMap(() => {
          this.isLoadingResults = true;
          return this.applicationDataService.getResolvedLGApplications();

          // return this.exampleDatabase!.getRepoIssues(
          //   this.sort.active, this.sort.direction, this.paginator.pageIndex);
        }),
        map(data => {
          // Flip flag to show that loading has finished.
          this.isLoadingResults = false;
          this.isRateLimitReached = false;
          //this.resultsLength = data.total_count;
          this.resultsLength = data.length;

          return data;
        }),
        catchError(() => {
          this.isLoadingResults = false;
          // Catch if the GitHub API has reached its rate limit. Return empty data.
          this.isRateLimitReached = true;
          return of([]);
        })
      ).subscribe((data: any) => this.data = data);
  }
}

export interface GithubApi {
  items: GithubIssue[];
  total_count: number;
}

export interface GithubIssue {
  created_at: string;
  number: string;
  state: string;
  title: string;
}

/** An example database that the data source uses to retrieve data for the table. */
export class ExampleHttpDatabase {
  constructor(private _httpClient: HttpClient) {}

  getRepoIssues(sort: string, order: string, page: number): Observable<GithubApi> {
    const href = 'https://api.github.com/search/issues';
    const requestUrl =
        `${href}?q=repo:angular/components&sort=${sort}&order=${order}&page=${page + 1}`;

    return this._httpClient.get<GithubApi>(requestUrl);
  }
}