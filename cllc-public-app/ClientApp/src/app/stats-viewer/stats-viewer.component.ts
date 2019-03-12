import { Component, OnInit, ViewChild } from '@angular/core';
import { StatsDataService } from '@app/services/stats-data.service';
import { Subscription } from 'rxjs';
import { Stat } from '../models/stat.model';
import { BaseChartDirective, Color  } from 'ng2-charts';
import { ChartOptions, ChartType, ChartDataSets } from 'assets/external-deps/Chart.min.js';


@Component({
  selector: 'app-stats-viewer',
  templateUrl: './stats-viewer.component.html',
  styleUrls: ['./stats-viewer.component.scss']
})
export class StatsViewerComponent implements OnInit {
  
  dataLoaded: boolean;
  loadCount: number;
  public chartColors: object = {
    red: 'rgb(255, 99, 132)',
    orange: 'rgb(255, 159, 64)',
    yellow: 'rgb(255, 205, 86)',
    green: 'rgb(45, 192, 45)',
    blue: 'rgb(54, 162, 235)',
    darkblue: 'rgb(24, 42, 75)',
    purple: 'rgb(153, 102, 255)',
    grey: 'rgb(231,238,235)',
    darkgrey: 'rgb(81,88,85)',
    black: 'rgb(20,20,20)',
    white: 'rgb(255,255,255)',
  };

  public bubbleChartColors: Color[] = [
    {
      backgroundColor: [
        'red',
        'green',
        'blue',
        'purple',
        'yellow',
        'brown',
        'magenta',
        'cyan',
        'orange',
        'pink'
      ]
    }
  ];



  public chartType: ChartType  = 'bubble';
  public summaryData: object = {};
  public summaryKeys: string[];
  @ViewChild(BaseChartDirective) chart: BaseChartDirective;

  public totalLabels: string[] = ["Region",
    "Applications Where Fee Has Been Paid",
    "Paid, But Incomplete Applications",
    "Applications Referred to Local Government or Indigenous Nation",
    "Applications Approved with Conditions",
    "Licences Issued"];
  public totalData: number[] = [0, 0, 0, 0, 0];
 
  public bubbleChartOptions: ChartOptions = {
    responsive: true,
    maintainAspectRatio: true,
    scales: {
      xAxes: [
        {
          ticks: {
            min: 0,
            max: 1000
          }
        }
      ],
      yAxes: [
        {
          ticks: {
            min: 0,
            max: 1000
          }
        }
      ]
    }
  };
  public bubbleChartData: ChartDataSets[] = [
    {
      data: [
        { x: 10, y: 10, r: 10 },       
      ],
      label: 'Applications Where Fee Has Been Paid',
      backgroundColor: 'green',
      borderColor: 'blue',
      hoverBackgroundColor: 'red',
      hoverBorderColor: 'orange',
    },
    {
      data: [
        { x: 10, y: 10, r: 10 },
      ],
      label: 'Paid, But Incomplete Applications',
      backgroundColor: 'green',
      borderColor: 'blue',
      hoverBackgroundColor: 'purple',
      hoverBorderColor: 'red',
    },
    {
      data: [
        { x: 10, y: 10, r: 10 },
      ],
      label: 'Applications Referred to Local Government or Indigenous Nation',
      backgroundColor: 'green',
      borderColor: 'blue',
      hoverBackgroundColor: 'purple',
      hoverBorderColor: 'red',
    },
    {
      data: [
        { x: 10, y: 10, r: 10 },
      ],
      label: 'Applications Approved with Conditions',
      backgroundColor: 'green',
      borderColor: 'blue',
      hoverBackgroundColor: 'purple',
      hoverBorderColor: 'red',
    },
    {
      data: [
        { x: 10, y: 10, r: 10 },
      ],
      label: 'Licences Issued',
      backgroundColor: 'green',
      borderColor: 'blue',
      hoverBackgroundColor: 'purple',
      hoverBorderColor: 'red',
    },
  ];

  public bubbleChartLegend: boolean = true;
  busy: Subscription;
  constructor(
    private statsDataService: StatsDataService
  ) {
    
  }

  private rand(max: number) {
    return Math.trunc(Math.random() * max);
  }

  private newPoint(new_x: number, new_y: number, radius: number) {
    const x = new_x;
    const y = new_y;
    const r = radius;
    return { x, y, r };
  }

  private calculateRadius(height: number, maxSize: number, theValue: number)
  {
    return ((theValue / 2) / maxSize ) * height;
  }

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
        this.totalData[4]++;
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

          this.totalData[3]++;
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
            this.totalData[2]++;
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
              this.totalData[1]++;
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
                this.totalData[0]++;
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

              /*
              this.chartColors = this.totalData.map(r => ({
                backgroundColor: this.chartColors[Object.keys(this.chartColors)[Math.floor(Math.random() * Object.keys(this.chartColors).length)]]
              }));
              */
              var maxSize = this.totalData[0] + this.totalData[3] + 10 + this.totalData[4];
              var maxScale = 10 + maxSize + 10;

              this.bubbleChartOptions.scales.xAxes[0].ticks.max = maxScale;
              this.bubbleChartOptions.scales.yAxes[0].ticks.max = maxScale;

              var height = 450;

              var series1Data = [];
              var series1Factor 
              series1Data.push(this.newPoint(this.totalData[0] / 2 + 10, 10 + this.totalData[0] / 2, this.calculateRadius(height, maxSize, this.totalData[0])));

              var series2Data = [];
              series2Data.push(this.newPoint(10 + this.calculateRadius(height, maxSize, this.totalData[0]), 10 + this.totalData[1] / 2, this.calculateRadius(height, maxSize, this.totalData[1])));


              var series3Data = [];
              series3Data.push(this.newPoint(10 + (this.totalData[2] / 2), 10 + (this.totalData[2] / 2), this.calculateRadius(height, maxSize, this.totalData[2])));


              var series4Data = [];
              series4Data.push(this.newPoint(this.totalData[0] + 10 + (this.totalData[3]), 10 + (this.totalData[3] / 2), this.calculateRadius(height, maxSize, this.totalData[3])));

              var series5Data = [];
              series5Data.push(this.newPoint(this.totalData[0] + 10 + (this.totalData[3]) + (this.totalData[4] / 2), (this.totalData[4]), this.calculateRadius(height, maxSize, this.totalData[4])));

              this.bubbleChartData[0].data = series1Data;
              this.bubbleChartData[1].data = series2Data;
              this.bubbleChartData[2].data = series3Data;
              this.bubbleChartData[3].data = series4Data;
              this.bubbleChartData[4].data = series5Data;

            this.dataLoaded = true;
            });
          });

        });

      });

        
    }); 

  
  }
}

