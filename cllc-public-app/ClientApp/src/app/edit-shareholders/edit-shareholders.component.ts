import { Component, OnInit } from '@angular/core';
import { MatPaginator, MatTableDataSource, MatSort } from '@angular/material';
import { Shareholder } from '../models/shareholder.model';
import { FormBuilder, FormGroup, FormControl, Validators, NgForm } from '@angular/forms';
import { AdoxioLegalEntityDataService } from "../services/adoxio-legal-entity-data.service";

@Component({
  selector: 'app-edit-shareholders',
  templateUrl: './edit-shareholders.component.html',
  styleUrls: ['./edit-shareholders.component.scss']
})

export class EditShareholdersComponent implements OnInit {

  shareholderForm: FormGroup;
  shareholderList: Shareholder[] = [];
  dataSource = new MatTableDataSource<Shareholder>();
  public dataLoaded;
  displayedColumns = ['shareholderType', 'name', 'email', 'numberOfNonVotingShares', 'numberOfVotingShares', 'dateIssued'];

  constructor(private frmbuilder: FormBuilder, private legalEntityDataservice: AdoxioLegalEntityDataService) {
    this.shareholderForm = frmbuilder.group({
      shareholderType: new FormControl(),
      firstName: new FormControl(),
      lastName: new FormControl(),
      email: new FormControl(),
      numberOfNonVotingShares: new FormControl(),
      numberOfVotingShares: new FormControl(),
      dateIssued: new FormControl()
    });
  }

  ngOnInit() {

    let shareholder: Shareholder;
    shareholder = this.getShareholderTest();
    this.shareholderList.push(shareholder);

    this.dataSource.data = this.shareholderList;
    this.dataLoaded = true;
  }

  addShareholder(shareholderForm: NgForm) {
    console.log(shareholderForm.controls);
    let shareholderModel = this.toShareholderModel(shareholderForm);
    console.log(shareholderModel);
    this.legalEntityDataservice.post(shareholderModel);
    this.addShareholdertoTable(shareholderForm);
    this.dataSource.data = this.shareholderList;
  }

  toShareholderModel(shareholderForm: NgForm): Shareholder {
    let shareholder: Shareholder = new Shareholder();
    if (shareholder.isindividual) {
      shareholder.shareholderType = 'Person';
    } else {
      shareholder.shareholderType = 'Organization';
    }
    shareholder.firstname = shareholderForm.controls.firstName.value;
    shareholder.lastname = shareholderForm.controls.lastName.value;
    //shareholder.email = shareholderForm.controls.email.value;
    shareholder.commonnonvotingshares = shareholderForm.controls.numberOfNonVotingShares.value;
    shareholder.commonvotingshares = shareholderForm.controls.numberOfVotingShares.value;
    //shareholder.dateIssued = shareholderForm.controls.dateIssued.value;
    return shareholder;
  }

  addShareholdertoTable(shareholderForm: NgForm) {
    let shareholder: Shareholder;
    shareholder = new Shareholder();
    shareholder.shareholderType = 'Person';
    shareholder.firstname = shareholderForm.controls.firstName.value;
    shareholder.lastname = shareholderForm.controls.lastName.value;
    //shareholder.email = shareholderForm.controls.email.value;
    shareholder.commonnonvotingshares = shareholderForm.controls.numberOfNonVotingShares.value;
    shareholder.commonvotingshares = shareholderForm.controls.numberOfVotingShares.value;
    //shareholder.dateIssued = shareholderForm.controls.dateIssued.value;
    this.shareholderList.push(shareholder);
  }

  getShareholderTest(): Shareholder {
    let shareholder: Shareholder;
    shareholder = new Shareholder();
    shareholder.shareholderType = 'Person';
    shareholder.firstname = 'Test';
    shareholder.lastname = 'Test';
    //shareholder.email = 'Test';
    shareholder.commonnonvotingshares = 0;
    shareholder.commonvotingshares = 0;
    //shareholder.dateIssued = new Date();
    return shareholder;
  }


}
