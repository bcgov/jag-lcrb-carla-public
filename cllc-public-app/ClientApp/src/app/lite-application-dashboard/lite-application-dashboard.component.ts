import { Component, OnInit, Input } from '@angular/core';
import { AdoxioApplicationDataService } from '../services/adoxio-application-data.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-lite-application-dashboard',
  templateUrl: './lite-application-dashboard.component.html',
  styleUrls: ['./lite-application-dashboard.component.scss']
})
export class LiteApplicationDashboardComponent implements OnInit {

  busy: Subscription;
  @Input() applicationStatus: string;

  constructor(private adoxioApplicationDataService: AdoxioApplicationDataService) { }

  ngOnInit() {
    this.busy = this.adoxioApplicationDataService.getAllCurrentApplications().subscribe(
      res => {
      });
  }

}
