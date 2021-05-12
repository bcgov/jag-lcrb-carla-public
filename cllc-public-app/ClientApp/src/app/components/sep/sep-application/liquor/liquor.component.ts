import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { SepApplication } from '@models/sep-application.model';
import { IndexDBService } from '@services/index-db.service';
import { SpecialEventsDataService } from '@services/special-events-data.service';

@Component({
  selector: 'app-liquor',
  templateUrl: './liquor.component.html',
  styleUrls: ['./liquor.component.scss']
})
export class LiquorComponent implements OnInit {
  selectedIndex = 0;
  value: any = {};
  @Output()
  saveComplete = new EventEmitter<boolean>();

  _appID: number;
  sepApplication: SepApplication;

  @Input()
  set applicationId(value: number) {
    this._appID = value;
    //get the last saved application
    this.db.getSepApplication(value)
      .then(app => {
        this.sepApplication = app;
      });
  };

  get applicationId() {
    return this._appID;
  }

  constructor(private db: IndexDBService, private sepDataService: SpecialEventsDataService) { }

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

    if (data.id) {
      this.db.applications.update(data.id, data);
      this.applicationId = data.id;
    } else {
      console.error("The id should already exist at this point.")
    }
  }

  saveToAPI() {
    this.db.getSepApplication(this.applicationId)
      .then((appData) => {
        if (appData.specialEventId) { // do an update ( the record exists in dynamics)
          this.sepDataService.updateSepApplication({...appData, invoiceTrigger: 1 }, appData.specialEventId)
            .subscribe(result => {
              if (result.id) {
                this.db.applications.update(result.id, result);
                this.applicationId = result.id;
              }
            });
        } else {
          this.sepDataService.createSepApplication({...appData, invoiceTrigger: 1 })
            .subscribe(result => {
              if (result.id) {
                this.db.applications.update(result.id, result);
                this.applicationId = result.id;
              }
            });
        }
      });
  }

  save() {
    this.saveToDB();
    this.saveToAPI();
  }

}
