import { Component, OnInit, Input, Inject, ChangeDetectorRef } from '@angular/core';
import { MatPaginator, MatTableDataSource, MatSort, MatDialog, MatDialogConfig, MatDialogRef } from '@angular/material';
import { AdoxioLegalEntity } from '../models/adoxio-legalentities.model';
import { DynamicsAccount } from '../models/dynamics-account.model';
import { FormBuilder, FormGroup, FormControl, Validators, NgForm } from '@angular/forms';
import { AdoxioLegalEntityDataService } from "../services/adoxio-legal-entity-data.service";
import { UserDataService } from '../services/user-data.service';
import { User } from '../models/user.model';
import { MatSnackBar } from '@angular/material';
import { Subscription } from 'rxjs';
import { MAT_DIALOG_DATA } from '@angular/material';
import { ActivatedRouteSnapshot, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-edit-shareholders',
  templateUrl: './edit-shareholders.component.html',
  styleUrls: ['./edit-shareholders.component.scss']
})

export class EditShareholdersComponent implements OnInit {
  @Input() accountId: string;
  @Input() parentLegalEntityId: string;
  @Input() businessType: string;

  shareholderForm: FormGroup;
  shareholderList: AdoxioLegalEntity[] = [];
  dataSource = new MatTableDataSource<AdoxioLegalEntity>();
  displayedColumns = ['position', 'name', 'email', 'commonvotingshares', 'edit', 'delete'];
  user: User;
  busy: Promise<any>;
  busyObsv: Subscription;


  constructor(private legalEntityDataservice: AdoxioLegalEntityDataService, private route: ActivatedRoute,
    public dialog: MatDialog, private userDataService: UserDataService, public snackBar: MatSnackBar) {
  }

  ngOnInit() {
    this.getShareholders();
    this.userDataService.getCurrentUser().then(user => {
      this.user = user;
    })
  }

  getShareholders() {
    this.busyObsv = this.legalEntityDataservice.getLegalEntitiesbyPosition(this.parentLegalEntityId, "shareholders")
      .subscribe((response) => {
        let data: AdoxioLegalEntity[]  = response.json();
        data.forEach(d => {
          d.position = this.getPosition(d);
        })
        this.dataSource.data = data;
      });
  }

  getPosition(shareholder: AdoxioLegalEntity): string {
    let position: string  = '';
    if(shareholder.isindividual){
      position = 'Individual';
    } else {
      switch (shareholder.legalentitytype) {
        case 'PrivateCorporation':
          position = 'Private Corporation';
          break;      
        case 'PublicCorporation':
          position = 'Public Corporation';
          break;      
        case 'UnlimitedLiabilityCorporation':
          position = 'Unlimited Liability Corporation';
          break;      
        case 'LimitedLiabilityCorporation':
          position = 'Limited Liability Corporation';
          break;      
        case 'GeneralPartnership':
          position = 'General Partnership';
          break;      
        case 'LimitedPartnership':
          position = 'Limited Partnership';
          break;      
        case 'LimitedLiabilityPartnership':
          position = 'Limited Liability Partnership';
          break;      
        case 'Society':
          position = 'Society';
          break;      
        case 'Coop':
          position = 'Co-op';
          break;      
        case 'Estate':
          position = 'Estate';
          break;      
        case 'Trust':
          position = 'Trust';
          break;      
        default:
          break;
      }
    }
    return position;
  }

  formDataToModelData(formData: any, shareholderType: string): AdoxioLegalEntity {
    let adoxioLegalEntity: AdoxioLegalEntity = new AdoxioLegalEntity();
    if (['GeneralPartnership', 'LimitedPartnership', 'LimitedLiabilityPartnership'].indexOf(this.businessType) !== -1) {
      adoxioLegalEntity.isPartner = true;
      adoxioLegalEntity.isShareholder = false;
    } else {
      adoxioLegalEntity.isShareholder = true;
      adoxioLegalEntity.isPartner = false;
    }
    adoxioLegalEntity.parentLegalEntityId = this.parentLegalEntityId;
    if (shareholderType == "Person") {
      adoxioLegalEntity.isindividual = true;
      adoxioLegalEntity.firstname = formData.firstname;
      adoxioLegalEntity.lastname = formData.lastname;
      adoxioLegalEntity.name = formData.firstname + " " + formData.lastname;
      adoxioLegalEntity.email = formData.email;
    } else {
      adoxioLegalEntity.isindividual = false;
      adoxioLegalEntity.name = formData.name;
      adoxioLegalEntity.legalentitytype = formData.legalentitytype;
    }
    adoxioLegalEntity.commonnonvotingshares = formData.commonnonvotingshares;
    adoxioLegalEntity.commonvotingshares = formData.commonvotingshares;
    // adoxioLegalEntity.dateIssued = formData.dateIssued;
    //adoxioLegalEntity.relatedentities = [];
    // the accountId is received as parameter from the business profile
    if (this.accountId) {
      adoxioLegalEntity.account = new DynamicsAccount();
      adoxioLegalEntity.account.id = this.accountId;
    }
    return adoxioLegalEntity;
  }

  editShareholder(shareholder: AdoxioLegalEntity) {
    if (shareholder.isindividual === true) {
      this.openPersonDialog(shareholder);
    } else {
      this.openOrganizationDialog(shareholder);
    }
  }

