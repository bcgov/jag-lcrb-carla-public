import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-stats-viewer',
  templateUrl: './stats-viewer.component.html',
  styleUrls: ['./stats-viewer.component.scss']
})
export class StatsViewerComponent implements OnInit {
  public statsData: any;
  dataLoaded: boolean;
  constructor() { }

  ngOnInit() {
  }

}
