import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { SepApplication } from '@models/sep-application.model';
import { SepSchedule } from '@models/sep-schedule.model';
import { IndexedDBService } from '@services/indexed-db.service';
import { SpecialEventsDataService } from '@services/special-events-data.service';

@Component({
  selector: 'app-summary',
  templateUrl: './summary.component.html',
  styleUrls: ['./summary.component.scss']
})
export class SummaryComponent implements OnInit {
  @Input() account: any; // TODO: change to Account and fix prod error
  @Output() saveComplete = new EventEmitter<boolean>();
  mode: 'readonlySummary' | 'pendingReview' | 'payNow' = 'readonlySummary';
  _appID: number;
  application: SepApplication;

  @Input() set localId(value: number) {
    this._appID = value;
    // get the last saved application
    this.db.getSepApplication(value)
      .then(app => {
        this.application = app;
        this.formatEventDatesForDisplay();
      });
  }

  get localId() {
    return this._appID;
  }

  constructor(private db: IndexedDBService,
    private sepDataService: SpecialEventsDataService) { }

  ngOnInit(): void {
  }

  formatEventDatesForDisplay() {
    if (this?.application?.eventLocations?.length > 0) {
      this.application.eventLocations.forEach(loc => {
        if (loc.eventDates?.length > 0) {
          const formatterdDates = [];
          loc.eventDates.forEach(ed => {
            ed = Object.assign(new SepSchedule(null), ed);
            formatterdDates.push({ ed, ...ed.toEventFormValue() });
          });
          loc.eventDates = formatterdDates;
        }
      });
    }
  }

  async submitApplication(): Promise<void> {
    const appData = await this.db.getSepApplication(this.localId);
    if (appData.id) { // do an update ( the record exists in dynamics)
      const result = await this.sepDataService.updateSepApplication({ ...appData, eventStatus: 'Submitted' } as SepApplication, appData.id)
        .toPromise();
      if (result.eventStatus === 'Approved') {
        this.mode = 'payNow';
      } else if (result.eventStatus === 'Pending Review') {
        this.mode  = 'pendingReview';
      }
      if (result.localId) {
        await this.db.applications.update(result.localId, result);
      }
    }
  }
}
