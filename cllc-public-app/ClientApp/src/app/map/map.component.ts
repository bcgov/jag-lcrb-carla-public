import { Component, OnInit } from '@angular/core';
import { EstablishmentDataService } from '@app/services/establishment-data.service';
import { Subscription } from 'rxjs';

declare var EstablishmentsMap: any;
declare var searchMapOptions: any;

@Component({
  selector: 'app-map',
  templateUrl: './map.component.html',
  styleUrls: ['./map.component.scss']
})


export class MapComponent implements OnInit {
  busy: Subscription;
  constructor(
    private establishmentDataService: EstablishmentDataService
  ) { }
  crsJson: string;
  hasData: boolean;
  rows: any;
  ngOnInit() {
    // get the json from the map service.
    this.establishmentDataService.getEstablishmentsMap()
      .subscribe(value => {
        this.crsJson = value;

        //$('[data-toggle="popover"]').popover({ 'trigger': 'hover' });
        var searchMap = new EstablishmentsMap(searchMapOptions);
        //var CrsJson = $("#Crs-json").attr("data-Crs-search-json") || null;
        //var Crs = JSON.parse(CrsJson);
        searchMap.drawAndFitBounds(this.crsJson);

        this.hasData = true;
        this.rows = JSON.parse(value);
      });
  }



}
