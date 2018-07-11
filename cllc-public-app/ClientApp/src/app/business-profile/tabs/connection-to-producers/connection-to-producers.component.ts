import { Component, OnInit, Input } from '@angular/core';
import { MatSnackBar } from '@angular/material';
import { FormBuilder } from '@angular/forms';
import { TiedHouseConnectionsDataService } from '../../../services/tied-house-connections-data.service';
import { TiedHouseConnection } from '../../../models/tied-house-connection.model';
import { auditTime } from 'rxjs/operators';
import { ActivatedRoute } from '../../../../../node_modules/@angular/router';
import { DynamicsDataService } from '../../../services/dynamics-data.service';
import { Observable, Subject } from '../../../../../node_modules/rxjs';

@Component({
  selector: 'app-connection-to-producers',
  templateUrl: './connection-to-producers.component.html',
  styleUrls: ['./connection-to-producers.component.css']
})
export class ConnectionToProducersComponent implements OnInit {
  @Input() accountId: string;
  @Input() businessType: string;

  operatingForMoreThanOneYear: any;
  form: any;
  _tiedHouseData: TiedHouseConnection;

  constructor(private fb: FormBuilder,
    public snackBar: MatSnackBar,
    private tiedHouseService: TiedHouseConnectionsDataService,
    private dynamicsDataService: DynamicsDataService,
    private route: ActivatedRoute) { }

  ngOnInit() {
    this.form = this.fb.group({
      corpConnectionFederalProducer: [''],
      corpConnectionFederalProducerDetails: [''],
      federalProducerConnectionToCorp: [''],
      federalProducerConnectionToCorpDetails: [''],
      share20PlusConnectionProducer: [''],
      share20PlusConnectionProducerDetails: [''],
      share20PlusFamilyConnectionProducer: [''],
      share20PlusFamilyConnectionProducerDetail: [''],
      partnersConnectionFederalProducer: [''],
      partnersConnectionFederalProducerDetails: [''],
      societyConnectionFederalProducer: [''],
      societyConnectionFederalProducerDetails: ['']
    });

    this.form.valueChanges
      .pipe(auditTime(2000)).subscribe(formData => {
        this.save();
      });

    this.route.parent.params.subscribe(p => {
      this.accountId = p.accountId;
      this.dynamicsDataService.getRecord('account', this.accountId)
        .then((data) => {
          this.businessType = data.businessType;
        });

      this.tiedHouseService.getTiedHouse(this.accountId)
        .subscribe(res => {
          this._tiedHouseData = res.json();
          this.form.patchValue(this._tiedHouseData);
        });
    });
  }

  canDeactivate(): Observable<boolean> | boolean {
    return this.save();
  }

  save(): Subject<boolean> {
    const data = (<any>Object).assign(this._tiedHouseData, this.form.value);
    const saveObservable = new Subject<boolean>();
    this.tiedHouseService.updateTiedHouse(data, data.id).subscribe(res => {
      // this.snackBar.open('Connections to producers have been saved', 'Success', { duration: 3500, extraClasses: ['red-snackbar'] });
      saveObservable.next(true);
    },
      err => {
        this.snackBar.open('Error saving Connections to producers', 'Fail', { duration: 3500, extraClasses: ['red-snackbar'] });
        saveObservable.next(false);
        console.log('Error occured');
      });
    return saveObservable;
  }

}
