import { Component, OnInit, Input, Inject } from '@angular/core';
import { MatPaginator, MatTableDataSource, MatSort, MatDialog, MatDialogConfig, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';

import { FormBuilder, FormGroup, FormControl, Validators, NgForm } from '@angular/forms';
import { MatSnackBar } from '@angular/material';
import { Subscription } from 'rxjs';
import { LegalEntityDataService } from '../../../services/legal-entity-data.service';
import { DynamicsAccount } from '../../../models/dynamics-account.model';
import { DynamicsDataService } from '../../../services/dynamics-data.service';
import { ActivatedRoute } from '@angular/router';
import { Store } from '@ngrx/store';
import { AppState } from '../../../app-state/models/app-state';
import { LegalEntity } from '@appmodels/legal-entity.model';

@Component({
  selector: 'app-directors-and-officers',
  templateUrl: './directors-and-officers.component.html',
  styleUrls: ['./directors-and-officers.component.scss']
})
export class DirectorsAndOfficersComponent implements OnInit {

  @Input() accountId: string;
  @Input() parentLegalEntityId: string;
  @Input() businessType: string;

  adoxioLegalEntityList: LegalEntity[] = [];
  dataSource = new MatTableDataSource<LegalEntity>();
  displayedColumns = ['name', 'email', 'position', 'dateofappointment', 'edit', 'delete'];
  busy: Promise<any>;
  busyObsv: Subscription;
  subscriptions: Subscription[] = [];

  constructor(private legalEntityDataservice: LegalEntityDataService,
    public dialog: MatDialog,
    private store: Store<AppState>,
    private dynamicsDataService: DynamicsDataService,
    private route: ActivatedRoute,
    public snackBar: MatSnackBar) { }

  ngOnInit() {

    if (this.businessType === 'SoleProprietor') {
      this.displayedColumns = ['name', 'email', 'position', 'edit', 'delete'];
    }
    this.getDirectorsAndOfficers();
    // const sub = this.store.select(state => state.currentAccountState)
    //   .filter(state => !!state)
    //   .subscribe(state => {
    //     this.accountId = state.currentAccount.id;
    //     this.businessType = state.currentAccount.businessType;
    //     if (this.businessType === 'SoleProprietor') {
    //       this.displayedColumns = ['name', 'email', 'position', 'edit', 'delete'];
    //     }
    //     const sub2 = this.route.parent.params.subscribe(p => {
    //       this.parentLegalEntityId = p.legalEntityId;
    //       this.getDirectorsAndOfficers();
    //     });
    //     this.subscriptions.push(sub2);
    //   });
    // this.subscriptions.push(sub);
  }

  getDirectorsAndOfficers() {
    this.busyObsv = this.legalEntityDataservice.getLegalEntitiesbyPosition(this.parentLegalEntityId, 'directors-officers-management')
      .subscribe((data) => {
        data.forEach(d => {
          const positionList: string[] = [];
          if (d.isDirector) {
            positionList.push('Director');
          }
          if (d.isOfficer) {
            positionList.push('Officer');
          }
          if (d.isOwner) {
            positionList.push('Owner');
          }
          if (d.isSeniorManagement) {
            positionList.push('Senior Manager');
          }
          d.position = positionList.join(', ');
        });
        this.dataSource.data = data;
      });
  }

  formDataToModelData(formData: any): LegalEntity {
    const adoxioLegalEntity: LegalEntity = new LegalEntity();
    adoxioLegalEntity.isShareholder = false;
    adoxioLegalEntity.parentLegalEntityId = this.parentLegalEntityId;
    adoxioLegalEntity.id = formData.id;
    adoxioLegalEntity.isindividual = true;
    adoxioLegalEntity.firstname = formData.firstname;
    adoxioLegalEntity.lastname = formData.lastname;
    adoxioLegalEntity.name = formData.firstname + ' ' + formData.lastname;
    adoxioLegalEntity.email = formData.email;
    adoxioLegalEntity.dateofappointment = formData.dateofappointment; // adoxio_dateofappointment
    // adoxioLegalEntity.legalentitytype = "PrivateCorporation";
    if (this.businessType === 'SoleProprietor') {
      adoxioLegalEntity.isOwner = true;
    } else {
      adoxioLegalEntity.isOfficer = formData.isOfficer;
      adoxioLegalEntity.isDirector = formData.isDirector;
      adoxioLegalEntity.isSeniorManagement = formData.isSeniorManagement;
    }
    // the accountId is received as parameter from the business profile
    if (this.accountId) {
      adoxioLegalEntity.account = <DynamicsAccount>{};
      adoxioLegalEntity.account.id = this.accountId;
    }
    // adoxioLegalEntity.relatedentities = [];
    return adoxioLegalEntity;
  }

  // Open Person shareholder dialog
  openPersonDialog(person: LegalEntity) {
    // set dialogConfig settings
    const dialogConfig = {
      disableClose: true,
      autoFocus: true,
      width: '500px',
      data: {
        person: person,
        businessType: this.businessType
      }
    };

    // open dialog, get reference and process returned data from dialog
    const dialogRef = this.dialog.open(DirectorAndOfficerPersonDialogComponent, dialogConfig);
    dialogRef.afterClosed().subscribe(
      formData => {
        if (formData) {
          const adoxioLegalEntity = this.formDataToModelData(formData);
          let save = this.legalEntityDataservice.createChildLegalEntity(adoxioLegalEntity);
          if (formData.id) {
            save = this.legalEntityDataservice.updateLegalEntity(adoxioLegalEntity, formData.id);
          }
          this.busyObsv = save.subscribe(
            res => {
              this.snackBar.open('Director / Officer Details have been saved', 'Success',
                { duration: 2500, panelClass: ['red-snackbar'] });
              this.getDirectorsAndOfficers();
            },
            err => {
              this.snackBar.open('Error saving Director / Officer Details', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
              this.handleError(err);
            });
        }
      }
    );

  }

  deleteIndividual(person: LegalEntity) {
    if (confirm('Delete person?')) {
      this.legalEntityDataservice.deleteLegalEntity(person.id).subscribe(data => {
        this.getDirectorsAndOfficers();
      });
    }
  }

  private handleError(error: Response | any) {
    let errMsg: string;
    if (error instanceof Response) {
      const body = error || '';
      const err = body || JSON.stringify(body);
      errMsg = `${error.status} - ${error.statusText || ''} ${err}`;
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
  selector: 'app-director-and-officer-person-dialog',
  templateUrl: 'director-and-officer-person-dialog.html',
})
export class DirectorAndOfficerPersonDialogComponent {
  directorOfficerForm: FormGroup;
  businessType: string;

  constructor(private fb: FormBuilder,
    private dialogRef: MatDialogRef<DirectorAndOfficerPersonDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    this.directorOfficerForm = fb.group({
      isDirector: [false],
      isOfficer: [false],
      isSeniorManagement: [false],
      firstname: ['', Validators.required],
      lastname: ['', Validators.required],
      email: ['', Validators.email],
      dateofappointment: ['', Validators.required]
    }, { validator: this.dateLessThanToday('dateofappointment') }
    );

    if (data && data.person) {
      this.directorOfficerForm.patchValue(data.person);
    }
    this.businessType = data.businessType;
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
    };
  }

  save() {
    let formData = this.data.person || {};
    formData = (<any>Object).assign(formData, this.directorOfficerForm.value);
    this.dialogRef.close(formData);

    if (!this.directorOfficerForm.valid) {
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
