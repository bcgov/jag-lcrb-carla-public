import { Component, OnInit } from "@angular/core";
import { EstablishmentDataService } from "@app/services/establishment-data.service";
import { FormBuilder } from "@angular/forms";
import { Subscription } from "rxjs";
import { takeWhile } from "rxjs/operators";
import { FormBase } from "@shared/form-base";
import { Meta, Title } from "@angular/platform-browser";
import { faSearch } from "@fortawesome/free-solid-svg-icons";

@Component({
  selector: "app-licensee-retail-stores",
  templateUrl: "./licensee-retail-stores.component.html",
  styleUrls: ["./licensee-retail-stores.component.scss"]
})
export class LicenseeRetailStoresComponent extends FormBase implements OnInit {

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

  lrsData: string;
  proposedLrsData: string;
  search: string;
  hasData: boolean;
  rows: any;

  ngOnInit() {
    this.meta.addTag({
      name: "viewport",
      content: "width=device-width, initial-scale=1,  maximum-scale=1.0, user-scalable=no"
    });
    this.titleService.setTitle("Licensee Retail Stores in B.C.");
    this.form = this.fb.group({
      name: [""]
    });
    // get the json from the map service.
    this.resetMap();
  }

  searchMap() {
    this.busy =
      this.establishmentDataService.getLrsSearch(this.search)
      .pipe(takeWhile(() => this.componentActive))
      .subscribe(value => {
        this.lrsData = value;
        this.hasData = true;
      });

    this.establishmentDataService.getProposedLrsSearch(this.search)
      .pipe(takeWhile(() => this.componentActive))
      .subscribe(value => {
        this.proposedLrsData = value;
      });
  }

  resetMap() {
    this.busy =
      this.establishmentDataService.getLrs()
      .pipe(takeWhile(() => this.componentActive))
      .subscribe(value => {
        this.lrsData = value;
      });

    this.establishmentDataService.getProposedLrs()
      .pipe(takeWhile(() => this.componentActive))
      .subscribe(value => {
        this.proposedLrsData = value;
      });
  }
}
