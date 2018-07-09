import { Component, OnInit, Input } from '@angular/core';
import { MatSnackBar } from '../../../../../node_modules/@angular/material';
import { FormBuilder } from '../../../../../node_modules/@angular/forms';
import { TiedHouseConnectionsDataService } from '../../../services/ties-house-connections-data.service';

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

  constructor(private fb: FormBuilder, public snackBar: MatSnackBar, private tiedHouseService: TiedHouseConnectionsDataService) { }

  ngOnInit() {
    this.form = this.fb.group({
      CorpConnectionFederalProducer: [''],
      CorpConnectionFederalProducerDetails: [''],
      FederalProducerConnectionToCorp: [''],
      FederalProducerConnectionToCorpDetails: [''],
      Share20PlusConnectionProducer: [''],
      Share20PlusConnectionProducerDetails: [''],
      Share20PlusFamilyConnectionProducer: [''],
      Share20PlusFamilyConnectionProducerDetail: [''],
      PartnersConnectionFederalProducer: [''],
      PartnersConnectionFederalProducerDetails: [''],
      SocietyConnectionFederalProducer: [''],
      SocietyConnectionFederalProducerDetails: ['']
    });
  }

  save() {
    let data = this.form.value;
    this.tiedHouseService.updateTiedHouse(data, "id").subscribe(res => {
      this.snackBar.open('Connections to producers have been saved', "Success", { duration: 3500, extraClasses: ['red-snackbar'] });
    },
      err => {
        this.snackBar.open('Error saving Connections to producers', "Fail", { duration: 3500, extraClasses: ['red-snackbar'] });
        console.log("Error occured");
      });
  }

}
