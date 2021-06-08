import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { PoliceTableElement } from '@components/police-representative/police-table-element';
import { SepApplicationSummary } from '@models/sep-application-summary.model';
import { PaymentDataService } from '@services/payment-data.service';
import { SpecialEventsDataService } from '@services/special-events-data.service';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-submitted-applications',
  templateUrl: './submitted-applications.component.html',
  styleUrls: ['./submitted-applications.component.scss']
})
export class SubmittedApplicationsComponent implements OnInit {
  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  dataSource = new MatTableDataSource<PoliceTableElement>();
  // angular material table columns to display
  columnsToDisplay = [
    'dateSubmitted', 'eventName', 'eventStartDate',
    'eventStatusLabel', 'maximumNumberOfGuests', 'actions'
  ];

  constructor(private sepDataService: SpecialEventsDataService,
    private snackBar: MatSnackBar,
    private paymentDataService: PaymentDataService) { }

  ngOnInit(): void {
    this.sepDataService.getSubmittedApplications()
      .subscribe(data => this.dataSource.data = data);
  }

  payNow(applicationId: string) {
    // and payment is required due to an invoice being generated
    if (applicationId) {
      // proceed to payment
      this.submitPayment(applicationId)
        .subscribe(res => {
        },
          error => {
            this.snackBar.open('Error submitting payment', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
          }
        );
    }
  }

  /**
 * Redirect to payment processing page (Express Pay / Bambora service)
 * */
  private submitPayment(applicationId: string) {
    return this.paymentDataService.getPaymentURI('specialEventInvoice', applicationId)
      .pipe(map(jsonUrl => {
        window.location.href = jsonUrl['url'];
        return jsonUrl['url'];
      }, (err: any) => {
        if (err._body === 'Payment already made') {
          this.snackBar.open('Application payment has already been made.', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
        }
      }));
  }

}
