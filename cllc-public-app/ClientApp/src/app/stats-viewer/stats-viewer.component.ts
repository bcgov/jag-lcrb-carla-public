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

  public summaryData: object = {};
  public summaryKeys: string[];

  busy: Subscription;
  constructor(
    private statsDataService: StatsDataService
  ) { }

  /*
   * saved Queries are:
   *  zLicences Issued (Reporting)
   *  zApplications Approved with Conditions (Reporting)
   *  zApplications Referred to Local Government or Indigenous Nation (Reporting)
   *  zPaid, But Incomplete Applications (Reporting)
   *  zApplications Where Fee Has Been Paid (Reporting)
   */ 

  ngOnInit() {
    this.busy = this.statsDataService.getStats("zLicences Issued (Reporting)").subscribe((stats: Stat[]) => {
      stats.forEach((stat: Stat | any) => {        
        this.statsData.push(stat);

        var current = this.summaryData[stat.commregion];
        if (!current) {
          current = 0;
        }
        current++;
        this.summaryData[stat.commregion] = current;

      });


      this.summaryKeys = Object.keys(this.summaryData).sort((n1, n2) => {
          if (n1 > n2) {
            return 1;
          }

          if (n1 < n2) {
            return -1;
          }

          return 0;
        });
      this.dataLoaded = true;
    });
  }


}
