import { Component, OnInit, Input } from '@angular/core';
import { AdoxioLegalEntity } from '../models/adoxio-legalentities.model';
import { AdoxioLegalEntityDataService } from "../services/adoxio-legal-entity-data.service";
import { MatPaginator, MatTableDataSource, MatSort, MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material';

@Component({
  selector: 'app-security-assessments',
  templateUrl: './security-assessments.component.html',
  styleUrls: ['./security-assessments.component.scss']
})
export class SecurityAssessmentsComponent implements OnInit {

  @Input() accountId: string;

  adoxioLegalEntityList: AdoxioLegalEntity[] = [];
  dataSource = new MatTableDataSource<AdoxioLegalEntity>();
  public dataLoaded;
  displayedColumns = ['firstname', 'lastname', 'email', 'position', 'emailsent'];

  constructor(private legalEntityDataservice: AdoxioLegalEntityDataService) { }

  ngOnInit() {
    this.getDirectorsAndOfficersAndShareholders();
  }

  getDirectorsAndOfficersAndShareholders() {
    this.legalEntityDataservice.getLegalEntitiesbyPosition("director-officer-shareholder")
      .then((data) => {
        //console.log("getLegalEntitiesbyPosition("director-officer-shareholder"): ", data);
        this.dataSource.data = data;
        this.dataLoaded = true;
      });
  }

}
