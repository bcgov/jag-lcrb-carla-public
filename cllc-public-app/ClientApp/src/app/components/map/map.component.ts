import { Component, OnInit } from '@angular/core';
import { EstablishmentDataService } from '@app/services/establishment-data.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';
import { takeWhile, filter, catchError, mergeMap } from 'rxjs/operators';
import { FormBase } from '@shared/form-base';
import { Meta, Title } from '@angular/platform-browser';

declare var EstablishmentsMap: any;
declare var searchMapOptions: any;

@Component({
  selector: 'app-map',
  templateUrl: './map.component.html',
  styleUrls: ['./map.component.scss']
})


export class MapComponent extends FormBase implements OnInit {
  busy: Subscription;

  constructor(
    private establishmentDataService: EstablishmentDataService, private fb: FormBuilder, private meta: Meta, private titleService: Title 
  ) { super(); }
  mapData: string;
  search: string;
  hasData: boolean;
  establishmentMap: any;
  rows: any;

  ngOnInit() {
    this.meta.addTag({ name: 'viewport', content: 'width=device-width, initial-scale=1,  maximum-scale=1.0, user-scalable=no' });
    this.titleService.setTitle("Map of Cannabis Retail Stores in B.C.");
    this.form = this.fb.group({
      name: ['']
    });
    // get the json from the map service.
      this.busy =
      this.establishmentDataService.getEstablishmentsMap()
      .pipe(takeWhile(() => this.componentActive))
      .subscribe(value => {
        this.mapData = value;

        //$('[data-toggle="popover"]').popover({ 'trigger': 'hover' });
        this.establishmentMap = new EstablishmentsMap(searchMapOptions);
        //var mapData = $("#Crs-json").attr("data-Crs-search-json") || null;
        //var Crs = JSON.parse(mapData);
        this.establishmentMap.drawAndFitBounds(this.mapData);
        this.hasData = true;        
      });
  }

  searchMap() {
    this.busy =
      this.establishmentDataService.getEstablishmentsMapSearch(this.search)
      .pipe(takeWhile(() => this.componentActive))
      .subscribe(value => {
        this.mapData = value;        
        this.establishmentMap.drawAndFitBounds(this.mapData);
        this.hasData = true;        
        });
  }

  resetMap() {
    this.busy =
      this.establishmentDataService.getEstablishmentsMapSearch("")
        .pipe(takeWhile(() => this.componentActive))
        .subscribe(value => {
          this.mapData = value;
          this.establishmentMap.drawAndFitBounds(this.mapData);
          this.hasData = true;
        });
  }
  }

  

