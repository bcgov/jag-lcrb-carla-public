import { AfterViewInit, Component, OnInit, ViewChild} from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { merge, of } from 'rxjs';
import { startWith, switchMap, map, catchError } from 'rxjs/operators';
import { faPencilAlt } from "@fortawesome/free-solid-svg-icons";
import { ApplicationDataService } from '@services/application-data.service';
import { Application } from '@models/application.model';
import { startOfToday, add, differenceInDays } from "date-fns";
import * as EventEmitter from 'events';

@Component({
  selector: 'app-decision-made-but-no-docs-applications',
  templateUrl: './decision-made-but-no-docs-applications.component.html',
  styleUrls: ['./decision-made-but-no-docs-applications.component.scss']
})
export class DecisionMadeButNoDocsApplicationsComponent implements OnInit, AfterViewInit  {
  displayedColumns: string[] = ['number', 'application', 'applyingBusiness', 'address', 'provideResolution','90DayCounter'];

  faPencilAlt = faPencilAlt;
  resultsLength = 0;
  isLoadingResults = true;
  isRateLimitReached = false;  

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
 
  data: TableElement[];

  constructor(private applicationDataService: ApplicationDataService) {}

  ngOnInit(): void {}
  ngAfterViewInit() {
    // If the user changes the sort order, reset back to the first page.
    this.sort.sortChange.subscribe(() => this.paginator.pageIndex = 0);
    merge(this.sort.sortChange, this.paginator.page)
      .pipe(
        startWith({}),
        switchMap(() => {
          this.isLoadingResults = true;
          return this.applicationDataService.getLGApprovalApplicationsDicisionMadeButNoDocs(this.paginator.pageIndex, this.paginator.pageSize);
        }),
        map(result => {
          // Flip flag to show that loading has finished.
          this.isLoadingResults = false;
          this.isRateLimitReached = false;
          this.resultsLength = result.count;
         
          return result.value;
        }),
        catchError(() => {
          this.isLoadingResults = false;        
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
  get90dayCount(submissionDate: Date): number {
    const submission = add(new Date(submissionDate), { days: 90 });
    const count = differenceInDays(startOfToday(), submission);
    return count;
  }
}

interface TableElement extends Application {
  index: number;
}
