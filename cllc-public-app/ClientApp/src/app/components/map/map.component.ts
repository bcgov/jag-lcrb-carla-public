import { Component, OnInit, AfterViewInit, ViewChild } from "@angular/core";
import { EstablishmentDataService } from "@app/services/establishment-data.service";
import { FormBuilder } from "@angular/forms";
import { Subscription } from "rxjs";
import { takeWhile } from "rxjs/operators";
import { FormBase } from "@shared/form-base";
import { Meta, Title } from "@angular/platform-browser";
import { faSearch } from "@fortawesome/free-solid-svg-icons";
import { MatPaginator, PageEvent } from "@angular/material/paginator";

declare var EstablishmentsMap: any;
declare var searchMapOptions: any;

@Component({
  selector: "app-map",
  templateUrl: "./map.component.html",
  styleUrls: ["./map.component.scss"]
})
export class MapComponent extends FormBase implements OnInit, AfterViewInit {
  faSearch = faSearch;
  busy: Subscription;

  constructor(
    private establishmentDataService: EstablishmentDataService,
    private fb: FormBuilder,
    private meta: Meta,
    private titleService: Title
  ) {
    super();
  }

  mapData: any[];
  search: string;
  hasData: boolean;
  establishmentMap: any;
  rows: any;
  private mapInitialized = false;

  // Pagination
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  pageSize = 25;
  pageIndex = 0;

  get pagedData(): any[] {
    if (!this.mapData) { return []; }
    const start = this.pageIndex * this.pageSize;
    return this.mapData.slice(start, start + this.pageSize);
  }

  handlePageEvent(event: PageEvent) {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
  }

  ngOnInit() {
    this.meta.addTag({
      name: "viewport",
      content: "width=device-width, initial-scale=1,  maximum-scale=1.0, user-scalable=no"
    });
    this.titleService.setTitle("Map of Cannabis Retail Stores in B.C.");
    this.form = this.fb.group({
      name: [""]
    });
  }

  ngAfterViewInit() {
    // Load data after the view (and #search-map div) is rendered.
    this.busy =
      this.establishmentDataService.getEstablishmentsMap()
      .pipe(takeWhile(() => this.componentActive))
      .subscribe(value => {
        this.mapData = value;
        this.initMap();
        this.hasData = true;
      });
  }

  searchMap() {
    this.busy =
      this.establishmentDataService.getEstablishmentsMapSearch(this.search)
      .pipe(takeWhile(() => this.componentActive))
      .subscribe(value => {
        this.mapData = value;
        this.pageIndex = 0;
        this.initMap();
        this.hasData = true;
      });
  }

  resetMap() {
    this.busy =
      this.establishmentDataService.getEstablishmentsMapSearch("")
      .pipe(takeWhile(() => this.componentActive))
      .subscribe(value => {
        this.mapData = value;
        this.pageIndex = 0;
        this.initMap();
        this.hasData = true;
      });
  }

  private initMap() {
    try {
      if (!this.mapInitialized) {
        this.establishmentMap = new EstablishmentsMap(searchMapOptions);
        this.mapInitialized = true;
      }
      this.establishmentMap.drawAndFitBounds(this.mapData);
    } catch (e) {
      console.error("Failed to initialize map:", e);
    }
  }
}
