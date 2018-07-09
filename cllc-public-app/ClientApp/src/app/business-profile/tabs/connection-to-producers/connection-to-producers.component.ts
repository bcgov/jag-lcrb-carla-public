import { Component, OnInit, Input } from '@angular/core';
import { MatSnackBar } from '../../../../../node_modules/@angular/material';
import { FormBuilder } from '../../../../../node_modules/@angular/forms';

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
    
  constructor(private fb: FormBuilder, public snackBar: MatSnackBar) { }

  ngOnInit() {
    this.form = this.fb.group({
      CorpConnectionFederalProducer : [''],
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
  
}
