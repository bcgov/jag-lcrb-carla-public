import { Component, OnInit, Input } from '@angular/core';
import { MatPaginator, MatTableDataSource, MatSort, MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material';
import { AdoxioLegalEntity } from '../models/adoxio-legalentities.model';
import { FormBuilder, FormGroup, FormControl, Validators, NgForm } from '@angular/forms';
import { AdoxioLegalEntityDataService } from "../services/adoxio-legal-entity-data.service";


@Component({
  selector: 'app-directors-and-officers',
  templateUrl: './directors-and-officers.component.html',
  styleUrls: ['./directors-and-officers.component.scss']
})
export class DirectorsAndOfficersComponent implements OnInit {

  @Input() accountId: string;

  adoxioLegalEntityList: AdoxioLegalEntity[] = [];
  dataSource = new MatTableDataSource<AdoxioLegalEntity>();
  public dataLoaded;
  displayedColumns = ['name', 'email', 'position', 'dateIssued'];

  constructor(private legalEntityDataservice: AdoxioLegalEntityDataService, public dialog: MatDialog) { }

  ngOnInit() {
    this.getDirectorsAndOfficers();
  }

  getDirectorsAndOfficers() {
    this.legalEntityDataservice.getLegalEntitiesbyPosition("directorofficer")
      .then((data) => {
        //console.log("getLegalEntitiesbyPosition("directorofficer"): ", data);
        this.dataSource.data = data;
        this.dataLoaded = true;
      });
  }

  formDataToModelData(formData: any): AdoxioLegalEntity {
    let adoxioLegalEntity: AdoxioLegalEntity = new AdoxioLegalEntity();
    adoxioLegalEntity.position = formData.position;
    adoxioLegalEntity.isindividual = true;
    adoxioLegalEntity.firstname = formData.firstName;
    adoxioLegalEntity.lastname = formData.lastName;
    adoxioLegalEntity.name = formData.firstName + " " + formData.lastName;
    //shareholder.email = formData.email;
    //adoxioLegalEntity.dateIssued = formData.dateIssued;
    adoxioLegalEntity.legalentitytype = "PrivateCorporation";
    // the accountId is received as parameter from the business profile
    //TODO: remove if when accountId is assigned properly
    if (this.accountId) {
      adoxioLegalEntity.account.id = this.accountId;
    }
    //adoxioLegalEntity.relatedentities = [];
    return adoxioLegalEntity;
  }

  // Open Person shareholder dialog
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
    const dialogRef = this.dialog.open(DirectorAndOfficerPersonDialog, dialogConfig);
    dialogRef.afterClosed().subscribe(
      formData => {
        //console.log("DirectorAndOfficerPersonDialog output:", formData);
        if (formData) {
          let adoxioLegalEntity = this.formDataToModelData(formData);
          //console.log("adoxioLegalEntity output:", adoxioLegalEntity);
          this.legalEntityDataservice.post(adoxioLegalEntity);
          this.getDirectorsAndOfficers();
        }
      }
    );
  }

}

/***************************************
 * Director and Officer Person Dialog
 ***************************************/
@Component({
  selector:    'director-and-officer-person-dialog',
  templateUrl: 'director-and-officer-person-dialog.html',
})
export class DirectorAndOfficerPersonDialog {
  directorOfficerForm: FormGroup;

  constructor(private frmbuilder: FormBuilder, private dialogRef: MatDialogRef<DirectorAndOfficerPersonDialog>) {
    this.directorOfficerForm = frmbuilder.group({
      position: ['', Validators.required],
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', Validators.email],
      dateIssued: ['']
    }, { validator: this.dateLessThanToday('dateIssued') });
  }

  dateLessThanToday(field1) {
    return form => {
      const d1 = form.controls[field1].value;
      if (!d1) {
        return {};
      }
      const d1Date = new Date(d1.year, d1.month, d1.day);
      if (d1Date < new Date()) {
        return { dateLessThanToday: true };
      }
      return {};
    }
  }

  save() {
    if (this.directorOfficerForm.valid) {
      this.dialogRef.close(this.directorOfficerForm.value);
    } else {
      Object.keys(this.directorOfficerForm.controls).forEach(field => {
        const control = this.directorOfficerForm.get(field);
        control.markAsTouched({ onlySelf: true });
      });
    }
  }

  close() {
    this.dialogRef.close();
  }

  isFieldError(field: string) {
    const isError = !this.directorOfficerForm.get(field).valid && this.directorOfficerForm.get(field).touched;
    return isError;
  }

}
