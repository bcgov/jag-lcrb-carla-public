import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { SepApplication } from '@models/sep-application.model';
import { IndexedDBService } from '@services/indexed-db.service';
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

  saveToAPI() {
    this.db.getSepApplication(this.localId)
      .then((appData) => {
        if (appData.id) { // do an update ( the record exists in dynamics)
          this.sepDataService.updateSepApplication({...appData, invoiceTrigger: 1 }, appData.id)
            .subscribe(result => {
              if (result.localId) {
                this.db.applications.update(result.localId, result);
                this.localId = result.localId;
              }
            });
        } else {
          this.sepDataService.createSepApplication({...appData, invoiceTrigger: 1 })
            .subscribe(result => {
              if (result.localId) {
                this.db.applications.update(result.localId, result);
                this.localId = result.localId;
              }
            });
        }
      });
  }

  save() {
    this.saveToDB();
    this.saveToAPI();
    this.saveComplete.emit(true);
  }

}
