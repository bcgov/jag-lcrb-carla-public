import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Account } from '@models/account.model';
import { TiedHouseConnection } from '@models/tied-house-connection.model';
import { TiedHouseConnectionsDataService } from '@services/tied-house-connections-data.service';
import { GenericMessageDialogComponent } from '@shared/components/dialog/generic-message-dialog/generic-message-dialog.component';
import { Subject, takeUntil } from 'rxjs';

export type ConnectionToProducersFormData = TiedHouseConnection;

/**
 * Component for managing connections to other cannabis producers.
 * - A singleton tied house connection for connections to cannabis producers
 *
 * @export
 * @class ConnectionToProducersComponent
 * @implements {OnInit}
 * @implements {OnDestroy}
 */
@Component({
  selector: 'app-connection-to-producers',
  templateUrl: './connection-to-producers.component.html',
  styleUrls: ['./connection-to-producers.component.scss']
})
export class ConnectionToProducersComponent implements OnInit, OnDestroy {
  @Input() account: Account;
  @Input() applicationTypeName: String;
  /**
   * Emits the form data on change.
   */
  @Output() onFormChanges = new EventEmitter();

  /**
   * The initial tied house data to populate the tied house declarations component with.
   */
  initialTiedHouseConnection: TiedHouseConnection;

  form: FormGroup;

  get busy(): boolean {
    return !this.hasLoadedData;
  }

  hasLoadedData = false;

  destroy$ = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private tiedHouseService: TiedHouseConnectionsDataService,
    public snackBar: MatSnackBar,
    public matDialog: MatDialog
  ) {}

  ngOnInit() {
    this.initForm();
    this.loadFormData();
  }

  initForm() {
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
      societyConnectionFederalProducerDetails: [''],
      liquorFinancialInterest: [''],
      liquorFinancialInterestDetails: [''],
      iNConnectionToFederalProducer: [''],
      iNConnectionToFederalProducerDetails: ['']
    });

    this.form.valueChanges.pipe(takeUntil(this.destroy$)).subscribe((value) => this.onFormChanges.emit(value));
  }

  loadFormData() {
    const cannabisTiedHouseConnectionForUserRequest$ = this.tiedHouseService.GetCannabisTiedHouseConnectionForUser(
      this.account.id
    );

    cannabisTiedHouseConnectionForUserRequest$.pipe(takeUntil(this.destroy$)).subscribe({
      next: (cannabisTiedHouseDataForUser) => {
        this.initialTiedHouseConnection = cannabisTiedHouseDataForUser;

        if (this.initialTiedHouseConnection) {
          this.form.patchValue(this.initialTiedHouseConnection);
        }

        // Register change handlers to clear the details field when the corresponding checkbox field is unchecked
        this.clearDetailsWhenCheckboxIsFalse('corpConnectionFederalProducer', 'corpConnectionFederalProducerDetails');
        this.clearDetailsWhenCheckboxIsFalse(
          'federalProducerConnectionToCorp',
          'federalProducerConnectionToCorpDetails'
        );
        this.clearDetailsWhenCheckboxIsFalse('share20PlusConnectionProducer', 'share20PlusConnectionProducerDetails');
        this.clearDetailsWhenCheckboxIsFalse(
          'share20PlusFamilyConnectionProducer',
          'share20PlusFamilyConnectionProducerDetail'
        );
        this.clearDetailsWhenCheckboxIsFalse(
          'partnersConnectionFederalProducer',
          'partnersConnectionFederalProducerDetails'
        );
        this.clearDetailsWhenCheckboxIsFalse(
          'societyConnectionFederalProducer',
          'societyConnectionFederalProducerDetails'
        );
        this.clearDetailsWhenCheckboxIsFalse('liquorFinancialInterest', 'liquorFinancialInterestDetails');
        this.clearDetailsWhenCheckboxIsFalse('iNConnectionToFederalProducer', 'iNConnectionToFederalProducerDetails');

        this.hasLoadedData = true;
      },
      error: (error) => {
        console.error('Error loading Cannabis Tied House data', error);
        this.matDialog.open(GenericMessageDialogComponent, {
          data: {
            title: 'Error Loading Tied House Data',
            message:
              'Failed to load Tied House data. Please try again. If the problem persists, please contact support.',
            closeButtonText: 'Close'
          }
        });
      }
    });
  }

  /**
   * Change handler to clear the details field when the corresponding checkbox field is unchecked.
   *
   * @param {string} checkboxFormControlName
   * @param {string} detailsFormControlName
   */
  clearDetailsWhenCheckboxIsFalse(checkboxFormControlName: string, detailsFormControlName: string) {
    this.form
      .get(checkboxFormControlName)
      ?.valueChanges.pipe(takeUntil(this.destroy$))
      .subscribe((value) => {
        if (value === 0) {
          this.form.get(detailsFormControlName)?.setValue('');
        }
      });
  }

  requiresWordingChange(name: String): boolean {
    if (
      name === 'Producer Retail Store' ||
      name == 'PRS Relocation' ||
      name == 'PRS Transfer of Ownership' ||
      name == 'Section 119 Authorization(PRS)' ||
      name == 'CRS Renewal'
    ) {
      return true;
    }

    return false;
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