  deleteShareholder(shareholder: AdoxioLegalEntity) {
    if (confirm('Delete shareholer?')) {
      this.legalEntityDataservice.deleteLegalEntity(shareholder.id).subscribe(data => {
        this.getShareholders();
      })
    }
  }

  // Open Person shareholder dialog
  openPersonDialog(shareholder: AdoxioLegalEntity) {
    // set dialogConfig settings
    let dialogConfig: any = {
      disableClose: true,
      autoFocus: true,
      data: {
        businessType: this.businessType,
        shareholder: shareholder
      }
    };


    // open dialog, get reference and process returned data from dialog
    const dialogRef = this.dialog.open(ShareholderPersonDialog, dialogConfig);
    dialogRef.afterClosed().subscribe(
      formData => {
        if (formData) {
          let shareholderType = "Person";
          let adoxioLegalEntity = this.formDataToModelData(formData, shareholderType);
          let save = this.legalEntityDataservice.createChildLegalEntity(adoxioLegalEntity);
          if (formData.id) {
            save = this.legalEntityDataservice.updateLegalEntity(formData, formData.id);
          }
          this.busyObsv = save.subscribe(
            res => {
              this.snackBar.open('Shareholder Details have been saved', "Success", { duration: 2500, extraClasses: ['green-snackbar'] });
              this.getShareholders();
            },
            err => {
              this.snackBar.open('Error saving Shareholder Details', "Fail", { duration: 3500, extraClasses: ['red-snackbar'] });
              this.handleError(err);
            }
          );
        }
      }
    );
  }

  // Open Organization shareholder dialog
  openOrganizationDialog(shareholder) {
    // set dialogConfig settings
    const dialogConfig = {
      disableClose: true,
      autoFocus: true,
      data: {
        businessType: this.businessType,
        shareholder: shareholder
      }
    }

    // open dialog, get reference and process returned data from dialog
    const dialogRef = this.dialog.open(ShareholderOrganizationDialog, dialogConfig);
    dialogRef.afterClosed().subscribe(
      formData => {
        //console.log("ShareholderOrganizationDialog output:", data)
        if (formData) {
          let shareholderType = "Organization";
          let adoxioLegalEntity = this.formDataToModelData(formData, shareholderType);
          let save = this.legalEntityDataservice.createChildLegalEntity(adoxioLegalEntity);
          if (formData.id) {
            save = this.legalEntityDataservice.updateLegalEntity(formData, formData.id);
          }
          this.busyObsv = save.subscribe(
            res => {
              this.snackBar.open('Shareholder Details have been saved', "Success", { duration: 2500, extraClasses: ['red-snackbar'] });
              this.getShareholders();
            },
            err => {
              //console.log("Error occured");
              this.snackBar.open('Error saving Shareholder Details', "Fail", { duration: 3500, extraClasses: ['red-snackbar'] });
              this.handleError(err);
            }
          );
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
 * Shareholder Person Dialog
 ***************************************/
@Component({
  selector: 'edit-shareholders-person-dialog',
  templateUrl: 'edit-shareholders-person-dialog.html',
})
export class ShareholderPersonDialog implements OnInit {
  form: FormGroup;

  constructor(private fb: FormBuilder,
    private dialogRef: MatDialogRef<ShareholderPersonDialog>,
    @Inject(MAT_DIALOG_DATA) public data: any) {
    this.form = this.fb.group({
      firstname: ['', Validators.required],
      lastname: ['', Validators.required],
      email: ['', Validators.email],
      commonvotingshares: ['', Validators.required],
      dateIssued: ['']
    });
    if (this.data.shareholder) {
      this.form.patchValue(this.data.shareholder);
    }

  }

  ngOnInit() {
  }

  save() {
    //console.log('shareholderForm', this.shareholderForm.value, this.shareholderForm.valid);
    if (!this.form.valid) {
      Object.keys(this.form.controls).forEach(field => {
        const control = this.form.get(field);
        control.markAsTouched({ onlySelf: true });
      });
    }
    let formData = this.data.shareholder || {};
    formData = (<any>Object).assign(formData, this.form.value);
    this.dialogRef.close(formData);
  }

  isFieldError(field: string) {
    const isError = !this.form.get(field).valid && this.form.get(field).touched;
    return isError;
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
  form: FormGroup;

  constructor(private frmbuilder: FormBuilder, private dialogRef: MatDialogRef<ShareholderOrganizationDialog>, @Inject(MAT_DIALOG_DATA) public data: any) {
    this.form = frmbuilder.group({
      legalentitytype: ['', Validators.required],
      name: ['', Validators.required],
      commonvotingshares: ['', Validators.required],
      commonnonvotingshares: ['', Validators.required],
      dateIssued: ['']
    });
    if (data.shareholder) {
      this.form.patchValue(data.shareholder);
    }
  }

  save() {
    //console.log('shareholderForm', this.shareholderForm.value, this.shareholderForm.valid);
    if (!this.form.valid){
      Object.keys(this.form.controls).forEach(field => {
        const control = this.form.get(field);
        control.markAsTouched({ onlySelf: true });
      });
    }
      let formData = this.data.shareholder || {};
      formData = (<any>Object).assign(formData, this.form.value);
      this.dialogRef.close(formData);
  }



  close() {
    this.dialogRef.close();
  }

}

