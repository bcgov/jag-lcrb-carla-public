import { Component, OnInit, Input } from '@angular/core';
import { MatSnackBar } from '../../../../../node_modules/@angular/material';
import { FormBuilder } from '../../../../../node_modules/@angular/forms';
import { TiedHouseConnectionsDataService } from '../../../services/tied-house-connections-data.service';
import { debug } from 'util';
import { TiedHouseConnection } from '../../../models/tied-house-connection.model';

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

  constructor(private fb: FormBuilder, public snackBar: MatSnackBar, private tiedHouseService: TiedHouseConnectionsDataService) { }

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

    this.tiedHouseService.getTiedHouse(this.accountId)
      .subscribe(res => {
        this._tiedHouseData = res.json();
        this.form.patchValue(this._tiedHouseData);
      });
  }

  save() {
    const data = (<any>Object).assign(this._tiedHouseData, this.form.value);
    this.tiedHouseService.updateTiedHouse(data, data.id).subscribe(res => {
      this.snackBar.open('Connections to producers have been saved', 'Success', { duration: 3500, extraClasses: ['red-snackbar'] });
    },
      err => {
        this.snackBar.open('Error saving Connections to producers', 'Fail', { duration: 3500, extraClasses: ['red-snackbar'] });
        console.log('Error occured');
      });
  }

}
