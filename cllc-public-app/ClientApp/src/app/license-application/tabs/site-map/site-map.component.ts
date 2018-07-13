import { Component, OnInit, Input } from '@angular/core';
import { UserDataService } from '../../../services/user-data.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-site-map',
  templateUrl: './site-map.component.html',
  styleUrls: ['./site-map.component.scss']
})
export class SiteMapComponent implements OnInit {
  applicationId: string;

  constructor(private userDataService: UserDataService, private route: ActivatedRoute) {
    this.applicationId = route.parent.snapshot.params.applicationId;
  }

  ngOnInit() {
  }

}
