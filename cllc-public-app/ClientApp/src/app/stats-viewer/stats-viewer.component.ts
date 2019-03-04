import { Component, OnInit } from '@angular/core';
import { StatsDataService } from '@app/services/stats-data.service';
import { Subscription } from 'rxjs';
import { Stat } from '../models/stat.model';

@Component({
  selector: 'app-stats-viewer',
  templateUrl: './stats-viewer.component.html',
  styleUrls: ['./stats-viewer.component.scss']
})
export class StatsViewerComponent implements OnInit {
  public statsData: any[] = [];
  dataLoaded: boolean;

  busy: Subscription;
  constructor(
    private statsDataService: StatsDataService
  ) { }

  ngOnInit() {
    this.busy = this.statsDataService.getStats().subscribe((stats: Stat[]) => {
      stats.forEach((stat: Stat | any) => {        
        this.statsData.push(stat);        
      });
    });
  }



}
