import { Component, OnInit, Input } from '@angular/core';
import { MatPaginator, MatTableDataSource, MatSort, MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material';
import { AdoxioLegalEntity } from '../models/adoxio-legalentities.model';
import { DynamicsAccount } from '../models/dynamics-account.model';
import { FormBuilder, FormGroup, FormControl, Validators, NgForm } from '@angular/forms';
import { AdoxioLegalEntityDataService } from "../services/adoxio-legal-entity-data.service";
import { MatSnackBar } from '@angular/material';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-directors-and-officers',
  templateUrl: './directors-and-officers.component.html',
  styleUrls: ['./directors-and-officers.component.scss']
})
export class DirectorsAndOfficersComponent implements OnInit {

  @Input() accountId: string;
  @Input() businessType: string;

  adoxioLegalEntityList: AdoxioLegalEntity[] = [];
  dataSource = new MatTableDataSource<AdoxioLegalEntity>();
  displayedColumns = ['name', 'email', 'position', 'dateofappointment'];
  busy: Promise<any>;
  busyObsv: Subscription;

  constructor(private legalEntityDataservice: AdoxioLegalEntityDataService, public dialog: MatDialog,
              public snackBar: MatSnackBar) { }

  ngOnInit() {
    this.getDirectorsAndOfficers();
  }

  getDirectorsAndOfficers() {
    this.busy = this.legalEntityDataservice.getLegalEntitiesbyPosition("director-officer")
      .then((data) => {
        //console.log("getLegalEntitiesbyPosition('director-officer'): ", data);
        //console.log("parameter: accountId = ", this.accountId)
        this.dataSource.data = data;
      });
  }

  formDataToModelData(formData: any): AdoxioLegalEntity {
    let adoxioLegalEntity: AdoxioLegalEntity = new AdoxioLegalEntity();
    adoxioLegalEntity.position = formData.position;
    adoxioLegalEntity.isindividual = true;
    adoxioLegalEntity.firstname = formData.firstName;
    adoxioLegalEntity.lastname = formData.lastName;
    adoxioLegalEntity.name = formData.firstName + " " + formData.lastName;
    adoxioLegalEntity.email = formData.email;
    adoxioLegalEntity.dateofappointment = formData.dateOfAppointment; //adoxio_dateofappointment
    adoxioLegalEntity.legalentitytype = "PrivateCorporation";
    // the accountId is received as parameter from the business profile
    if (this.accountId) {
      adoxioLegalEntity.account = new DynamicsAccount();
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

    // open dialog, get reference and process returned data from dialog
    const dialogRef = this.dialog.open(DirectorAndOfficerPersonDialog, dialogConfig);
    dialogRef.afterClosed().subscribe(
      formData => {
        if (formData) {
          let adoxioLegalEntity = this.formDataToModelData(formData);
          this.busyObsv = this.legalEntityDataservice.createLegalEntity(adoxioLegalEntity).subscribe(
            res => {
              this.snackBar.open('Director / Officer Details have been saved', "Success", { duration: 2500, extraClasses: ['red-snackbar'] });
              this.getDirectorsAndOfficers();
            },
            err => {
              this.snackBar.open('Error saving Director / Officer Details', "Fail", { duration: 3500, extraClasses: ['red-snackbar'] });
              this.handleError(err);
            });
        }
      }
    );
    
  }

  private handleError(error: Response | any) {
    let errMsg: string;
    if (error instanceof Response) {
      const body = error.json() || "";
      const err = body || JSON.stringify(body);
      errMsg = `${error.status} - ${error.statusText || ""} ${err}`;
    } else {
      errMsg = error.message ? error.message : error.toString();
    }
    console.error(errMsg);
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
      dateOfAppointment: ['', Validators.required]
    }, { validator: this.dateLessThanToday('dateOfAppointment') }
    );
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
