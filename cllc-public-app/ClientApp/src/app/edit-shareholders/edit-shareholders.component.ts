import { Component, OnInit } from '@angular/core';
import { MatPaginator, MatTableDataSource, MatSort, MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material';
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

  constructor(private frmbuilder: FormBuilder, private legalEntityDataservice: AdoxioLegalEntityDataService,
              public dialog: MatDialog)
  {
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
    //shareholder = this.getShareholderTest();
    //this.shareholderList.push(shareholder);

    this.dataSource.data = this.shareholderList;
    this.dataLoaded = true;
  }

  //addShareholderPerson(shareholderForm: NgForm) {
  //  console.log(shareholderForm.controls);
  //  let shareholderModel = this.toShareholderModel(shareholderForm, "Person");
  //  console.log(shareholderModel);
  //  this.legalEntityDataservice.post(shareholderModel);
  //  this.addShareholdertoTable(shareholderForm);
  //  this.dataSource.data = this.shareholderList;
  //}

  toShareholderModel(formData: any, shareholderType: string ): Shareholder {
    let shareholder: Shareholder = new Shareholder();
    shareholder.id = this.guid();
    if (shareholder.shareholderType = 'Person') {
      shareholder.isindividual = true;
    } else {
      shareholder.shareholderType = 'Organization'
      shareholder.isindividual = false;
    }
    shareholder.firstname = formData.firstName;
    shareholder.lastname = formData.lastName;
    //shareholder.email = formData.email;
    shareholder.commonnonvotingshares = formData.numberOfNonVotingShares;
    shareholder.commonvotingshares = formData.numberOfVotingShares;
    //shareholder.dateIssued = formData.dateIssued;
    shareholder.legalentitytype = "845280000"; //845280000 = PrivateCorporation
    shareholder.position = "1"; //1 = Shareholder
    return shareholder;
  }

  addShareholdertoTable(formData: any, shareholderType: string) {
    let shareholder: Shareholder = this.toShareholderModel(formData, shareholderType);
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

  guid() {
    return this.s4() + this.s4() + '-' + this.s4() + '-' + this.s4() + '-' +
      this.s4() + '-' + this.s4() + this.s4() + this.s4();
  }

  s4() {
  return Math.floor((1 + Math.random()) * 0x10000)
    .toString(16).substring(1);
  }

  // Person shareholder dialog
  openPersonDialog() {
    // set dialogConfig settings
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;

    // set dialogConfig data
    //dialogConfig.data = {
    //  id: 1,
    //  title: 'Angular For Beginners'
    //};

    // open dialog, get reference and process returned data from dialog
    this.dialog.open(ShareholderPersonDialog, dialogConfig);
    const dialogRef = this.dialog.open(ShareholderPersonDialog, dialogConfig);
    dialogRef.afterClosed().subscribe(
      data => {
        console.log("ShareholderPersonDialog output:", data)
        let shareholderType: string = "Person";
        let shareholderModel = this.toShareholderModel(data, shareholderType);
        console.log("shareholderModel output:", shareholderModel);
        this.legalEntityDataservice.post(shareholderModel);
        this.addShareholdertoTable(data, shareholderType);
        this.dataSource.data = this.shareholderList;
      }
    );
  }

  // Organization shareholder dialog
  openOrganizationDialog() {
    //const dialogRef = this.dialog.open(ShareholderOrganizationDialog, {
    //  height: '350px'
    //});

    //dialogRef.afterClosed().subscribe(result => {
    //  console.log(`Dialog result: ${result}`);
    //});

    const dialogConfig = new MatDialogConfig();

    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;

    dialogConfig.data = {
      id: 1,
      title: 'Angular For Beginners'
    };

    this.dialog.open(ShareholderOrganizationDialog, dialogConfig);
  }

}

@Component({
  selector: 'edit-shareholders-person-dialog',
  templateUrl: 'edit-shareholders-person-dialog.html',
})
export class ShareholderPersonDialog {
  shareholderForm: FormGroup;

  constructor(private frmbuilder: FormBuilder, private legalEntityDataservice: AdoxioLegalEntityDataService,
    private dialogRef: MatDialogRef<ShareholderPersonDialog>) {
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

  save() {
    console.log(this.shareholderForm.value);
    this.dialogRef.close(this.shareholderForm.value);
    
  }

  close() {
    this.dialogRef.close();
  }

}

@Component({
  selector: 'edit-shareholders-organization-dialog',
  templateUrl: 'edit-shareholders-organization-dialog.html',
})
export class ShareholderOrganizationDialog { }
