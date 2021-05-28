import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { SepApplication } from '@models/sep-application.model';
import { SpecialEventsDataService } from '@services/special-events-data.service';

@Component({
  selector: 'app-police-summary',
  templateUrl: './police-summary.component.html',
  styleUrls: ['./police-summary.component.scss']
})
export class PoliceSummaryComponent implements OnInit {
  @Input() account: any; 
  @Output() saveComplete = new EventEmitter<boolean>();

  _appID: string;
  application: SepApplication;

  @Input() set specialEventId(value: string) {
    this._appID = value;
    //get the special event application
    this.specialEventsDataService.getSpecialEvent(value)
      .subscribe(application => this.application = application);
  };

  get specialEventId() {
    return this._appID;
  }

  constructor(private specialEventsDataService: SpecialEventsDataService) { }

  ngOnInit(): void {
  }

}