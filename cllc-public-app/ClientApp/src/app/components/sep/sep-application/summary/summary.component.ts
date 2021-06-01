import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { SepApplication } from '@models/sep-application.model';
import { SepSchedule } from '@models/sep-schedule.model';
import { IndexedDBService } from '@services/indexed-db.service';

@Component({
  selector: 'app-summary',
  templateUrl: './summary.component.html',
  styleUrls: ['./summary.component.scss']
})
export class SummaryComponent implements OnInit {
  @Input() account: any; // TODO: change to Account and fix prod error
  @Output() saveComplete = new EventEmitter<boolean>();

  _appID: number;
  application: SepApplication;

  @Input() set localId(value: number) {
    this._appID = value;
    //get the last saved application
    this.db.getSepApplication(value)
      .then(app => {
        this.application = app;
        this.formatEventDatesForDisplay();
      });
  };



  get localId() {
    return this._appID;
  }

  constructor(private db: IndexedDBService) { }

  ngOnInit(): void {
  }

  formatEventDatesForDisplay(){
    if (this?.application?.eventLocations?.length > 0) {
      this.application.eventLocations.forEach(loc =>{
        if(loc.eventDates?.length > 0){
          const formatterdDates = [];
          loc.eventDates.forEach(ed => {
            ed = Object.assign(new SepSchedule(null), ed);
            formatterdDates.push({ed, ...ed.toEventFormValue()});
          });
          loc.eventDates = formatterdDates;
        }
      })
    }
  }
}
