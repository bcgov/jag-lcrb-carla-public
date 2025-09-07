import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faPrint, faAngleDoubleLeft } from '@fortawesome/free-solid-svg-icons';
import { HttpErrorResponse } from '@angular/common/http';
import { SepPaymentService, SepPayment } from '../../services/sep.payment.service';

@Component({
  selector: 'app-standalone-payment-confirmation',
  templateUrl: './standalone-payment-confirmation.component.html',
  styleUrls: ['./standalone-payment-confirmation.component.scss']
})
export class StandalonePaymentConfirmationComponent implements OnInit {
  faPrint = faPrint;
  faAngleDoubleLeft = faAngleDoubleLeft;

  /** Normal-flow data */
  sepId: string | null = null;
  trnId: string | null = null;
  payment: SepPayment | null = null;
  error: string | null = null;

  /** Duplicate-transaction data (set only when trnApproved === '0') */
  isDuplicate = false;
  messageId: string | null = null;
  messageText: string | null = null;
  trnOrderNumber: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private sepService: SepPaymentService
  ) {}

  ngOnInit(): void {
    const query = this.route.snapshot.queryParamMap;

    /* ---------- 1. Handle duplicate-transaction case ---------- */
    this.isDuplicate = query.get('trnApproved') === '0';
    if (this.isDuplicate) {
      /* Only parse the specific items requested—no extra checks */
      this.trnId         = query.get('trnId');
      this.messageId     = query.get('messageId');
      this.messageText   = query.get('messageText');
      this.trnOrderNumber= query.get('trnOrderNumber');
      return;            // ← Skip every API call when duplicate
    }

    /* ---------- 2. Normal confirmation flow (unchanged) ---------- */
    const segments = this.route.snapshot.url;
    if (segments.length) {
      this.sepId = segments[segments.length - 1].path;
    }

    this.trnId = query.get('trnId');
    if (!this.trnId) {
      this.error = 'An unexpected error occurred. Please try again later.';
      return;
    }

    if (this.sepId) {
      this.fetchPayment(this.sepId, this.trnId);
    } else {
      this.error = 'An unexpected error occurred. Please try again later.';
    }
  }

  private fetchPayment(id: string, txnId: string): void {
    this.sepService.getPaymentById(id, txnId).subscribe({
      next: (data) => {
        this.payment = data;
        this.error = null;
      },
      error: (err: HttpErrorResponse) => {
        if (err.status === 404) {
          this.error = 'The information you provided is invalid or was not found.';
        } else {
          this.error = 'An unexpected error occurred. Please try again later.';
        }
        this.payment = null;
      }
    });
  }

  /* Optional print helper (unchanged) */
  printPage(): void {
    window.print();
  }
}
