import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { ClientConfigDataService } from '../services/client-config.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  window = window;
  isLiteVersion: boolean;
  isDataLoaded: boolean = false;
  busy: Subscription;

  constructor(private titleService: Title, private clientConfigDataService: ClientConfigDataService) {}

  ngOnInit() {
    this.titleService.setTitle('Home - Liquor and Cannabis Regulation Branch');
    this.busy = this.clientConfigDataService.getConfig().subscribe(data => {
      this.isLiteVersion = data.isLiteVersion;
      this.isDataLoaded = true;
    });
  }

}
