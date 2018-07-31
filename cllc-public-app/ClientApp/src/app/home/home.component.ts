import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { ClientConfigDataService } from '../services/client-config.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  window = window;
  isLiteVersion: boolean;

  constructor(private titleService: Title, private clientConfigDataService: ClientConfigDataService) {}

  ngOnInit() {
    this.titleService.setTitle('Home - Liquor and Cannabis Regulation BranchB');
    this.clientConfigDataService.getConfig().subscribe(data => {
      this.isLiteVersion = data.isLiteVersion;
    });
  }

}
