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
  
  dataLoaded: boolean;
  loadCount: number;
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
    this.loadCount = 0;
    this.busy = this.statsDataService.getStats("zLicences Issued (Reporting)").subscribe((stats: Stat[]) => {
      stats.forEach((stat: Stat | any) => {
        

        var current = this.summaryData[stat.commregion];
        if (!current) {
          current = {};
        }
        var currentValue = current.licencesIssued;

        if (!currentValue) {
          currentValue = 0;
        }
        currentValue++;
        current.licencesIssued = currentValue;
        this.summaryData[stat.commregion] = current;

      });
      this.loadCount++;

      this.statsDataService.getStats("zApplications Approved with Conditions (Reporting)").subscribe((stats: Stat[]) => {
        stats.forEach((stat: Stat | any) => {
          

          var current = this.summaryData[stat.commregion];
          if (!current) {
            current = {};
          }
          var currentValue = current.approvedWithConditions;

          if (!currentValue) {
            currentValue = 0;
          }
          currentValue++;
          current.approvedWithConditions = currentValue;
          this.summaryData[stat.commregion] = current;
        });
        this.loadCount++;

        this.statsDataService.getStats("zApplications Referred to Local Government or Indigenous Nation (Reporting)").subscribe((stats: Stat[]) => {
          stats.forEach((stat: Stat | any) => {
            

            var current = this.summaryData[stat.commregion];
            if (!current) {
              current = {};
            }
            var currentValue = current.referred;

            if (!currentValue) {
              currentValue = 0;
            }
            currentValue++;
            current.referred = currentValue;
            this.summaryData[stat.commregion] = current;
          });


          this.statsDataService.getStats("zPaid, But Incomplete Applications (Reporting)").subscribe((stats: Stat[]) => {
            stats.forEach((stat: Stat | any) => {
             

              var current = this.summaryData[stat.commregion];
              if (!current) {
                current = {};
              }
              var currentValue = current.incomplete;

              if (!currentValue) {
                currentValue = 0;
              }
              currentValue++;
              current.incomplete = currentValue;
              this.summaryData[stat.commregion] = current;
            });            

            this.statsDataService.getStats("zApplications Where Fee Has Been Paid (Reporting)").subscribe((stats: Stat[]) => {
              stats.forEach((stat: Stat | any) => {                

                var current = this.summaryData[stat.commregion];
                if (!current) {
                  current = {};
                }
                var currentValue = current.paid;

                if (!currentValue) {
                  currentValue = 0;
                }
                currentValue++;
                current.paid = currentValue;
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
          });

        });

      });

        
    }); 

  
  }
}

