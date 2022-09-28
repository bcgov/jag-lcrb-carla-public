import { ChangeDetectorRef, Component, Input, OnInit, ViewChild  } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { PoliceTableElement } from '../police-table-element';
import { Subscription } from "rxjs";
import { SpecialEventsDataService } from '@services/special-events-data.service';
import { SelectionModel } from '@angular/cdk/collections';
import { MatSelectChange } from '@angular/material/select';
import { MatSort } from '@angular/material/sort';
import { MatPaginator } from '@angular/material/paginator';
import { Contact } from '@models/contact.model';
import { User } from '@models/user.model';
import { Router } from '@angular/router';
import { merge, of } from 'rxjs';
import { startWith, switchMap, map, catchError } from 'rxjs/operators';
import { SepApplicationSummary } from '../../../models/sep-application-summary.model';

@Component({
  selector: 'app-police-grid-denied',
  templateUrl: './police-grid-denied.component.html',
  styleUrls: ['./police-grid-denied.component.scss']
})
export class PoliceGridDeniedComponent implements OnInit {
  resultsLength = 0;
  isLoadingResults = true;
  isRateLimitReached = false;
  dataLoaded = false; // this is set to true when all page data is loaded

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  busy: Subscription;
  _availableContacts: Contact[];
  _currentUser: User;
  currentValueMap = {};
  currentNameMap = {};
  selectedIndex: any;

  // table state
  dataSource = new MatTableDataSource<PoliceTableElement>();

  // angular material table columns to display
  columnsToDisplay = [
    'select', 'dateSubmitted', 'eventName', 'eventStartDate', 'eventStatusLabel',
    'policeDecisionByLabel', 'maximumNumberOfGuests', 'typeOfEventLabel', 'actions'
  ];

  // table state
  initialSelection = [];
  allowMultiSelect = true;
  selection = new SelectionModel<PoliceTableElement>(this.allowMultiSelect, this.initialSelection);

  @Input()
  set availableContacts(value: Contact[]) {
    this._availableContacts = value;
  }
  get availableContacts() {
    return this._availableContacts;
  }

  @Input()
  set currentUser(value: User) {
    this._currentUser = value;
  };
  get currentUser() {
    return this._currentUser;
  }


  constructor(    private sepDataService: SpecialEventsDataService,
    private cd: ChangeDetectorRef,
    private router: Router) { }


  ngOnInit(): void { }
  ngAfterViewInit() {
    this.dataSource.sort = this.sort;
    // If the user changes the sort order, reset back to the first page.
    this.dataSource.sort.sortChange.subscribe(() => {
      this.paginator.pageIndex = 0;
      this.paginator._changePageSize(this.paginator.pageSize);
    });
    merge(this.paginator.page)
      .pipe(
        startWith({}),
        switchMap(() => {
          this.isLoadingResults = true;
          this.dataLoaded = false;
          return this.sepDataService.getPoliceDeniedSepApplications(this.paginator.pageIndex, this.paginator.pageSize, this.dataSource.sort.active, this.dataSource.sort.direction);
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
          return of([] as SepApplicationSummary[]);
        })
      ).subscribe((data) => this.dataSource.data = data.map((el, i) => {
        return {
          ...el,
          index: 1 + i + this.paginator.pageIndex * this.paginator.pageSize
        };
      }));
  }

  isAssigned(sepData: PoliceTableElement): boolean {
    // TODO: Implement logic to show appropriate button text when application has been assigned
    return sepData.policeDecisionBy != null;
  }

  assign(row: PoliceTableElement) {
    const assignee = this.currentValueMap['assignee_' + row.specialEventId];

    this.busy = this.sepDataService.policeAssignSepApplication(row.specialEventId, assignee)
      .subscribe(data => {
        row.policeDecisionBy = data;
        // ensure the grid refreshes.
        this.cd.detectChanges();      
      });
  }

  batchAssign() {
    // TODO: Call backend endpoint for batch updates/assignments
    //Seems to be unused at this time
    const selected = this.selection.selected;
    console.log(`Call API to batch assign SEP applications:`);
    selected.forEach(x => console.log(`${x.specialEventId}`));
  }

  /** Whether the number of selected elements matches the total number of rows. */
  isAllSelected() {
    const numSelected = this.selection.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  /** Selects all rows if they are not all selected; otherwise clear selection. */
  masterToggle() {
    this.isAllSelected() ?
      this.selection.clear() :
      this.dataSource.data.forEach(row => this.selection.select(row));
  }

  updateValue(event: MatSelectChange) {
    this.currentValueMap[event.source.id] = event.value;

  }

  /**
   *
   * @param applicationId
   * @param establishmentName
   * @param applicationName
   */
  openApplication(id: string) {
    this.router.navigateByUrl(`sep/police/${id}`);
  }

  /*handlePage(e: any) {
    console.log(e);  
    this.getData(e.pageIndex, e.pageSize);
  }*/


}

interface TableElement extends SepApplicationSummary {
  index: number;
}
