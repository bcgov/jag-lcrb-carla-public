import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { SepApplication } from '@models/sep-application.model';
import { IndexedDBService } from '@services/indexed-db.service';

@Component({
  selector: 'app-summary',
  templateUrl: './summary.component.html',
  styleUrls: ['./summary.component.scss']
})
export class SummaryComponent implements OnInit {
  @Input() account: Account;
  @Output() saveComplete = new EventEmitter<boolean>();

  _appID: number;
  application: SepApplication;

  @Input() set localId(value: number) {
    this._appID = value;
    //get the last saved application
    this.db.getSepApplication(value)
      .then(app => {
        this.application = app;
      });
  };

  get localId() {
    return this._appID;
  }

  constructor(private db: IndexedDBService) { }

  ngOnInit(): void {
  }

}
