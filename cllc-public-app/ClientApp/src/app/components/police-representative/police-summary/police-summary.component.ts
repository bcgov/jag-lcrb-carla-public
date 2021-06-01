import { Component, EventEmitter, Inject, Input, OnInit, Output } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import { SepApplication } from '@models/sep-application.model';
import { SpecialEventsDataService } from '@services/special-events-data.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-police-summary',
  templateUrl: './police-summary.component.html',
  styleUrls: ['./police-summary.component.scss']
})
export class PoliceSummaryComponent implements OnInit {

  busy: Subscription;
  specialEventId: string;
  public application: SepApplication;


  constructor(private specialEventsDataService: SpecialEventsDataService,
    private router: Router,
    private route: ActivatedRoute,
    private snackBar: MatSnackBar,
    ) {
      this.route.paramMap.subscribe(params => {
        this.specialEventId = params.get("specialEventId");
      });
      
  }

  ngOnInit(): void {
    console.log ("INIT");
    console.log (this.specialEventId);
    this.busy =
    this.specialEventsDataService.getSpecialEvent(this.specialEventId)
      .subscribe(application => {
        this.application = application;
        console.log (application);
      });
  }

  approve(): void {
    this.busy = this.specialEventsDataService.policeApproveSepApplication(this.specialEventId)
      .subscribe(() => 
      
      this.snackBar.open("Approved application.",
          "Success",
          { duration: 3500, panelClass: ["green-snackbar"] })
      );
  }

  deny(): void {
    this.busy = this.specialEventsDataService.policeDenySepApplication(this.specialEventId)
      .subscribe(() => this.snackBar.open("Denied application.",
      "Success",
      { duration: 3500, panelClass: ["green-snackbar"] })
  );
  }


}