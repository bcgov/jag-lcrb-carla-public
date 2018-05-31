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
  displayedColumns = ['position', 'name', 'email', 'commonnonvotingshares', 'commonvotingshares', 'dateIssued'];

  constructor(private legalEntityDataservice: AdoxioLegalEntityDataService, public dialog: MatDialog) {
  }

  ngOnInit() {
    let shareholder: Shareholder;
    this.legalEntityDataservice.getShareholders()
      .then((data) => {
        //console.log("getShareholders(): ", data);
        this.dataSource.data = data;
        this.dataLoaded = true;
      });
  }

  formDataToModelData(formData: any, shareholderType: string ): Shareholder {
    let shareholder: Shareholder = new Shareholder();
    if (shareholderType == "Person") {
      shareholder.isindividual = true;
      shareholder.firstname = formData.firstName;
      shareholder.lastname = formData.lastName;
      shareholder.legalentitytype = "PrivateCorporation";
      //shareholder.email = formData.email;
    } else {
      shareholder.isindividual = false;
      shareholder.name = formData.organizationName;
      shareholder.legalentitytype = formData.organizationType;
    }
    shareholder.commonnonvotingshares = formData.numberOfNonVotingShares;
    shareholder.commonvotingshares = formData.numberOfVotingShares;
    ////shareholder.dateIssued = formData.dateIssued;
    shareholder.position = "Shareholder";
    shareholder.relatedentities = [];
    return shareholder;
  }

  // Open Person shareholder dialog
  openPersonDialog() {
    // set dialogConfig settings
    const dialogConfig = new MatDialogConfig();
    dialogConfig.disableClose = false;
    dialogConfig.autoFocus = true;

    // set dialogConfig data
    //dialogConfig.data = {
    //  id: 1,
    //  title: 'Angular For Beginners'
    //};

    // open dialog, get reference and process returned data from dialog
    const dialogRef = this.dialog.open(ShareholderPersonDialog, dialogConfig);
    dialogRef.afterClosed().subscribe(
      formData => {
        //console.log("ShareholderPersonDialog output:", data);
        let shareholderType = "Person";
        let shareholderModel = this.formDataToModelData(formData, shareholderType);
        //console.log("shareholderModel output:", shareholderModel);
        this.legalEntityDataservice.post(shareholderModel);
      }
    );
  }

  // Open Organization shareholder dialog
  openOrganizationDialog() {
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
    const dialogRef = this.dialog.open(ShareholderOrganizationDialog, dialogConfig);
    dialogRef.afterClosed().subscribe(
      formData => {
        //console.log("ShareholderOrganizationDialog output:", data)
        let shareholderType = "Organization";
        let shareholderModel = this.formDataToModelData(formData, shareholderType);
        //console.log("shareholderModel output:", shareholderModel);
        this.legalEntityDataservice.post(shareholderModel);
      }
    );
  }

}

/***************************************
 * Shareholder Person Dialog
 ***************************************/
@Component({
  selector: 'edit-shareholders-person-dialog',
  templateUrl: 'edit-shareholders-person-dialog.html',
})
export class ShareholderPersonDialog {
  shareholderForm: FormGroup;

  constructor(private frmbuilder: FormBuilder, private dialogRef: MatDialogRef<ShareholderPersonDialog>) {
    this.shareholderForm = frmbuilder.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', Validators.email],
      numberOfVotingShares: ['', Validators.required],
      numberOfNonVotingShares: ['', Validators.required],
      dateIssued: ['']
    });
  }

  save() {
    //console.log('shareholderForm', this.shareholderForm.value, this.shareholderForm.valid);
    if (this.shareholderForm.valid) {
      this.dialogRef.close(this.shareholderForm.value);
    } else {
      Object.keys(this.shareholderForm.controls).forEach(field => {
        const control = this.shareholderForm.get(field);
        control.markAsTouched({ onlySelf: true });
      });
    }
  }

  close() {
    this.dialogRef.close();
  }

}

/***************************************
 * Shareholder Organization Dialog
 ***************************************/
@Component({
  selector: 'edit-shareholders-organization-dialog',
  templateUrl: 'edit-shareholders-organization-dialog.html',
})
export class ShareholderOrganizationDialog {
  shareholderForm: FormGroup;

  constructor(private frmbuilder: FormBuilder, private dialogRef: MatDialogRef<ShareholderOrganizationDialog>) {
    this.shareholderForm = frmbuilder.group({
      organizationType: ['', Validators.required],
      organizationName: ['', Validators.required],
      numberOfVotingShares: ['', Validators.required],
      numberOfNonVotingShares: ['', Validators.required],
      dateIssued: ['']
    });
  }

  save() {
    //console.log('shareholderForm', this.shareholderForm.value, this.shareholderForm.valid);
    if (this.shareholderForm.valid) {
      this.dialogRef.close(this.shareholderForm.value);
    } else {
      Object.keys(this.shareholderForm.controls).forEach(field => {
        const control = this.shareholderForm.get(field);
        control.markAsTouched({ onlySelf: true });
      });
    }
  }

  close() {
    this.dialogRef.close();
  }

}

