import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { SepApplication } from '@models/sep-application.model';
import { IndexedDBService } from '@services/indexed-db.service';
import { SpecialEventsDataService } from '@services/special-events-data.service';
import { environment } from 'environments/environment';
import { env } from 'process';

@Component({
  selector: 'app-liquor',
  templateUrl: './liquor.component.html',
  styleUrls: ['./liquor.component.scss']
})
export class LiquorComponent implements OnInit {
  selectedIndex = 0;
  value: any = {};
  @Output()
  saveComplete = new EventEmitter<SepApplication>();

  _appID: number;
  sepApplication: SepApplication;

  @Input()
  set localId(value: number) {
    this._appID = value;
    //get the last saved application
    this.db.getSepApplication(value)
      .then(app => {
        this.sepApplication = app;
      });
  };

  get localId() {
    return this._appID;
  }

  constructor(private db: IndexedDBService, private sepDataService: SpecialEventsDataService) { }

  updateValue(value) {
    this.value = { ...this.value, ...value };
  }

  ngOnInit(): void {
  }

  saveToDB() {
    const data = {
      ...this.sepApplication,
      lastUpdated: new Date(),
      status: 'unsubmitted',
      stepsCompleted: (steps => {
        const step = 'event';
        if (steps.indexOf(step) === -1) {
          steps.push(step);
        }
        return steps;
      })(this?.sepApplication?.stepsCompleted || []),
      ...this.value,
    } as SepApplication;

    if (data.localId) {
      this.db.applications.update(data.localId, data);
      this.localId = data.localId;
    } else {
      console.error("The id should already exist at this point.")
    }
  }

  save() {
    const data = {
      ...this.sepApplication,
      lastUpdated: new Date(),
      status: 'unsubmitted',
      stepsCompleted: (steps => {
        const step = 'liquor';
        if (steps.indexOf(step) === -1) {
          steps.push(step);
        }
        return steps;
      })(this?.sepApplication?.stepsCompleted || []),
      ...this.value,
    } as SepApplication;
    this.saveComplete.emit(data);
  }

}
