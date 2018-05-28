import { Component, OnInit } from '@angular/core';
import { MatPaginator, MatTableDataSource, MatSort } from '@angular/material';
import { Shareholder } from '../models/shareholder.model';
import { FormBuilder, FormGroup, FormControl, Validators, NgForm } from '@angular/forms' 

@Component({
    selector: 'app-edit-shareholders',
    templateUrl: './edit-shareholders.component.html',
    styleUrls: ['./edit-shareholders.component.scss']
})

export class EditShareholdersComponent implements OnInit {

  shareholderForm: FormGroup;
  dataSource = new MatTableDataSource<Shareholder>();
  public dataLoaded;
  displayedColumns = ['shareholderType', 'name', 'email', 'numberOfNonVotingShares', 'numberOfVotingShares', 'dateIssued'];

  constructor(private frmbuilder: FormBuilder) {
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
    
    let shareholderList: Shareholder[] = [];
    let shareholder: Shareholder;
    shareholder = this.getTestShareholder();

    shareholderList.push(shareholder);

    this.dataSource.data = shareholderList;
    this.dataLoaded = true;
  }

  saveShareholder(shareholderForm: NgForm) {
    console.log(shareholderForm.controls);
  }

  getTestShareholder(): Shareholder {
    let shareholder: Shareholder;
    shareholder = new Shareholder();
    shareholder.shareholderType = 'Test';
    shareholder.firstName = 'Test';
    shareholder.lastName = 'Test';
    shareholder.email = 'Test';
    shareholder.numberOfNonVotingShares = 0;
    shareholder.numberOfVotingShares = 0;
    shareholder.dateIssued = new Date();
    return shareholder;
  }

}
